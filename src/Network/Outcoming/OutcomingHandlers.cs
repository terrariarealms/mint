using Terraria;
using Terraria.GameContent.Events;
using Terraria.Social;
using Player = Mint.Server.Player;

namespace Mint.Network.Outcoming;

public static class OutcomingHandlers
{
    internal static void Initialize()
    {
        Log.Information("OutcomingHandlers -> Initialize()");
        MintServer.Network.OutcomingHijack[PacketID.WorldData] = OnWorldData;
    }

    internal static void OnWorldData(Player player, OutcomingPacket packet, ref bool ignore)
    {
        PacketWriter writer = new PacketWriter(new MemoryStream());
        writer.WriteByte(7);

        writer.Write((int)Main.time);
        BitsByte bitsByte5 = (byte)0;
        bitsByte5[0] = Main.dayTime;
        bitsByte5[1] = Main.bloodMoon;
        bitsByte5[2] = Main.eclipse;
        writer.Write(bitsByte5);
        writer.Write((byte)Main.moonPhase);
        writer.Write((short)Main.maxTilesX);
        writer.Write((short)Main.maxTilesY);
        writer.Write((short)Main.spawnTileX);
        writer.Write((short)Main.spawnTileY);
        writer.Write((short)Main.worldSurface);
        writer.Write((short)Main.rockLayer);
        writer.Write(Main.worldID);
        writer.Write(Main.worldName);
        writer.Write((byte)Main.GameMode);
        writer.Write(Main.ActiveWorldFileData.UniqueId.ToByteArray());
        writer.Write(Main.ActiveWorldFileData.WorldGeneratorVersion);
        writer.Write((byte)Main.moonType);
        writer.Write((byte)WorldGen.treeBG1);
        writer.Write((byte)WorldGen.treeBG2);
        writer.Write((byte)WorldGen.treeBG3);
        writer.Write((byte)WorldGen.treeBG4);
        writer.Write((byte)WorldGen.corruptBG);
        writer.Write((byte)WorldGen.jungleBG);
        writer.Write((byte)WorldGen.snowBG);
        writer.Write((byte)WorldGen.hallowBG);
        writer.Write((byte)WorldGen.crimsonBG);
        writer.Write((byte)WorldGen.desertBG);
        writer.Write((byte)WorldGen.oceanBG);
        writer.Write((byte)WorldGen.mushroomBG);
        writer.Write((byte)WorldGen.underworldBG);
        writer.Write((byte)Main.iceBackStyle);
        writer.Write((byte)Main.jungleBackStyle);
        writer.Write((byte)Main.hellBackStyle);
        writer.Write(Main.windSpeedTarget);
        writer.Write((byte)Main.numClouds);
        for (int n = 0; n < 3; n++)
        {
        	writer.Write(Main.treeX[n]);
        }
        for (int num8 = 0; num8 < 4; num8++)
        {
        	writer.Write((byte)Main.treeStyle[num8]);
        }
        for (int num9 = 0; num9 < 3; num9++)
        {
        	writer.Write(Main.caveBackX[num9]);
        }
        for (int num10 = 0; num10 < 4; num10++)
        {
        	writer.Write((byte)Main.caveBackStyle[num10]);
        }
        WorldGen.TreeTops.SyncSend(writer);
        if (!Main.raining)
        {
        	Main.maxRaining = 0f;
        }
        writer.Write(Main.maxRaining);
        BitsByte bitsByte6 = (byte)0;
        bitsByte6[0] = WorldGen.shadowOrbSmashed;
        bitsByte6[1] = NPC.downedBoss1;
        bitsByte6[2] = NPC.downedBoss2;
        bitsByte6[3] = NPC.downedBoss3;
        bitsByte6[4] = Main.hardMode;
        bitsByte6[5] = NPC.downedClown;
        bitsByte6[6] = Main.ServerSideCharacter;
        bitsByte6[7] = NPC.downedPlantBoss;
        writer.Write(bitsByte6);
        BitsByte bitsByte7 = (byte)0;
        bitsByte7[0] = NPC.downedMechBoss1;
        bitsByte7[1] = NPC.downedMechBoss2;
        bitsByte7[2] = NPC.downedMechBoss3;
        bitsByte7[3] = NPC.downedMechBossAny;
        bitsByte7[4] = Main.cloudBGActive >= 1f;
        bitsByte7[5] = WorldGen.crimson;
        bitsByte7[6] = Main.pumpkinMoon;
        bitsByte7[7] = Main.snowMoon;
        writer.Write(bitsByte7);
        BitsByte bitsByte8 = (byte)0;
        bitsByte8[1] = Main.fastForwardTimeToDawn;
        bitsByte8[2] = Main.slimeRain;
        bitsByte8[3] = NPC.downedSlimeKing;
        bitsByte8[4] = NPC.downedQueenBee;
        bitsByte8[5] = NPC.downedFishron;
        bitsByte8[6] = NPC.downedMartians;
        bitsByte8[7] = NPC.downedAncientCultist;
        writer.Write(bitsByte8);
        BitsByte bitsByte9 = (byte)0;
        bitsByte9[0] = NPC.downedMoonlord;
        bitsByte9[1] = NPC.downedHalloweenKing;
        bitsByte9[2] = NPC.downedHalloweenTree;
        bitsByte9[3] = NPC.downedChristmasIceQueen;
        bitsByte9[4] = NPC.downedChristmasSantank;
        bitsByte9[5] = NPC.downedChristmasTree;
        bitsByte9[6] = NPC.downedGolemBoss;
        bitsByte9[7] = BirthdayParty.PartyIsUp;
        writer.Write(bitsByte9);
        BitsByte bitsByte10 = (byte)0;
        bitsByte10[0] = NPC.downedPirates;
        bitsByte10[1] = NPC.downedFrost;
        bitsByte10[2] = NPC.downedGoblins;
        bitsByte10[3] = Sandstorm.Happening;
        bitsByte10[4] = DD2Event.Ongoing;
        bitsByte10[5] = DD2Event.DownedInvasionT1;
        bitsByte10[6] = DD2Event.DownedInvasionT2;
        bitsByte10[7] = DD2Event.DownedInvasionT3;
        writer.Write(bitsByte10);
        BitsByte bitsByte11 = (byte)0;
        bitsByte11[0] = NPC.combatBookWasUsed;
        bitsByte11[1] = LanternNight.LanternsUp;
        bitsByte11[2] = NPC.downedTowerSolar;
        bitsByte11[3] = NPC.downedTowerVortex;
        bitsByte11[4] = NPC.downedTowerNebula;
        bitsByte11[5] = NPC.downedTowerStardust;
        bitsByte11[6] = Main.forceHalloweenForToday;
        bitsByte11[7] = Main.forceXMasForToday;
        writer.Write(bitsByte11);
        BitsByte bitsByte12 = (byte)0;
        bitsByte12[0] = NPC.boughtCat;
        bitsByte12[1] = NPC.boughtDog;
        bitsByte12[2] = NPC.boughtBunny;
        bitsByte12[3] = NPC.freeCake;
        bitsByte12[4] = Main.drunkWorld;
        bitsByte12[5] = NPC.downedEmpressOfLight;
        bitsByte12[6] = NPC.downedQueenSlime;
        bitsByte12[7] = Main.getGoodWorld;
        writer.Write(bitsByte12);
        BitsByte bitsByte13 = (byte)0;
        bitsByte13[0] = Main.tenthAnniversaryWorld;
        bitsByte13[1] = Main.dontStarveWorld;
        bitsByte13[2] = NPC.downedDeerclops;
        bitsByte13[3] = Main.notTheBeesWorld;
        bitsByte13[4] = Main.remixWorld;
        bitsByte13[5] = NPC.unlockedSlimeBlueSpawn;
        bitsByte13[6] = NPC.combatBookVolumeTwoWasUsed;
        bitsByte13[7] = NPC.peddlersSatchelWasUsed;
        writer.Write(bitsByte13);
        BitsByte bitsByte14 = (byte)0;
        bitsByte14[0] = NPC.unlockedSlimeGreenSpawn;
        bitsByte14[1] = NPC.unlockedSlimeOldSpawn;
        bitsByte14[2] = NPC.unlockedSlimePurpleSpawn;
        bitsByte14[3] = NPC.unlockedSlimeRainbowSpawn;
        bitsByte14[4] = NPC.unlockedSlimeRedSpawn;
        bitsByte14[5] = NPC.unlockedSlimeYellowSpawn;
        bitsByte14[6] = NPC.unlockedSlimeCopperSpawn;
        bitsByte14[7] = Main.fastForwardTimeToDusk;
        writer.Write(bitsByte14);
        BitsByte bitsByte15 = (byte)0;
        bitsByte15[0] = Main.noTrapsWorld;
        bitsByte15[1] = Main.zenithWorld;
        bitsByte15[2] = NPC.unlockedTruffleSpawn;
        writer.Write(bitsByte15);
        writer.Write((byte)Main.sundialCooldown);
        writer.Write((byte)Main.moondialCooldown);
        writer.Write((short)WorldGen.SavedOreTiers.Copper);
        writer.Write((short)WorldGen.SavedOreTiers.Iron);
        writer.Write((short)WorldGen.SavedOreTiers.Silver);
        writer.Write((short)WorldGen.SavedOreTiers.Gold);
        writer.Write((short)WorldGen.SavedOreTiers.Cobalt);
        writer.Write((short)WorldGen.SavedOreTiers.Mythril);
        writer.Write((short)WorldGen.SavedOreTiers.Adamantite);
        writer.Write((sbyte)Main.invasionType);
        if (SocialAPI.Network != null)
        {
        	writer.Write(SocialAPI.Network.GetLobbyId());
        }
        else
        {
        	writer.Write(0uL);
        }
        writer.Write(Sandstorm.IntendedSeverity);

        byte[] bytes = writer.Build();

        NetworkUtils.SendPacket(bytes, packet.RemoteClient, packet.IgnoreClient);
    }
}