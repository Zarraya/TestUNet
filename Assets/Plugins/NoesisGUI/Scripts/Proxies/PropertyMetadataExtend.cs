using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Reflection;

namespace Noesis
{
    public delegate void PropertyChangedCallback(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e
    );

    public partial class PropertyMetadata
    {
        public PropertyMetadata(object defaultValue)
            : this(Create(defaultValue, null), true)
        {
        }

        public PropertyMetadata(object defaultValue,
            PropertyChangedCallback propertyChangedCallback)
            : this(Create(defaultValue, propertyChangedCallback), true)
        {
        }

        private static IntPtr Create(object defaultValue,
            PropertyChangedCallback propertyChangedCallback)
        {
            return Create(defaultValue, propertyChangedCallback,
                (def, invoke) => Noesis_CreatePropertyMetadata_(def, invoke));
        }

        protected delegate IntPtr CreateMetadataCallback(HandleRef defaultValue,
            DelegateInvokePropertyChangedCallback invokePropertyChanged);

        protected static IntPtr Create(object defaultValue,
            PropertyChangedCallback propertyChangedCallback,
            CreateMetadataCallback createCallback)
        {
            if (defaultValue is Type)
            {
                defaultValue = Noesis.Extend.GetResourceKeyType((Type)defaultValue);
            }

            DelegateInvokePropertyChangedCallback invokePropertyChanged =
                propertyChangedCallback == null ? null : _invokePropertyChanged;

            IntPtr propertyMetadataPtr = createCallback(Noesis.Extend.GetInstanceHandle(defaultValue),
                invokePropertyChanged);

            // Register property changed callback
            if (propertyChangedCallback != null)
            {
                _PropertyChangedCallback.Add(propertyMetadataPtr, propertyChangedCallback);
            }

            return propertyMetadataPtr;
        }

        #region PropertyChangedCallback management

        protected delegate void DelegateInvokePropertyChangedCallback(IntPtr cPtr, IntPtr sender, IntPtr e);
        private static DelegateInvokePropertyChangedCallback _invokePropertyChanged =
            InvokePropertyChangedCallback;

        [MonoPInvokeCallback(typeof(DelegateInvokePropertyChangedCallback))]
        protected static void InvokePropertyChangedCallback(IntPtr cPtr, IntPtr sender, IntPtr e)
        {
            try
            {
                if (!_PropertyChangedCallback.ContainsKey(cPtr))
                {
                    throw new System.Exception("PropertyChangedCallback not found");
                }
                if (sender == System.IntPtr.Zero && e == System.IntPtr.Zero)
                {
                    _PropertyChangedCallback.Remove(cPtr);
                    return;
                }
                if (Noesis.Extend.Initialized)
                {
                    PropertyChangedCallback handler = _PropertyChangedCallback[cPtr];
                    if (handler != null)
                    {
                        handler((DependencyObject)Noesis.Extend.GetProxy(sender, false),
                            new DependencyPropertyChangedEventArgs(e, false));
                    }
                }
            }
            catch (System.Exception exception)
            {
                Noesis.Error.SetNativePendingError(exception);
            }
        }

        static protected Dictionary<IntPtr, PropertyChangedCallback> _PropertyChangedCallback =
            new Dictionary<IntPtr, PropertyChangedCallback>();

        internal static void ClearCallbacks()
        {
            _PropertyChangedCallback.Clear();
        }

        #endregion

        #region Imports

        private static IntPtr Noesis_CreatePropertyMetadata_(HandleRef defaultValue,
            DelegateInvokePropertyChangedCallback invokePropertyChangedCallback)
        {
            IntPtr result = Noesis_CreatePropertyMetadata(defaultValue, invokePropertyChangedCallback);
            #if UNITY_EDITOR || NOESIS_API
            Error.Check();
            #endif
            return result;
        }

    #if UNITY_EDITOR

        ////////////////////////////////////////////////////////////////////////////////////////////////
        public static void RegisterFunctions(Library lib)
        {
            _CreatePropertyMetadata = lib.Find<CreatePropertyMetadataDelegate>(
                "Noesis_CreatePropertyMetadata");
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////
        public static void UnregisterFunctions()
        {
            _CreatePropertyMetadata = null;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////
        delegate IntPtr CreatePropertyMetadataDelegate(IntPtr defaultValue,
            DelegateInvokePropertyChangedCallback invokePropertyChangedCallback);
        static CreatePropertyMetadataDelegate _CreatePropertyMetadata;
        private static IntPtr Noesis_CreatePropertyMetadata(HandleRef defaultValue,
            DelegateInvokePropertyChangedCallback invokePropertyChangedCallback)
        {
            return _CreatePropertyMetadata(defaultValue.Handle, invokePropertyChangedCallback);
        }

    #else

        ////////////////////////////////////////////////////////////////////////////////////////////////
        #if UNITY_IPHONE || UNITY_XBOX360
        [DllImport("__Internal", EntryPoint="Noesis_CreatePropertyMetadata")]
        #else
        [DllImport("Noesis", EntryPoint = "Noesis_CreatePropertyMetadata")]
        #endif
        private static extern IntPtr Noesis_CreatePropertyMetadata(HandleRef defaultValue,
            DelegateInvokePropertyChangedCallback invokePropertyChangedCallback);

    #endif
        #endregion
    }

}
