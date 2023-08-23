using System;
using UnityEngine;

namespace TexDrawLib
{
    public class BigOperatorAtom : ScriptsAtom
    {
        public SymbolAtom sublimit, superlimit;

        private static Box ExpandWidth(Box box, float maxWidth)
        {
            // Centre specified box in new box of specified width, if necessary.
            if (Math.Abs(maxWidth - box.width) > Math.E)
            {
                return HorizontalBox.Get(box, maxWidth, TexAlignment.Center);
            }
            else
                return box;
        }

        public new static ScriptsAtom Get()
        {
            return ObjPool<BigOperatorAtom>.Get();
        }

        public new static BigOperatorAtom Get(Atom atom)
        {
            var a = ObjPool<BigOperatorAtom>.Get();
            a.atom = atom;
            return a;
        }

        // True then script will drawn smaller.
        public bool textMode;

        public override Box CreateBox(TexBoxingState state)
        {
            if (textMode || (subscript == null && superscript == null))
            {
                return base.CreateBox(state);
            }

            // Create box for base atom.
            Box baseBox;
            bool isBigOp = false;
            float delta;

            if (atom is SymbolAtom satom && satom.Type == CharType.BigOperator)
            {
                isBigOp = true;
                // Find character of best scale for operator symbol.
                var opChar = GetDisplayBox(satom);
                TexParserUtility.CentreBox(opChar, median);
                opChar.shift = -(opChar.height + opChar.depth) / 2 - median;
                baseBox = HorizontalBox.Get(opChar);
                delta = opChar.bearing;
            }
            else
            {
                baseBox = HorizontalBox.Get(atom == null ? StrutBox.Empty : atom.CreateBox(state));
                delta = 0;
            }

            // Create boxes for upper and lower limits.
            Box upperLimitBox, lowerLimitBox;
            if (superscript is SymbolAtom)
                upperLimitBox = ((SymbolAtom)superscript).CreateBoxMinWidth(baseBox.width, state);
            else
                upperLimitBox = superscript?.CreateBox(state);

            if (subscript is SymbolAtom)
                lowerLimitBox = ((SymbolAtom)subscript).CreateBoxMinWidth(baseBox.width, state);
            else
                lowerLimitBox = subscript?.CreateBox(state);

            if (atom is SymbolAtom ssatom && (ssatom.metadata.larger.Has || ssatom.metadata.extension.enabled))
            {
                baseBox.Flush();
                baseBox = ssatom.CreateBoxMinWidth(Mathf.Max(
                    upperLimitBox != null ? upperLimitBox.width : 0,
                    lowerLimitBox != null ? lowerLimitBox.width : 0
                    ), state);
            }

            if (sublimit != null || superlimit != null)
            {
                var verbox = VerticalBox.Get(baseBox);
                if (superlimit != null)
                {
                    verbox.Add(0, StrutBox.Get(0, upMargin, 0));
                    var ext = superlimit.CreateBoxMinWidth(baseBox.width, state);
                    baseBox.shift = (ext.width - baseBox.width) / 2;
                    verbox.Add(0, ext);
                }
                if (sublimit != null)
                {
                    verbox.Add(StrutBox.Get(0, downMargin, 0));
                    var ext = sublimit.CreateBoxMinWidth(baseBox.width, state);
                    baseBox.shift = (ext.width - baseBox.width) / 2;
                    verbox.Add(ext);
                }
                baseBox = verbox;
            }

            // Make all component boxes equally wide.
            var maxWidth = Mathf.Max(Mathf.Max(baseBox.width, upperLimitBox == null ? 0 : upperLimitBox.width),
                      lowerLimitBox == null ? 0 : lowerLimitBox.width);

            if (baseBox != null)
                baseBox = ExpandWidth(baseBox, maxWidth);
            if (upperLimitBox != null)
                upperLimitBox = ExpandWidth(upperLimitBox, maxWidth);
            if (lowerLimitBox != null)
                lowerLimitBox = ExpandWidth(lowerLimitBox, maxWidth);

            var resultBox = VerticalBox.Get();
            var opSpacing = 0;
            var kern = 0f;

            // Create and add box for upper limit.
            if (superscript != null)
            {
                resultBox.Add(StrutBox.Get(0, opSpacing, 0));
                upperLimitBox.shift = delta / 2;
                upperLimitBox.shift += TopOffset(atom);
                resultBox.Add(upperLimitBox);
                kern = Mathf.Max(upShift, upMargin - upperLimitBox.depth);
                resultBox.Add(StrutBox.Get(0, kern, 0));
            }

            // Add box for base atom.
            resultBox.Add(baseBox);

            // Create and add box for lower limit.
            if (subscript != null)
            {
                resultBox.Add(StrutBox.Get(0, Mathf.Max(downShift, downMargin - lowerLimitBox.height), 0));
                lowerLimitBox.shift = -delta / 2;
                lowerLimitBox.shift += BottomOffset(atom);
                resultBox.Add(lowerLimitBox);
                resultBox.Add(StrutBox.Get(0, opSpacing, 0));
            }

            // Adjust height and depth of result box.
            var baseBoxHeight = isBigOp ? baseBox.TotalHeight / 2 + median : baseBox.height;
            var totalHeight = resultBox.height + resultBox.depth;
            if (upperLimitBox != null)
                baseBoxHeight += opSpacing + kern + upperLimitBox.height + upperLimitBox.depth;
            resultBox.height = baseBoxHeight;
            resultBox.depth = totalHeight - baseBoxHeight;
            return resultBox;
        }

