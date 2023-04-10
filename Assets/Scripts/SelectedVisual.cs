using UnityEngine;

/**
 * Attach this script to a game object to display a visual if you look at it
 */
public class SelectedVisual : MonoBehaviour {
	ISelectable selected;
	[SerializeField] Material selectedMaterial;
	[SerializeField] Vector3 scale = new Vector3(1.01f, 1.01f, 1.01f);
	[SerializeField] GameObject visualGameObject;
	GameObject selectedGameObject;
	public bool IsSelected { get; set; }

	public void SetVisualGameObject(GameObject visualGameObject) {
		this.visualGameObject = visualGameObject;
	}

	// Start is called before the first frame update
	void Start() {
		Player.Instance.OnSelectedCounterChanged += Player_OnSelectedCounterChanged;
		if(!visualGameObject)
			foreach(Transform i in this.GetComponentInChildren<Transform>())
				if(i.name.EndsWith("_Visual")) {
					visualGameObject = i.gameObject;
					break;
				}
		if(!visualGameObject)
			Debug.LogError("No child object ending in _Visual for: " + this);
		selected = this.GetComponent<ISelectable>();
		//Debug.Log(visualGameObject + ", " + visualGameObject.transform + ", " + visualGameObject.transform.parent);
		//selected = visualGameObject.transform.parent.GetComponent<ISelectable>();
		selectedGameObject = Instantiate(visualGameObject);
		Destroy(selectedGameObject.GetComponent<SelectedVisual>()); // this is to prevent an endless loop of creating selected counter visual objects
		selectedGameObject.name = visualGameObject.name + "_selected";
		selectedGameObject.transform.parent = this.transform;
		selectedGameObject.transform.position = this.transform.position;
		selectedGameObject.transform.rotation = this.transform.rotation;
		selectedGameObject.transform.localScale = scale;
		foreach(Transform i in selectedGameObject.GetComponentInChildren<Transform>())
			if(i.GetComponent<MeshRenderer>() != null)
				i.GetComponent<MeshRenderer>().material = selectedMaterial;
		selectedGameObject.SetActive(false);
		if(TryGetComponent(out AbstractKitchenObjectParent parent))
			parent.AddGameObjectVisual(selectedGameObject.transform);
	}

	private void Player_OnSelectedCounterChanged(object sender, Player.OnSelectedCounterChangedEventArgs e) {
		//Debug.Log("Event triggered: " + e.selected + ", " + selected);
		if(e.selected == selected)
			Show();
		else
			Hide();
	}

	private void Show() {
		//Debug.Log("Looking at: " + selected);
		selectedGameObject.SetActive(true);
		IsSelected = true;
	}

	private void Hide() {
		//Debug.Log("NOT Looking at: " + selected);
		if(selectedGameObject == null) // does destroyed objects continue to loop through this
			Debug.Log("selectedGameObject is null: " + this + ", " + selectedGameObject);
		else
			selectedGameObject.SetActive(false);
		IsSelected = false;
	}

	private void OnDestroy() { // must destroy any deleted gameobjects event handlers for performance
		Player.Instance.OnSelectedCounterChanged -= Player_OnSelectedCounterChanged;
	}
}
