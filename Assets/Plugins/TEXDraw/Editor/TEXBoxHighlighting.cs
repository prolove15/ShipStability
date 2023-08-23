using TexDrawLib;

using UnityEditor;
using UnityEngine;

public class TEXBoxHighlighting
{
    private static GUIStyle style = new GUIStyle(EditorStyles.textArea);
    private static GUIStyle styleBg = new GUIStyle(GUI.skin.label);
    private static GUIStyle textArea = new GUIStyle(GUI.skin.textArea);
    private static GUIContent textContent = new GUIContent();

    static TEXBoxHighlighting()
    {
        styleBg.font = style.font = textArea.font = Font.CreateDynamicFontFromOSFont(
#if UNITY_EDITOR_WIN
            "Consolas"
#else
            "Courier New"
#endif
            , textArea.fontSize);
        textArea.padding = new RectOffset(4, 4, 2, 2);
        styleBg.normal.background = EditorGUIUtility.whiteTexture;
        styleBg.padding = new RectOffset();
    }

    public static void DrawText(SerializedProperty prop)
    {
        EditorGUILayout.PropertyField(prop);
        // var text = prop.hasMultipleDifferentValues ? "..." : prop.stringValue;
        // EditorGUI.BeginChangeCheck();
        // text = EditorGUILayout.TextArea(text, textArea);
        // if (EditorGUI.EndChangeCheck())
        // {
        //     // if (prop.serializedObject.isEditingMultipleObjects)
        //     //     Undo.RecordObjects(prop.serializedObject.targetObjects, "Edit TEXDraw(s)");
        //     // else
        //     //     Undo.RecordObject(prop.serializedObject.targetObject, "Edit TEXDraw");
        //     prop.stringValue = text;
        // }
    }

    public static string DrawText(string text)
    {
        float realHeight;
        float guiHeight = CalculateHeight(textContent, EditorStyles.textArea, 15, out realHeight);
        return DrawText(text, realHeight, GUILayout.Height(guiHeight));
    }

    public static string DrawText(string text, int maxLineNumber, params GUILayoutOption[] options)
    {
        float realHeight;
        float guiHeight = CalculateHeight(textContent, EditorStyles.textArea, maxLineNumber, out realHeight);
        ArrayUtility.Add(ref options, GUILayout.Height(guiHeight));
        return DrawText(text, realHeight, options);
    }

    private static Rect lastWorkingRect;

    public static string DrawText(string text, float realHeight, params GUILayoutOption[] options)
    {
        textContent.text = text;
        Rect bounds = EditorGUILayout.GetControlRect(options);
        float guiHeight = bounds.height;
        /// BUG: GetControlRect (when EventType is Layout) is returning value incorrectly....
        /// This issue have BIG impact on scrolling system
        if (bounds.size == Vector2.one)
            bounds = lastWorkingRect;
        else
            lastWorkingRect = bounds;
        var ev = Event.current;
        if (realHeight > guiHeight)
        {
            bounds.width -= GUI.skin.verticalScrollbar.fixedWidth;
        }

        text = GUI.TextArea(bounds, text, textArea);
        var id = GUIUtility.keyboardControl;
        TextEditor editor = (TextEditor)GUIUtility.GetStateObject(typeof(TextEditor), id);
        if (editor.text == null || editor.text != text)
        {
            return text; //Maybe we catch the wrong one?
        }

        //GUI can't handle cut/copy operations. so DIY.
        var et = ev.GetTypeForControl(id);

        if (et != EventType.Layout)
        {
            HandleScrolling(bounds, editor, ev, textArea);
        }

        // if (et == EventType.ExecuteCommand)
        // {
        //     var scroll = editor.scrollOffset;
        //     if (HandleExecuteCommands(editor, ev))
        //     {
        //         editor.scrollOffset = scroll;
        //         text = editor.text;
        //         GUI.changed = true;
        //     }
        // }
        // else if (et == EventType.ValidateCommand)
        //     HandleValidateCommands(editor, ev);
        else if (et == EventType.KeyDown)
        {
            if (ev.character == '\t')
            {
                editor.ReplaceSelection("\t");
                ev.Use();
                text = editor.text;
                GUI.changed = true;
            }
        }
        // else if (ev.isMouse)
        // {
        //     HandleScrolling(bounds, editor, ev, textArea);
        // }
        // else if (et == EventType.Repaint)
        // {

        // int pos = editor.cursorIndex;

        // //Find & Highlight Left Group
        // int first = FindBeforeGroup(text, pos);
        // bool redrawCursor = false;
        // if (first >= 0)
        // {
        //     DrawHighlight(style, editor, text, first, first + 1, Color.yellow);
        //     redrawCursor = true;
        // }

        // //Find & Highlight Right Group
        // int last = FindAfterGroup(text, pos);
        // if (last < text.Length)
        // {
        //     DrawHighlight(style, editor, text, last, last + 1, Color.yellow);
        //     redrawCursor = true;
        // }

        // //Do we need to redraw cursor?
        // if (editor.cursorIndex != editor.selectIndex)
        //     redrawCursor = false;
        // else
        // {
        //     //Find & Highlight Escapes
        //     int res = FindPossibleEscape(text, pos, out first, out last);
        //     if (res > 0)
        //     {
        //         Color scheme;
        //         switch (res)
        //         {
        //             case 1:
        //                 scheme = Color.green;
        //                 break;
        //             case 2:
        //                 scheme = Color.cyan;
        //                 break;
        //             case 3:
        //                 scheme = new Color(0.8f, 1f, 0);
        //                 break;
        //             default:
        //                 scheme = new Color(0.7f, 0.7f, 0.7f);
        //                 break;
        //         }
        //         DrawHighlight(style, editor, text, first, last + 1, scheme);
        //         redrawCursor = true;
        //     }
        // }
        // if (redrawCursor)
        // {
        //     bounds.position -= editor.scrollOffset;
        //     style.DrawCursor(bounds, textContent, id, editor.cursorIndex);
        // }
        // }

        return text;
    }

