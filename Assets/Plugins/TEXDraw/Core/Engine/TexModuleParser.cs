using System;
using System.Collections.Generic;
using static TexDrawLib.TexParserUtility;

namespace TexDrawLib
{
    public partial class TexModuleParser
    {
        public HashSet<char> ignoredChars;
        public HashSet<char> whiteSpaceChars;
        public HashSet<string> allCommands;
        public Dictionary<string, string> aliasCommands;
        public Dictionary<string, Func<Atom>> generalCommands;
        public Dictionary<string, GroupAtom.GroupState> environmentGroups;
        public Dictionary<string, InlineMacroAtom.InlineState> genericCommands;
        public Dictionary<string, InlineMacroAtom.ParametrizedMacro> macroCommands;

        public TexModuleParser()
        {
            // See TexModuleInitiator.cs
            Initialize();
        }


        /// <summary>
        /// Global parsing until the end of string
        /// </summary>
        public DocumentAtom Parse(string text, TexParserState state)
        {
            var p = 0;
            state.PushStates();
            var val = Parse(text, state, ref p);
            state.PopStates();
            return val;
        }

        /// <summary>
        /// Main parsing loop until the end of string
        /// </summary>
        public DocumentAtom Parse(string text, TexParserState state, ref int position)
        {
            SkipWhiteSpace(text, ref position);
            var doc = DocumentAtom.Get();
            var layoutAtom = CreateAtom(state);
            state.LayoutContainer.Push(layoutAtom);
            state.Environment.Push(TexEnvironment.Inline, true);

            // Simple macro get called oftenly
            Action newParagraph = () =>
            {
                if (layoutAtom.HasContent())
                {
                    state.Environment.Push(TexEnvironment.Inline, false);
                    layoutAtom.ProcessParameters("", state);
                    doc.Add(layoutAtom);
                    state.LayoutContainer.Set(layoutAtom = CreateAtom(state));
                    state.Environment.Pop();
                }
            };

            while (position < text.Length)
            {
                Atom atom = ParseToken(text, state, ref position);
                if (atom is null)
                {
                    continue;
                }
                else if (atom is DocumentAtom subDoc)
                {
                    // merge

                    bool firstLine = true;
                    foreach (LayoutAtom subline in subDoc.children)
                    {
                        if (firstLine)
                        {
                            if (subDoc.mergeable)
                            {
                                (layoutAtom as ParagraphAtom)?.CleanupWord();
                                if (subline is MathAtom ma)
                                {
                                    var x = MathAtom.Get();
                                    x.ProcessParameters("", state);
                                    x.children.AddRange(ma.children);
                                    layoutAtom.Add(x);
                                    doc.Add(layoutAtom);
                                    break;
                                }
                                else 
                                {
                                    subline.children.InsertRange(0, layoutAtom.children);
                                    layoutAtom.children.Clear();
                                    layoutAtom.Flush();
                                }
                            }
                            else if (layoutAtom.HasContent())
                            {
                                state.Environment.Push(TexEnvironment.Inline, false);
                                layoutAtom.ProcessParameters("", state);
                                state.Environment.Pop();
                                doc.Add(layoutAtom);
                            }
                            else
                            {
                                layoutAtom.Flush();
                            }
                            firstLine = false;
                        }
                        doc.Add(subline);
                        state.LayoutContainer.Set(layoutAtom = subline);
                    }

                    if (subDoc.mergeable)
                        doc.children.RemoveAt(doc.children.Count - 1);
                    else
                    {
                        state.Environment.Push(TexEnvironment.Inline, false);
                        state.LayoutContainer.Set(layoutAtom = CreateAtom(state));
                        state.Environment.Pop();
                    }
                    subDoc.children.Clear();
                    subDoc.Flush();
                }
                else if (atom is BlockAtom subBlock)
                {
                    // put inline guards (blocks around)
                    newParagraph();
                    layoutAtom.Add(subBlock);
                    newParagraph();
                }
                else if (atom is ParagraphBreakAtom)
                {
                    // commit new paragraph
                    atom.Flush();
                    newParagraph();
                }
                else
                {
                    layoutAtom.Add(atom);
                }
            }
            state.Environment.Pop();

            if (layoutAtom.HasContent() || !doc.HasContent())
            {
                layoutAtom.ProcessParameters("", state);
                doc.Add(layoutAtom);
            }
            else
            {
                layoutAtom.Flush();
            }
            state.LayoutContainer.Pop();
            return doc;
        }

