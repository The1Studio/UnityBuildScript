using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.AddressableAssets.Settings;
using UnityEngine;

namespace BuildScripts.Editor.Addressable
{
    [Serializable]
    public abstract class ConditionalBuildSchemaBase : AddressableAssetGroupSchema
    {
        [SerializeField]
        protected List<string> m_DefineSymbols = new List<string>();

        public List<string> DefineSymbols
        {
            get => m_DefineSymbols;
            set
            {
                m_DefineSymbols = value;
                SetDirty(true);
            }
        }

        protected abstract string GetHelpBoxMessage();
        protected abstract string GetBooleanToggleLabel();
        protected abstract bool GetBooleanToggleValue();
        protected abstract void SetBooleanToggleValue(bool value);
        public abstract bool ShouldIncludeInBuild();

        public override void OnGUI()
        {
            EditorGUILayout.HelpBox(GetHelpBoxMessage(), MessageType.Info);
            EditorGUILayout.Space();

            bool toggleValue = EditorGUILayout.Toggle(GetBooleanToggleLabel(), GetBooleanToggleValue());
            SetBooleanToggleValue(toggleValue);

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Define Symbols:", EditorStyles.boldLabel);

            if (m_DefineSymbols == null)
                m_DefineSymbols = new List<string>();

            RenderSymbolsList();

            EditorGUILayout.Space();

            if (GUILayout.Button("Add Symbol"))
            {
                m_DefineSymbols.Add("");
                SetDirty(true);
            }

            EditorGUILayout.Space();

            EditorGUILayout.HelpBox(
                $"Current Status: {(ShouldIncludeInBuild() ? "✓ Will be included" : "✗ Will be excluded")}",
                ShouldIncludeInBuild() ? MessageType.Info : MessageType.Warning);
        }

        protected void RenderSymbolsList()
        {
            for (int i = 0; i < m_DefineSymbols.Count; i++)
            {
                EditorGUILayout.BeginHorizontal();
                m_DefineSymbols[i] = EditorGUILayout.TextField($"Symbol {i + 1}", m_DefineSymbols[i]);

                if (GUILayout.Button("Remove", GUILayout.Width(BuildScripts.Editor.BuildConstants.REMOVE_BUTTON_WIDTH)))
                {
                    m_DefineSymbols.RemoveAt(i);
                    SetDirty(true);
                    i--;
                }
                EditorGUILayout.EndHorizontal();
            }
        }

        public override void OnGUIMultiple(List<AddressableAssetGroupSchema> otherSchemas)
        {
            OnGUI();
        }
    }
}
