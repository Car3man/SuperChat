using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HGL;

public class MenuPanel : MonoBehaviour {
	public void EnterInRoom () {
		WindowCommon1 windowCommon1 = HGL_WindowManager.I.GetWindow ("WindowCommon1").GetComponent<WindowCommon1> ();
		windowCommon1.OnValueEntered += OnRoomNameEntered_EnterInRoom;
		windowCommon1.Init ("Enter name room");
		HGL_WindowManager.I.OpenWindow (null, null, "WindowCommon1", false, true);
	}

	public void CreateRoom () {
		WindowCommon1 windowCommon1 = HGL_WindowManager.I.GetWindow ("WindowCommon1").GetComponent<WindowCommon1> ();
		windowCommon1.OnValueEntered += OnRoomNameEntered_CreateRoom;
		windowCommon1.Init ("Enter name room");
		HGL_WindowManager.I.OpenWindow (null, null, "WindowCommon1", false, true);
	}

	public void GetListRooms () {
		SuperChat.I.GetListRooms ();
	}

	private void OnRoomNameEntered_EnterInRoom (string name) {
		SuperChat.I.EnterInRoom (name);

		WindowCommon1 windowCommon1 = HGL_WindowManager.I.GetWindow ("WindowCommon1").GetComponent<WindowCommon1> ();
		windowCommon1.OnValueEntered -= OnRoomNameEntered_EnterInRoom;
		HGL_WindowManager.I.CloseWindow (null, null, "WindowCommon1", false);

		GUIManager.I.ToggleMenu ();
	}

	private void OnRoomNameEntered_CreateRoom (string name) {
		SuperChat.I.CreateRoom (name);

		WindowCommon1 windowCommon1 = HGL_WindowManager.I.GetWindow ("WindowCommon1").GetComponent<WindowCommon1> ();
		windowCommon1.OnValueEntered -= OnRoomNameEntered_CreateRoom;
		HGL_WindowManager.I.CloseWindow (null, null, "WindowCommon1", false);

		GUIManager.I.ToggleMenu ();
	}
}
