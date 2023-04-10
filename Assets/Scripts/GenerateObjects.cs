using UnityEngine;
using System.Collections;
using UnityEngine.Windows;
using UnityEditor;
using Unity.VisualScripting;
using System.Text.RegularExpressions;
using System;
using System.IO;
using UnityEditor.VersionControl;

[System.Serializable]
public class GenerateObjects : MonoBehaviour {
	[SerializeField] GameObject baseKitchenObjectPrefab;
	[SerializeField] string inputKitchenObjectsVisualsPath = "_Assets/PrefabsVisuals/KitchenObjectsVisuals";
	[SerializeField] string outputKitchenObjectsPrefabPath = "/Prefabs/KitchenObjects/";
	[SerializeField] string inputSpriteIconPath = "_Assets/Textures/Icons/";
	[SerializeField] string outputKitchenObjectsSOPath = "Assets/Resources/ScriptableObjects/KitchenObjectSO/";
	[SerializeField] GameObject baseContainerCounterPrefab;
	[SerializeField] string outputCountersPrefabPath = "/Prefabs/Counters/";

	string assetFolder = Application.dataPath;
	// Assets/Prefabs/KitchenObjects
	public void CreatePrefabVariant() {
		foreach(UnityEngine.Object f in Resources.LoadAll(inputKitchenObjectsVisualsPath)) {
			GameObject objectVisual = (GameObject) f;
			GameObject visual = PrefabUtility.InstantiatePrefab(objectVisual) as GameObject;
			string name = Regex.Replace(objectVisual.name, "_.*", "");
			// create kitchen object prefab
			string createdPrefabPath = assetFolder + outputKitchenObjectsPrefabPath + name + ".prefab";
			GameObject baseKitchenObjectInstance = PrefabUtility.InstantiatePrefab(baseKitchenObjectPrefab) as GameObject;
			visual.transform.SetParent(baseKitchenObjectInstance.transform, false);
			// set kitchen object prefabs visual
			baseKitchenObjectInstance.GetComponent<SelectedVisual>().SetVisualGameObject(objectVisual);
			// create kitchen object so
			KitchenObjectSO so = ScriptableObject.CreateInstance<KitchenObjectSO>();
			so.objectName = name;
			so.sprite = Resources.Load<Sprite>(inputSpriteIconPath + so.objectName);
			// create counter prefab
			string createdCounterPrefabPath = assetFolder + outputCountersPrefabPath + "ContainerCounter_"+name + ".prefab";
			GameObject baseCounterInstance = PrefabUtility.InstantiatePrefab(baseContainerCounterPrefab) as GameObject;
			// set counter prefab visual
			Transform counterVisual = baseCounterInstance.transform.GetChild(1); // assuming that the visual is always the second item here
			Transform singleDoor = counterVisual.GetChild(2); // assuming the door is always the 3rd item
			singleDoor.GetChild(0).GetComponent<SpriteRenderer>().sprite = so.sprite;
			AssetDatabase.CreateAsset(so, outputKitchenObjectsSOPath + so.objectName + ".asset");
			// set kitchen object prefabs so
			baseKitchenObjectInstance.GetComponent<KitchenObject>().SetKitchenObjectSO(so);
			baseCounterInstance.GetComponent<ContainerCounter>().SetKitchenObjectSO(so);
			// save kitchen object prefab set the so's prefab to kitche object prefab and clean up
			GameObject createPrefab = PrefabUtility.SaveAsPrefabAssetAndConnect(baseKitchenObjectInstance, createdPrefabPath, InteractionMode.AutomatedAction);
			PrefabUtility.SaveAsPrefabAssetAndConnect(baseCounterInstance, createdCounterPrefabPath, InteractionMode.AutomatedAction);
			so.prefab = createPrefab.transform;

			EditorUtility.SetDirty(so);
			DestroyImmediate(baseKitchenObjectInstance);
			DestroyImmediate(baseCounterInstance);
		}

		AssetDatabase.SaveAssets();
		AssetDatabase.Refresh();
		AssetDatabase.AllowAutoRefresh();
	}
}

