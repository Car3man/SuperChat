using System;
using System.Collections.Generic;
using System.Text;

namespace Common.EventData {
    [Serializable]
    public class ChatMessageData : BaseEventData {
        public string Owner;
        public string Text;

        public ChatMessageData(string owner, string text) {
            Owner = owner;
            Text = text;
        }
    }
}