        static float TopOffset(Atom symbol)
        {
            if (!(symbol is SymbolAtom))
                return 0;
            var name = ((SymbolAtom)symbol).metadata.symbol;
            switch (name)
            {
                case "int":
                case "oint":
                    return .6f;
                case "varint":
                case "varoint":
                case "iint":
                case "iiint":
                case "oiint":
                case "oiiint":
                    return .3f;
                default:
                    return 0;
            }
        }

        static float BottomOffset(Atom symbol)
        {
            if (!(symbol is SymbolAtom))
                return 0;
            var name = ((SymbolAtom)symbol).metadata.symbol;
            switch (name)
            {
                case "int":
                case "oint":
                    return -.15f;
                case "varint":
                case "varoint":
                case "iint":
                case "iiint":
                case "oiint":
                case "oiiint":
                    return -.1f;
                default:
                    return 0;
            }
        }

        static CharBox GetDisplayBox(SymbolAtom atom)
        {
            var ch = atom.metadata;
            if (ch.larger.Has)
            {
                return atom.CreateBoxSubtituted(ch.larger.Get);
            }
            else if (ch.symbol.EndsWith("text"))
            {
                var altSymbol = ch.symbol.Substring(0, ch.symbol.Length - 4) + "display";
                var ch2 = TEXPreference.main.GetChar(altSymbol, ch.fontIndex);
                if (ch2 != null)
                    return atom.CreateBoxSubtituted(ch2);
            }
            return atom.CreateBoxSubtituted(ch);
        }

        float upMargin, upShift, downMargin, downShift, median;

        public override void ProcessParameters(string command, TexParserState state, string value, ref int position)
        {
            var r = state.Ratio;
            upMargin = state.Math.upperMinimumDistance * r;
            upShift = state.Math.upperBaselineDistance * r;
            downMargin = state.Math.lowerMinimumDistance * r;
            downShift = state.Math.lowerBaselineDistance * r;
            median = state.Typeface.lineMedian * r;

            if (command == "lim") {
                atom = WordAtom.Get();
                atom.ProcessParameters("lim", state);
            }
            else if (command.StartsWith("under"))
            {
                sublimit = SymbolAtom.Get(TEXPreference.main.GetChar(command.Substring(5) + "right", state.Font.current), state);
                atom = state.parser.ParseToken(value, state, ref position);
            }
            else if (command.StartsWith("over"))
            {
                superlimit = SymbolAtom.Get(TEXPreference.main.GetChar(command.Substring(4) + "left", state.Font.current), state);
                atom = state.parser.ParseToken(value, state, ref position);
            }

            base.ProcessParameters(command, state, value, ref position);
        }
        public override void Flush()
        {
            ObjPool<BigOperatorAtom>.Release(this);
            if (sublimit != null)
            {
                sublimit.Flush();
                sublimit = null;
            }
            if (superlimit != null)
            {
                superlimit.Flush();
                superlimit = null;
            }
            base.Flush();
        }
    }
}
