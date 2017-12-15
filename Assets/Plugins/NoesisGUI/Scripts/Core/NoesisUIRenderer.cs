using UnityEngine;
using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;

namespace Noesis
{
    public enum AntialiasingMode
    {
        /// <summary>
        /// Indicates that the antialising algorithm that will be used rely on the multisampling
        /// that is active in the target surface, if any.
        /// </summary>
        MSAA,
        /// <summary>
        /// Indicates that besides the multisampling in the target surface a per-primitive algorithm
        /// will be used. PPA implements antialiasing by extruding the contours of the triangles
        /// smoothing them.
        /// </summary>
        PPAA
    }

    public enum TessellationMode
    {
        /// <summary>
        /// Tessellation is done only the first time.
        /// </summary>
        Once,
        /// <summary>
        /// Tessellates geometry always.
        /// </summary>
        Always,
        /// <summary>
        /// Tessellates geometry when scale changes by a certain factor.
        /// </summary>
        Threshold
    }

    public enum TessellationQuality
    {
        /// <summary>
        /// The lowest tessellation quality for curves.
        /// </summary>
        Low,
        /// <summary>
        /// Medium tessellation quality for curves.
        /// </summary>
        Medium,
        /// <summary>
        /// The highest tessellation quality for curves.
        /// </summary>
        High
    }

    [Flags]
    public enum RendererFlags
    {
        /// <summary>
        /// No render flag is active.
        /// </summary>
        None = 0,
        /// <summary>
        /// Toggles wireframe mode when rendering triangles.
        /// </summary>
        Wireframe = 1,
        /// <summary>
        /// Each batch submitted to the GPU is given a unique solid color.
        /// </summary>
        ColorBatches = 2,
        /// <summary>
        /// Display pixel overdraw using blending layers. Different colors are used for each type
        /// of triangle. Green for normal, Red for opacities and Blue for clipping masks.
        /// </summary>
        ShowOverdraw = 4,
        /// <summary>
        /// Inverts the render vertically. Useful when rendering to OpenGL texture frame buffers.
        /// </summary>
        FlipY = 8,
        /// <summary>
        /// By default the stencil buffer is cleared automatically. Use this flag to avoid it.
        /// </summary>
        DisableStencil = 16,
        /// <summary>
        /// Use this flag to clear the color buffer to transparent (#00000000) before rendering.
        /// </summary>
        ClearColor = 32
    }

    /////////////////////////////////////////////////////////////////////////////////////
    /// Manages updates, render and input events of a Noesis UI panel
    /////////////////////////////////////////////////////////////////////////////////////
    internal partial class UIRenderer
    {
        /////////////////////////////////////////////////////////////////////////////////
        private int _rendererId;

        private Noesis.FrameworkElement _content;
        private Noesis.Visual _root;

        private UnityEngine.Vector2 _offscreenSize;
        private UnityEngine.Vector3 _mousePos;

        private UnityEngine.GameObject _target;

        private UnityEngine.RenderTexture _texture;
        private UnityEngine.Camera _textureCamera;

        private UnityEngine.Camera _targetCamera;

        /////////////////////////////////////////////////////////////////////////////////
        public bool IsRenderToTexture { get { return _texture != null; } }

        /////////////////////////////////////////////////////////////////////////////////
        public UIRenderer(Noesis.FrameworkElement content, UnityEngine.Vector2 offscreenSize,
            UnityEngine.GameObject target)
        {
            _rendererId = Noesis_CreateRenderer(Noesis.FrameworkElement.getCPtr(content).Handle);

            _content = content;
            _root = VisualTreeHelper.GetRoot(_content);

            _offscreenSize = offscreenSize;
            _mousePos = UnityEngine.Input.mousePosition;

            _target = target;

            _texture = FindTexture();
            if (IsRenderToTexture)
            {
                _textureCamera = _target.AddComponent<Camera>();
                _textureCamera.clearFlags = CameraClearFlags.SolidColor;
                _textureCamera.backgroundColor = new UnityEngine.Color(0.0f, 0.0f, 0.0f, 0.0f);
                _textureCamera.renderingPath = RenderingPath.Forward;
                _textureCamera.depthTextureMode = DepthTextureMode.None;
#if !UNITY_4_6 && !UNITY_5_0 && !UNITY_5_1
                _textureCamera.opaqueSortMode = UnityEngine.Rendering.OpaqueSortMode.NoDistanceSort;
#endif
                _textureCamera.transparencySortMode = TransparencySortMode.Orthographic;
                _textureCamera.clearStencilAfterLightingPass = false;
                _textureCamera.hdr = false;
                _textureCamera.useOcclusionCulling = false;
                _textureCamera.cullingMask = 0;
                _textureCamera.targetTexture = _texture;
            }

            _targetCamera = _target.GetComponent<Camera>();

            _Panels.Add(_rendererId, _target.GetComponent<NoesisGUIPanel>());
        }

