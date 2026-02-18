using System.Reflection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.Localization;
using Terraria.UI;

namespace VanillaPlus.Common.UI
{
    /// <summary>
    /// Custom GroupOptionButton that supports Asset<Texture2D> icons and cycling text colors.
    /// Based on TheConfectionRebirth's implementation.
    /// </summary>
    public class RaptureGroupOptionButton<T> : GroupOptionButton<T>
    {
        private readonly Color[] _cycleColors;
        private readonly float _cycleSpeed;
        private UIText _titleText;

        public RaptureGroupOptionButton(T option, LocalizedText title, LocalizedText description, Color textColor,
                                        string iconTexturePath, float textSize = 1, float titleAlignmentX = 0.5f,
                                        float titleWidthReduction = 10)
            : base(option, title, description, textColor, iconTexturePath, textSize, titleAlignmentX, titleWidthReduction)
        {
            _cycleColors = null;
            _cycleSpeed = 0f;
        }

        public RaptureGroupOptionButton(T option, LocalizedText title, LocalizedText description, Color textColor,
                                        Asset<Texture2D> iconTexture, float textSize = 1, float titleAlignmentX = 0.5f,
                                        float titleWidthReduction = 10)
            : this(option, title, description, textColor, (string)null, textSize, titleAlignmentX, titleWidthReduction)
        {
            typeof(GroupOptionButton<T>)
                .GetField("_iconTexture", BindingFlags.Instance | BindingFlags.NonPublic)?.SetValue(this, iconTexture);
        }

        public RaptureGroupOptionButton(T option, LocalizedText title, LocalizedText description, Color[] cycleColors,
                                        Asset<Texture2D> iconTexture, float cycleSpeed = 2f, float textSize = 1,
                                        float titleAlignmentX = 0.5f, float titleWidthReduction = 10)
            : this(option, title, description, cycleColors[0], iconTexture, textSize, titleAlignmentX, titleWidthReduction)
        {
            _cycleColors = cycleColors;
            _cycleSpeed = cycleSpeed;
        }

        private void FindTitleText()
        {
            if (_titleText != null) return;

            // The title is stored in _title field as a UIText
            var titleField = typeof(GroupOptionButton<T>).GetField("_title", BindingFlags.Instance | BindingFlags.NonPublic);
            _titleText = titleField?.GetValue(this) as UIText;
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            if (_cycleColors != null && _cycleColors.Length > 1)
            {
                FindTitleText();

                if (_titleText != null)
                {
                    float time = Main.GlobalTimeWrappedHourly * _cycleSpeed;
                    float progress = time % _cycleColors.Length;
                    int colorIndex = (int)progress;
                    int nextColorIndex = (colorIndex + 1) % _cycleColors.Length;
                    float lerpAmount = progress - colorIndex;

                    Color currentColor = Color.Lerp(_cycleColors[colorIndex], _cycleColors[nextColorIndex], lerpAmount);
                    _titleText.TextColor = currentColor;
                }
            }

            base.DrawSelf(spriteBatch);
        }
    }
}
