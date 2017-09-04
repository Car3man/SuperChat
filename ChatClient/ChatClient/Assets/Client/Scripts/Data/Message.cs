using System;

public class Message {
	public MessageLevel MessageLevel;
	public string Text;

	public Message (MessageLevel messageLevel, string text) {
		MessageLevel = messageLevel;
		Text = text;
	}
}

public enum MessageLevel {
	UserMessage,
	Info
}

