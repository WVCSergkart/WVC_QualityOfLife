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
		// public bool enableThingCategoryFilter = false;

		public IEnumerable<string> GetEnabledSettings => from specificSetting in GetType().GetFields()
														 where specificSetting.FieldType == typeof(bool) && (bool)specificSetting.GetValue(this)
														 select specificSetting.Name;

		public override void ExposeData()
		{
			// Misc
			// Scribe_Values.Look(ref enableThingCategoryFilter, "enableThingCategoryFilter", defaultValue: false);
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
			// listingStandard.Label("WVC_TweaksSettings_Label_Misc".Translate() + ":", -1);
			// listingStandard.CheckboxLabeled("WVC_Label_TweaksSettings_enableThingCategoryFilter".Translate() , ref settings.enableThingCategoryFilter, "WVC_Tooltip_TweaksSettings_enableThingCategoryFilter".Translate());
			// ===============
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
}
