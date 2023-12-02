using System.Collections.Concurrent;
using Terraria.Localization;

namespace Mint.Network;

public static class AsyncMessage
{
    /// <summary>
    /// Available network sessions.
    /// </summary>
    public static NetworkSession?[] Sessions = new NetworkSession?[257];

    internal static void Initialize()
    {
        // 256 session is broadcast.
        StartSession(256);

        On.Terraria.NetMessage.SendData += OnSendData;
    }

    private static void OnSendData(On.Terraria.NetMessage.orig_SendData orig, int msgType, int remoteClient, int ignoreClient, NetworkText text, int number, float number2, float number3, float number4, int number5, int number6, int number7)
    {
        AsyncSend(msgType, remoteClient, ignoreClient, text, number, number2, number3, number4, number5, number6, number7);
    }

    internal static void StartSession(int playerId)
    {
        NetworkSession session = new NetworkSession(playerId);
        session.Start();
        Sessions[playerId] = session;
    }

    internal static void StopSession(int playerId)
    {
        NetworkSession? session = Sessions[playerId];
        if (session == null) return;

        session.Stop();
    }

    /// <summary>
    /// Async implement of NetMessage.SendData.
    /// </summary>
    /// <param name="packetId">Packet ID</param>
    /// <param name="remoteClient">Remote Client ID</param>
    /// <param name="ignoreClient">Ignore Client ID</param>
    /// <param name="text">Network Text</param>
    /// <param name="num0"></param>
    /// <param name="num1"></param>
    /// <param name="num2"></param>
    /// <param name="num3"></param>
    /// <param name="num4"></param>
    /// <param name="num5"></param>
    /// <param name="num6"></param>
    public static void AsyncSend(int packetId, int remoteClient, int ignoreClient, NetworkText? text, int num0 = 0, float num1 = 0f, float num2 = 0f, float num3 = 0f, int num4 = 0, int num5 = 0, int num6 = 0)
    {
        NetworkSession? session = Sessions[remoteClient == -1 ? 256 : remoteClient];
        if (session == null) return;

        SendMessageRequest request = new SendMessageRequest(packetId, remoteClient, ignoreClient, text, num0, num1, num2, num3, num4, num5, num6);
        session.AddRequest(request);
    }

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

        public void Handle() =>
            Net.SendData(PacketID, RemoteClient, IgnoreClient, Text ?? NetworkText.Empty, Number0, Number1, Number2, Number3, Number4, Number5, Number6);
        

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

    public class NetworkSession
    {
        public NetworkSession(int playerId)
        {
            _playerID = playerId;
            _packets = new BlockingCollection<SendMessageRequest>();
        }

        public void Start()
        {
            Console.WriteLine($"Start> network session on {_playerID}.");
            _tokenSrc = new CancellationTokenSource();

            _sessionTask = Task.Run(SessionHandler);
        }

        public void Stop()
        {
            Console.WriteLine($"Stop> network session on {_playerID}.");
            _tokenSrc?.Cancel();
        }

        public void AddRequest(SendMessageRequest request) => _packets.Add(request);

        private void SessionHandler()
        {
            while (_tokenSrc != null && _tokenSrc.IsCancellationRequested)
            {
                SendMessageRequest request = _packets.Take(_tokenSrc.Token);
                request.Handle();
            }
            _packets.Dispose();
        }

        private CancellationTokenSource? _tokenSrc;
        private BlockingCollection<SendMessageRequest> _packets;
        private Task? _sessionTask;
        private int _playerID;
    }
}