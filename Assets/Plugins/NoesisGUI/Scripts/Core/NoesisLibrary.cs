using UnityEngine;
using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;

namespace Noesis
{

#if UNITY_EDITOR

    ////////////////////////////////////////////////////////////////////////////////////////////////
    // Loads Noesis library
    ////////////////////////////////////////////////////////////////////////////////////////////////
    public class Library: IDisposable
    {
        private IntPtr handle_;
        private string filename_;

        public Library(string filename)
        {
            UnityEngine.RuntimePlatform platform = UnityEngine.Application.platform;
            if (platform == UnityEngine.RuntimePlatform.WindowsEditor)
            {
#if UNITY_EDITOR_64
                filename_ = filename + "_64.dll";
#else
                filename_ = filename + ".dll";
#endif

                // Verify that library is not already loaded, otherwise something went wrong
                if (GetModuleHandleWindows(filename_) != IntPtr.Zero)
                {
                    throw new Exception(String.Format("Critical problem, {0} already loaded", filename_));
                }

                handle_ = LoadLibraryWindows(filename_);

                if (handle_ == IntPtr.Zero)
                {
                    throw new Exception(String.Format("LoadLibrary {0}", filename_));
                }
            }
            else if (platform == UnityEngine.RuntimePlatform.OSXEditor)
            {
                filename_ = filename + ".bundle/Contents/MacOS/Noesis";

                if (GetModuleHandleOSX(filename_) != IntPtr.Zero)
                {
                    throw new Exception(String.Format("Critical problem, {0} already loaded", filename_));
                }

                handle_ = LoadLibraryOSX(filename_);

                if (handle_ == IntPtr.Zero)
                {
                    throw new Exception(String.Format("dlopen {0}", filename + ".bundle"));
                }
            }
            else
            {
                throw new Exception(String.Format("Platform {0} not supported", UnityEngine.Application.platform));
            }
        }

        ~Library()
        {
            Dispose();
        }

