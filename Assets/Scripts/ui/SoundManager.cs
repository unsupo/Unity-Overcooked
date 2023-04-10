using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour {
	const string PLAYER_PREFS_SOUND_EFFECTS_VOLUME = "SoundEffectsVolume";

	public static SoundManager Instance { get; private set; }
	[SerializeField] AudioClipRefsSO audioClipRefsSO;
	private float volume = 1f;

	private void Awake() {
		if(Instance != null && Instance != this)
			Destroy(this);
		else
			Instance = this;
		volume = PlayerPrefs.GetFloat(PLAYER_PREFS_SOUND_EFFECTS_VOLUME, 1f);
	}

	public void PlaySound(string clipName, Vector3? position = null, float volumeMultiplier = 1f) {
		PlaySound(audioClipRefsSO.GetAudioClipsByName(clipName), position, volume * volumeMultiplier);
	}

	public void PlaySound(AudioClip audioClip, Vector3? position = null, float volumeMultiplier = 1f) {
		if(position == null)
			position = Camera.main.transform.position;
		AudioSource.PlayClipAtPoint(audioClip, position.Value, volumeMultiplier);
	}
	public void PlaySound(AudioClip[] audioClipArray, Vector3? position = null, float volumeMultiplier = 1f) {
		PlaySound(audioClipArray[Random.Range(0, audioClipArray.Length)], position, volume * volumeMultiplier);
	}

	internal static void ChangeVolume() {
		Instance.volume += .1f;
		if(Instance.volume > 1f)
			Instance.volume = 0f;
		PlayerPrefs.SetFloat(PLAYER_PREFS_SOUND_EFFECTS_VOLUME, Instance.volume);
		PlayerPrefs.Save();
	}

	internal static float GetNormalizedVolume() {
		return (float)System.Math.Round(Instance.volume * 10f);
	}
}
