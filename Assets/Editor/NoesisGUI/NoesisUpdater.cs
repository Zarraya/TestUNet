using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using System;
using System.Text;
using System.IO;
using System.Collections;
using System.Text.RegularExpressions;


[InitializeOnLoad]
public class NoesisUpdater: EditorWindow
{
    static NoesisUpdater()
    {
        EditorApplication.update += CheckVersion;
    }

    static void CheckVersion()
    {
        EditorApplication.update -= CheckVersion;

        if (UnityEditorInternal.InternalEditorUtility.inBatchMode || EditorApplication.isPlayingOrWillChangePlaymode)
        {
            return;
        }

        string localVersion = NoesisVersion.GetCached();
        string version = NoesisVersion.Get();

        if (localVersion != version && version != "0.0.0")
        {
            var window = (NoesisUpdater)ScriptableObject.CreateInstance(typeof(NoesisUpdater));
#if UNITY_4_6 || UNITY_5_0
            window.title = "NoesisGUI";
#else
            window.titleContent = new GUIContent("NoesisGUI");
#endif
            window.position = new Rect(Screen.currentResolution.width / 2 - 250, Screen.currentResolution.height / 2 - 22, 500, 55);
            window.minSize = new Vector2(500, 55);
            window.maxSize = new Vector2(500, 55);
            window.localVersion_ = localVersion;
            window.version_ = version;

            if (localVersion != "")
            {
                window.label_ = "Upgrading NoesisGUI " + localVersion + " -> " + version;
            }
            else
            {
                window.label_ = "Installing NoesisGUI " +  version;
            }

            window.ShowUtility();
        }
    }

    private string localVersion_;
    private string version_;
    private string label_;
    private string state_;
    private float progress_ = 0.0f;
    private IEnumerator updater_;

    void OnEnable()
    {
        updater_ = UpdateVersion();
    }

    void OnGUI()
    {
        GUI.Label(new Rect (5, 5, 420, 20), label_);
        EditorGUI.ProgressBar(new Rect(5, 25, 490, 20), progress_, state_);
    }

    void OnInspectorUpdate()
    {
        if (updater_.MoveNext())
        {
            Repaint();
        }
        else
        {
            Close();
        }
    }

    private IEnumerator UpdateVersion()
    {
        GoogleAnalyticsHelper.LogEvent("Install", version_, 0);
        progress_ = 0.05f;

        state_ = "Upgrading project";
        yield return null;
        Upgrade(localVersion_);
        progress_ = 0.10f;

        state_ = "Cleaning resources";
        yield return null;
        Noesis.BuildToolKernel.BuildBegin();
        Noesis.BuildToolKernel.Clean();
        progress_ = 0.20f;

        string[] activePlatforms = NoesisSettings.ActivePlatforms;
        foreach (var platform in activePlatforms)
        {
            state_ = "Regenerating " + platform + " resources";
            yield return null;

            using (var builder = new Noesis.BuildToolKernel(platform))
            {
                builder.BuildAll();
            }

            progress_ = 0.20f + 0.60f * (1 + ArrayUtility.IndexOf(activePlatforms, platform)) / activePlatforms.Length;
        }

        state_ = "Updating version";
        yield return null;
        NoesisVersion.SetCached(version_);
        progress_ = 0.85f;

        state_ = "Extracting documentation...\n";
        yield return null;
        ExtractDocumentation();
        progress_ = 0.99f;

        state_ = "Opening Welcome Window...\n";
        yield return null;
        EditorWindow.GetWindow(typeof(NoesisWelcome), true, "Welcome to NoesisGUI!");
        progress_ = 1.0f;

        Debug.Log("NoesisGUI v" + version_ + " successfully installed");
    }

