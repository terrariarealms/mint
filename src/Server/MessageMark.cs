namespace Mint.Server;

public enum MessageMark : byte
{
    OK,
    Info,
    Error,
    Warning,

    PageHeader,
    PageItem,
    PageFooter
}