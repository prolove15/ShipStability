using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static TexDrawLib.TexParserUtility;

namespace TexDrawLib
{

    public partial class TexModuleParser
    {

        public readonly Dictionary<string, TexAssetStyle> styleVariants = new Dictionary<string, TexAssetStyle>()
        {
            { "rm", TexAssetStyle.Normal },
            { "bf", TexAssetStyle.Boldface },
            { "sl", TexAssetStyle.Slanted },
            { "it", TexAssetStyle.Italic },
            { "tt", TexAssetStyle.Typewriter },
            { "sc", TexAssetStyle.SmallCase },
            { "sf", TexAssetStyle.SansSerif },
        };

        public readonly Dictionary<char, string> mathPunctuationSymbols = new Dictionary<char, string>()
        {
            { '+', "plus" },
            { '-', "minus" },
            { '*', "asteriskmath" },
            { '/', "slash" },
            { '=', "equal" },
            { '(', "parenleft" },
            { ')', "parenright" },
            { '[', "bracketleft" },
            { ']', "bracketright" },
            { '{', "braceleft" },
            { '}', "braceright" },
            { '<', "less" },
            { '>', "greater" },
            { '|', "bar" },
            { '.', "period" },
            { ',', "comma" },
            { ':', "colon" },
            { ';', "semicolon" },
            { '`', "quoteleft" },
            { '\'', "quoteright" },
            { '"', "quotedblright" },
            { '?', "question" },
            { '!', "exclam" },
            { '@', "at" },
            { '#', "numbersign" },
            { '$', "dollar" },
            { '%', "percent" },
            { '&', "ampersand" },
            { '1', "one" },
            { '2', "two" },
            { '3', "three" },
            { '4', "four" },
            { '5', "five" },
            { '6', "six" },
            { '7', "seven" },
            { '8', "eight" },
            { '9', "nine" },
            { '0', "zero" },
        };

        public readonly Dictionary<string, (string, string)> matrixDelimVariants = new Dictionary<string, (string, string)>()
        {
            { "pmatrix", ("parenleft", "parenright") },
            { "bmatrix", ("bracketleft", "bracketright") },
            { "Bmatrix", ("braceleft", "braceright") },
            { "vmatrix", ("bar", "bar") },
            { "Vmatrix", ("bardbl", "bardbl") },
        };

        public readonly string[] colorVariants = new string[] {
            "red", "cyan", "blue", "darkblue", "lightblue", "purple", "yellow",
            "lime", "fuchsia", "white", "silver", "grey", "black", "orange",
            "brown", "maroon", "green", "olive", "navy", "teal", "aqua", "magenta"
        };

        public readonly string[] bigOperatorVariants = new string[] {
            "underbrace", "underbracket", "underbrack", "underangle",
            "overbrace", "overbracket", "overbrack", "overangle",
        };

        public readonly string[] delimiterVariants = new string[] { "big", "Big", "bigg", "Bigg" };

        public readonly string[] whitespaceVariants = new string[] { " ", ",", "s", "w", "quad", "qquad", "enskip" };

        public readonly string[] regularFunctions = new string[] {
              "cos", "sec", "arccos", "cosh", "coth", "sin", "csc", "arcsin", "sinh",
              "tan", "cot", "arctan", "tanh", "arg", "dim", "hom", "lg", "max", "sup",
              "deg", "exp", "inf", "lim", "min", "det", "gcd", "ker", "sup", "mod",
        };

        public readonly Dictionary<string, float> sizeVariants = new Dictionary<string, float>()
        {
            { "tiny", .5f },
            { "ssmall", .6f }, // moresize
            { "scriptsize", .7f },
            { "footnotesize", .8f },
            { "small", .9f },
            { "normalsize", 1f },
            { "large", 1.2f },
            { "Large", 1.44f },
            { "LARGE", 1.728f },
            { "huge", 2.074f },
            { "Huge", 2.488f },
            { "HUGE", 2.986f }, // moresize
        };

