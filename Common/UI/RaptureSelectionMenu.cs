using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using ReLogic.Content;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.GameContent.UI.States;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.UI;
using Terraria.UI.Gamepad;
using VanillaPlus.Common.Systems;

namespace VanillaPlus.Common.UI
{
    /// <summary>
    /// IL hooks to add Rapture selection to world creation UI.
    /// Based on TheConfectionRebirth's ConfectionSelectionMenu.
    /// </summary>
    internal static class RaptureSelectionMenu
    {
        private static readonly GroupOptionButton<RaptureOptions>[] RaptureButtons =
            new GroupOptionButton<RaptureOptions>[Enum.GetValues<RaptureOptions>().Length];

        /// <summary>
        /// Expand the world creation container to make room for new options.
        /// </summary>
        public static void ILBuildPage(ILContext il)
        {
            var c = new ILCursor(il);

            // Increase world gen container size by 48 pixels
            c.GotoNext(i => i.MatchStloc(0));
            c.Emit(OpCodes.Ldc_I4, 48);
            c.Emit(OpCodes.Add);

            // Fix page position
            c.GotoNext(i => i.MatchLdcR4(170f))
                .GotoNext(i => i.MatchLdloc0());
            c.Emit(OpCodes.Ldc_R4, 38f);
            c.Emit(OpCodes.Add);
        }

        /// <summary>
        /// Inject the Rapture option buttons into the world creation menu.
        /// </summary>
        public static void ILMakeInfoMenu(ILContext il)
        {
            var c = new ILCursor(il);

            // Navigate to position to add options (after the last 48f height addition)
            c.Index = c.Instrs.Count - 1;
            c.GotoPrev(i => i.MatchLdcR4(48));
            c.GotoNext(i => i.MatchCall(out _));
            c.Index++;

            // Adding Rapture options
            c.Emit(OpCodes.Ldarg_0); // self
            c.Emit(OpCodes.Ldloc_0); // container
            c.Emit(OpCodes.Ldloc_1); // accumulatedHeight
            c.Emit(OpCodes.Ldloc, 10); // usableWidthPercent
            c.EmitDelegate((UIWorldCreation self, UIElement container, float accumulatedHeight, float usableWidthPercent) =>
                AddRaptureOptions(self, container, accumulatedHeight, ClickRaptureOption, "rapture", usableWidthPercent));
        }

        /// <summary>
        /// Set up gamepad navigation for the new buttons.
        /// </summary>
        internal static void ILSetUpGamepadPoints(ILContext il)
        {
            var c = new ILCursor(il);
            List<SnapPoint> snapGroupRapture = null;
            UILinkPoint[] arrayUW = null;

            c.GotoNext(MoveType.After, i => i.MatchLdarg0(), i => i.MatchLdloc1(), i => i.MatchLdstr("evil"), i => i.MatchCall<UIWorldCreation>("GetSnapGroup"), i => i.MatchStloc(10));
            c.EmitLdloc1(); // snapPoints
            c.EmitDelegate((List<SnapPoint> snapPoints) =>
            {
                snapGroupRapture = GetSnapGroup(snapPoints, "rapture");
            });

            c.GotoNext(MoveType.After, i => i.MatchLdloc(26), i => i.MatchLdloc(10), i => i.MatchCallvirt<List<SnapPoint>>("get_Count"), i => i.MatchBlt(out _));
            c.EmitLdloc0(); // num
            c.EmitLdloc(12); // uILinkPoint
            c.EmitDelegate((int num, UILinkPoint uILinkPoint) =>
            {
                arrayUW = new UILinkPoint[snapGroupRapture.Count];
                for (int l = 0; l < snapGroupRapture.Count; l++)
                {
                    UILinkPointNavigator.SetPosition(num, snapGroupRapture[l].Position);
                    uILinkPoint = UILinkPointNavigator.Points[num];
                    uILinkPoint.Unlink();
                    arrayUW[l] = uILinkPoint;
                    num++;
                }
            });

            c.GotoNext(MoveType.After, i => i.MatchLdloc(28), i => i.MatchLdloc(20), i => i.MatchLdlen(), i => i.MatchConvI4(), i => i.MatchBlt(out _));
            c.EmitLdloc(20); // array3 (Evils button)
            c.EmitLdloc(12); // uILinkPoint2 (Create button)
            c.EmitDelegate((UILinkPoint[] array3, UILinkPoint uILinkPoint2) =>
            {
                LoopHorizontalLineLinks(arrayUW);
                EstablishUpDownRelationship(array3, arrayUW);
                for (int n = 0; n < arrayUW.Length; n++)
                {
                    arrayUW[n].Down = uILinkPoint2.ID;
                }
            });

            c.GotoNext(MoveType.After, i => i.MatchLdloc(12), i => i.MatchLdloc(20), i => i.MatchLdcI4(0), i => i.MatchLdelemRef(), i => i.MatchLdfld<UILinkPoint>("ID"), i => i.MatchStfld<UILinkPoint>("Up"));
            c.EmitLdloc(20); // array3 (Evils button)
            c.EmitLdloc(13); // uILinkPoint3 (Back button)
            c.EmitLdloc(12); // uILinkPoint2 (Create button)
            c.EmitDelegate((UILinkPoint[] array3, UILinkPoint uILinkPoint3, UILinkPoint uILinkPoint2) =>
            {
                array3[^1].Down = arrayUW[^1].ID;
                arrayUW[^1].Down = uILinkPoint3.ID;
                uILinkPoint3.Up = arrayUW[^1].ID;
                uILinkPoint2.Up = arrayUW[0].ID;
            });
        }

