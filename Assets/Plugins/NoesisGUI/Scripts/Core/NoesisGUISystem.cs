using UnityEngine;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

////////////////////////////////////////////////////////////////////////////////////////////////
/// NoesisGUI main system
////////////////////////////////////////////////////////////////////////////////////////////////
public class NoesisGUISystem : MonoBehaviour
{
    #region Public properties

    /// <summary>
    /// Returns NoesisGUI System Instance.
    /// </summary>
    public static NoesisGUISystem Instance { get { return _instance; } }

    /// <summary>
    /// Indicates if NoesisGUI System is initialized.
    /// </summary>
    public static bool IsInitialized { get { return _isInitialized; } }

    /// <summary>
    /// Sets the software keyboard manager used when virtual keyboard is shown.
    /// </summary>
    public static Noesis.SoftwareKeyboardManager SoftwareKeyboardManager
    {
        get { return _isInitialized ? _instance._softwareKeyboardManager : null; }
        set
        {
            if (value == null) { throw new ArgumentNullException(); }
            if (_isInitialized) { _instance._softwareKeyboardManager = value; }
        }
    }

    #endregion

    #region Public methods

    /// <summary>
    /// Loads a XAML resource.
    /// </summary>
    /// <param name="xamlFile">Path to the resource.</param>
    /// <returns>Root of the loaded XAML.</returns>
    public static object LoadXaml(string xamlFile)
    {
        return _isInitialized ? _instance.Load(xamlFile) : null;
    }

    /// <summary>
    /// Tries to load a XAML resource.
    /// </summary>
    /// <param name="xamlFile">Path to the resource.</param>
    /// <returns>Root of the loaded XAML. Returns null if resource cannot be found.</returns>
    public static object TryLoadXaml(string xamlFile)
    {
        return _isInitialized ? _instance.TryLoad(xamlFile) : null;
    }

    /// <summary>
    /// Loads a font with the specified settings. It should be used for text measuring purposes.
    /// </summary>
    /// <param name="family">Family name object.</param>
    /// <param name="style">Italic style.</param>
    /// <param name="weight">Bold style.</param>
    /// <param name="size">Font size in pixels.</param>
    /// <param name="strokeThickness">Stroke thickness in pixels.</param>
    /// <returns>Font object.</returns>
    public static Noesis.Font LoadFont(Noesis.FontFamily family, Noesis.FontStyle style,
        Noesis.FontWeight weight, float size, float strokeThickness)
    {
        return new Noesis.Font(family, style, weight, size, strokeThickness);
    }

    #endregion

    #region Private members

    #region MonoBehavior component messages

