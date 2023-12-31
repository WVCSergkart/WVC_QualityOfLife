using RimWorld;
using System.Collections.Generic;
using Verse;

namespace WVC_Tweaks
{

    public class CompProperties_KillPawnIfFogged : CompProperties
	{

		public CompProperties_KillPawnIfFogged()
		{
			compClass = typeof(CompKillPawnIfFogged);
		}
	}

	public class CompKillPawnIfFogged : ThingComp
	{

		public override void CompTick()
		{
			base.CompTick();
			Pawn pawn = parent as Pawn;
			if (!pawn.IsHashIntervalTick(6000))
			{
				return;
			}
			KillIfFogged();
		}

		public void KillIfFogged()
		{
			Pawn pawn = parent as Pawn;
			if (pawn.Map.fogGrid.IsFogged(pawn.Position) && (pawn.Faction == null || pawn.Faction != Faction.OfPlayer))
			{
				if (Prefs.DevMode)
				{
					Log.Warning(pawn.LabelCap + " should not exist, cause fogged.");
				}
				pawn.Kill(null, null);
				// pawn.DeSpawn();
			}
		}

	}

}
