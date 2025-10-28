using System;
using UnityEngine;

namespace BuildScripts.Editor.Addressable
{
    [Serializable]
    public class ExcludeInBuildWithSymbolSchema : ConditionalBuildSchemaBase
    {
        [SerializeField]
        private bool m_ExcludeIfAny = true;

        public bool ExcludeIfAny
        {
            get => m_ExcludeIfAny;
            set
            {
                m_ExcludeIfAny = value;
                SetDirty(true);
            }
        }

        protected override string GetHelpBoxMessage() =>
            "This group will be excluded from the build if the specified scripting define symbols are set.";

        protected override string GetBooleanToggleLabel() => "Exclude If Any Symbol Set";

        protected override bool GetBooleanToggleValue() => m_ExcludeIfAny;

        protected override void SetBooleanToggleValue(bool value) => ExcludeIfAny = value;

        public bool ShouldExcludeFromBuild()
        {
            if (m_DefineSymbols == null || m_DefineSymbols.Count == 0)
                return false;

            return m_ExcludeIfAny
                ? BuildTools.IsAnyDefineSet(m_DefineSymbols)
                : !BuildTools.AreAllDefinesSet(m_DefineSymbols);
        }

        public override bool ShouldIncludeInBuild()
        {
            return !ShouldExcludeFromBuild();
        }
    }
}
