using System;

namespace TexDrawLib
{
    public class MathAtom : RowAtom
    {
        float alignment;
        float glueRatio;

        public static new MathAtom Get()
        {
            return ObjPool<MathAtom>.Get();
        }

        public override Box CreateBox(TexBoxingState state)
        {
            var box = HorizontalBox.Get();
            if (children.Count == 1 && children[0] is SymbolAtom satom && (satom.metadata.larger.Has || satom.metadata.extension.enabled))
            {
                if (!state.restricted)
                {
                    box.Add(satom.CreateBoxMinWidth(state.width, state));
                    return box;
                } 
                else if (!state.interned)
                {
                    box.Add(satom.CreateBoxMinHeight(state.height, state));
                    return box;
                }
            }
            state.Push();
            state.restricted = true;
            var lastAtomType = CharTypeInternal.Inner;
            for (int i = 0; i < children.Count; i++)
            {
                if (children[i] is SpaceAtom sa && sa.width == 0) 
                    continue; 
                if (i > 0)
                {
                    if (i == 1 && children[0] is SymbolAtom sy && sy.metadata.symbol == "minus")
                    {
                        // a minus sign
                        goto skipGlue;
                    }
                    var curAtomType = children[i].LeftType;
                    var glue = TEXPreference.main.GetGlue(lastAtomType, curAtomType) * glueRatio;
                    if (glue > 0)
                    {
                        var glueBox = StrutBox.Get(glue, 0, 0);
                        box.Add(glueBox);
                    }
                    skipGlue:;
                }
                lastAtomType = children[i].RightType;
                box.Add(children[i].CreateBox(state));
            }
            state.Pop();
            if (state.restricted)
                return CheckBox(box);
            else
                return state.interned ? box : HorizontalBox.Get(CheckBox(box), state.width, alignment);
        }

        float minHeight, minDepth, lineSpace;

        Box CheckBox(Box b)
        {
            b.height = Math.Max(b.height, minHeight) + lineSpace / 2;
            b.depth = Math.Max(b.depth, minDepth) + lineSpace / 2;
            return b;
        }

        public override void ProcessParameters(string command, TexParserState state, string value, ref int position)
        {
            var r = state.Ratio;
            alignment = state.Paragraph.alignment;
            glueRatio = state.Math.glueRatio * r;
            minHeight = state.Typeface.lineAscent * r;
            minDepth = state.Typeface.lineDescent * r;
            lineSpace = state.Paragraph.lineSpacing * r;

            base.ProcessParameters(command, state, value, ref position);
        }

        public override void Flush()
        {
            ObjPool<MathAtom>.Release(this);
            base.Flush();
        }
    }
}