        /// <summary>
        /// Temporal parsing that only seeks for next token.
        /// This ParseToken should assumes all condition are in inline mode.
        /// </summary>
        public Atom ParseToken(string text, TexParserState state, ref int position)
        {
            while (position < text.Length)
            {
                var ch = text[position];
                if (ch == commentChar) // Skip comments
                    SkipCurrentLine(text, ref position);
                else if (ignoredChars.Contains(ch))
                    position++; // Skip non-visible chars
                else
                    break;
            }
            if (position < text.Length)
            {
                var ch = text[position];
                var env = state.Environment.current;

                if (whiteSpaceChars.Contains(ch))
                {
                    if (ProcessSkipWhitespaces(text, ref position) >= 2)
                        return ParagraphBreakAtom.Get();
                    else
                        return SpaceAtom.Get(" ", state);
                }
                else if (ch == escapeChar)
                {
                    position++;
                    string cmd = LookForAWord(text, ref position);
                    if (cmd.Length == 0 && position < text.Length)
                    {
                        char oneCharSymbol = text[position++];
                        if (reservedChars.Contains(oneCharSymbol) && oneCharSymbol != escapeChar)
                            return ProcessOneCharCommand(state, oneCharSymbol);
                        else if (oneCharSymbol == '[')
                        {
                            var block = GroupAtom.Get();
                            block.ProcessParameters("[", state, text, ref position);
                            return TryToUnpack(block);
                        }
                        else if (oneCharSymbol == '(')
                        {
                            state.Environment.Push(TexEnvironment.MathMode, true);
                            state.PushMathStyle(x => TexMathStyle.Text);
                            state.Font.Push(state.Typeface.mathIndex);
                            position -= 2;
                            var m = Parse(ReadStringGroup(text, ref position, "\\(", "\\)"), state);
                            m.mergeable = true;

                            state.Font.Pop();
                            state.PopMathStyle();
                            state.Environment.Pop();

                            return m;
                        }
                        else
                            cmd = Char2Str(oneCharSymbol);
                    }
                    if (aliasCommands.TryGetValue(cmd, out string alias))
                    {
                        cmd = alias;
                    }
                    if (state.GetMetadata(cmd, out Atom atom) && atom is ScriptedAtom satom)
                    {
                        atom = ScriptedAtom.Get(satom);
                        atom.ProcessParameters(cmd, state, text, ref position);
                        return TryToUnpack(atom);
                    }
                    else if (generalCommands.TryGetValue(cmd, out Func<Atom> call))
                    {
                        return ProcessInlineCommand(text, state, ref position, cmd, call);
                    }
                    else
                    {
                        return null;
                    }
                }
                else if (ch == beginGroupChar)
                {
                    var atom = Parse(ReadGroup(text, ref position), state);
                    atom.mergeable = true;
                    return atom;
                }
                else if (ch == endGroupChar)
                {
                    position++;
                    return ParseToken(text, state, ref position);
                }
                else if (ch == alignmentChar)
                {
                    position++;
                    return AlignmentAtom.Get();
                }
                else if ((ch == subScriptChar || ch == superScriptChar))
                {
                    var bs = ScriptsAtom.RetrieveBaseScript(state.LayoutContainer.current) ?? SpaceAtom.Get();
                    var bisIsBig = env.GetMathStyle() < TexMathStyle.Text && ((bs as SymbolAtom)?.Type == CharType.BigOperator || (bs as WordAtom)?.word == "lim");
                    if (position + 1 < text.Length && (text[position + 1] == superScriptChar || text[position + 1] == subScriptChar)) {
                        position++;
                        bisIsBig = true;
                    }
                    var sp = bisIsBig ? BigOperatorAtom.Get(bs) : ScriptsAtom.Get(bs);
                    sp.ProcessParameters("", state, text, ref position);
                    return sp;
                }
                else if (ch == mathModeChar)
                {
                    if (position + 1 < text.Length && text[position + 1] == mathModeChar)
                    {
                        position += 2;
                        var block = GroupAtom.Get();
                        block.ProcessParameters("$$", state, text, ref position);
                        return TryToUnpack(block);
                    }
                    else
                    {
                        state.Environment.Push(TexEnvironment.MathMode, true);
                        state.PushMathStyle(x => TexMathStyle.Text);
                        state.Font.Push(state.Typeface.mathIndex);

                        var m = Parse(ReadGroup(text, ref position, '$', '$'), state);
                        m.mergeable = true;

                        state.Font.Pop();
                        state.PopMathStyle();
                        state.Environment.Pop();

                        return m;
                    }
                }
                else if (env.IsMathMode() && mathPunctuationSymbols.TryGetValue(text[position], out string symbol))
                {
                    position++;
                    var chc = TEXPreference.main.GetChar(symbol, state.Font.current);
                    return chc == null ? null : (Atom)SymbolAtom.Get(chc, state);
                }
                else
                    return (CharAtom.Get(text[position++], state.Font.current, state));
            }
            else
                return null;
        }

        private Atom ProcessInlineCommand(string text, TexParserState state, ref int position, string cmd, Func<Atom> call)
        {
            Atom tmp = call();
            if (tmp != null)
            {
                tmp.ProcessParameters(cmd, state, text, ref position);
                return TryToUnpack(tmp);
            }
            return null;
        }

        private int ProcessSkipWhitespaces(string text, ref int position)
        {
            var ch = text[position];
            var newSpaces = 0;
            do
            {
                if (ch == newLineChar)
                    newSpaces++;
            } while (++position < text.Length && whiteSpaceChars.Contains(ch = text[position]));
            return newSpaces;
        }

        private CharAtom ProcessOneCharCommand(TexParserState state, char oneCharSymbol)
        {
            TexChar oneCharTex;
            if (mathPunctuationSymbols.TryGetValue(oneCharSymbol, out string symbol) && (oneCharTex = TEXPreference.main.GetChar(symbol, state.Font.current)) != null)
                return (SymbolAtom.Get(oneCharTex, state));
            else
                return (CharAtom.Get(oneCharSymbol, state.Font.current, state));
        }


        private static LayoutAtom CreateAtom(TexParserState state)
        {
            if (state.Environment.current.IsMathMode())
                return MathAtom.Get();
            else
                return ParagraphAtom.Get();
        }
    }
}
