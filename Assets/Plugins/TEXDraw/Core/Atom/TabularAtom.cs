using System;
using System.Linq;
using UnityEngine;
using static TexDrawLib.TexParserUtility;
using System.Collections.Generic;

namespace TexDrawLib
{
    public class TabularAtom : LayoutAtom
    {
        public static TabularAtom Get()
        {
            return ObjPool<TabularAtom>.Get();
        }

        public override CharType Type => CharTypeInternal.Inner;
        public override CharType LeftType => Type;
        public override CharType RightType => Type;

        public const int maximumColumnSet = 32;

        // width 0 == auto
        public (float alignment, float border, float width, string macro)[] columnMetrics = new (float, float, float, string)[maximumColumnSet];

        public int columnCount;

        public override void Flush()
        {
            ObjPool<TabularAtom>.Release(this);
            columnCount = Math.Min(30, columnCount);
            while (columnCount-- >= 0)
            {
                columnMetrics[columnCount + 1] = default;
            }
            columnCount = 0;
            base.Flush();
        }

        protected float minHeight, minDepth, tableSpace, paraSpace, leftMargin, rightMargin, alignment, median;
        protected bool justified;

        public override Box CreateBox(TexBoxingState state)
        {
            if (columnCount == 0) return StrutBox.Empty;

            state.Push();
            state.restricted = true;
            state.interned = true;

            List<float> columnSizes = ListPool<float>.Get(Enumerable.Repeat(0f, columnCount));
            List<float> rowHSizes = ListPool<float>.Get(Enumerable.Repeat(0f, children.Count / columnCount));
            List<float> rowDSizes = ListPool<float>.Get(Enumerable.Repeat(0f, children.Count / columnCount));

            for (int i = 0; i < children.Count; i++)
            {
                Vector3 size = (children[i] as TabularCellAtom).PrecomputeBox(state);
                int x = i % columnCount, y = i / columnCount;
                columnSizes[x] = Math.Max(columnSizes[x], size.x);
                rowHSizes[y] = Math.Max(rowHSizes[y], size.y);
                rowDSizes[y] = Math.Max(rowDSizes[y], size.z);
            }

            int totalColumnAuto = 0; float totalColumnSize = 0f;
            for (int i = 0; i < columnCount; i++)
            {
                if (columnMetrics[i].width <= float.Epsilon)
                    totalColumnAuto++;
                else
                    columnSizes[i] = columnMetrics[i].width;
                totalColumnSize += columnSizes[i];
            }
            // If Justify, expand
            if (justified && totalColumnAuto > 0 && state.width > totalColumnSize)
            {
                float excessUnit = (state.width - totalColumnSize) / totalColumnAuto;
                for (int i = 0; i < columnCount; i++)
                {
                    if (columnMetrics[i].width <= float.Epsilon)
                        columnSizes[i] += excessUnit;
                }
            }

            for (int i = 0; i < children.Count; i++)
            {
                int x = i % columnCount, y = i / columnCount;
                Vector3 m = new Vector3(columnSizes[x], rowHSizes[y], rowDSizes[y]);
                (children[i] as TabularCellAtom).AssignFinalMetric(m);
            }

            // Draw
            VerticalBox vbox = VerticalBox.Get();
            HorizontalBox rbox = HorizontalBox.Get();
            for (int i = 0; i < children.Count; i++)
            {
                if (i % columnCount == 0 && i > 0)
                {
                    vbox.Add(rbox);
                    rbox = HorizontalBox.Get();
                }
                rbox.Add(children[i].CreateBox(state));

            }
            vbox.Add(rbox);

            if ((!justified || totalColumnAuto == 0) && state.width > totalColumnSize)
            {
                TexUtility.CentreBox(vbox, median);
            }
            ListPool<float>.FlushAndRelease(columnSizes);
            ListPool<float>.FlushAndRelease(rowHSizes);
            ListPool<float>.FlushAndRelease(rowDSizes);

            state.Pop();

            return vbox;
        }

