using RimWorld;
using System.Collections.Generic;
using Verse;

namespace WVC_Tweaks
{

    public class CompProperties_KillPawnIfFogged : CompProperties
	{

		public int refreshTicks = 12000;

		public CompProperties_KillPawnIfFogged()
		{
			compClass = typeof(CompKillPawnIfFogged);
		}
	}

	public class CompKillPawnIfFogged : ThingComp
	{

		private CompProperties_KillPawnIfFogged Props => (CompProperties_KillPawnIfFogged)props;

		public override void CompTick()
		{
			base.CompTick();
			Pawn pawn = parent as Pawn;
			if (!pawn.IsHashIntervalTick(Props.refreshTicks))
			{
				return;
			}
			KillIfFogged(pawn);
		}

		public void KillIfFogged(Pawn pawn)
		{
			if (pawn?.Map == null)
			{
				return;
			}
			// Pawn pawn = parent as Pawn;
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
