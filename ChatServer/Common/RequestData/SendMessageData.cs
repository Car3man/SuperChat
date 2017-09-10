using System;
using System.Collections.Generic;
using System.Text;

namespace Common.RequestData {
    [Serializable]
    public class SendMessageData : BaseRequestData {
        public string Message;

        public SendMessageData(string message) {
            Message = message;
        }
    }
}
