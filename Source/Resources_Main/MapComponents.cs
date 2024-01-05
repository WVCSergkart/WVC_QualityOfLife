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

	public class MapComponentTweaks : MapComponent
	{

		private int nextResearch = 0;
		private int nextCleaning = 0;
		private int nextLetterClose = 0;

		public MapComponentTweaks(Map map)
			: base(map)
		{
		}

		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.Look(ref nextCleaning, "nextCleaning", 0);
			Scribe_Values.Look(ref nextResearch, "nextResearch", 0);
			Scribe_Values.Look(ref nextLetterClose, "nextLetterClose", 0);
		}

		public override void MapComponentTick()
		{
			base.MapComponentTick();
			if (!WVC_Tweaks.settings.enableAutoFeatures)
			{
				return;
			}
			if (WVC_Tweaks.settings.enableAutoClosingLetters && Find.TickManager.TicksGame >= nextLetterClose)
			{
				LetterStack letterStack = Find.LetterStack;
				List<Letter> dismissibleLetters = letterStack.LettersListForReading.Where((Letter x) => x.CanDismissWithRightClick).ToList();
				if (dismissibleLetters.Count > 0)
				{
					for (int i = dismissibleLetters.Count - 1; i >= 0; i--)
					{
						Letter cur = dismissibleLetters[i];
						letterStack.RemoveLetter(cur);
					}
				}
				nextLetterClose = Find.TickManager.TicksGame + WVC_Tweaks.settings.frequencyAutoClosingLetters;
			}
			if (WVC_Tweaks.settings.enableAutoCleaning && Find.TickManager.TicksGame >= nextCleaning)
			{
				LetterStack letterStack = Find.LetterStack;
				List<Letter> dismissibleLetters = letterStack.LettersListForReading.Where((Letter x) => x.CanDismissWithRightClick).ToList();
				if (dismissibleLetters.Count > 0)
				{
					for (int i = dismissibleLetters.Count - 1; i >= 0; i--)
					{
						Letter cur = dismissibleLetters[i];
						letterStack.RemoveLetter(cur);
					}
				}
				List<IArchivable> archives = Find.Archive.ArchivablesListForReading;
				if (archives.Count > 0)
				{
					for (int i = archives.Count - 1; i >= 0; i--)
					{
						Find.Archive.Remove(archives[i]);
					}
				}
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
				Find.TaleManager.TaleManagerTick();
				Find.HistoryEventsManager.HistoryEventsManagerTick();
				nextCleaning = Find.TickManager.TicksGame + WVC_Tweaks.settings.frequencyAutoCleaning;
			}
			if (WVC_Tweaks.settings.enableAutoResearch && Find.TickManager.TicksGame >= nextResearch)
			{
				if (Find.ResearchManager.currentProj == null)
				{
					List<ResearchProjectDef> allDefsListForReading = DefDatabase<ResearchProjectDef>.AllDefsListForReading;
					foreach (TechLevel techLevel in from TechLevel tl in Enum.GetValues(typeof(TechLevel))
						orderby tl
						select tl)
					{
						ResearchProjectDef researchProjectDef = (from r in allDefsListForReading
							where !r.IsFinished && r.techLevel == techLevel
							orderby r.baseCost
							select r).FirstOrDefault();
						if (researchProjectDef != null)
						{
							Find.ResearchManager.currentProj = researchProjectDef;
							break;
						}
					}
				}
				nextResearch = Find.TickManager.TicksGame + WVC_Tweaks.settings.frequencyAutoResearch;
			}
		}

	}

}
