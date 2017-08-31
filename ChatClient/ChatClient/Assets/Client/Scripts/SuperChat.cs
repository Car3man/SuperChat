using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Common;
using System;
using HGL;
using TMPro;

public class SuperChat : MonoBehaviour {
	[SerializeField] private GameObject messagePrefab;
	[SerializeField] private GameObject contentMessages;
	[SerializeField] private TMP_InputField messageInputField;

	private Client client;
	private NetworkStream stream;
	private Thread clientThread;

	private bool successLogin;

	private Queue<string> messages = new Queue<string> ();

	public void SendMessage () {
		if (client != null) {
			List<byte> data = new List<byte> ();

			data.Add ((byte)OperationCodes.SendMessage);
			data.AddRange (Encoding.UTF8.GetBytes (string.Format ("{0} - {1}", client.Nickname, messageInputField.text)));

			stream.Write (data.ToArray (), 0, data.Count);
		}

		messageInputField.text = string.Empty;
	}

	private IEnumerator Start () {
		yield return new WaitForSeconds (0.5F);

		EnterNicknamePanel enterNicknamePanel = HGL_WindowManager.I.GetWindow ("EnterNicknamePanel").GetComponent<EnterNicknamePanel> ();
		enterNicknamePanel.OnNicknameEntered += OnNicknameEntered;
		HGL_WindowManager.I.OpenWindow (null, null, "EnterNicknamePanel", false, true);
	}

	private void OnNicknameEntered (string nickname) {
		EnterNicknamePanel enterNicknamePanel = HGL_WindowManager.I.GetWindow ("EnterNicknamePanel").GetComponent<EnterNicknamePanel> ();
		enterNicknamePanel.OnNicknameEntered -= OnNicknameEntered;

		client = new Client (new TcpClient ());
		client.Nickname = nickname;
		client.CurrentClient.Connect ("127.0.0.1", 6969);
		stream = client.CurrentClient.GetStream ();

		List<byte> data = new List<byte> ();

		data.Add ((byte)OperationCodes.Login);
		data.AddRange (Encoding.UTF8.GetBytes (client.Nickname));

		stream.Write (data.ToArray (), 0, data.Count);

		clientThread = new Thread (ClientThread);
		clientThread.Start ();
	}

	private void Update () {
		if (messages.Count > 0) {
			string message = messages.Dequeue ();

			GameObject instanceMessage = Instantiate (messagePrefab, contentMessages.transform);
			instanceMessage.GetComponent<Text> ().text = message;

			instanceMessage.GetComponent<RectTransform> ().sizeDelta = new Vector2 (
				instanceMessage.GetComponent<RectTransform> ().sizeDelta.x, 
				instanceMessage.GetComponent<Text> ().preferredHeight
			);
		}

		if (Input.GetKeyDown (KeyCode.C)) {
			if (client != null) {
				List<byte> data = new List<byte> ();

				data.Add ((byte)OperationCodes.CreateRoom);
				data.AddRange (Encoding.UTF8.GetBytes ("NewRoom"));

				stream.Write (data.ToArray (), 0, data.Count);

				Debug.Log ("Sended request \"CreateRoom\"");
			}
		}

		if (Input.GetKeyDown (KeyCode.V)) {
			if (client != null) {
				List<byte> data = new List<byte> ();

				data.Add ((byte)OperationCodes.EnterInRoom);
				data.AddRange (BitConverter.GetBytes (0));

				stream.Write (data.ToArray (), 0, data.Count);

				Debug.Log ("Sended request \"EnterRoom\"");
			}
		}
	}

	private void ClientThread () {
		byte[] buffer = new byte[4096];

		while (true) {
			int bytesCount = stream.Read (buffer, 0, buffer.Length);

			if (bytesCount > 0) {
				HandleOperation (buffer);
				buffer = new byte[4096];
			}
		}
	}

	private void HandleOperation (byte[] data) {
		OperationCodes code = (OperationCodes)data [0];

		switch (code) {
			case OperationCodes.Login:
				if (((ResponseCode)data [1]) == ResponseCode.Ok) {
					successLogin = true;
					Debug.Log ("Success login");
				}
				break;
			case OperationCodes.SendMessage:
				messages.Enqueue (Encoding.UTF8.GetString (data, 1, data.Length - 1));
				break;
			case OperationCodes.EnterInRoom:
				messages.Enqueue (string.Format ("Entered in room with ID {0}", BitConverter.ToInt32 (data, 1)));
				break;
			default:
				Debug.Log ("Unknow operation");
				break;
		}
	}

	private void OnDestroy () {
		if (stream != null)
			stream.Close ();
		if (client != null)
			client.CurrentClient.Close ();
		if (clientThread != null)
			clientThread.Abort ();
	}
}
		 
