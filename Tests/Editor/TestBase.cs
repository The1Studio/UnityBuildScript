using NUnit.Framework;
using UnityEditor;
using UnityEngine;

namespace BuildScript.Editor.Tests
{
    /// <summary>
    /// Base class for tests that need to manipulate scripting define symbols.
    /// Provides automatic setup/teardown to preserve original defines.
    /// </summary>
    public abstract class ScriptingDefinesTestBase
    {
        protected BuildTargetGroup testTargetGroup;
        protected string originalDefines;

        [SetUp]
        public virtual void Setup()
        {
            // Use Standalone for testing (works on all platforms)
            testTargetGroup = BuildTargetGroup.Standalone;

            // Save original defines to restore later
            originalDefines = PlayerSettings.GetScriptingDefineSymbolsForGroup(testTargetGroup);

            // Clear all defines for clean test state
            PlayerSettings.SetScriptingDefineSymbolsForGroup(testTargetGroup, "");
        }

        [TearDown]
        public virtual void TearDown()
        {
            // Restore original defines
            PlayerSettings.SetScriptingDefineSymbolsForGroup(testTargetGroup, originalDefines);
        }

        /// <summary>
        /// Helper method to set scripting defines for tests.
        /// </summary>
        protected void SetDefines(params string[] symbols)
        {
            PlayerSettings.SetScriptingDefineSymbolsForGroup(testTargetGroup, string.Join(";", symbols));
        }

        /// <summary>
        /// Helper method to get current scripting defines.
        /// </summary>
        protected string GetDefines()
        {
            return PlayerSettings.GetScriptingDefineSymbolsForGroup(testTargetGroup);
        }
    }

    /// <summary>
    /// Helper class for working with ScriptableObject instances in tests.
    /// Ensures proper cleanup to prevent memory leaks.
    /// </summary>
    public static class ScriptableObjectTestHelper
    {
        /// <summary>
        /// Creates a ScriptableObject instance for testing.
        /// Remember to call Destroy when done.
        /// </summary>
        public static T Create<T>() where T : ScriptableObject
        {
            return ScriptableObject.CreateInstance<T>();
        }

        /// <summary>
        /// Destroys a ScriptableObject instance immediately.
        /// Safe to call with null.
        /// </summary>
        public static void Destroy(Object obj)
        {
            if (obj != null)
            {
                Object.DestroyImmediate(obj);
            }
        }

        /// <summary>
        /// Creates a ScriptableObject, executes an action with it, then destroys it.
        /// Ensures cleanup even if the action throws an exception.
        /// </summary>
        public static void WithInstance<T>(System.Action<T> action) where T : ScriptableObject
        {
            T instance = null;
            try
            {
                instance = Create<T>();
                action(instance);
            }
            finally
            {
                Destroy(instance);
            }
        }

        /// <summary>
        /// Creates a ScriptableObject, executes a function with it, returns result, then destroys it.
        /// Ensures cleanup even if the function throws an exception.
        /// </summary>
        public static TResult WithInstance<T, TResult>(System.Func<T, TResult> func) where T : ScriptableObject
        {
            T instance = null;
            try
            {
                instance = Create<T>();
                return func(instance);
            }
            finally
            {
                Destroy(instance);
            }
        }
    }
}
