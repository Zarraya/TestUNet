using UnityEditor;
using UnityEngine;
using Noesis;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Globalization;
using System.Reflection;


public class NoesisSettings : EditorWindow
{
    public static string[] ActivePlatforms
    {
        get
        {
            var platforms = new List<string>();
            int mask = ReadSettings();

            if ((mask & DX9Platform) > 0)
            {
                platforms.Add("dx9");
            }

            if ((mask & DX11Platform) > 0)
            {
                platforms.Add("dx11");
            }

            if ((mask & GLPlatform) > 0)
            {
                platforms.Add("gl");
            }

            if ((mask & IOSPlatform) > 0)
            {
                platforms.Add("ios");
            }

            if ((mask & AndroidPlatform) > 0)
            {
                platforms.Add("android");
            }

            return platforms.ToArray();
        }
    }

    private static int GetNumAssets()
    {
        int numAssets = BuildToolKernel.ReadAssets().Count;
        if (numAssets == 0)
        {
            // approximate with number of xamls files
            numAssets = Directory.GetFiles(
                UnityEngine.Application.dataPath, "*.xaml", SearchOption.AllDirectories).Length;
        }

        return numAssets;
    }

    public static void RebuildActivePlatforms(string progressTitle)
    {
        try
        {
            string[] activePlatforms = ActivePlatforms;
            string platform = "";
            float platformProgress = 0.0f;
            float platformDelta = 1.0f / (GetNumAssets() * (activePlatforms.Length + 1));
            float progress = 0.0f;
            float deltaProgress = 1.0f / (activePlatforms.Length + 1);

            EditorUtility.DisplayProgressBar(progressTitle, "Cleaning...", 0.0f);
            BuildToolKernel.BuildBegin();
            BuildToolKernel.Clean();

            Action<String> action = s =>
            {
                EditorUtility.DisplayProgressBar(progressTitle,
                    "[" + platform + "] " + s + "...", progress + platformProgress);
                platformProgress = Math.Min(platformProgress + platformDelta, deltaProgress);
            };
            BuildToolKernel.BuildEvent += action;

            foreach (string activePlatform in activePlatforms)
            {
                platformProgress = 0.0f;
                progress += deltaProgress;
                platform = activePlatform;
                Build(platform);
            }

            BuildToolKernel.BuildEvent -= action;
            EditorUtility.DisplayProgressBar(progressTitle, "Done.", 1.0f);
        }
        finally
        {
            EditorUtility.ClearProgressBar();
        }
    }

    public static void ClearLog()
    {
        Assembly assembly = Assembly.GetAssembly(typeof(SceneView));
        System.Type type = assembly.GetType("UnityEditorInternal.LogEntries");
        MethodInfo method = type.GetMethod("Clear");
        method.Invoke(new object(), null);
    }

    private static int DX9Platform = 1;
    private static int GLPlatform = 2;
    private static int IOSPlatform = 4;
    private static int AndroidPlatform = 8;
    private static int DX11Platform = 16;

    private static string DX9Field = "NoesisDX9";
    private static string DX11Field = "NoesisDX11";
    private static string GLField = "NoesisGL";
    private static string AndroidField = "NoesisAndroid";
    private static string IOSField = "NoesisIOS";
    
    private static int ReadSettings()
    {
        int activePlatforms = 0;

        bool isWindows = UnityEngine.Application.platform == UnityEngine.RuntimePlatform.WindowsEditor;
        bool isOSX = !isWindows;

        if (PlayerPrefs.GetInt(DX9Field, 0) > 0)
        {
            activePlatforms |= DX9Platform;
        }

        if (PlayerPrefs.GetInt(DX11Field, isWindows ? 1 : 0) > 0)
        {
            activePlatforms |= DX11Platform;
        }

        if (PlayerPrefs.GetInt(GLField, isOSX ? 1 : 0) > 0)
        {
            activePlatforms |= GLPlatform;
        }

        if (PlayerPrefs.GetInt(IOSField, 0) > 0)
        {
            activePlatforms |= IOSPlatform;
        }

        if (PlayerPrefs.GetInt(AndroidField, 0) > 0)
        {
            activePlatforms |= AndroidPlatform;
        }

        return activePlatforms;
    }

