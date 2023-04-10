using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeliveryCounter : AbstractKitchenObjectParent {
	[SerializeField] string soundNameWhenDeliverySuccess;
	[SerializeField] string soundNameWhenDeliveryFailure;
	[SerializeField] GameObject deliveryMessageUI;
	[SerializeField] Vector3 uiRelativePosition = new Vector3(0, 2.5f, .5f);
	private GameObject deliveryMessage;

	private void Awake() {
		if(deliveryMessage == null) {
			Debug.Log("Creating progress bar");
			deliveryMessage = Instantiate(deliveryMessageUI.gameObject);
			deliveryMessage.transform.SetParent(transform);
			deliveryMessage.transform.localPosition = uiRelativePosition;
			deliveryMessage.gameObject.SetActive(false);
		}

	}

	public override bool InteractPickUp(ISelectable interacted) {
		if(TryGetPlate(interacted, out Plate plate)) {
			if(!DeliveryManager.SubmitPlate(plate, transform.position)) {
				// TODO indicate wrong plate here
				SoundManager.Instance.PlaySound(soundNameWhenDeliveryFailure, transform.position);
				deliveryMessage.GetComponent<ResultUI>().ShowResult(false);
				plate.GetComponent<KitchenObject>().DestroySelf();
				return false;
			}
			plate.GetComponent<KitchenObject>().DestroySelf();
			SoundManager.Instance.PlaySound(soundNameWhenDeliverySuccess, transform.position);
			deliveryMessage.GetComponent<ResultUI>().ShowResult(true);
			return true;
		}
		return false;
	}

	bool TryGetPlate(ISelectable interacted, out Plate plate) {
		if(interacted is Plate || (interacted is Player && ((Player) interacted).HasKitchenObject() && ((Player) interacted).GetKitchenObject().GetComponent<Plate>())) {
			if(interacted is Plate)
				plate = (Plate) interacted;
			else
				plate = ((Player) interacted).GetKitchenObject().GetComponent<Plate>();
			return true;
		}
		plate = null;
		return false;
	}
}
