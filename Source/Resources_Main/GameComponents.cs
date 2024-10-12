using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using Verse;
using Verse.AI;

namespace WVC_Tweaks
{

	public class QoL_GameComponent : GameComponent
	{

		public int nextResearch = 0;
		public int nextCleaning = 0;
		public int nextLetterClose = 0;

		public Game currentGame;

		public QoL_GameComponent(Game game)
		{
			currentGame = game;
		}

		public override void GameComponentTick()
		{
			Tweaks();
		}

		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.Look(ref nextCleaning, "autoFeatures_nextCleaning", 0);
			Scribe_Values.Look(ref nextResearch, "autoFeatures_nextResearch", 0);
			Scribe_Values.Look(ref nextLetterClose, "autoFeatures_nextLetterClose", 0);
		}

		private int royaltyAutoCast = 1500;

		public void Royalty()
		{
			royaltyAutoCast--;
			if (royaltyAutoCast > 0 || !WVC_Tweaks.settings.enableAutoCastResourcePermits)
			{
				return;
			}
			royaltyAutoCast = 60000;
			if (Faction.OfEmpire.HostileTo(Faction.OfPlayer))
			{
				return;
			}
			foreach (Pawn noble in PawnsFinder.AllMaps_FreeColonists.ToList())
            {
                if (noble?.Map == null || noble.Map.generatorDef?.isUnderground == true || noble?.royalty == null)
                {
                    continue;
                }
				//Log.Error("Noble " + noble.Name);
				List<FactionPermit> permits = noble.royalty.PermitsFromFaction(Faction.OfEmpire);
				foreach (FactionPermit permit in permits)
				{
					//Log.Error(permit.Permit.label);
					if (permit.OnCooldown)
					{
						continue;
					}
					RoyalTitlePermitDef royalTitlePermitDef = permit.Permit;
					if (royalTitlePermitDef.Worker is RoyalTitlePermitWorker_DropResources dropResources && noble.royalty.HasPermit(royalTitlePermitDef, Faction.OfEmpire) && noble.Map.AllCells.Where((IntVec3 cell) => !cell.IsForbidden(noble) && !cell.Roofed(noble.Map) && noble.CanReach(cell, PathEndMode.OnCell, Danger.Deadly)).TryRandomElement(out IntVec3 target))
					{
						MiscUtility.CallResources(target, permit.Permit, Faction.OfEmpire, noble, !permit.OnCooldown);
					}
				}
            }
        }

		public void Tweaks()
		{
			if (!WVC_Tweaks.settings.enableAutoFeatures)
			{
				return;
			}
			Royalty();
			if (Find.TickManager.TicksGame >= nextLetterClose)
			{
				if (WVC_Tweaks.settings.enableAutoClosingLetters)
				{
					CleanLetterStack();
				}
				nextLetterClose = Find.TickManager.TicksGame + WVC_Tweaks.settings.frequencyAutoClosingLetters;
			}
			if (Find.TickManager.TicksGame >= nextCleaning)
			{
				if (WVC_Tweaks.settings.enableAutoCleaning)
				{
					CleanLetterStack();
					CleanArchive();
					CleanQuestHistory();
				}
				nextCleaning = Find.TickManager.TicksGame + WVC_Tweaks.settings.frequencyAutoCleaning;
			}
			if (Find.TickManager.TicksGame >= nextResearch)
			{
				if (WVC_Tweaks.settings.enableAutoResearch)
				{
					StartResearch();
				}
				nextResearch = Find.TickManager.TicksGame + WVC_Tweaks.settings.frequencyAutoResearch;
			}
		}

		public static void StartResearch()
		{
			if (Find.ResearchManager.GetProject() == null)
			{
				List<ResearchProjectDef> allDefsListForReading = DefDatabase<ResearchProjectDef>.AllDefsListForReading;
				foreach (TechLevel techLevel in from TechLevel tl in Enum.GetValues(typeof(TechLevel))
												orderby tl
												select tl)
				{
					ResearchProjectDef researchProjectDef = (from r in allDefsListForReading
															 where r.CanStartNow && r.techLevel == techLevel && r.tab.minMonolithLevelVisible < 0
															 orderby r.baseCost
															 select r).FirstOrDefault();
					if (researchProjectDef != null)
					{
						Find.ResearchManager.SetCurrentProject(researchProjectDef);
						break;
					}
				}
			}
		}

		public static void CleanQuestHistory()
		{
			List<Quest> quests = Find.QuestManager.QuestsListForReading;
			if (quests.Count > 0)
			{
				for (int i = quests.Count - 1; i >= 0; i--)
				{
					if (quests[i].Historical)
					{
						Find.QuestManager.Remove(quests[i]);
					}
				}
			}
		}

		public static void CleanArchive()
		{
			List<IArchivable> archives = Find.Archive.ArchivablesListForReading;
			if (archives.Count > 0)
			{
				for (int i = archives.Count - 1; i >= 0; i--)
				{
					Find.Archive.Remove(archives[i]);
				}
			}
		}

		public static void CleanLetterStack()
		{
			LetterStack letterStack = Find.LetterStack;
			List<Letter> dismissibleLetters = letterStack.LettersListForReading.Where((Letter x) => x.CanDismissWithRightClick && x.arrivalTick + 60000 < Find.TickManager.TicksGame).ToList();
			if (dismissibleLetters.Count > 0)
			{
				for (int i = dismissibleLetters.Count - 1; i >= 0; i--)
				{
					Letter cur = dismissibleLetters[i];
					letterStack.RemoveLetter(cur);
				}
			}
		}

	}
}