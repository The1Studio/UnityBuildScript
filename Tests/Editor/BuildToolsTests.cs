using NUnit.Framework;
using UnityEditor;

namespace BuildScript.Editor.Tests
{
    [TestFixture]
    public class BuildToolsTests : ScriptingDefinesTestBase
    {

        #region IsDefineSet Tests

        [Test]
        public void IsDefineSet_WithExistingSymbol_ReturnsTrue()
        {
            // Arrange
            SetDefines("TEST_SYMBOL");

            // Act
            bool result = BuildTools.IsDefineSet("TEST_SYMBOL", testTargetGroup);

            // Assert
            Assert.IsTrue(result, "Should return true for existing symbol");
        }

        [Test]
        public void IsDefineSet_WithNonExistentSymbol_ReturnsFalse()
        {
            // Arrange
            SetDefines("OTHER_SYMBOL");

            // Act
            bool result = BuildTools.IsDefineSet("TEST_SYMBOL", testTargetGroup);

            // Assert
            Assert.IsFalse(result, "Should return false for non-existent symbol");
        }

        [Test]
        public void IsDefineSet_WithPartialMatch_ReturnsFalse()
        {
            // Arrange: Set "PRODUCTION" but check for "PROD"
            SetDefines("PRODUCTION");

            // Act
            bool result = BuildTools.IsDefineSet("PROD", testTargetGroup);

            // Assert
            Assert.IsFalse(result, "Should not match partial symbol names");
        }

        [Test]
        public void IsDefineSet_WithMultipleSymbols_FindsCorrectOne()
        {
            // Arrange
            SetDefines("SYMBOL1", "SYMBOL2", "SYMBOL3");

            // Act & Assert
            Assert.IsTrue(BuildTools.IsDefineSet("SYMBOL1", testTargetGroup));
            Assert.IsTrue(BuildTools.IsDefineSet("SYMBOL2", testTargetGroup));
            Assert.IsTrue(BuildTools.IsDefineSet("SYMBOL3", testTargetGroup));
            Assert.IsFalse(BuildTools.IsDefineSet("SYMBOL4", testTargetGroup));
        }

        [Test]
        public void IsDefineSet_WithEmptyDefines_ReturnsFalse()
        {
            // Arrange
            SetDefines();

            // Act
            bool result = BuildTools.IsDefineSet("ANY_SYMBOL", testTargetGroup);

            // Assert
            Assert.IsFalse(result);
        }

        #endregion

        #region AddDefineSymbol Tests

        [Test]
        public void AddDefineSymbol_ToEmptyList_AddsSymbol()
        {
            // Arrange
            SetDefines();

            // Act
            BuildTools.AddDefineSymbol(testTargetGroup, "NEW_SYMBOL");

            // Assert
            string defines = PlayerSettings.GetScriptingDefineSymbolsForGroup(testTargetGroup);
            Assert.AreEqual("NEW_SYMBOL", defines);
        }

        [Test]
        public void AddDefineSymbol_ToExistingList_AppendsSymbol()
        {
            // Arrange
            SetDefines("EXISTING");

            // Act
            BuildTools.AddDefineSymbol(testTargetGroup, "NEW_SYMBOL");

            // Assert
            string defines = PlayerSettings.GetScriptingDefineSymbolsForGroup(testTargetGroup);
            Assert.That(defines, Does.Contain("EXISTING"));
            Assert.That(defines, Does.Contain("NEW_SYMBOL"));
        }

        [Test]
        public void AddDefineSymbol_DuplicateSymbol_DoesNotAddTwice()
        {
            // Arrange
            SetDefines("EXISTING");

            // Act
            BuildTools.AddDefineSymbol(testTargetGroup, "EXISTING");

            // Assert
            string defines = PlayerSettings.GetScriptingDefineSymbolsForGroup(testTargetGroup);
            // Should not have duplicate separators like "EXISTING;;EXISTING"
            Assert.AreEqual("EXISTING", defines);
        }

        #endregion

        #region RemoveDefineSymbol Tests

        [Test]
        public void RemoveDefineSymbol_FromSingleSymbol_ClearsDefines()
        {
            // Arrange
            SetDefines("SYMBOL_TO_REMOVE");

            // Act
            BuildTools.RemoveDefineSymbol(testTargetGroup, "SYMBOL_TO_REMOVE");

            // Assert
            string defines = PlayerSettings.GetScriptingDefineSymbolsForGroup(testTargetGroup);
            Assert.IsEmpty(defines);
        }

