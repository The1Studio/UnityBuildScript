namespace BuildScripts.Editor.Addressable
{
    using System;
    using UnityEditor;
    using UnityEditor.AddressableAssets;
#if PAD
    using UnityEditor.AddressableAssets.Android;
    using UnityEngine.AddressableAssets.Android;
#endif
    using UnityEditor.AddressableAssets.Build;
    using UnityEditor.AddressableAssets.Build.DataBuilders;
    using UnityEditor.AddressableAssets.Settings;
    using UnityEditor.AddressableAssets.Settings.GroupSchemas;
    using UnityEngine;

    public class AddressableBuildTool
    {
        public static void BuildAddressable()
        {
#if PRODUCTION || CLEAN_ADDRESSABLES
            Console.WriteLine($"--------------------");
            Console.WriteLine($"Clean addressable");
            Console.WriteLine($"--------------------");
            AddressableAssetSettings.CleanPlayerContent();
#endif

            var setting = AddressableAssetSettingsDefaultObject.Settings;

            // Always apply conditional build rules based on symbols
            ApplyConditionalBuildRules();
#if UNITY_6000_0_OR_NEWER
            //TODO disable it when find a case that need to split APK
            PlayerSettings.Android.splitApplicationBinary = false; // Disable split APK
#if PAD
#if ONDEMAND_ASSET
            PlayerSettings.Android.splitApplicationBinary = true;
            var buildScript = setting.DataBuilders.Find(builder => builder is BuildScriptPlayAssetDelivery);
#else
            var buildScript = setting.DataBuilders.Find(builder => builder is BuildScriptPackedMode);
            ChangeDeliveryTypeFromOnDemandToInstallTime();
#endif
            if (buildScript == null)
            {
                throw new Exception("BuildScriptPackedMode not found.");
            }

            setting.ActivePlayerDataBuilderIndex = setting.DataBuilders.IndexOf(buildScript);
#endif
#endif

            //Refresh
            EditorUtility.SetDirty(setting);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Console.WriteLine($"--------------------");
            Console.WriteLine($"Build addressable");
            Console.WriteLine($"--------------------");
            AddressableAssetSettings.BuildPlayerContent(out AddressablesPlayerBuildResult result);
            var success = string.IsNullOrEmpty(result.Error);
            if (!success)
            {
                var errorMessage = "Addressable build error encountered: " + result.Error;
                Debug.LogError(errorMessage);
                throw new Exception(errorMessage);
            }

            Console.WriteLine($"--------------------");
            Console.WriteLine($"Finish building addressable");
            Console.WriteLine($"--------------------");
        }

        [MenuItem("TheOne/Set All Groups to LZMA")]
        public static void SetAllGroupsToLZMA() { SetAddressableGroupCompressionMode(BundledAssetGroupSchema.BundleCompressionMode.LZMA); }

        [MenuItem("TheOne/Set All Groups to LZ4")]
        public static void SetAllGroupsToLZ4() { SetAddressableGroupCompressionMode(BundledAssetGroupSchema.BundleCompressionMode.LZ4); }

        private static void SetAddressableGroupCompressionMode(BundledAssetGroupSchema.BundleCompressionMode compressionMode)
        {
            // Access the addressable asset settings
            var settings = AddressableAssetSettingsDefaultObject.Settings;

            // Iterate over all groups
            foreach (var group in settings.groups)
            {
                // Set the compression type to LZMA for each group
                var schema = group.GetSchema<BundledAssetGroupSchema>();
                if (schema != null)
                {
                    schema.Compression                       = compressionMode;
                    schema.UseUnityWebRequestForLocalBundles = false;
                }
            }

            settings.SetDirty(AddressableAssetSettings.ModificationEvent.GroupSchemaModified, null, true, true);
        }

#if PAD
        [MenuItem("TheOne/Change from OnDemand to InstallTime")]
        public static void ChangeDeliveryTypeFromOnDemandToInstallTime()
        {
            // Access the addressable asset settings
            var settings = AddressableAssetSettingsDefaultObject.Settings;

            // Iterate over all groups
            foreach (var group in settings.groups)
            {
                // Set the compression type to LZMA for each group
                var schema = group.GetSchema<PlayAssetDeliverySchema>();
                if (schema != null && schema.AssetPackDeliveryType == DeliveryType.OnDemand)
                {
                    schema.AssetPackDeliveryType = DeliveryType.InstallTime;
                    settings.SetDirty(AddressableAssetSettings.ModificationEvent.GroupSchemaModified, null, true, true);
                }
            }
        }
#endif

        public static void CreateOrUpdateTheOneCDNProfile(string buildPath, string loadPath)
        {
#if UNITY_6000_0_OR_NEWER
            var settings    = AddressableAssetSettingsDefaultObject.Settings;
            var profileName = "TheOneCDN";
            var profileId   = settings.profileSettings.GetProfileId(profileName);

            // If the profile does not exist, create it
            if (string.IsNullOrEmpty(profileId)) ;
            {
                profileId = settings.profileSettings.AddProfile(profileName, "Default");
            }
            settings.activeProfileId = profileId;

            // Assuming 'BuildPath' and 'LoadPath' are the keys for the respective paths in your profile settings
            settings.profileSettings.SetValue(profileId, AddressableAssetSettings.kRemoteBuildPath, buildPath);
            settings.profileSettings.SetValue(profileId, AddressableAssetSettings.kRemoteLoadPath, loadPath);

            // Save changes
            settings.SetDirty(AddressableAssetSettings.ModificationEvent.ProfileModified, null, true, true);
            AssetDatabase.Refresh();
#endif
        }

        public static void ApplyConditionalBuildRules()
        {
            bool isProduction = BuildTools.IsDefineSet("PRODUCTION");
            
            // Check for specific defines or development build
            bool includeDebug = EditorUserBuildSettings.development || BuildTools.IsDefineSet("INCLUDE_DEBUG_ASSETS");
            bool includeCreative = BuildTools.IsDefineSet("INCLUDE_CREATIVE_ASSETS");
            bool includeEditor = BuildTools.IsDefineSet("INCLUDE_EDITOR_ASSETS");

            // In PRODUCTION, force exclude Debug and Creative groups regardless of other settings
            if (isProduction)
            {
                includeDebug = false;
                includeCreative = false;
                includeEditor = false;
                Debug.Log($"[Addressables Conditional Build] PRODUCTION build - forcing exclusion of Debug, Creative, and Editor groups");
            }

            // Apply conditional rules for different group prefixes
            ToggleGroupsByNamePrefix("Debug", includeDebug);
            ToggleGroupsByNamePrefix("Creative", includeCreative);
            ToggleGroupsByNamePrefix("Editor", includeEditor);

            // Log the applied rules
            Debug.Log($"[Addressables Conditional Build] Debug groups: {(includeDebug ? "Included" : "Excluded")}");
            Debug.Log($"[Addressables Conditional Build] Creative groups: {(includeCreative ? "Included" : "Excluded")}");
            Debug.Log($"[Addressables Conditional Build] Editor groups: {(includeEditor ? "Included" : "Excluded")}");
        }

        public static void ToggleGroupsByNamePrefix(string prefix, bool include)
        {
            var settings = AddressableAssetSettingsDefaultObject.Settings;
            if (settings == null)
            {
                Debug.LogWarning("Addressables settings not found.");
                return;
            }

            foreach (var group in settings.groups)
            {
                if (group == null) continue;
                // Case-insensitive comparison for prefix
                if (!group.name.StartsWith(prefix, System.StringComparison.OrdinalIgnoreCase)) continue;

                var schema = group.GetSchema<BundledAssetGroupSchema>();
                if (schema == null) continue;

                if (schema.IncludeInBuild != include)
                {
                    schema.IncludeInBuild = include;
                    EditorUtility.SetDirty(schema);
                    Debug.Log($"[Addressables] Group '{group.name}' IncludeInBuild = {include}");
                }
            }

            AssetDatabase.SaveAssets();
        }

        // Entry point for CI/CD conditional build (same as BuildAddressable now)
        public static void BuildConditional()
        {
            Debug.Log("[Addressables] Starting conditional build from CI/CD...");
            BuildAddressable();
        }
    }
}