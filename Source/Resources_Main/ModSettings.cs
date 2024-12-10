using RimWorld;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using UnityEngine;
using Verse;

namespace WVC_Tweaks
{

	public class WVC_TweakSettings : ModSettings
	{
		// Misc
		public bool enableBuildingsStuffPatch = false;
		public bool enableBuildingsMetalStuffPatch = false;
		public bool enableJunkEmptyPrecepts = false;
		public bool enableVanillaMemesForFactions = false;
		public bool enableAutoFeatures = false;
		public bool enableAutoCastResourcePermits = false;
		// Auto
		public bool enableAutoClosingLetters = false;
		public bool enableAutoCleaning = false;
		public bool enableAutoResearch = false;
		public int frequencyAutoClosingLetters = 40000;
		public int frequencyAutoCleaning = 160000;
		public int frequencyAutoResearch = 60000;

		public IEnumerable<string> GetEnabledSettings => from specificSetting in GetType().GetFields()
														 where specificSetting.FieldType == typeof(bool) && (bool)specificSetting.GetValue(this)
														 select specificSetting.Name;

		public override void ExposeData()
		{
			// Misc
			Scribe_Values.Look(ref enableBuildingsStuffPatch, "enableBuildingsStuffPatch", defaultValue: false);
			Scribe_Values.Look(ref enableBuildingsMetalStuffPatch, "enableBuildingsMetalStuffPatch", defaultValue: false);
			Scribe_Values.Look(ref enableJunkEmptyPrecepts, "enableJunkEmptyPrecepts", defaultValue: false);
			Scribe_Values.Look(ref enableVanillaMemesForFactions, "enableVanillaMemesForFactions", defaultValue: false);
			Scribe_Values.Look(ref enableAutoFeatures, "enableAutoFeatures", defaultValue: false);
			Scribe_Values.Look(ref enableAutoCastResourcePermits, "enableAutoCastResourcePermits", defaultValue: false);
			// Auto
			Scribe_Values.Look(ref enableAutoClosingLetters, "enableAutoClosingLetters", defaultValue: false);
			Scribe_Values.Look(ref enableAutoCleaning, "enableAutoCleaning", defaultValue: false);
			Scribe_Values.Look(ref enableAutoResearch, "enableAutoResearch", defaultValue: false);
			Scribe_Values.Look(ref frequencyAutoClosingLetters, "frequencyAutoClosingLetters", defaultValue: 40000);
			Scribe_Values.Look(ref frequencyAutoCleaning, "frequencyAutoCleaning", defaultValue: 160000);
			Scribe_Values.Look(ref frequencyAutoResearch, "frequencyAutoResearch", defaultValue: 60000);
		}
	}

	public class WVC_Tweaks : Mod
	{
		public static WVC_TweakSettings settings;

		private int PageIndex = 0;

		private static Vector2 scrollPosition = Vector2.zero;

		public WVC_Tweaks(ModContentPack content) : base(content)
		{
			settings = GetSettings<WVC_TweakSettings>();
		}

		public override void DoSettingsWindowContents(Rect inRect)
		{
			Rect rect = new(inRect);
			rect.y = inRect.y + 40f;
			Rect baseRect = rect;
			rect = new Rect(inRect)
			{
				height = inRect.height - 40f,
				y = inRect.y + 40f
			};
			Rect rect2 = rect;
			Widgets.DrawMenuSection(rect2);
			List<TabRecord> tabs = new()
			{
				new TabRecord("WVC_TweaksSettings_Tab_General".Translate(), delegate
				{
					PageIndex = 0;
					WriteSettings();
				}, PageIndex == 0),
			};
			TabDrawer.DrawTabs(baseRect, tabs);
			switch (PageIndex)
			{
				case 0:
					GeneralSettings(rect2.ContractedBy(15f));
					break;
			}
		}

		// General Settings
		// General Settings
		// General Settings

