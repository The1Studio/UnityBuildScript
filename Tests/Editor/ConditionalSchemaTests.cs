using NUnit.Framework;
using System.Collections.Generic;
using BuildScripts.Editor.Addressable;

namespace BuildScript.Editor.Tests
{
    [TestFixture]
    public class ConditionalSchemaTests : ScriptingDefinesTestBase
    {
        #region IncludeInBuildWithSymbolSchema Tests

        [Test]
        public void IncludeSchema_RequireAll_AllSymbolsPresent_ReturnsTrue()
        {
            ScriptableObjectTestHelper.WithInstance<IncludeInBuildWithSymbolSchema>(schema =>
            {
                schema.DefineSymbols = new List<string> { "SYMBOL1", "SYMBOL2", "SYMBOL3" };
                schema.RequireAll = true;
                SetDefines("SYMBOL1", "SYMBOL2", "SYMBOL3");

                Assert.IsTrue(schema.ShouldIncludeInBuild(), "Should include when all required symbols are present");
            });
        }

        [Test]
        public void IncludeSchema_RequireAll_OneMissing_ReturnsFalse()
        {
            ScriptableObjectTestHelper.WithInstance<IncludeInBuildWithSymbolSchema>(schema =>
            {
                schema.DefineSymbols = new List<string> { "SYMBOL1", "SYMBOL2", "SYMBOL3" };
                schema.RequireAll = true;
                SetDefines("SYMBOL1", "SYMBOL2");

                Assert.IsFalse(schema.ShouldIncludeInBuild(), "Should not include when one required symbol is missing");
            });
        }

        [Test]
        public void IncludeSchema_RequireAny_OnePresent_ReturnsTrue()
        {
            ScriptableObjectTestHelper.WithInstance<IncludeInBuildWithSymbolSchema>(schema =>
            {
                schema.DefineSymbols = new List<string> { "SYMBOL1", "SYMBOL2", "SYMBOL3" };
                schema.RequireAll = false;
                SetDefines("SYMBOL2");

                Assert.IsTrue(schema.ShouldIncludeInBuild(), "Should include when any symbol is present (RequireAny mode)");
            });
        }

        [Test]
        public void IncludeSchema_RequireAny_NonePresent_ReturnsFalse()
        {
            ScriptableObjectTestHelper.WithInstance<IncludeInBuildWithSymbolSchema>(schema =>
            {
                schema.DefineSymbols = new List<string> { "SYMBOL1", "SYMBOL2", "SYMBOL3" };
                schema.RequireAll = false;
                SetDefines("OTHER_SYMBOL");

                Assert.IsFalse(schema.ShouldIncludeInBuild(), "Should not include when no symbols are present");
            });
        }

        [Test]
        public void IncludeSchema_EmptySymbolList_ReturnsTrue()
        {
            ScriptableObjectTestHelper.WithInstance<IncludeInBuildWithSymbolSchema>(schema =>
            {
                schema.DefineSymbols = new List<string>();
                schema.RequireAll = true;
                SetDefines("ANYTHING");

                Assert.IsTrue(schema.ShouldIncludeInBuild(), "Should include when symbol list is empty (no requirements)");
            });
        }

        [Test]
        public void IncludeSchema_WithWhitespace_IgnoresWhitespace()
        {
            ScriptableObjectTestHelper.WithInstance<IncludeInBuildWithSymbolSchema>(schema =>
            {
                schema.DefineSymbols = new List<string> { "SYMBOL1", "", "  ", "SYMBOL2" };
                schema.RequireAll = true;
                SetDefines("SYMBOL1", "SYMBOL2");

                Assert.IsTrue(schema.ShouldIncludeInBuild(), "Should ignore whitespace-only symbols");
            });
        }

        #endregion

        #region ExcludeInBuildWithSymbolSchema Tests

        [Test]
        public void ExcludeSchema_ExcludeIfAny_OnePresent_ReturnsFalse()
        {
            ScriptableObjectTestHelper.WithInstance<ExcludeInBuildWithSymbolSchema>(schema =>
            {
                schema.DefineSymbols = new List<string> { "EXCLUDE1", "EXCLUDE2", "EXCLUDE3" };
                schema.ExcludeIfAny = true; // Exclude if ANY symbol is present
                SetDefines("EXCLUDE2");

                Assert.IsFalse(schema.ShouldIncludeInBuild(), "Should exclude when any exclusion symbol is present");
            });
        }

        [Test]
        public void ExcludeSchema_ExcludeIfAny_NonePresent_ReturnsTrue()
        {
            ScriptableObjectTestHelper.WithInstance<ExcludeInBuildWithSymbolSchema>(schema =>
            {
                schema.DefineSymbols = new List<string> { "EXCLUDE1", "EXCLUDE2", "EXCLUDE3" };
                schema.ExcludeIfAny = true; // Exclude if ANY symbol is present
                SetDefines("OTHER_SYMBOL");

                Assert.IsTrue(schema.ShouldIncludeInBuild(), "Should include when no exclusion symbols are present");
            });
        }

