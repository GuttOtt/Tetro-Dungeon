using Extensions;
using UnityEditor;
using UnityEngine;
/*
[CustomEditor(typeof(CharacterBlockConfig))]
public class CharacterBlockConfigEditor : Editor
{
    public override void OnInspectorGUI() {
        // CharacterBlockConfig ��ü�� ������
        CharacterBlockConfig config = (CharacterBlockConfig)target;

        // �⺻ �ν����͸� �׸��ϴ�.
        DrawDefaultInspector();

        // LevelShapePair �߰� ��ư
        if (GUILayout.Button("Add LevelShapePair")) {
            AddLevelShapePair(config);
        }

        // �ν����͸� ������Ʈ
        if (GUI.changed) {
            EditorUtility.SetDirty(target);
        }
    }

    private void AddLevelShapePair(CharacterBlockConfig config) {
        LevelShapePair newShapePair = new LevelShapePair();
        newShapePair.Level = 0; // �⺻ ���� ����

        // TArray<bool>�� �⺻ ũ�� �� �ʱⰪ ����
        newShapePair.Shape = new TArray<bool>();
        newShapePair.Shape.Resize(3, 3); // ��: 5�� ũ�� ����

        // �⺻���� ������ LevelShapePair �߰�
        ArrayUtility.Add(ref config.Shapes, newShapePair);
    }
}
*/