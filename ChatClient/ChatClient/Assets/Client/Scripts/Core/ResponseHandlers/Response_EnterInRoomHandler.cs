using Common;
using Common.ResponseData;
using System;
using UnityEngine;

public class Response_EnterInRoomHandler : LocalSingletonBehaviour<Response_EnterInRoomHandler> {
    public void DoHandle(byte[] data) {
        BaseResponseData baseResponseData = Utils.ToObjectFromBytes<BaseResponseData>(data);

        string nickname = Convert.ToString(baseResponseData.GetValue("Nickname"));
        if(!nickname.Equals(SuperChat.I.Client.Nickname)) {
            GUIManager.I.DrawMessage(new Message(MessageLevel.Info, string.Format("{0} was entered in room", nickname)));
        }

        Debug.Log(string.Format("Response, request type: {0}, result: {1}", baseResponseData.Request, baseResponseData.Result));
    }
}