        public override void ProcessParameters(string command, TexParserState state, string value, ref int position)
        {
            float r = state.Ratio;
            alignment = state.Paragraph.alignment;
            justified = state.Paragraph.justify;
            minHeight = state.Typeface.lineAscent * r;
            minDepth = state.Typeface.lineDescent * r;
            median = state.Typeface.lineMedian * r;
            leftMargin = state.Paragraph.leftPadding * r;
            rightMargin = state.Paragraph.rightPadding * r;
            tableSpace = state.Paragraph.lineSpacing * r;
            paraSpace = state.Paragraph.paragraphSpacing * r;

            if (command == "tabular")
            {
                SkipWhiteSpace(value, ref position);
                string columnToken = ReadGroup(value, ref position);
                int seek = 0;
                while (seek < columnToken.Length)
                {
                    SkipWhiteSpace(columnToken, ref seek);
                    if (seek < columnToken.Length)
                    {
                        switch (columnToken[seek++])
                        {
                            case 'l':
                            case 'c':
                            case 'r':
                                if (++columnCount > maximumColumnSet) break;
                                columnMetrics[columnCount - 1].alignment = AlignmentCode(
                                    columnToken[seek - 1]);
                                break;
                            case '{':
                                seek--;
                                string unit = ReadGroup(columnToken, ref seek); // seek width
                                columnMetrics[columnCount - 1].width = TexUtility.ParseUnit(unit, state.Document.pixelsPerInch, state.Document.initialSize);
                                break;
                            case '|':
                                columnMetrics[columnCount].border = state.Math.lineThinkness * state.Ratio;
                                break;
                            case '>':
                                columnMetrics[columnCount].macro = ReadGroup(columnToken, ref seek);
                                break;
                        }
                    }
                }
            }
            else if (command == "align*")
            {
                tableSpace = 0;
                columnCount = maximumColumnSet - 1;
                for (int i = 0; i < columnCount; i++)
                {
                    columnMetrics[i] = (i % 2 == 0 ? 1 : 0, 0, 0, string.Empty);
                }
            }
            else if (command == "matrix")
            {
                columnCount = maximumColumnSet - 1;
                for (int i = 0; i < columnCount; i++)
                {
                    columnMetrics[i] = (0.5f, 0, 0, string.Empty);
                }
            }

        }

