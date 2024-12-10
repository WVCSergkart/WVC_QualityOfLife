using HarmonyLib;
using RimWorld;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace WVC_Tweaks
{

	[StaticConstructorOnStartup]
	public static class PostInitializationMain
	{
		static PostInitializationMain()
		{
			// Precepts
			Precepts();
			// Memes
			Memes();
			// Things
			BuildingsStuff();
		}

		private static void BuildingsStuff()
        {
            List<ThingDef> pathcedThings = new();
			foreach (ThingDef thingDef in DefDatabase<ThingDef>.AllDefsListForReading)
			{
				BuildingsStuff_Stuffable(thingDef, pathcedThings, new() { StuffCategoryDefOf.Stony, StuffCategoryDefOf.Metallic, StuffCategoryDefOf.Woody });
				//BuildingsStuff_Metal(thingDef, pathcedThings, new() { StuffCategoryDefOf.Metallic });
			}
			if (!pathcedThings.NullOrEmpty())
			{
				Log.Warning("WVC - Tweaks Patch | All pathced buildings:" + "\n" + pathcedThings.Select((ThingDef x) => x.defName).ToLineList(" - "));
			}
            //else
            //{
            //	Log.Warning("WVC - Tweaks Patch | buildings list is null");
            //}
            //if (WVC_Tweaks.settings.enableBuildingsMetalStuffPatch)
            //{
            //    var harmony = new Harmony("wvc.sergkart.qolpatches");
            //    harmony.Patch(AccessTools.Method(typeof(BuildableDef), "GetColorForStuff"), prefix: new HarmonyMethod(typeof(PostInitializationMain).GetMethod(nameof(WhiteColorFromStuff))));
            //}
        }

        public static bool WhiteColorFromStuff(ref Color __result, BuildableDef __instance)
        {
            if (StaticCollectionsClass.patchedBuildings.Contains(__instance))
            {
                //Log.Error("0");
                __result = Color.white;
                return false;
            }
            return true;
        }

        private static void BuildingsStuff_Metal(ThingDef thingDef, List<ThingDef> pathcedThings, List<StuffCategoryDef> allowedStuff)
        {
            if (!WVC_Tweaks.settings.enableBuildingsMetalStuffPatch)
            {
                return;
            }
            if (thingDef.building == null)
            {
                return;
            }
            if (thingDef.IsFrame || thingDef.building.IsTurret || thingDef.building.IsMortar)
            {
                return;
            }
            if (!thingDef.stuffCategories.NullOrEmpty())
            {
                return;
            }
            if (thingDef.costList.NullOrEmpty())
            {
                return;
            }
            if (!IsSteelStuff(thingDef.costList))
            {
                return;
            }
            if (thingDef.comps != null && thingDef.GetCompProperties<CompProperties_FireOverlay>() != null)
            {
                return;
            }
            thingDef.stuffCategories = allowedStuff;
            int countedCost = 0;
            foreach (ThingDefCountClass stuff in thingDef.costList.ToList())
            {
                if (stuff.thingDef == ThingDefOf.Steel)
                {
                    countedCost += stuff.count;
                    thingDef.costList.Remove(stuff);
                }
            }
            thingDef.costStuffCount = countedCost;
            if (StaticCollectionsClass.patchedBuildings == null)
            {
                StaticCollectionsClass.patchedBuildings = null;
            }
            pathcedThings.Add(thingDef);
            StaticCollectionsClass.patchedBuildings.Add(thingDef);
            //thingDef.graphicData.shaderType = ShaderTypeDefOf.CutoutComplex;
            //thingDef.graphicData.color = Color.white;
            //thingDef.graphicData.colorTwo = Color.white;
            //thingDef.graphicData.ignoreThingDrawColor = true;
            //if (!thingDef.graphicData.attachments.NullOrEmpty())
            //{
            //    foreach (GraphicData graphicData in thingDef.graphicData.attachments)
            //    {
            //        graphicData.ignoreThingDrawColor = true;
            //    }
            //}
        }

        private static bool IsSteelStuff(List<ThingDefCountClass> costList)
        {
            foreach (ThingDefCountClass stuff in costList)
            {
                if (stuff.thingDef == ThingDefOf.Steel)
                {
                    return true;
                }
            }
            return false;
        }

        private static void PatchStuff(ThingDef thingDef, List<ThingDef> pathcedThings, List<StuffCategoryDef> allowedStuff)
        {
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
                return;
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
                return;
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
                return;
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

        private static void BuildingsStuff_Stuffable(ThingDef thingDef, List<ThingDef> pathcedThings, List<StuffCategoryDef> allowedStuff)
		{
			if (!WVC_Tweaks.settings.enableBuildingsStuffPatch)
			{
				return;
			}
			if (thingDef.building == null)
			{
				return;
			}
			if (thingDef.IsFrame || thingDef.building.IsTurret || thingDef.building.IsMortar)
			{
				return;
			}
			if (thingDef.costStuffCount > 0 && !thingDef.stuffCategories.NullOrEmpty() && thingDef.stuffCategories.Count > 1)
			{
				if (thingDef.comps != null && thingDef.GetCompProperties<CompProperties_FireOverlay>() != null)
				{
					return;
				}
				PatchStuff(thingDef, pathcedThings, allowedStuff);
			}
		}

		private static void Precepts()
		{
			if (!WVC_Tweaks.settings.enableJunkEmptyPrecepts)
			{
				return;
			}
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

		private static void Memes()
		{
			if (!WVC_Tweaks.settings.enableVanillaMemesForFactions)
			{
				return;
			}
			List<MemeDef> pathcedPrecepts = new();
			foreach (MemeDef memeDef in DefDatabase<MemeDef>.AllDefsListForReading)
			{
				if (memeDef.modContentPack == null || !memeDef.modContentPack.IsOfficialMod && !memeDef.modContentPack.IsCoreMod)
				{
					memeDef.randomizationSelectionWeightFactor = 0f;
					pathcedPrecepts.Add(memeDef);
				}
			}
			if (!pathcedPrecepts.NullOrEmpty())
			{
				Log.Warning("WVC - Tweaks FactionMemesPatch | All pathced memes:" + "\n" + pathcedPrecepts.Select((MemeDef x) => x.defName).ToLineList(" - "));
			}
			else
			{
				Log.Warning("WVC - Tweaks FactionMemesPatch | memes list is null");
			}
		}

    }
    public static class StaticCollectionsClass
    {

        public static List<ThingDef> patchedBuildings = new();

    }

}