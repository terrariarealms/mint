namespace Mint.Server;

public delegate void AutoAuthorizeEvent(Player player, ref Account? account, ref bool ignore);