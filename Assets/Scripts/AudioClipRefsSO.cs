using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;


[CreateAssetMenu()]
public class AudioClipRefsSO : ScriptableObject {
	// TODO could have done another resource lookup like i did for recipes so you don't have to add it manually name SFDX_name01
	public List<NameAudioClip> nameAudioClips = new List<NameAudioClip>();
	Dictionary<string, AudioClip[]> audioClips;

	[Serializable]
	public class NameAudioClip {
		public string name;
		public AudioClip[] audioClip;
	}
	public Dictionary<string, AudioClip[]> GetAudioClipsDict() {
		if(audioClips == null) {
			audioClips = new Dictionary<string, AudioClip[]>();
			foreach(NameAudioClip nameAudioClip in nameAudioClips)
				audioClips.Add(nameAudioClip.name, nameAudioClip.audioClip);
		}
		return audioClips;
	}

	public AudioClip[] GetAudioClipsByName(string name) {
		return GetAudioClipsDict()[name];
	}
}

