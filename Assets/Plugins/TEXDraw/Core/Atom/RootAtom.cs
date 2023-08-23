using UnityEngine;
using static TexDrawLib.TexParserUtility;

namespace TexDrawLib
{
    public class RootAtom : DisplayAtom
    {
        public Atom atom;
        public Atom degree;
        public SymbolAtom surdsign;

        float clearance, thickness;
        public override CharType Type => CharTypeInternal.Inner;

        public static RootAtom Get()
        {
            return ObjPool<RootAtom>.Get();
        }

        public static VerticalBox GetOverBar(Color32 color, Box box, float kern, float thickness)
        {
            var atom = VerticalBox.Get();
            atom.Add(RuleBox.Get(color, box.width, thickness, 0));
            kern += thickness;
            atom.Add(StrutBox.Get(0, kern, 0));
            atom.Add(box);
            return atom;
        }

        public override Box CreateBox(TexBoxingState state)
        {
            var baseBox = atom?.CreateBox(state) ?? StrutBox.Empty;

            // Create box for radical sign.
            var totalHeight = baseBox.TotalHeight;
            var radicalSignBox = surdsign.CreateBoxMinHeight(totalHeight + clearance + thickness, state);

            // Add some clearance to left and right side
            baseBox = HorizontalBox.Get(baseBox, baseBox.width + clearance * 2, TexAlignment.Center);

            // Add half of excess height to clearance.
            //thickness = Math.Max(radicalSignBox.height, thickness);
            var realclearance = radicalSignBox.TotalHeight - totalHeight - thickness * 2;

            // Create box for square-root containing base box.
            TexUtility.CentreBox(radicalSignBox);
            var overBar = GetOverBar(surdsign.color, baseBox, realclearance, thickness);
            TexUtility.CentreBox(overBar);
            var radicalContainerBox = HorizontalBox.Get(radicalSignBox);
            radicalContainerBox.Add(overBar);

            radicalContainerBox.shift = -(radicalContainerBox.depth - baseBox.depth);
            // If atom is simple radical, just return square-root box.
            if (degree == null)
                return radicalContainerBox;

            // Atom is complex radical (nth-root).

            // Create box for root atom.

            var rootBox = degree.CreateBox(state);

            var bottomShift = (radicalContainerBox.height + radicalContainerBox.depth);
            rootBox.shift = radicalContainerBox.depth - rootBox.depth - bottomShift;

            // Create result box.
            var resultBox = HorizontalBox.Get();

            // Add box for negative kern.
            var negativeKern = StrutBox.Get(-((radicalSignBox.width) / 2f), 0, 0);

            var xPos = rootBox.width + negativeKern.width;
            if (xPos < 0)
                resultBox.Add(StrutBox.Get(-xPos, 0, 0));

            resultBox.Add(rootBox);
            resultBox.Add(negativeKern);
            resultBox.Add(radicalContainerBox);

            return resultBox;
        }

        public override void ProcessParameters(string command, TexParserState state, string value, ref int position)
        {
            var r = state.Ratio;
            clearance = state.Math.upperMinimumDistance * r;
            thickness = state.Math.lineThinkness * r;
            surdsign = SymbolAtom.Get(TEXPreference.main.GetChar("radical"), state);

            SkipWhiteSpace(value, ref position);

            if (position < value.Length && value[position] == '[')
            {
                state.PushMathStyle((x) => x.GetRootStyle());
                degree = state.parser.Parse(ReadGroup(value, ref position, '[', ']'), state);
                state.PopMathStyle();
            }
            state.PushMathStyle((x) => x.GetCrampedStyle());
            atom = state.parser.ParseToken(value, state, ref position);
            state.PopMathStyle();
        }

        public override void Flush()
        {
            ObjPool<RootAtom>.Release(this);
            atom?.Flush();
            atom = null;
            degree?.Flush();
            degree = null;
        }
    }
}
