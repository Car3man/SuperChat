using System;
using System.Collections.Generic;

namespace Common.ResponseData {
    [Serializable]
    public class BaseResponseData {
        public RequestTypes Request;
        public RequestResult Result;
        public Dictionary<string, object> Data;

        public BaseResponseData(RequestTypes request, RequestResult result, Dictionary<string, object> data) {
            Request = request;
            Result = result;
            Data = data;

            if(data.Count == 0) Data = null;
        }

        public BaseResponseData(RequestTypes request, RequestResult result) {
            Request = request;
            Result = result;
        }

        public object GetValue(string key) {
            object o = null;

            if(Data != null && Data.ContainsKey(key)) {
                o = Data[key];
            }

            return o;
        }

        public void AddValue(string key, object value, bool overwrite = true) {
            if(Data == null) Data = new Dictionary<string, object>();
            if(Data.ContainsKey(key)) {
                if(overwrite) Data[key] = value;
            } else Data.Add(key, value);
        }
    }
}