        /////////////////////////////////////////////////////////////////////////////////
        public void Destroy()
        {
            _Panels.Remove(_rendererId);

            if (IsRenderToTexture)
            {
                UnityEngine.Object.Destroy(_textureCamera);
                _textureCamera = null;
                _texture = null;
            }

            _content = null;
            _root = null;

            if (NoesisGUISystem.IsInitialized)
            {
                Noesis_NotifyDestroyRenderer(_rendererId);
            }
        }

        /////////////////////////////////////////////////////////////////////////////////
        public void Update(double timeInSeconds, Noesis.AntialiasingMode aaMode,
            Noesis.TessellationMode tessMode, Noesis.TessellationQuality tessQuality,
            Noesis.RendererFlags flags, bool enableMouse, bool enableTouch, bool emulateTouch,
            bool enablePostProcess, bool flipVertically)
        {
            UpdateSettings(aaMode, tessMode, tessQuality, flags, enablePostProcess, flipVertically);
            UpdateInputs(enableMouse, enableTouch, emulateTouch);
            Noesis_UpdateRenderer(_rendererId, timeInSeconds);
        }

        /////////////////////////////////////////////////////////////////////////////////
        public event System.EventHandler Rendering
        {
            add
            {
                if (!_Rendering.ContainsKey(_rendererId))
                {
                    _Rendering.Add(_rendererId, null);
                    Noesis_BindRenderingEvent(_rendererId, _raiseRendering);
                }

                _Rendering[_rendererId] += value;
            }
            remove
            {
                if (_Rendering.ContainsKey(_rendererId))
                {
                    _Rendering[_rendererId] -= value;

                    if (_Rendering[_rendererId] == null)
                    {
                        Noesis_UnbindRenderingEvent(_rendererId, _raiseRendering);
                        _Rendering.Remove(_rendererId);
                    }
                }
            }
        }

        internal delegate void RaiseRenderingCallback(int id);
        private static RaiseRenderingCallback _raiseRendering = RaiseRendering;

        [MonoPInvokeCallback(typeof(RaiseRenderingCallback))]
        private static void RaiseRendering(int id)
        {
            try
            {
                if (!_Rendering.ContainsKey(id))
                {
                    throw new InvalidOperationException("Rendering delegate not registered");
                }
                EventHandler handler = _Rendering[id];
                if (handler != null)
                {
                    handler(_Panels[id], System.EventArgs.Empty);
                }
            }
            catch(Exception e)
            {
                Noesis.Error.SetNativePendingError(e);
            }
        }

        static Dictionary<int, EventHandler> _Rendering = new Dictionary<int, EventHandler>();
        static Dictionary<int, NoesisGUIPanel> _Panels = new Dictionary<int, NoesisGUIPanel>();

        /////////////////////////////////////////////////////////////////////////////////
        private static class RenderCallback
        {
            public static IntPtr Get() { return _renderCallback; }
            private static IntPtr _renderCallback = Noesis_GetRenderCallback();
        }

        /////////////////////////////////////////////////////////////////////////////////
        private void RenderEvent(int eventId)
        {
#if UNITY_4_6 || UNITY_5_0 || UNITY_5_1
            UnityEngine.GL.IssuePluginEvent(eventId);
#else
            UnityEngine.GL.IssuePluginEvent(RenderCallback.Get(), eventId);
#endif
        }

        /////////////////////////////////////////////////////////////////////////////////
        private const int PluginId = 0x0ACE0ACE;
        private const int PreRenderId = 2000 + PluginId;
        private const int PostRenderId = 1000 + PluginId;

        /////////////////////////////////////////////////////////////////////////////////
        public void PreRender()
        {
            // Wait until Update finishes to generate render commands
            Noesis_WaitUpdateRenderer(_rendererId);
            RenderEvent(PreRenderId + _rendererId);
        }

