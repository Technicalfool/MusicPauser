/*!
 * MusicPauser: Pause the music along with the game.
 * A mod for Kerbal Space Program by Technicalfool.
 */
using System;
using System.Diagnostics;
using System.IO;

using System.Timers;

using UnityEngine;
using Newtonsoft.Json;


namespace MusicPauser
{
	public class VolumeSettings
	{
		public float fadeTime;
	};

	[KSPAddon(KSPAddon.Startup.EveryScene, true)]
	public class MusicPauser : MonoBehaviour
	{
		private const int DEBUG_LEVEL = 1;

		private static VolumeSettings vSettings;
		private const string filePath = "vsettings.cfg";

		/*public MusicPauser()
		{
		}*/
		static float oldVolume1;
		//static float oldVolume2; //To be honest, this isn't even needed.
		private static int fadeDirection;
		//private static double fadeStarted;
		private static float oldVolume;

		private static Stopwatch fWatch;
		private static Timer fTimer;

		public void Start()
		{
			vSettings = new VolumeSettings();
			fadeDirection = 0;
			//fadeStarted = DateTime.Now.Ticks / 1000000.0d;
			vSettings.fadeTime = 250;


			/*
			 * Capture the pause and unpause events, send them to the provided functions.
			 */
			GameEvents.onGamePause.Add(onGamePause);
			GameEvents.onGameUnpause.Add(onGameUnpause);

			fWatch = new Stopwatch();
			fTimer = new Timer(25); //25 = 1/50th a second, or 50fps.
			fTimer.Elapsed += fadeCallback;
			//load(); (Doesn't work yet)
		}

		private void fadeCallback(System.Object source, ElapsedEventArgs e)
		{
			//double tNow = DateTime.Now.Ticks / 1000000.0d;
			long tDelta = fWatch.ElapsedMilliseconds;
			if (DEBUG_LEVEL < 1)
				UnityEngine.Debug.Log("[MusicPauser] tDelta: " + tDelta);
			float fTime = vSettings.fadeTime;
			float ratio;
			if (tDelta > 0)
			{
				if (fadeDirection < 0)
				{
					ratio = 1 - ((float)tDelta / fTime);
				}
				else if (fadeDirection > 0){
					ratio = (float)tDelta / fTime;
				}
				else{
					ratio = 0.0f;
				}
				if (DEBUG_LEVEL < 1)
					UnityEngine.Debug.Log("[MusicPauser] Current ratio: " + ratio + ")");
			}
			else{
				ratio = 0.0f;
			}
			if (tDelta >= fTime)
			{
				if (fadeDirection > 0)
					MusicLogic.SetVolume(oldVolume);
				if (fadeDirection < 0)
					MusicLogic.SetVolume(0.0f);
				fTimer.Enabled = false;
				fadeDirection = 0;
				fWatch.Stop();
			}
			else{
				MusicLogic.SetVolume(oldVolume * ratio);
			}
		}

		private void startFading(int dir)
		{
			if (DEBUG_LEVEL < 1)
				UnityEngine.Debug.Log("[MusicPauser] Starting fade.");
			//fadeStarted = DateTime.Now.Ticks / 1000000.0f;
			fadeDirection = dir;
			if (!fTimer.Enabled){
				if (DEBUG_LEVEL < 1)
					UnityEngine.Debug.Log("[MusicPauser] Enabling fade timer.");
				if (fadeDirection < 0)
					oldVolume = MusicLogic.fetch.audio1.volume;
				fWatch.Reset();
				fWatch.Start();
				fTimer.Enabled = true;
			}
		}

		/*
		 * Functions that react to game events here.
		 */
		private void onGamePause()
		{
			oldVolume1 = MusicLogic.fetch.audio1.volume;
			//oldVolume2 = MusicLogic.fetch.audio2.volume;
			if (DEBUG_LEVEL < 1)
				UnityEngine.Debug.Log("[MusicPauser] Setting music from " + oldVolume1 + " to 0.");
			//MusicLogic.SetVolume(0.0f);
			startFading(-1);
		}
		private void onGameUnpause()
		{
			if (DEBUG_LEVEL < 1)
				UnityEngine.Debug.Log("[MusicPauser] Setting music from 0 to " + oldVolume1 + ".");
			//MusicLogic.SetVolume(oldVolume1);
			startFading(1);
		}


		/*!
		 * TODO: read fadeTime in from a file.
		 */
		public static void load()
		{
			try
			{
				if (System.IO.File.Exists(filePath))
				{
					using (StreamReader sr = File.OpenText(filePath))
					{
						string fulltext = "";
						string s = "";
						while((s = sr.ReadLine()) != null)
						{
							fulltext += s;
						}
						vSettings = JsonConvert.DeserializeObject<VolumeSettings>(fulltext);
						if (DEBUG_LEVEL < 1)
							UnityEngine.Debug.Log("[MusicPauser] Saved volume settings.");
					}
				}
				else
				{
					save();
				}
			}
			catch(Exception e)
			{
				if (DEBUG_LEVEL < 1)
					UnityEngine.Debug.Log("[MusicPauser] Unable to load volume settings file:" + e.Message);
			}
		}

		/*!
		 * TODO: write fadeTime out to a file.
		 */
		public static void save()
		{
			try
			{
				if (System.IO.File.Exists(filePath))
					System.IO.File.Delete(filePath);
				using (StreamWriter sw = File.CreateText(filePath))
				{
					sw.Write(JsonConvert.SerializeObject(vSettings));
				}
			}
			catch(Exception e)
			{
				if (DEBUG_LEVEL < 1)
					UnityEngine.Debug.Log("[MusicPauser] Unable to save volume settings file:" + e.Message);
			}
		}


	}
}

