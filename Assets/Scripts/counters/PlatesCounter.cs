using UnityEngine;

public class PlatesCounter : AbstractKitchenObjectParent {
	private int maxPlates = 5;
	private int currentPlates = 0;
	private AbstractKitchenObjectParent lastPlate;

	public override void Start() {
		base.Start();
		CanSpawn = false;
	}
	private void Update() {
		if(currentPlates < maxPlates)
			SpawnPlates(maxPlates - currentPlates);
	}

	public void SpawnPlates(int count = 1) {
		for(int i = 0; i < count; i++)
			push();
	}

	private bool TryGetTopPlate(out KitchenObject plate) {
		if(lastPlate is Plate) {
			plate = lastPlate.GetComponent<KitchenObject>();
			return true;
		}
		plate = null;
		return false;
	}
	private void push() {
		KitchenObject plate = KitchenObject.SpawnKitchenObject(GetKitchenObjectSO(), lastPlate ? lastPlate : this);
		lastPlate = plate.GetComponent<AbstractKitchenObjectParent>();
		currentPlates++;

	}
	private void pop() {
		if(lastPlate.TryGetParent(out AbstractKitchenObjectParent p))
			lastPlate = p;
		else
			lastPlate = null;
	}

	public override bool InteractPickUp(ISelectable interacting) {
		if(interacting is Player && !((Player)interacting).HasKitchenObject()) { 
			Debug.Log(this+": give to player");
			// give the object to player
			if(TryGetTopPlate(out KitchenObject kitchenObject)) {
				pop();
				kitchenObject.SetKitchenObjectParent((Player) interacting);
				Debug.Log(this + ": Got plate=" + kitchenObject + ", give to player=" + interacting+", lastplate="+lastPlate);
			}
		}
		return false;
	}
}