    private static void WriteSettings(int activePlatforms)
    {
        PlayerPrefs.SetInt(DX9Field, (activePlatforms & DX9Platform) > 0 ? 1 : 0);
        PlayerPrefs.SetInt(DX11Field, (activePlatforms & DX11Platform) > 0 ? 1 : 0);
        PlayerPrefs.SetInt(GLField, (activePlatforms & GLPlatform) > 0 ? 1 : 0);
        PlayerPrefs.SetInt(IOSField, (activePlatforms & IOSPlatform) > 0 ? 1 : 0);
        PlayerPrefs.SetInt(AndroidField, (activePlatforms & AndroidPlatform) > 0 ? 1 : 0);
        PlayerPrefs.Save();

        using (StreamWriter file = new StreamWriter(UnityEngine.Application.dataPath + "/Editor/NoesisGUI/Platform_.cs"))
        {
            file.WriteLine("// This file is automatically generated");
            file.WriteLine();

            if ((activePlatforms & DX9Platform) == 0 && (activePlatforms & DX11Platform) == 0)
            {
                file.WriteLine("#if UNITY_STANDALONE_WIN");
                file.WriteLine("#    error DirectX is not active! Please, activate it in Tools -> NoesisGUI -> Settings");
                file.WriteLine("#endif");
            }

            if ((activePlatforms & GLPlatform) == 0)
            {
                file.WriteLine("#if UNITY_STANDALONE_OSX");
                file.WriteLine("#    error GL is not active! Please, activate it in Tools -> NoesisGUI -> Settings");
                file.WriteLine("#endif");
            }

            if ((activePlatforms & IOSPlatform) == 0)
            {
                file.WriteLine("#if UNITY_IPHONE");
                file.WriteLine("#    error iOS is not active! Please, activate it in Tools -> NoesisGUI -> Settings");
                file.WriteLine("#endif");
            }

            if ((activePlatforms & AndroidPlatform) == 0)
            {
                file.WriteLine("#if UNITY_ANDROID");
                file.WriteLine("#    error Android is not active! Please, activate it in Tools -> NoesisGUI -> Settings");
                file.WriteLine("#endif");
            }
        }

        AssetDatabase.ImportAsset("Assets/Editor/NoesisGUI/Platform_.cs");
    }

    private int activePlatforms_;

    [UnityEditor.MenuItem("Tools/NoesisGUI/About NoesisGUI...", false, 30000)]
    static void OpenAbout()
    {
        GetWindow(typeof(NoesisAbout), true, "About NoesisGUI");
    }

    [UnityEditor.MenuItem("Tools/NoesisGUI/Settings...", false, 30050)]
    static void OpenSettings()
    {
        GetWindow(typeof(NoesisSettings), false, "NoesisGUI");
    }

    [UnityEditor.MenuItem("Tools/NoesisGUI/Welcome Screen...", false, 30100)]
    static void OpenWelcome()
    {
        GetWindow(typeof(NoesisWelcome), true, "Welcome to NoesisGUI!");
    }

    [UnityEditor.MenuItem("Tools/NoesisGUI/Video Tutorials", false, 30102)]
    static void OpenVideoTutorial()
    {
        UnityEngine.Application.OpenURL("https://vimeo.com/groups/264371");
    }

    [UnityEditor.MenuItem("Tools/NoesisGUI/Documentation", false, 30103)]
    static void OpenDocumentation()
    {
        string docPath = Application.dataPath + "/../NoesisDoc/index.html";

        if (File.Exists(docPath))
        {
            UnityEngine.Application.OpenURL("file://" + docPath);
        }
        else
        {
            UnityEngine.Application.OpenURL("http://www.noesisengine.com/docs");
        }
    }

