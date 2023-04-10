using UnityEngine;

public class Plate : AbstractRecipeBuilder {
	public override bool InteractPickUp(ISelectable interacted) {
		// plates are a bit different from other recipe objects
		KitchenObject ko = GetISelectableKitchenObject(interacted);
		Debug.Log("Interacted with a plate: " + this + ", " + interacted + ", " + ko);
		if(ko) {
			if(ko.TryGetComponent(out Plate plate)) {
				if(TryGetComponent(out Plate myPlate) && BuilderToBuilderTransfer(plate)) {
					// plate to plate transfer
					return true;
				} else {
					// plate to builder transfer
					// in this case if parent give the plate and ingrediants to the parent of builder otherwise give builder's ingrediants to plate
					GiveIngrediantsTo(plate);
				}
			} else if(ko.TryGetComponent(out AbstractRecipeBuilder recipeBuilder)) {
				return BuilderToBuilderTransfer(recipeBuilder);
			} else {
				AbstractKitchenObjectParent p = ko.GetKitchenObjectParent();
				if(!p)
					return false;
				bool b = AddKitchenObject(ko);
				if(!b)
					return false;
				if(p is not Player && !p.GetComponent<KitchenObject>())
					GetComponent<KitchenObject>().SetKitchenObjectParent(p);
				return true;
			}
		}
		return base.InteractPickUp(interacted);
	}

	public override bool Trash() {
		Debug.Log("Trashing plate");
		bool hasKO = kitchenObjectsOnThis.Count > 0;
		foreach(KitchenObject ko in kitchenObjectsOnThis) {
			Debug.Log("Destroying ko: " + ko);
			ko.DestroySelf();
		}
		Clear();
		return hasKO;
	}

	protected bool BuilderToBuilderTransfer(AbstractRecipeBuilder recipeBuilder) {
		AbstractKitchenObjectParent parent = null;
		Debug.Log("Trying builder to builder transfer: " + this + ", " + recipeBuilder);
		if(recipeBuilder.TryGetParent(out AbstractKitchenObjectParent p))
			parent = p;
		else return false;
		Debug.Log("Trying builder to builder transfer, got parent: " + this + ", " + recipeBuilder+", "+parent+", "+recipeBuilder.GetMyKitchenObjects().Count+", "+GetMyKitchenObjects().Count);
		// if recipeBuilder is not a plate and i can give all my ingrediants to recipeBuilder then give it the plate too
		// if plate is empty give ingrediants from not empty to empty
		if(recipeBuilder is Plate) {
			if(recipeBuilder.GetMyKitchenObjects().Count == 0 && GetMyKitchenObjects().Count == 0)
				return false;
			if(recipeBuilder.GetMyKitchenObjects().Count == 0 && GetMyKitchenObjects().Count > 0)
				return GiveIngrediantsTo(recipeBuilder); // don't need can add check because it's baked into give
			if(recipeBuilder.GetMyKitchenObjects().Count > 0 && GetMyKitchenObjects().Count == 0)
				return recipeBuilder.GiveIngrediantsTo(this);
			// if interacting plate has ingrediants then give it to interacted
			if(GetMyKitchenObjects().Count > 0 && recipeBuilder.GetMyKitchenObjects().Count > 0)
				return GiveIngrediantsTo(recipeBuilder);
		}

		if(parent is Player) {
			if(TryGetParent(out AbstractKitchenObjectParent p1) && p1 is not Player)
				parent = p1;
			else
				return false; // recipeBuilder's parent is a player and this has no parent then don't pick this up
			Debug.Log("New parent: " + parent);
		}
		// not a plate and already checked above if it has a parent
		if(recipeBuilder.GiveIngrediantsTo(this)) { // if i can give the ingrediants to the plate then do it and give the plate to recipe builder's parent
			GetComponent<KitchenObject>().SetKitchenObjectParent(parent);
			return true;
		}
		return false;
	}

	protected override void AddSelf() {
		Debug.Log("Not adding a plate as an ingrediant");
	}

	public bool TryGetRecipe(out RecipeSO recipe) {
		if(base.recipe != null && base.recipe.GetIngrediantCount() == kitchenObjectsOnThis.Count) {
			recipe = base.recipe;
			return true;
		}
		recipe = null;
		return false;
	}
}