        [Test]
        public void ExcludeSchema_NotExcludeIfAny_AllPresent_ReturnsFalse()
        {
            ScriptableObjectTestHelper.WithInstance<ExcludeInBuildWithSymbolSchema>(schema =>
            {
                schema.DefineSymbols = new List<string> { "EXCLUDE1", "EXCLUDE2", "EXCLUDE3" };
                schema.ExcludeIfAny = false; // Exclude only if NOT all symbols are present
                SetDefines("EXCLUDE1", "EXCLUDE2", "EXCLUDE3");

                // When ExcludeIfAny=false and all symbols present: !AreAllDefinesSet(true) = !true = false
                // ShouldExcludeFromBuild = false, so ShouldIncludeInBuild = true
                Assert.IsTrue(schema.ShouldIncludeInBuild(), "Should include when all symbols are present (ExcludeIfAny=false)");
            });
        }

        [Test]
        public void ExcludeSchema_NotExcludeIfAny_OneMissing_ReturnsTrue()
        {
            ScriptableObjectTestHelper.WithInstance<ExcludeInBuildWithSymbolSchema>(schema =>
            {
                schema.DefineSymbols = new List<string> { "EXCLUDE1", "EXCLUDE2", "EXCLUDE3" };
                schema.ExcludeIfAny = false; // Exclude if NOT all symbols are present
                SetDefines("EXCLUDE1", "EXCLUDE2");

                // When ExcludeIfAny=false and one missing: !AreAllDefinesSet(false) = !false = true
                // ShouldExcludeFromBuild = true, so ShouldIncludeInBuild = false
                Assert.IsFalse(schema.ShouldIncludeInBuild(), "Should exclude when not all symbols are present (ExcludeIfAny=false)");
            });
        }

        [Test]
        public void ExcludeSchema_EmptySymbolList_ReturnsTrue()
        {
            ScriptableObjectTestHelper.WithInstance<ExcludeInBuildWithSymbolSchema>(schema =>
            {
                schema.DefineSymbols = new List<string>();
                schema.ExcludeIfAny = true;
                SetDefines("ANYTHING");

                Assert.IsTrue(schema.ShouldIncludeInBuild(), "Should include when exclusion list is empty (no exclusions)");
            });
        }

        [Test]
        public void ExcludeSchema_WithWhitespace_IgnoresWhitespace()
        {
            ScriptableObjectTestHelper.WithInstance<ExcludeInBuildWithSymbolSchema>(schema =>
            {
                schema.DefineSymbols = new List<string> { "EXCLUDE1", "", "  " };
                schema.ExcludeIfAny = true;
                SetDefines("OTHER");

                Assert.IsTrue(schema.ShouldIncludeInBuild(), "Should ignore whitespace-only symbols in exclusion list");
            });
        }

        #endregion

        #region Combined Logic Tests

        [Test]
        public void IncludeAndExcludeSchemas_ComplexScenario_WorkCorrectly()
        {
            var includeSchema = ScriptableObjectTestHelper.Create<IncludeInBuildWithSymbolSchema>();
            var excludeSchema = ScriptableObjectTestHelper.Create<ExcludeInBuildWithSymbolSchema>();

            try
            {
                includeSchema.DefineSymbols = new List<string> { "FEATURE_A", "FEATURE_B" };
                includeSchema.RequireAll = false; // Any

                excludeSchema.DefineSymbols = new List<string> { "EXCLUDE_DEBUG" };
                excludeSchema.ExcludeIfAny = true; // Exclude if any

                // Scenario 1: Has FEATURE_A, no exclusions -> Should include
                SetDefines("FEATURE_A");
                Assert.IsTrue(includeSchema.ShouldIncludeInBuild(), "Should include with FEATURE_A");
                Assert.IsTrue(excludeSchema.ShouldIncludeInBuild(), "Should not exclude without EXCLUDE_DEBUG");

                // Scenario 2: Has FEATURE_A and EXCLUDE_DEBUG -> Include yes, Exclude no
                SetDefines("FEATURE_A", "EXCLUDE_DEBUG");
                Assert.IsTrue(includeSchema.ShouldIncludeInBuild(), "Should include with FEATURE_A");
                Assert.IsFalse(excludeSchema.ShouldIncludeInBuild(), "Should exclude with EXCLUDE_DEBUG");

                // Scenario 3: No features -> Should not include
                SetDefines("OTHER");
                Assert.IsFalse(includeSchema.ShouldIncludeInBuild(), "Should not include without features");
                Assert.IsTrue(excludeSchema.ShouldIncludeInBuild(), "Should not exclude without EXCLUDE_DEBUG");
            }
            finally
            {
                ScriptableObjectTestHelper.Destroy(includeSchema);
                ScriptableObjectTestHelper.Destroy(excludeSchema);
            }
        }

        #endregion
    }
}