        [Test]
        public void RemoveDefineSymbol_FromMultiple_RemovesOnlyTarget()
        {
            // Arrange
            SetDefines("KEEP1", "REMOVE", "KEEP2");

            // Act
            BuildTools.RemoveDefineSymbol(testTargetGroup, "REMOVE");

            // Assert
            string defines = PlayerSettings.GetScriptingDefineSymbolsForGroup(testTargetGroup);
            Assert.That(defines, Does.Contain("KEEP1"));
            Assert.That(defines, Does.Contain("KEEP2"));
            Assert.That(defines, Does.Not.Contain("REMOVE"));
        }

        [Test]
        public void RemoveDefineSymbol_NonExistent_DoesNotError()
        {
            // Arrange
            SetDefines("EXISTING");

            // Act & Assert (should not throw)
            Assert.DoesNotThrow(() => BuildTools.RemoveDefineSymbol(testTargetGroup, "NON_EXISTENT"));

            string defines = PlayerSettings.GetScriptingDefineSymbolsForGroup(testTargetGroup);
            Assert.That(defines, Does.Contain("EXISTING"));
        }

        #endregion

        #region RemoveDefineSymbols Tests

        [Test]
        public void RemoveDefineSymbols_Multiple_RemovesAll()
        {
            // Arrange
            SetDefines("REMOVE1", "KEEP", "REMOVE2", "REMOVE3");

            // Act
            BuildTools.RemoveDefineSymbols(testTargetGroup, "REMOVE1", "REMOVE2", "REMOVE3");

            // Assert
            string defines = PlayerSettings.GetScriptingDefineSymbolsForGroup(testTargetGroup);
            Assert.AreEqual("KEEP", defines);
        }

        [Test]
        public void RemoveDefineSymbols_EmptyArray_DoesNothing()
        {
            // Arrange
            SetDefines("KEEP");

            // Act
            BuildTools.RemoveDefineSymbols(testTargetGroup);

            // Assert
            string defines = PlayerSettings.GetScriptingDefineSymbolsForGroup(testTargetGroup);
            Assert.AreEqual("KEEP", defines);
        }

        #endregion

        #region AreAllDefinesSet Tests

        [Test]
        public void AreAllDefinesSet_AllPresent_ReturnsTrue()
        {
            // Arrange
            SetDefines("SYMBOL1", "SYMBOL2", "SYMBOL3");

            // Act
            bool result = BuildTools.AreAllDefinesSet(new[] { "SYMBOL1", "SYMBOL2", "SYMBOL3" });

            // Assert
            Assert.IsTrue(result);
        }

        [Test]
        public void AreAllDefinesSet_OneMissing_ReturnsFalse()
        {
            // Arrange
            SetDefines("SYMBOL1", "SYMBOL2");

            // Act
            bool result = BuildTools.AreAllDefinesSet(new[] { "SYMBOL1", "SYMBOL2", "SYMBOL3" });

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void AreAllDefinesSet_EmptyList_ReturnsTrue()
        {
            // Arrange
            SetDefines("ANYTHING");

            // Act
            bool result = BuildTools.AreAllDefinesSet(new string[] { });

            // Assert
            Assert.IsTrue(result, "Empty list should return true");
        }

        [Test]
        public void AreAllDefinesSet_WithWhitespace_IgnoresWhitespace()
        {
            // Arrange
            SetDefines("SYMBOL1");

            // Act
            bool result = BuildTools.AreAllDefinesSet(new[] { "SYMBOL1", "", "  " });

            // Assert
            Assert.IsTrue(result, "Should ignore empty/whitespace entries");
        }

        #endregion

        #region IsAnyDefineSet Tests

        [Test]
        public void IsAnyDefineSet_OnePresent_ReturnsTrue()
        {
            // Arrange
            SetDefines("SYMBOL2");

            // Act
            bool result = BuildTools.IsAnyDefineSet(new[] { "SYMBOL1", "SYMBOL2", "SYMBOL3" });

            // Assert
            Assert.IsTrue(result);
        }

        [Test]
        public void IsAnyDefineSet_NonePresent_ReturnsFalse()
        {
            // Arrange
            SetDefines("OTHER");

            // Act
            bool result = BuildTools.IsAnyDefineSet(new[] { "SYMBOL1", "SYMBOL2", "SYMBOL3" });

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void IsAnyDefineSet_EmptyList_ReturnsFalse()
        {
            // Arrange
            SetDefines("ANYTHING");

            // Act
            bool result = BuildTools.IsAnyDefineSet(new string[] { });

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void IsAnyDefineSet_WithWhitespace_IgnoresWhitespace()
        {
            // Arrange
            SetDefines();

            // Act
            bool result = BuildTools.IsAnyDefineSet(new[] { "", "  ", "   " });

            // Assert
            Assert.IsFalse(result, "Should ignore whitespace-only entries");
        }

        #endregion
    }
}