        public void Initialize()
        {
            // Unlike before v5.x we index all commands now in dictionary rather
            // than hardcoding them. For simplicity and performance in mind.

            ignoredChars = new HashSet<char>() { '\0', '\r', '\x200c', '\t' };
            whiteSpaceChars = new HashSet<char>() { ' ', '\r', '\t', '\n' };
            macroCommands = new Dictionary<string, InlineMacroAtom.ParametrizedMacro>();
            aliasCommands = new Dictionary<string, string>();
            {
                foreach (var item in TEXConfiguration.main.SymbolAliases)
                {
                    aliasCommands[item.key] = item.value;
                }
            }
            generalCommands = new Dictionary<string, Func<Atom>>();
            {
                if (TEXPreference.main.symbols.Count == 0)
                {
                    // Idk why sometimes it's lazy loaded
                    TEXPreference.main.PushToDictionaries();
                }

                foreach (var item in TEXPreference.main.symbols)
                    if (TEXPreference.main.GetChar(item.Value).type == CharType.Accent)
                        generalCommands[item.Key] = AccentedAtom.Get;
                    else
                        generalCommands[item.Key] = SymbolAtom.Get;
                foreach (var item in regularFunctions)
                    generalCommands[item] = WordAtom.Get;
                foreach (var item in whitespaceVariants)
                    generalCommands[item] = SpaceAtom.Get;
                foreach (var key in NegateAtom.modes.Keys)
                    generalCommands[key] = NegateAtom.Get;
                foreach (var key in bigOperatorVariants)
                    generalCommands[key] = BigOperatorAtom.Get;
                generalCommands["link"] = BoxedLinkAtom.Get;
                generalCommands["href"] = BoxedLinkAtom.Get;
                generalCommands["left"] = AutoDelimitedGroupAtom.Get;
                generalCommands["sqrt"] = RootAtom.Get;
                generalCommands["frac"] = FractionAtom.Get;
                generalCommands["nfrac"] = FractionAtom.Get;

                generalCommands["\\"] = SoftBreakAtom.Get;
                generalCommands["-"] = HyphenBreak.Get;
                generalCommands["par"] = ParagraphBreakAtom.Get;
                generalCommands["verb"] = VerbAtom.Get;
                generalCommands["count"] = CounterAtom.Get;
                generalCommands["advance"] = CounterAtom.Get;
                generalCommands["cmultiply"] = CounterAtom.Get;
                generalCommands["cdivide"] = CounterAtom.Get;
                generalCommands["inlineinput"] = InputAtom.Get;
                generalCommands["hbox"] = BoxedAtom.Get;
                generalCommands["vbox"] = BoxedAtom.Get;
                generalCommands["hfil"] = FlexibleAtom.Get;
                generalCommands["vfil"] = FlexibleAtom.Get;
                generalCommands["hfill"] = FlexibleAtom.Get;
                generalCommands["vfill"] = FlexibleAtom.Get;
                generalCommands["hss"] = FlexibleAtom.Get;
                generalCommands["vss"] = FlexibleAtom.Get;

                generalCommands["hline"] = HorizontalRuleAtom.Get;
                generalCommands["toprule"] = HorizontalRuleAtom.Get;
                generalCommands["bottomrule"] = HorizontalRuleAtom.Get;
                generalCommands["midrule"] = HorizontalRuleAtom.Get;

                generalCommands["input"] = InputAtom.Get;
                generalCommands["begingroup"] = GroupAtom.Get;
                generalCommands["begin"] = GroupAtom.Get;
                generalCommands["vskip"] = VerticalSkipAtom.Get;
                foreach (var item in TEXConfiguration.main.BlockMacros)
                    if (!string.IsNullOrEmpty(item.key))
                    {
                        var parsed = new InlineMacroAtom.ParametrizedMacro(item);
                        macroCommands[parsed.macroKey] = parsed;
                        generalCommands[parsed.macroKey] = DocumentAtom.Get;
                    }
                foreach (var item in TEXConfiguration.main.InlineMacros)
                    if (!string.IsNullOrEmpty(item.key))
                    {
                        var parsed = new InlineMacroAtom.ParametrizedMacro(item);
                        macroCommands[parsed.macroKey] = parsed;
                        generalCommands[parsed.macroKey] = InlineMacroAtom.Get;
                    }
            }
            environmentGroups = new Dictionary<string, GroupAtom.GroupState>();
            {
                environmentGroups["document"] = new GroupAtom.GroupState("document");
                environmentGroups["math"] = new GroupAtom.GroupState("math", (state) =>
                    {
                        state.Environment.Set(x => x.SetFlag(TexEnvironment.MathMode, true));
                        state.Paragraph.alignment = 0.5f;
                        state.Paragraph.justify = false;
                        state.Font.current = state.Typeface.mathIndex;
                    }
                );
                environmentGroups["verbatim"] = new GroupAtom.GroupState("verbatim", beginState: (state) =>
                    {
                        state.Paragraph.justify = false;
                        state.Paragraph.rightToLeft = false;
                        var font = TEXPreference.main.fonts[state.Font.current = state.Typeface.typewriterIndex];
                        if (font is TexFont)
                        {
                            var f = font as TexFont;
                            var r = state.Size.current / state.Ratio;
                            state.Paragraph.lineSpacing = 0;
                            state.Paragraph.paragraphSpacing = 0;
                            state.Typeface.lineAscent = (f.asset.ascent) * r / f.asset.fontSize;
                            state.Typeface.lineDescent = (f.asset.lineHeight - f.asset.ascent) * r / f.asset.fontSize;
                            state.Typeface.blankSpaceWidth = f.SpaceWidth((int)(state.Size.current * state.Document.retinaRatio) + 1) * r;
                        }
                    }, interpreter: (state, value, pos) =>
                    {
                        var row = DocumentAtom.Get();
                        var line = ParagraphAtom.Get();
                        while (pos < value.Length)
                        {
                            var ch = value[pos++];
                            if (state.parser.ignoredChars.Contains(ch))
                                continue;
                            else if (ch == '\n')
                            {
                                if (line.children.Count == 0)
                                {
                                    line.Add(CharAtom.Get(' ', state));
                                }
                                line.ProcessParameters(null, state);
                                row.Add(line);
                                line = ParagraphAtom.Get();

                            }
                            else if (state.parser.whiteSpaceChars.Contains(ch))
                                line.Add(CharAtom.Get(' ', state));
                            else
                                line.Add(CharAtom.Get(ch, state));
                        }
                        line.ProcessParameters(null, state);
                        row.Add(line);
                        return row;
                    }
                );
                environmentGroups["enumerate"] = new GroupAtom.GroupState("enumerate", beginState: (state) =>
                {
                    if (state.Metadata.TryGetValue("count5", out Atom counter) && counter is CounterAtom catom)
                    {
                        catom.number = 0;
                    }
                    state.SetMetadata("item", ScriptedAtom.Get(@"\\\hbox by 30pt{\hss\the\advance\count5 by 1.\,\,\,}"));
                    state.Paragraph.leftPadding += 1;
                    state.Paragraph.indent = false;
                });
                environmentGroups["itemize"] = new GroupAtom.GroupState("itemize", beginState: (state) =>
                {
                    state.SetMetadata("item", ScriptedAtom.Get(@"\\\hbox by 20pt{\hss\bullet\,\,\,}"));
                    state.Paragraph.leftPadding += 1;
                    state.Paragraph.indent = false;
                });
                environmentGroups["flushleft"] = new GroupAtom.GroupState("flushleft", (state) =>
                   {
                       state.Paragraph.alignment = 0;
                       state.Paragraph.justify = false;
                   }
               );
                environmentGroups["flushright"] = new GroupAtom.GroupState("flushright", (state) =>
                    {
                        state.Paragraph.alignment = 1;
                        state.Paragraph.justify = false;
                    }
                );
                environmentGroups["center"] = new GroupAtom.GroupState("center", (state) =>
                    {
                        state.Paragraph.alignment = 0.5f;
                        state.Paragraph.justify = false;
                        state.Paragraph.indent = false;
                    }
                );
                environmentGroups["comment"] = new GroupAtom.GroupState("comment",
                    interpreter: (state, text, pos) =>
                    {
                        return SpaceAtom.Empty;
                    });
                environmentGroups["tabular"] = new GroupAtom.GroupState("tabular",
                    interpreter: (state, text, pos) =>
                    {
                        var table = TabularAtom.Get();
                        table.ProcessParameters("tabular", state, text, ref pos);
                        table.Interprete(text, ref pos, state);
                        return DocumentAtom.GetAsBlock(table, state);
                    });
                environmentGroups["align*"] = environmentGroups["aligned"] = new GroupAtom.GroupState("align*",
                    interpreter: (state, text, pos) =>
                    {
                        var table = TabularAtom.Get();
                        table.ProcessParameters("align*", state, text, ref pos);
                        table.Interprete(text, ref pos, state);
                        return DocumentAtom.GetAsBlock(table, state);
                    });
                environmentGroups["matrix"] = new GroupAtom.GroupState("matrix",
                    interpreter: (state, text, pos) =>
                    {
                        var table = TabularAtom.Get();
                        table.ProcessParameters("matrix", state, text, ref pos);
                        table.Interprete(text, ref pos, state);
                        return table;
                    });
                environmentGroups["minipage"] = new GroupAtom.GroupState("minipage",
                    interpreter: (state, text, pos) =>
                    {
                        var container = BoxedAtom.Get();
                        container.horizontal = true;
                        container.relative = false;
                        if (pos < text.Length && text[pos] == '{')
                        {
                            container.size = TexUtility.ParseUnit(ReadGroup(text, ref pos), state);
                        }
                        else
                        {
                            container.size = 0;
                        }
                        container.atom = state.parser.Parse(text, state, ref pos);
                        return container;
                    });
                foreach (var item in matrixDelimVariants)
                {
                    string key = item.Key, left = item.Value.Item1, right = item.Value.Item2;
                    environmentGroups[key] = new GroupAtom.GroupState(key,
                    interpreter: (state, text, pos) =>
                    {
                        var table = TabularAtom.Get();
                        table.ProcessParameters("matrix", state, text, ref pos);
                        table.Interprete(text, ref pos, state);
                        var delims = AutoDelimitedGroupAtom.Get();
                        delims.left = SymbolAtom.Get(TEXPreference.main.GetChar(left), state);
                        delims.right = SymbolAtom.Get(TEXPreference.main.GetChar(right), state);
                        delims.atom = table;
                        return delims;
                    });
                }
            }
            genericCommands = new Dictionary<string, InlineMacroAtom.InlineState>();
            {
                foreach (var item in TEXPreference.main.fonts)
                {
                    genericCommands[item.name] = new InlineMacroAtom.InlineState(item.name,
                        (state) => state.Font.current = item.assetIndex);
                }
                foreach (var item in styleVariants)
                {
                    void styleProcessor(TexParserState state)
                    {
                        int subStyleProcessor()
                        {
                            var font = TEXPreference.main.fonts[state.Font.current];
                            var flag = item.Value;
                            if (font.metadata.style.HasFlag(flag))
                                return state.Font.current;
                            var cumulativeFlag = flag;// == TexAssetStyle.Normal ? flag : flag | font.metadata.style;
                            font = font.metadata.baseAsset ?? font;
                            foreach (var variant in font.metadata.variantAssets)
                            {
                                if (variant.metadata.style == cumulativeFlag)
                                    return variant.assetIndex;
                            }
                            foreach (var variant in font.metadata.variantAssets)
                            {
                                if (variant.metadata.style.HasFlag(cumulativeFlag))
                                    return variant.assetIndex;
                            }
                            return state.Font.current;
                        }
                        state.Font.current = subStyleProcessor();
                    }

                    genericCommands[item.Key] = new InlineMacroAtom.InlineState(item.Key, styleProcessor);
                    genericCommands["text" + item.Key] = new InlineMacroAtom.InlineState("text" + item.Key, styleProcessor, requiresGroupContext: true);
                }
                foreach (var item in sizeVariants)
                {
                    genericCommands[item.Key] = new InlineMacroAtom.InlineState(item.Key, (state) => state.Size.current = item.Value * state.Document.initialSize);
                }
                genericCommands["fontsize"] = new InlineMacroAtom.InlineState("fontsize", parameterProcessor: (state, value, pos) =>
                {
                    state.Size.current = TexUtility.ParseUnit(value, ref pos, state);
                    return pos;
                });

                foreach (var item in colorVariants)
                {
                    ColorUtility.TryParseHtmlString(item, out Color color);
                    genericCommands[item] = new InlineMacroAtom.InlineState(item, (state) =>
                    {
                        state.Color.current = color;
                    });
                }
                for (int i = 0; i < delimiterVariants.Length; i++)
                {
                    var item = delimiterVariants[i];
                    var index = i;
                    genericCommands[item] = new InlineMacroAtom.InlineState(item, parameterProcessor: (state, value, pos) =>
                    {
                        var del = state.parser.ParseToken(value, state, ref pos);
                        if (del is CharAtom ch && mathPunctuationSymbols.TryGetValue(ch.character, out string putname))
                        {
                            del.Flush();
                            del = SymbolAtom.Get(TEXPreference.main.GetChar(putname), state);
                        }
                        if (del is SymbolAtom dels)
                        {
                            TexChar meta = dels.metadata;
                            {
                                del.Flush();
                                del = DelimitedSymbolAtom.Get(meta, state, index + 1);
                            }
                            state.LayoutContainer.current.Add(del);
                        }

                        return pos;
                    });
                }

                int colorProcessor(TexParserState state, string value, int pos)
                {
                    state.Color.current = ColorAtom.ParseColor(value, ref pos);
                    return pos;
                }

                int caseProcessor(TexParserState state, string value, int pos, Action<CharAtom> replacer)
                {
                    var atom = state.parser.ParseToken(value, state, ref pos);
                    if (atom is LayoutAtom)
                    {
                        var latom = atom as LayoutAtom;
                        for (int i = 0; i < latom.children.Count; i++)
                        {
                            var litem = latom.children[i];
                            if (litem is WordAtom watom)
                            {
                                for (int j = 0; j < watom.children.Count; j++)
                                {
                                    if (watom.children[j] is CharAtom wcatom)
                                        replacer(wcatom);
                                }
                            }
                            else if (litem is CharAtom wcatom)
                            {
                                replacer(wcatom);
                            }
                            state.LayoutContainer.current.Add(litem);
                        }
                    }
                    else
                        state.LayoutContainer.current.Add(atom);
                    return pos;
                }

                int numberProcessor(TexParserState state, string value, int pos, Func<int, string> replacer)
                {
                    var atom = state.parser.ParseToken(value, state, ref pos);
                    int number = 0;
                    if (atom is CounterAtom catom)
                    {
                        number = catom.number;
                    }
                    else if (atom is LayoutAtom watom)
                    {
                        foreach (var item in watom.children)
                            if (item is CharAtom wcatom)
                                number = number * 10 + (wcatom.character - '0');
                    }
                    else if (atom is CharAtom ccatom)
                    {
                        number = (ccatom.character - '0');
                    }

                    string str = replacer(number);
                    atom.Flush();
                    var wwatom = WordAtom.Get();
                    for (int i = 0; i < str.Length; i++)
                    {
                        wwatom.Add(CharAtom.Get(str[i], state));
                    }
                    state.LayoutContainer.current.Add(wwatom);
                    return pos;
                }

                genericCommands["color"] = new InlineMacroAtom.InlineState("color", parameterProcessor: colorProcessor);
                genericCommands["textcolor"] = new InlineMacroAtom.InlineState("color", parameterProcessor: colorProcessor, requiresGroupContext: true);
                genericCommands["displaystyle"] = new InlineMacroAtom.InlineState("displaystyle",
                    (state) => state.Environment.Set((x) => x.SetMathStyle(TexMathStyle.Display)));
                genericCommands["textstyle"] = new InlineMacroAtom.InlineState("textstyle",
                    (state) => state.Environment.Set((x) => x.SetMathStyle(TexMathStyle.Text)));
                genericCommands["raggedleft"] = new InlineMacroAtom.InlineState("raggedleft",
                    (state) => { state.Paragraph.alignment = 0f; state.Paragraph.justify = false; });
                genericCommands["raggedright"] = new InlineMacroAtom.InlineState("raggedright",
                    (state) => { state.Paragraph.alignment = 1f; state.Paragraph.justify = false; });
                genericCommands["centering"] = new InlineMacroAtom.InlineState("centering",
                    (state) => { state.Paragraph.alignment = 0.5f; state.Paragraph.justify = false; });
                genericCommands["noindent"] = new InlineMacroAtom.InlineState("noindent",
                    (state) => { state.Paragraph.indent = false; });
                genericCommands["indent"] = new InlineMacroAtom.InlineState("indent",
                    (state) => { state.Paragraph.indent = true; });
                genericCommands["uppercase"] = new InlineMacroAtom.InlineState("uppercase", parameterProcessor:
                    (state, value, pos) => caseProcessor(state, value, pos, (c) => c.character = char.ToUpperInvariant(c.character)));
                genericCommands["lowercase"] = new InlineMacroAtom.InlineState("lowercase", parameterProcessor:
                    (state, value, pos) => caseProcessor(state, value, pos, (c) => c.character = char.ToLowerInvariant(c.character)));
                genericCommands["the"] = new InlineMacroAtom.InlineState("the", parameterProcessor:
                    (state, value, pos) => numberProcessor(state, value, pos, (c) => c.ToString()));
                genericCommands["roman"] = new InlineMacroAtom.InlineState("roman", parameterProcessor:
                    (state, value, pos) => numberProcessor(state, value, pos, (c) => TexUtility.ToRoman(c)));
                genericCommands["columncolor"] = new InlineMacroAtom.InlineState("columncolor", parameterProcessor: (state, text, pos) =>
                {
                    var atom = ColorAtom.Get();
                    atom.ProcessParameters("columncolor", state, text, ref pos);
                    state.SetMetadata("columncolor", atom);
                    return pos;
                });
                genericCommands["def"] = new InlineMacroAtom.InlineState("def", parameterProcessor: (state, value, pos) =>
                {
                    var nextBrace = value.IndexOf(beginGroupChar, pos);
                    if (nextBrace >= 0)
                    {
                        pos = nextBrace;
                        ReadGroup(value, ref pos);
                        return pos;
                    }
                    else
                    {
                        SkipCurrentLine(value, ref pos);
                        return pos;
                    }
                });
                genericCommands["over"] = new InlineMacroAtom.InlineState("frac", parameterProcessor: (state, value, pos) =>
                {
                    var lay = state.LayoutContainer.current;
                    var overlay = MathAtom.Get();
                    overlay.ProcessParameters(string.Empty, state);
                    (overlay.children, lay.children) = (lay.children, overlay.children);
                    var underlay = Parse(value, state, ref pos);
                    var frac = FractionAtom.Get();
                    frac.ProcessParameters("frac", state);
                    frac.numerator = overlay;
                    frac.denominator = underlay;
                    lay.Add(frac);
                    return pos;
                });
                genericCommands["choose"] = new InlineMacroAtom.InlineState("pmatrix", parameterProcessor: (state, value, pos) =>
                {
                    var lay = state.LayoutContainer.current;
                    var overlay = MathAtom.Get();
                    overlay.ProcessParameters(string.Empty, state);
                    (overlay.children, lay.children) = (lay.children, overlay.children);
                    var underlay = Parse(value, state, ref pos);
                    var frac = FractionAtom.Get();
                    frac.ProcessParameters("nfrac", state);
                    frac.numerator = overlay;
                    frac.denominator = underlay;

                    var delims = AutoDelimitedGroupAtom.Get();
                    delims.left = SymbolAtom.Get(TEXPreference.main.GetChar("parenleft"), state);
                    delims.right = SymbolAtom.Get(TEXPreference.main.GetChar("parenright"), state);
                    delims.atom = frac;
                    lay.Add(delims);
                    return pos;
                });

            }
            {
                foreach (var item in genericCommands)
                    generalCommands[item.Key] = InlineMacroAtom.Get;
            }
            {
                allCommands = new HashSet<string>(
                    generalCommands.Keys.Concat(aliasCommands.Keys)
                );
            }
        }
    }
}