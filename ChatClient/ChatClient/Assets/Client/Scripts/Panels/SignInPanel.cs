using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SignInPanel : MonoBehaviour {
    public event Action<string, string> OnFormFilled = delegate { };

    [SerializeField]
    private TMP_InputField loginInputField;
    [SerializeField]
    private TMP_InputField passwordInputField;

    public void Login() {
        if(string.IsNullOrEmpty(loginInputField.text)) return;
        if(string.IsNullOrEmpty(passwordInputField.text)) return;
        OnFormFilled.Invoke(loginInputField.text, passwordInputField.text);
    }
}
