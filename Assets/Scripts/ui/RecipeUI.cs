using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEditor.AnimatedValues;

public class RecipeUI : MonoBehaviour {
	[SerializeField] string spritePath = "top/CompleteImageContainer/Plate/CompleteRecipe";
	[SerializeField] string progressBarPath = "top/progress/progressbar";
	[SerializeField] string fillName = "fill";
	[SerializeField] string ingrediantCookMethodUIPath = "bottom"; // todo move this to ingrediantCookMethodUI gameobject
	[SerializeField] GameObject ingrediantCookMethodUI;
	[SerializeField] Gradient foodTimerGradient; 
	[SerializeField] float timeUntilExpire = 10f;
	float timeLeftTimer;
	bool isDestroyed = false;
	RecipeSO recipeSO;

	public bool IsDestroyed() {
		return isDestroyed;
	}

	public RecipeSO GetRecipe() {
		return recipeSO;
	}

	List<GameObject> spawnedIngrediantCookMethodUIs = new List<GameObject>();

	internal void setRecipe(RecipeSO recipe) {
		recipeSO = recipe;
		if(Utility.TryGetChildByPath(gameObject, spritePath, out GameObject child) && child.TryGetComponent(out Image image)) {
			image.sprite = recipe.visualSprite;
		}
		if(Utility.TryGetChild(gameObject, ingrediantCookMethodUIPath, out GameObject child1)) {
			foreach(KeyValuePair<string, RecipeSO.RecipeObject> ingrediantCount in recipe.GetRecipeNameQuantityDict()) {
				// TODO if things are cooked togehter then they should be under one umbrella
				GameObject spawnedIngrediantCookMethodUI = Instantiate(ingrediantCookMethodUI);
				if(Utility.TryGetChildByPath(spawnedIngrediantCookMethodUI, "plate/ingrediant", out GameObject ingrediant) && ingrediant.TryGetComponent(out Image ingrediantImage)) {
					ingrediantImage.sprite = ingrediantCount.Value.GetKitchenObjectSO().sprite;
					if(Utility.TryGetChild(spawnedIngrediantCookMethodUI, "CookMethod", out GameObject cookMethod)) {
						if(ingrediantCount.Value.GetKitchenObjectSO().cookingSprite == null)
							DestroyImmediate(cookMethod);
						else
							cookMethod.GetComponent<Image>().sprite = ingrediantCount.Value.GetKitchenObjectSO().cookingSprite;
					}
				}
				spawnedIngrediantCookMethodUI.transform.SetParent(child1.transform);
				spawnedIngrediantCookMethodUI.gameObject.SetActive(true);
				spawnedIngrediantCookMethodUIs.Add(ingrediantCookMethodUI);
				UnityEngine.UI.LayoutRebuilder.ForceRebuildLayoutImmediate(GetComponent<RectTransform>());
			}
		}
		UnityEngine.UI.LayoutRebuilder.ForceRebuildLayoutImmediate(GetComponent<RectTransform>());
	}

	// Use this for initialization
	void Start() {
		timeLeftTimer = timeUntilExpire;
		UpdateProgressBar();
	}

	// Update is called once per frame
	void Update() {
		timeLeftTimer -= Time.deltaTime;
		UpdateProgressBar();
		if(timeLeftTimer <= 0f) {
			isDestroyed = true;
		}

	}

	private void UpdateProgressBar() {
		if(Utility.TryGetChildByPath(gameObject, progressBarPath, out GameObject progressbar)) {
			int barsCount = progressbar.GetComponentInChildren<Transform>().childCount;
			float barPercent = barsCount * timeLeftTimer / timeUntilExpire;
			int barIndex = (int) Math.Floor(barPercent);
			int progressBarNum = 0;
			float barProgress = (float) (barPercent - Math.Truncate(barPercent));
			int i0 = barIndex - 1 < 0 ? 0 : barIndex - 1;
			int i1 = barIndex - 2 < i0 ? i0 + 1 : barIndex - 2;
			foreach(Transform bar in progressbar.GetComponentInChildren<Transform>()) {
				if(Utility.TryGetChild(bar.gameObject, fillName, out GameObject fillGameObject) && fillGameObject.TryGetComponent(out Image fillImage)) {
					if(progressBarNum < barIndex) {
						fillImage.fillAmount = 1; // set color to be green and fill to full
					} else if(progressBarNum > barIndex) {
						fillImage.fillAmount = 0; // set fill to 0, color doesn't matter because fill is 0
					} else {
						fillImage.fillAmount = barProgress; // set color to be between the gradiant color keys and fill to be between
					}
					fillImage.color = foodTimerGradient.Evaluate(timeLeftTimer / timeUntilExpire);
					UnityEngine.UI.LayoutRebuilder.ForceRebuildLayoutImmediate(GetComponent<RectTransform>());
				}
				progressBarNum++;
			}
		}
	}

	public void DestroySelf() {
		DestroyImmediate(gameObject);
	}
}

