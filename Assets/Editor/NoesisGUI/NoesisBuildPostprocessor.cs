using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using System.IO;


public class NoesisBuildPostprocessor
{
    [PostProcessBuild]
    public static void OnPostprocessBuild(BuildTarget target, string pathToBuiltProject)
    {
        if (target == BuildTarget.WSAPlayer)
        {
#if UNITY_4_6 || UNITY_5_0 || UNITY_5_1
            OnPostprocessBuildWSA(pathToBuiltProject);
#endif
        }
    }

    private static void OnPostprocessBuildWSA(string pathToBuiltProject)
    {
        string exportedPath = Path.Combine(pathToBuiltProject, PlayerSettings.productName);
        string[] filesToSearch = new[]
        {
            "App.xaml.cs",
            "MainPage.xaml.cs", 
            Path.Combine(PlayerSettings.productName + ".Shared", "App.xaml.cs"), 
            Path.Combine(PlayerSettings.productName + ".Shared", "MainPage.xaml.cs")
        };

        bool patched = false;
        for (int i = 0; i < filesToSearch.Length; i++)
        {
            string path = Path.Combine(exportedPath, filesToSearch[i]);
            if (PatchFile(path, "appCallbacks.SetBridge(_bridge);", "appCallbacks.SetBridge(_bridge);\n\t\t\tappCallbacks.LoadGfxNativePlugin(\"Noesis.dll\");"))
            {
                patched = true;
                break;
            }
        }

        if (!patched)
        {
            Debug.LogError("NoesisGUI: Failed to patch file with LoadGfxNativePlugin()");
        }
    }

    private static bool PatchFile(string fileName, string targetString, string replacement)
    {
        if (File.Exists(fileName) == false)
        {
            return false;
        }

        string text = File.ReadAllText(fileName);

        if (text.IndexOf(targetString) == -1)
        {
            return false;
        }

        // Already patched ?
        if (text.IndexOf(replacement) != -1)
        {
            return true;
        }

        text = text.Replace(targetString, replacement);

        File.WriteAllText(fileName, text);

        return true;
    }
}
