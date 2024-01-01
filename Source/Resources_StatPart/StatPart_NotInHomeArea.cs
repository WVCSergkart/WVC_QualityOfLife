// RimWorld.StatPart_Age
using RimWorld;
using Verse;

namespace WVC_Tweaks
{

	public class StatPart_NotInHomeArea : StatPart
	{

		public override void TransformValue(StatRequest req, ref float val)
		{
			if (req.HasThing && req.Thing is Thing thing && thing is not Pawn && !InHomeArea(thing))
			{
				val = 0f;
			}
		}

		public override string ExplanationPart(StatRequest req)
		{
			if (req.HasThing && req.Thing is Thing thing && thing is not Pawn && !InHomeArea(thing))
			{
				return "WVC_Tweaks_NotInHomeArea".Translate();
			}
			return null;
		}

		public bool InHomeArea(Thing thing)
		{
			if (thing.Map != null && thing.Map.areaManager.Home[thing.Position])
			{
				return true;
			}
			return false;
		}

	}

}
