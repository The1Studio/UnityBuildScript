using NUnit.Framework;
using System.Text.RegularExpressions;
using BuildScripts.Editor;

namespace BuildScript.Editor.Tests
{
    [TestFixture]
    public class BuildConstantsTests
    {
        #region Path Constants Tests

        [Test]
        public void BUILD_LOGS_PATH_IsNotEmpty()
        {
            // Assert
            Assert.IsNotEmpty(BuildConstants.BUILD_LOGS_PATH, "BUILD_LOGS_PATH should not be empty");
        }

        [Test]
        public void BUILD_LOGS_PATH_IsValidRelativePath()
        {
            // Assert
            Assert.That(BuildConstants.BUILD_LOGS_PATH, Does.StartWith(".."),
                "BUILD_LOGS_PATH should be a relative path starting with ..");
        }

        [Test]
        public void BUILD_LOGS_PATH_DoesNotContainInvalidCharacters()
        {
            // Assert
            Assert.IsFalse(BuildConstants.BUILD_LOGS_PATH.Contains("\\"),
                "BUILD_LOGS_PATH should use forward slashes, not backslashes");
        }

        #endregion

        #region Log File Constants Tests

        [Test]
        public void LOG_FILE_PREFIX_IsNotEmpty()
        {
            // Assert
            Assert.IsNotEmpty(BuildConstants.LOG_FILE_PREFIX, "LOG_FILE_PREFIX should not be empty");
        }

        [Test]
        public void LOG_FILE_PREFIX_IsDescriptive()
        {
            // Assert
            Assert.That(BuildConstants.LOG_FILE_PREFIX, Does.Contain("Build"),
                "LOG_FILE_PREFIX should contain 'Build' for clarity");
        }

        [Test]
        public void LOG_FILE_PREFIX_DoesNotContainInvalidFilenameCharacters()
        {
            // Arrange
            char[] invalidChars = System.IO.Path.GetInvalidFileNameChars();

            // Assert
            foreach (char invalidChar in invalidChars)
            {
                Assert.IsFalse(BuildConstants.LOG_FILE_PREFIX.Contains(invalidChar.ToString()),
                    $"LOG_FILE_PREFIX should not contain invalid filename character: {invalidChar}");
            }
        }

        #endregion

        #region UI Constants Tests

        [Test]
        public void REMOVE_BUTTON_WIDTH_IsPositive()
        {
            // Assert
            Assert.Greater(BuildConstants.REMOVE_BUTTON_WIDTH, 0,
                "REMOVE_BUTTON_WIDTH should be a positive value");
        }

        [Test]
        public void REMOVE_BUTTON_WIDTH_IsReasonable()
        {
            // Assert - Button width should be between 50 and 200 pixels (reasonable UI range)
            Assert.GreaterOrEqual(BuildConstants.REMOVE_BUTTON_WIDTH, 50,
                "REMOVE_BUTTON_WIDTH should be at least 50 pixels");
            Assert.LessOrEqual(BuildConstants.REMOVE_BUTTON_WIDTH, 200,
                "REMOVE_BUTTON_WIDTH should not exceed 200 pixels");
        }

        #endregion

        #region iOS Constants Tests

        [Test]
        public void DEFAULT_IOS_TARGET_VERSION_IsNotEmpty()
        {
            // Assert
            Assert.IsNotEmpty(BuildConstants.DEFAULT_IOS_TARGET_VERSION,
                "DEFAULT_IOS_TARGET_VERSION should not be empty");
        }

        [Test]
        public void DEFAULT_IOS_TARGET_VERSION_HasValidFormat()
        {
            // Assert - Should be in format like "13.0", "14.0", "15.2", etc.
            var versionPattern = @"^\d+\.\d+$";
            Assert.IsTrue(Regex.IsMatch(BuildConstants.DEFAULT_IOS_TARGET_VERSION, versionPattern),
                $"DEFAULT_IOS_TARGET_VERSION should match format 'X.Y', got: {BuildConstants.DEFAULT_IOS_TARGET_VERSION}");
        }

        [Test]
        public void DEFAULT_IOS_TARGET_VERSION_IsReasonableMinimum()
        {
            // Assert - Should be iOS 11.0 or higher (reasonable minimum for modern apps)
            var parts = BuildConstants.DEFAULT_IOS_TARGET_VERSION.Split('.');
            var majorVersion = int.Parse(parts[0]);

            Assert.GreaterOrEqual(majorVersion, 11,
                $"DEFAULT_IOS_TARGET_VERSION should be iOS 11.0 or higher, got: {BuildConstants.DEFAULT_IOS_TARGET_VERSION}");
        }

        [Test]
        public void DEFAULT_IOS_TARGET_VERSION_IsNotTooHigh()
        {
            // Assert - Should not be unreasonably high (e.g., not iOS 20+)
            var parts = BuildConstants.DEFAULT_IOS_TARGET_VERSION.Split('.');
            var majorVersion = int.Parse(parts[0]);

            Assert.LessOrEqual(majorVersion, 18,
                $"DEFAULT_IOS_TARGET_VERSION seems unreasonably high: {BuildConstants.DEFAULT_IOS_TARGET_VERSION}");
        }

        #endregion

        #region Consistency Tests

        [Test]
        public void AllStringConstants_AreNotNull()
        {
            // Assert
            Assert.IsNotNull(BuildConstants.BUILD_LOGS_PATH, "BUILD_LOGS_PATH should not be null");
            Assert.IsNotNull(BuildConstants.LOG_FILE_PREFIX, "LOG_FILE_PREFIX should not be null");
            Assert.IsNotNull(BuildConstants.DEFAULT_IOS_TARGET_VERSION, "DEFAULT_IOS_TARGET_VERSION should not be null");
        }

        [Test]
        public void AllStringConstants_AreNotWhitespace()
        {
            // Assert
            Assert.IsFalse(string.IsNullOrWhiteSpace(BuildConstants.BUILD_LOGS_PATH),
                "BUILD_LOGS_PATH should not be whitespace");
            Assert.IsFalse(string.IsNullOrWhiteSpace(BuildConstants.LOG_FILE_PREFIX),
                "LOG_FILE_PREFIX should not be whitespace");
            Assert.IsFalse(string.IsNullOrWhiteSpace(BuildConstants.DEFAULT_IOS_TARGET_VERSION),
                "DEFAULT_IOS_TARGET_VERSION should not be whitespace");
        }

        [Test]
        public void AllNumericConstants_AreNonNegative()
        {
            // Assert
            Assert.GreaterOrEqual(BuildConstants.REMOVE_BUTTON_WIDTH, 0,
                "REMOVE_BUTTON_WIDTH should be non-negative");
        }

        #endregion

        #region Usage Documentation Tests

        [Test]
        public void BuildConstants_ClassExists_AndIsStatic()
        {
            // Assert - Verify the BuildConstants class exists and is static
            var type = typeof(BuildConstants);
            Assert.IsTrue(type.IsAbstract && type.IsSealed,
                "BuildConstants should be a static class (abstract and sealed in IL)");
        }

        [Test]
        public void BuildConstants_AllFields_ArePublicConst()
        {
            // Assert - All fields should be public const for compile-time constants
            var type = typeof(BuildConstants);
            var fields = type.GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);

            foreach (var field in fields)
            {
                Assert.IsTrue(field.IsLiteral && !field.IsInitOnly,
                    $"Field {field.Name} should be a const, not readonly or variable");
            }
        }

        #endregion
    }
}
