using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialUI : MonoBehaviour {
	public static TutorialUI Instance { get; private set; }
	[SerializeField] private string progressBarFillPath = "skipContainer/progressbar/fill";
	[SerializeField] private float interactPickUpTimerMax = 5f;
	[SerializeField] private float totalTimerMax = 5f;
	private float interactPickUpTimer;
	private float totalTimer;

	private bool isStarted = true;

	internal static bool IsStarted() {
		return Instance.isStarted;
	}

	private void Awake() {
		if(Instance != null && Instance != this)
			Destroy(this);
		else
			Instance = this;
	}

	private void Start() {
		UpdateProgressBar(0);
		totalTimer = totalTimerMax;
	}

	private void Update() {
		//Debug.Log("Tutorial: " + GameHandler.Instance.gameInput.IsInteractCut() + ", " + GameHandler.Instance.gameInput.IsInteractPickUp());
		totalTimer -= Time.deltaTime;
		if(GameHandler.Instance.gameInput.IsInteractPickUp())
			interactPickUpTimer += Time.deltaTime;
		else 
			interactPickUpTimer -= Time.deltaTime;
		if(interactPickUpTimer > interactPickUpTimerMax)
			interactPickUpTimer = interactPickUpTimerMax;
		if(interactPickUpTimer < 0)
			interactPickUpTimer = 0;
		UpdateProgressBar(interactPickUpTimer / interactPickUpTimerMax);
		if(interactPickUpTimer >= interactPickUpTimerMax || totalTimer <= 0) {
			gameObject.SetActive(false);
			isStarted = false;
		}
	}

	private void UpdateProgressBar(float fillAmount) {
		if(Utility.TryGetChildByPath(gameObject, progressBarFillPath, out GameObject fillGameObject) && fillGameObject.TryGetComponent(out Image fill)) {
			//Debug.Log("Setting progressbar: "+fillGameObject+", "+fill+", "+fillAmount);
			fill.fillAmount = fillAmount;
		}
	}
}
