namespace Mint.Server;

public delegate void AuthorizeEvent(Player player, ref Account account, ref bool tokenChange, ref bool ignore);