using UnityEngine;
#if TEXDRAW_TMP
using System.Collections.Generic;
using System.IO;
using TMPro;
#if UNITY_EDITOR
using UnityEditor;
#endif
#endif

namespace TexDrawLib
{
    public class TexFontSigned : TexAsset
    {
        public override TexAssetType type { get { return TexAssetType.FontSigned; } }

        public string rawpath;

#if TEXDRAW_TMP

        public override float LineHeight() { return asset.faceInfo.lineHeight / asset.faceInfo.pointSize; }

        public override float SpaceWidth() { return asset.faceInfo.meanLine / asset.faceInfo.pointSize; }

        public override Texture2D Texture() { return asset.atlasTexture; }

        public TMP_FontAsset asset;

        protected Dictionary<char, SpriteMetrics> assetmetrices = new Dictionary<char, SpriteMetrics>();

        public SpriteMetrics GenerateMetric(char ch)
        {
            if (asset.atlasPopulationMode == AtlasPopulationMode.Dynamic)
            {
                if (!asset.HasCharacter(ch))
                {
                    var s = TexParserUtility.Char2Str(ch);
                    Debug.Assert(asset.TryAddCharacters(s));
                }

                var c = asset.characterLookupTable[(uint)ch].glyph;
                var factor = c.scale / asset.faceInfo.pointSize;
                var padding = asset.atlasPadding;
                float invW = 1.0f / asset.atlasWidth, invH = 1.0f / asset.atlasHeight;

                return new SpriteMetrics()
                {
                    size = new Vector4()
                    {
                        x = (-c.metrics.horizontalBearingX + 0) * factor,
                        y = (c.metrics.height - c.metrics.horizontalBearingY + 0) * factor,
                        z = (c.metrics.width + c.metrics.horizontalBearingX + 0) * factor,
                        w = (c.metrics.horizontalBearingY + 0) * factor,
                    },
                    advance = c.metrics.horizontalAdvance * factor,
                    uv = new Rect()
                    {
                        x = (c.glyphRect.x - padding) * invW,
                        y = (c.glyphRect.y - padding) * invH,
                        width = (c.glyphRect.width + 2 * padding) * invW,
                        height = (c.glyphRect.height + 2 * padding) * invH
                    }
                };
            }


            return assetmetrices[ch];
        }

        public override void ImportDictionary()
        {
            base.ImportDictionary();

            if (!asset) return;

            // sanitize input

            assetmetrices.Clear();

            asset.ReadFontAssetDefinition();

            if (asset.atlasPopulationMode == AtlasPopulationMode.Dynamic) return;

            var info = asset.faceInfo;

            var padding = asset.atlasPadding;

            float invW = 1.0f / asset.atlasWidth, invH = 1.0f / asset.atlasHeight;

            for (int i = 0; i < editorMetadata.catalogs.Length; i++)
            {
                var ch = editorMetadata.catalogs[i];
                if (!asset.characterLookupTable.ContainsKey(ch))
                {
                    assetmetrices[ch] = new SpriteMetrics();
                    continue;
                }

                var c = asset.characterLookupTable[ch].glyph;

                var factor = c.scale / info.pointSize;

                assetmetrices[ch] = new SpriteMetrics()
                {
                    size = new Vector4()
                    {
                        x = (-c.metrics.horizontalBearingX + 0) * factor,
                        y = (c.metrics.height - c.metrics.horizontalBearingY + 0) * factor,
                        z = (c.metrics.width + c.metrics.horizontalBearingX + 0) * factor,
                        w = (c.metrics.horizontalBearingY + 0) * factor,
                    },
                    advance = c.metrics.horizontalAdvance * factor,
                    uv = new Rect()
                    {
                        x = (c.glyphRect.x - padding) * invW,
                        y = (c.glyphRect.y - padding) * invH,
                        width = (c.glyphRect.width + 2 * padding) * invW,
                        height = (c.glyphRect.height + 2 * padding) * invH
                    }
                };
            }
        }

#else

        public override float LineHeight() { return 0; }

        public override float SpaceWidth() { return 0; }

        public override Texture2D Texture() { return null; }

#endif

#if UNITY_EDITOR

        public override void ImportAsset(string path)
        {
#if TEXDRAW_TMP
            if (path.EndsWith(".asset"))
            {
                asset = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(path);
                if (!asset)
                {
                    var font = AssetDatabase.LoadAssetAtPath<Font>(rawpath);
                    asset = TMP_FontAsset.CreateFontAsset(font, 16, 2, UnityEngine.TextCore.LowLevel.GlyphRenderMode.SDFAA, 512, 512, AtlasPopulationMode.Static);
                    AssetDatabase.CreateAsset(asset, path);

                    // Create new Material and Add it as Sub-Asset
                    Shader default_Shader = Shader.Find("TextMeshPro/Distance Field");
                    Material tmp_material = new Material(default_Shader);

                    tmp_material.name = name + " Material";

                    asset.material = tmp_material;

                    AssetDatabase.AddObjectToAsset(tmp_material, asset);
                }
            }
            else if (path.EndsWith(".ttf") || path.EndsWith(".otf"))
            {
                rawpath = path;
                path = Path.GetDirectoryName(Path.GetDirectoryName(path));
                path += "/TMPro/" + this.name + ".asset";
                ImportAsset(path);
            }
#endif
        }

#endif
    }
}
