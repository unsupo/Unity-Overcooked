using System;
using UnityEngine;

public class FryingPan : AbstractKitchenObjectParentProgressable {
	[SerializeField] GameObject burnWarningUI;
	[SerializeField] Vector3 burnWarningRelativePosition = new Vector3(0, -.25f, 0);
	[SerializeField] string warningSoundName = "warning";
	bool isRunning = false;
	bool isShowingIngrediant = false;
	bool isBurnable = false;
	int nullCount = 0;
	float warningSoundTimer = .2f;
	GameObject burnWarningUICanvas;

	public override void Start() {
		RequireHoldPoint();
		SetIsCooking(false);
		GetBurnWarningUICanvas();
	}

	private GameObject GetBurnWarningUICanvas() {
		if(burnWarningUICanvas == null) {
			Debug.Log("Creating burn warning ui canvas bar");
			burnWarningUICanvas = Instantiate(burnWarningUI);
			burnWarningUICanvas.transform.SetParent(transform);
			burnWarningUICanvas.transform.localPosition = burnWarningRelativePosition;
			burnWarningUICanvas.gameObject.SetActive(false);
		}
		return burnWarningUICanvas;
	}

	/*
 * Same logic ould be moved up for a pot
 */
	public override bool InteractPickUp(ISelectable intereacted) {
		Debug.Log("Interacted with frying pan: " + this + ", " + intereacted);
		if(intereacted is KitchenObject) {
			if(intereacted is KitchenObject) {
				if(((KitchenObject) intereacted).GetComponent<FryingPanable>() && !HasKitchenObject()) {
					// add fryingpanable to this
					((KitchenObject) intereacted).SetKitchenObjectParent(this);
					return true;
				} else if(((KitchenObject) intereacted).TryGetComponent(out FryingPan fryingPan)) //frying pan interact with frying pan
					return InteractFryingPan(fryingPan);
				else if(((KitchenObject) intereacted).TryGetComponent(out AbstractRecipeBuilder plate) && HasKitchenObject()) {
					// only interact with the plate if i have a kitchen object
					// frying pan interact with plate
					if(plate.InteractPickUp(GetKitchenObject())) {
						return true;
					}
				} else
					return false;
			}
		}

		if(intereacted is AbstractKitchenObjectParent) {
			AbstractKitchenObjectParent p = (AbstractKitchenObjectParent) intereacted;
			// stove or player doesn't have a ko then give the frying pan to that parent
			if(!p.HasKitchenObject()) {
				GetComponent<KitchenObject>().SetKitchenObjectParent(p);
				return true;
			} else if(intereacted is Player) {
				// if it's a player
				if(!HasKitchenObject() && p.GetKitchenObject().GetComponent<FryingPanable>()) {
					// if this frying pan is empty and the player has a ko that can go into a frying pan then put it in
					p.GetKitchenObject().SetKitchenObjectParent(this);
					return true;
				} else if(HasKitchenObject() && p.GetKitchenObject().TryGetComponent(out AbstractRecipeBuilder plate)) {
					// if this is not empty and the player has a plate then put it on the plate
					return plate.InteractPickUp(GetKitchenObject());
				}
			} else if(intereacted is AbstractRecipeBuilder && ((AbstractRecipeBuilder) p).InteractPickUp(GetKitchenObject())) {
				return true;
			} else if(intereacted is FryingPan)
				return InteractFryingPan(p);
			else {
				// must be a counter top of some sort with a ko
				// if the ko is a recipe builder then try to give frying pan's ko to the recipe builder (not just plate)
				if(p.GetKitchenObject().TryGetComponent(out AbstractRecipeBuilder recipeBuilder)) {
					if(!HasKitchenObject())
						return false;
					return recipeBuilder.AddKitchenObject(GetKitchenObject());
				} else if(p.GetKitchenObject().TryGetComponent(out FryingPan fryingPan)) {
					return false;
				}
				p.GetKitchenObject().SetKitchenObjectParent(this);
				GetComponent<KitchenObject>().SetKitchenObjectParent(p);
				return true;
			}
		}
		// no other options 
		return false;
	}

