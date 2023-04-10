using UnityEngine;
using System.Collections;

public class IngrediantIcon : MonoBehaviour {
	[SerializeField] bool isIngrediant = false; // TODO would be nice to figure this out based on recipes
	[SerializeField] public bool shouldShowChild = false;
	[SerializeField] FoodIconsUI foodIconsUI;
	[SerializeField] Vector3 uiRelativePosition = new Vector3(0, .75f, 0);
	GameObject foodIconsUICanvas;
	FoodIconsUI spawnedFoodIconsUI;
	bool isSpriteSet = false;

	private void Awake() {
		if(foodIconsUI != null && foodIconsUICanvas == null) { // TODO duplicate code
			Debug.Log("Creating food icons ui");
			foodIconsUICanvas = Instantiate(foodIconsUI.gameObject);
			spawnedFoodIconsUI = foodIconsUICanvas.GetComponent<FoodIconsUI>();
			foodIconsUICanvas.transform.SetParent(transform);
			foodIconsUICanvas.transform.localPosition = uiRelativePosition;
			foodIconsUICanvas.gameObject.SetActive(true);
		}
	}

	private void Start() {
		if(isIngrediant && !isSpriteSet) {
			if(TryGetComponent(out KitchenObject kitchenObject)) {
				if(foodIconsUICanvas.GetComponent<FoodIconsUI>().SetSprites(kitchenObject.GetKitchenObjectSO().sprite))
					isSpriteSet = true;
			} else
				Debug.Log("Can't find ko for: " + this);
		}
	}

	private void Update() {
		if(TryGetComponent(out KitchenObject kitchenObject) && kitchenObject.GetKitchenObjectParent() && kitchenObject.GetKitchenObjectParent().GetComponent<IngrediantIcon>())
			foodIconsUICanvas.SetActive(false);
		else
			foodIconsUICanvas.SetActive(true);
	}

	public FoodIconsUI GetFoodIconsUI() {
		return foodIconsUICanvas.GetComponent<FoodIconsUI>();
	}

}

