namespace Mint.Server.Commands;

public delegate void CommandInvokeEvent(Player sender, ICommand command, ref CommandInvokeContext ctx, ref bool ignore);