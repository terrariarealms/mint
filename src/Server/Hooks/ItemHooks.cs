using Terraria;
using Terraria.DataStructures;

namespace Mint.Server.Hooks;

public class ItemHooks : Item
{
    public static event EventItemSpawnDelegate? OnItemSpawn;

    internal static void AttachHooks()
    {
        On.Terraria.Item.NewItem_IEntitySource_int_int_int_int_int_int_bool_int_bool_bool +=
            NewItem_IEntitySource_int_int_int_int_int_int_bool_int_bool_bool;
    }

    private static int NewItem_IEntitySource_int_int_int_int_int_int_bool_int_bool_bool
    (On.Terraria.Item.orig_NewItem_IEntitySource_int_int_int_int_int_int_bool_int_bool_bool orig,
        IEntitySource source, int X, int Y, int Width, int Height, int Type, int Stack, bool noBroadcast, int pfix,
        bool noGrabDelay, bool reverseLookup)
    {
        if (MintServer.Config.Game.DisableItems || MintServer.Config.Game.StrippedDownMode)
            return 0;

        var id = orig(source, X, Y, Width, Height, Type, Stack, noBroadcast, pfix, noGrabDelay, reverseLookup);
        OnItemSpawn?.Invoke(id);

        return id;
    }
}