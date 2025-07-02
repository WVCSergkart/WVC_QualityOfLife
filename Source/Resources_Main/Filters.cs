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

	public class SpecialThingFilterWorker_PowerArmor : SpecialThingFilterWorker
	{
		public override bool Matches(Thing t)
		{
			if (t is Apparel)
			{
				return t.def.tradeTags?.Contains("HiTechArmor") == true;
			}
			return false;
		}

		public override bool CanEverMatch(ThingDef def)
		{
			return def.IsApparel;
		}
	}

	//public class SpecialThingFilterWorker_NonPowerArmor : SpecialThingFilterWorker
	//{
	//	public override bool Matches(Thing t)
	//	{
	//		if (t is Apparel)
	//		{
	//			return t.def.tradeTags?.Contains("HiTechArmor") != true;
	//		}
	//		return false;
	//	}

	//	public override bool CanEverMatch(ThingDef def)
	//	{
	//		return def.IsApparel;
	//	}
	//}
	public class SpecialThingFilterWorker_ShieldBelts : SpecialThingFilterWorker
	{
		public override bool Matches(Thing t)
		{
			if (t is Apparel)
			{
				return t.HasComp<CompShield>();
			}
			return false;
		}

		public override bool CanEverMatch(ThingDef def)
		{
			return def.IsApparel;
		}
	}

}
