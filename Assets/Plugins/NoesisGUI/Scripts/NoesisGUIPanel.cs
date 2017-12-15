using UnityEngine;
using Noesis;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

[AddComponentMenu("NoesisGUI/NoesisGUI Panel")]
public class NoesisGUIPanel : MonoBehaviour
{
    #region Public properties

    /// <summary>
    /// Path to the XAML that will be loaded when this component is enabled.
    /// </summary>
    public string Xaml
    {
        set { this._xamlFile = value; }
        get { return this._xamlFile; }
    }

    /// <summary>
    /// Path to a resources XAML that defines the style of your application.
    /// </summary>
    public string Style
    {
        set { this._styleFile = value; }
        get { return this._styleFile; }
    }

    /// <summary>
    /// Size relative to screen size. Determines size of the offscreen textures used for opacity
    /// groups (UIElement.Opacity, UIElement.OpacityMask, and VisualBrush).
    /// </summary>
    public Vector2 OffscreenSize
    {
        set { this._offscreenSize = value; }
        get { return this._offscreenSize; }
    }

    /// <summary>
    /// Selects antialiasing mode.
    /// </summary>
    public AntialiasingMode AntiAliasingMode
    {
        set { this._antiAliasingMode = value; }
        get { return this._antiAliasingMode; }
    }

    /// <summary>
    /// Selects tessellation behavior.
    /// </summary>
    public TessellationMode TessellationMode
    {
        set { this._tessellationMode = value; }
        get { return this._tessellationMode; }
    }

    /// <summary>
    /// Determines the quantity of triangles generated for vector shapes.
    /// </summary>
    public TessellationQuality TessellationQuality
    {
        set { this._tessellationQuality = value; }
        get { return this._tessellationQuality; }
    }

    /// <summary>
    /// Bit flags used for debug rendering purposes.
    /// </summary>
    public RendererFlags RenderFlags
    {
        set { this._renderFlags = value; }
        get { return this._renderFlags; }
    }

    /// <summary>
    /// Enables keyboard input management.
    /// </summary>
    public bool EnableKeyboard
    {
        set { this._enableKeyboard = value; }
        get { return this._enableKeyboard; }
    }

    /// <summary>
    /// Enables mouse input management.
    /// </summary>
    public bool EnableMouse
    {
        set { this._enableMouse = value; }
        get { return this._enableMouse; }
    }

    /// <summary>
    /// Enables touch input management.
    /// </summary>
    public bool EnableTouch 
    {
        set { this._enableTouch = value; }
        get { return this._enableTouch; }
    }

    /// <summary>
    /// Emulate touch input with mouse.
    /// </summary>
    public bool EmulateTouch
    {
        set { this._emulateTouch = value; }
        get { return this._emulateTouch; }
    }

    /// <summary>
    /// When enabled, UI is affected by image post-processing.
    /// </summary>
    public bool EnablePostProcess 
    {
        set { this._enablePostProcess = value; }
        get { return this._enablePostProcess; }
    }

    /// <summary>
    /// Flips UI render vertically.
    /// </summary>
    public bool FlipVertically 
    {
        set { this._flipVertically = value; }
        get { return this._flipVertically; }
    }

    /// <summary>
    /// When enabled, UI is updated using Time.realtimeSinceStartup.
    /// </summary>
    public bool UseRealTimeClock 
    {
        set { this._useRealTimeClock = value; }
        get { return this._useRealTimeClock; }
    }

    /// <summary>
    /// Gets the root of the loaded Xaml.
    /// </summary>
    /// <returns>Root element.</returns>
    public FrameworkElement GetContent()
    {
        if (_uiRenderer != null)
        {
            return _uiRenderer.GetContent();
        }
        else
        {
            return null;
        }
    }