	private bool InteractFryingPan(AbstractKitchenObjectParent p) {
		// if this frying pan has a ko and the other doesn't or vice versa then swap ko
		if(HasKitchenObject() && !p.HasKitchenObject() && GetKitchenObject().GetComponent<FryingPanable>()) {
			((FryingPan) p).SetProgress(GetProgress());
			GetKitchenObject().SetKitchenObjectParent(p);
			return true;
		} else if(!HasKitchenObject() && p.HasKitchenObject() && p.GetKitchenObject().GetComponent<FryingPanable>()) {
			SetProgress(((FryingPan) p).GetProgress());
			p.GetKitchenObject().SetKitchenObjectParent(this);
			return true;
		}
		return false;
	}
	private void ClearIngrediantIcons() {
		if(TryGetComponent(out IngrediantIcon ingrediant) && ingrediant.shouldShowChild)
			ingrediant.GetFoodIconsUI().ClearSprites();
		isShowingIngrediant = false;
	}

	public override void Update() {
		if(!HasKitchenObject()) {
			Debug.Log("ko: " + GetKitchenObject());
			SetIsCooking(false);
			ClearIngrediantIcons();
			isBurnable = false;
		} else {
			if(HasKitchenObject() && !isShowingIngrediant && TryGetComponent(out IngrediantIcon ingrediant) && ingrediant.shouldShowChild) {
				ingrediant.GetFoodIconsUI().SetSprites(GetKitchenObject().GetKitchenObjectSO().sprite);
				isShowingIngrediant = true;
				if(isBurnable)
					SetVisible(false);
			}
		}
		base.Update();
		// this must be in update because we don't want the player to have to press the button start frying
		IfProgressableTypeThen((FryingPanable fryingPanObject) => { // there is an object in the frying pan
			if(GetComponent<KitchenObject>().GetKitchenObjectParent() is StoveCounter) { // the frying pan is on the stove
				if(!isRunning)
					IfProgressableTypeThen((FryingPanable FryingPanable) => { CanMove = false; isRunning = true; StartProgress(FryingPanable.ChangeProgressMax); });
				SetIsCooking(true);
				if(isBurnable) {
					SetVisible(false);
					float burnPercent = GetProgress() / (float) fryingPanObject.ChangeProgressMax;
					if(burnPercent > .25f && burnPercent < .5f) {
						PlayWarningSound(.15f, 1);
					} else if(burnPercent > .5f && burnPercent < .75f) {
						PlayWarningSound(.075f, 2);
					} else if(burnPercent > .75f && burnPercent < 1) {
						PlayWarningSound(.05f, 4);
					} else {
						GetBurnWarningUICanvas().SetActive(false);
					}
				} else
					burnWarningUICanvas.SetActive(false);
				if(GetProgress() >= fryingPanObject.ChangeProgressMax) {
					if(fryingPanObject.KitchenObjectSO.isBurnable) {
						SetVisible(false);
						isBurnable = true;
					} else {
						SetVisible(true);
						isBurnable = false;
					}
					ClearIngrediantIcons();
					CanMove = true;
					SetIsCooking(false);
					GetKitchenObject().DestroySelf(); // unfortunately order matters here.  If this goes second then it will clear the new spawned object from the parent
					KitchenObject.SpawnKitchenObject(fryingPanObject.KitchenObjectSO, this);
					isRunning = false;
				}
			} else { // this means the frying pan is not on the stove
				SetIsCooking(false);
			}
		}, () => SetIsBurned(true)); // if the frying pan has a kitchenobject, but ist's not a fryingpanable then that means it's burned
	}

	private void PlayWarningSound(float beeptime, float speed) {
		GetBurnWarningUICanvas().SetActive(true);
		GetBurnWarningUICanvas().GetComponent<Animator>().SetFloat("Speed", speed);
		warningSoundTimer -= Time.deltaTime;
		if(warningSoundTimer <= 0f) {
			SoundManager.Instance.PlaySound(warningSoundName, transform.position);
			warningSoundTimer = beeptime;
		}
	}

	private void SetIsCooking(bool isCooking) {
		if(!isCooking) {
			shouldProgress = false;
			isRunning = false;
			GetBurnWarningUICanvas().SetActive(false);
		} else {
			shouldProgress = true;
			isRunning = true;
		}
		gameObject.GetComponent<Stoveable>().StoveState = isCooking ? Stoveable.StovableState.OnStoveCooking : Stoveable.StovableState.OffStove;
	}
	private void SetIsBurned(bool isBurned) {
		if(isBurned) {
			Debug.Log("It's burned: " + this);
			gameObject.GetComponent<Stoveable>().StoveState = Stoveable.StovableState.OnStoveBurned;
			gameObject.GetComponent<Stoveable>().BurnState = Stoveable.BurnableState.Burned;
			SetIsCooking(false);
		}
	}
}