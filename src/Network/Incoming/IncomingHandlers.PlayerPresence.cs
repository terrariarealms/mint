using Microsoft.Xna.Framework;
using Terraria;

namespace Mint.Network.Incoming;

public static partial class IncomingHandlers
{
    private static void OnPlayerPresence(Server.Player player, IncomingPacket packet, ref bool ignore)
    {
        var reader = packet.GetReader();
        reader.ReadByte();

        var skinVariant = reader.ReadByte();
        var hair = reader.ReadByte();
        var name = reader.ReadString();

        if (!player.SentPackets[4])
            player.SetName(name, true);

        if (name.Length > 20)
        {
            player.Kick(MintServer.Localization.Translate("Your nickname is too long!"));
            return;
        }

        if (name.Length == 0)
        {
            player.Kick(MintServer.Localization.Translate("Invalid nickname!"));
            return;
        }

        Predicate<Server.Player> sameNamePredicate = (p) =>
            p != null && p.Index != player.Index && p.PlayerState == PlayerState.Left && p.Name == name;
        if (MintServer.Players.First(sameNamePredicate) != null)
        {
            player.Kick(MintServer.Localization.Translate("Player with same nickname is playing on server!"));
            return;
        }


        var hairDye = reader.ReadByte();
        ReadAccessoryVisibility(reader, player.TPlayer.hideVisibleAccessory);
        var hideMisc = reader.ReadByte();
        var hairColor = reader.ReadRGB();
        var skinColor = reader.ReadRGB();
        var eyeColor = reader.ReadRGB();
        var shirtColor = reader.ReadRGB();
        var ushirtColor = reader.ReadRGB();
        var pantsColor = reader.ReadRGB();
        var shoesColor = reader.ReadRGB();

        var visuals = new CharacterVisuals()
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

        var difficulty = (CharacterDifficulty)reader.ReadByte();
        var extra1 = (CharacterExtraFirst)reader.ReadByte();
        var extra2 = (CharacterExtraSecond)reader.ReadByte();

        var stats = new CharacterStats()
        {
            Visuals = visuals,
            Difficulty = difficulty,
            ExtraFirst = extra1,
            ExtraSecond = extra2
        };

        if (difficulty.HasFlag(CharacterDifficulty.Journey) && Main.GameMode != 4)
        {
            player.Kick(MintServer.Localization.Translate("This server is not supporting Journey."));
            return;
        }

        player.Character.SetStats(stats, CharacterOperation.RequestedByPlayer);

        player.SentPackets[4] = true;
    }
}