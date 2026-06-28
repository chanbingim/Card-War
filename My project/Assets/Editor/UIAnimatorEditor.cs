using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(DoTweenAnimator))]
public class UIAnimatorEditor : Editor
{
    private SerializedProperty _animDatas;

    private void OnEnable()
    {
        _animDatas = serializedObject.FindProperty("_AnimList");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        // 리스트 그리기
        EditorGUILayout.PropertyField(_animDatas, true);

        EditorGUILayout.Space();

        // 추가 버튼
        if (GUILayout.Button("ADD Animation"))
        {
            ShowAddMenu();
        }

        ShowAnimation_TimeLine();
        serializedObject.ApplyModifiedProperties();
    }

    private void ShowAddMenu()
    {
        GenericMenu menu = new GenericMenu();

        menu.AddItem(
            new GUIContent("Transform Animation"),
            false,
            () => AddAnimation(typeof(TransformAnimData)));

        menu.ShowAsContext();
    }

    private void AddAnimation(System.Type type)
    {
        int index = _animDatas.arraySize;
        _animDatas.InsertArrayElementAtIndex(index);

        SerializedProperty element =
            _animDatas.GetArrayElementAtIndex(index);

        element.managedReferenceValue =
            System.Activator.CreateInstance(type);

        serializedObject.ApplyModifiedProperties();
    }

    void ShowAnimation_TimeLine()
    {
        int index = _animDatas.arraySize;
        int FrameWidth = 10;
        for (int i = 0; i < index; i++)
        {
            Rect rect = GUILayoutUtility.GetRect(300, 30);
            var property = _animDatas.GetArrayElementAtIndex(i);
            var anim = property.managedReferenceValue as UIAnimData;

            float startX = rect.x + anim.StartFrame * FrameWidth;
            float width = (anim.EndFrame - anim.StartFrame) * FrameWidth;

            Rect animRect = new Rect(startX, rect.y, width, 20);
            EditorGUI.DrawRect(animRect, Color.cyan);
        }
    }
}
