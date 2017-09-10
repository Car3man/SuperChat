using Common;
using Common.EventData;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Event_ChatMessageHandler : LocalSingletonBehaviour<Event_ChatMessageHandler> {
    public void DoHandle(byte[] data) {
        ChatMessageData chatMessageData = Utils.ToObjectFromBytes<ChatMessageData>(data);
        GUIManager.I.DrawMessage(new Message(MessageLevel.UserMessage, chatMessageData.Owner, chatMessageData.Text));
    }
}
