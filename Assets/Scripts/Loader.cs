using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class Loader {
	public enum Scene {
		MainMenuScene,
		GameScene,
		LoadingScene
	}

	public static Scene targetScene;

	public static void Load(Scene targetScene) {
		Loader.targetScene = targetScene;
		SceneManager.LoadScene(Scene.LoadingScene.ToString());
	}

	internal static void LoaderCallback() {
		Debug.Log("Loading target: " + targetScene);
		SceneManager.LoadScene(targetScene.ToString());
	}
}
