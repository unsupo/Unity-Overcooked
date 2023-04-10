using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameHandler : MonoBehaviour {
	public static GameHandler Instance { get; private set; }

	[SerializeField] float waitingToStartTimer = 1f;
	[SerializeField] float countdownToStartTimer = 3f;
	[SerializeField] public GameInput gameInput;
	bool isNewGameState = false;
	bool isGamePlaying = true;

	public enum GameState {
		Tutotrial,
		WaitingToStart,
		CountdownToStart,
		GamePlaying,
		GameOver
	}

	public static GameState GetGameState() => Instance.gameState;

	private GameState gameState;

	private void Awake() {
		if(Instance != null && Instance != this)
			Destroy(this);
		else
			Instance = this;
		gameState = (GameState) 0;
	}

	private void Start() {
		gameInput.OnPauseAction += GameInput_OnPauseAction;
		SetGameObjects(false);
	}

	private void GameInput_OnPauseAction(object sender, EventArgs e) {
		TogglePauseGame();
	}

	private void TogglePauseGame() {
		if(gameState != GameState.GamePlaying)
			return;
		SetPause(isGamePlaying);
	}

	private void SetPause(bool isPaused) {
		if(isPaused) {
			Time.timeScale = 0f;
			isGamePlaying = false;
			SetMusic(false);
			PauseMenuUI.Show(() => SetPause(false));
		} else {
			Time.timeScale = 1f;
			isGamePlaying = true;
			SetMusic(true);
			PauseMenuUI.Hide();
		}
	}

	private void Update() {
		switch(gameState) {
			case GameState.Tutotrial:
				if(isNewGameState)
					TutorialUI.Instance.gameObject.SetActive(true);
				isNewGameState = false;
				if(!TutorialUI.IsStarted()) {
					TutorialUI.Instance.gameObject.SetActive(false);
					gameState = GameState.WaitingToStart;
					isNewGameState = true;
				}
				break;
			case GameState.WaitingToStart:
				if(isNewGameState)
					GameStateManager.AddImageTime(GameStateManager.States.ready, waitingToStartTimer);
				waitingToStartTimer -= Time.deltaTime;
				isNewGameState = false;
				if(waitingToStartTimer < 0f) {
					gameState = GameState.CountdownToStart;
					isNewGameState = true;
				}
				break;
			case GameState.CountdownToStart:
				if(isNewGameState)
					GameStateManager.AddImageTime(GameStateManager.States.go, countdownToStartTimer);
				countdownToStartTimer -= Time.deltaTime;
				isNewGameState = false;
				if(countdownToStartTimer < 0f) {
					isNewGameState = true;
					gameState = GameState.GamePlaying;
				}
				break;
			case GameState.GamePlaying:
				if(isNewGameState)
					SetGameObjects(true);
				isNewGameState = false;
				if(TimeManager.isTimerStopped()) {
					gameState = GameState.GameOver;
					isNewGameState = true;
				}
				break;
			case GameState.GameOver:
				if(isNewGameState)
					GameStateManager.AddImageTime(GameStateManager.States.timesup);
				SoundManager.Instance.gameObject.SetActive(false);
				break;
		}
		//Debug.Log("Gamxestate: " + gameState);
	}

	private void SetGameObjects(bool isActive) {
		TimeManager.Instance.gameObject.SetActive(isActive);
		PointsManager.Instance.gameObject.SetActive(isActive);
		SoundManager.Instance.gameObject.SetActive(isActive);
	}
	private void ToggleMusic() {
		SoundManager.Instance.gameObject.SetActive(!SoundManager.Instance.gameObject.activeSelf);
	}
	private void SetMusic(bool isActive) {
		//SoundManager.Instance.gameObject.SetActive(isActive); // probably should pause music so it doesn't start over
		AudioSource audioSource = SoundManager.Instance.GetComponent<AudioSource>();
		if(!isActive)
			audioSource.Pause();
		else
			audioSource.Play();
	}

}
