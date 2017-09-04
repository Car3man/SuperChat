using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Common;
using HGL;

public class ListRoomsPanel : MonoBehaviour {
	[SerializeField] private GameObject content;
	[SerializeField] private GameObject buttonRoomPrefab;

	public void Init (List<Room> rooms) {
		ClearRooms ();

		foreach (Room r in rooms) {
			GameObject b = Instantiate (buttonRoomPrefab, content.transform);
			b.GetComponent<ButtonRoom> ().Init (r.Name);
		}
	}

	private void ClearRooms () {
		for (int i = 0; i < content.transform.childCount; i++) {
			DestroyObject (content.transform.GetChild (0).gameObject);
		}
	}

	public void Close () {
		HGL_WindowManager.I.CloseWindow (null, null, "ListRoomsPanel", false);
	}
}
