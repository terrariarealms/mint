namespace Mint.Server.Chat;

public delegate void ChatFilterDelegate(Player sender, ref string text, ref bool block);