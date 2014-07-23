/*!
 * MusicPauser: Pause the music along with the game.
 * A mod for Kerbal Space Program by Technicalfool.
 */
using System;

using UnityEngine;


namespace MusicPauser
{
	[KSPAddon(KSPAddon.Startup.EveryScene, true)]
	public class MusicPauser : MonoBehaviour
	{
		const int DEBUG_LEVEL = 0;


		/*public MusicPauser()
		{
		}*/
		static float oldVolume1;
		static float oldVolume2; //To be honest, this isn't even needed.

		public void Start()
		{
			/*
			 * Capture the pause and unpause events, send them to the provided functions.
			 */
			GameEvents.onGamePause.Add(onGamePause);
			GameEvents.onGameUnpause.Add(onGameUnpause);
		}

		/*
		 * Functions that react to game events here.
		 */
		private void onGamePause()
		{
			oldVolume1 = MusicLogic.fetch.audio1.volume;
			oldVolume2 = MusicLogic.fetch.audio2.volume;
			if (DEBUG_LEVEL < 1)
				Debug.Log("[MusicPauser] Setting music from " + oldVolume1 + " to 0.");
			MusicLogic.SetVolume(0.0f);
		}
		private void onGameUnpause()
		{
			if (DEBUG_LEVEL < 1)
				Debug.Log("[MusicPauser] Setting music from 0 to " + oldVolume1 + ".");
			MusicLogic.SetVolume(oldVolume1);
		}
	}
}

