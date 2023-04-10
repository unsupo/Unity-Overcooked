using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour {
	const string PLAYER_PREFS_MUSIC_VOLUME = "MusicVolume";
	public static MusicManager Instance { get; private set; }
	private AudioSource audioSource;
	private float volume = .3f;

	private void Awake() {
		if(Instance != null && Instance != this)
			Destroy(this);
		else
			Instance = this;
		audioSource = GetComponent<AudioSource>();
		volume = PlayerPrefs.GetFloat(PLAYER_PREFS_MUSIC_VOLUME, .3f);
		audioSource.volume = volume;
	}


	internal static void ChangeVolume() {
		Instance.volume += .1f;
		if(Instance.volume > 1f)
			Instance.volume = 0f;
		Instance.audioSource.volume = Instance.volume;
		PlayerPrefs.SetFloat(PLAYER_PREFS_MUSIC_VOLUME, Instance.volume);
		PlayerPrefs.Save();
	}

	internal static float GetNormalizedVolume() {
		return (float) System.Math.Round(Instance.volume * 10f);
	}
}