    private static string NormalizeVersion(string version)
    {
        string pattern = @"^(\d+).(\d+).(\d+)((a|b|rc|f)(\d*))?$";
        var match = Regex.Match(version.ToLower(), pattern);

        string normalized = "";

        if (match.Success)
        {
            normalized += match.Groups[1].Value.PadLeft(2, '0');
            normalized += ".";
            normalized += match.Groups[2].Value.PadLeft(2, '0');
            normalized += ".";
            normalized += match.Groups[3].Value.PadLeft(2, '0');

            if (match.Groups[4].Length > 0)
            {
                if (match.Groups[5].Value == "a")
                {
                    normalized += ".0.";
                }
                else if (match.Groups[5].Value == "b")
                {
                    normalized += ".1.";
                }
                else if (match.Groups[5].Value == "rc")
                {
                    normalized += ".2.";
                }
                else if (match.Groups[5].Value == "f")
                {
                    normalized += ".3.";
                }

                normalized += match.Groups[6].Value.PadLeft(2, '0');
            }
            else
            {
                normalized += ".3.00";
            }
        }
        else
        {
            Debug.LogError("Unexpected version format " + version);
        }

        return normalized;
    }

    private static bool PatchNeeded(string from, string to)
    {
        if (from.Length == 0)
        {
            return false;
        }
        else
        {
            return String.Compare(NormalizeVersion(from), NormalizeVersion(to)) < 0;
        }
    }

    private static void Upgrade(string version)
    {
        if (PatchNeeded(version, "1.1.9"))
        {
            Upgrade_1_1_9();
        }

        if (PatchNeeded(version, "1.1.12"))
        {
            Upgrade_1_1_12();
        }

        if (PatchNeeded(version, "1.2.0b1"))
        {
            Upgrade_1_2_0b1();
        }

        if (PatchNeeded(version, "1.2.0b6"))
        {
            Upgrade_1_2_0b6();
        }

        if (PatchNeeded(version, "1.2.0b7"))
        {
            Upgrade_1_2_0b7();
        }

        if (PatchNeeded(version, "1.2.0b8"))
        {
            Upgrade_1_2_0b8();
        }

        if (PatchNeeded(version, "1.2.2"))
        {
            Upgrade_1_2_2();
        }

        if (PatchNeeded(version, "1.2.4"))
        {
            Upgrade_1_2_4();
        }

        if (PatchNeeded(version, "1.2.5f2"))
        {
            Upgrade_1_2_5f2();
        }

        if (PatchNeeded(version, "1.2.6f1"))
        {
            Upgrade_1_2_6f1();
        }

        FixPluginImporterSettings();
    }

    private static void Upgrade_1_1_9()
    {
        AssetDatabase.DeleteAsset("Assets/Plugins/x86/UnityRenderHook.dll");
        AssetDatabase.DeleteAsset("Assets/Plugins/x86_64/UnityRenderHook.dll");
        AssetDatabase.DeleteAsset("Assets/Plugins/x86/libUnityRenderHook.so");
        AssetDatabase.DeleteAsset("Assets/Plugins/UnityRenderHook.bundle");
    }

    private static void Upgrade_1_1_12()
    {
        EditorPrefs.DeleteKey("NoesisReviewStatus");
        EditorPrefs.DeleteKey("NoesisReviewDate");
    }

    private static void Upgrade_1_2_0b1()
    {
        AssetDatabase.DeleteAsset("Assets/meta.ANDROID.cache");
        AssetDatabase.DeleteAsset("Assets/meta.GL.cache");
        AssetDatabase.DeleteAsset("Assets/meta.IOS.cache");
        AssetDatabase.DeleteAsset("Assets/meta.DX9.cache");
        AssetDatabase.DeleteAsset("Assets/NoesisGUI.build.ANDROID.log");
        AssetDatabase.DeleteAsset("Assets/NoesisGUI.build.GL.log");
        AssetDatabase.DeleteAsset("Assets/NoesisGUI.build.IOS.log");
        AssetDatabase.DeleteAsset("Assets/NoesisGUI.build.DX9.log");
        AssetDatabase.DeleteAsset("Assets/NoesisGUI.play.log");
        AssetDatabase.DeleteAsset("Assets/Editor/NoesisGUI/Build_.cs");

        string[] makes = Directory.GetFiles(UnityEngine.Application.dataPath, "*.make", SearchOption.AllDirectories);
        foreach (string make in makes)
        {
            string asset = ("Assets" + make.Substring(UnityEngine.Application.dataPath.Length)).Replace('\\', '/');
            AssetDatabase.DeleteAsset(asset);
        }

        string[] fonts = Directory.GetFiles(UnityEngine.Application.dataPath, "*.font", SearchOption.AllDirectories);
        foreach (string font in fonts)
        {
            string asset = ("Assets" + font.Substring(UnityEngine.Application.dataPath.Length)).Replace('\\', '/');
            AssetDatabase.DeleteAsset(asset);
        }

        EditorPrefs.DeleteKey("NoesisDelayedBuildDoScan");
        EditorPrefs.DeleteKey("NoesisDelayedBuildDoBuild");
    }

