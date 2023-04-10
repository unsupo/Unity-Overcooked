using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ContainerCounter : AbstractKitchenObjectParent {
	// this isn't behavior that overcooked has so i'll not include it by default
	[SerializeField] bool spawnOnPlate = false;

	public override void Start() {
		RequireKitechenObjectSO();
	}

	public override bool InteractPickUp(ISelectable interacted) {
		if(!HasKitchenObject() && interacted is Player) {
			Player player = (Player) interacted;
			// player doesn't have ko then spawn a new so into player
			if(!player.HasKitchenObject()) {
				Spawn(player);
				return true;
			} else if(spawnOnPlate && player.GetKitchenObject().TryGetComponent(out AbstractRecipeBuilder plate)) {
				// player has a ko but it's a plate then spawn it on the plate
				Spawn(plate);
				return true;
			}
		}
		// no spawning then do placing
		return base.InteractPickUp(interacted);
	}

	private void Spawn(AbstractKitchenObjectParent interacting) {
		Debug.Log("SPAWN -- Give to interacting: interacting=" + interacting + ", interacted=" + this);
		KitchenObject.SpawnKitchenObject(this.GetKitchenObjectSO(), interacting);
		RaiseOnPlayerGrabbedObjectEvent();  // animate the container counter
		return;
	}
}
