// AlphaBiomes.MapComponentExtender
using System;
using System.Collections.Generic;
using System.Linq;
// using AlphaBiomes;
using RimWorld;
using Verse;
using Verse.Sound;

namespace WVC_Tweaks
{

	public class MapComponentTweaks : MapComponent
	{

		// private int nextResearch = 0;
		// private int nextCleaning = 0;
		// private int nextLetterClose = 0;

		public MapComponentTweaks(Map map)
			: base(map)
		{
		}

		// public override void ExposeData()
		// {
			// base.ExposeData();
			// Scribe_Values.Look(ref nextCleaning, "nextCleaning", 0);
			// Scribe_Values.Look(ref nextResearch, "nextResearch", 0);
			// Scribe_Values.Look(ref nextLetterClose, "nextLetterClose", 0);
		// }

		public override void MapComponentTick()
		{
			// base.MapComponentTick();
		}

	}

}
