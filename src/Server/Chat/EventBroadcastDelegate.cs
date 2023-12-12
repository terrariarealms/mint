using Microsoft.Xna.Framework;

namespace Mint.Server.Chat;

public delegate void EventBroadcastDelegate(ref string message, ref Color color, ref bool ignore);