    private static void Upgrade_1_2_0b6()
    {
        AssetDatabase.DeleteAsset("Assets/NoesisGUI/Docs/Images.zip");
        AssetDatabase.DeleteAsset("Assets/NoesisGUI/Docs/Integration.zip");
        AssetDatabase.DeleteAsset("Assets/NoesisGUI/Docs/Shapes.zip");
        AssetDatabase.DeleteAsset("Assets/NoesisGUI/Docs/Text.zip");
    }

    private static void Upgrade_1_2_0b7()
    {
        AssetDatabase.DeleteAsset("Assets/NoesisGUI/Samples/ControlGallery/Samples/Images/blog_compose.png");
        AssetDatabase.DeleteAsset("Assets/NoesisGUI/Samples/ControlGallery/Samples/Images/button_blue_pause.png");
        AssetDatabase.DeleteAsset("Assets/NoesisGUI/Samples/ControlGallery/Samples/Images/button_blue_play.png");
        AssetDatabase.DeleteAsset("Assets/NoesisGUI/Samples/ControlGallery/Samples/Images/calculator.png");
        AssetDatabase.DeleteAsset("Assets/NoesisGUI/Samples/ControlGallery/Samples/Images/calendar.png");
        AssetDatabase.DeleteAsset("Assets/NoesisGUI/Samples/ControlGallery/Samples/Images/camera.png");
        AssetDatabase.DeleteAsset("Assets/NoesisGUI/Samples/ControlGallery/Samples/Images/folder_open.png");
        AssetDatabase.DeleteAsset("Assets/NoesisGUI/Samples/ControlGallery/Samples/Images/notepad.png");
    }

    private static void Upgrade_1_2_0b8()
    {
        AssetDatabase.DeleteAsset("Assets/NoesisGUI/Samples/ControlGallery/Samples/Images/space.jpg");
    }

    private static void Upgrade_1_2_2()
    {
        AssetDatabase.DeleteAsset("Assets/Editor/NoesisGUI/BuildTool/tbb.dll");
        AssetDatabase.DeleteAsset("Assets/Plugins/x86/tbb.dll");
    }

    private static void Upgrade_1_2_4()
    {
        AssetDatabase.DeleteAsset("Assets/NoesisGUI/Docs/Touch.zip");
    }

    private static void Upgrade_1_2_5f2()
    {
        AssetDatabase.DeleteAsset("Assets/NoesisGUI/Docs");
        AssetDatabase.DeleteAsset("Assets/NoesisGUI/index.html");
        AssetDatabase.DeleteAsset("Assets/Plugins/Metro/Win81/arm/NoesisUnityRenderHook.dll");
        AssetDatabase.DeleteAsset("Assets/Plugins/Metro/Win81/x86/NoesisUnityRenderHook.dll");
        AssetDatabase.DeleteAsset("Assets/Plugins/Metro/WindowsPhone81/arm/NoesisUnityRenderHook.dll");
        AssetDatabase.DeleteAsset("Assets/Plugins/Metro/WindowsPhone81/x86/NoesisUnityRenderHook.dll");
        AssetDatabase.DeleteAsset("Assets/Plugins/x86/libNoesisUnityRenderHook.so");
        AssetDatabase.DeleteAsset("Assets/Plugins/x86_64/libNoesisUnityRenderHook.so");

        // Unity is still quite buggy about this. We need to manually patch previous settings
        var importer = AssetImporter.GetAtPath("Assets/Plugins/NoesisUnityRenderHook.bundle") as PluginImporter;
        if (importer != null)
        {
            importer.SetCompatibleWithPlatform(BuildTarget.StandaloneOSXUniversal, false);
            importer.SetCompatibleWithPlatform(BuildTarget.StandaloneOSXIntel, false);
            importer.SetCompatibleWithPlatform(BuildTarget.StandaloneOSXIntel64, false);
            importer.SaveAndReimport();
        }
    }

