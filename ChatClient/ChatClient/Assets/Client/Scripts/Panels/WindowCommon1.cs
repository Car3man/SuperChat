using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;
using HGL;

public class WindowCommon1 : MonoBehaviour {
	[SerializeField] private TextMeshProUGUI titleText;
	[SerializeField] private TMP_InputField valueInputField;

	public Action<string> OnValueEntered = delegate {
	};

	public void Init (string title) {
		ClearInputField ();

		titleText.text = title;
	}

	public void ClearInputField () {
		valueInputField.text = string.Empty;
	}

	public void Ok () {
		if (!string.IsNullOrEmpty (valueInputField.text)) {
			OnValueEntered.Invoke (valueInputField.text);
			HGL_WindowManager.I.CloseWindow (null, null, this.GetType ().ToString (), false);
		}
	}
}
