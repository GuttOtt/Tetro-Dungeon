using Extensions;
using UnityEditor;
using UnityEngine;
/*
[CustomEditor(typeof(CharacterBlockConfig))]
public class CharacterBlockConfigEditor : Editor
{
    public override void OnInspectorGUI() {
        // CharacterBlockConfig 객체를 가져옴
        CharacterBlockConfig config = (CharacterBlockConfig)target;

        // 기본 인스펙터를 그립니다.
        DrawDefaultInspector();

        // LevelShapePair 추가 버튼
        if (GUILayout.Button("Add LevelShapePair")) {
            AddLevelShapePair(config);
        }

        // 인스펙터를 업데이트
        if (GUI.changed) {
            EditorUtility.SetDirty(target);
        }
    }

    private void AddLevelShapePair(CharacterBlockConfig config) {
        LevelShapePair newShapePair = new LevelShapePair();
        newShapePair.Level = 0; // 기본 레벨 설정

        // TArray<bool>의 기본 크기 및 초기값 설정
        newShapePair.Shape = new TArray<bool>();
        newShapePair.Shape.Resize(3, 3); // 예: 5로 크기 설정

        // 기본값을 설정한 LevelShapePair 추가
        ArrayUtility.Add(ref config.Shapes, newShapePair);
    }
}
*/