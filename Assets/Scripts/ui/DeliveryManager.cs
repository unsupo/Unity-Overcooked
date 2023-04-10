using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DeliveryManager : MonoBehaviour {
	public static DeliveryManager Instance { get; private set; }

	[SerializeField] static string recipeResourcesRelativePath = "ScriptableObjects/RecipeSO";
	[SerializeField] int startingRecipeCount = 1;
	[SerializeField] int waitingRecipeMax = 5;
	[SerializeField] float spawnRecipeMaxTime = 4f; // every 25 seconds
	[SerializeField] GameObject recipeManagerUI;
	[SerializeField] GameObject recipeUI;
	float spawnRecipeTimer = 4f;
	GameObject spawnedRecipeManagerUI;
	List<GameObject> waitingForRecipes = new List<GameObject>();


	static List<RecipeSO> recipeSOs;
	public static List<RecipeSO> GetRecipes() {
		if(recipeSOs == null) {
			recipeSOs = new List<RecipeSO>();
			foreach(Object f in Resources.LoadAll(recipeResourcesRelativePath))
				recipeSOs.Add((RecipeSO) f);
		}
		return recipeSOs;
	}

	public static bool SubmitPlate(Plate plate, Vector3 deliveryCounterPosition) {
		return Instance.TurninPlate(plate, deliveryCounterPosition);
	}

	public bool TurninPlate(Plate plate, Vector3 deliveryCounterPosition) {
		Debug.Log("Submitting plate: " + ", " + plate);
		if(plate.TryGetRecipe(out RecipeSO recipe1)) {
			Debug.Log("Submitting plate, recipe found: " + ", " + plate + ", " + recipe1);
			foreach(GameObject recipe in Instance.waitingForRecipes) { // this should go in order
				RecipeSO recipeSO = recipe.GetComponent<RecipeUI>().GetRecipe();
				if(recipeSO.name.Equals(recipe1.name)) {
					// assuming unique names among recipes
					// TODO points
					PointsManager.AddPoints(100);
					// start timing if you're not already
					TimeManager.StartTiming();
					Debug.Log("Successfully submitted plate: " + ", " + plate + ", " + recipe1 + ", " + recipeSO);
					recipe.GetComponent<RecipeUI>().DestroySelf();
					Instance.waitingForRecipes.Remove(recipe);
					return true;
				}
			}
			return false;
		} else {
			return false;
		}
	}

	private void AddRecipe() {
		RecipeSO recipe =  GetRecipes()[Random.Range(0, GetRecipes().Count)];
		// add to ui create a recipe UI and make it a child of spawnedRecipeManagerUI
		GameObject spawnedRecipeUI = Instantiate(recipeUI);
		spawnedRecipeUI.transform.SetParent(transform);
		spawnedRecipeUI.gameObject.SetActive(true);
		spawnedRecipeUI.GetComponent<RecipeUI>().setRecipe(recipe);


		waitingForRecipes.Add(spawnedRecipeUI);
	}

	private void Awake() {
		if(Instance != null && Instance != this)
			Destroy(this);
		else
			Instance = this;
		if(spawnedRecipeManagerUI != null) { // TODO duplicate code
			Debug.Log("Creating recipe manager ui");
			spawnedRecipeManagerUI = Instantiate(recipeManagerUI);
			spawnedRecipeManagerUI.transform.SetParent(transform);
			spawnedRecipeManagerUI.gameObject.SetActive(true);
		}
	}
	bool isStarted = false;
	// Update is called once per frame
	void Update() {
		if(GameHandler.GetGameState() != GameHandler.GameState.GamePlaying)
			return;

		if(!isStarted) {
			for(int i = 0; i < startingRecipeCount; i++)
				AddRecipe();
			isStarted = true;
		}

		spawnRecipeTimer -= Time.deltaTime;
		if(spawnRecipeTimer <= 0f && waitingForRecipes.Count < waitingRecipeMax) {
			spawnRecipeTimer = spawnRecipeMaxTime;
			AddRecipe();
		}
		waitingForRecipes.RemoveAll((GameObject recipeUI) => {
			if(recipeUI.TryGetComponent(out RecipeUI recipe) && recipe.IsDestroyed()) {
				recipe.DestroySelf();
				return true;
			}
			return false;
		});
	}
}
