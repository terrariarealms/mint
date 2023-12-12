using Extensions;
using Microsoft.Xna.Framework;
using MongoDB.Driver.Linq;
using Terraria;
using Terraria.Localization;
using Player = Mint.Server.Player;

namespace Mint.Network.Incoming;

public static class IncomingHandlers
{
    internal static void Initialize()
    {
        Log.Information("IncomingHandlers -> Initialize()");

        MintServer.Network.IncomingModulesHijack[1] = OnChatNetModule;

        MintServer.Network.IncomingHijack[PacketID.PlayerUUID] = OnPlayerUUID;
        MintServer.Network.IncomingHijack[PacketID.PlayerUpdate] = OnPlayerUpdate;

        MintServer.Network.IncomingHijack[PacketID.PlayerPresence] = OnPlayerPresence;
        MintServer.Network.IncomingHijack[PacketID.PlayerSlot] = OnPlayerSlot;
        MintServer.Network.IncomingHijack[PacketID.PlayerLife] = OnPlayerLife;
        MintServer.Network.IncomingHijack[PacketID.PlayerMana] = OnPlayerMana;
    }

    static void OnChatNetModule(Player player, IncomingPacket packet, ref bool ignore)
    {
        BinaryReader reader = packet.GetReader();

        string command = reader.ReadString();
        string text = reader.ReadString();

        if (text.Length > 300)
        {
            if (player.IgnoreAnticheat)
            {
                player.Messenger.Send(MessageMark.Warning, "Mint", "Ваше сообщение не было отправлено, так как оно превышает максимальную длину сообщения.");
                player.Messenger.Send(MessageMark.Warning, "Mint", "Вы также не были исключены с сервера, так как имеете право на игнорирование античита.");
                return;
            }

            player.KickAnticheat("Слишком длинное сообщение.");
            return;
        }

        MintServer.Chat.HandleMessage(new ChatMessage(player, command == "Say" ? text : $"/{command.ToLower()} {text}", DateTime.UtcNow));
    }

    static void OnPlayerUpdate(Player player, IncomingPacket packet, ref bool ignore)
    {
        BinaryReader reader = packet.GetReader();
        
        reader.ReadByte(); // index

        TPlayer tplayer = player.TPlayer;
        
        byte controlsByte = reader.ReadByte();
        byte pulleyByte = reader.ReadByte();
        byte misc1Byte = reader.ReadByte();
        byte misc2Byte = reader.ReadByte();

        player.Controls = (PlayerControl)controlsByte;
        player.Pulley = (PlayerPulley)pulleyByte;
        player.MiscFirst = (PlayerMiscFirst)misc1Byte;
        player.MiscSecond = (PlayerMiscSecond)misc2Byte;

#region Terraria stolen
        BitsByte controls = controlsByte;
        BitsByte pulley = pulleyByte;
        BitsByte misc1 = misc1Byte;
        BitsByte misc2 = misc2Byte;
        tplayer.controlUp = controls[0];
        tplayer.controlDown = controls[1];
        tplayer.controlLeft = controls[2];
        tplayer.controlRight = controls[3];
        tplayer.controlJump = controls[4];
        tplayer.controlUseItem = controls[5];
        tplayer.direction = (controls[6] ? 1 : (-1));
        if (pulley[0])
        {
        	tplayer.pulley = true;
        	tplayer.pulleyDir = (byte)((!pulley[1]) ? 1u : 2u);
        }
        else
        {
        	tplayer.pulley = false;
        }
        tplayer.vortexStealthActive = pulley[3];
        tplayer.gravDir = (pulley[4] ? 1 : (-1));
        tplayer.TryTogglingShield(pulley[5]);
        tplayer.ghost = pulley[6];
        tplayer.selectedItem = reader.ReadByte();
        tplayer.position = reader.ReadVector2();
        if (pulley[2])
        {
        	tplayer.velocity = reader.ReadVector2();
        }
        else
        {
        	tplayer.velocity = Vector2.Zero;
        }
        if (misc1[6])
        {
        	tplayer.PotionOfReturnOriginalUsePosition = reader.ReadVector2();
        	tplayer.PotionOfReturnHomePosition = reader.ReadVector2();
        }
        else
        {
        	tplayer.PotionOfReturnOriginalUsePosition = null;
        	tplayer.PotionOfReturnHomePosition = null;
        }
        tplayer.tryKeepingHoveringUp = misc1[0];
        tplayer.IsVoidVaultEnabled = misc1[1];
        tplayer.sitting.isSitting = misc1[2];
        tplayer.downedDD2EventAnyDifficulty = misc1[3];
        tplayer.isPettingAnimal = misc1[4];
        tplayer.isTheAnimalBeingPetSmall = misc1[5];
        tplayer.tryKeepingHoveringDown = misc1[7];
        tplayer.sleeping.SetIsSleepingAndAdjustPlayerRotation(tplayer, misc2[0]);
        tplayer.autoReuseAllWeapons = misc2[1];
        tplayer.controlDownHold = misc2[2];
        tplayer.isOperatingAnotherEntity = misc2[3];
        tplayer.controlUseTile = misc2[4];
#endregion

        if (player.RemoteClient.State == 10)
        {
        	NetMessage.TrySendData(13, -1, player.Index, NetworkText.Empty, player.Index);
        }
    }


