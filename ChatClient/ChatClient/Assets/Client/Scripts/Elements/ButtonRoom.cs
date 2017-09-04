using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using TMPro;

public class ButtonRoom : MonoBehaviour {
	public string NameRoom { get; private set; }

	public event Action<string> OnClick = delegate { };

	public void Init (string nameRoom) {
		NameRoom = nameRoom;

		GetComponent<Button> ().onClick.AddListener (OnClickEvent);
		transform.GetChild (0).GetComponent<TextMeshProUGUI> ().text = nameRoom;
	}

	public void OnClickEvent () {
		OnClick.Invoke (NameRoom);
	}
}