        #region Helper Methods from UIWorldCreation
        private static List<SnapPoint> GetSnapGroup(List<SnapPoint> ptsOnPage, string groupName)
        {
            List<SnapPoint> list = ptsOnPage.Where((SnapPoint a) => a.Name == groupName).ToList();
            list.Sort(SortPoints);
            return list;
        }

        private static int SortPoints(SnapPoint a, SnapPoint b)
        {
            return a.Id.CompareTo(b.Id);
        }

        private static void LoopHorizontalLineLinks(UILinkPoint[] pointsLine)
        {
            for (int i = 1; i < pointsLine.Length - 1; i++)
            {
                pointsLine[i - 1].Right = pointsLine[i].ID;
                pointsLine[i].Left = pointsLine[i - 1].ID;
                pointsLine[i].Right = pointsLine[i + 1].ID;
                pointsLine[i + 1].Left = pointsLine[i].ID;
            }
        }

        private static void EstablishUpDownRelationship(UILinkPoint[] topSide, UILinkPoint[] bottomSide)
        {
            int num = Math.Max(topSide.Length, bottomSide.Length);
            for (int i = 0; i < num; i++)
            {
                int num2 = Math.Min(i, topSide.Length - 1);
                int num3 = Math.Min(i, bottomSide.Length - 1);
                topSide[num2].Down = bottomSide[num3].ID;
                bottomSide[num3].Up = topSide[num2].ID;
            }
        }
        #endregion

        /// <summary>
        /// Reset to default option when creating new world.
        /// </summary>
        public static void OnSetDefaultOptions(On_UIWorldCreation.orig_SetDefaultOptions orig, UIWorldCreation self)
        {
            orig(self);

            ModContent.GetInstance<RaptureWorldSystem>().SelectedRaptureOption = RaptureOptions.Random;
            foreach (GroupOptionButton<RaptureOptions> raptureButton in RaptureButtons)
            {
                raptureButton?.SetCurrentOption(RaptureOptions.Random);
            }
        }

        /// <summary>
        /// Handle description display for our buttons.
        /// </summary>
        public static void ILShowOptionDescription(ILContext il)
        {
            var c = new ILCursor(il);

            // Navigate to before final break
            c.Index = c.Instrs.Count - 1;
            c.GotoPrev(i => i.MatchBrfalse(out _));

            // Add description handling logic
            c.Emit(OpCodes.Pop);
            c.Emit(OpCodes.Ldloc_0); // localizedText
            c.Emit(OpCodes.Ldarg_2); // listeningElement
            c.EmitDelegate((LocalizedText localizedText, UIElement listeningElement) =>
                listeningElement is not GroupOptionButton<RaptureOptions> raptureButton ? localizedText : raptureButton.Description);
            c.Emit(OpCodes.Stloc_0);
            c.Emit(OpCodes.Ldloc_0);
        }

