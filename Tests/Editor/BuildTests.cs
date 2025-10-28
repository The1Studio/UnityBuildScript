using NUnit.Framework;

namespace BuildScript.Editor.Tests
{
    /// <summary>
    /// Tests for Build class public API.
    /// Note: Many Build class members are private/internal, so tests focus on publicly accessible constants.
    /// </summary>
    [TestFixture]
    public class BuildTests
    {
        #region Platform Constants Tests

        [Test]
        public void PlatformConstants_AreNotEmpty()
        {
            // Assert - Platform constants should be defined
            Assert.IsNotEmpty(Build.PlatformWin32, "PlatformWin32 should not be empty");
            Assert.IsNotEmpty(Build.PlatformWin64, "PlatformWin64 should not be empty");
            Assert.IsNotEmpty(Build.PlatformOsx, "PlatformOsx should not be empty");
            Assert.IsNotEmpty(Build.PlatformAndroid, "PlatformAndroid should not be empty");
            Assert.IsNotEmpty(Build.PlatformIOS, "PlatformIOS should not be empty");
            Assert.IsNotEmpty(Build.PlatformWebGL, "PlatformWebGL should not be empty");
        }

        [Test]
        public void PlatformConstants_HaveExpectedValues()
        {
            // Assert - Verify platform string values match expected identifiers
            Assert.AreEqual("win-x86", Build.PlatformWin32);
            Assert.AreEqual("win-x64", Build.PlatformWin64);
            Assert.AreEqual("osx-x64", Build.PlatformOsx);
            Assert.AreEqual("android", Build.PlatformAndroid);
            Assert.AreEqual("ios", Build.PlatformIOS);
            Assert.AreEqual("webgl", Build.PlatformWebGL);
        }

        [Test]
        public void PlatformConstants_AreAllUnique()
        {
            // Arrange
            var platforms = new System.Collections.Generic.HashSet<string>
            {
                Build.PlatformWin32,
                Build.PlatformWin64,
                Build.PlatformOsx,
                Build.PlatformAndroid,
                Build.PlatformIOS,
                Build.PlatformWebGL
            };

            // Assert - All 6 platforms should be unique
            Assert.AreEqual(6, platforms.Count, "All platform constants should be unique");
        }

        #endregion

        #region GetLogFileName Tests

        [Test]
        public void GetLogFileName_WithValidPlatform_ReturnsNonEmptyString()
        {
            // Act
            var logFileName = Build.GetLogFileName(Build.PlatformWin64);

            // Assert
            Assert.IsNotEmpty(logFileName, "Log file name should not be empty");
        }

        [Test]
        public void GetLogFileName_WithDifferentPlatforms_ReturnsDifferentNames()
        {
            // Act
            var win64Log = Build.GetLogFileName(Build.PlatformWin64);
            var androidLog = Build.GetLogFileName(Build.PlatformAndroid);

            // Assert - Different platforms should produce different log file names
            Assert.AreNotEqual(win64Log, androidLog, "Different platforms should have different log file names");
        }

        [Test]
        public void GetLogFileName_ContainsPlatformIdentifier()
        {
            // Act
            var platform = Build.PlatformAndroid;
            var logFileName = Build.GetLogFileName(platform);

            // Assert - Log filename should contain the platform identifier
            Assert.That(logFileName, Does.Contain(platform),
                $"Log filename should contain platform identifier '{platform}'");
        }

        #endregion
    }
}
