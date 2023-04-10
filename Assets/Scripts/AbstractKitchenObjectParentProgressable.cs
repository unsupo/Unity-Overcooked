using UnityEngine;
using System.Collections;
using System;

public abstract class AbstractKitchenObjectParentProgressable : AbstractKitchenObjectParent {
	[SerializeField] ProgressBarUI progressBarUI;
	[SerializeField] Vector3 uiRelativePosition = new Vector3(0, 2.5f, 0);
	[SerializeField] float delaySeconds = .25f;
	GameObject progressBarUICanvas;
	ProgressBarUI spawnedProgressBarUI;
	bool isWaiting = false;
	float max;
	protected bool shouldProgress = false;
	int progress;
	float normalizedProgress;


	internal void SetProgress(int progress) {
		this.progress = progress;
		if(max > 0)
			SetNormalizedProgress(progress / max);
	}
	internal int GetProgress() {
		return progress;
	}

	public override void Awake() {
		base.Awake();
		if(progressBarUICanvas == null) {
			Debug.Log("Creating progress bar");
			progressBarUICanvas = Instantiate(progressBarUI.gameObject);
			spawnedProgressBarUI = progressBarUICanvas.GetComponent<ProgressBarUI>();
			spawnedProgressBarUI.SetProgress(progress);
			progressBarUICanvas.transform.SetParent(transform);
			progressBarUICanvas.transform.localPosition = uiRelativePosition;
			progressBarUICanvas.gameObject.SetActive(false);
			//progressBarUICanvas.transform.localRotation = transform.localRotation; // don't change rotation
		}
	}
	public virtual void Update() {
		if(!isWaiting && shouldProgress)
			StartCoroutine(_IncrementProgress());
	}
	public void SetNormalizedProgress(float normalizedProgress) {
		this.normalizedProgress = normalizedProgress;
		spawnedProgressBarUI.SetProgress(normalizedProgress);
	}
	internal float GetNormalizedProgress() {
		return normalizedProgress;
	}
	protected void StartProgress(float max) {
		this.max = max;
		shouldProgress = true;
	}
	protected void SetVisible(bool isVisible) {
		progressBarUICanvas.SetActive(isVisible);
	}
	protected virtual void ActionOnIncrementProgress() {
		// default do nothing
	}
	IEnumerator _IncrementProgress() {
		isWaiting = true;
		ActionOnIncrementProgress();
		spawnedProgressBarUI.SetProgress(++progress / max);
		yield return new WaitForSeconds(delaySeconds);
		isWaiting = false;
	}
	#nullable enable
	protected void IfProgressableTypeThen<T>(Action<T> f, Action? elseAction = null) {
		if(HasKitchenObject())
			if(GetKitchenObject().TryGetComponent(out T progressableObject))
				f(progressableObject);
			else if(elseAction != null)
				elseAction();
	}
}