    /// <summary>
    /// Indicates if this component is rendering UI to a RenderTexture.
    /// </summary>
    /// <returns></returns>
    public bool IsRenderToTexture()
    {
        // Check if NoesisGUI was attached to a Unity GUI RawImage object with a RenderTexture
        UnityEngine.UI.RawImage img = gameObject.GetComponent<UnityEngine.UI.RawImage>();
        if (img != null && img.texture != null)
        {
            return img.texture is UnityEngine.RenderTexture;
        }

        // Check if NoesisGUI was attached to a GameObject with a RenderTexture set
        // in the diffuse texture of its main Material
        UnityEngine.Renderer renderer = gameObject.GetComponent<UnityEngine.Renderer>();
        if (renderer != null && renderer.sharedMaterial != null)
        {
            return renderer.sharedMaterial.mainTexture is UnityEngine.RenderTexture;
        }

        // No valid texture found
        return false;
    }

    #endregion

    #region Public events

    #region Keyboard input events
    /// <summary>
    /// Notifies Renderer that a key was pressed.
    /// </summary>
    /// <param name="key">Key identifier.</param>
    public void KeyDown(Noesis.Key key)
    {
        if (_uiRenderer != null)
        {
            _uiRenderer.KeyDown(key);
        }
    }

    /// <summary>
    /// Notifies Renderer that a key was released.
    /// </summary>
    /// <param name="key">Key identifier.</param>
    public void KeyUp(Noesis.Key key)
    {
        if (_uiRenderer != null)
        {
            _uiRenderer.KeyUp(key);
        }
    }

    /// <summary>
    /// Notifies Renderer that a key was translated to the corresponding character.
    /// </summary>
    /// <param name="ch">Unicode character value.</param>
    public void Char(uint ch)
    {
        if (_uiRenderer != null)
        {
            _uiRenderer.Char(ch);
        }
    }
    #endregion

    #region Mouse input events
    /// <summary>
    /// Notifies Renderer that mouse was moved. The mouse position is specified in renderer
    /// surface pixel coordinates.
    /// </summary>
    /// <param name="x">Mouse x-coordinate.</param>
    /// <param name="y">Mouse y-coordinate.</param>
    public void MouseMove(float x, float y)
    {
        if (_uiRenderer != null)
        {
            _uiRenderer.MouseMove(x, y);
        }
    }

    /// <summary>
    /// Notifies Renderer that a mouse button was pressed. The mouse position is specified in
    /// renderer surface pixel coordinates.
    /// </summary>
    /// <param name="x">Mouse x-coordinate.</param>
    /// <param name="y">Mouse y-coordinate.</param>
    /// <param name="button">Indicates which button was pressed.</param>
    public void MouseDown(float x, float y, Noesis.MouseButton button)
    {
        if (_uiRenderer != null)
        {
            _uiRenderer.MouseDown(x, y, button);
        }
    }

    /// Notifies Renderer that a mouse button was released. The mouse position is specified in
    /// renderer surface pixel coordinates.
    /// </summary>
    /// <param name="x">Mouse x-coordinate.</param>
    /// <param name="y">Mouse y-coordinate.</param>
    /// <param name="button">Indicates which button was released.</param>
    public void MouseUp(float x, float y, Noesis.MouseButton button)
    {
        if (_uiRenderer != null)
        {
            _uiRenderer.MouseUp(x, y, button);
        }
    }

    /// <summary>
    /// Notifies Renderer of a mouse button double click. The mouse position is specified in
    /// renderer surface pixel coordinates.
    /// </summary>
    /// <param name="x">Mouse x-coordinate.</param>
    /// <param name="y">Mouse y-coordinate.</param>
    /// <param name="button">Indicates which button was pressed.</param>
    public void MouseDoubleClick(float x, float y, Noesis.MouseButton button)
    {
        if (_uiRenderer != null)
        {
            _uiRenderer.MouseDoubleClick(x, y, button);
        }
    }

