using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using static RecipeSO;

public abstract class AbstractRecipeBuilder : AbstractKitchenObjectParent {
	[SerializeField] Vector3 uiRelativePosition = new Vector3(0, 2.5f, 0);

	Dictionary<string, RecipeObject> recipeDictionary = new Dictionary<string, RecipeObject>();
	protected RecipeSO recipe;
	GameObject completeVisual;

	public List<KitchenObject> kitchenObjectsOnThis = new List<KitchenObject>();
	protected List<RecipeSO> possibleRecipes = new List<RecipeSO>();

	public override void Start() {
		base.Start();
		// must set itself into the recipe if it's not a plate
		AddSelf();
	}

	protected virtual void AddSelf() {
		AddKitchenObject(GetComponent<KitchenObject>(), false);
		GetGameObjectVisual()[0].gameObject.SetActive(false);
	}
	public bool CanAddAllIngrediants(AbstractRecipeBuilder recipeBuilder) {
		foreach(KitchenObject ko in GetMyKitchenObjects())
			if(!recipeBuilder.CanAddKitchenObject(ko)) {
				//Debug.Log("Can't add: " + ko + " to " + recipeBuilder);
				return false;
			}
		return true;
	}


	public bool CanAddKitchenObject(KitchenObject kitchenObject) {
		// no ko's on this plate 
		if(possibleRecipes == null || possibleRecipes.Count == 0) {
			// add all recipes to possible recipes
			possibleRecipes = DeliveryManager.GetRecipes().GetRange(0, DeliveryManager.GetRecipes().Count);
			// remove all recipes without first ingrediant
			foreach(RecipeSO recipeSO in possibleRecipes.GetRange(0, possibleRecipes.Count))
				if(!recipeSO.HasKitchenObject(kitchenObject))
					possibleRecipes.Remove(recipeSO);
		}
		// check if ingrediant matches any recipes
		string name = kitchenObject.gameObject.name.Replace("(Clone)", "");
		bool hasRecipe = false;
		foreach(RecipeSO recipeSO in possibleRecipes)
			if(recipeSO.HasKitchenObject(kitchenObject, recipeDictionary.ContainsKey(name) ? recipeDictionary[name].count + 1 : 1)) {
				hasRecipe = true;
				break;
			} else
				Debug.Log("Recipe does not have ko or quantity: "+this+ ", "+ recipeSO + ", " + name + ", " + recipeDictionary.ContainsKey(name) + ", " + (recipeDictionary.ContainsKey(name) ? recipeDictionary[name].count+1 : ""));
		//Debug.Log("Has recipe? " + hasRecipe);
		// if not don't add the incredinat
		//Debug.Log("Trying to add ko results: " + hasRecipe + ", " +possibleRecipes.Count);
		if(!hasRecipe)
			return false;
		return true;
	}

	public bool AddKitchenObject(KitchenObject kitchenObject, bool shouldRemove = true) {
		string name = kitchenObject.gameObject.name.Replace("(Clone)", "");
		if(!CanAddKitchenObject(kitchenObject))
			return false;
		// remove any recipes that doesn't match the added item
		foreach(RecipeSO recipeSO in possibleRecipes.GetRange(0, possibleRecipes.Count))
			if(!recipeSO.HasKitchenObject(kitchenObject, recipeDictionary.ContainsKey(name) ? recipeDictionary[name].count + 1 : 1))
				possibleRecipes.Remove(recipeSO);
		// set recipe to be first item in possible recipes, if it's different than the set visual then change it
		possibleRecipes.Sort((a,b)=>a.GetIngrediantCount().CompareTo(b.GetIngrediantCount()));
		if(possibleRecipes.Count > 0 && possibleRecipes[0] != recipe) {
			if(completeVisual)
				DestroyImmediate(completeVisual);
			completeVisual = null;
			recipe = possibleRecipes[0];
		}
		// add the items to the dictionary
		if(!recipeDictionary.ContainsKey(name))
			recipeDictionary.Add(name, new RecipeObject(kitchenObject.gameObject, 0));
		recipeDictionary[name].count++;
		if(shouldRemove)
			kitchenObject.SetKitchenObjectParent(this);
		// make item visible
		if(recipe) {
			if(!completeVisual) {
				completeVisual = Instantiate(recipe.GetVisualCompletedRecipe());
				completeVisual.transform.SetParent(GetKitchenObjectFollowTransform());
				completeVisual.transform.localPosition = Vector3.zero;
			}
			Dictionary<string, int> ingrediantCount = new Dictionary<string, int>();
			foreach(Transform transform in completeVisual.GetComponentInChildren<Transform>()) {
				string nameL = transform.gameObject.name.Replace("(Clone)", "").Replace("_Visual","");
				Debug.Log("Setting active or not: " + this + ", " + nameL + ", " + ingrediantCount.ContainsKey(nameL) + ", " + recipeDictionary.ContainsKey(nameL));
				if(ingrediantCount.ContainsKey(nameL)) { // this is for duplicate ingrediants
					ingrediantCount[nameL]++;
					if(ingrediantCount[nameL] > recipeDictionary[nameL].count)
						transform.gameObject.SetActive(false);
					else
						transform.gameObject.SetActive(true);
				} else if(recipeDictionary.ContainsKey(nameL)) { // this is if the recipe has this ingrediant
					ingrediantCount[nameL] = 1;
					transform.gameObject.SetActive(true);
				} else // this is if recipe doesn't have ingrediant
					transform.gameObject.SetActive(false);
			}
		}
		// add kitchen object to plate
		kitchenObject.isOnPlate = true;
		kitchenObjectsOnThis.Add(kitchenObject);
		if(shouldRemove) {
			kitchenObject.gameObject.SetActive(false);
			SetKitchenObject(null);
		}
		if(TryGetComponent(out IngrediantIcon ingrediant) && ingrediant.shouldShowChild) 
			ingrediant.GetFoodIconsUI().SetSprites(kitchenObject.GetKitchenObjectSO().sprite);
		return true;
	}

