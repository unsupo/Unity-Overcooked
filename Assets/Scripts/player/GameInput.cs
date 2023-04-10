using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameInput : MonoBehaviour {
	public static GameInput Instance { get; private set; }
	public event EventHandler OnInteractPickUpAction;
	public event EventHandler OnPauseAction;
	//public event EventHandler OnInteractCutAction;
	PlayerInputActions playerInputActions;
	private float interactStartHoldTime;
	private float interactHeldTime;
	private static string PLAYER_REBINDS = "rebinds";

	public enum Binding {
		MOVE_UP, MOVE_DOWN, MOVE_LEFT, MOVE_RIGHT, INTERACT, INTERACT_ALTERNATE, PAUSE
	}

	private void Awake() {
		if(Instance != null && Instance != this)
			Destroy(this);
		else
			Instance = this;
		playerInputActions = new PlayerInputActions();
		if(PlayerPrefs.HasKey(PLAYER_REBINDS))
			playerInputActions.LoadBindingOverridesFromJson(PlayerPrefs.GetString(PLAYER_REBINDS));
		playerInputActions.Player.Enable();

		playerInputActions.Player.Interact_Pick_Up.performed += Interact_Pick_Up;
		playerInputActions.Player.Pause.performed += Pause_performed; 
		//playerInputActions.Player.Interact_Cut.performed +=	Interact_Cut;
	}

	private void OnDestroy() {
		playerInputActions.Player.Interact_Pick_Up.performed -= Interact_Pick_Up;
		playerInputActions.Player.Pause.performed -= Pause_performed;
		playerInputActions.Dispose();
	}

	private void Pause_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj) {
		OnPauseAction?.Invoke(this, EventArgs.Empty);
	}

	private void Interact_Pick_Up(UnityEngine.InputSystem.InputAction.CallbackContext obj) {
		OnInteractPickUpAction?.Invoke(this, EventArgs.Empty);
	}

	//private void Interact_Cut(UnityEngine.InputSystem.InputAction.CallbackContext obj) {
	//	OnInteractCutAction?.Invoke(this, EventArgs.Empty);
	//}

	public bool IsInteractPickUp() {
		return playerInputActions.Player.Interact_Pick_Up.IsPressed();
	}

	public bool IsInteractCut() {
		return playerInputActions.Player.Interact_Cut.IsPressed();
	}

	public Vector2 GetInputVector() {
		return playerInputActions.Player.Move.ReadValue<Vector2>();
	}

	public Vector2 GetInputVectorNormalized() {
		return GetInputVector().normalized;
	}

	public float TimeHeldInteractPickUp() {
		return Time.time - interactStartHoldTime;
	}

	private void Update() {
		if(playerInputActions.Player.Interact_Pick_Up.IsPressed())
			interactStartHoldTime = Time.time;
		else
			interactStartHoldTime = 0;

	}

	public class BindingObject {
		public int index = 0;
		public InputAction inputAction;
		public InputBinding inputBinding;
		public BindingObject(InputAction inputAction) {
			Init(inputAction);
		} 
		public BindingObject(InputAction inputAction, int index) {
			this.index = index;
			Init(inputAction);
		}
		void Init(InputAction inputAction) {
			this.inputAction = inputAction;
			this.inputBinding = inputAction.bindings[index];
		}
	}

	public static BindingObject GetBinding(Binding binding) {
		switch(binding) {
			default:
			case Binding.INTERACT:
				return new BindingObject(Instance.playerInputActions.Player.Interact_Pick_Up);
			case Binding.INTERACT_ALTERNATE:
				return new BindingObject(Instance.playerInputActions.Player.Interact_Cut);
			case Binding.PAUSE:
				return new BindingObject(Instance.playerInputActions.Player.Pause);
			case Binding.MOVE_UP:
				return new BindingObject(Instance.playerInputActions.Player.Move,1);
			case Binding.MOVE_DOWN:
				return new BindingObject(Instance.playerInputActions.Player.Move,2);
			case Binding.MOVE_LEFT:
				return new BindingObject(Instance.playerInputActions.Player.Move, 3);
			case Binding.MOVE_RIGHT:
				return new BindingObject(Instance.playerInputActions.Player.Move, 4);
		}
	}

	public static string GetBindingString(Binding binding) {
		string str = GetBinding(binding).inputBinding.ToDisplayString();
		return str.Substring(0, Math.Min(str.Length, 3));
	}

	public static void RebindBinding(Binding binding, Action onActionRebound) {
		Instance.playerInputActions.Player.Disable();
		BindingObject bindingObject = GetBinding(binding);
		Debug.Log("Rebinding: " + GetBindingString(binding));
		bindingObject.inputAction.PerformInteractiveRebinding(bindingObject.index)
			.OnComplete(callback => {
				callback.Dispose(); // TODO can't rebind spacebar for some reason
				PlayerPrefs.SetString(PLAYER_REBINDS, Instance.playerInputActions.SaveBindingOverridesAsJson());
				Instance.playerInputActions.Player.Enable();
				onActionRebound();
			})
			.Start();
	}
}
