using Common;
using Common.ResponseData;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Response_BaseHandler : LocalSingletonBehaviour<Response_BaseHandler> {
    public void DoHandle(byte[] data) {
        BaseResponseData baseResponseData = Utils.ToObjectFromBytes<BaseResponseData>(data);
        Debug.Log(string.Format("Response, request type: {0}, result: {1}", baseResponseData.Request, baseResponseData.Result));
    }
}