    static void OnPlayerSlot(Player player, IncomingPacket packet, ref bool ignore)
    {
        BinaryReader reader = packet.GetReader();
        
        reader.ReadByte();

        short slotId = reader.ReadInt16();
        short stack = reader.ReadInt16();
        byte prefix = reader.ReadByte();
        short netId = reader.ReadInt16();
        
        Item item = new Item();
        item.netDefaults(netId);
        
        if (stack > item.maxStack)
        {
            player.Character.SetSlot(slotId, new NetItem(0, 0, 0), CharacterOperation.RequestedByServer);
            return;
        }

        player.Character.SetSlot(slotId, new NetItem(netId, stack, prefix), CharacterOperation.RequestedByPlayer);
    }

    static void OnPlayerLife(Player player, IncomingPacket packet, ref bool ignore)
    {
        BinaryReader reader = packet.GetReader();
        
        reader.ReadByte();

        short current = reader.ReadInt16();
        short max = reader.ReadInt16();

        if (!player.IgnoreAnticheat && (current > max || max % 20 != 0 || max > 500))
        {
            player.KickAnticheat("нихуя");
            return;
        }

        player.TPlayer.statLife = current;
        player.TPlayer.statLifeMax = max;
        player.TPlayer.statLifeMax2 = max;

        player.Character.SetLife(current, CharacterOperation.RequestedByPlayer);
    }


    static void OnPlayerMana(Player player, IncomingPacket packet, ref bool ignore)
    {
        BinaryReader reader = packet.GetReader();
        
        reader.ReadByte();

        short current = reader.ReadInt16();
        short max = reader.ReadInt16();

        if (!player.IgnoreAnticheat && (current > max || max % 20 != 0 || max > 200))
        {
            player.KickAnticheat("ебаный ты волшебник");
            return;
        }

        player.TPlayer.statMana = current;
        player.TPlayer.statManaMax = max;
        player.TPlayer.statManaMax2 = max;

        player.Character.SetMana(current, CharacterOperation.RequestedByPlayer);
    }

    static void OnPlayerPresence(Player player, IncomingPacket packet, ref bool ignore)
    {
        BinaryReader reader = packet.GetReader();
        reader.ReadByte();

        byte skinVariant = reader.ReadByte();
        byte hair = reader.ReadByte();
        string name = reader.ReadString();

        if (name.Length > 20)
        {
            player.Kick("Ваш ник слишком длинный!");
            return;
        }
        if (name.Length == 0)
        {
            player.Kick("Некорректный ник!");
            return;
        }

        foreach (Player plr in MintServer.Players.players)
        {
            if (plr != null && plr.Index != player.Index && plr.PlayerState == PlayerState.Joined && plr.Name == name)
            {
                player.Kick("Игрок с таким ником уже играет на сервере.");
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

        CharacterVisuals visuals = new CharacterVisuals()
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

        CharacterStats stats = new CharacterStats()
        {
            Visuals = visuals,
            Difficulty = difficulty,
            ExtraFirst = extra1,
            ExtraSecond = extra2,
        };

        if (difficulty.HasFlag(CharacterDifficulty.Journey) && Main.GameMode != 4)
        {
            player.Kick("Этот сервер не поддерживает креатив.");
            return;
        }

        player.Character.SetStats(stats, CharacterOperation.RequestedByPlayer);

        if (player.SentPackets[4])
            return;
        
        player.SentPackets[4] = true;

        player.SetName(name, true);
    }

    static void OnPlayerUUID(Player player, IncomingPacket packet, ref bool ignore)
    {
        if (player.SentPackets[68])
            return;
        
        player.SentPackets[68] = true;

        BinaryReader reader = packet.GetReader();
        string uuid = reader.ReadString();

        player.UUID = uuid;

        if (!player.Authorized)
        {
            player.AutoAuthorize();
        }
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