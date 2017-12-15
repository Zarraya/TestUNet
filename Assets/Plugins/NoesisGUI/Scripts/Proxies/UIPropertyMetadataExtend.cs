using System;
using System.Runtime.InteropServices;
using System.Reflection;

namespace Noesis
{

    public partial class UIPropertyMetadata
    {
        public UIPropertyMetadata(object defaultValue)
            : this(Create(defaultValue, null), true)
        {
        }

        public UIPropertyMetadata(object defaultValue, PropertyChangedCallback propertyChangedCallback)
            : this(Create(defaultValue, propertyChangedCallback), true)
        {
        }

        private static IntPtr Create(object defaultValue, PropertyChangedCallback propertyChangedCallback)
        {
            return Create(defaultValue, propertyChangedCallback,
                (def, invoke) => Noesis_CreateUIPropertyMetadata_(def, invoke));
        }

        #region Imports
        private static IntPtr Noesis_CreateUIPropertyMetadata_(HandleRef defaultValue,
            DelegateInvokePropertyChangedCallback invokePropertyChangedCallback)
        {
            IntPtr result = Noesis_CreateUIPropertyMetadata(defaultValue,
                invokePropertyChangedCallback);
            #if UNITY_EDITOR || NOESIS_API
            Error.Check();
            #endif
            return result;
        }

    #if UNITY_EDITOR

        ////////////////////////////////////////////////////////////////////////////////////////////////
        public new static void RegisterFunctions(Library lib)
        {
            _CreateUIPropertyMetadata = lib.Find<CreateUIPropertyMetadataDelegate>(
                "Noesis_CreateUIPropertyMetadata");
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////
        public new static void UnregisterFunctions()
        {
            _CreateUIPropertyMetadata = null;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////
        delegate IntPtr CreateUIPropertyMetadataDelegate(IntPtr defaultValue,
            DelegateInvokePropertyChangedCallback invokePropertyChangedCallback);
        static CreateUIPropertyMetadataDelegate _CreateUIPropertyMetadata;
        private static IntPtr Noesis_CreateUIPropertyMetadata(HandleRef defaultValue,
            DelegateInvokePropertyChangedCallback invokePropertyChangedCallback)
        {
            return _CreateUIPropertyMetadata(defaultValue.Handle, invokePropertyChangedCallback);
        }

    #else

        ////////////////////////////////////////////////////////////////////////////////////////////////
        #if UNITY_IPHONE || UNITY_XBOX360
        [DllImport("__Internal", EntryPoint="Noesis_CreateUIPropertyMetadata")]
        #else
        [DllImport("Noesis", EntryPoint = "Noesis_CreateUIPropertyMetadata")]
        #endif
        private static extern IntPtr Noesis_CreateUIPropertyMetadata(HandleRef defaultValue,
            DelegateInvokePropertyChangedCallback invokePropertyChangedCallback);

    #endif
        #endregion
    }

}
