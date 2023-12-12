namespace Mint.Server;

public delegate void FindAccountEvent(Player player, ref Account? candidate, ref bool ignore);