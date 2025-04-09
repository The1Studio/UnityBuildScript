#if UNITY_IOS
namespace BuildScripts.Editor
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using GameFoundation.BuildScripts.Runtime;
    using UnityEditor;
    using UnityEditor.Callbacks;
    using UnityEditor.iOS.Xcode;
    using UnityEditor.iOS.Xcode.Extensions;
    using UnityEngine;

    public class IOSPostProcessingBuildTool
    {
        #region SKNetworks

        private static readonly List<string> SkNetworks = new()
        {
            "22mmun2rn5", "238da6jt44", "24t9a8vw3c", "24zw6aqk47", "252b5q8x7y", "275upjj5gd", "294l99pt4k", "2fnua5tdw4", "2q884k2j68", "2rq3zucswp", "2tdux39lx8", "2u9pt9hc89", "32z4fx6l9h", "33r6p7g8nc", "3cgn6rq224", "3l6bd9hu43", "3qcr597p9d", "3qy4746246", "3rd42ekr43", "3sh42y64q3", "424m5254lk", "4468km3ulz", "44jx6755aq", "44n7hlldy6", "47vhws6wlr", "488r3q3dtq", "4dzt52r2t5", "4fzdc2evr5", "4mn522wn87", "4pfyvq9l8r", "4w7y6s5ca2", "523jb4fst2", "52fl2v3hgk", "54nzkqm89y", "55644vm79v", "55y65gfgn7", "577p5t736z", "578prtvx9j", "5a6flpkh64", "5f5u5tfb26", "5ghnmfs3dh", "5l3tpt7t6e", "5lm9lj6jb7", "5mv394q32t", "5tjdwbrq8w", "627r9wr2y5", "633vhxswh4", "67369282zy", "6964rsfnh4", "6g9af3uyq4", "6p4ks3rnbw", "6qx585k4p6", "6rd35atwn8", "6v7lgmsu45", "6xzpu9s2p8", "6yxyv74ff7", "737z793b9f", "74b6s63p6l", "7953jerfzd", "79pbpufp6p", "79w64w269u", "7bxrt786m8", "7fbxrn65az", "7fmhfwg9en", "7k3cvf297u", "7rz58n8ntl", "7tnzynbdc7", "7ug5zh24hu", "84993kbrcf", "866k9ut3g3", "87u5trcl3r", "88k8774x49", "899vrgt9g8", "89z7zv988g", "8c4e2ghe7u", "8m87ys6875", "8qiegk9qfv", "8r8llnkz5a", "8s468mfl3y", "8w3np9l82g", "97r2b46745", "9b89h5y424", "9g2aggbj52", "9nlqeag3gk", "9rd848q2bz", "9t245vhmpl", "9vvzujtq5s", "9wsyqb3ku7", "9yg77x724h", "a2p9lx4jpn", "a7xqa6mtl2", "a8cz6cu7e5", "au67k4efj4", "av6w8kgt66", "axh5283zss", "b55w3d8y8z", "b9bk5wbcq9", "bvpn9ufa9b", "bxvub5ada5", "c3frkrj4fj", "c6k4g5qg8m", "c7g47wypnu", "cad8qz2s3j", "ce8ybjwass", "cg4yq2srnc", "cj5566h2ga", "CJ5566H2GA", "cp8zw746q7", "cs644xg564", "cstr6suwn9", "cwn433xbcr", "d7g9azk84q", "dbu4b84rxf", "dd3a75yxkv", "dkc879ngq3", "dmv22haz9p", "dn942472g5", "dr774724x4", "dt3cjx1a9i", "dticjx1a9i", "dzg6xy7pwj", "e5fvkxwrpn", "ecpz2srf59", "eh6m2bh4zr", "ejvt5qm6ak", "eqhxz8m8av", "f2zub97jtl", "f38h382jlk", "f73kdq92p3", "f7s53z58qe", "feyaarzu9v", "fkak3gfpt6", "fq6vru337s", "fz2k2k5tej", "g28c52eehv", "g2y4y55b64", "g69uk9uh2b", "g6gcrrvk4p", "gfat3222tu", "ggvn48r87g", "glqzh8vgby", "gta8lk7p23", "gta9lk7p23", "gvmwg8q7h5", "h5jmj969g5", "h65wbv5k3f", "h8vml93bkz", "hb56zgv37p", "hdw39hrw9y", "hjevpa356n", "hs6bdukanm", "jb7bn6koa5", "jk2fsx2rgz", "k674qkevps", "k6y4y55b64", "kbd757ywx3", "kbmxgpxpgc", "klf5c3l5u5", "krvm3zuq6h", "l6nv3x923s", "l93v5h6a4m", "ln5gz23vtd", "Ln5gz23vtd", "lr83yxwka7", "ludvb6z3bs", "m297p6643m", "m5mvw97r93", "M5mvw97r93", "m8dbw4sv7c", "mj797d8u6f", "mlmmfzh3r3", "mls7yz5dvl", "mp6xlyr22a", "mqn7fxpca7", "mtkv5xtk9e", "n38lu8286q", "n66cz3y3bx", "n6fk4nfna4", "n9x2a789qt", "nfqy3847ph", "nrt9jy4kw9", "ns5j362hk7", "nu4557a4je", "nzq8sh4pbs", "p78axxw29g", "pd25vrrwzn", "ppxm28t8ap", "prcb7njmu6", "pt89h2hlb7", "pu4na253f3", "pwa73g5rt2", "pwdxu55a5a", "qlbq5gtkt8", "qqp299437r", "qu637u8glc", "qwpu75vrh2", "r26jy69rpl", "r45fhb6rf7", "r8lj5b58b5", "rvh3l7un93", "rx5hdcabgc", "s39g8k73mm", "s69wq72ugq", "sczv5946wb", "su67r6k2v3", "t38b2kh725", "t3b3f7n3x8", "t6d3zquu66", "t7ky8fmwkd", "tl55sbb4fm", "tmhh9296z4", "tvvz7th9br", "u679fj5vs4", "uw77j35x4d", "uzqba5354d", "v4nxqhlyqp", "v72qych5uu", "V72QYCH5UU", "v7896pgt74", "v79kvwwj4g", "v9wttpbfk9", "vc83br9sjg", "vcra2ehyfk", "Vcra2ehyfk", "vhf287vqwu", "vutu7akeur", "w28pnjg2k4", "w7jznl3r6g", "w9q455wk68", "W9Q455WK68", "wg4vff78zm", "wzmmz9fp6w", "wzmmZ9fp6w", "x2jnk7ly8j", "x44k69ngh6", "x5854y7y24", "x5l83yy675", "x8jxxk4ff5", "x8uqf25wch", "x8yj322td6", "xga6mpmplv", "xmn954pzmp", "xx9sdjej2w", "xy9t38ct57", "XY9T38CT57", "y45688jllp", "y5ghdn5j9k", "y755zyxw56", "yclnxrl5pm", "ydx93a7ass", "yrqqpx2mcb", "z24wtl6j62", "z4gj7hsk7h", "z5b3gh5ugf", "z959bm4gru", "zh3b7bxvad", "zmvfpc5aq8", "zq492l623r"
        };

        #endregion

        #region Embed Dynamic Libraries

        private const string TARGET_UNITY_IPHONE_PODFILE_LINE    = "target 'Unity-iPhone' do";
        private const string USE_FRAMEWORKS_PODFILE_LINE         = "use_frameworks!";
        private const string USE_FRAMEWORKS_DYNAMIC_PODFILE_LINE = "use_frameworks! :linkage => :dynamic";
        private const string USE_FRAMEWORKS_STATIC_PODFILE_LINE  = "use_frameworks! :linkage => :static";

        private static readonly List<string> DynamicLibrariesToEmbed = new List<string>
        {
            "DTBiOSSDK.xcframework",
            #if !UNITY_2021_3_OR_NEWER
            "IASDKCore.xcframework",
            #endif
            "ATOM.xcframework",
            "OMSDK_Pubnativenet.xcframework",
            "OMSDK_Ogury.xcframework",
            "OMSDK_Appodeal.xcframework",
            #if IRONSOURCE
            "InMobiSDK.xcframework",
            "AppLovinSDK.xcframework",
            #endif
        };

        #endregion

        [PostProcessBuild(int.MaxValue)]
        public static void OnPostProcessBuild(BuildTarget buildTarget, string path)
        {
            try
            {
                SetPlistConfig(path);
                SetProjectConfig(path);

                Debug.Log("onelog: IOSPostProcessingBuildTool OnPostProcessBuild Success");
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                throw;
            }
        }

        #region Main

        private static void SetProjectConfig(string path)
        {
            var projectPath = path + "/Unity-iPhone.xcodeproj/project.pbxproj";

            var pbxProject = new PBXProject();
            pbxProject.ReadFromString(File.ReadAllText(projectPath));

            var mainTargetGuid           = pbxProject.GetUnityMainTargetGuid();
            var testTargetGuid           = pbxProject.TargetGuidByName(PBXProject.GetUnityTestTargetName());
            var unityFrameworkTargetGuid = pbxProject.GetUnityFrameworkTargetGuid();
            var projectGuid              = pbxProject.ProjectGuid();
            var pbxProjectPath           = PBXProject.GetPBXProjectPath(path);

            SetProjectConfig(pbxProject, mainTargetGuid, testTargetGuid, unityFrameworkTargetGuid, projectGuid);
            SetCapability(pbxProjectPath, mainTargetGuid);
            EmbedDynamicLibrariesIfNeeded(path, pbxProject, mainTargetGuid);

            File.WriteAllText(projectPath, pbxProject.WriteToString());
            Debug.Log("onelog: IOSPostProcessingBuildTool SetProjectConfig Success");
        }

        private static void SetPlistConfig(string pathToBuiltProject)
        {
            var plistPath = pathToBuiltProject + "/Info.plist";
            var plist     = new PlistDocument();
            plist.ReadFromString(File.ReadAllText(plistPath));
            var rootDict = plist.root;

            // Disable Firebase screen view tracking
            rootDict.SetBoolean("FirebaseAutomaticScreenReportingEnabled", false);
            rootDict.SetBoolean("FirebaseAppStoreReceiptURLCheckEnabled", false);

            // add this to use google mobile ads (iron source mediation)
            rootDict.SetBoolean("GADIsAdManagerApp", true);

            // add NSUserTrackingUsageDescription for iOS 14
            rootDict.SetString("NSUserTrackingUsageDescription", "This identifier will be used to personalized your advertising experience.");

            #if APPSFLYER
		    rootDict.SetString("NSAdvertisingAttributionReportEndpoint", "https://appsflyer-skadnetwork.com");
            #elif ADJUST
		    rootDict.SetString("NSAdvertisingAttributionReportEndpoint", "https://adjust-skadnetwork.com");
            #elif IRONSOURCE
            rootDict.SetString("NSAdvertisingAttributionReportEndpoint", "https://postbacks-is.com");
            #endif

            // add IOS 14 Network Support
            var array = rootDict.CreateArray("SKAdNetworkItems");

            foreach (var skNetwork in SkNetworks)
            {
                var item = array.AddDict();
                item.SetString("SKAdNetworkIdentifier", $"{skNetwork}.skadnetwork"); //ironSource
            }

            // bypass Provide Export Compliance in Appstore Connect
            rootDict.SetBoolean("ITSAppUsesNonExemptEncryption", false);

            // set build version
            rootDict.SetString("CFBundleVersion", GameVersion.BuildNumber.ToString());

            rootDict.SetString("method", "debugging");

            // allow insecure http IOS
            #if ALLOW_INSECURE_HTTP_LOAD
            try
            {
                PlistElementDict dictNSAppTransportSecurity = (PlistElementDict)rootDict["NSAppTransportSecurity"];
                PlistElementDict dictNSExceptionDomains = dictNSAppTransportSecurity.CreateDict("NSExceptionDomains");
                PlistElementDict dictDomain = dictNSExceptionDomains.CreateDict("ip-api.com");
                dictDomain.SetBoolean("NSExceptionAllowsInsecureHTTPLoads", true);
            }
            catch (Exception e)
            {
                Debug.Log("Add allow insecure http IOS has exception. " + e);
            }
            #endif

            // URL Scheme
            var urlTypeArray   = rootDict.CreateArray("CFBundleURLTypes");
            var urlTypeSubDict = urlTypeArray.AddDict();
            var urlSchemeArray = urlTypeSubDict.CreateArray("CFBundleURLSchemes");
            urlTypeSubDict.SetString("CFBundleURLName", PlayerSettings.applicationIdentifier);
            urlSchemeArray.AddString(PlayerSettings.applicationIdentifier);

            // Write to file
            File.WriteAllText(plistPath, plist.WriteToString());
            Debug.Log($"onelog: IOSPostProcessingBuildTool End SetPlistConfig");
        }

        #endregion

        #region Embed Dynamic Libraries

        private static void EmbedDynamicLibrariesIfNeeded(string buildPath, PBXProject project, string targetGuid)
        {
            // Check that the Pods directory exists (it might not if a publisher is building with Generate Podfile setting disabled in EDM).
            Debug.Log("oneLog: EmbedDynamicLibrariesIfNeeded");
            var podsDirectory = Path.Combine(buildPath, "Pods");
            if (!Directory.Exists(podsDirectory)) return;
            var dynamicLibraryPathsPresentInProject = new List<string>();
            foreach (var dynamicLibraryToSearch in DynamicLibrariesToEmbed)
            {
                // both .framework and .xcframework are directories, not files
                var directories = Directory.GetDirectories(podsDirectory, dynamicLibraryToSearch, SearchOption.AllDirectories);
                if (directories.Length <= 0) continue;

                var dynamicLibraryAbsolutePath = directories[0];
                var index                      = dynamicLibraryAbsolutePath.LastIndexOf("Pods");
                var relativePath               = dynamicLibraryAbsolutePath.Substring(index);
                dynamicLibraryPathsPresentInProject.Add(relativePath);
            }

            if (dynamicLibraryPathsPresentInProject.Count <= 0) return;

            if (ShouldEmbedDynamicLibraries(buildPath))
            {
                foreach (var dynamicLibraryPath in dynamicLibraryPathsPresentInProject)
                {
                    var fileGuid = project.AddFile(dynamicLibraryPath, dynamicLibraryPath);
                    Debug.Log("oneLog: Add library for search path: " + dynamicLibraryPath);
                    project.AddFileToEmbedFrameworks(targetGuid, fileGuid);
                }
            }
        }

        private static bool ShouldEmbedDynamicLibraries(string buildPath)
        {
            var podfilePath = Path.Combine(buildPath, "Podfile");
            if (!File.Exists(podfilePath)) return false;

            // If the Podfile doesn't have a `Unity-iPhone` target, we should embed the dynamic libraries.
            var lines                     = File.ReadAllLines(podfilePath);
            var containsUnityIphoneTarget = lines.Any(line => line.Contains(TARGET_UNITY_IPHONE_PODFILE_LINE));
            if (!containsUnityIphoneTarget) return true;

            // If the Podfile does not have a `use_frameworks! :linkage => static` line, we should not embed the dynamic libraries.
            var useFrameworksStaticLineIndex = Array.FindIndex(lines, line => line.Contains(USE_FRAMEWORKS_STATIC_PODFILE_LINE));
            if (useFrameworksStaticLineIndex == -1) return false;

            // If more than one of the `use_frameworks!` lines are present, CocoaPods will use the last one.
            var useFrameworksLineIndex        = Array.FindIndex(lines, line => line.Trim() == USE_FRAMEWORKS_PODFILE_LINE); // Check for exact line to avoid matching `use_frameworks! :linkage => static/dynamic`
            var useFrameworksDynamicLineIndex = Array.FindIndex(lines, line => line.Contains(USE_FRAMEWORKS_DYNAMIC_PODFILE_LINE));

            // Check if `use_frameworks! :linkage => :static` is the last line of the three. If it is, we should embed the dynamic libraries.
            return useFrameworksLineIndex < useFrameworksStaticLineIndex && useFrameworksDynamicLineIndex < useFrameworksStaticLineIndex;
        }

        #endregion

        #region Set Project Config function

        private static void SetProjectConfig(PBXProject pbxProject, string mainTargetGuid, string testTargetGuid, string frameworkTargetGuid, string projectGuid)
        {
            // disable bitcode by default, reduce app size
            pbxProject.SetBuildProperty(mainTargetGuid, "ENABLE_BITCODE", "NO");
            pbxProject.SetBuildProperty(testTargetGuid, "ENABLE_BITCODE", "NO");
            pbxProject.SetBuildProperty(frameworkTargetGuid, "ENABLE_BITCODE", "NO");
            pbxProject.SetBuildProperty(projectGuid, "ENABLE_BITCODE", "NO");

            pbxProject.AddFrameworkToProject(mainTargetGuid, "iAd.framework", false);       // for Appsflyer tracking search ads
            pbxProject.AddFrameworkToProject(mainTargetGuid, "AdSupport.framework", false); // Add framework for (iron source mediation)

            pbxProject.AddBuildProperty(mainTargetGuid, "OTHER_LDFLAGS", "-lxml2"); // Add '-lxml2' of facebook to "Other Linker Flags"
            pbxProject.SetBuildProperty(mainTargetGuid, "ARCHS", "arm64");

            // Disable Unity Framework Target
            pbxProject.SetBuildProperty(mainTargetGuid, "ALWAYS_EMBED_SWIFT_STANDARD_LIBRARIES", "NO");
            pbxProject.SetBuildProperty(testTargetGuid, "ALWAYS_EMBED_SWIFT_STANDARD_LIBRARIES", "NO");
            pbxProject.SetBuildProperty(frameworkTargetGuid, "ALWAYS_EMBED_SWIFT_STANDARD_LIBRARIES", "NO");
            pbxProject.SetBuildProperty(projectGuid, "ALWAYS_EMBED_SWIFT_STANDARD_LIBRARIES", "NO");
        }

        private static void SetCapability(string pbxProjectPath, string mainTargetGuid)
        {
            var projectCapabilityManager = new ProjectCapabilityManager(pbxProjectPath, "Production.entitlements", null, mainTargetGuid);
            #if THEONE_SIGN_IN
            projectCapabilityManager.AddSignInWithApple();
            #endif
            #if THEONE_IAP
            projectCapabilityManager.AddInAppPurchase();
            #endif
            projectCapabilityManager.AddBackgroundModes(BackgroundModesOptions.RemoteNotifications);
            projectCapabilityManager.AddPushNotifications(false);
            projectCapabilityManager.WriteToFile();
            Debug.Log("onelog:  OnPostProcessBuild SetCapability");
        }

        #endregion
    }
}
#endif