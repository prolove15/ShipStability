namespace TexDrawLib
{
    public class SpaceAtom : AbstractAtom
    {
        public float width, height, depth;

        public static SpaceAtom Empty => ObjPool<SpaceAtom>.Get();

        public static SpaceAtom Get()
        {
            return Empty;
        }

        public static SpaceAtom Get(string command, TexParserState state)
        {
            var atom = ObjPool<SpaceAtom>.Get();
            atom.ProcessParameters(command, state);
            return atom;
        }


        public override Box CreateBox(TexBoxingState state)
        {
            return StrutBox.Get(width, height, depth);
        }

        public override void Flush()
        {

            width = height = depth = 0;
            ObjPool<SpaceAtom>.Release(this);
        }

        public override void ProcessParameters(string command, TexParserState state, string value, ref int position)
        {
            var ratio = state.Ratio;
            
            width = DetermineWidth(command, state) * ratio;
            height = state.Typeface.lineAscent * ratio;
            depth = state.Typeface.lineDescent * ratio;
        }

        float DetermineWidth(string command, TexParserState state)
        {
            switch (command)
            {
                case "qquad":
                    return (state.Typeface.lineAscent) * 2f;
                case "quad":
                    return (state.Typeface.lineAscent);
                case "enskip":
                    return state.Typeface.blankSpaceWidth * 2f;
                case ",":
                    return state.Typeface.blankSpaceWidth * 0.5f;
                case "w":
                    return state.Typeface.blankSpaceWidth;
                case " ":
                default:
                    return state.Environment.current.IsMathMode() ? 0 : state.Typeface.blankSpaceWidth;
            }
        }

    }
}