    private static void HandleValidateCommands(TextEditor editor, Event ev)
    {
        string commandName = ev.commandName;
        switch (commandName)
        {
            case "Cut":
            case "Copy":
                if (editor.hasSelection)
                {
                    ev.Use();
                }
                break;
            case "Paste":
                if (editor.CanPaste())
                {
                    ev.Use();
                }
                break;
            case "SelectAll":
            case "UndoRedoPerformed":
                ev.Use();
                break;
        }
    }

    private static float CalculateHeight(GUIContent content, GUIStyle style, int maxLineNumber, out float realHeight)
    {
        realHeight = EditorStyles.textArea.CalcHeight(
            content, EditorGUIUtility.currentViewWidth - 15);
        int num2 = Mathf.Clamp(Mathf.CeilToInt(realHeight / 15f), 3, maxLineNumber);
        return (32f + ((num2 - 1) * 15));
    }

    private static float scroll;

    private static void HandleScrolling(Rect r, TextEditor editor, Event ev, GUIStyle style)
    {
        float v = editor.scrollOffset.y;
        var vMax = style.CalcHeight(textContent, r.width);
        if (ev.type != EventType.Layout)
        {
            r.x += r.width;
            r.width = GUI.skin.verticalScrollbar.fixedWidth;
            scroll = GUI.VerticalScrollbar(r, scroll, r.height, 0, vMax);
            v = scroll;
        }
        else if (ev.type == EventType.ScrollWheel)
        {
            float num4 = v + (ev.delta.y * 10f);
            v = Mathf.Clamp(num4, 0f, vMax);
            Event.current.Use();
        }
        if (ev.type != EventType.Layout)
        {
            editor.scrollOffset = new Vector2(editor.scrollOffset.x, v);
        }
    }

    private static bool HandleExecuteCommands(TextEditor editor, Event ev)
    {
        string commandName = ev.commandName;
        Debug.Log(commandName);
        switch (commandName)
        {
            case "Cut":
                editor.Cut();
                return true;
            case "Copy":
                editor.Copy();
                ev.Use();
                return false;
            case "Paste":
                editor.Paste();
                ev.Use();
#if UNITY_5_2 || UNITY_5_3 || UNITY_5_4
                editor.UpdateScrollOffsetIfNeeded();
#else
                editor.UpdateScrollOffsetIfNeeded(ev);
#endif
                return true;
            case "SelectAll":
                editor.SelectAll();
                ev.Use();
                return false;
            case "UndoRedoPerformed":
                Undo.PerformUndo();
                ev.Use();
                return true;
        }
        return false;
    }

    private static void DrawHighlight(GUIStyle style, TextEditor editor, string text, int charFrom, int charTo, Color tint)
    {
        string word = text.Substring(charFrom, charTo - charFrom);
        var rect = new Rect(style.GetCursorPixelPosition(editor.position, textContent, charFrom)
            , style.GetCursorPixelPosition(editor.position, textContent, charTo));
        rect.size -= rect.position;
        if (rect.height > 0)
            return;
        rect.position -= editor.scrollOffset;
        rect.height += 16;

        var c = GUI.backgroundColor;
        GUI.backgroundColor = tint;
        GUI.Label(rect, word, styleBg);
        GUI.backgroundColor = c;
    }

