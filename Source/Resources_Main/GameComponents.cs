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
			if (!WVC_Tweaks.settings.enableAutoCastResourcePermits)
			{
				return;
			}
			royaltyAutoCast--;
			if (royaltyAutoCast > 0)
			{
				return;
			}
			royaltyAutoCast = 60000;
			//if (Faction.OfEmpire.HostileTo(Faction.OfPlayer))
			//{
			//	return;
			//}
			foreach (Pawn noble in PawnsFinder.AllMaps_FreeColonists.ToList())
            {
                if (noble?.Map == null || noble.Map.generatorDef?.isUnderground == true || noble?.royalty == null)
                {
                    continue;
                }
				//Log.Error("Noble " + noble.Name);
				List<FactionPermit> permits = noble.royalty?.AllFactionPermits;
				if (permits == null)
				{
					continue;
				}
				foreach (FactionPermit permit in permits)
				{
					//Log.Error(permit.Permit.label);
					if (permit.OnCooldown)
					{
						continue;
					}
					IntVec3 targetCell = DropCellFinder.TradeDropSpot(noble.Map);
					if (targetCell == null)
					{
						continue;
					}
					RoyalTitlePermitDef royalTitlePermitDef = permit.Permit;
                    if (royalTitlePermitDef.Worker is RoyalTitlePermitWorker_DropResources)
					{
						MiscUtility.CallResources(targetCell, royalTitlePermitDef, permit.Faction, noble, !permit.OnCooldown);
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
            //Log.Error("0");
            if (ModsConfig.AnomalyActive && ShouldStartAnomalyProject())
            {
                //Log.Error("1");
                List<ResearchProjectDef> allDefsListForReading = DefDatabase<ResearchProjectDef>.AllDefsListForReading.Where((r) => r.CanStartNow).ToList();
                foreach (KnowledgeCategoryDef knowledgeCategoryDef in DefDatabase<KnowledgeCategoryDef>.AllDefsListForReading)
                {
                    //Log.Error("2");
                    ResearchProjectDef researchProjectDef = allDefsListForReading.Where((r) => r.knowledgeCategory == knowledgeCategoryDef).FirstOrDefault();
                    if (researchProjectDef != null)
                    {
                        Find.ResearchManager.SetCurrentProject(researchProjectDef);
                    }
                }
                //ResearchProjectDef advanced = allDefsListForReading.Where((r) => r.CanStartNow && r.knowledgeCategory == KnowledgeCategoryDefOf.Advanced).FirstOrDefault();
                //if (advanced != null)
                //{
                //	Find.ResearchManager.SetCurrentProject(advanced);
                //}
            }
        }

        private static bool ShouldStartAnomalyProject()
		{
			if (Find.ResearchManager.CurrentAnomalyKnowledgeProjects.NullOrEmpty())
			{
				return true;
			}
			foreach (ResearchManager.KnowledgeCategoryProject knowledgeCategoryProject in Find.ResearchManager.CurrentAnomalyKnowledgeProjects)
			{
				if (knowledgeCategoryProject.project != null)
				{
					return false;
				}
			}
			return true;
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