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
using Common.RequestData;
using Common.Data;
using Common.ResponseData;

public class SuperChat : LocalSingletonBehaviour<SuperChat> {
    public Client Client {
        get {
            return client;
        }
    }

	private Client client;
	private NetworkStream stream;
	private UnityThreading.ActionThread clientThread;

    private bool quiting;

	private Queue<Message> messages = new Queue<Message> ();

	public override void DoAwake () {
        #if UNITY_STANDALONE
            Screen.SetResolution (360, 640, false);
		#endif
	}

	private IEnumerator Start () {
		yield return new WaitForSeconds (0.5F);

        HGL_WindowManager.I.OpenWindow(null, null, "WelcomePanel", false, true);
    }

	public void StartClient (string nickname) {
		client = new Client (new TcpClient ());
		client.Nickname = nickname;
		client.CurrentClient.Connect ("192.168.0.87", 6969);
		stream = client.CurrentClient.GetStream ();

        LoginData loginData = new LoginData(client.Nickname, "");
        SendRequest(client, (byte)RequestTypes.Login, loginData);

		clientThread = UnityThreadHelper.CreateThread (ClientThread);
	}

	private void ClientThread () {
		Debug.Log ("Client thread started");

		byte[] buffer = new byte[4096];
		byte[] trimBuffer;

		while (!quiting) {
            if(stream.DataAvailable) {
                int bytesCount = stream.Read(buffer, 0, buffer.Length);

                if(bytesCount > 0) {
                    trimBuffer = new byte[bytesCount];
                    Array.Copy(buffer, trimBuffer, bytesCount);

                    DataInfo dataInfo = Utils.ToObjectFromBytes<DataInfo>(trimBuffer);
                    buffer = new byte[dataInfo.Length];
                    bytesCount = stream.Read(buffer, 0, dataInfo.Length);

                    BaseData data = Utils.ToObjectFromBytes<BaseData>(buffer);

                    switch(dataInfo.Type) {
                        case DataInfo.Types.EventData:
                            HandleEvent(data.Type, data.Data);
                            break;
                        case DataInfo.Types.ResponseData:
                            HandleResponse(data.Type, data.Data);
                            break;
                        default:
                            Console.WriteLine("Unknow data type");
                            break;
                    }

                    buffer = new byte[4096];
                }
            }
        }

        if(stream != null)
            stream.Close();
        if(client != null)
            client.CurrentClient.Close();

        UnityThreadHelper.Dispatcher.Dispatch(() => {
            Application.Quit();
        });
	}

    private void HandleEvent(byte type, byte[] data) {
        EventTypes code = (EventTypes)type;

        UnityThreadHelper.Dispatcher.Dispatch(() => {
            switch(code) {
                case EventTypes.ChatMessage:
                    Event_ChatMessageHandler.I.DoHandle(data);
                    break;
                default:
                    Debug.Log("Unknow event");
                    break;
            }
        });
    }

    private void HandleResponse(byte type, byte[] data) {
        RequestTypes code = (RequestTypes)type;

        UnityThreadHelper.Dispatcher.Dispatch(() => {
            switch(code) {
                case RequestTypes.GetListRooms:
                    Response_GetListRoomsHandler.I.DoHandle(data);
                    break;
                default:
                    Response_BaseHandler.I.DoHandle(data);
                    break;
            }
        });
    }

