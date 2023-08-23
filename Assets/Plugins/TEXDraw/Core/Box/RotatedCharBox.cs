using UnityEngine;

namespace TexDrawLib
{
    public class RotatedCharBox : Box
    {

        public static RotatedCharBox Get(TexChar ch, float scale, float resolution, Color32 color)
        {
            var box = ObjPool<RotatedCharBox>.Get();
            var font = ch.Font;
            box.ch = ch;
            box.font = font.assetIndex;
            box.color = color;


            switch (box.type = font.type)
            {
                case TexAssetType.Font:
                    var c = box.c = ((TexFont)font).GenerateFont(ch.characterIndex,
                        (int)(resolution * scale) + 1, FontStyle.Normal);
                    float ratio = scale / c.size;
                    box.Set(c.maxX * ratio, (-c.minX) * ratio, 0, (c.maxY - c.minY) * ratio);
                    return box;
                case TexAssetType.Sprite:
                    {
                        var b = (box.o = (TexSprite)font).GenerateMetric(ch.characterIndex);
                        box.uv = b.uv; var s = b.size;
                        box.Set(s.z, s.x, s.w, s.y);
                        if (((TexSprite)font).alphaOnly)
                        {
                            box.color = new Color32(0, 0, 0, box.color.a);
                        }
                    }
                    return box;
#if TEXDRAW_TMP
                case TexAssetType.FontSigned:
                    {
                        var b = ((TexFontSigned)font).GenerateMetric(ch.characterIndex);
                        box.uv = b.uv; var s = b.size;
                        box.Set(s.z, s.x, s.w, s.y);
                    }
                    return box;
#endif
                default:
                    return null;
            }
        }

        public TexChar ch;

        private int font;

        private CharacterInfo c;

        private TexSprite o;

        private Rect uv;

        private Color32 color;

        public float bearing, italic;

        public TexAssetType type;

        private new void Set(float depth, float height, float bearing, float italic)
        {
            this.depth = depth;
            this.height = height;
            this.bearing = bearing;
            this.italic = italic;
            this.width = italic + bearing;
            this.shift = 0;
        }

        public override void Draw(TexRendererState state)
        {
            // Draw character at given position.
            var rect = new Rect((state.x - bearing), (state.y - depth), (bearing + italic), (depth + height));

            switch (type)
            {
                case TexAssetType.Font:
#pragma warning disable CS0618 // Type or member is obsolete
                    state.Draw(new TexRendererState.FlexibleUVQuadState(font, rect, c.uvBottomRight, c.uvTopRight, c.uvTopLeft, c.uvBottomLeft, color, !c.flipped));
#pragma warning restore CS0618 // Type or member is obsolete

                    break;
                case TexAssetType.Sprite:
                    state.Draw(new TexRendererState.FlexibleUVQuadState(font, rect,
                            uv.min, new Vector2(uv.xMax, uv.y),
                            uv.max, new Vector2(uv.x, uv.yMax),
                            color, true));
                    break;
#if TEXDRAW_TMP
                case TexAssetType.FontSigned:
                    {
                    state.Draw(new TexRendererState.FlexibleUVQuadState(font, rect,
                            uv.min, new Vector2(uv.xMax, uv.y),
                            uv.max, new Vector2(uv.x, uv.yMax),
                            color, true));
                    }
                    break;
#endif
            }
        }

        public override void Flush()
        {
            ObjPool<RotatedCharBox>.Release(this);
        }
    }
}
