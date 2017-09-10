using System;

public class Message {
	public MessageLevel MessageLevel;
    public string Owner;
	public string Text;

    public Message(MessageLevel messageLevel, string text) {
        MessageLevel = messageLevel;
        Text = text;
    }

    public Message (MessageLevel messageLevel, string owner, string text) {
		MessageLevel = messageLevel;
        Owner = owner;
		Text = text;
	}
}

public enum MessageLevel {
	UserMessage,
	Info
}