        public void Dispose()
        {
            if (handle_ != IntPtr.Zero)
            {
                UnityEngine.RuntimePlatform platform = UnityEngine.Application.platform;
                if (platform == UnityEngine.RuntimePlatform.WindowsEditor)
                {
                    FreeLibraryWindows(handle_);

                    IntPtr handle = GetModuleHandleWindows(filename_);
                    if (handle != IntPtr.Zero)
                    {
                        Debug.LogWarning(String.Format("Forcing unload of {0}", filename_));

                        int count = 0;
                        while (handle != IntPtr.Zero && count++ < 10)
                        {
                            FreeLibraryWindows(handle_);
                            handle = GetModuleHandleWindows(filename_);
                        }

                        if (handle != IntPtr.Zero)
                        {
                            Debug.LogWarning(String.Format(
                                "Unable to unload {0}. Please, restart Unity", filename_));
                        }
                    }
                }
                else if (platform == UnityEngine.RuntimePlatform.OSXEditor)
                {
                    FreeLibraryOSX(handle_);

                    IntPtr handle = GetModuleHandleOSX(filename_);
                    if (handle != IntPtr.Zero)
                    {
                        Debug.LogWarning(String.Format("Forcing unload of {0}", filename_));

                        int count = 0;
                        while (handle != IntPtr.Zero && count++ < 10)
                        {
                            FreeLibraryOSX(handle_);
                            handle = GetModuleHandleOSX(filename_);
                        }

                        if (handle != IntPtr.Zero)
                        {
                            Debug.LogWarning(String.Format(
                                "Unable to unload {0}. Please, restart Unity", filename_));
                        }
                    }
                }

                handle_ = IntPtr.Zero;
            }

            System.GC.SuppressFinalize(this);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////
        public T Find<T>(string funcName)
        {
            UnityEngine.RuntimePlatform platform = UnityEngine.Application.platform;
            if (platform == UnityEngine.RuntimePlatform.WindowsEditor)
            {
                IntPtr address = GetProcAddressWindows(handle_, funcName);
                if (address == IntPtr.Zero)
                {
                    throw new Exception(String.Format("GetProcAddress {0}", funcName));
                }
                return (T)(object)Marshal.GetDelegateForFunctionPointer(address, typeof(T));
            }
            else if (platform == UnityEngine.RuntimePlatform.OSXEditor)
            {
                IntPtr address = GetProcAddressOSX(handle_, funcName);
                if (address == IntPtr.Zero)
                {
                    throw new Exception(String.Format("dlsym {0}", funcName));
                }
                return (T)(object)Marshal.GetDelegateForFunctionPointer(address, typeof(T));
            }
            else
            {
                throw new Exception("Can't look for function in library for current platform");
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////
        // NOTE: This indirect call to PInvoke is needed to load kernel32.dll only when really needed
        ////////////////////////////////////////////////////////////////////////////////////////////////
        //@{
        private IntPtr LoadLibraryWindows(string dllToLoad)
        {
            return LoadLibrary(dllToLoad);
        }
        private IntPtr GetModuleHandleWindows(string dllToLoad)
        {
            return GetModuleHandle(dllToLoad);
        }
        private IntPtr GetProcAddressWindows(IntPtr hModule, string procedureName)
        {
            return GetProcAddress(hModule, procedureName);
        }
        private bool FreeLibraryWindows(IntPtr hModule)
        {
            if (!FreeLibrary(hModule))
            {
                Debug.LogError(String.Format("Cannot free {0} library: error {1}", filename_,
                    Marshal.GetLastWin32Error()));
                return false;
            }
            else
            {
                return true;
            }
        }

        [DllImport("kernel32", CharSet = CharSet.Unicode)]
        private static extern IntPtr LoadLibrary(string dllToLoad);

        [DllImport("kernel32", CharSet = CharSet.Unicode)]
        private static extern IntPtr GetModuleHandle(string dllToLoad);
        
        [DllImport("kernel32")]
        private static extern IntPtr GetProcAddress(IntPtr hModule, string procedureName);
        
        [DllImport("kernel32", SetLastError = true)]
        private static extern bool FreeLibrary(IntPtr hModule);
        //@}

        ////////////////////////////////////////////////////////////////////////////////////////////////
        // NOTE: This indirect call to PInvoke is needed to load dl.dylib only when really needed
        ////////////////////////////////////////////////////////////////////////////////////////////////
        //@{
        private IntPtr LoadLibraryOSX(string dllToLoad)
        {
            return dlopen(dllToLoad, RTLD_LAZY | RTLD_LOCAL);
        }
        private IntPtr GetModuleHandleOSX(string dllToLoad)
        {
            IntPtr h = dlopen(dllToLoad, RTLD_NOLOAD);
            if (h != IntPtr.Zero)
            {
                dlclose(h);
            }

            return h;
        }
        private IntPtr GetProcAddressOSX(IntPtr handle, string symbol)
        {
            return dlsym(handle, symbol);
        }
        private int FreeLibraryOSX(IntPtr handle)
        {
            int result = dlclose(handle);
            if (result != 0)
            {
                Debug.LogError(String.Format("Cannot close {0} library", filename_));
            }

            return result;
        }

        const int RTLD_LAZY = 0x1;
        const int RTLD_NOW = 0x2;
        const int RTLD_LOCAL = 0x4;
        const int RTLD_GLOBAL = 0x8;

        const int RTLD_NOLOAD = 0x10;
        const int RTLD_NODELETE = 0x80;
        const int RTLD_FIRST = 0x100;

        [DllImport("dl")]
        static extern IntPtr dlopen([MarshalAs(UnmanagedType.LPTStr)] string filename, int flags);

        [DllImport("dl")]
        static extern IntPtr dlsym(IntPtr handle, [MarshalAs(UnmanagedType.LPTStr)] string symbol);

        [DllImport("dl")]
        static extern int dlclose(IntPtr handle);
        //@}
    }

#endif // UNITY_EDITOR
}

