using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Stackable : MonoBehaviour {
	[SerializeField] List<Transform> canStackOnTopOf = new List<Transform>();
}

