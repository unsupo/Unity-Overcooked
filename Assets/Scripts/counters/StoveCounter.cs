using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

/**
 * Notes, non cuttable objects CANNOT be placed
 */
public class StoveCounter : AbstractKitchenObjectParent {

	[SerializeField] List<GameObject> activeOnCooking = new List<GameObject>();
	//[SerializeField] List<GameObject> activeWhenFinishCooking = new List<GameObject>(); // the green check shows up for a second
	//[SerializeField] List<GameObject> activeBeforeBurn = new List<GameObject>(); // the warning shows up just before it burns and increases in intensity, 3 stages of beeping
	//[SerializeField] List<GameObject> activeWhenBurned = new List<GameObject>(); // fire icon and fire with smoke animation then i assume fire randomly spreads to a counter nearby
	[SerializeField] string soundChildName = "Sound";

	public override void Start() {
		RequireHoldPoint();
	}
	public override bool InteractPickUp(ISelectable interacting) {
		Debug.Log("Interacted with stove: " + this + ", " + interacting);
		if(interacting is AbstractKitchenObjectParent || (interacting is KitchenObject && ((KitchenObject)interacting).GetComponent<AbstractKitchenObjectParent>())) {
			AbstractKitchenObjectParent p = interacting is AbstractKitchenObjectParent ? (AbstractKitchenObjectParent) interacting : ((KitchenObject) interacting).GetComponent<AbstractKitchenObjectParent>();
			if(p.GetComponent<Stoveable>()) {
				p.GetComponent<KitchenObject>().SetKitchenObjectParent(this);
				return true;
			}
			if(p is Player) {
				// player is holding a frying pan or other staveable
				if(!HasKitchenObject() && p.HasKitchenObject() && p.GetKitchenObject().GetComponent<Stoveable>()) {
					p.GetKitchenObject().GetComponent<KitchenObject>().SetKitchenObjectParent(this);
					return true;
				}
				// frying pan (stoveable) is on stove
				if(HasKitchenObject()) {
					Debug.Log("I have ko: " + GetKitchenObject());
					// and player has no ko then give frying pan to player
					if(!p.HasKitchenObject()) {
						GetKitchenObject().SetKitchenObjectParent(p);
						return true;
					}
					// and player is holding ko then try to give it to the frying pan
					if(p.HasKitchenObject())
						return GetKitchenObject().GetComponent<AbstractKitchenObjectParent>().InteractPickUp(p.GetKitchenObject());
				}
			}
		} else
			Debug.Log("Interacted is not a AbstractKitchenObjectParent: " + interacting);
		return false;
	}

	private void Update() {
		SetState(Stoveable.StovableState.OnStoveCooking, IsCooking());
	}

	Dictionary<Stoveable.StovableState, bool> previousStoveStates = new Dictionary<Stoveable.StovableState, bool>();
	private void SetState(Stoveable.StovableState stoveState, bool isActive) {
		if(!previousStoveStates.ContainsKey(stoveState))
			previousStoveStates.Add(stoveState, isActive);
		else if(previousStoveStates[stoveState] == isActive)
			return;
		previousStoveStates[stoveState] = isActive;
		switch(stoveState) {
			case Stoveable.StovableState.OnStoveCooking:
				foreach(GameObject gameObject in activeOnCooking)
					gameObject.SetActive(isActive);
				if(Utility.TryGetChild(gameObject, soundChildName, out GameObject child) && child.TryGetComponent(out AudioSource audioSource)) {
					if(isActive) {
						if(!audioSource.isPlaying) {
							audioSource.Play();
							Debug.Log("Playing sound");
						}
					} else {
						audioSource.Pause();
						Debug.Log("NOT Playing sound");
					}
				}
				break;
		}
	}

	private bool HasStoveable() {
		return HasKitchenObject() && GetKitchenObject().GetComponent<Stoveable>();
	}

	private bool IsCooking() {
		return HasStoveable() && ((Stoveable) GetKitchenObject().GetComponent<Stoveable>()).StoveState == Stoveable.StovableState.OnStoveCooking;
	}
}