		private void GeneralSettings(Rect inRect)
		{
			Rect outRect = new(inRect.x, inRect.y, inRect.width, inRect.height);
			// Rect rect = new(0f, 0f, inRect.width, inRect.height);
			Rect rect = new(0f, 0f, inRect.width - 30f, inRect.height * 2);
			// var labelRect = new Rect(rect.x + 5, rect.y, 60, 24);
			// var resetRect = new Rect(labelRect.x, labelRect.yMax + 5, 265, 24f);
			Widgets.BeginScrollView(outRect, ref scrollPosition, rect);
			Listing_Standard listingStandard = new();
			listingStandard.Begin(rect);
			// ===============
			listingStandard.Label("WVC_TweaksSettings_Label_Misc".Translate() + ":", -1);
			listingStandard.CheckboxLabeled("WVC_Label_TweaksSettings_enableBuildingsStuffPatch".Translate() , ref settings.enableBuildingsStuffPatch, "WVC_Tooltip_TweaksSettings_enableBuildingsStuffPatch".Translate());
			listingStandard.CheckboxLabeled("WVC_Label_TweaksSettings_enableBuildingsMetalStuffPatch".Translate(), ref settings.enableBuildingsMetalStuffPatch, "WVC_Tooltip_TweaksSettings_enableBuildingsMetalStuffPatch".Translate());
			listingStandard.CheckboxLabeled("WVC_Label_TweaksSettings_enableJunkEmptyPrecepts".Translate() , ref settings.enableJunkEmptyPrecepts, "WVC_Tooltip_TweaksSettings_enableJunkEmptyPrecepts".Translate());
			listingStandard.CheckboxLabeled("WVC_Label_TweaksSettings_enableVanillaMemesForFactions".Translate() , ref settings.enableVanillaMemesForFactions, "WVC_Tooltip_TweaksSettings_enableVanillaMemesForFactions".Translate());
			listingStandard.GapLine();
			// ===============
			listingStandard.CheckboxLabeled("WVC_Label_TweaksSettings_enableAutoFeatures".Translate() , ref settings.enableAutoFeatures, "WVC_Tooltip_TweaksSettings_enableAutoFeatures".Translate());
			// ===============
			listingStandard.CheckboxLabeled("WVC_Label_TweaksSettings_enableAutoCastResourcePermits".Translate(), ref settings.enableAutoCastResourcePermits, "WVC_Tooltip_TweaksSettings_enableAutoCastResourcePermits".Translate());
			// listingStandard.Gap();
			listingStandard.CheckboxLabeled("WVC_Label_TweaksSettings_enableAutoClosingLetters".Translate() , ref settings.enableAutoClosingLetters, "WVC_Tooltip_TweaksSettings_enableAutoClosingLetters".Translate());
			if (settings.enableAutoClosingLetters)
			{
				float val = settings.frequencyAutoClosingLetters;
				listingStandard.SliderLabeledWithRef("WVC_Label_TweaksSettings_frequencyAutoClosingLetters".Translate(settings.frequencyAutoClosingLetters.ToStringTicksToDays()), ref val, 12000f, 360000f);
				settings.frequencyAutoClosingLetters = (int)val;
			}
			// ===============
			// listingStandard.Gap();
			listingStandard.CheckboxLabeled("WVC_Label_TweaksSettings_enableAutoCleaning".Translate() , ref settings.enableAutoCleaning, "WVC_Tooltip_TweaksSettings_enableAutoCleaning".Translate());
			if (settings.enableAutoCleaning)
			{
				float val = settings.frequencyAutoCleaning;
				listingStandard.SliderLabeledWithRef("WVC_Label_TweaksSettings_frequencyAutoCleaning".Translate(settings.frequencyAutoCleaning.ToStringTicksToDays()), ref val, 20000f, 360000f);
				settings.frequencyAutoCleaning = (int)val;
			}
			// ===============
			// listingStandard.Gap();
			listingStandard.CheckboxLabeled("WVC_Label_TweaksSettings_enableAutoResearch".Translate() , ref settings.enableAutoResearch, "WVC_Tooltip_TweaksSettings_enableAutoResearch".Translate());
			if (settings.enableAutoResearch)
			{
				float val = settings.frequencyAutoResearch;
				listingStandard.SliderLabeledWithRef("WVC_Label_TweaksSettings_frequencyAutoResearch".Translate(settings.frequencyAutoResearch.ToStringTicksToDays()), ref val, 1500f, 360000f);
				settings.frequencyAutoResearch = (int)val;
			}
			// ===============
			listingStandard.GapLine();
			//listingStandard.Gap();
			// ===============
			listingStandard.End();
			Widgets.EndScrollView();
		}

		public override string SettingsCategory()
		{
			return "WVC - " + "WVC_TweaksSettings".Translate();
		}
	}

	public class PatchOperationOptional : PatchOperation
	{
		public string settingName;
		public PatchOperation caseTrue;
		public PatchOperation caseFalse;

		protected override bool ApplyWorker(XmlDocument xml)
		{
			if (WVC_Tweaks.settings.GetEnabledSettings.Contains(settingName) && caseTrue != null)
			{
				return caseTrue.Apply(xml);
			}
			else if (WVC_Tweaks.settings.GetEnabledSettings.Contains(settingName) != true && caseFalse != null)
			{
				return caseFalse.Apply(xml);
			}
			return true;
		}
	}

	internal static class SettingsHelper
	{

		public static void SliderLabeledWithRef(this Listing_Standard ls, string label, ref float val, float min = 0f, float max = 1f, string tooltip = null)
		{
			Rect rect = ls.GetRect(Text.LineHeight);
			Rect rect2 = rect.LeftPart(0.5f).Rounded();
			Rect rect3 = rect.RightPart(0.62f).Rounded().LeftPart(0.97f).Rounded();
			// Rect rect4 = rect.RightPart(0.1f).Rounded();
			TextAnchor anchor = Text.Anchor;
			Text.Anchor = TextAnchor.MiddleLeft;
			Widgets.Label(rect2, label);
			float _ = (val = Widgets.HorizontalSlider(rect3, val, min, max, middleAlignment: true));
			Text.Anchor = TextAnchor.MiddleRight;
			// Widgets.Label(rect4, string.Format(format, val));
			if (!tooltip.NullOrEmpty())
			{
				TooltipHandler.TipRegion(rect, tooltip);
			}
			Text.Anchor = anchor;
			ls.Gap(ls.verticalSpacing);
		}

	}

}
