using System;
using System.Runtime.InteropServices;

namespace Noesis
{
    public class Error
    {
        public static void RegisterCallback()
        {
            Noesis_RegisterErrorCallback(_errorCallback);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////
        private class NoesisException: Exception
        {
            public NoesisException(string message): base(message) {}
        }

        public static void Check()
        {
            if (_pendingError.Length > 0)
            {
                string message = _pendingError;
                _pendingError = "";

                throw new NoesisException(message);
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////
        public static void SetNativePendingError(System.Exception exception)
        {
            Debug.LogException(exception);
    #if UNITY_EDITOR || NOESIS_API
            Noesis_CppSetPendingError(exception.GetType() + ": " + exception.Message);
    #endif
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////
        private delegate void ErrorCallback([MarshalAs(UnmanagedType.LPWStr)]string desc);
        private static ErrorCallback _errorCallback = SetPendingError;

        [MonoPInvokeCallback(typeof(ErrorCallback))]
        private static void SetPendingError(string desc)
        {
            // Do not overwrite if there is already an exception pending
            if (_pendingError.Length == 0)
            {
                _pendingError = desc;
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////
        private static string _pendingError = "";

    #if UNITY_EDITOR
        ////////////////////////////////////////////////////////////////////////////////////////////////
        public static void RegisterFunctions(Library lib)
        {
            _registerErrorCallback = lib.Find<RegisterErrorCallbackDelegate>("Noesis_RegisterErrorCallback");
            _noesisCppSetPendingError = lib.Find<NoesisCppSetPendingErrorDelegate>("Noesis_CppSetPendingError");
            RegisterCallback();
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////
        public static void UnregisterFunctions()
        {
            _registerErrorCallback = null;
            _noesisCppSetPendingError = null;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////
        delegate void RegisterErrorCallbackDelegate(ErrorCallback errorCallback);
        static RegisterErrorCallbackDelegate _registerErrorCallback;
        static void Noesis_RegisterErrorCallback(ErrorCallback errorCallback)
        {
            _registerErrorCallback(errorCallback);
        }

        delegate void NoesisCppSetPendingErrorDelegate([MarshalAs(UnmanagedType.LPWStr)]string message);
        static NoesisCppSetPendingErrorDelegate _noesisCppSetPendingError = null;
        private static void Noesis_CppSetPendingError(string message)
        {
            _noesisCppSetPendingError(message);
        }

#else

        ////////////////////////////////////////////////////////////////////////////////////////////////
        #if UNITY_IPHONE || UNITY_XBOX360
        [DllImport("__Internal", EntryPoint="Noesis_RegisterErrorCallback")]
        #else
        [DllImport("Noesis", EntryPoint = "Noesis_RegisterErrorCallback")]
        #endif
        private static extern void Noesis_RegisterErrorCallback(ErrorCallback errorCallback);

        ////////////////////////////////////////////////////////////////////////////////////////////////
        #if UNITY_IPHONE || UNITY_XBOX360
        [DllImport("__Internal", EntryPoint="Noesis_CppSetPendingError")]
        #else
        [DllImport("Noesis", EntryPoint = "Noesis_CppSetPendingError")]
        #endif
        private static extern void Noesis_CppSetPendingError([MarshalAs(UnmanagedType.LPWStr)]string message);

    #endif
    }
}

