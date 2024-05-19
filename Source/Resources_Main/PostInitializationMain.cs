using RimWorld;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace WVC_Tweaks
{

    [StaticConstructorOnStartup]
	public static class PostInitializationMain
	{
		static PostInitializationMain()
		{
			// Precepts
			if (WVC_Tweaks.settings.enableJunkEmptyPrecepts)
			{
				List<PreceptDef> pathcedPrecepts = new();
				foreach (PreceptDef preceptDef in DefDatabase<PreceptDef>.AllDefsListForReading)
				{
					if (preceptDef.comps.NullOrEmpty())
					{
						if (preceptDef.modContentPack.IsOfficialMod)
						{
							continue;
						}
						if (preceptDef.defaultSelectionWeight > 0f)
						{
							preceptDef.defaultSelectionWeight = 0f;
							// Log.Error(preceptDef.defName + " junked.");
							pathcedPrecepts.Add(preceptDef);
						}
					}
				}
				if (!pathcedPrecepts.NullOrEmpty())
				{
					Log.Warning("WVC - Tweaks JunkPreceptPatch | All pathced precepts:" + "\n" + pathcedPrecepts.Select((PreceptDef x) => x.defName).ToLineList(" - "));
				}
				else
				{
					Log.Warning("WVC - Tweaks JunkPreceptPatch | precepts list is null");
				}
			}
			// Things
			if (!WVC_Tweaks.settings.enableBuildingsStuffPatch)
			{
				return;
			}
			List<StuffCategoryDef> allowedStuff = new() 
			{ 
				StuffCategoryDefOf.Stony,
				StuffCategoryDefOf.Metallic,
				StuffCategoryDefOf.Woody
			};
			List<ThingDef> pathcedThings = new();
			foreach (ThingDef thingDef in DefDatabase<ThingDef>.AllDefsListForReading)
			{
				if (thingDef.building == null)
				{
					continue;
				}
				if (thingDef.IsFrame || thingDef.building.IsTurret || thingDef.building.IsMortar)
				{
					continue;
				}
				if (thingDef.costStuffCount > 0 && !thingDef.stuffCategories.NullOrEmpty() && thingDef.stuffCategories.Count > 1)
				{
					if (thingDef.comps != null && thingDef.GetCompProperties<CompProperties_FireOverlay>() != null)
					{
						continue;
					}
					bool stopAllCycle = false;
					int countedCost = 0;
					foreach (StuffCategoryDef stuff in thingDef.stuffCategories)
					{
						if (!allowedStuff.Contains(stuff))
						{
							stopAllCycle = true;
						}
					}
					if (stopAllCycle)
					{
						continue;
					}
					bool patched = false;
					foreach (StuffCategoryDef stuffCategoryDef in allowedStuff)
					{
						if (!thingDef.stuffCategories.Contains(stuffCategoryDef))
						{
							thingDef.stuffCategories.Add(stuffCategoryDef);
							patched = true;
						}
					}
					if (patched)
					{
						pathcedThings.Add(thingDef);
					}
					if (thingDef.costList == null)
					{
						continue;
					}
					bool stopCycle = false;
					foreach (ThingDefCountClass thingCost in thingDef.costList)
					{
						if (thingCost.thingDef.smallVolume == true || thingCost.thingDef.stuffProps == null || thingCost.thingDef.stuffProps.categories.NullOrEmpty())
						{
							stopCycle = true;
							continue;
						}
						foreach (StuffCategoryDef stuff in thingCost.thingDef.stuffProps.categories)
						{
							if (!allowedStuff.Contains(stuff))
							{
								stopCycle = true;
								stopAllCycle = true;
							}
						}
						countedCost += thingCost.count;
					}
					if (stopAllCycle)
					{
						continue;
					}
					if (!stopCycle)
					{
						thingDef.costStuffCount += countedCost;
						thingDef.costList = new();
						patched = true;
					}
					if (patched)
					{
						if (!pathcedThings.Contains(thingDef))
						{
							pathcedThings.Add(thingDef);
						}
					}
				}
			}
			if (!pathcedThings.NullOrEmpty())
			{
				Log.Warning("WVC - Tweaks Patch | All pathced buildings:" + "\n" + pathcedThings.Select((ThingDef x) => x.defName).ToLineList(" - "));
			}
			else
			{
				Log.Warning("WVC - Tweaks Patch | buildings list is null");
			}
		}
	}
}