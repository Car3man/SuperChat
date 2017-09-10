using Common;
using Common.ResponseData;
using HGL;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Response_GetListRoomsHandler : LocalSingletonBehaviour<Response_GetListRoomsHandler> {
    public void DoHandle(byte[] data) {
        BaseResponseData baseResponseData = Utils.ToObjectFromBytes<BaseResponseData>(data);

        ListRoomsPanel listRoomsPanel = HGL_WindowManager.I.GetWindow("ListRoomsPanel").GetComponent<ListRoomsPanel>();
        listRoomsPanel.Init(Utils.ToObjectFromBytes<List<Room>>((byte[])baseResponseData.GetValue("Rooms")));

        HGL_WindowManager.I.OpenWindow(null, null, "ListRoomsPanel", false, true);

        Debug.Log(string.Format("Response, request type: {0}, result: {1}", baseResponseData.Request, baseResponseData.Result));
    }
}