        /////////////////////////////////////////////////////////////////////////////////
        public void PostRender()
        {
            // Render UI to the active camera
            RenderEvent(PostRenderId + _rendererId);

#if !UNITY_4_1 && !UNITY_4_2
            if (IsRenderToTexture)
            {
                _texture.DiscardContents(false, true);
            }
#endif
        }

        /////////////////////////////////////////////////////////////////////////////////
        public FrameworkElement GetContent()
        {
            return _content;
        }

        /////////////////////////////////////////////////////////////////////////////////
        public void KeyDown(Noesis.Key key)
        {
            Noesis_KeyDown(_rendererId, (int)key);
        }

        /////////////////////////////////////////////////////////////////////////////////
        public void KeyUp(Noesis.Key key)
        {
            Noesis_KeyUp(_rendererId, (int)key);
        }

        /////////////////////////////////////////////////////////////////////////////////
        public void Char(uint ch)
        {
            Noesis_Char(_rendererId, ch);
        }

        /////////////////////////////////////////////////////////////////////////////////
        public void MouseMove(float x, float y)
        {
            Noesis_MouseMove(_rendererId, x, y);
        }

        /////////////////////////////////////////////////////////////////////////////////
        public void MouseDown(float x, float y, Noesis.MouseButton button)
        {
            Noesis_MouseButtonDown(_rendererId, x, y, (int)button);
        }

        /////////////////////////////////////////////////////////////////////////////////
        public void MouseUp(float x, float y, Noesis.MouseButton button)
        {
            Noesis_MouseButtonUp(_rendererId, x, y, (int)button);
        }

        /////////////////////////////////////////////////////////////////////////////////
        public void MouseDoubleClick(float x, float y, Noesis.MouseButton button)
        {
            Noesis_MouseDoubleClick(_rendererId, x, y, (int)button);
        }

        /////////////////////////////////////////////////////////////////////////////////
        public void MouseWheel(float x, float y, int wheelRotation)
        {
            Noesis_MouseWheel(_rendererId, x, y, wheelRotation);
        }

        /////////////////////////////////////////////////////////////////////////////////
        public void TouchMove(float x, float y, uint touchId)
        {
            Noesis_TouchMove(_rendererId, x, y, (int)touchId);
        }

        /////////////////////////////////////////////////////////////////////////////////
        public void TouchDown(float x, float y, uint touchId)
        {
            Noesis_TouchDown(_rendererId, x, y, (int)touchId);
        }

        /////////////////////////////////////////////////////////////////////////////////
        public void TouchUp(float x, float y, uint touchId)
        {
            Noesis_TouchUp(_rendererId, x, y, (int)touchId);
        }

        /////////////////////////////////////////////////////////////////////////////////
        private UnityEngine.EventModifiers _modifiers = 0;

        private void ProcessModifierKey(EventModifiers modifiers, EventModifiers delta,
            EventModifiers flag, Noesis.Key key)
        {
            if ((delta & flag) > 0)
            {
                if ((modifiers & flag) > 0)
                {
                    Noesis_KeyDown(_rendererId, (int)key);
                }
                else
                {
                    Noesis_KeyUp(_rendererId, (int)key);
                }
            }
        }

#if !UNITY_EDITOR && UNITY_STANDALONE_OSX
        private static int lastFrame;
        private static int lastKeyDown;
#endif

        private bool _touchEmulated = false;

