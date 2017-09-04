using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HGL;

public class GUIManager : LocalSingletonBehaviour<GUIManager> {
	public void ToggleMenu () {
		HGL_UGUIWindow menu = HGL_WindowManager.I.GetWindow ("MenuPanel").GetComponent<HGL_UGUIWindow> ();

		if (HGL_WindowManager.I.CanOpen (menu.State)) {
			HGL_WindowManager.I.OpenWindow (null, null, "MenuPanel", false, true);
		} else {
			HGL_WindowManager.I.CloseWindow (null, null, "MenuPanel", false);
		}
	}
}
