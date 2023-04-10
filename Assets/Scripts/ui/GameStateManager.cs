using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class GameStateManager : MonoBehaviour { // confusing name i know
	public static GameStateManager Instance { get; private set; }
	[SerializeField] float readyTime = 5f;
	[SerializeField] float goTime = 5f;
	List<float> timers = new List<float>();
	List<States> states = new List<States>();
	List<bool> isActiveStates = new List<bool>();

	public enum States { ready, go, timesup }

	private void Awake() {
		if(Instance != null && Instance != this)
			Destroy(this);
		else
			Instance = this;
		SetInactive();
	}

	private void SetInactive() {
		foreach(Transform child in GetComponentInChildren<Transform>())
			child.gameObject.SetActive(false);
	}

	private void SetActive(States state, bool isActive = true) {
		foreach(Transform child in GetComponentInChildren<Transform>())
			if(child.name.Equals(state.ToString())) 
				child.gameObject.SetActive(isActive);
	}

	public static void AddImageTime(States state, float time = 5f) {
		Instance.timers.Add(time);
		Instance.states.Add(state);
		Instance.isActiveStates.Add(false);
	}

	// Update is called once per frame
	void Update() {
		if(timers.Count > 0) {
			if(timers[0] > 0) {
				if(!isActiveStates[0])
					SetActive(states[0]);
				timers[0] -= Time.deltaTime;
			} else {
				SetActive(states[0], false);
				timers.RemoveAt(0);
				states.RemoveAt(0);
				isActiveStates.RemoveAt(0);
			}
		}
	}
}