        public void ProcessEvent(UnityEngine.Event ev, bool enableKeyboard, bool enableMouse,
            bool emulateTouch)
        {
            // Process keyboard modifiers
            if (enableKeyboard)
            {
                EventModifiers delta = ev.modifiers ^ _modifiers;
                if (delta > 0)
                {
                    _modifiers = ev.modifiers;

                    ProcessModifierKey(ev.modifiers, delta, EventModifiers.Shift, Key.Shift);
                    ProcessModifierKey(ev.modifiers, delta, EventModifiers.Control, Key.Control);
                    ProcessModifierKey(ev.modifiers, delta, EventModifiers.Command, Key.Control);
                    ProcessModifierKey(ev.modifiers, delta, EventModifiers.Alt, Key.Alt);
                }
            }

            switch (ev.type)
            {
                case UnityEngine.EventType.MouseDown:
                {
                    if (enableMouse)
                    {
                        UnityEngine.Vector2 mouse = ProjectPointer(ev.mousePosition.x,
                            UnityEngine.Screen.height - ev.mousePosition.y);

                        if (HitTest(mouse.x, mouse.y))
                        {
                            ev.Use();
                        }

                        if (emulateTouch)
                        {
                            Noesis_TouchDown(_rendererId, mouse.x, mouse.y, 0);
                            _touchEmulated = true;
                        }
                        else
                        {
                            // Ignore events generated by Unity to simulate a mouse down when a
                            // touch event occurs
                            bool mouseEmulated = Input.simulateMouseWithTouches && Input.touchCount > 0;
                            if (!mouseEmulated)
                            {
                                Noesis_MouseButtonDown(_rendererId, mouse.x, mouse.y, ev.button);

                                if (ev.clickCount == 2)
                                {
                                    Noesis_MouseDoubleClick(_rendererId, mouse.x, mouse.y, ev.button);
                                }
                            }
                        }
                    }
                    break;
                }
                case UnityEngine.EventType.MouseUp:
                {
                    if (enableMouse)
                    {
                        UnityEngine.Vector2 mouse = ProjectPointer(ev.mousePosition.x,
                            UnityEngine.Screen.height - ev.mousePosition.y);

                        if (HitTest(mouse.x, mouse.y))
                        {
                            ev.Use();
                        }

                        if (emulateTouch && _touchEmulated)
                        {
                            Noesis_TouchUp(_rendererId, mouse.x, mouse.y, 0);
                            _touchEmulated = false;
                        }
                        else
                        {
                            // Ignore events generated by Unity to simulate a mouse up when a
                            // touch event occurs
                            bool mouseEmulated = Input.simulateMouseWithTouches && Input.touchCount > 0;
                            if (!mouseEmulated)
                            {
                                Noesis_MouseButtonUp(_rendererId, mouse.x, mouse.y, ev.button);
                            }
                        }
                    }
                    break;
                }
                case UnityEngine.EventType.KeyDown:
                {
                    if (enableKeyboard)
                    {
                        if (ev.keyCode != KeyCode.None)
                        {
                            int noesisKeyCode = NoesisKeyCodes.Convert(ev.keyCode);
                            if (noesisKeyCode != 0)
                            {
#if !UNITY_EDITOR && UNITY_STANDALONE_OSX
                                // In OSX Standalone, CMD + key always sends two KeyDown events for the key.
                                // This seems to be a bug in Unity. 
                                if (!ev.command || lastFrame != Time.frameCount || lastKeyDown != noesisKeyCode)
                                {
                                    lastFrame = Time.frameCount;
                                    lastKeyDown = noesisKeyCode;
#endif
                                    Noesis_KeyDown(_rendererId, noesisKeyCode);
#if !UNITY_EDITOR && UNITY_STANDALONE_OSX
                                }
#endif
                            }
                        }

                        if (ev.character != 0)
                        {
                            // Filter out character events when CTRL is down
                            bool isControl = (_modifiers & EventModifiers.Control) != 0 || (_modifiers & EventModifiers.Command) != 0;
                            bool isAlt = (_modifiers & EventModifiers.Alt) != 0;
                            bool filter = isControl && !isAlt;

                            if (!filter)
                            {
#if !UNITY_EDITOR && UNITY_STANDALONE_LINUX
                                // It seems that linux is sending KeySyms instead of Unicode points
                                // https://github.com/substack/node-keysym/blob/master/data/keysyms.txt
                                ev.character = NoesisKeyCodes.KeySymToUnicode(ev.character);
#endif
                                Noesis_Char(_rendererId, (uint)ev.character);
                            }
                        }
                    }
                    break;
                }
                case UnityEngine.EventType.KeyUp:
                {
                    if (enableKeyboard)
                    {
                        if (ev.keyCode != UnityEngine.KeyCode.None)
                        {
                            int noesisKeyCode = NoesisKeyCodes.Convert(ev.keyCode);
                            if (noesisKeyCode != 0)
                            {
                                Noesis_KeyUp(_rendererId, noesisKeyCode);
                            }
                        }
                    }
                    break;
                }
            }
        }

        /////////////////////////////////////////////////////////////////////////////////
        public void Activate()
        {
            Noesis_Activate(_rendererId);
        }

        /////////////////////////////////////////////////////////////////////////////////
        public void Deactivate()
        {
            Noesis_Deactivate(_rendererId);
            _modifiers = 0;
        }

