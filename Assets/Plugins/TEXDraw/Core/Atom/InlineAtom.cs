using System;
using System.Text.RegularExpressions;
using static TexDrawLib.TexParserUtility;

namespace TexDrawLib
{
    public abstract class AbstractAtom : Atom
    {
        public override CharType Type => CharTypeInternal.Invalid;
        public override CharType LeftType => Type;
        public override CharType RightType => Type;
    }
    public abstract class DisplayAtom : Atom
    {
        public override CharType LeftType => Type;
        public override CharType RightType => Type;
    }

    public abstract class InlineAtom : Atom
    {
        public Atom atom;

        public override CharType Type => atom.Type;
        public override CharType LeftType => atom?.LeftType ?? CharTypeInternal.Invalid;
        public override CharType RightType => atom?.RightType ?? CharTypeInternal.Invalid;

        public override void Flush()
        {
            atom?.Flush();
            atom = null;
        }
    }

    public class ScriptedAtom : InlineMacroAtom
    {
        public static ScriptedAtom Get(string token)
        {
            var atom = ObjPool<ScriptedAtom>.Get();
            atom.token = token;
            return atom;
        }
         public static ScriptedAtom Get(ScriptedAtom source)
        {
            var atom = ObjPool<ScriptedAtom>.Get();
            atom.token = source.token;
            return atom;
        }
        public string token;

        public override Box CreateBox(TexBoxingState state)
        {
           return atom?.CreateBox(state);
        }
        public override void Flush()
        {
            ObjPool<ScriptedAtom>.Release(this);
            token = string.Empty;
            base.Flush();
        }
        public override void ProcessParameters(string command, TexParserState state, string value, ref int position)
        {
            var doc = state.parser.Parse(token, state);
            doc.mergeable = true;
            atom = doc;
        }
    }
    
    public class InlineMacroAtom : InlineAtom
    {
        public static Atom Get()
        {
            return ObjPool<InlineMacroAtom>.Get();
        }

        public override Box CreateBox(TexBoxingState state)
        {
            return atom?.CreateBox(state) ?? StrutBox.Empty;
        }

        public override void Flush()
        {
            ObjPool<InlineMacroAtom>.Release(this);
            base.Flush();
        }

        // Not multi-thread-safe
        static readonly string[] temp = new string[10];


        public override void ProcessParameters(string command, TexParserState state, string value, ref int position)
        {
            if (state.parser.macroCommands.TryGetValue(command, out ParametrizedMacro macro))
            {
                var count = Math.Min(temp.Length, macro.bracetedParameters + macro.bracketedParameters);
                for (int i = 1; i <= count; i++)
                {
                    SkipWhiteSpace(value, ref position);

                    if (i <= macro.bracketedParameters)
                    {
                        if (position < value.Length && value[position] == '[')
                            temp[i] = ReadGroup(value, ref position, '[', ']');
                        else
                            continue;
                    } else
                    {
                        temp[i] = LookForALetter(value, ref position);
                    }
                }
                var a = state.parser.Parse(string.Format(macro.formatableValue, temp), state);
                atom = TryToUnpack(a);
            }
            else if (state.parser.genericCommands.TryGetValue(command, out InlineState context))
            {
                SkipWhiteSpace(value, ref position);
                if (context.requiresGroupContext)
                    state.PushStates();
                context.stateProcessor?.Invoke(state);
                position = context.parameterProcessor?.Invoke(state, value, position) ?? position;

                if (context.requiresGroupContext)
                {
                    SkipWhiteSpace(value, ref position);
                    if (!(position < value.Length && value[position] == beginGroupChar))
                    {
                        state.PopStates();
                    } else
                    {
                        var doc = state.parser.Parse(ReadGroup(value, ref position), state);
                        doc.mergeable = true;
                        atom = doc;
                        state.PopStates();
                    }
                }
            }
            if (position < value.Length && char.IsWhiteSpace(value[position]))
                position++;
        }

        public readonly struct InlineState
        {
            public readonly string name;
            public readonly Action<TexParserState> stateProcessor;
            public readonly Func<TexParserState, string, int, int> parameterProcessor;
            public readonly bool requiresGroupContext;

            public InlineState(string name,
                               Action<TexParserState> stateProcessor = null,
                               Func<TexParserState, string, int, int> parameterProcessor = null,
                               bool requiresGroupContext = false)
            {
                this.name = name;
                this.stateProcessor = stateProcessor;
                this.parameterProcessor = parameterProcessor;
                this.requiresGroupContext = requiresGroupContext;
            }
        }

        public readonly struct ParametrizedMacro
        {
            public readonly string macroKey;
            public readonly string formatableValue;
            public readonly int bracketedParameters;
            public readonly int bracetedParameters;

            public readonly string rawKey, rawValue;
            static readonly Regex replacer = new Regex(@"#(\d)");

            public ParametrizedMacro(StringPair pair)
            {
                int brack = 0, brace = 0, pos = 0, stop = 0;
                string key = pair.key;

                while (pos < key.Length)
                {
                    var offset = key.IndexOf('#', pos);
                    if (offset < 0) { if (stop == 0) stop = key.Length - 1; break; }
                    if (key[offset] == '#')
                    {
                        if (stop == 0)
                            stop = offset - 1;
                        if (key[offset - 1] == '[')
                            brack++;
                        else
                            brace++;
                        pos = offset + 1;
                    }
                }
                if (key[stop] == '[')
                    stop--;
                if (key[0] == '\\')
                    macroKey = key.Substring(1, stop);
                else
                    macroKey = key.Substring(0, stop + 1);
                bracketedParameters = brack;
                bracetedParameters = brace;
                formatableValue = replacer.Replace(pair.value.Replace("{", "{{").Replace("}", "}}"), "{$1}");
                rawKey = pair.key;
                rawValue = pair.value;
            }
        }

    }
}