    [UnityEditor.MenuItem("Tools/NoesisGUI/Forums", false, 30104)]
    static void OpenForum()
    {
        UnityEngine.Application.OpenURL("http://forums.noesisengine.com/");
    }

    [UnityEditor.MenuItem("Tools/NoesisGUI/Review...", false, 30105)]
    static void OpenReview()
    {
        EditorWindow.GetWindow(typeof(NoesisReview), true, "Support our development");
    }

    [UnityEditor.MenuItem("Tools/NoesisGUI/Release Notes", false, 30150)]
    static public void OpenReleaseNotes()
    {
        string docPath = Application.dataPath + "/../NoesisDoc/Doc/Gui.Core.Changelog.html";

        if (File.Exists(docPath))
        {
            UnityEngine.Application.OpenURL("file://" + docPath);
        }
        else
        {
            UnityEngine.Application.OpenURL("http://www.noesisengine.com/docs/Gui.Core.Changelog.html");
        }
    }

    [UnityEditor.MenuItem("Tools/NoesisGUI/Report a bug", false, 30151)]
    static void OpenReportBug()
    {
        UnityEngine.Application.OpenURL("http://bugs.noesisengine.com/");
    }

    void Awake()
    {
        activePlatforms_ = ReadSettings();
    }
        
    void OnGUI()
    {
        bool isCompiling = EditorApplication.isCompiling;
        GUI.enabled = !isCompiling;

        GUIStyle titleStyle = new GUIStyle(EditorStyles.whiteLabel);
        titleStyle.alignment = TextAnchor.MiddleCenter;
        titleStyle.fontStyle = UnityEngine.FontStyle.Bold;
        titleStyle.fontSize = 12;
        GUILayout.Label("NoesisGUI Settings", titleStyle);
        GUILayout.Label("Build Platforms", EditorStyles.boldLabel);

        int activePlatforms = 0;
        int numActivePlatforms = 0;

        GUILayout.BeginHorizontal();

        bool isWindows = UnityEngine.Application.platform == UnityEngine.RuntimePlatform.WindowsEditor;
        if (!isWindows)
        {
            GUI.enabled = false;
        }

        if (GUILayout.Toggle((activePlatforms_ & DX9Platform) > 0, "DX9"))
        {
            activePlatforms |= DX9Platform;
            numActivePlatforms++;
        }
        if (GUILayout.Toggle((activePlatforms_ & DX11Platform) > 0, "DX11"))
        {
            activePlatforms |= DX11Platform;
            numActivePlatforms++;
        }

        GUI.enabled = !isCompiling;
        if (GUILayout.Toggle((activePlatforms_ & GLPlatform) > 0, "GL"))
        {
            activePlatforms |= GLPlatform;
            numActivePlatforms++;
        }

        if (GUILayout.Toggle((activePlatforms_ & IOSPlatform) > 0, "iOS"))
        {
            activePlatforms |= IOSPlatform;
            numActivePlatforms++;
        }

        if (GUILayout.Toggle((activePlatforms_ & AndroidPlatform) > 0, "Android"))
        {
            activePlatforms |= AndroidPlatform;
            numActivePlatforms++;
        }
        GUILayout.EndHorizontal();

        int delta = activePlatforms ^ activePlatforms_;
        if (delta > 0)
        {
            activePlatforms_ = activePlatforms;
            WriteSettings(activePlatforms_);
        }

        GUILayout.Space(15.0f);
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        
        if (GUILayout.Button(isCompiling ? "Unity is compiling scripts..." : "Rebuild", GUILayout.MinWidth(100)))
        {
            RebuildActivePlatforms("Building Resources");
        }

        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
        GUI.enabled = true;
    }
   
    private static void Build(string platform)
    {
        using (BuildToolKernel builder = new BuildToolKernel(platform))
        {
            builder.BuildAll();
        }
    }
}
