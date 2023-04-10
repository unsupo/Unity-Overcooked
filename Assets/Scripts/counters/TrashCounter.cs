using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrashCounter : MonoBehaviour, ISelectable, IInteractPickUp {
	[SerializeField] private string soundWhenTrash;
	public bool CanMove { get; set; }
	public List<KitchenObjectSO> CanInteractWithThese { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }

	public bool InteractPickUp(ISelectable interacting) {
		Debug.Log("Trashing: " + this + ", " + interacting);
		// must be a player to trigger the trash can so that throw items don't get trashed accidently
		if(interacting is Player) {
			if(((Player) interacting).Trash()) // the player's trash with handle it's child's trash
				SoundManager.Instance.PlaySound(soundWhenTrash, transform.position);
		}
		return true;
	}
}