        /////////////////////////////////////////////////////////////////////////////////
        private void UpdateSettings(Noesis.AntialiasingMode aaMode,
            Noesis.TessellationMode tessMode, Noesis.TessellationQuality tessQuality,
            Noesis.RendererFlags flags, bool enablePostProcess, bool flipVertically)
        {
            // update renderer size
            if (!IsRenderToTexture)
            {
                Noesis_RendererSurfaceSize(_rendererId,
                    UnityEngine.Screen.width, UnityEngine.Screen.height,
                    _offscreenSize.x, _offscreenSize.y,
                    UnityEngine.QualitySettings.antiAliasing);

                if (_isGraphicsDeviceDirectX)
                {
                    UnityEngine.Camera camera = _targetCamera ?? UnityEngine.Camera.main;

                    if (enablePostProcess && camera != null && (
                        camera.actualRenderingPath == UnityEngine.RenderingPath.DeferredLighting ||
                        camera.actualRenderingPath == UnityEngine.RenderingPath.DeferredShading))
                    {
                        flags |= RendererFlags.FlipY;
                    }
                }
            }
            else // Render to Texture
            {
                System.Diagnostics.Debug.Assert(_texture.width > 0);
                System.Diagnostics.Debug.Assert(_texture.height > 0);

                Noesis_RendererSurfaceSize(_rendererId,
                    _texture.width, _texture.height,
                    _offscreenSize.x, _offscreenSize.y,
                    UnityEngine.QualitySettings.antiAliasing);

                if (_isGraphicsDeviceDirectX)
                {
                    flags |= RendererFlags.FlipY;
                }
            }

            if (flipVertically)
            {
                flags ^= RendererFlags.FlipY;
            }

            // update renderer settings
            Noesis_RendererAntialiasingMode(_rendererId, (int)aaMode);
            Noesis_RendererTessMode(_rendererId, (int)tessMode);
            Noesis_RendererTessQuality(_rendererId, (int)tessQuality);
            Noesis_RendererFlags(_rendererId, (int)flags);
        }

        /////////////////////////////////////////////////////////////////////////////////
        private void UpdateInputs(bool enableMouse, bool enableTouch, bool emulateTouch)
        {
#if !UNITY_STANDALONE && !UNITY_EDITOR
            enableMouse = false;
#endif
            if (enableMouse)
            {
                // mouse move
                if (_mousePos != UnityEngine.Input.mousePosition)
                {
                    _mousePos = UnityEngine.Input.mousePosition;
                    UnityEngine.Vector2 mouse = ProjectPointer(_mousePos.x, _mousePos.y);

                    if (emulateTouch && _touchEmulated)
                    {
                        Noesis_TouchMove(_rendererId, mouse.x, mouse.y, 0);
                    }
                    else
                    {
                        Noesis_MouseMove(_rendererId, mouse.x, mouse.y);
                    }
                }

                // mouse wheel
                int mouseWheel = (int)(UnityEngine.Input.GetAxis("Mouse ScrollWheel") * 10.0f);
                if (mouseWheel != 0)
                {
                    UnityEngine.Vector2 mouse = ProjectPointer(_mousePos.x, _mousePos.y);
                    Noesis_MouseWheel(_rendererId, mouse.x, mouse.y, mouseWheel);
                }
            }

            if (enableTouch)
            {
                for (int i = 0; i < UnityEngine.Input.touchCount; i++) 
                {
                    UnityEngine.Touch touch = UnityEngine.Input.GetTouch(i);
                    int id = touch.fingerId;
                    UnityEngine.Vector2 pos = ProjectPointer(touch.position.x, touch.position.y);
                    UnityEngine.TouchPhase phase = touch.phase;

                    if (phase == UnityEngine.TouchPhase.Began)
                    {
                        Noesis_TouchDown(_rendererId, pos.x, pos.y, id);
                    }
                    else if (phase == UnityEngine.TouchPhase.Moved ||
                        phase == UnityEngine.TouchPhase.Stationary)
                    {
                        Noesis_TouchMove(_rendererId, pos.x, pos.y, id);
                    }
                    else
                    {
                        Noesis_TouchUp(_rendererId, pos.x, pos.y, id);
                    }
                }
            }
        }

        UnityEngine.EventSystems.PointerEventData _pointerData;

