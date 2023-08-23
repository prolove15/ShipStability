using System;
using UnityEngine;
using static TexDrawLib.TexParserUtility;

namespace TexDrawLib
{
    public class FractionAtom : DisplayAtom
    {
        public override CharType Type => CharTypeInternal.Inner;

        public Atom numerator;
        public Atom denominator;
        public bool line;

        float thickness, gapUp, gapDown, margin, lineMedian;
        Color32 color;

        public override Box CreateBox(TexBoxingState state)
        {

            // Create boxes for numerator and demoninator atoms, and make them of equal width.
            var numeratorBox = numerator?.CreateBox(state) ?? StrutBox.Empty;
            var denominatorBox = denominator?.CreateBox(state) ?? StrutBox.Empty;

            float maxWidth = Math.Max(numeratorBox.width, denominatorBox.width) + margin;
            numeratorBox = HorizontalBox.Get(numeratorBox, maxWidth, TexAlignment.Center);
            denominatorBox = HorizontalBox.Get(denominatorBox, maxWidth, TexAlignment.Center);

            // Calculate preliminary shift-up and shift-down amounts.
            float shiftUp, shiftDown;

            shiftUp = gapUp;
            shiftDown = gapDown;

            // Create result box.
            var resultBox = VerticalBox.Get();

            // add box for numerator.
            resultBox.Add(numeratorBox);

            // Calculate clearance and adjust shift amounts.
            //var axis = TEXConfiguration.main.AxisHeight * TexContext.Scale;

            // Calculate clearance amount.
            float clearance = thickness > 0 ? gapUp : gapUp + this.thickness;

            // Adjust shift amounts.
            var kern1 = shiftUp - numeratorBox.depth;
            var kern2 = shiftDown - denominatorBox.height;
            var delta1 = clearance - kern1;
            var delta2 = clearance - kern2;
            if (delta1 > 0)
            {
                shiftUp += delta1;
                kern1 += delta1;
            }
            if (delta2 > 0)
            {
                shiftDown += delta2;
                kern2 += delta2;
            }

            if (thickness > 0)
            {
                // Draw fraction line.

                resultBox.Add(StrutBox.Get(0, kern1, 0));
                resultBox.Add(RuleBox.Get(color, maxWidth, thickness, 0));
                resultBox.Add(StrutBox.Get(0, kern2, 0));
            }
            else
            {
                // Do not draw fraction line.

                var kern = kern1 + kern2;
                resultBox.Add(StrutBox.Get(0, kern, 0));
            }

            // add box for denominator.
            resultBox.Add(denominatorBox);

            // Adjust height and depth of result box.
            resultBox.height = shiftUp + numeratorBox.height;
            resultBox.depth = shiftDown + thickness + denominatorBox.depth;
            resultBox.shift -= lineMedian * 0.5f;
            return resultBox;
        }

        public override void ProcessParameters(string command, TexParserState state, string value, ref int position)
        {
            var r = state.Ratio;
            gapUp = state.Math.upperMinimumDistance * r;
            gapDown = state.Math.lowerMinimumDistance * r;
            thickness = margin = state.Math.lineThinkness * r;
            lineMedian = state.Typeface.lineMedian * r;
            color = state.Color.current;

            if (command.Contains("n")) {
                thickness = 0;
            }

            if (string.IsNullOrEmpty(value)) return;


            state.PushMathStyle((x) => x.GetCrampedStyle());
            SkipWhiteSpace(value, ref position);
            numerator = position < value.Length && !char.IsLetterOrDigit(value[position]) ? state.parser.ParseToken(value, state, ref position) : TryToUnpack(state.parser.Parse(LookForAWordOrDigit(value, ref position), state));
            SkipWhiteSpace(value, ref position);
            denominator = position < value.Length && !char.IsLetterOrDigit(value[position]) ? state.parser.ParseToken(value, state, ref position) : TryToUnpack(state.parser.Parse(LookForAWordOrDigit(value, ref position), state));
            state.PopMathStyle();
        }

        public override void Flush()
        {
            numerator?.Flush();
            numerator = null;
            denominator?.Flush();
            denominator = null;
            ObjPool<FractionAtom>.Release(this);
        }

        internal static FractionAtom Get()
        {
            return ObjPool<FractionAtom>.Get();
        }
    }
}
