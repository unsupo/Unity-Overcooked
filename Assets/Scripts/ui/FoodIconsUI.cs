using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FoodIconsUI : MonoBehaviour {
	[SerializeField] GameObject iconTemplate;
	[SerializeField] string iconImageName = "Icon";
	Image icon;
	List<GameObject> iconTemplates = new List<GameObject>();

	private void Awake() {
		if(!iconTemplate)
			throw new System.Exception("No iconTemplate set for: " + this);
		if(iconTemplates.Count == 0)
			iconTemplate.SetActive(false);
	}

	private static GameObject GetChild(GameObject parent, string name) {
		foreach(Transform i in parent.GetComponentInChildren<Transform>())
			if(i.name == name)
				return i.gameObject;
		Debug.LogError("Can't find bar image on progress bar ui: " + parent);
		return default;
	}

	public bool SetSprites(Sprite sprite) {
		Debug.Log("Set sprite: " + this + ", " + sprite);
		// duplicate the iconTemplate set the sprite make active and add it to iconTemplates
		if(!iconTemplate)
			return false;
		Debug.Log("creating icon template for: " + this);
		GameObject createdIconTemplate = Instantiate(iconTemplate) as GameObject;
		GetChild(createdIconTemplate, iconImageName).GetComponent<Image>().sprite = sprite;
		createdIconTemplate.transform.SetParent(gameObject.transform);
		createdIconTemplate.transform.localPosition = Vector3.zero;
		createdIconTemplate.SetActive(true);
		iconTemplates.Add(createdIconTemplate);
		return true;
	}
	public void SetSprites(List<Sprite> sprites) {
		foreach(Sprite sprite in sprites)
			SetSprites(sprite);
	}
	public void ClearSprites() {
		foreach(GameObject iconTemplate in iconTemplates)
			DestroyImmediate(iconTemplate);
	}
}
