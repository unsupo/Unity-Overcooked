using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingsUI : MonoBehaviour {
	public static SettingsUI Instance { get; private set; }
	[SerializeField] string soundEffectsButtonName = "SoundEffectsButton";
	[SerializeField] string soundEffectsButtonTextPath = "SoundEffectsButton/SoundEffects";
	[SerializeField] string musicButtonName = "MusicButton";
	[SerializeField] string musicButtonTextPath = "MusicButton/Music";
	[SerializeField] string GameInputPath = "GameInput";
	[SerializeField] string closeButtonName = "CloseButton";
	[SerializeField] string buttonTextName = "Text";
	[SerializeField] string rebindName = "RebindUI";
	private TextMeshProUGUI soundsEffectsButtonText;
	private TextMeshProUGUI musicButtonText;
	Button soundEffectsButton;

	private static List<Action> hideCallBacks = new List<Action>();

	private void Awake() {
		if(Instance != null && Instance != this)
			Destroy(this);
		else
			Instance = this;
		soundEffectsButton = Utility.ThrowGetChildAndComponent<Button>(gameObject, soundEffectsButtonName);
		soundEffectsButton.onClick.AddListener(() => { SoundManager.ChangeVolume(); UpdateVisual(); });
		soundEffectsButton.Select();
		Utility.ThrowGetChildAndComponent<Button>(gameObject, musicButtonName).onClick.AddListener(() => { MusicManager.ChangeVolume(); UpdateVisual(); });
		Utility.ThrowGetChildAndComponent<Button>(gameObject, closeButtonName).onClick.AddListener(() => Hide(true));
		soundsEffectsButtonText = Utility.ThrowGetChildAndComponentByPath<TextMeshProUGUI>(gameObject, soundEffectsButtonTextPath);
		musicButtonText = Utility.ThrowGetChildAndComponentByPath<TextMeshProUGUI>(gameObject, musicButtonTextPath);
		foreach(Transform child in Utility.ThrowGetChildByPath(gameObject, GameInputPath).GetComponentInChildren<Transform>())
			if(child.name.EndsWith("Button") && child.TryGetComponent(out Button button)) {
				Debug.Log("adding listener button: " + child.name);
				button.onClick.AddListener(() => {
					string name = Utility.ToSnakeCase(child.name.Replace("Button", ""));
					ShowPressToRebindKey(name.Replace("_", " "));
					Debug.Log("firing listener button: " + name);
					GameInput.RebindBinding(Enum.Parse<GameInput.Binding>(name.ToUpper()), () => {
						Debug.Log("rebinding button: " + name);
						HidePressToRebindKey();
						UpdateVisual();
					});
				});
			}
		Hide();
		HidePressToRebindKey();
	}

	private void HidePressToRebindKey() {
		Utility.ThrowGetChildByPath(gameObject, rebindName).SetActive(false);
	}

	private void ShowPressToRebindKey(string buttonName) {
		GameObject rebindUI = Utility.ThrowGetChildByPath(gameObject, rebindName);
		rebindUI.SetActive(true);
		Utility.ThrowGetChildAndComponent<TextMeshProUGUI>(rebindUI, buttonTextName).text = "Press a key to rebind <KEY> to...".Replace("<KEY>",buttonName);
	}

	private void Start() {
		GameInput.Instance.OnPauseAction += GameInput_OnPauseAction;
		UpdateVisual();
	}

	private void GameInput_OnPauseAction(object sender, System.EventArgs e) {
		Hide();
	}

	void UpdateVisual() {
		soundsEffectsButtonText.text = "Sound Effects: " + SoundManager.GetNormalizedVolume();
		musicButtonText.text = "Music: " + MusicManager.GetNormalizedVolume();
		foreach(Transform child in Utility.ThrowGetChildByPath(gameObject, GameInputPath).GetComponentInChildren<Transform>())
			if(child.name.EndsWith("Button"))
				Utility.ThrowGetChildAndComponent<TextMeshProUGUI>(child.gameObject, buttonTextName).text = GameInput.GetBindingString(Enum.Parse<GameInput.Binding>(Utility.ToSnakeCase(child.name.Replace("Button", "")).ToUpper()));
	}

	public static void ToggleActive() { // TODO repeated code maybe a toast ui parent of some kind
		Instance.gameObject.SetActive(!Instance.gameObject.activeSelf);
	}
#nullable enable
	public static void Show(Action? onHide = null) {
		if(onHide != null)
			hideCallBacks.Add(onHide);
		Instance.gameObject.SetActive(true);
		Instance.soundEffectsButton.Select();
	}
	public static void Hide(bool isCallBacks = false) {
		Debug.Log("Hiding settings");
		if(isCallBacks) {
			List<Action> actions = new List<Action>(hideCallBacks);
			hideCallBacks.Clear();
			foreach(Action action in actions)
				action();
		}
		hideCallBacks.Clear();
		Instance.gameObject.SetActive(false);
	}
}