        /////////////////////////////////////////////////////////////////////////////////
        private UnityEngine.Vector2 ProjectPointer(float x, float y)
        {
            if (!IsRenderToTexture)
            {
                // Screen coordinates
                return new UnityEngine.Vector2(x, UnityEngine.Screen.height - y);
            }
            else
            {
                // Texture coordinates

                // First try with Unity GUI RawImage objects
                UnityEngine.EventSystems.EventSystem eventSystem = 
                    UnityEngine.EventSystems.EventSystem.current;

                if (eventSystem != null && eventSystem.IsPointerOverGameObject())
                {
                    UnityEngine.Vector2 pos = new UnityEngine.Vector2(x, y);

                    if (_pointerData == null)
                    {
                        _pointerData = new UnityEngine.EventSystems.PointerEventData(eventSystem)
                        {
                            pointerId = 0,
                            position = pos
                        };
                    }
                    else
                    {
                        _pointerData.Reset();
                    }

                    _pointerData.delta = pos - _pointerData.position;
                    _pointerData.position = pos;

                    UnityEngine.RectTransform rect =
                        _target.GetComponent<UnityEngine.RectTransform>();

                    if (rect != null &&
                        UnityEngine.RectTransformUtility.ScreenPointToLocalPointInRectangle(
                            rect, _pointerData.position, _pointerData.pressEventCamera, out pos))
                    {
                        UnityEngine.Vector2 pivot = new UnityEngine.Vector2(
                            rect.pivot.x * rect.rect.width,
                            rect.pivot.y * rect.rect.height);

                        float texCoordX = (pos.x + pivot.x) / rect.rect.width;
                        float texCoordY = (pos.y + pivot.y) / rect.rect.height;

                        float localX = _texture.width * texCoordX;
                        float localY = _texture.height * (1.0f - texCoordY);
                        return new UnityEngine.Vector2(localX, localY);
                    }
                }

                // NOTE: A MeshCollider must be attached to the target to obtain valid
                // texture coordintates, otherwise Hit Testing won't work

                UnityEngine.Ray ray = UnityEngine.Camera.main.ScreenPointToRay(
                    new UnityEngine.Vector3(x, y, 0));

                UnityEngine.RaycastHit hit;
                if (UnityEngine.Physics.Raycast(ray, out hit))
                {
                    if (hit.collider.gameObject == _target)
                    {
                        float localX = _texture.width * hit.textureCoord.x;
                        float localY = _texture.height * (1.0f - hit.textureCoord.y);
                        return new UnityEngine.Vector2(localX, localY);
                    }
                }

                return new UnityEngine.Vector2(-1, -1);
            }
        }
        
        /////////////////////////////////////////////////////////////////////////////////
        private bool HitTest(float x, float y)
        {
            return Noesis_HitTest(Noesis.Visual.getCPtr(_root).Handle, x, y);
        }

        /////////////////////////////////////////////////////////////////////////////////
        private UnityEngine.RenderTexture FindTexture()
        {
            // Check if NoesisGUI was attached to a GameObject with a RenderTexture set
            // in the diffuse texture of its main Material
            Renderer renderer = _target.GetComponent<Renderer>();
            if (renderer != null && renderer.material != null)
            {
                return renderer.material.mainTexture as UnityEngine.RenderTexture;
            }

            // Check if NoesisGUI was attached to a Unity GUI RawImage object with a RenderTexture
            UnityEngine.UI.RawImage img = _target.GetComponent<UnityEngine.UI.RawImage>();
            if (img != null && img.texture != null)
            {
                return img.texture as UnityEngine.RenderTexture;
            }

            // No valid texture found
            return null;
        }

        /////////////////////////////////////////////////////////////////////////////////
        enum GfxDeviceRenderer
        {
            OpenGL = 0,              // OpenGL
            D3D9 = 1,                // Direct3D 9
            D3D11 = 2,               // Direct3D 11
            GCM = 3,                 // Sony PlayStation 3 GCM
            Null = 4,                // "null" device (used in batch mode)
            Xenon = 6,               // Xbox 360
            OpenGLES20 = 8,          // OpenGL ES 2.0
            OpenGLES30 = 11,         // OpenGL ES 3.0
            GXM = 12,                // PlayStation Vita
            PS4 = 13,                // PlayStation 4
            XboxOne = 14,            // Xbox One
            Metal = 16,              // iOS Metal
            UnifiedOpenGL = 17,      // Unified OpenGL (Unity 5.1)
        };

        private static bool _isGraphicsDeviceDirectX = false;

        public static void SetDeviceType(int deviceType)
        {
            GfxDeviceRenderer gfxDeviceRenderer = (GfxDeviceRenderer)deviceType;

            _isGraphicsDeviceDirectX = gfxDeviceRenderer == GfxDeviceRenderer.D3D9 ||
                gfxDeviceRenderer == GfxDeviceRenderer.D3D11;
        }
    }
}
