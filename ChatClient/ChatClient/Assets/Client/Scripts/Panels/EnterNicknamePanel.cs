using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;
using HGL;

public class EnterNicknamePanel : MonoBehaviour {
	[SerializeField] private TMP_InputField nicknameInputField;

	public Action<string> OnNicknameEntered = delegate {
	};

	public void Ok () {
		if (!string.IsNullOrEmpty (nicknameInputField.text)) {
			OnNicknameEntered.Invoke (nicknameInputField.text);
			HGL_WindowManager.I.CloseWindow (null, null, this.GetType ().ToString (), false);
		}
	}
}
