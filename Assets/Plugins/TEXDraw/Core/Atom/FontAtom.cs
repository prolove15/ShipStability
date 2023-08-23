namespace TexDrawLib
{
    public static class FontAtom
    {
        
        public static Atom Get(string command, TexParserState state)
        {
            state.Font.current = FindFont(command, state);
            return null;
        }

        public static int FindFont(string command, TexParserState state)
        {
            if (state.parser.styleVariants.TryGetValue(command, out TexAssetStyle flag))
            {
                var font = TEXPreference.main.fonts[state.Font.current];
                if (font.metadata.style.HasFlag(flag))
                    return state.Font.current;
                var cumulativeFlag = flag == TexAssetStyle.Normal ? flag : flag | font.metadata.style;
                font = font.metadata.baseAsset ?? font;
                foreach (var item in font.metadata.variantAssets)
                {
                    if (item.metadata.style == cumulativeFlag)
                        return item.assetIndex;
                }
                foreach (var item in font.metadata.variantAssets)
                {
                    if (item.metadata.style.HasFlag(cumulativeFlag))
                        return item.assetIndex;
                }
            } else if (TEXPreference.main.fontnames.TryGetValue(command, out TexAsset asset))
            {
                return asset.assetIndex;
            }
            return state.Font.current;
        }

    }
}
