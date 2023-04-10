using UnityEngine;
using UnityEngine.UI;
using System.ComponentModel;
using UnityEditor;
using System;
using TMPro;
using Unity.VisualScripting;

// Define a custom attribute to specify default values for serialized fields
public class DefaultAttribute : PropertyAttribute {
	public object defaultValue { get; set; }

	public DefaultAttribute() {
		defaultValue = null;
	}

	public DefaultAttribute(Type type, string defaultPath) {
		defaultValue = Resources.Load(defaultPath, type);
	}
}
public class ResultUI : MonoBehaviour {
	[SerializeField, Default(typeof(Sprite), "Resources/unity_builtin_extra/Checkmark")] private Sprite successSprite;
	[SerializeField] private Sprite failureSprite = null;
	[SerializeField] private string successText = "Success";
	[SerializeField] private string failureText = "Failure";
	[SerializeField] private Color backgroundColorSuccess = Color.green;
	[SerializeField] private Color backgroundColorFailure = Color.red;
	[SerializeField] private string textName = "Text";
	[SerializeField] private string imageName = "Image";
	private Image background;
	private TextMeshProUGUI resultText;
	private Image image;

	private void Awake() {
		background = GetComponent<Image>();
		resultText = Utility.ThrowGetChildAndComponent<TextMeshProUGUI>(gameObject, textName);
		image = Utility.ThrowGetChildAndComponent<Image>(gameObject, imageName);
		HideResult();
	}

	public void ShowResult(bool success) {
		Debug.Log("showing result ui: " + success);
		if(success) {
			background.color = backgroundColorSuccess;
			resultText.text = successText;
			image.sprite = successSprite;
		} else {
			background.color = backgroundColorFailure;
			resultText.text = failureText;
			image.sprite = failureSprite;
		}

		gameObject.SetActive(true);
		StartCoroutine(CoroutineUtils.DelaySeconds(HideResult,1));
	}

	public void HideResult() {
		Debug.Log("Hiding result ui");
		gameObject.SetActive(false);
	}
}
