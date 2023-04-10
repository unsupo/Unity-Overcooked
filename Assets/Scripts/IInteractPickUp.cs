using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInteractPickUp {
	/**
	 * This object is interacting with another object
	 */
	public abstract bool InteractPickUp(ISelectable interacted);
	public bool CanMove { get; set; }
}
