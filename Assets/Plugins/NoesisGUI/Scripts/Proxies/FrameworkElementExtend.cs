using System;
using System.Runtime.InteropServices;

namespace Noesis
{

    public partial class FrameworkElement
    {
        protected void InitializeComponent()
        {
        }

        public object FindResource(object key)
        {
            if (key is string)
            {
                return FindStringResource(key as string);
            }

            if (key is System.Type)
            {
                return FindTypeResource(key as Type);
            }

            throw new Exception("Only String or Type resource keys supported.");
        }

        public object TryFindResource(object key)
        {
            if (key is string)
            {
                return TryFindStringResource(key as string);
            }

            if (key is System.Type)
            {
                return TryFindTypeResource(key as Type);
            }

            throw new Exception("Only String or Type resource keys supported.");
        }

        #region FindResource implementation

        private object FindStringResource(string key)
        {
            IntPtr cPtr = NoesisGUI_PINVOKE.FrameworkElement_FindStringResource(swigCPtr, key);
            #if UNITY_EDITOR || NOESIS_API
            if (NoesisGUI_PINVOKE.SWIGPendingException.Pending) throw NoesisGUI_PINVOKE.SWIGPendingException.Retrieve();
            #endif
            return Noesis.Extend.GetProxy(cPtr, false);
        }

        private object FindTypeResource(System.Type key)
        {
            IntPtr nativeType = Noesis.Extend.GetNativeType(key);
            IntPtr cPtr = NoesisGUI_PINVOKE.FrameworkElement_FindTypeResource(swigCPtr, nativeType);
            #if UNITY_EDITOR || NOESIS_API
            if (NoesisGUI_PINVOKE.SWIGPendingException.Pending) throw NoesisGUI_PINVOKE.SWIGPendingException.Retrieve();
            #endif
            return Noesis.Extend.GetProxy(cPtr, false);
        }
        private object TryFindStringResource(string key)
        {
            IntPtr cPtr = NoesisGUI_PINVOKE.FrameworkElement_TryFindStringResource(swigCPtr, key);
            #if UNITY_EDITOR || NOESIS_API
            if (NoesisGUI_PINVOKE.SWIGPendingException.Pending) throw NoesisGUI_PINVOKE.SWIGPendingException.Retrieve();
            #endif
            return Noesis.Extend.GetProxy(cPtr, false);
        }

        private object TryFindTypeResource(System.Type key)
        {
            IntPtr nativeType = Noesis.Extend.GetNativeType(key);
            IntPtr cPtr = NoesisGUI_PINVOKE.FrameworkElement_TryFindTypeResource(swigCPtr, nativeType);
            #if UNITY_EDITOR || NOESIS_API
            if (NoesisGUI_PINVOKE.SWIGPendingException.Pending) throw NoesisGUI_PINVOKE.SWIGPendingException.Retrieve();
            #endif
            return Noesis.Extend.GetProxy(cPtr, false);
        }

        #endregion
    }

}