	public override bool InteractPickUp(ISelectable interacted) {
		// bread can't interact with bread and only way bread can interact with plate is if plate is empty
		// plate and plate can interact but can't transfer ingrediants if both plates have a bread
		// if the interacting is a ko try to add it to the plate
		// recipe builders can't pick things off the floor and if they add a ko then they get put down where the ko is
		KitchenObject ko = GetISelectableKitchenObject(interacted);
		Debug.Log("Interacted with base recipe: " + this + ", " + interacted+", "+ko);
		// only time no parent for ko is if it's on the floor
		if(ko && !ko.GetComponent<AbstractRecipeBuilder>() && ko.GetKitchenObjectParent()) {
			if(ko.GetComponent<FryingPan>() || ko.GetKitchenObjectParent() is FryingPan) { // only plate can take out of frying pan
				Debug.Log("Only plate can take from frying pan");
				return false;
			}
			// player can't add ingrediants if it's holding recipe builder, it must put it down onto the ingrediant
			if(ko.GetKitchenObjectParent() is not Player) {
				// i'm assuming all recipe builders are ko
				AbstractKitchenObjectParent koParent = ko.GetKitchenObjectParent();
				bool r = AddKitchenObject(ko);
				if(r)
					GetComponent<KitchenObject>().SetKitchenObjectParent(koParent);
				return r;
			} else
				return AddKitchenObject(ko);
		}
		// otherwise just do normal ko things which is to not be a counter
		return false;
	}
	protected KitchenObject GetISelectableKitchenObject(ISelectable interacted) {
		return interacted is KitchenObject ? (KitchenObject) interacted : interacted is AbstractKitchenObjectParent && ((AbstractKitchenObjectParent) interacted).HasKitchenObject() ? ((AbstractKitchenObjectParent) interacted).GetKitchenObject() : null;
	}

	public bool GiveIngrediantsTo(AbstractRecipeBuilder recipeBuilder) {
		Debug.Log("Trying to give ingrediants: " + this + ", " + recipeBuilder);
		// This is all or nothing operation
		if(!recipeBuilder.CanAddAllIngrediants(this))
			return false;
		foreach(KitchenObject ko in GetMyKitchenObjects())
			recipeBuilder.AddKitchenObject(ko);
		if(this is not Plate && GetMyKitchenObjects().Count == 0)
			Trash();
		else
			Clear();
		return true;
	}


	public List<KitchenObject> GetMyKitchenObjects() {
		return kitchenObjectsOnThis;
	}

	public void SetMyKitchenObjects(List<KitchenObject> kitchenObjects) {
		kitchenObjectsOnThis = kitchenObjects;
	}

	private Dictionary<string, RecipeObject> GetRecipeDictionary() {
		return recipeDictionary;
	}

	private List<RecipeSO> GetPossibleRecipes() {
		return possibleRecipes;
	}

	private void SetRecipeDictionary(Dictionary<string, RecipeObject> recipeDictionary) {
		this.recipeDictionary = recipeDictionary;
	}

	private void SetPossibleRecipes(List<RecipeSO> possibleRecipes) {
		this.possibleRecipes = possibleRecipes;
	}

	internal void Clear() {
		if(TryGetComponent(out IngrediantIcon ingrediant))
			ingrediant.GetFoodIconsUI().ClearSprites();
		kitchenObjectsOnThis.Clear();
		possibleRecipes = null;
		recipeDictionary = new Dictionary<string, RecipeObject>();
		if(completeVisual)
			DestroyImmediate(completeVisual);
		completeVisual = null;
		recipe = null;
	}
}
