using UnityEngine;

namespace TexDrawLib
{
    public class RuleBox : StrutBox
    {

        public static RuleBox Get(Color32 color, float width, float height, float depth)
        {
            var box = ObjPool<RuleBox>.Get();
            box.color = color;
            box.Set(width, height, depth);
            return box;
        }

        public Color32 color;

        public override void Flush()
        {
            ObjPool<RuleBox>.Release(this);
            base.Flush();
        }

        public override void Draw(TexRendererState state)
        {
            state.Draw(new TexRendererState.QuadState(TexUtility.frontBlockIndex, new Rect(state.x, state.y - depth, width, height + depth), new Rect(), color));
        }
    }
}
