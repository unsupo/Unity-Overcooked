using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public abstract class AbstractKitchenObjectParent : MonoBehaviour, ISelectable, IInteractPickUp {
	[SerializeField] protected KitchenObjectSO kitchenObjectSO;
	[SerializeField] protected Transform kitchenObjectHoldPoint;
	[SerializeField] protected List<Transform> gameObjectVisual = new List<Transform>();
	public event EventHandler OnPlayerGrabbedObject;
	public event EventHandler OnPlayerCutObject;

	KitchenObject kitchenObject;
	private bool canPickUp = true;
	public bool CanMove { get { return canPickUp; } set { canPickUp = value; } }
	private bool canSpawn = true;
	public bool CanSpawn { get { return canSpawn; } set { canSpawn = value; } }

	public void SetKitchenObjectSO(KitchenObjectSO kitchenObjectSO) {
		this.kitchenObjectSO = kitchenObjectSO;
	}
	public virtual bool Trash() {
		Debug.Log("Parent Trashing: " + GetKitchenObject());
		if(HasKitchenObject()) {
			if(GetKitchenObject().CanTrash) {
				GetKitchenObject().DestroySelf();
				return true;
			} else if(GetKitchenObject().TryGetComponent(out AbstractKitchenObjectParent c))
				return c.Trash();
		}
		return false;
	}
	protected virtual void RequireHoldPoint() {
		if(kitchenObjectHoldPoint == null) {
			foreach(Transform child in GetComponentInChildren<Transform>())
				if(child.name.EndsWith("TopPoint")) {
					kitchenObjectHoldPoint = child;
					break;
				}
			if(kitchenObjectHoldPoint == null)
				throw new Exception("No top/hold point set for: " + this);
		}
	}
	public List<Transform> GetGameObjectVisual() {
		if(gameObjectVisual.Count == 0) {
			foreach(Transform child in GetComponentInChildren<Transform>())
				if(child.name.Contains("_Visual")) {
					AddGameObjectVisual(child);
					break;
				}
			if(gameObjectVisual == null)
				throw new Exception("No gameobject visual set for (or child gameobject with name ending in _Visual): " + this);
		}
		return gameObjectVisual;
	}
	public void AddGameObjectVisual(Transform transform) {
		gameObjectVisual.Add(transform);
	}
	protected void RaiseOnPlayerCutEvent() {
		OnPlayerCutObject?.Invoke(this, EventArgs.Empty);
	}
	protected void RaiseOnPlayerGrabbedObjectEvent() {
		OnPlayerGrabbedObject?.Invoke(this, EventArgs.Empty);
	}

	protected virtual void RequireKitechenObjectSO() {
		if(kitchenObjectSO == null)
			throw new Exception("No kitechenObjectSO set for: " + this);
	}
	public virtual void Awake() {
		GetGameObjectVisual();
	}

	public virtual void Start() {
		RequireHoldPoint();
	}
	public virtual void InteractProgress(Player player) {
		RaiseOnPlayerCutEvent();
	}
	public Transform GetKitchenObjectFollowTransform() {
		return kitchenObjectHoldPoint;
	}
	public void SetKitchenObject(KitchenObject kitchenObject) {
		Debug.Log("Set ko: " + kitchenObject + ", parent: " + this);
		this.kitchenObject = kitchenObject;
	}
	public KitchenObject GetKitchenObject() {
		return kitchenObject;
	}
	public void ClearKitchenObject() {
		Debug.Log("Cleared ko");
		if(this is AbstractKitchenObjectParentProgressable)
			((AbstractKitchenObjectParentProgressable) this).SetProgress(0);
		SetKitchenObject(null);
	}
	public bool HasKitchenObject() {
		return kitchenObject != null;
	}
	public void DropKitchenObject(Vector3 forward) {
		if(!HasKitchenObject())
			return;
		kitchenObject.Drop(forward);
	}
	public KitchenObjectSO GetKitchenObjectSO() {
		return kitchenObjectSO;
	}
	/**
	 * This is interacting with interacted two cases
	 * this interacts with ko in which case try to pick it up
	 * this interacts with parent in which case see if you can pick up it's ko or pick up it's ko
	 */
	public virtual bool InteractPickUp(ISelectable interacted) {
		Debug.Log("Interacted: " + this + ", " + interacted);
		// interacted is a ko and this has no ko and can stack the interacted then add it to this
		if(interacted is KitchenObject && !HasKitchenObject() && CanStack(((KitchenObject) interacted).GetKitchenObjectSO())) {
			((KitchenObject) interacted).SetKitchenObjectParent(this);
			return true;
		}
		if(interacted is AbstractKitchenObjectParent || (interacted is KitchenObject && ((KitchenObject) interacted).GetComponent<AbstractKitchenObjectParent>())) {
			AbstractKitchenObjectParent interactedParent = interacted is AbstractKitchenObjectParent ? (AbstractKitchenObjectParent) interacted : ((KitchenObject) interacted).GetComponent<AbstractKitchenObjectParent>();
			Debug.Log("Interacted with parent: " + interactedParent);
			// give to this
			if(interactedParent.HasKitchenObject() && !HasKitchenObject()) {
				if(this is Player)
					interactedParent.InteractPickUp(this);
				else
					interactedParent.GetKitchenObject().SetKitchenObjectParent(this);
				return true;
			}
			// give to other
			if(!interactedParent.HasKitchenObject() && HasKitchenObject()) {
				if(this is Player)
					interactedParent.InteractPickUp(this);
				else
					GetKitchenObject().SetKitchenObjectParent(interactedParent);
				return true;
			}
			// interacted has a child that can be interacted with
			if(interactedParent.TryGetChild(out AbstractKitchenObjectParent child) && child.InteractPickUp(this)) {
				return true;
			}
			// this has a child that can be interacted with, this is here so that thrown objects won't stick to plate
			if(TryGetChild(out AbstractKitchenObjectParent child1) && child1.InteractPickUp(interacted)) {
				return true;
			}
		}
		return false;
	}

	public bool TryGetChildIfParentAndCanStack(KitchenObjectSO koSO, out AbstractKitchenObjectParent child) {
		if(TryGetChild(out AbstractKitchenObjectParent c) && CanStack(koSO)) {
			child = c;
			return true;
		}
		child = null;
		return false;

	}
	public bool TryGetChild(out AbstractKitchenObjectParent child) {
		if(HasKitchenObject() && GetKitchenObject().TryGetComponent(out AbstractKitchenObjectParent c)) {
			child = c;
			return true;
		}
		child = null;
		return false;
	}
	// if this is a ko then it could have a prent
	public bool TryGetParent(out AbstractKitchenObjectParent parent) {
		if(TryGetComponent(out KitchenObject ko) && ko.GetKitchenObjectParent() != null) {
			parent = ko.GetKitchenObjectParent();
			return true;
		}
		parent = null;
		return false;
	}
	/**
	 * It can always stack if there are no so in the list because there is a different property for if it can at all
	 * This can't get rid of fryingpanable because of properties of fryingpanable like change progress and change into ect
	 * This might be useless
	 */
	public bool CanStack(KitchenObjectSO kitchenObjectSO) {
		return true; // CanHaveThese.Count == 0 ? true : CanHaveThese.Contains(kitchenObjectSO);
	}

}