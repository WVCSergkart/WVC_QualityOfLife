using RimWorld;
using System.Collections.Generic;
using Verse;

namespace WVC_Tweaks
{

	public static class MiscUtility
	{

		public static void CallResources(IntVec3 cell, RoyalTitlePermitDef def, Faction faction, Pawn caller, bool free)
		{
			List<Thing> list = new();
			for (int i = 0; i < def.royalAid.itemsToDrop.Count; i++)
			{
				Thing thing = ThingMaker.MakeThing(def.royalAid.itemsToDrop[i].thingDef);
				thing.stackCount = def.royalAid.itemsToDrop[i].count;
				list.Add(thing);
			}
			if (list.Any())
			{
				ActiveDropPodInfo activeDropPodInfo = new();
				activeDropPodInfo.innerContainer.TryAddRangeOrTransfer(list);
				DropPodUtility.MakeDropPodAt(cell, caller.Map, activeDropPodInfo);
				Messages.Message("MessagePermitTransportDrop".Translate(faction.Named("FACTION")), new LookTargets(cell, caller.Map), MessageTypeDefOf.NeutralEvent);
				caller.royalty.GetPermit(def, faction).Notify_Used();
				if (!free)
				{
					caller.royalty.TryRemoveFavor(faction, def.royalAid.favorCost);
				}
			}
		}

	}

}