    private static void Upgrade_1_2_6f1()
    {
        EnsureFolder("Assets/Plugins/Android/libs");
        EnsureFolder("Assets/Plugins/Android/libs/armeabi-v7a");
        EnsureFolder("Assets/Plugins/Android/libs/x86");

        AssetDatabase.MoveAsset(
            "Assets/Plugins/Android/libNoesis.so",
            "Assets/Plugins/Android/libs/armeabi-v7a/libNoesis.so");
    }

    private static void FixPluginImporterSettings()
    {
#if UNITY_5_0
        // http://issuetracker.unity3d.com/issues/plugin-importer-settings-do-not-persist-when-exporting-a-unitypackage
        // Fixed in Unity 5.1.0
        PluginImporter importer;

        importer = AssetImporter.GetAtPath("Assets/Plugins/Noesis.bundle") as PluginImporter;
        if (importer != null)
        {
            importer.SetCompatibleWithAnyPlatform(false);
            importer.SetCompatibleWithEditor(false);
            importer.SetCompatibleWithPlatform(BuildTarget.StandaloneOSXUniversal, true);
            importer.SetCompatibleWithPlatform(BuildTarget.StandaloneOSXIntel, true);
            importer.SetCompatibleWithPlatform(BuildTarget.StandaloneOSXIntel64, true);
            importer.SaveAndReimport();
        }

        importer = AssetImporter.GetAtPath("Assets/Plugins/NoesisUnityRenderHook.bundle") as PluginImporter;
        if (importer != null)
        {
            importer.SetCompatibleWithAnyPlatform(false);
            importer.SetCompatibleWithEditor(true);
            importer.SetCompatibleWithPlatform(BuildTarget.StandaloneOSXUniversal, false);
            importer.SetCompatibleWithPlatform(BuildTarget.StandaloneOSXIntel, false);
            importer.SetCompatibleWithPlatform(BuildTarget.StandaloneOSXIntel64, false);
            importer.SaveAndReimport();
        }

        importer = AssetImporter.GetAtPath("Assets/Editor/NoesisGUI/BuildTool/Noesis.bundle") as PluginImporter;
        if (importer != null)
        {
            importer.SetCompatibleWithAnyPlatform(false);
            importer.SetCompatibleWithEditor(true);
            importer.SaveAndReimport();
        }
#endif
    }

    private static void EnsureFolder(string path)
    {
        if (!AssetDatabase.IsValidFolder(path))
        {
            string parentFolder = System.IO.Path.GetDirectoryName(path);
            string newFolder = System.IO.Path.GetFileName(path);

            AssetDatabase.CreateFolder(parentFolder, newFolder);
        }
    }

    private static string TarLocation = "NoesisGUI/Doc.tar";

    private static void ExtractDocumentation()
    {
        string tarPath = Path.Combine(Application.dataPath, TarLocation);

        if (File.Exists(tarPath))
        {
            string destPath = Application.dataPath + "/../NoesisDoc";
            byte[] buffer = new byte[512];

            try
            {
                if (Directory.Exists(destPath))
                {
                    Directory.Delete(destPath, true);
                }
            }
            catch (Exception) { }

            using (var tar = File.OpenRead(tarPath))
            {
                while (tar.Read(buffer, 0, 512) > 0)
                {
                    string filename = Encoding.ASCII.GetString(buffer, 0, 100).Trim((char)0);

                    if (!String.IsNullOrEmpty(filename))
                    {
                        long size = Convert.ToInt64(Encoding.ASCII.GetString(buffer, 124, 11).Trim(), 8);

                        if (size > 0)
                        {
                            string path = destPath + "/" + filename;
                            Directory.CreateDirectory(Path.GetDirectoryName(path));

                            using (var file = File.Create(path))
                            {
                                long blocks = (size + 511) / 512;
                                for (int i = 0; i < blocks; i++)
                                {
                                    tar.Read(buffer, 0, 512);
                                    file.Write(buffer, 0, (Int32)Math.Min(size, 512));
                                    size -= 512;
                                }
                            }
                        }
                    }
                }
            }

            AssetDatabase.DeleteAsset(Path.Combine("Assets", TarLocation));
        }
    }
}
