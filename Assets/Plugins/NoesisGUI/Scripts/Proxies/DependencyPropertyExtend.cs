using System;
using System.Runtime.InteropServices;
using System.Reflection;
using System.Collections.Generic;

namespace Noesis
{

    public partial class DependencyProperty
    {
        public static DependencyProperty Register(string name, System.Type propertyType,
            System.Type ownerType)
        {
            return RegisterCommon(name, propertyType, ownerType, null);
        }
        public static DependencyProperty Register(string name, System.Type propertyType,
            System.Type ownerType, PropertyMetadata propertyMetadata)
        {
            return RegisterCommon(name, propertyType, ownerType, propertyMetadata);
        }

        public static DependencyProperty RegisterAttached(string name, System.Type propertyType,
            System.Type ownerType)
        {
            return RegisterCommon(name, propertyType, ownerType, null);
        }

        public static DependencyProperty RegisterAttached(string name, System.Type propertyType,
            System.Type ownerType, PropertyMetadata propertyMetadata)
        {
            return RegisterCommon(name, propertyType, ownerType, propertyMetadata);
        }

        public void OverrideMetadata(System.Type forType, PropertyMetadata propertyMetadata)
        {
            IntPtr forTypePtr = Noesis.Extend.EnsureNativeType(forType, false);

            Noesis_OverrideMetadata_(forTypePtr, swigCPtr.Handle,
                PropertyMetadata.getCPtr(propertyMetadata).Handle);
        }

        internal static bool RegisterCalled { get; set; }

        #region Register implementation

        private static DependencyProperty RegisterCommon(string name, System.Type propertyType,
            System.Type ownerType, PropertyMetadata propertyMetadata)
        {
            ValidateParams(name, propertyType, ownerType);

            // Force native type registration, but skip DP registration because we are inside
            // static constructor and DP are already being registered
            IntPtr ownerTypePtr = Noesis.Extend.EnsureNativeType(ownerType, false);

            // Check property type is supported and get the registered native type
            Type originalPropertyType = propertyType;
            IntPtr nativeType = ValidatePropertyType(ref propertyType);

            // Create and register dependency property
            IntPtr dependencyPtr = Noesis_RegisterDependencyProperty_(ownerTypePtr,
                name, nativeType, PropertyMetadata.getCPtr(propertyMetadata).Handle);

            DependencyProperty dependencyProperty = new DependencyProperty(dependencyPtr, false);
            dependencyProperty.OriginalPropertyType = originalPropertyType;

            RegisterCalled = true;
            return dependencyProperty;
        }

        private static void ValidateParams(string name, System.Type propertyType,
            System.Type ownerType)
        {
            if (name == null)
            {
                throw new ArgumentNullException("name");
            }

            if (name.Length == 0)
            {
                throw new ArgumentException("Property name can't be empty");
            }

            if (ownerType == null)
            {
                throw new ArgumentNullException("ownerType");
            }

            if (propertyType == null)
            {
                throw new ArgumentNullException("propertyType");
            }
        }

        private Type OriginalPropertyType { get; set; }

        private static IntPtr ValidatePropertyType(ref Type propertyType)
        {
            Type validType;
            if (_validTypes.TryGetValue(propertyType.TypeHandle, out validType))
            {
                propertyType = validType;
            }

            return Noesis.Extend.EnsureNativeType(propertyType);
        }

        private static Dictionary<RuntimeTypeHandle, Type> _validTypes = CreateValidTypes();

        private static Dictionary<RuntimeTypeHandle, Type> CreateValidTypes()
        {
            Dictionary<RuntimeTypeHandle, Type> validTypes = new Dictionary<RuntimeTypeHandle, Type>(13);

            validTypes[typeof(decimal).TypeHandle] = typeof(double);
            validTypes[typeof(long).TypeHandle] = typeof(int);
            validTypes[typeof(ulong).TypeHandle] = typeof(uint);
            validTypes[typeof(char).TypeHandle] = typeof(uint);
            validTypes[typeof(sbyte).TypeHandle] = typeof(short);
            validTypes[typeof(byte).TypeHandle] = typeof(ushort);

            validTypes[typeof(decimal?).TypeHandle] = typeof(double?);
            validTypes[typeof(long?).TypeHandle] = typeof(int?);
            validTypes[typeof(ulong?).TypeHandle] = typeof(uint?);
            validTypes[typeof(char?).TypeHandle] = typeof(uint?);
            validTypes[typeof(sbyte?).TypeHandle] = typeof(short?);
            validTypes[typeof(byte?).TypeHandle] = typeof(ushort?);

            validTypes[typeof(Type).TypeHandle] = typeof(ResourceKeyType);

            return validTypes;
        }

