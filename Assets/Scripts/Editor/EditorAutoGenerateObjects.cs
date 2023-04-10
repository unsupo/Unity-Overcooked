using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Windows;

[CustomEditor(typeof(GenerateObjects))]
[CanEditMultipleObjects]
public class EditorAutoGenerateObjects : Editor {
	void OnEnable() { }
	public override void OnInspectorGUI() {
		base.OnInspectorGUI();
		GenerateObjects generateObjects = (GenerateObjects) target;
		if(GUILayout.Button("Generate Prefab")) {
			generateObjects.CreatePrefabVariant();
			//Debug.Log(target);
		}
	}
}
