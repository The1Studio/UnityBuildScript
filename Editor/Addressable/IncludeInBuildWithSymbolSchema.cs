using System;
using UnityEngine;

namespace BuildScripts.Editor.Addressable
{
    [Serializable]
    public class IncludeInBuildWithSymbolSchema : ConditionalBuildSchemaBase
    {
        [SerializeField]
        private bool m_RequireAll = false;

        public bool RequireAll
        {
            get => m_RequireAll;
            set
            {
                m_RequireAll = value;
                SetDirty(true);
            }
        }

        protected override string GetHelpBoxMessage() =>
            "This group will only be included in the build if the specified scripting define symbols are set.";

        protected override string GetBooleanToggleLabel() => "Require All Symbols";

        protected override bool GetBooleanToggleValue() => m_RequireAll;

        protected override void SetBooleanToggleValue(bool value) => RequireAll = value;

        public override bool ShouldIncludeInBuild()
        {
            if (m_DefineSymbols == null || m_DefineSymbols.Count == 0)
                return true;

            return m_RequireAll
                ? BuildTools.AreAllDefinesSet(m_DefineSymbols)
                : BuildTools.IsAnyDefineSet(m_DefineSymbols);
        }
    }
}
