using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KitchenObject : MonoBehaviour, ISelectable, IDestroyable, IPickUpable {
	[SerializeField] KitchenObjectSO kitchenObjectSO;
	[SerializeField] Vector3 throwForce = new Vector3(5f, .2f, 5f);
	[SerializeField] private bool canTrash = true;
	[SerializeField] private bool canMove = true;
	[SerializeField] private List<KitchenObjectSO> canInteractWithThese = new List<KitchenObjectSO>();
	AbstractKitchenObjectParent kitchenObjectParent;
	public bool CanTrash { get => canTrash; set => canTrash = value; }
	public bool CanLookAt { get; set; }
	public List<KitchenObjectSO> CanInteractWithThese { get => canInteractWithThese; set => canInteractWithThese = value; }
	public bool CanMove { get => canMove; set => canMove = value; }
	public bool isOnPlate = false;

	public static KitchenObject SpawnKitchenObject(KitchenObjectSO kitchenObjectSO, AbstractKitchenObjectParent kitchenObjectParent) {
		Transform kitchenObjectTransform = Instantiate(kitchenObjectSO.prefab);
		KitchenObject kitchenObject = kitchenObjectTransform.GetComponent<KitchenObject>();
		kitchenObject.SetKitchenObjectParent(kitchenObjectParent);
		return kitchenObject;
	}

	public KitchenObjectSO GetKitchenObjectSO() {
		return kitchenObjectSO;
	}
	public void SetKitchenObjectSO(KitchenObjectSO kitchenObjectSO) {
		this.kitchenObjectSO = kitchenObjectSO;
	}

	public void SetKitchenObjectParent(AbstractKitchenObjectParent kitchenObjectParent) {
		if(this.kitchenObjectParent != null) // if i have a parent then clear the parent
			this.kitchenObjectParent.ClearKitchenObject();
		this.kitchenObjectParent = kitchenObjectParent;
		if(kitchenObjectParent) {
			kitchenObjectParent.SetKitchenObject(this);
			transform.parent = kitchenObjectParent.GetKitchenObjectFollowTransform();
			transform.localPosition = Vector3.zero;
			SetPhysics(false);
		} else {
			transform.parent = null;
			SetPhysics(true);
		}
	}

	private void SetPhysics(bool isPhysics) {
		GetComponent<Rigidbody>().isKinematic = !isPhysics;
		GetComponent<Collider>().enabled = isPhysics;
		GetComponent<Rigidbody>().useGravity = isPhysics;
		CanLookAt = !isPhysics;
	}

	public AbstractKitchenObjectParent GetKitchenObjectParent() {
		return kitchenObjectParent;
	}

	internal void Drop(Vector3 forward) {
		SetKitchenObjectParent(null);
		// Throw it up only if the character is moving
		if(forward != Vector3.zero)
			forward.y = 1;
		transform.position += forward;
		GetComponent<Rigidbody>().velocity = Vector3.Scale(forward, throwForce);
		GetComponent<Rigidbody>().AddForce(Vector3.Scale(forward, throwForce), ForceMode.Impulse);
	}
	void OnCollisionEnter(Collision collision) {
		print("Another object has entered the trigger: " + collision + ", " + this);
		// if this ko collides with a akoparent and it's not a player then interact
		if(collision.gameObject.TryGetComponent(out IInteractPickUp interacting) && interacting is not Player) {
			Debug.Log("Doing this");
			// I want to call the selectable's interactpickup though
			interacting.InteractPickUp(this);
		}
	}

	public void DestroySelf() {
		SetKitchenObjectParent(null);
		Debug.Log("Destroying myself: " + this);
		Destroy(gameObject);
	}
}
