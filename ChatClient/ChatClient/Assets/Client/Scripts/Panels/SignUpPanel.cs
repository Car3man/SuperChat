using Common.RequestData;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SignUpPanel : MonoBehaviour {
    public event Action<RegisterData> OnFormFilled = delegate { };

    [SerializeField]
    private TMP_InputField nameInputField;
    [SerializeField]
    private TMP_InputField surnameInputField;
    [SerializeField]
    private TMP_InputField nicknameInputField;
    [SerializeField]
    private TMP_InputField emailInputField;
    [SerializeField]
    private TMP_InputField cityInputField;
    [SerializeField]
    private TMP_InputField ageInputField;
    [SerializeField]
    private TMP_InputField passwordInputField;

    public void Register() {
        if(string.IsNullOrEmpty(nameInputField.text)) return;
        if(string.IsNullOrEmpty(surnameInputField.text)) return;
        if(string.IsNullOrEmpty(nicknameInputField.text)) return;
        if(string.IsNullOrEmpty(emailInputField.text)) return;
        if(string.IsNullOrEmpty(cityInputField.text)) return;
        if(string.IsNullOrEmpty(ageInputField.text)) return;
        if(string.IsNullOrEmpty(passwordInputField.text)) return;

        OnFormFilled.Invoke(new RegisterData(nameInputField.text, surnameInputField.text, nicknameInputField.text, emailInputField.text, cityInputField.text, GetInt(ageInputField.text), passwordInputField.text));
    }

    private int GetInt(string text) {
        int result = 0;
        int.TryParse(text, out result);
        return result;
    }
}
