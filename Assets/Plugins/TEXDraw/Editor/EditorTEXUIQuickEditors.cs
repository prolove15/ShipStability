using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class EditorTEXUIQuickEditors : EditorWindow
{
    public RectTransform root;
    public Vector2 scr;
    public Vector2 size;

    public const string helpInfo = "This editor helps you to edit TEXDraw UI quickly" +
        " as possible with benefit from TEXDraw's dedicated layout System.\n" +
        "Put an empty RectTransform and we'll begin editing";

    private void OnGUI()
    {
        root = (RectTransform)EditorGUILayout.ObjectField(root, typeof(RectTransform), true);
        if (root)
        {
            HandleFitting();
            scr = EditorGUILayout.BeginScrollView(scr, GUILayout.ExpandHeight(true));
            size = root.rect.size;
            GUIDraw(root.gameObject);
            EditorGUILayout.EndScrollView();
        }
        else
        {
            EditorGUILayout.HelpBox(helpInfo, MessageType.Info);
        }
    }

    private void HandleFitting()
    {
        EditorGUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));
        if (GUILayout.Button("<>", GUILayout.Width(30)))
            Selection.activeGameObject = root.gameObject;
        var fitter = root.GetComponent<ContentSizeFitter>();
        if (!fitter)
        {
            if (GUILayout.Button("Add ContentSizeFitter"))
                Undo.AddComponent(root.gameObject, typeof(ContentSizeFitter));
        }
        else
        {
            EditorGUI.BeginChangeCheck();
            var bX = EditorGUILayout.ToggleLeft("Fit Horizontal", fitter.horizontalFit == ContentSizeFitter.FitMode.PreferredSize);
            if (EditorGUI.EndChangeCheck())
                fitter.horizontalFit = bX ? ContentSizeFitter.FitMode.PreferredSize : ContentSizeFitter.FitMode.Unconstrained;

            EditorGUI.BeginChangeCheck();
            var bY = EditorGUILayout.ToggleLeft("Fit Vertical", fitter.verticalFit == ContentSizeFitter.FitMode.PreferredSize);
            if (EditorGUI.EndChangeCheck())
                fitter.verticalFit = bY ? ContentSizeFitter.FitMode.PreferredSize : ContentSizeFitter.FitMode.Unconstrained;
        }
        EditorGUILayout.EndHorizontal();
    }

    private void GUIDraw(GameObject obj)
    {
        var tex = obj.GetComponent<TEXDraw>();
        if (tex)
            GUIDrawNode(tex);
        else
        {
            var layout = obj.GetComponent<HorizontalOrVerticalLayoutGroup>();
            if (layout)
                GUIDrawChildrens(layout);
        }
    }

    private void GUIDrawChildrens(HorizontalOrVerticalLayoutGroup layout)
    {
        var isHorizontal = layout is HorizontalLayoutGroup;
        Rect r;
        r = EditorGUILayout.BeginVertical();
        GUI.Box(r, GUIContent.none);
        DrawLayoutHandler(layout);
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.GetControlRect(GUILayout.Width(10));
        if (!isHorizontal)
            EditorGUILayout.BeginVertical();
        {
            var t = layout.GetComponent<RectTransform>();
            if (!t)
                t = layout.gameObject.AddComponent<RectTransform>();
            inHorizontalLayout = isHorizontal;
            for (int i = 0; i < t.childCount; i++)
            {
                GUIDraw(t.GetChild(i).gameObject);
            }
        }
        EditorGUILayout.Space();
        if (!isHorizontal)
            EditorGUILayout.EndVertical();
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.EndVertical();
    }

    private void DrawLayoutHandler(HorizontalOrVerticalLayoutGroup layout)
    {
        {
            bool isHorizontal = layout is HorizontalLayoutGroup;
            GUILayout.Space(10);
            EditorGUILayout.BeginHorizontal();
            EditorGUI.BeginChangeCheck();
            if (GUILayout.Button("<>", GUILayout.Width(30)))
                Selection.activeGameObject = layout.gameObject;
            if (GUILayout.Button(isHorizontal ? "H" : "V", GUILayout.Width(30)))
            {
                var space = layout.spacing;
                var padding = layout.padding;
                var obj = layout.gameObject;
                Undo.DestroyObjectImmediate(layout);
                if (isHorizontal)
                    Undo.AddComponent(obj, typeof(VerticalLayoutGroup));
                else
                    Undo.AddComponent(obj, typeof(HorizontalLayoutGroup));
                layout.spacing = space;
                layout.padding = padding;
            }
            var rS = EditorGUILayout.GetControlRect(GUILayout.MinWidth(30));
            if (rS.width < 100)
                layout.spacing = EditorGUI.FloatField(rS, layout.spacing);
            else
                layout.spacing = EditorGUI.Slider(rS, layout.spacing, 0, 100f);
            var r = EditorGUILayout.GetControlRect(GUILayout.Width(50));
            if (GUI.Button(r, "+"))
            {
                OnAddInside = true;
                EditorUtility.DisplayCustomMenu(r, addContext, -1, new EditorUtility.SelectMenuItemFunction(OnAddPopup), layout.gameObject);
            }
            if (EditorGUI.EndChangeCheck())
                EditorUtility.SetDirty(layout);
            EditorGUILayout.EndHorizontal();
        }
    }

    private static GUIContent[] insertContext = new GUIContent[] {
        new GUIContent("Duplicate"),
        new GUIContent("Insert TEXDraw"),
        new GUIContent("Insert Image"),
        new GUIContent("Insert Empty"),
        new GUIContent("Insert Horizontal Layout"),
        new GUIContent("Insert Vertical Layout")
    };

    private static GUIContent[] addContext = new GUIContent[] {
        new GUIContent("Duplicate"),
        new GUIContent("Add TEXDraw"),
        new GUIContent("Add Image"),
        new GUIContent("Add Empty"),
        new GUIContent("Add Horizontal Layout"),
        new GUIContent("Add Vertical Layout")
    };

    private bool OnAddInside = false;

    private void OnAddPopup(object u, string[] opt, int sel)
    {
        if (sel == 0 && OnAddInside)
        {
            if (((GameObject)u) == root.gameObject)
                throw new ArgumentException("Can't duplicate root. It is simply impractical");
            OnAddInside = false;
        }
        var transObj = ((GameObject)u).transform;
        var parent = OnAddInside ? transObj : transObj.parent;
        GameObject newObj;
        switch (sel)
        {
            case 0:
                newObj = Instantiate<GameObject>((GameObject)u);
                break;
            case 1:
                newObj = new GameObject("TEXDraw", new Type[] { typeof(TEXDraw) });
                break;
            case 2:
                newObj = new GameObject("Image", new Type[] { typeof(TEXDraw) });
                break;
            case 3:
                newObj = new GameObject("GameObject", new Type[] { typeof(RectTransform) });
                break;
            case 4:
                newObj = new GameObject("Horizontal Group", new Type[] { typeof(HorizontalLayoutGroup) });
                break;
            case 5:
                newObj = new GameObject("Vertical Group", new Type[] { typeof(VerticalLayoutGroup) });
                break;
            default:
                newObj = null;
                break;
        }
        newObj.transform.SetParent(parent, false);
        if (!OnAddInside)
            newObj.transform.SetSiblingIndex(transObj.GetSiblingIndex());

        Undo.RegisterCreatedObjectUndo(newObj, "Creating a GameObject");
    }

    private void OnHierarchyChange()
    {
        Repaint();
    }

    private bool inHorizontalLayout = false;

    private void GUIHandleFitter(GameObject obj)
    {
        EditorGUI.BeginChangeCheck();
        var fitter = obj.GetComponent<LayoutElement>();
        float h = fitter ? fitter.preferredWidth : -1, v = fitter ? fitter.preferredHeight : -1;
        //var rect = obj.GetComponent<RectTransform>();
        if (inHorizontalLayout)
            h = EditorGUILayout.Slider(h, -1, 500);
        else
            v = EditorGUILayout.Slider(v, -1, 500);
        if (EditorGUI.EndChangeCheck())
        {
            if (!fitter)
                fitter = obj.AddComponent<LayoutElement>();
            fitter.preferredWidth = h;
            fitter.preferredHeight = v;
        }
    }

    private void GUIDrawNode(TEXDraw tex)
    {
        EditorGUI.BeginChangeCheck();
        EditorGUILayout.BeginVertical();
        EditorGUILayout.Space();
        var rBox = EditorGUILayout.BeginVertical();
        GUI.Box(rBox, GUIContent.none);
        EditorGUILayout.BeginHorizontal();
        {
            var r = EditorGUILayout.GetControlRect(GUILayout.Width(20f));
            if (GUI.Button(r, "+"))
            {
                OnAddInside = false;
                EditorUtility.DisplayCustomMenu(r, insertContext, -1, new EditorUtility.SelectMenuItemFunction(OnAddPopup), tex.gameObject);
            }
            if (GUI.Button(EditorGUILayout.GetControlRect(GUILayout.Width(20f)), "-"))
                Undo.DestroyObjectImmediate(tex.gameObject);
            if (GUI.Button(EditorGUILayout.GetControlRect(GUILayout.Width(30f)), "<>"))
                Selection.activeGameObject = tex.gameObject;
            //tex.autoFit = (Fitting)EditorGUILayout.EnumPopup(tex.autoFit, GUILayout.ExpandWidth(true));
        }
        EditorGUILayout.EndHorizontal();
        if (!tex)
        {
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndVertical();
            EditorGUI.EndChangeCheck();
            return;
        }
        tex.text = TEXBoxHighlighting.DrawText(tex.text, 10, GUILayout.Width(90f), GUILayout.ExpandWidth(true));
        {
            //if (tex.autoFit != Fitting.RectSize)
            GUIHandleFitter(tex.gameObject);
        }
        EditorGUILayout.Space();
        EditorGUILayout.EndVertical();
        EditorGUILayout.EndVertical();
        if (EditorGUI.EndChangeCheck() && tex)
        {
            EditorUtility.SetDirty(tex);
        }
    }
}
