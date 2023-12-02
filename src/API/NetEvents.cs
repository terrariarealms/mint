namespace Mint.API;

public static class NetEvents
{
    public delegate void OnSendData(Player? player, SendMessageRequest sendRequest, ref bool ignore);
    public delegate void OnGetData(Player player, Packet packet, ref bool ignore);

    /// <summary>
    /// Invokes on NetMessage.SendData.
    /// </summary>
    public static event OnSendData? SendData;
    internal static void InvokeSendData(Player? player, SendMessageRequest sendRequest, ref bool ignore) 
        => SendData?.Invoke(player, sendRequest, ref ignore);

    /// <summary>
    /// Invokes on MessageBuffer.GetData.
    /// </summary>
    public static event OnGetData? GetData;
    internal static void InvokeGetData(Player player, Packet packet, ref bool ignore) 
        => GetData?.Invoke(player, packet, ref ignore);
}