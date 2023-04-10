using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Notes, non cuttable objects can be placed
 */
public class CuttingCounter : AbstractKitchenObjectParentProgressable {
	[SerializeField] string soundNameWhenCut;

	public override void Start() {
		RequireHoldPoint();
		Player.Instance.OnSelectedCounterChanged += Player_OnSelectedCounterChanged;
	}
	// this goes here because AbstractKitchenObjectParentProgressable can include cooking and cutting and cooking doesnt' stop when you look away
	private void Player_OnSelectedCounterChanged(object sender, Player.OnSelectedCounterChangedEventArgs e) {
		// if it's progressing and then you look away from this cutting counter then stop cutting
		if(e.selected != (ISelectable) this)
			shouldProgress = false;
	}
	// this goes here too because you don't interact to start cooking, however it might work for dishwashing to move it up
	public override void InteractProgress(Player player) {
		IfProgressableTypeThen((CuttableObject cuttableObject) => {
			CanMove = false; StartProgress(cuttableObject.ChangeProgressMax);
		});
	}
	protected override void ActionOnIncrementProgress() {
		SoundManager.Instance.PlaySound(soundNameWhenCut, transform.position);
		RaiseOnPlayerCutEvent();
	}
	public override void Update() {
		base.Update();
		// this must be in update because we don't want the player to have to press the button to finish cutting
		IfProgressableTypeThen((CuttableObject cuttableObject) => {
			if(GetProgress() >= cuttableObject.ChangeProgressMax) {
				CanMove = true;
				shouldProgress = false;
				GetKitchenObject().DestroySelf(); // unfortunately order matters here.  If this goes second then it will clear the new spawned object from the parent
				KitchenObject.SpawnKitchenObject(cuttableObject.KitchenObjectSO, this);
			}
		});
	}
}
