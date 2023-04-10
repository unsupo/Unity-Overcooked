using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : AbstractKitchenObjectParent {
	public static Player Instance { get; private set; }

	public event EventHandler<OnSelectedCounterChangedEventArgs> OnSelectedCounterChanged;
	public class OnSelectedCounterChangedEventArgs : EventArgs {
		public ISelectable selected; // assuming only 1 thing can be selected at a time
	}

	[SerializeField] private float moveSpeed = 7f;
	[SerializeField] private GameObject capsule;
	[SerializeField] private float rotateSpeed = 10f;
	[SerializeField] private float interactDistance = 2f;
	[SerializeField] private GameInput gameInput;
	[SerializeField] private LayerMask countersLayerMask;
	[SerializeField] private string soundWhenPlayerPickUp;
	[SerializeField] private string soundWhenPlayerDrop;
	[SerializeField] private string soundWhenPlayerWalking;
	[SerializeField] private float footstepSoundTimer = .1f;
	private float footstepTimer;

	CapsuleCollider capsuleCollider;
	float playerHeight;
	float playerRadius;
	bool isWalking;
	Vector3 posChange;
	Vector3 lastInteractDir;
	ISelectable selected;

	public bool IsWalking() {
		return isWalking;
	}

	public override void Awake() {
		if(Instance != null)
			Debug.Log("More than one player instance");
		Instance = this;
	}

	new void Start() {
		capsuleCollider = capsule.GetComponent<CapsuleCollider>();
		playerHeight = capsuleCollider.height;
		playerRadius = capsuleCollider.radius;
		gameInput.OnInteractPickUpAction += GameInput_OnInteractPickUpAction;
		//gameInput.OnInteractCutAction += GameInput_OnInteractCutAction;
	}

	private void GameInput_OnInteractPickUpAction(object sender, System.EventArgs e) { // this is handle interact
		bool hasKo = HasKitchenObject();
		if(selected != null) {  // selected is a counter
			if(selected is IInteractPickUp)
				((IInteractPickUp) selected).InteractPickUp(this); // the selected interacts with the player this way the selected's interact will proc
			else if(selected is KitchenObject)
				InteractPickUp(selected);
		} else if(GetKitchenObject() != null) // no selected counter and holding an object, then Throw the object
			DropKitchenObject(GetMoveDelta());
		if(!hasKo && HasKitchenObject()) {  // if the player had no ko but now does then it picked up a ko
			SoundManager.Instance.PlaySound(soundWhenPlayerPickUp, transform.position);
		} else if(hasKo && !HasKitchenObject()) {
			SoundManager.Instance.PlaySound(soundWhenPlayerDrop, transform.position);
		}
	}

	//private void GameInput_OnInteractCutAction(object sender, System.EventArgs e) { // this is handle interact
	//	if(seletedCounter != null)
	//		seletedCounter.InteractCut(this);
	//}

	// Update is called once per frame
	void Update() {
		if(GameHandler.GetGameState() == GameHandler.GameState.GamePlaying) {
			HandleMovement();
			HandleLookAt();
			HandleProgressableAction();
		}
	}

	private void HandleProgressableAction() { // this is handle look
		if(gameInput.IsInteractCut() && selected != null && selected is AbstractKitchenObjectParentProgressable) {
			// if it's a cutting table then when look away stop progressing otherwise keep progressing
			((AbstractKitchenObjectParentProgressable) selected).InteractProgress(this);
		}
	}
	private void HandleLookAt() { // this is handle look
		Vector3 moveDir = GetMoveDirection();
		if(moveDir != Vector3.zero)
			lastInteractDir = moveDir;
		// out keyword defines the variable that it will modify by reference in place.
		if(Physics.CapsuleCast(transform.position + Vector3.up * playerRadius, transform.position + Vector3.up * (playerHeight - playerRadius), playerRadius, transform.forward, out RaycastHit raycastHit, interactDistance, countersLayerMask)) {
			if(raycastHit.transform.TryGetComponent(out ISelectable selectable)) {
				//Debug.Log("This was looked at: " + selectable);
				SetSelected(selectable);
			} else {
				//Debug.Log("No selectable was found");
				SetSelected(null); // no selectable was found
			}
		} else {
			//Debug.Log("nothing was found");
			SetSelected(null); // nothing was found
		}
	}

	private void SetSelected(ISelectable selected) {
		//Debug.Log("1. Looking at: " + selected + ", " + (selected == null? "" : selected.GetType()));
		this.selected = null;
		if(this.selected != selected)
			this.selected = selected;
		OnSelectedCounterChanged?.Invoke(this, new OnSelectedCounterChangedEventArgs {
			selected = selected
		});
	}

	private void HandleMovement() {
		Vector3 moveDir = GetMoveDirection();
		posChange = GetMoveDelta();
		GetComponent<Rigidbody>().position += posChange;
		isWalking = moveDir != Vector3.zero;
		if(isWalking) {
			footstepTimer -= Time.deltaTime;
			if(footstepTimer < 0f) {
				SoundManager.Instance.PlaySound(soundWhenPlayerWalking, transform.position);
				footstepTimer = footstepSoundTimer;
			}
			transform.forward = Vector3.Slerp(transform.forward, moveDir, Time.deltaTime * rotateSpeed);
		}
	}

	private Vector3 GetMoveDelta() {
		return GetMoveDirection() * Time.deltaTime * moveSpeed;
	}

	private Vector3 GetMoveDirection() {
		Vector2 inputVector = gameInput.GetInputVectorNormalized();
		return new Vector3(inputVector.x, 0f, inputVector.y);
	}
}
