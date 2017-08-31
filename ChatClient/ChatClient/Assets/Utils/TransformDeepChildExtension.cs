using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TransformDeepChildExtension {
	public static Transform FindDeepChild (this Transform aParent , string aName) {
		var result = aParent.Find(aName);
		if (result != null)
			return result;
		foreach (Transform child in aParent) {
			result = child.FindDeepChild(aName);
			if (result != null)
				return result;
		}
		return null;
	}

	public static List<Transform> GetChildsDeep (this Transform aParent , List<Transform> childs = null) {
		List<Transform> result = childs;
		if (childs == null) result = new List<Transform>();
		foreach (Transform child in aParent) {
			result.Add(child);
			result.AddRange(child.GetChildsDeep());
		}
		return result;
	}
}
