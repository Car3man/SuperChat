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
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Runtime.Serialization;

public class SuperChat : LocalSingletonBehaviour<SuperChat> {
	[SerializeField] private GameObject messagePrefab;
	[SerializeField] private GameObject contentMessages;
	[SerializeField] private TMP_InputField messageInputField;

	[SerializeField] private Color userMessageColor;
	[SerializeField] private Color infoMessageColor;

	private Client client;
	private NetworkStream stream;
	private Thread clientThread;

	private bool successLogin;

	private Queue<Message> messages = new Queue<Message> ();

	private void Awake () {
		#if UNITY_STANDALONE
			Screen.SetResolution (360, 640, false);
		#endif
	}

	private IEnumerator Start () {
		yield return new WaitForSeconds (0.5F);

		WindowCommon1 enterNicknamePanel = HGL_WindowManager.I.GetWindow ("WindowCommon1").GetComponent<WindowCommon1> ();
		enterNicknamePanel.OnValueEntered += OnNicknameEntered;
		enterNicknamePanel.Init ("Enter nickname");
		HGL_WindowManager.I.OpenWindow (null, null, "WindowCommon1", false, true);
	}

	public void SendMessage () {
		if (client != null) {
			List<byte> data = new List<byte> ();

			data.Add ((byte)OperationCodes.SendMessage);
			data.AddRange (Encoding.UTF8.GetBytes (string.Format ("{0} - {1}", client.Nickname, messageInputField.text)));

			stream.Write (data.ToArray (), 0, data.Count);
		}

		messageInputField.text = string.Empty;
	}

	private void OnNicknameEntered (string nickname) {
		WindowCommon1 enterNicknamePanel = HGL_WindowManager.I.GetWindow ("WindowCommon1").GetComponent<WindowCommon1> ();
		enterNicknamePanel.OnValueEntered -= OnNicknameEntered;

		client = new Client (new TcpClient ());
		client.Nickname = nickname;
		client.CurrentClient.Connect ("192.168.0.54", 6969);
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
			Message message = messages.Dequeue ();

			GameObject instanceMessage = Instantiate (messagePrefab, contentMessages.transform);
			instanceMessage.GetComponent<TextMeshProUGUI> ().text = message.Text;

			switch (message.MessageLevel) {
				case MessageLevel.Info:
					instanceMessage.GetComponent<TextMeshProUGUI> ().color = infoMessageColor;
					break;
				case MessageLevel.UserMessage:
					instanceMessage.GetComponent<TextMeshProUGUI> ().color = userMessageColor;
					break;
			}

			instanceMessage.GetComponent<RectTransform> ().sizeDelta = new Vector2 (
				instanceMessage.GetComponent<RectTransform> ().sizeDelta.x, 
				instanceMessage.GetComponent<TextMeshProUGUI> ().preferredHeight
			);
		}
	}

	private void ClientThread () {
		byte[] buffer = new byte[4096];
		byte[] trimBuffer;

		while (true) {
			int bytesCount = stream.Read (buffer, 0, buffer.Length);

			if (bytesCount > 0) {
				trimBuffer = new byte[bytesCount];
				Array.Copy (buffer, trimBuffer, bytesCount);
				HandleOperation (trimBuffer);
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
				} else {
					Debug.Log ("Login error");
				}
				break;
			case OperationCodes.SendMessage:
				Debug.Log (data.Length);

				if (data.Length == 2) {
					
				} else {
					messages.Enqueue (new Message (MessageLevel.UserMessage, Encoding.UTF8.GetString (data, 1, data.Length - 1)));
				}
				break;
			case OperationCodes.EnterInRoom:
				if (((ResponseCode)data [1]) == ResponseCode.Ok) {
					string nickname = Encoding.UTF8.GetString (data, 2, data.Length - 2);

					if (nickname == this.client.Nickname)
						Debug.Log ("Entered in room");
					else {
						messages.Enqueue (new Message (MessageLevel.Info, string.Format ("{0} was entered", nickname)));
					}
				} else {
					Debug.Log (((ResponseCode)data [1]).ToString ());
				}
				break;
			case OperationCodes.CreateRoom:
				if (((ResponseCode)data [1]) == ResponseCode.Ok) {
					Debug.Log ("Created room");
				} else {
					Debug.Log (((ResponseCode)data [1]).ToString ());
				}
				break;
			case OperationCodes.GetListRooms:
				if (((ResponseCode)data [1]) == ResponseCode.Ok) {
					List<Room> rooms = null;
					using (MemoryStream ms = new MemoryStream (Utils.SubArray<byte> (data, 2, data.Length - 2))) {
						IFormatter br = new BinaryFormatter ();
						rooms = (br.Deserialize (ms) as List<Room>);
					}

					ListRoomsPanel listRoomsPanel = HGL_WindowManager.I.GetWindow ("ListRoomsPanel").GetComponent<ListRoomsPanel> ();
					listRoomsPanel.Init (rooms);

					HGL_WindowManager.I.OpenWindow (null, null, "ListRoomsPanel", false, true);

				} else {
					Debug.Log (((ResponseCode)data [1]).ToString ());
				}
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

	#region Public methods

	public void EnterInRoom (string name) {
		if (client != null) {
			List<byte> data = new List<byte> ();

			data.Add ((byte)OperationCodes.EnterInRoom);
			data.AddRange (Encoding.UTF8.GetBytes (name));

			stream.Write (data.ToArray (), 0, data.Count);

			Debug.Log ("Sended request \"EnterRoom\"");
		}
	}

	public void CreateRoom (string name) {
		if (client != null) {
			List<byte> data = new List<byte> ();

			data.Add ((byte)OperationCodes.CreateRoom);
			data.AddRange (Encoding.UTF8.GetBytes (name));

			stream.Write (data.ToArray (), 0, data.Count);

			Debug.Log ("Sended request \"CreateRoom\"");
		}
	}

	public void GetListRooms () {
		if (client != null) {
			List<byte> data = new List<byte> ();

			data.Add ((byte)OperationCodes.GetListRooms);

			stream.Write (data.ToArray (), 0, data.Count);

			Debug.Log ("Sended request \"GetListRooms\"");
		}
	}

	#endregion
}
		 
