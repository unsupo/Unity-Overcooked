using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

[CreateAssetMenu()]
public class RecipeSO : ScriptableObject {
	[SerializeField] private GameObject visualCompletedRecipe;
	[SerializeField] private GameObject visualGameObject;
	[SerializeField] public Sprite visualSprite;
	Dictionary<string, RecipeObject> recipeDictionary;
	[Serializable]
	public class RecipeObject {
		public int count = 0;
		public GameObject gameObject;
		public RecipeObject(GameObject gameObject) {
			this.gameObject = gameObject;
			count = 1;
		}
		public RecipeObject(GameObject gameObject, int count) : this(gameObject) {
			this.count = count;
		}
		public override string ToString() {
			return gameObject.name+", "+count;
		}
		public KitchenObjectSO GetKitchenObjectSO() {
			return gameObject.GetComponent<Ingrediant>().kitchenObjectSO;
		}
	}
	public GameObject GetVisualCompletedRecipe() {
		return visualCompletedRecipe;
	}
	public Dictionary<string, RecipeObject> GetRecipeNameQuantityDict() {
		if(visualCompletedRecipe == null)
			throw new Exception("Visual Completed Recipe is empty for recipe: " + this);
		if(visualGameObject == null) {
			visualGameObject = Instantiate(visualCompletedRecipe);
			visualGameObject.SetActive(false);
		}
		if(recipeDictionary == null) {
			recipeDictionary = new Dictionary<string, RecipeObject>();
			foreach(Transform child in visualGameObject.GetComponentInChildren<Transform>())
				if(!recipeDictionary.ContainsKey(child.name))
					recipeDictionary.Add(Regex.Replace(child.name,@"/w+\([0-9]+\)","").Replace("_Visual",""), new RecipeObject(child.gameObject));
				else
					recipeDictionary[child.name].count++;
		}
		return recipeDictionary;
	}

	internal bool HasKitchenObject(KitchenObject kitchenObject, int count = 1) {
		string name = kitchenObject.name.Replace("(Clone)", "");
		//Debug.Log("Checking if i can add ingrediant to the recipe: " + this + ", " + name + ", " + count + ", " + GetRecipeNameQuantityDict().ContainsKey(name) + ", " +
		//	(GetRecipeNameQuantityDict().ContainsKey(name) ? GetRecipeNameQuantityDict()[name].count : ""));
		return GetRecipeNameQuantityDict().ContainsKey(name) && count <= GetRecipeNameQuantityDict()[name].count;
	}

	public int GetIngrediantCount() {
		int count = 0;
		foreach(KeyValuePair<string, RecipeObject> valuePair in GetRecipeNameQuantityDict())
			count += valuePair.Value.count;
		return count;
	}
}