	private void HandleOperation (byte[] data) {
        RequestTypes code = (RequestTypes)data [0];

		switch (code) {
			case RequestTypes.Login:
				if (((RequestResult)data [1]) == RequestResult.Ok) {
					Debug.Log ("Success login");
				} else {
					Debug.Log ("Login error");
				}
				break;
			case RequestTypes.SendMessage:
				Debug.Log (data.Length);

				if (data.Length == 2) {
					
				} else {
                    Debug.Log(Encoding.UTF8.GetString(data, 1, data.Length - 1));
					messages.Enqueue (new Message (MessageLevel.UserMessage, Encoding.UTF8.GetString (data, 1, data.Length - 1)));
				}
				break;
			case RequestTypes.EnterInRoom:
				if (((RequestResult)data [1]) == RequestResult.Ok) {
					string nickname = Encoding.UTF8.GetString (data, 2, data.Length - 2);

					if (nickname == this.client.Nickname)
						Debug.Log ("Entered in room");
					else {
						messages.Enqueue (new Message (MessageLevel.Info, string.Format ("{0} was entered", nickname)));
					}
				} else {
					Debug.Log (((RequestResult)data [1]).ToString ());
				}
				break;
			case RequestTypes.CreateRoom:
				if (((RequestResult)data [1]) == RequestResult.Ok) {
					Debug.Log ("Created room");
				} else {
					Debug.Log (((RequestResult)data [1]).ToString ());
				}
				break;
			case RequestTypes.GetListRooms:
				if (((RequestResult)data [1]) == RequestResult.Ok) {
					List<Room> rooms = null;
					using (MemoryStream ms = new MemoryStream (Utils.SubArray<byte> (data, 2, data.Length - 2))) {
						IFormatter br = new BinaryFormatter ();
						rooms = (br.Deserialize (ms) as List<Room>);
					}

					UnityThreadHelper.Dispatcher.Dispatch (() => {
						ListRoomsPanel listRoomsPanel = HGL_WindowManager.I.GetWindow ("ListRoomsPanel").GetComponent<ListRoomsPanel> ();
						listRoomsPanel.Init (rooms);

						HGL_WindowManager.I.OpenWindow (null, null, "ListRoomsPanel", false, true);
					});

				} else {
					Debug.Log (((RequestResult)data [1]).ToString ());
				}
				break;
			default:
				Debug.Log ("Unknow operation");
				break;
		}
	}

    private void OnApplicationQuit() {
        if(!quiting) Application.CancelQuit();
        quiting = true; 
    }

    private void SendRequest(Client client, byte type, BaseRequestData data) {
        NetworkStream stream = client.CurrentClient.GetStream();

        BaseData d = new BaseData(type, Utils.ToByteArray(data));

        List<byte> bytesInfo = new List<byte>();
        List<byte> bytes = new List<byte>();

        bytes.AddRange(Utils.ToByteArray(d));
        bytesInfo.AddRange(Utils.ToByteArray(new DataInfo(DataInfo.Types.RequestData, bytes.Count)));

        stream.Write(bytesInfo.ToArray(), 0, bytesInfo.Count);
        Thread.Sleep(50);
        stream.Write(bytes.ToArray(), 0, bytes.Count);
    }

    #region Public methods

    public void Register(string name, string surname, string nickname, string email, string city, int age, string password) {
        if(client != null) {
            RegisterData registerData = new RegisterData(name, surname, nickname, email, city, age, password);
            SendRequest(client, (byte)RequestTypes.Register, registerData);

            Debug.Log("Sended request \"EnterRoom\"");
        }
    }

    public void EnterInRoom (string name) {
		if (client != null) {
            EnterInRoomData enterInRoomData = new EnterInRoomData(name);
            SendRequest(client, (byte)RequestTypes.EnterInRoom, enterInRoomData);

			Debug.Log ("Sended request \"EnterRoom\"");
		}
	}

	public void CreateRoom (string name) {
		if (client != null) {
            CreateRoomData createRoomData = new CreateRoomData(name);
            SendRequest(client, (byte)RequestTypes.CreateRoom, createRoomData);

            Debug.Log ("Sended request \"CreateRoom\"");
		}
	}

	public void GetListRooms () {
		if (client != null) {
            GetListRoomsData getListRoomsData = new GetListRoomsData();
            SendRequest(client, (byte)RequestTypes.GetListRooms, getListRoomsData);

            Debug.Log ("Sended request \"GetListRooms\"");
		}
	}

    public void SendMessage(string message) {
        if(client != null) {
            SendMessageData sendMessageData = new SendMessageData(message);
            SendRequest(client, (byte)RequestTypes.SendMessage, sendMessageData);
        }
    }

    #endregion
}
		 