    ////////////////////////////////////////////////////////////////////////////////////////////////
    void Awake()
    {
        Init();
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////
    void OnDisable()
    {
        if (_isInitialized)
        {
            Shutdown();

            Destroy(gameObject);

            _instance = null;
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////
    void Update()
    {
        if (_isInitialized)
        {
            Noesis_Tick();

            _softwareKeyboardManager.UpdateText();
            Noesis.Extend.RemoveDestroyedExtends();
        }
    }

    #endregion

    #region System initialization and shut down

    ////////////////////////////////////////////////////////////////////////////////////////////////
    private static NoesisGUISystem _instance = null;
    private static bool _isCreated = false;
    private static bool _isInitialized = false;

    ////////////////////////////////////////////////////////////////////////////////////////////////
    internal static void Create()
    {
        if (!_isCreated)
        {
            GameObject go = new GameObject("NoesisGUISystem");
            _isCreated = true;

            go.AddComponent<NoesisGUISystem>();
            _instance = go.GetComponent<NoesisGUISystem>();
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////
    private void Init()
    {
#if UNITY_EDITOR
        if (PlayerPrefs.HasKey("NoesisErrors"))
        {
            Debug.LogWarning("<color=green>NoesisGUI:</color> Please, fix all XAML errors before hitting PLAY");
        }
#endif
        UnityInitDevice();

#if UNITY_EDITOR
        try
        {
            _library = new Noesis.Library(UnityEngine.Application.dataPath +
                "/Editor/NoesisGUI/BuildTool/Noesis");

            RegisterFunctions(_library);
            Noesis.Error.RegisterFunctions(_library);
            Noesis.Log.RegisterFunctions(_library);
            Noesis.Extend.RegisterFunctions(_library);
            Noesis.UIRenderer.RegisterFunctions(_library);
            Noesis.NoesisGUI_PINVOKE.RegisterFunctions(_library);
#endif
            DoInit();

            _isInitialized = true;
#if UNITY_EDITOR
        }
        catch (System.Exception e)
        {
            Shutdown();
            throw e;
        }
#endif

        // To avoid that this GameObject is destroyed when a new scene is loaded
        DontDestroyOnLoad(gameObject);
    }

    private delegate void UnityLogCallback([MarshalAs(UnmanagedType.LPWStr)]string message, bool isError);
    static private UnityLogCallback _unityLog = UnityLog;

    [MonoPInvokeCallback(typeof(UnityLogCallback))]
    private static void UnityLog(string message, bool isError)
    {
        if (isError)
        {
            Debug.LogError("NoesisError: " + message);
        }
        else
        {
#if UNITY_5_4_OR_NEWER
            var logType = Application.GetStackTraceLogType(LogType.Log);
            Application.SetStackTraceLogType(LogType.Log, StackTraceLogType.None);
#elif UNITY_5_3_OR_NEWER
            var logType = Application.stackTraceLogType;
            Application.stackTraceLogType = StackTraceLogType.None;
#endif
            Debug.Log(message);

#if UNITY_5_4_OR_NEWER
            Application.SetStackTraceLogType(LogType.Log, logType);
#elif UNITY_5_3_OR_NEWER
            Application.stackTraceLogType = logType;
#endif
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////
    private void DoInit()
    {
#if !UNITY_EDITOR
        Noesis.NoesisGUI_PINVOKE.RuntimeInit();
#endif
        Noesis_RegisterSoftwareKeyboardCallbacks(_showSoftwareKeyboard, _hideSoftwareKeyboard);
        Noesis.TextureSource.RegisterCallbacks();
        Noesis.Extend.RegisterCallbacks();

        int deviceType = Noesis_Init(Application.streamingAssetsPath,
            Application.dataPath + "/Plugins", Application.isEditor, _unityLog);

        Noesis.Extend.Initialized = true;

        Noesis.UIRenderer.SetDeviceType(deviceType);
        GL.InvalidateState();

        Noesis.Log.Info(String.Format("Host is Unity v{0}", UnityEngine.Application.unityVersion));

        Noesis.Extend.RegisterNativeTypes();

        _softwareKeyboardManager = new Noesis.SoftwareKeyboardManager();
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////
    private void Shutdown()
    {
        // Destroy UIRenderer of each GUI panel
        int numPanels = _panels.Count;
        for (int i = numPanels - 1; i >= 0; --i)
        {
            _panels[i].DestroyRenderer();
        }

        _isInitialized = false;
        _softwareKeyboardManager = null;

        Noesis.Extend.RemoveDestroyedExtends();
        Noesis.Extend.ResetDependencyProperties();

        GC.Collect();
        GC.WaitForPendingFinalizers();

        Noesis.Extend.Initialized = false;

#if UNITY_EDITOR
        try
        {
#endif
            Noesis_Shutdown();

#if UNITY_EDITOR
        }
        catch (System.Exception e)
        {
            Debug.LogError(e.Message);
        }
        finally
        {
#endif
            Noesis.Extend.UnregisterCallbacks();

#if UNITY_EDITOR
            UnregisterFunctions();
            Noesis.Error.UnregisterFunctions();
            Noesis.Log.UnregisterFunctions();
            Noesis.Extend.UnregisterFunctions();
            Noesis.UIRenderer.UnregisterFunctions();
            Noesis.NoesisGUI_PINVOKE.UnregisterFunctions();

            DisposeLibrary();
        }
#else
        Noesis.NoesisGUI_PINVOKE.RuntimeShutdown();
#endif
    }

    #endregion

    #region Xaml loading

    ////////////////////////////////////////////////////////////////////////////////////////////////
    private object Load(string xamlFile)
    {
        return Noesis.Extend.GetProxy(Noesis_LoadXAML(xamlFile), true);
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////
    private object TryLoad(string xamlFile)
    {
        return Noesis.Extend.GetProxy(Noesis_TryLoadXAML(xamlFile), true);
    }

    #endregion

    #region NoesisGUI Panel component registry

    ////////////////////////////////////////////////////////////////////////////////////////////////
    internal static void AddPanel(NoesisGUIPanel panel)
    {
        if (_isInitialized)
        {
            _instance._panels.Add(panel);
        }
    }

    internal static void RemovePanel(NoesisGUIPanel panel)
    {
        if (_isInitialized)
        {
            _instance._panels.Remove(panel);
        }
    }

    private List<NoesisGUIPanel> _panels = new List<NoesisGUIPanel>();

    #endregion

    #region Virtual keyboard management

    ////////////////////////////////////////////////////////////////////////////////////////////////
    private delegate void InternalShowSoftwareKeyboardCallback(IntPtr focusedElement);
    static private InternalShowSoftwareKeyboardCallback _showSoftwareKeyboard = ShowSoftwareKeyboard;

    [MonoPInvokeCallback(typeof(InternalShowSoftwareKeyboardCallback))]
    private static void ShowSoftwareKeyboard(IntPtr focusedElement)
    {
        try
        {
            if (_isInitialized)
            {
                Noesis.UIElement element = Noesis.Extend.GetProxy(focusedElement, false) as Noesis.UIElement;
                _instance._softwareKeyboardManager.ShowKeyboard(element);
            }
        }
        catch(Exception e)
        {
            Noesis.Error.SetNativePendingError(e);
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////
    private delegate void InternalHideSoftwareKeyboardCallback();
    static private InternalHideSoftwareKeyboardCallback _hideSoftwareKeyboard = HideSoftwareKeyboard;

    [MonoPInvokeCallback(typeof(InternalHideSoftwareKeyboardCallback))]
    private static void HideSoftwareKeyboard()
    {
        try
        {
            if (_isInitialized)
            {
                _instance._softwareKeyboardManager.HideKeyboard();
            }
        }
        catch(Exception e)
        {
            Noesis.Error.SetNativePendingError(e);
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////
    private Noesis.SoftwareKeyboardManager _softwareKeyboardManager = null;

    #endregion

    #region Function imports

#if UNITY_EDITOR
    ////////////////////////////////////////////////////////////////////////////////////////////////
    private Noesis.Library _library = null;

    ////////////////////////////////////////////////////////////////////////////////////////////////
    private void DisposeLibrary()
    {
        if (_library != null)
        {
            _library.Dispose();
            _library = null;
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////
    private void RegisterFunctions(Noesis.Library lib)
    {
        _loadXAML = lib.Find<LoadXAMLDelegate>("Noesis_LoadXAML");
        _tryLoadXAML = lib.Find<LoadXAMLDelegate>("Noesis_TryLoadXAML");
        _initKernel = lib.Find<InitKernelDelegate>("Noesis_Init");
        _shutdownKernel = lib.Find<ShutdownKernelDelegate>("Noesis_Shutdown");
        _tickKernel = lib.Find<TickKernelDelegate>("Noesis_Tick");
        _registerSoftwareKeyboardCallbacks = lib.Find<RegisterSoftwareKeyboardCallbacksDelegate>(
            "Noesis_RegisterSoftwareKeyboardCallbacks");
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////
    private void UnregisterFunctions()
    {
        _loadXAML = null;
        _tryLoadXAML = null;
        _initKernel = null;
        _shutdownKernel = null;
        _tickKernel = null;
        _registerSoftwareKeyboardCallbacks = null;
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////
    delegate IntPtr LoadXAMLDelegate([MarshalAs(UnmanagedType.LPWStr)]string xamlFile);
    private LoadXAMLDelegate _loadXAML = null;
    private IntPtr Noesis_LoadXAML(string xamlFile)
    {
        IntPtr ret = _loadXAML(xamlFile);
        Noesis.Error.Check();
        return ret;
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////
    private LoadXAMLDelegate _tryLoadXAML = null;
    private IntPtr Noesis_TryLoadXAML(string xamlFile)
    {
        IntPtr ret = _tryLoadXAML(xamlFile);
        Noesis.Error.Check();
        return ret;
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////
    delegate int InitKernelDelegate([MarshalAs(UnmanagedType.LPWStr)]string dataPath,
        [MarshalAs(UnmanagedType.LPWStr)]string pluginsPath, bool isEditor,
        UnityLogCallback unityLog);
    private InitKernelDelegate _initKernel = null;
    private int Noesis_Init(string dataPath, string pluginsPath, bool isEditor,
        UnityLogCallback unityLog)
    {
        int deviceType = _initKernel(dataPath, pluginsPath, isEditor, unityLog);
        Noesis.Error.Check();

        return deviceType;
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////
    delegate void ShutdownKernelDelegate();
    private ShutdownKernelDelegate _shutdownKernel = null;
    private void Noesis_Shutdown()
    {
        _shutdownKernel();
        Noesis.Error.Check();
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////
    private delegate void TickKernelDelegate();
    private TickKernelDelegate _tickKernel = null;
    private void Noesis_Tick()
    {
        _tickKernel();
        Noesis.Error.Check();
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////
    delegate void RegisterSoftwareKeyboardCallbacksDelegate(
        InternalShowSoftwareKeyboardCallback showCallback,
        InternalHideSoftwareKeyboardCallback hideCallback);
    static RegisterSoftwareKeyboardCallbacksDelegate _registerSoftwareKeyboardCallbacks = null;
    static void Noesis_RegisterSoftwareKeyboardCallbacks(
        InternalShowSoftwareKeyboardCallback showCallback,
        InternalHideSoftwareKeyboardCallback hideCallback)
    {
        _registerSoftwareKeyboardCallbacks(showCallback, hideCallback);
    }

#else
    ////////////////////////////////////////////////////////////////////////////////////////////////
    #if UNITY_IPHONE || UNITY_XBOX360
    [DllImport("__Internal", EntryPoint="Noesis_LoadXAML")]
    #else
    [DllImport("Noesis", EntryPoint = "Noesis_LoadXAML")]
    #endif
    static extern IntPtr Noesis_LoadXAML([MarshalAs(UnmanagedType.LPWStr)]string xamlFile);

    ////////////////////////////////////////////////////////////////////////////////////////////////
    #if UNITY_IPHONE || UNITY_XBOX360
    [DllImport("__Internal", EntryPoint="Noesis_TryLoadXAML")]
    #else
    [DllImport("Noesis", EntryPoint = "Noesis_TryLoadXAML")]
    #endif
    static extern IntPtr Noesis_TryLoadXAML([MarshalAs(UnmanagedType.LPWStr)]string xamlFile);

    ////////////////////////////////////////////////////////////////////////////////////////////////
    #if UNITY_IPHONE || UNITY_XBOX360
    [DllImport("__Internal", EntryPoint="Noesis_Init")]
    #else
    [DllImport("Noesis", EntryPoint = "Noesis_Init")]
    #endif
    static extern int Noesis_Init([MarshalAs(UnmanagedType.LPWStr)]string dataPath,
        [MarshalAs(UnmanagedType.LPWStr)]string pluginsPath, bool isEditor,
        UnityLogCallback unityLog);

    ////////////////////////////////////////////////////////////////////////////////////////////////
    #if UNITY_IPHONE || UNITY_XBOX360
    [DllImport("__Internal", EntryPoint="Noesis_Shutdown")]
    #else
    [DllImport("Noesis", EntryPoint = "Noesis_Shutdown")]
    #endif
    static extern void Noesis_Shutdown();

    ////////////////////////////////////////////////////////////////////////////////////////////////
    #if UNITY_IPHONE || UNITY_XBOX360
    [DllImport("__Internal", EntryPoint="Noesis_Tick")]
    #else
    [DllImport("Noesis", EntryPoint = "Noesis_Tick")]
    #endif
    static extern void Noesis_Tick();

    ////////////////////////////////////////////////////////////////////////////////////////////////
    #if UNITY_IPHONE || UNITY_XBOX360
    [DllImport("__Internal", EntryPoint="Noesis_RegisterSoftwareKeyboardCallbacks")]
    #else
    [DllImport("Noesis", EntryPoint = "Noesis_RegisterSoftwareKeyboardCallbacks")]
    #endif
    static extern void Noesis_RegisterSoftwareKeyboardCallbacks(
        InternalShowSoftwareKeyboardCallback showCallback,
        InternalHideSoftwareKeyboardCallback hideCallback);
#endif

#if UNITY_EDITOR
    // Trampoline library used in editor to allow hot loading Noesis
    [DllImport("NoesisUnityRenderHook", EntryPoint = "UnityInitDevice")]
    private static extern void UnityInitDevice();
#else
    private static void UnityInitDevice() { }
#endif

    #endregion

    #endregion
}