    private static int FindBeforeGroup(string text, int pos)
    {
        pos--;
        if (text.Length <= 0 || pos < 0 || pos >= text.Length)
            return -1;
        var group = 0;
        while (pos >= 0)
        {
            if (text[pos] == '{')
            {
                if (pos == 0 || text[pos - 1] != '\\')
                {
                    if (group <= 0)
                        return pos;
                    else
                        group--;
                }
            }
            else if (text[pos] == '}')
            {
                if (pos == 0 || text[pos - 1] != '\\')
                    group++;
            }
            else if (text[pos] == '\n')
                return -1;
            pos--;
        }
        return pos;
    }

    private static int FindAfterGroup(string text, int pos)
    {
        if (text.Length <= 0 || pos < 0 || pos >= text.Length)
            return text.Length;
        var group = 0;
        while (pos < text.Length)
        {
            if (text[pos] == '}')
            {
                if (pos == 0 || text[pos - 1] != '\\')
                {
                    if (group <= 0)
                        return pos;
                    else
                        group--;
                }
            }
            else if (text[pos] == '{')
            {
                if (pos == 0 || text[pos - 1] != '\\')
                    group++;
            }
            else if (text[pos] == '\n')
                return text.Length;
            pos++;
        }
        return pos;
    }

    ///Returning: 0-Not found, 1-Symbol, 2-Command, 3-Font, 4-Misc
    private static int FindPossibleEscape(string text, int pos, out int start, out int end)
    {
        start = 0;
        end = 0;
        pos--;
        if (text.Length <= 0 || pos < 0 || pos >= text.Length)
            return 0;
        if (TexParserUtility.reservedChars.Contains(text[pos]))
        {
            if (pos == 0 || text[pos - 1] != '\\')
            {
                if (pos == 0 || text[pos] != '\\')
                    return 0;
                else if (pos < text.Length - 1)
                    pos++;
                else
                {
                    start = pos - 1;
                    end = pos;
                    return 4;
                }
            }
            else
            {
                start = pos - 1;
                end = pos;
                return 4;
            }
        }
        end = pos;
        while (true)
        {
            if (char.IsLetter(text[pos]))
            {
                pos--;
                if (pos >= 0)
                    continue;
                else
                    return 0;
            }
            if (text[pos] == '\\')
            {
                if (pos == 0 || text[pos - 1] != '\\')
                {
                    start = pos;
                    break;
                }
            }
            return 0;
        }
        while (end < text.Length && char.IsLetter(text[end]))
            end++;
        end--;
        var s = text.Substring(start + 1, end - start);
        if (TEXPreference.main.GetFontIndexByID(s) >= 0)
            return 3;
        //if (TexFormulaParser.isCommandRegistered(s))
        //    return 2;
        if (TEXPreference.main.GetChar(s) != null)
            return 1;
        return 4;
    }

    /* INCOMPLETE AUTOSUGGESTION API

	static int message2focus = -1;

	static bool HandleAutoComplete (TextEditor editor, ref string text)
	{
		var pos = editor.cursorIndex - 1;
		var end = pos;
		var start = 0;
		while (true) {
			if (char.IsLetter (text [pos])) {
				pos--;
				if (pos >= 0)
					continue;
				else
					return false;
			}
			if (text [pos] == '\\') {
				if (pos == 0 || text [pos - 1] != '\\') {
					start = pos;
					break;
				}
			}
			return false;
		}
		while (end < text.Length && char.IsLetter (text [end]))
			end++;
		var s = text.Substring (start + 1, end - start - 1);
		var sym = TEXPreference.main.symbolData;
		int t = sym.FindKeyIndex (s);
		if (t >= 0) {
			if (editor.cursorIndex != end) {
				editor.cursorIndex = end;
				return true;
			} else if (t < sym.Count - 1) {
				text = text.Substring (0, start + 1) + sym.keys [t + 1] + text.Substring (end);
				return true;
			}
			return false;
		} else {
			text = text.Substring (0, start + 1) + SuggestAutocomplete (s, sym.keys) + text.Substring (end);
			return true;
		}
	}
	static string SuggestAutocomplete (string s, List<string> keys)
	{
		for (int i = 0; i < keys.Count; i++) {
			var x = keys [i];
			if (x.Length >= s.Length && x.Substring (0, s.Length) == s)
				return x;
		}
		return s;
	}
	*/
}
