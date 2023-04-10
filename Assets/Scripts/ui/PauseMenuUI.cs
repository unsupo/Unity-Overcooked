using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PauseMenuUI : MonoBehaviour {
	public static PauseMenuUI Instance { get; private set; }
	[SerializeField] string resumeButtonName = "Pause/ResumeButton";
	[SerializeField] string mainMenuButtonName = "Pause/MainMenuButton";
	[SerializeField] string settingsButtonName = "Pause/Settings";
	Button resumeButton;

	private static List<Action> hideCallBacks = new List<Action>();

	private void Awake() {
		if(Instance != null && Instance != this)
			Destroy(this);
		else
			Instance = this;
		resumeButton = Utility.ThrowGetChildAndComponentByPath<Button>(gameObject, resumeButtonName);
		resumeButton.onClick.AddListener(() => Hide(true));
		Utility.ThrowGetChildAndComponentByPath<Button>(gameObject, mainMenuButtonName).onClick.AddListener(() => Loader.Load(Loader.Scene.MainMenuScene));
		Utility.ThrowGetChildAndComponentByPath<Button>(gameObject, settingsButtonName).onClick.AddListener(() => { Hide(); SettingsUI.Show(()=>Show()); });
		GameInput.Instance.OnInteractPickUpAction += Instance_OnInteractPickUpAction;
	}

	private void Instance_OnInteractPickUpAction(object sender, System.EventArgs e) {
		ExecuteEvents.Execute(EventSystem.current.currentSelectedGameObject, new PointerEventData(EventSystem.current), ExecuteEvents.pointerClickHandler);
	}
	private void Start() {
		Hide(false);
		SettingsUI.Hide();
		resumeButton.Select();
	}

	public static void ToggleActive() { // TODO repeated code maybe a toast ui parent of some kind
		Instance.gameObject.SetActive(!Instance.gameObject.activeSelf);
	}
#nullable enable
	public static void Show(Action? onHide = null) {
		if(onHide != null)
			hideCallBacks.Add(onHide);
		Instance.gameObject.SetActive(true);
		Utility.ThrowGetChildAndComponentByPath<Button>(Instance.gameObject, Instance.resumeButtonName).Select();

	}
	public static void Hide(bool isCallBacks = false) {
		if(isCallBacks) {
			List<Action> actions = new List<Action>(hideCallBacks);
			hideCallBacks.Clear();
			foreach(Action action in actions)
				action();
		}
		Instance.gameObject.SetActive(false);
	}
}
