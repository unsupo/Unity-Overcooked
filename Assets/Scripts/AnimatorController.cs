using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

/*
 * This goes on the parent
 */
public class AnimatorController : MonoBehaviour {
	enum TriggerOn {
		PickUp, Cut
	}
	//const string OPEN_CLOSE = "OpenClose"; // name of animation to activate
	[SerializeField] TriggerOn triggerOn;
	[SerializeField] string animationName;
	List<Animator> animators;

	private void Awake() {
		if(TryGetComponent<AbstractKitchenObjectParent>(out AbstractKitchenObjectParent parent)) {
			switch(triggerOn) {
				case TriggerOn.PickUp:
					parent.OnPlayerGrabbedObject += ContainerCounter_OnPlayerGrabbedObject;
					break;
				case TriggerOn.Cut:
					parent.OnPlayerCutObject += Parent_OnPlayerCutObject;
					break;
			}
		} else
			Debug.LogError("No AbstractKitchenObjectParent component found: " + this);
	}

	private void Parent_OnPlayerCutObject(object sender, EventArgs e) {
		foreach(Animator animator in GetAnimators())
			animator.SetTrigger(animationName);
	}

	private void ContainerCounter_OnPlayerGrabbedObject(object sender, EventArgs e) {
		foreach(Animator animator in GetAnimators())
			animator.SetTrigger(animationName);
	}

	private List<Animator> GetAnimators() {
		if(animators == null) {
			animators = new List<Animator>();
			if(TryGetComponent<AbstractKitchenObjectParent>(out AbstractKitchenObjectParent parent)) {
				foreach(Transform transform in parent.GetGameObjectVisual())
					if(transform.gameObject.TryGetComponent<Animator>(out Animator animator))
						this.animators.Add(animator);
				if(animators.Count == 0)
					Debug.LogError("No animator on parent: " + parent + ", visual game object: " + parent.GetGameObjectVisual());
			} else
				Debug.LogError("No AbstractKitchenObjectParent component found: " + this);
		}
		return animators;
	}
}

