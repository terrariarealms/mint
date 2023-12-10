using Microsoft.Xna.Framework;
using MongoDB.Driver.Linq;
using Terraria;
using Player = Mint.Server.Player;

namespace Mint.Network.Incoming;

public static class IncomingHandlers
{
    internal static void Initialize()
    {
        MintServer.Network.IncomingModulesHijack[1] = OnChatNetModule;

        MintServer.Network.IncomingHijack[PacketID.PlayerPresence] = OnPlayerPresence;
    }

    static void OnChatNetModule(Player? player, IncomingPacket packet, ref bool ignore)
    {
        ignore = true;
        if (player == null) return;

        BinaryReader reader = packet.GetReader();

        string command = reader.ReadString();
        string text = reader.ReadString();

        MintServer.Chat.HandleMessage(new ChatMessage(player, command == "Say" ? text : $"/{command.ToLower()} {text}", DateTime.UtcNow));
    }

    static void OnPlayerPresence(Player? player, IncomingPacket packet, ref bool ignore)
    {
        ignore = true;
        if (player == null) return;

        BinaryReader reader = packet.GetReader();
        reader.ReadByte();

        byte skinVariant = reader.ReadByte();
        byte hair = reader.ReadByte();
        string name = reader.ReadString();

        if (name.Length > 20)
        {
            player?.Kick("Ваш ник слишком длинный!");
            return;
        }
        if (name.Length == 0)
        {
            player?.Kick("Некорректный ник!");
            return;
        }

        foreach (Player plr in MintServer.Players.players)
        {
            if (plr != null && plr.Index != player.Index && plr.PlayerState == PlayerState.Joined && plr.Name == name)
            {
                player?.Kick("Игрок с таким ником уже играет на сервере.");
                return;
            }
        }

        byte hairDye = reader.ReadByte();
		ReadAccessoryVisibility(reader, player.TPlayer.hideVisibleAccessory);
        byte hideMisc = reader.ReadByte();
        Color hairColor = reader.ReadRGB();
        Color skinColor = reader.ReadRGB();
        Color eyeColor = reader.ReadRGB();
        Color shirtColor = reader.ReadRGB();
        Color ushirtColor = reader.ReadRGB();
        Color pantsColor = reader.ReadRGB();
        Color shoesColor = reader.ReadRGB();

        PlayerVisuals visuals = new PlayerVisuals()
        {
            SkinVariant = skinVariant,
            Hair = hair,
            HairDye = hairDye,
            HideAccessories = player.TPlayer.hideVisibleAccessory,
            HideMisc = hideMisc,
            HairColor = hairColor,
            SkinColor = skinColor,
            EyesColor = eyeColor,
            ShirtColor = shirtColor,
            UndershirtColor = ushirtColor,
            PantsColor = pantsColor,
            ShoesColor = shoesColor
        };

        CharacterDifficulty difficulty = (CharacterDifficulty)reader.ReadByte();
        CharacterExtraFirst extra1 = (CharacterExtraFirst)reader.ReadByte();
        CharacterExtraSecond extra2 = (CharacterExtraSecond)reader.ReadByte();

        player?.Character?.SetVisuals(visuals, true);
        player?.Character?.SetDifficulty(difficulty, true);
        player?.Character?.SetExtraFirst(extra1, true);
        player?.Character?.SetExtraSecond(extra2, true);

        player?.SetName(name, true);

    }

	static void ReadAccessoryVisibility(BinaryReader reader, bool[] hideVisibleAccessory)
	{
		ushort num = reader.ReadUInt16();
		for (int i = 0; i < hideVisibleAccessory.Length; i++)
		{
			hideVisibleAccessory[i] = (num & (1 << i)) != 0;
		}
	}
}