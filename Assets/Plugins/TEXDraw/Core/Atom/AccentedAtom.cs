using System;

namespace TexDrawLib
{
    public class AccentedAtom : InlineAtom
    {

        // Atom representing accent symbol to place over base atom.
        public SymbolAtom accent;

        float accentMargin;

        public static AccentedAtom Get()
        {
            return ObjPool<AccentedAtom>.Get();
        }

        public static AccentedAtom Get(string command, TexParserState state)
        {
            return ObjPool<AccentedAtom>.Get();
        }

        public override void Flush()
        {
            ObjPool<AccentedAtom>.Release(this);
            atom?.Flush();
            accent?.Flush();
            atom = null;
            accent = null;
        }


        public override Box CreateBox(TexBoxingState state)
        {
            //// Create box for base atom.
            var baseBox = atom == null ? StrutBox.Empty : atom.CreateBox(state);

            //// Find character of best scale for accent symbol.
            var acct = (CharBox)accent.CreateBoxMinWidth(baseBox.width, state);

            var resultBox = VerticalBox.Get();

            //// Create and add box for accent symbol.
            var accentWidth = (acct.bearing + acct.italic) * .5f;
            acct.italic = accentWidth + (acct.width * .5f);
            acct.bearing = accentWidth - (acct.width * .5f);
            resultBox.Add(acct);

            resultBox.Add(StrutBox.Get(0, accentMargin, 0));

            //// Centre and add box for base atom. Centre base box and accent box with respect to each other.
            var boxWidthsDiff = (baseBox.width - acct.width) / 2f;
            acct.shift = Math.Max(boxWidthsDiff, 0);
            if (boxWidthsDiff < 0)
                baseBox = HorizontalBox.Get(baseBox, acct.width, TexAlignment.Center);

            resultBox.Add(baseBox);

            // Adjust height and depth of result box.
            var depth = baseBox.depth;
            var totalHeight = resultBox.height + resultBox.depth;
            resultBox.depth = depth;
            resultBox.height = totalHeight - depth;

            return resultBox;
        }

        public override void ProcessParameters(string command, TexParserState state, string value, ref int position)
        {
            accentMargin = state.Math.upperMinimumDistance * state.Ratio;
            TexChar symbol;
            if (command.Length == 1)
            {
                symbol = null;
            }
            else
            {
                symbol = TEXPreference.main.GetChar(command);
                accent = SymbolAtom.Get(symbol, state);
            }
            if (symbol != null)
            {
                atom = state.parser.ParseToken(value, state, ref position);
            }
        }
    }
}