        /// <summary>
        /// Add the three Rapture option buttons to the container.
        /// </summary>
        private static void AddRaptureOptions(UIWorldCreation self, UIElement container, float accumulatedHeight,
                                              UIElement.MouseEvent clickEvent, string tagGroup,
                                              float usableWidthPercent)
        {
            LocalizedText[] titles =
            {
                Language.GetText("Mods.VanillaPlus.RaptureSelection.Random.Title"),
                Language.GetText("Mods.VanillaPlus.RaptureSelection.Hallow.Title"),
                Language.GetText("Mods.VanillaPlus.RaptureSelection.Rapture.Title"),
            };
            LocalizedText[] descriptions =
            {
                Language.GetText("Mods.VanillaPlus.RaptureSelection.Random.Description"),
                Language.GetText("Mods.VanillaPlus.RaptureSelection.Hallow.Description"),
                Language.GetText("Mods.VanillaPlus.RaptureSelection.Rapture.Description"),
            };
            Color[][] colors =
            {
                new[] { Color.White },
                new[] { Color.Cyan, Color.LightPink }, // Hallow cycles cyan/magenta
                new[] { Color.Yellow, Color.White, Color.LightBlue }, // Rapture cycles yellow/white/sky blue
            };
            Asset<Texture2D>[] icons =
            {
                Main.Assets.Request<Texture2D>("Images/UI/WorldCreation/IconEvilRandom"),
                ModContent.Request<Texture2D>("VanillaPlus/Assets/WorldCreation/IconGoodHallow"),
                ModContent.Request<Texture2D>("VanillaPlus/Assets/WorldCreation/IconGoodRapture"),
            };
            float[] cycleSpeeds =
            {
                1f,         // Random (no cycling)
                2f / 3f,    // Hallow cycles 2/3 as fast (2 colors)
                1f,         // Rapture (3 colors)
            };

            for (int i = 0; i < RaptureButtons.Length; i++)
            {
                var groupOptionButton = new RaptureGroupOptionButton<RaptureOptions>(
                    Enum.GetValues<RaptureOptions>()[i],
                    titles[i],
                    descriptions[i],
                    colors[i],
                    icons[i],
                    cycleSpeeds[i],
                    1f, // textSize
                    1f, // titleAlignmentX
                    16f)
                {
                    Width = StyleDimension.FromPixelsAndPercent(
                        -4 * (RaptureButtons.Length - 1),
                        1f / RaptureButtons.Length * usableWidthPercent),
                    Left = StyleDimension.FromPercent(1f - usableWidthPercent),
                    HAlign = i / (float)(RaptureButtons.Length - 1),
                };
                groupOptionButton.Top.Set(accumulatedHeight, 0f);
                groupOptionButton.OnLeftMouseDown += clickEvent;
                groupOptionButton.OnMouseOver += self.ShowOptionDescription;
                groupOptionButton.OnMouseOut += self.ClearOptionDescription;
                groupOptionButton.SetSnapPoint(tagGroup, i);
                container.Append(groupOptionButton);
                RaptureButtons[i] = groupOptionButton;
            }
        }

        /// <summary>
        /// Handle click on a Rapture option button.
        /// </summary>
        private static void ClickRaptureOption(UIMouseEvent evt, UIElement listeningElement)
        {
            var groupOptionButton = (GroupOptionButton<RaptureOptions>)listeningElement;
            ModContent.GetInstance<RaptureWorldSystem>().SelectedRaptureOption = groupOptionButton.OptionValue;

            foreach (GroupOptionButton<RaptureOptions> raptureButton in RaptureButtons)
            {
                raptureButton.SetCurrentOption(groupOptionButton.OptionValue);
            }
        }
    }
}
