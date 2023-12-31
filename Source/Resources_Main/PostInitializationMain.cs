using RimWorld;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace WVC_Tweaks
{

    // [StaticConstructorOnStartup]
	// public static class PostInitializationMain
	// {
		// static PostInitializationMain()
		// {
			// if (!WVC_Tweaks.settings.enableThingCategoryFilter)
			// {
				// return;
			// }
			// foreach (ThingDef thingDef in DefDatabase<ThingDef>.AllDefsListForReading)
			// {
				// if (thingDef.thingCategories.NullOrEmpty())
				// {
					// continue;
				// }
				// if (thingDef.stuffProps == null)
				// {
					// continue;
				// }
				// if (!thingDef.stuffProps.categories.NullOrEmpty())
				// {
					// if (thingDef.stuffProps.categories.Contains(StuffCategoryDefOf.Metallic))
					// {
						// thingDef.thingCategories.Add(TweaksDefOf.WVC_Tweaks_MetalResources_Raw);
					// }
					// if (thingDef.stuffProps.categories.Contains(StuffCategoryDefOf.Woody))
					// {
						// thingDef.thingCategories.Add(TweaksDefOf.WVC_Tweaks_WoodResources_Raw);
					// }
				// }
			// }
		// }
	// }
}