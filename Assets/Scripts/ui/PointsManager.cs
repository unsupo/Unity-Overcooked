using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PointsManager : MonoBehaviour {
	public static PointsManager Instance { get; private set; }
	[SerializeField] string progressbarPath = "Base/progressbar/fill";
	[SerializeField] string progressTextPath = "Base/progressbar/multiplier";
	[SerializeField] string scorePath = "Base/Score";
	[SerializeField] int maxScore = 500;

	private int points = 0;
	private float multiplier = 1f;

	private void Awake() {
		if(Instance != null && Instance != this)
			Destroy(this);
		else
			Instance = this;
		SetUI();
		gameObject.SetActive(false);
	}

	public static int GetPoints() {
		return Instance.points;
	}

	public static float GetMultiplier() {
		return Instance.multiplier;
	}
	public static void AddPoints(int points) {
		Instance.points += (int) (points * Instance.multiplier);
		Instance.SetUI();
	}

	public static void setMultiplier(float multiplier) {
		Instance.multiplier = multiplier;
		Instance.SetUI();
	}

	void SetUI() {
		if(Utility.TryGetChildByPath(gameObject, progressbarPath, out GameObject progressBarGO) && progressBarGO.TryGetComponent(out Image image)) {
			image.fillAmount = points / (float) maxScore;
		}
		if(Utility.TryGetChildByPath(gameObject, scorePath, out GameObject scoreGO) && scoreGO.TryGetComponent(out TextMeshProUGUI text)) {
			text.text = points + "";
		}
		if(Utility.TryGetChildByPath(gameObject, progressTextPath, out GameObject progressTextGO) && progressTextGO.TryGetComponent(out TextMeshProUGUI multiplierText)) {
			if(multiplier == 1)
				progressTextGO.SetActive(false);
			else {
				progressTextGO.SetActive(true);
				multiplierText.text = string.Format("TIP x " + multiplier);
			}
		}
	}

}
