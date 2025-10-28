using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.AddressableAssets.Settings;
using UnityEngine;

namespace BuildScripts.Editor.Addressable
{
    [Serializable]
    public class ExcludeInBuildWithSymbolSchema : AddressableAssetGroupSchema
    {
        [SerializeField]
        private List<string> m_DefineSymbols = new List<string>();
        
        [SerializeField]
        private bool m_ExcludeIfAny = true;
        
        public List<string> DefineSymbols
        {
            get => m_DefineSymbols;
            set
            {
                m_DefineSymbols = value;
                SetDirty(true);
            }
        }
        
        public bool ExcludeIfAny
        {
            get => m_ExcludeIfAny;
            set
            {
                m_ExcludeIfAny = value;
                SetDirty(true);
            }
        }
        
        public bool ShouldExcludeFromBuild()
        {
            if (m_DefineSymbols == null || m_DefineSymbols.Count == 0)
                return false;
            
            if (m_ExcludeIfAny)
            {
                foreach (var symbol in m_DefineSymbols)
                {
                    if (!string.IsNullOrWhiteSpace(symbol) && BuildTools.IsDefineSet(symbol))
                        return true;
                }
                return false;
            }
            else
            {
                foreach (var symbol in m_DefineSymbols)
                {
                    if (!string.IsNullOrWhiteSpace(symbol) && !BuildTools.IsDefineSet(symbol))
                        return false;
                }
                return true;
            }
        }
        
        public bool ShouldIncludeInBuild()
        {
            return !ShouldExcludeFromBuild();
        }
        
        public override void OnGUI()
        {
            EditorGUILayout.HelpBox(
                "This group will be excluded from the build if the specified scripting define symbols are set.", 
                MessageType.Info);
            
            EditorGUILayout.Space();
            
            m_ExcludeIfAny = EditorGUILayout.Toggle("Exclude If Any Symbol Set", m_ExcludeIfAny);
            
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Define Symbols:", EditorStyles.boldLabel);
            
            if (m_DefineSymbols == null)
                m_DefineSymbols = new List<string>();
            
            for (int i = 0; i < m_DefineSymbols.Count; i++)
            {
                EditorGUILayout.BeginHorizontal();
                m_DefineSymbols[i] = EditorGUILayout.TextField($"Symbol {i + 1}", m_DefineSymbols[i]);
                
                if (GUILayout.Button("Remove", GUILayout.Width(70)))
                {
                    m_DefineSymbols.RemoveAt(i);
                    SetDirty(true);
                    i--;
                }
                EditorGUILayout.EndHorizontal();
            }
            
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
        
        public override void OnGUIMultiple(List<AddressableAssetGroupSchema> otherSchemas)
        {
            OnGUI();
        }
    }
}