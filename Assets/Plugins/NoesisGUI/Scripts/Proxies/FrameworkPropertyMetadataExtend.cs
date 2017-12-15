using System;
using System.Runtime.InteropServices;
using System.Reflection;

namespace Noesis
{

    public partial class FrameworkPropertyMetadata
    {
        public FrameworkPropertyMetadata(object defaultValue)
            : this(Create(defaultValue, FrameworkOptions.None, null), true)
        {
        }

        public FrameworkPropertyMetadata(object defaultValue,
            PropertyChangedCallback propertyChangedCallback)
            : this(Create(defaultValue, FrameworkOptions.None, propertyChangedCallback), true)
        {
        }

        public FrameworkPropertyMetadata(object defaultValue, FrameworkOptions options)
            : this(Create(defaultValue, options, null), true)
        {
        }

        public FrameworkPropertyMetadata(object defaultValue, FrameworkOptions options,
            PropertyChangedCallback propertyChangedCallback)
            : this(Create(defaultValue, options, propertyChangedCallback), true)
        {
        }

        private static IntPtr Create(object defaultValue, FrameworkOptions options,
            PropertyChangedCallback propertyChangedCallback)
        {
            return Create(defaultValue, propertyChangedCallback,
                (def, invoke) => Noesis_CreateFrameworkPropertyMetadata_(def, (int)options, invoke));
        }

        #region Imports
        private static IntPtr Noesis_CreateFrameworkPropertyMetadata_(HandleRef defaultValue,
            int options, DelegateInvokePropertyChangedCallback invokePropertyChangedCallback)
        {
            IntPtr result = Noesis_CreateFrameworkPropertyMetadata(defaultValue,
                options, invokePropertyChangedCallback);
            #if UNITY_EDITOR || NOESIS_API
            Error.Check();
            #endif
            return result;
        }

    #if UNITY_EDITOR

        ////////////////////////////////////////////////////////////////////////////////////////////////
        public new static void RegisterFunctions(Library lib)
        {
            _CreateFrameworkPropertyMetadata = lib.Find<CreateFrameworkPropertyMetadataDelegate>(
                "Noesis_CreateFrameworkPropertyMetadata");
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////
        public new static void UnregisterFunctions()
        {
            // create FrameworkPropertyMetadata 
            _CreateFrameworkPropertyMetadata = null;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////
        delegate IntPtr CreateFrameworkPropertyMetadataDelegate(IntPtr defaultValue, int options,
            DelegateInvokePropertyChangedCallback invokePropertyChangedCallback);
        static CreateFrameworkPropertyMetadataDelegate _CreateFrameworkPropertyMetadata;
        private static IntPtr Noesis_CreateFrameworkPropertyMetadata(HandleRef defaultValue,
            int options, DelegateInvokePropertyChangedCallback invokePropertyChangedCallback)
        {
            return _CreateFrameworkPropertyMetadata(defaultValue.Handle, options,
                invokePropertyChangedCallback);
        }

    #else

        ////////////////////////////////////////////////////////////////////////////////////////////////
        #if UNITY_IPHONE || UNITY_XBOX360
        [DllImport("__Internal", EntryPoint="Noesis_CreateFrameworkPropertyMetadata")]
        #else
        [DllImport("Noesis", EntryPoint = "Noesis_CreateFrameworkPropertyMetadata")]
        #endif
        private static extern IntPtr Noesis_CreateFrameworkPropertyMetadata(HandleRef defaultValue,
            int options, DelegateInvokePropertyChangedCallback invokePropertyChangedCallback);

    #endif
        #endregion
    }

}