    /// <summary>
    /// Notifies Renderer that mouse wheel was rotated. The mouse position is specified in
    /// renderer surface pixel coordinates.
    /// </summary>
    /// <param name="x">Mouse x-coordinate.</param>
    /// <param name="y">Mouse y-coordinate.</param>
    /// <param name="wheelRotation">Indicates the amount mouse wheel has changed.</param>
    public void MouseWheel(float x, float y, int wheelRotation)
    {
        if (_uiRenderer != null)
        {
            _uiRenderer.MouseWheel(x, y, wheelRotation);
        }
    }
    #endregion

    #region Touch input events
    /// <summary>
    /// Notifies Renderer that a finger is moving on the screen. The finger position is
    /// specified in renderer surface pixel coordinates.
    /// </summary>
    /// <param name="x">Finger x-coordinate.</param>
    /// <param name="y">Finger y-coordinate.</param>
    /// <param name="touchId">Finger identifier.</param>
    public void TouchMove(float x, float y, uint touchId)
    {
        if (_uiRenderer != null)
        {
            _uiRenderer.TouchMove(x, y, touchId);
        }
    }

    /// <summary>
    /// Notifies Renderer that a finger touches the screen. The finger position is
    /// specified in renderer surface pixel coordinates.
    /// </summary>
    /// <param name="x">Finger x-coordinate.</param>
    /// <param name="y">Finger y-coordinate.</param>
    /// <param name="touchId">Finger identifier.</param>
    public void TouchDown(float x, float y, uint touchId)
    {
        if (_uiRenderer != null)
        {
            _uiRenderer.TouchDown(x, y, touchId);
        }
    }

    /// <summary>
    /// Notifies Renderer that a finger is raised off the screen. The finger position is
    /// specified in renderer surface pixel coordinates.
    /// </summary>
    /// <param name="x">Finger x-coordinate.</param>
    /// <param name="y">Finger y-coordinate.</param>
    /// <param name="touchId">Finger identifier.</param>
    public void TouchUp(float x, float y, uint touchId)
    {
        if (_uiRenderer != null)
        {
            _uiRenderer.TouchUp(x, y, touchId);
        }
    }
    #endregion

    /// <summary>
    /// Occurs just before the objects in the UI tree are rendered.
    /// </summary>
    public event System.EventHandler Rendering
    {
        add { if (_uiRenderer != null) { _uiRenderer.Rendering += value; } }
        remove { if (_uiRenderer != null) { _uiRenderer.Rendering -= value; } }
    }

    #endregion

    #region Public methods

    /// <summary>
    /// Unloads previously loaded XAML (if any), and forces loading the XAML specified in the
    /// component Xaml and Style properties.
    /// </summary>
    public void ForceLoadXaml()
    {
        DestroyRenderer();
        LoadXaml();
    }

    #endregion

    #region Private members

