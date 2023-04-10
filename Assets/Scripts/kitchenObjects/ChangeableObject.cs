using UnityEngine;
using System.Collections;

public class ChangeableObject : MonoBehaviour {
	public enum ProgressMoveBehavior{
		PauseWhenMoved, ResetWhenMoved
	}
	public enum VisibleMoveBehavior {
		ShowWhenMoved, HideWhenMoved
	}

	[SerializeField] KitchenObjectSO changeInto;
	[SerializeField] int changeProgressMax;
	[SerializeField] ProgressMoveBehavior progressMoveBehavior = ProgressMoveBehavior.PauseWhenMoved;
	[SerializeField] VisibleMoveBehavior visibleMoveBehavior = VisibleMoveBehavior.ShowWhenMoved;

	public ProgressMoveBehavior GetProgressMoveBehavior() {
		return progressMoveBehavior;
	}
	public VisibleMoveBehavior GetVisibleMoveBehavior() {
		return visibleMoveBehavior;
	}
	public KitchenObjectSO KitchenObjectSO { get => changeInto; set => changeInto = value; }
	public int ChangeProgressMax { get => changeProgressMax; set => changeProgressMax = value; }
}

