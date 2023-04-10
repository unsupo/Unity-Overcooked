using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stoveable : ChangeableObject { 
	public enum StovableState {
		OnStoveEmpty, OnStoveCooking, OffStove, OnStoveBurned
	}
	public StovableState StoveState { get; set; }
	public enum BurnableState {
		None, Early, Medium, Late, Burned
	}
	public BurnableState BurnState { get; set; }
}
