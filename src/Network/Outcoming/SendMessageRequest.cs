using Terraria.Localization;

namespace Mint.Network.Outcoming;

public struct SendMessageRequest
{
    public SendMessageRequest(int packetId, int remote, int ignore, NetworkText? text, int num0, float num1, float num2, float num3, int num4, int num5, int num6)
    {
        PacketID = packetId;
        RemoteClient = remote;
        IgnoreClient = ignore;
        Text = text;
        Number0 = num0;
        Number1 = num1;
        Number2 = num2;
        Number3 = num3;
        Number4 = num4;
        Number5 = num5;
        Number6 = num6;
    }

    public void Handle() 
    {
        bool ignore = false;
        NetEvents.InvokeSendData(RemoteClient == -1 ? null : MintServer.Players[RemoteClient], this, ref ignore);

        if (!ignore)
            Net.SendData(PacketID, RemoteClient, IgnoreClient, Text ?? NetworkText.Empty, Number0, Number1, Number2, Number3, Number4, Number5, Number6);
    }
        

    public int PacketID;
    public int RemoteClient;
    public int IgnoreClient;
    public NetworkText? Text;
    public int Number0;
    public float Number1;
    public float Number2;
    public float Number3;
    public int Number4;
    public int Number5;
    public int Number6;
}