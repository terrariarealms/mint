namespace Mint.Server.Chat;

public delegate void EventChatDelegate(Player player, ref ChatMessage message, ref bool ignore);