        #endregion

        #region Imports

        private static IntPtr Noesis_RegisterDependencyProperty_(IntPtr classType,
            string propertyName, IntPtr propertyType, IntPtr propertyMetadata)
        {
            IntPtr result = Noesis_RegisterDependencyProperty(classType, propertyName, propertyType, propertyMetadata);
            #if UNITY_EDITOR || NOESIS_API
            Error.Check();
            #endif
            return result;
        }

        private static void Noesis_OverrideMetadata_(IntPtr classType, IntPtr dependencyProperty,
            IntPtr propertyMetadata)
        {
            Noesis_OverrideMetadata(classType, dependencyProperty, propertyMetadata);
            #if UNITY_EDITOR || NOESIS_API
            Error.Check();
            #endif
        }

    #if UNITY_EDITOR

        ////////////////////////////////////////////////////////////////////////////////////////////////
        public static void RegisterFunctions(Library lib)
        {
            // register DependencyProperty
            _RegisterDependencyProperty = lib.Find<RegisterDependencyPropertyDelegate>(
                "Noesis_RegisterDependencyProperty");

            // override PropertyMetadata 
            _OverrideMetadata = lib.Find<OverrideMetadataDelegate>("Noesis_OverrideMetadata");
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////
        public static void UnregisterFunctions()
        {
            // register DependencyProperty
            _RegisterDependencyProperty = null;
            _OverrideMetadata = null;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////
        delegate IntPtr RegisterDependencyPropertyDelegate(IntPtr classType,
            [MarshalAs(UnmanagedType.LPStr)]string propertyName, IntPtr propertyType,
            IntPtr propertyMetadata);
        static RegisterDependencyPropertyDelegate _RegisterDependencyProperty;
        private static IntPtr Noesis_RegisterDependencyProperty(IntPtr classType,
            string propertyName, IntPtr propertyType, IntPtr propertyMetadata)
        {
            return _RegisterDependencyProperty(classType, propertyName, propertyType,
                propertyMetadata);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////
        delegate void OverrideMetadataDelegate(IntPtr classType, IntPtr dependencyProperty,
            IntPtr propertyMetadata);
        static OverrideMetadataDelegate _OverrideMetadata;
        private static void Noesis_OverrideMetadata(IntPtr classType, IntPtr dependencyProperty,
            IntPtr propertyMetadata)
        {
            _OverrideMetadata(classType, dependencyProperty, propertyMetadata);
        }

    #else

        ////////////////////////////////////////////////////////////////////////////////////////////////
        #if UNITY_IPHONE || UNITY_XBOX360
        [DllImport("__Internal", EntryPoint="Noesis_RegisterDependencyProperty")]
        #else
        [DllImport("Noesis", EntryPoint = "Noesis_RegisterDependencyProperty")]
        #endif
        private static extern IntPtr Noesis_RegisterDependencyProperty(IntPtr classType,
            [MarshalAs(UnmanagedType.LPStr)]string propertyName, IntPtr propertyType,
            IntPtr propertyMetadata);

        ////////////////////////////////////////////////////////////////////////////////////////////////
        #if UNITY_IPHONE || UNITY_XBOX360
        [DllImport("__Internal", EntryPoint="Noesis_OverrideMetadata")]
        #else
        [DllImport("Noesis", EntryPoint = "Noesis_OverrideMetadata")]
        #endif
        private static extern void Noesis_OverrideMetadata(IntPtr classType,
            IntPtr dependencyProperty, IntPtr propertyMetadata);

    #endif

        #endregion
    }

}
