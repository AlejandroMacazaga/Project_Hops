using System;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Utils.BlackboardSystem
{
    [CustomEditor(typeof(BlackboardData))]
    public class BlackboardDataEditor : Editor
    {
        private ReorderableList _list;

        private void OnEnable()
        {
            _list = new ReorderableList(serializedObject, serializedObject.FindProperty("entries"), true, true, true,
                true)
            {
                drawHeaderCallback = rect =>
                {
                    EditorGUI.LabelField(new Rect(rect.x, rect.y, rect.width * 0.3f, EditorGUIUtility.singleLineHeight),
                        "Key");
                    EditorGUI.LabelField(new Rect(rect.x + rect.width * 0.3f + 10, rect.y, rect.width * 0.3f,
                            EditorGUIUtility.singleLineHeight), "Type");
                    EditorGUI.LabelField(new Rect(rect.x + rect.width * 0.6f + 5, rect.y, rect.width * 0.4f,  EditorGUIUtility.singleLineHeight),
                        "Value");
                }
            };

            _list.drawElementCallback = (rect, index, active, focused) =>
            {
                var element = _list.serializedProperty.GetArrayElementAtIndex(index);

                rect.y += 2;
                var keyName = element.FindPropertyRelative("keyName");
                var valueType = element.FindPropertyRelative("valueType");
                var value = element.FindPropertyRelative("value");
                
                var keyNameRect = new Rect(rect.x, rect.y, rect.width * 0.3f, EditorGUIUtility.singleLineHeight);
                var valueTypeRect = new Rect(rect.x + rect.width * 0.3f, rect.y, rect.width * 0.3f, EditorGUIUtility.singleLineHeight);
                var valueRect = new Rect(rect.x + rect.width * 0.6f, rect.y, rect.width * 0.4f, EditorGUIUtility.singleLineHeight);

                EditorGUI.PropertyField(keyNameRect, keyName, GUIContent.none);
                EditorGUI.PropertyField(valueTypeRect, valueType, GUIContent.none);

                switch ((AnyValue.ValueType)valueType.enumValueIndex)
                {
                    case AnyValue.ValueType.Int:
                        break;
                    case AnyValue.ValueType.Float:
                        break;
                    case AnyValue.ValueType.String:
                        break;
                    case AnyValue.ValueType.Bool:
                        var boolValue = value.FindPropertyRelative("boolValue");
                        EditorGUI.PropertyField(valueRect, boolValue, GUIContent.none);
                        break;
                    case AnyValue.ValueType.Vector3:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            };
        }
        
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            _list.DoLayoutList();
            serializedObject.ApplyModifiedProperties();
        }
    }
}