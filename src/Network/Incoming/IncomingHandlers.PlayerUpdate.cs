using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Localization;

namespace Mint.Network.Incoming;

public static partial class IncomingHandlers
{
    private static void OnPlayerUpdate(Server.Player player, IncomingPacket packet, ref bool ignore)
    {
        var reader = packet.GetReader();

        reader.ReadByte(); // index

        var tplayer = player.TPlayer;

        var controlsByte = reader.ReadByte();
        var pulleyByte = reader.ReadByte();
        var misc1Byte = reader.ReadByte();
        var misc2Byte = reader.ReadByte();

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
        tplayer.direction = controls[6] ? 1 : -1;
        if (pulley[0])
        {
            tplayer.pulley = true;
            tplayer.pulleyDir = (byte)(!pulley[1] ? 1u : 2u);
        }
        else
        {
            tplayer.pulley = false;
        }

        tplayer.vortexStealthActive = pulley[3];
        tplayer.gravDir = pulley[4] ? 1 : -1;
        tplayer.TryTogglingShield(pulley[5]);
        tplayer.ghost = pulley[6];
        tplayer.selectedItem = reader.ReadByte();
        tplayer.position = reader.ReadVector2();
        if (pulley[2])
            tplayer.velocity = reader.ReadVector2();
        else
            tplayer.velocity = Vector2.Zero;
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
            NetMessage.TrySendData(13, -1, player.Index, NetworkText.Empty, player.Index);
    }
}