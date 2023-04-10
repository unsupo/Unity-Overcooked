using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour {
	[SerializeField] string playButtonName = "PlayButton";
	[SerializeField] string quitButtonName = "QuitButton";
	[SerializeField] string settingsButtonName = "Settings";
	Button playButton;
	Button quitButton;

	private void Awake() {
		playButton = Utility.ThrowGetChildAndComponent<Button>(gameObject, playButtonName);
		quitButton = Utility.ThrowGetChildAndComponent<Button>(gameObject, quitButtonName);
		playButton.onClick.AddListener(() => Loader.Load(Loader.Scene.GameScene));
		quitButton.onClick.AddListener(() => Application.Quit());
		Time.timeScale = 1f; // just in case we are coming here from pause
		Utility.ThrowGetChildAndComponent<Button>(gameObject, settingsButtonName).onClick.AddListener(() => SettingsUI.Show(() => playButton.Select()));
	}
	private void Start() {
		playButton.Select();
		GameInput.Instance.OnInteractPickUpAction += Instance_OnInteractPickUpAction;
	}
	private void Instance_OnInteractPickUpAction(object sender, System.EventArgs e) {
		ExecuteEvents.Execute(EventSystem.current.currentSelectedGameObject, new PointerEventData(EventSystem.current), ExecuteEvents.pointerClickHandler);
	}
}
