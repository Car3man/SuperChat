using HGL;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class WelcomePanel : MonoBehaviour {
    public void SignIn() {
        HGL_WindowManager.I.CloseWindow(null, null, "WelcomePanel", false);

        SignInPanel signInPanel = HGL_WindowManager.I.GetWindow("SignInPanel").GetComponent<SignInPanel>();
        signInPanel.OnFormFilled += SignInPanel_OnFormFilled;
        HGL_WindowManager.I.OpenWindow(null, null, "SignInPanel", false, true);
    }

    private void SignInPanel_OnFormFilled(string nickname, string password) {
        SignInPanel signInPanel = HGL_WindowManager.I.GetWindow("SignInPanel").GetComponent<SignInPanel>();
        signInPanel.StartCoroutine(TryLogin(nickname, password));
    }

    private IEnumerator TryLogin(string nickname, string password) {
        WWWForm form = new WWWForm();
        form.AddField("nickname", nickname);
        form.AddField("password", password);

        using(UnityWebRequest www = UnityWebRequest.Post("http://topsecret500.pe.hu/login.php", form)) {
            yield return www.Send();

            if(www.isNetworkError || www.isHttpError) {
                Debug.Log(www.error);
            } else {
                bool result = Convert.ToBoolean(www.downloadHandler.text);

                Debug.Log("Login result: " + result);

                if(result) {
                    HGL_WindowManager.I.CloseWindow(null, null, "SignInPanel", false);
                    SuperChat.I.StartClient(nickname);
                } else {
                    //failed login
                }
            }
        }
    }

    public void SignUp() {

    }
}
