using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HGL;
using TMPro;

public class GUIManager : LocalSingletonBehaviour<GUIManager> {
    [SerializeField]
    private GameObject messagePrefab;
    [SerializeField]
    private GameObject contentMessages;
    [SerializeField]
    private TMP_InputField messageInputField;

    [SerializeField]
    private Color userMessageColor;
    [SerializeField]
    private Color infoMessageColor;

    public void ToggleMenu () {
		HGL_UGUIWindow menu = HGL_WindowManager.I.GetWindow ("MenuPanel").GetComponent<HGL_UGUIWindow> ();

		if (HGL_WindowManager.I.CanOpen (menu.State)) {
			HGL_WindowManager.I.OpenWindow (null, null, "MenuPanel", false, true);
		} else {
			HGL_WindowManager.I.CloseWindow (null, null, "MenuPanel", false);
		}
	}

    public void SendMessage() {
        SuperChat.I.SendMessage(messageInputField.text);
        messageInputField.text = string.Empty;
    }

    public void DrawMessages(List<Message> messages) {
        foreach(Message m in messages) {
            DrawMessage(m);
        }
    }

    public void DrawMessage(Message message) {
        GameObject instanceMessage = Instantiate(messagePrefab, contentMessages.transform);

        if(string.IsNullOrEmpty(message.Owner)) instanceMessage.GetComponent<TextMeshProUGUI>().text = message.Text;
        else instanceMessage.GetComponent<TextMeshProUGUI>().text = string.Format("{0} - {1}", message.Owner, message.Text);

        switch(message.MessageLevel) {
            case MessageLevel.Info:
                instanceMessage.GetComponent<TextMeshProUGUI>().color = infoMessageColor;
                break;
            case MessageLevel.UserMessage:
                instanceMessage.GetComponent<TextMeshProUGUI>().color = userMessageColor;
                break;
        }

        instanceMessage.GetComponent<RectTransform>().sizeDelta = new Vector2(
            instanceMessage.GetComponent<RectTransform>().sizeDelta.x,
            instanceMessage.GetComponent<TextMeshProUGUI>().preferredHeight
        );
    }

    public void ClearMessages() {

    }
}