        public void Interprete(string text, ref int position, TexParserState state)
        {
            int column = 0;
            int realColumnCount = 0;
            int lastrowindex = 0;
            if (columnCount == 0)
                return;

            HorizontalRuleAtom therule = null;
            while (position < text.Length)
            {
                SkipWhiteSpace(text, ref position);
                state.PushStates();
                LayoutAtom container = state.Environment.current.IsMathMode() ? (LayoutAtom)MathAtom.Get() : ParagraphAtom.Get();
                container.ProcessParameters(string.Empty, state);
                state.LayoutContainer.current = container;
                realColumnCount = Math.Max(realColumnCount, column + 1);
                if (column >= columnCount)
                {
                    // actual column count turns out larger
                    //state.Paragraph.alignment = 0.5f;
                    //realColumnCount = Mathf.Max(column + 1, realColumnCount);
                }
                else
                {
                    // Apply macros and other stuff
                    if (!string.IsNullOrEmpty(columnMetrics[column].macro))
                        container.Add(state.parser.Parse(columnMetrics[column].macro, state).children[0]);
                    TabularCellAtom.currentAlignment = columnMetrics[column].alignment;
                    TabularCellAtom.currentTableSpace = tableSpace;
                }
                while (position < text.Length)
                {
                    Atom atom = state.parser.ParseToken(text, state, ref position) ?? SpaceAtom.Empty;
                    if (atom is AlignmentAtom)
                    {
                        atom.Flush();
                        break;
                    }
                    else if (atom is SoftBreakAtom)
                    {
                        while (++column < columnCount)
                        {
                            // force this to get complete cells in a row
                            TabularCellAtom tcelll = TabularCellAtom.Get(container);
                            tcelll.ProcessParameters(string.Empty, state, text, ref position);
                            tcelll.left.border = columnMetrics[column].border;
                            tcelll.left.color = state.Color.current;
                            tcelll.right.border = columnMetrics[(column + 1) % maximumColumnSet].border;
                            tcelll.right.color = state.Color.current;
                            children.Add(tcelll);
                            container = RowAtom.Get();
                        }
                        lastrowindex = children.Count;
                        column--;
                        atom.Flush();
                        break;
                    }
                    else if (atom is HorizontalRuleAtom ratom)
                    {
                        therule = ratom;
                    }
                    else if (atom is DocumentAtom docatom)
                    {
                        Atom aaa = docatom.children[0];
                        docatom.children.Clear();
                        docatom.Flush();
                        container.Add(aaa);
                    }
                    else if (atom is SpaceAtom && !container.HasContent())
                    {
                        atom.Flush();
                        continue;
                    }
                    else
                    {
                        container.Add(atom);
                    }
                }
                TabularCellAtom tcell = TabularCellAtom.Get();
                if (container is ParagraphAtom para)
                {
                    para.CleanupWord();
                    //para.indent = 0;
                    //para.alignment = 0;
                }
                if (container.children.Count > 0)
                    tcell.atom = container;
                tcell.ProcessParameters(string.Empty, state, text, ref position);
                tcell.x.size = columnMetrics[column].width;
                tcell.left.border = columnMetrics[column].border;
                tcell.left.color = state.Color.current;
                if (column == columnCount - 1 && column < maximumColumnSet - 1)
                {
                    tcell.right.border = columnMetrics[1 + column].border;
                    tcell.right.color = state.Color.current;
                }
                if (therule != null)
                {
                    for (int i = lastrowindex; i < children.Count; i++)
                    {
                        (children[i] as TabularCellAtom).bottom.padding += therule.top;
                    }
                    tcell.top.border = therule.thickness;
                    tcell.top.color = therule.color;
                    tcell.top.padding += therule.bottom;
                }
                column = (column + 1) % columnCount;
                if (column == 0 && therule != null)
                {
                    therule.Flush();
                    therule = null;
                }
                children.Add(tcell);
                state.PopStates();
            }
            if (realColumnCount < columnCount)
            {
                // remove exceesss
                for (int i = children.Count; i-- > 0;)
                {
                    if (i % columnCount >= realColumnCount)
                    {
                        children[i].Flush();
                        children.RemoveAt(i);
                    }
                }
                while (columnCount > realColumnCount)
                {
                    columnMetrics[columnCount] = default;
                    columnCount--;
                }
            } 
            else if (realColumnCount > columnCount)
            {

            }
            while (children.Count % columnCount != 0)
            {
                TabularCellAtom tcelll = TabularCellAtom.Get();
                tcelll.ProcessParameters(string.Empty, state, text, ref position);
                children.Add(tcelll);
            }
            if (therule != null && (children[lastrowindex + 1] as TabularCellAtom).atom == null)
            {
                while (children.Count > lastrowindex + 1)
                {
                    children.RemoveAt(children.Count - 1);
                }
                for (int i = lastrowindex - columnCount + 1; i < children.Count; i++)
                {
                    if (children[i] is TabularCellAtom tatom) {
                        tatom.bottom.color = therule.color;
                        tatom.bottom.border += therule.thickness;
                        tatom.bottom.padding += therule.top;
                    }
                }
            }
            Debug.Assert(children.Count % columnCount == 0);
            if (therule != null)
                therule.Flush();
        }

        public void MergeAtom(LayoutAtom atom)
        {
            atom = TryToUnpack(atom) as LayoutAtom;
            children.AddRange(atom.children);
            atom.children.Clear();
            atom.Flush();
        }

        static float AlignmentCode(char c)
        {
            switch (c)
            {
                case 'R':
                case 'r':
                    return 1f;
                case 'C':
                case 'c':
                    return 0.5f;
                case 'L':
                case 'l':
                default:
                    return 0;
            }
        }
    }
}
