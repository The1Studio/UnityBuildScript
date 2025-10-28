using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.AddressableAssets.Settings;
using UnityEngine;

namespace BuildScripts.Editor.Addressable
{
    [Serializable]
    public class IncludeInBuildWithSymbolSchema : AddressableAssetGroupSchema
    {
        [SerializeField]
        private List<string> m_DefineSymbols = new List<string>();
        
        [SerializeField]
        private bool m_RequireAll = false;
        
        public List<string> DefineSymbols
        {
            get => m_DefineSymbols;
            set
            {
                m_DefineSymbols = value;
                SetDirty(true);
            }
        }
        
        public bool RequireAll
        {
            get => m_RequireAll;
            set
            {
                m_RequireAll = value;
                SetDirty(true);
            }
        }
        
        public bool ShouldIncludeInBuild()
        {
            if (m_DefineSymbols == null || m_DefineSymbols.Count == 0)
                return true;
            
            if (m_RequireAll)
            {
                foreach (var symbol in m_DefineSymbols)
                {
                    if (!string.IsNullOrWhiteSpace(symbol) && !BuildTools.IsDefineSet(symbol))
                        return false;
                }
                return true;
            }
            else
            {
                foreach (var symbol in m_DefineSymbols)
                {
                    if (!string.IsNullOrWhiteSpace(symbol) && BuildTools.IsDefineSet(symbol))
                        return true;
                }
                return false;
            }
        }
        
        public override void OnGUI()
        {
            EditorGUILayout.HelpBox(
                "This group will only be included in the build if the specified scripting define symbols are set.", 
                MessageType.Info);
            
            EditorGUILayout.Space();
            
            m_RequireAll = EditorGUILayout.Toggle("Require All Symbols", m_RequireAll);
            
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