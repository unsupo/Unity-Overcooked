using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProgressBarUI : MonoBehaviour {
	[SerializeField] string barImageName = "Bar";
	Image bar;

	private Image GetBar() {
		if(!bar) {
			foreach(Transform i in gameObject.GetComponentInChildren<Transform>()) {
				if(i.name == barImageName) {
					bar = i.gameObject.GetComponent<Image>();
					break;
				}
			}
			if(!bar)
				Debug.LogError("Can't find bar image on progress bar ui: " + this);
		}
		return bar;
	}

	public void SetProgress(float progressNormalized) {
		if(progressNormalized > 0 && progressNormalized < 1) {
			gameObject.SetActive(true);
			GetBar().fillAmount = progressNormalized;
		} else
			gameObject.SetActive(false);
	}
}
