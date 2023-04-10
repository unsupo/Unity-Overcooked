using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TimeManager : MonoBehaviour {
	public static TimeManager Instance { get; private set; }

	[SerializeField] float totalTime = 4f * 60; // 4 minutes is default time
	[SerializeField] bool timedAtStart = false; // if this is false then it'll only start counting down timer when you turn in your first order
	[SerializeField] string timeLeftPath = "Timer/timeleft";
	[SerializeField] string progressbarPath = "Timer/progressbar/fill";
	private float timeLeft;
	private bool isTimeStarted;

	private void Awake() {
		if(Instance != null && Instance != this)
			Destroy(this);
		else
			Instance = this;
	}

	// Start is called before the first frame update
	void Start() {
		isTimeStarted = timedAtStart;
		this.timeLeft = totalTime;
		if(Utility.TryGetChildByPath(gameObject, progressbarPath, out GameObject progressbarGO) && progressbarGO.TryGetComponent(out Image fill)) {
			fill.fillAmount = 1;
		}
		if(Utility.TryGetChildByPath(gameObject, timeLeftPath, out GameObject timeleftGO) && timeleftGO.TryGetComponent(out TextMeshProUGUI timeLeft)) {
			timeLeft.text = Utility.FormatTime(this.timeLeft);
		}
	}

	// Update is called once per frame
	void Update() {
		if(isTimeStarted && GameHandler.GetGameState() == GameHandler.GameState.GamePlaying) {
			this.timeLeft -= Time.deltaTime;
		}
		if(Utility.TryGetChildByPath(gameObject, progressbarPath, out GameObject progressbarGO) && progressbarGO.TryGetComponent(out Image fill)) {
			fill.fillAmount = this.timeLeft / totalTime;
		}
		if(Utility.TryGetChildByPath(gameObject, timeLeftPath, out GameObject timeleftGO) && timeleftGO.TryGetComponent(out TextMeshProUGUI timeLeft)) {
			timeLeft.text = Utility.FormatTime(this.timeLeft);
		}
	}

	public static void StartTiming() {
		Instance.isTimeStarted = true;
	}

	public static bool isTimerStopped() {
		return Instance.isTimeStarted && Instance.timeLeft <= 0;
	}
}
