using System;
using System.IO;
using UnityEngine;
using Noesis;


public class NoesisVersion
{
    private static string Version = "1.2.6f6";
    private static string VersionFilename = Application.dataPath + "/Editor/NoesisGUI/version.txt";

    public static string GetCached()
    {
        string version = null;

        try
        {
            if (File.Exists(VersionFilename))
            {
                using (var reader = new StreamReader(VersionFilename))
                {
                    version = reader.ReadLine();
                }
            }
        }
        catch (Exception) { }

        // If there is no version file it must be a clean new version or an old version (<=1.1.8)
        if (String.IsNullOrEmpty(version))
        {
            if (File.Exists(Application.dataPath + "/Plugins/x86/libUnityRenderHook.so"))
            {
                version = "1.1.8";
            }
            else
            {
                version = "";
            }
        }

        return version;
    }

    public static void SetCached(string version)
    {
        try
        {
            using (var writer = new StreamWriter(VersionFilename))
            {
                writer.WriteLine(version);
            }
        }
        catch (Exception) { }
    }

    public static string Get()
    {
        return Version;
    }

    public static bool IsTrial()
    {
        try
        {
            using (var library = new Library(UnityEngine.Application.dataPath + "/Editor/NoesisGUI/BuildTool/Noesis"))
            {
                var isTrial = library.Find<Noesis_IsTrialDelegate>("Noesis_IsTrial");
                return isTrial();
            }
        }
        catch
        {
            return false;
        }
    }

    private delegate bool Noesis_IsTrialDelegate();
}