    #region MonoBehavior component messages
    ////////////////////////////////////////////////////////////////////////////////////////////////
    void Reset()
    {
        // Called once when component is attached to GameObject for the first time
        _offscreenSize = new Vector2(1, 1);
        _antiAliasingMode = AntialiasingMode.MSAA;
        _tessellationMode = TessellationMode.Threshold;
        _tessellationQuality = TessellationQuality.Medium;
        _renderFlags = RendererFlags.None;
        _enableKeyboard = true;
        _enableMouse = true;
        _enableTouch = true;
        _emulateTouch = false;
        _enablePostProcess = false;
        _flipVertically = false;
        _useRealTimeClock = false;
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////
    void Start()
    {
        // Disabling this lets you skip the GUI layout phase
        useGUILayout = false;
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////
    void OnEnable()
    {
        LoadXaml();
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////
    void LateUpdate()
    {
        if (_uiRenderer != null)
        {
            _uiRenderer.Update(_useRealTimeClock ? Time.realtimeSinceStartup : Time.time,
                _antiAliasingMode, _tessellationMode, _tessellationQuality, _renderFlags,
                _enableMouse, _enableTouch, _emulateTouch, _enablePostProcess, _flipVertically);
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////
    void OnBecameInvisible()
    {
        if (_uiRenderer != null && _uiRenderer.IsRenderToTexture)
        {
            _enableRender = false;
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////
    void OnBecameVisible()
    {
        if (_uiRenderer != null && _uiRenderer.IsRenderToTexture)
        {
            _enableRender = true;
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////
    void OnPreRender()
    {
        if (_uiRenderer != null && _enableRender)
        {
            _uiRenderer.PreRender();
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////
    void OnPostRender()
    {
        if (_uiRenderer != null && _enableRender)
        {
            if (_enablePostProcess || _uiRenderer.IsRenderToTexture)
            {
                _uiRenderer.PostRender();
            }
            else
            {
                StartCoroutine(RenderAtEndOfFrame());
            }
        }
    }

    private bool _enableRender = true;

    ////////////////////////////////////////////////////////////////////////////////////////////////
    IEnumerator RenderAtEndOfFrame()
    {
        yield return waitEndOfFrame;
        _uiRenderer.PostRender();
    }

    private YieldInstruction waitEndOfFrame = new WaitForEndOfFrame();

    ////////////////////////////////////////////////////////////////////////////////////////////////
    void OnGUI()
    {
        if (_uiRenderer != null)
        {
            _uiRenderer.ProcessEvent(UnityEngine.Event.current, _enableKeyboard, _enableMouse,
                _emulateTouch);
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////
    void OnApplicationFocus(bool focused)
    {
        if (_uiRenderer != null)
        {
            if (!NoesisGUISystem.SoftwareKeyboardManager.IsOpen)
            {
                if (focused)
                {
                    _uiRenderer.Activate();
                }
                else
                {
                    _uiRenderer.Deactivate();
                }
            }
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////
    void OnDestroy()
    {
        DestroyRenderer();
    }
    #endregion

    ////////////////////////////////////////////////////////////////////////////////////////////////
    private void LoadXaml()
    {
        // Create NoesisGUI System
        NoesisGUISystem.Create();

        // Create UI Renderer
        if (NoesisGUISystem.IsInitialized && _xamlFile.Length > 0 && _uiRenderer == null)
        {
            FrameworkElement root = NoesisGUISystem.LoadXaml(_xamlFile) as FrameworkElement;
            if (root != null)
            {
                if (_styleFile != "")
                {
                    ResourceDictionary resources = NoesisGUISystem.LoadXaml(_styleFile) as ResourceDictionary;
                    if (resources != null)
                    {
                        root.Resources.MergedDictionaries.Add(resources);
                    }
                    else
                    {
                        throw new System.Exception("Unable to load style xaml: " + _styleFile);
                    }
                }

                _uiRenderer = new Noesis.UIRenderer(root, _offscreenSize, gameObject);
                NoesisGUISystem.AddPanel(this);
            }
            else
            {
                throw new System.Exception("Unable to load xaml: " + _xamlFile);
            }
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////
    internal void DestroyRenderer()
    {
        // Destroy native UI renderer
        if (_uiRenderer != null)
        {
            NoesisGUISystem.RemovePanel(this);
            _uiRenderer.Destroy();
            _uiRenderer = null;
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////
    private Noesis.UIRenderer _uiRenderer;

    #region Properties needed by component editor
    public string _xamlFile = string.Empty;
    public Object _xaml = null;
    public string _styleFile = string.Empty;
    public Object _style = null;

    public Vector2 _offscreenSize = new Vector2(1, 1);

    public AntialiasingMode _antiAliasingMode = AntialiasingMode.MSAA;
    public TessellationMode _tessellationMode = TessellationMode.Threshold;
    public TessellationQuality _tessellationQuality = TessellationQuality.Medium;
    public RendererFlags _renderFlags;

    public bool _enableKeyboard = true;
    public bool _enableMouse = true;
    public bool _enableTouch = true;
    public bool _emulateTouch = false;

    public bool _enablePostProcess = false;

    public bool _flipVertically = false;

    public bool _useRealTimeClock = false;
    #endregion

    #endregion
}
