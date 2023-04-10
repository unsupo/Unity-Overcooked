using UnityEngine;
using System;
using System.Collections;

public static class CoroutineUtils {
	/**
	 * Usage: StartCoroutine(CoroutineUtils.DelaySeconds(action, delay))
	 * For example:
	 *     StartCoroutine(CoroutineUtils.DelaySeconds(
	 *         () => DebugUtils.Log("2 seconds past"),
	 *         2);
	 */
	public static IEnumerator DelaySeconds(Action action, float delay) {
		yield return new WaitForSeconds(delay);
		action();
	}

	public static IEnumerator WaitForSeconds(float time) {
		yield return new WaitForSeconds(time);
	}

	public static IEnumerator Do(Action action) {
		action();
		yield return 0;
	}
}