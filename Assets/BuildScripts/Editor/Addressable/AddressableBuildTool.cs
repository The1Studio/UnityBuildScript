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
#if !THEONE_NO_LZMA
            SetAllGroupsToLZMA();
#endif
            Console.WriteLine($"--------------------");
            Console.WriteLine($"Clean addressable");
            Console.WriteLine($"--------------------");
            AddressableAssetSettings.CleanPlayerContent();
            Console.WriteLine($"--------------------");
            Console.WriteLine($"Build addressable");
            Console.WriteLine($"--------------------");
            var setting = AddressableAssetSettingsDefaultObject.Settings;
#if PAD
#if ONDEMAND_ASSET
            PlayerSettings.Android.splitApplicationBinary = true;
            var buildScript = setting.DataBuilders.Find(builder => builder is BuildScriptPlayAssetDelivery);
#else
            PlayerSettings.Android.splitApplicationBinary = false;
            var buildScript = setting.DataBuilders.Find(builder => builder is BuildScriptPackedMode);
            ChangeDeliveryTypeFromOnDemandToInstallTime();
#endif
            if (buildScript == null)
            {
                throw new Exception("BuildScriptPackedMode not found.");
            }
            setting.ActivePlayerDataBuilderIndex = setting.DataBuilders.IndexOf(buildScript);
#endif
            //Refresh
            EditorUtility.SetDirty(setting);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            
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
        public static void SetAllGroupsToLZMA()
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
                    schema.Compression                       = BundledAssetGroupSchema.BundleCompressionMode.LZMA;
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
            var settings    = AddressableAssetSettingsDefaultObject.Settings;
            var profileName = "TheOneCDN";
            var profileId   = settings.profileSettings.GetProfileId(profileName);

            // If the profile does not exist, create it
            if (string.IsNullOrEmpty(profileId)) ;
            {
                profileId =  settings.profileSettings.AddProfile(profileName, "Default");
            }
            settings.activeProfileId = profileId;

            // Assuming 'BuildPath' and 'LoadPath' are the keys for the respective paths in your profile settings
            settings.profileSettings.SetValue(profileId, AddressableAssetSettings.kRemoteBuildPath, buildPath);
            settings.profileSettings.SetValue(profileId, AddressableAssetSettings.kRemoteLoadPath, loadPath);
            
            // Save changes
            settings.SetDirty(AddressableAssetSettings.ModificationEvent.ProfileModified, null, true, true);
            AssetDatabase.Refresh();
        }
    }
}