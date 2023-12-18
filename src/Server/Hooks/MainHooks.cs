
using System.Collections;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Cinematics;
using Terraria.GameContent;
using Terraria.GameContent.Skies;
using Terraria.GameInput;
using Terraria.Graphics.Capture;
using Terraria.Initializers;

namespace Mint.Server.Hooks;

public class MainHooks : Main
{
	public static event EventGameUpdateDelegate? OnGamePreUpdate;
	public static event EventGameUpdateDelegate? OnGamePostUpdate;

    internal static void AttachHooks()
    {
        On.Terraria.Main.Update += OnUpdate;
    }

    private static void OnUpdate(On.Terraria.Main.orig_Update orig, Terraria.Main self, object _gameTime)
    {
        GameTime gameTime = (GameTime)_gameTime;
		if (!IsEnginePreloaded)
		{
			IsEnginePreloaded = true;
		}

		if (!self._isDrawingOrUpdating)
		{
			
			self._isDrawingOrUpdating = true;
			OnGamePreUpdate?.Invoke(gameTime);
			DoUpdate(self, ref gameTime);
			OnGamePostUpdate?.Invoke(gameTime);
			
			if (netMode == 2)
			{
				for (int i = 0; i < 256; i++)
				{
					if (Netplay.Clients[i].Socket != null)
					{
						Netplay.Clients[i].Socket.SendQueuedPackets();
					}
				}
			}
			
			self._isDrawingOrUpdating = false;
		}
		
		ConsumeAllMainThreadActions();
		
    }

	internal static int GameTimeSync;

	protected static void DoUpdate(Terraria.Main self, ref GameTime gameTime)
	{
		if (GameTimeSync == 60)
		{
			NetMessage.SendData(18);
			GameTimeSync = 0;
		}
			
		GameTimeSync++;

		gameTimeCache = gameTime;
		
		PartySky.MultipleSkyWorkaroundFix = true;
		LocalPlayer.cursorItemIconReversed = false;
		if (!GlobalTimerPaused)
		{
			GlobalTimeWrappedHourly = (float)(gameTime.TotalGameTime.TotalSeconds % 3600.0);
		}
		UpdateCreativeGameModeOverride();
		UpdateWorldPreparationState();
		
		
		for (int num = DelayedProcesses.Count - 1; num >= 0; num--)
		{
			IEnumerator enumerator = DelayedProcesses[num];
			if (!enumerator.MoveNext())
			{
				DelayedProcesses.Remove(enumerator);
			}
		}
		
		if (_hasPendingNetmodeChange)
		{
			netMode = _targetNetMode;
			_hasPendingNetmodeChange = false;
		}
		
		if (ActivePlayerFileData != null)
		{
			ActivePlayerFileData.UpdatePlayTimer();
		}
		Netplay.UpdateInMainThread();
		gameInactive = !self.IsActive;

		self._worldUpdateTimeTester.Restart();
		if (!WorldGen.gen)
		{
			WorldGen.destroyObject = false;
		}
		if (gameMenu)
		{
			mapFullscreen = false;
		}
		UpdateSettingUnlocks();
		
		DoUpdate_AutoSave();
		
		if (!MintServer.Config.Game.StrippedDownMode)
		self.UpdateWindyDayState();
		
		cloudAlpha = maxRaining;
		bool isActive = self.IsActive;
		
		if (CanPauseGame())
		{
			DoUpdate_WhilePaused();
			gamePaused = true;
			return;
		}
		gamePaused = false;
		
		if (!MintServer.Config.Game.StrippedDownMode)
		if (AmbienceServer != null)
		{
			AmbienceServer.Update();
		}

		WorldGen.BackgroundsCache.UpdateFlashValues();
		if (LocalGolfState != null)
		{
			LocalGolfState.Update();
		}

		if (!MintServer.Config.Game.StrippedDownMode)
		if ((isActive || netMode == 1) && cloudAlpha > 0f)
		{
			Rain.MakeRain();
		}

		self.updateCloudLayer();

		if (!MintServer.Config.Game.StrippedDownMode)
	
		for (int i = 0; i < dayRate; i++)
		{
			self.UpdateWeather(gameTime, i);
		}
			
		UnpausedUpdateSeed = Utils.RandomNextSeed(UnpausedUpdateSeed);
		Ambience();
		
		
		PortalHelper.UpdatePortalPoints();
		LucyAxeMessage.UpdateMessageCooldowns();

		if (!MintServer.Config.Game.StrippedDownMode)
		if (self.ShouldUpdateEntities())
		{
			self.DoUpdateInWorld(self._worldUpdateTimeTester);
		}
	}

}