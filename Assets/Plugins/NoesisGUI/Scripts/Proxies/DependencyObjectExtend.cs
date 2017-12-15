using System;
using System.Runtime.InteropServices;
using System.Reflection;
using System.Collections.Generic;

namespace Noesis
{

    public partial class DependencyObject
    {
        public object GetValue(DependencyProperty dp)
        {
            if (dp == null)
            {
                throw new Exception("Can't get value, DependencyProperty is null");
            }

            Type dpType = dp.PropertyType;

            GetDelegate getDelegate;
            if (dpType.GetTypeInfo().IsEnum)
            {
                _getFunctions.TryGetValue(typeof(int).TypeHandle, out getDelegate);
                int value = (int)getDelegate(swigCPtr.Handle, DependencyProperty.getCPtr(dp).Handle);
                return Enum.ToObject(dpType, value);
            }
            else if (_getFunctions.TryGetValue(dpType.TypeHandle, out getDelegate))
            {
                return getDelegate(swigCPtr.Handle, DependencyProperty.getCPtr(dp).Handle);
            }
            else
            {
                IntPtr ptr = Noesis_DependencyGet_BaseComponent_(swigCPtr.Handle, DependencyProperty.getCPtr(dp).Handle);
                return Noesis.Extend.GetProxy(ptr, false);
            }
        }


        public void SetValue(DependencyProperty dp, object value)
        {
            if (dp == null)
            {
                throw new Exception("Can't set value, DependencyProperty is null");
            }

            Type dpType = dp.PropertyType;

            SetDelegate setDelegate;
            if (dpType.GetTypeInfo().IsEnum)
            {
                _setFunctions.TryGetValue(typeof(int).TypeHandle, out setDelegate);
                setDelegate(swigCPtr.Handle, DependencyProperty.getCPtr(dp).Handle, (int)Convert.ToInt64(value));
            }
            else if (_setFunctions.TryGetValue(dpType.TypeHandle, out setDelegate))
            {
                setDelegate(swigCPtr.Handle, DependencyProperty.getCPtr(dp).Handle, value);
            }
            else
            {
                Noesis_DependencySet_BaseComponent_(swigCPtr.Handle, DependencyProperty.getCPtr(dp).Handle,
                    Noesis.Extend.GetInstanceHandle(value).Handle);
            }
        }

        #region Getter and Setter map

        private delegate object GetDelegate(IntPtr cPtr, IntPtr dp);
        private static Dictionary<RuntimeTypeHandle, GetDelegate> _getFunctions = CreateGetFunctions();

        private delegate void SetDelegate(IntPtr cPtr, IntPtr dp, object value);
        private static Dictionary<RuntimeTypeHandle, SetDelegate> _setFunctions = CreateSetFunctions();

        private static Dictionary<RuntimeTypeHandle, GetDelegate> CreateGetFunctions()
        {
            Dictionary<RuntimeTypeHandle, GetDelegate> getFunctions = new Dictionary<RuntimeTypeHandle, GetDelegate>(46);

            getFunctions[typeof(bool).TypeHandle] = (cPtr, dp) =>
            {
                bool isNull;
                return Noesis_DependencyGet_Bool_(cPtr, dp, false, out isNull);
            };
            getFunctions[typeof(float).TypeHandle] = (cPtr, dp) =>
            {
                bool isNull;
                return Noesis_DependencyGet_Float_(cPtr, dp, false, out isNull);
            };
            getFunctions[typeof(double).TypeHandle] = (cPtr, dp) =>
            {
                bool isNull;
                return Noesis_DependencyGet_Double_(cPtr, dp, false, out isNull);
            };
            getFunctions[typeof(decimal).TypeHandle] = (cPtr, dp) =>
            {
                bool isNull;
                return (decimal)Noesis_DependencyGet_Double_(cPtr, dp, false, out isNull);
            };
            getFunctions[typeof(int).TypeHandle] = (cPtr, dp) =>
            {
                bool isNull;
                return Noesis_DependencyGet_Int_(cPtr, dp, false, out isNull);
            };
            getFunctions[typeof(long).TypeHandle] = (cPtr, dp) =>
            {
                bool isNull;
                return (long)Noesis_DependencyGet_Int_(cPtr, dp, false, out isNull);
            };
            getFunctions[typeof(uint).TypeHandle] = (cPtr, dp) =>
            {
                bool isNull;
                return Noesis_DependencyGet_UInt_(cPtr, dp, false, out isNull);
            };
            getFunctions[typeof(ulong).TypeHandle] = (cPtr, dp) =>
            {
                bool isNull;
                return (ulong)Noesis_DependencyGet_UInt_(cPtr, dp, false, out isNull);
            };
            getFunctions[typeof(char).TypeHandle] = (cPtr, dp) =>
            {
                bool isNull;
                return (char)Noesis_DependencyGet_UInt_(cPtr, dp, false, out isNull);
            };
            getFunctions[typeof(short).TypeHandle] = (cPtr, dp) =>
            {
                bool isNull;
                return Noesis_DependencyGet_Short_(cPtr, dp, false, out isNull);
            };
            getFunctions[typeof(sbyte).TypeHandle] = (cPtr, dp) =>
            {
                bool isNull;
                return (sbyte)Noesis_DependencyGet_Short_(cPtr, dp, false, out isNull);
            };
            getFunctions[typeof(ushort).TypeHandle] = (cPtr, dp) =>
            {
                bool isNull;
                return Noesis_DependencyGet_UShort_(cPtr, dp, false, out isNull);
            };
            getFunctions[typeof(byte).TypeHandle] = (cPtr, dp) =>
            {
                bool isNull;
                return (byte)Noesis_DependencyGet_UShort_(cPtr, dp, false, out isNull);
            };
            getFunctions[typeof(string).TypeHandle] = (cPtr, dp) =>
            {
                IntPtr ptr = Noesis_DependencyGet_String_(cPtr, dp);
                return Marshal.PtrToStringAnsi(ptr);
            };
            getFunctions[typeof(Noesis.Color).TypeHandle] = (cPtr, dp) =>
            {
                bool isNull;
                IntPtr ptr = Noesis_DependencyGet_Color_(cPtr, dp, false, out isNull);
                return Marshal.PtrToStructure<Noesis.Color>(ptr);
            };
            getFunctions[typeof(Noesis.Point).TypeHandle] = (cPtr, dp) =>
            {
                bool isNull;
                IntPtr ptr = Noesis_DependencyGet_Point_(cPtr, dp, false, out isNull);
                return Marshal.PtrToStructure<Noesis.Point>(ptr);
            };
            getFunctions[typeof(Noesis.Rect).TypeHandle] = (cPtr, dp) =>
            {
                bool isNull;
                IntPtr ptr = Noesis_DependencyGet_Rect_(cPtr, dp, false, out isNull);
                return Marshal.PtrToStructure<Noesis.Rect>(ptr);
            };
            getFunctions[typeof(Noesis.Size).TypeHandle] = (cPtr, dp) =>
            {
                bool isNull;
                IntPtr ptr = Noesis_DependencyGet_Size_(cPtr, dp, false, out isNull);
                return Marshal.PtrToStructure<Noesis.Size>(ptr);
            };
            getFunctions[typeof(Noesis.Thickness).TypeHandle] = (cPtr, dp) =>
            {
                bool isNull;
                IntPtr ptr = Noesis_DependencyGet_Thickness_(cPtr, dp, false, out isNull);
                return Marshal.PtrToStructure<Noesis.Thickness>(ptr);
            };
            getFunctions[typeof(Noesis.CornerRadius).TypeHandle] = (cPtr, dp) =>
            {
                bool isNull;
                IntPtr ptr = Noesis_DependencyGet_CornerRadius_(cPtr, dp, false, out isNull);
                return Marshal.PtrToStructure<Noesis.CornerRadius>(ptr);
            };
            getFunctions[typeof(System.TimeSpan).TypeHandle] = (cPtr, dp) =>
            {
                bool isNull;
                IntPtr ptr = Noesis_DependencyGet_TimeSpan_(cPtr, dp, false, out isNull);
                return (System.TimeSpan)Marshal.PtrToStructure<Noesis.TimeSpanStruct>(ptr);
            };
            getFunctions[typeof(Noesis.Duration).TypeHandle] = (cPtr, dp) =>
            {
                bool isNull;
                IntPtr ptr = Noesis_DependencyGet_Duration_(cPtr, dp, false, out isNull);
                return Marshal.PtrToStructure<Noesis.Duration>(ptr);
            };
            getFunctions[typeof(Noesis.KeyTime).TypeHandle] = (cPtr, dp) =>
            {
                bool isNull;
                IntPtr ptr = Noesis_DependencyGet_KeyTime_(cPtr, dp, false, out isNull);
                return Marshal.PtrToStructure<Noesis.KeyTime>(ptr);
            };

            getFunctions[typeof(bool?).TypeHandle] = (cPtr, dp) =>
            {
                bool isNull;
                bool val = Noesis_DependencyGet_Bool_(cPtr, dp, true, out isNull);
                return isNull ? null : (bool?)val;
            };
            getFunctions[typeof(float?).TypeHandle] = (cPtr, dp) =>
            {
                bool isNull;
                float val = Noesis_DependencyGet_Float_(cPtr, dp, true, out isNull);
                return isNull ? null : (float?)val;
            };
            getFunctions[typeof(double?).TypeHandle] = (cPtr, dp) =>
            {
                bool isNull;
                double val = Noesis_DependencyGet_Double_(cPtr, dp, true, out isNull);
                return isNull ? null : (double?)val;
            };
            getFunctions[typeof(decimal?).TypeHandle] = (cPtr, dp) =>
            {
                bool isNull;
                double val = Noesis_DependencyGet_Double_(cPtr, dp, true, out isNull);
                return isNull ? null : (decimal?)(decimal)val;
            };
            getFunctions[typeof(int?).TypeHandle] = (cPtr, dp) =>
            {
                bool isNull;
                int val = Noesis_DependencyGet_Int_(cPtr, dp, true, out isNull);
                return isNull ? null : (int?)val;
            };
            getFunctions[typeof(long?).TypeHandle] = (cPtr, dp) =>
            {
                bool isNull;
                int val = Noesis_DependencyGet_Int_(cPtr, dp, true, out isNull);
                return isNull ? null : (long?)(long)val;
            };
            getFunctions[typeof(uint?).TypeHandle] = (cPtr, dp) =>
            {
                bool isNull;
                uint val = Noesis_DependencyGet_UInt_(cPtr, dp, true, out isNull);
                return isNull ? null : (uint?)val;
            };
            getFunctions[typeof(ulong?).TypeHandle] = (cPtr, dp) =>
            {
                bool isNull;
                uint val = Noesis_DependencyGet_UInt_(cPtr, dp, true, out isNull);
                return isNull ? null : (ulong?)(ulong)val;
            };
            getFunctions[typeof(char?).TypeHandle] = (cPtr, dp) =>
            {
                bool isNull;
                uint val = Noesis_DependencyGet_UInt_(cPtr, dp, true, out isNull);
                return isNull ? null : (char?)(char)val;
            };
            getFunctions[typeof(short?).TypeHandle] = (cPtr, dp) =>
            {
                bool isNull;
                short val = Noesis_DependencyGet_Short_(cPtr, dp, true, out isNull);
                return isNull ? null : (short?)val;
            };
            getFunctions[typeof(sbyte?).TypeHandle] = (cPtr, dp) =>
            {
                bool isNull;
                short val = Noesis_DependencyGet_Short_(cPtr, dp, true, out isNull);
                return isNull ? null : (sbyte?)(sbyte)val;
            };
            getFunctions[typeof(ushort?).TypeHandle] = (cPtr, dp) =>
            {
                bool isNull;
                ushort val = Noesis_DependencyGet_UShort_(cPtr, dp, true, out isNull);
                return isNull ? null : (ushort?)val;
            };
            getFunctions[typeof(byte?).TypeHandle] = (cPtr, dp) =>
            {
                bool isNull;
                ushort val = Noesis_DependencyGet_UShort_(cPtr, dp, true, out isNull);
                return isNull ? null : (byte?)(byte)val;
            };
            getFunctions[typeof(Noesis.Color?).TypeHandle] = (cPtr, dp) =>
            {
                bool isNull;
                IntPtr ptr = Noesis_DependencyGet_Color_(cPtr, dp, true, out isNull);
                return isNull ? null : (Noesis.Color?)Marshal.PtrToStructure<Noesis.Color>(ptr);
            };
            getFunctions[typeof(Noesis.Point?).TypeHandle] = (cPtr, dp) =>
            {
                bool isNull;
                IntPtr ptr = Noesis_DependencyGet_Point_(cPtr, dp, true, out isNull);
                return isNull ? null : (Noesis.Point?)Marshal.PtrToStructure<Noesis.Point>(ptr);
            };
            getFunctions[typeof(Noesis.Rect?).TypeHandle] = (cPtr, dp) =>
            {
                bool isNull;
                IntPtr ptr = Noesis_DependencyGet_Rect_(cPtr, dp, true, out isNull);
                return isNull ? null : (Noesis.Rect?)Marshal.PtrToStructure<Noesis.Rect>(ptr);
            };
            getFunctions[typeof(Noesis.Size?).TypeHandle] = (cPtr, dp) =>
            {
                bool isNull;
                IntPtr ptr = Noesis_DependencyGet_Size_(cPtr, dp, true, out isNull);
                return isNull ? null : (Noesis.Size?)Marshal.PtrToStructure<Noesis.Size>(ptr);
            };
            getFunctions[typeof(Noesis.Thickness?).TypeHandle] = (cPtr, dp) =>
            {
                bool isNull;
                IntPtr ptr = Noesis_DependencyGet_Thickness_(cPtr, dp, true, out isNull);
                return isNull ? null : (Noesis.Thickness?)Marshal.PtrToStructure<Noesis.Thickness>(ptr);
            };
            getFunctions[typeof(Noesis.CornerRadius?).TypeHandle] = (cPtr, dp) =>
            {
                bool isNull;
                IntPtr ptr = Noesis_DependencyGet_CornerRadius_(cPtr, dp, true, out isNull);
                return isNull ? null : (Noesis.CornerRadius?)Marshal.PtrToStructure<Noesis.CornerRadius>(ptr);
            };
            getFunctions[typeof(System.TimeSpan?).TypeHandle] = (cPtr, dp) =>
            {
                bool isNull;
                IntPtr ptr = Noesis_DependencyGet_TimeSpan_(cPtr, dp, true, out isNull);
                return isNull ? null : (System.TimeSpan?)(System.TimeSpan)Marshal.PtrToStructure<Noesis.TimeSpanStruct>(ptr);
            };
            getFunctions[typeof(Noesis.Duration?).TypeHandle] = (cPtr, dp) =>
            {
                bool isNull;
                IntPtr ptr = Noesis_DependencyGet_Duration_(cPtr, dp, true, out isNull);
                return isNull ? null : (Noesis.Duration?)Marshal.PtrToStructure<Noesis.Duration>(ptr);
            };
            getFunctions[typeof(Noesis.KeyTime?).TypeHandle] = (cPtr, dp) =>
            {
                bool isNull;
                IntPtr ptr = Noesis_DependencyGet_KeyTime_(cPtr, dp, true, out isNull);
                return isNull ? null : (Noesis.KeyTime?)Marshal.PtrToStructure<Noesis.KeyTime>(ptr);
            };
            getFunctions[typeof(Type).TypeHandle] = (cPtr, dp) =>
            {
                IntPtr ptr = Noesis_DependencyGet_BaseComponent_(cPtr, dp);
                if (ptr != IntPtr.Zero)
                {
                    ResourceKeyType key = new ResourceKeyType(ptr, false);
                    return key.Type;
                }
                else
                {
                    return null;
                }
            };

            return getFunctions;
        }

        private static Dictionary<RuntimeTypeHandle, SetDelegate> CreateSetFunctions()
        {
            Dictionary<RuntimeTypeHandle, SetDelegate> setFunctions = new Dictionary<RuntimeTypeHandle, SetDelegate>(46);

            setFunctions[typeof(bool).TypeHandle] = (cPtr, dp, value) =>
            {
                Noesis_DependencySet_Bool_(cPtr, dp, (bool)value, false, false);
            };
            setFunctions[typeof(float).TypeHandle] = (cPtr, dp, value) =>
            {
                Noesis_DependencySet_Float_(cPtr, dp, (float)value, false, false);
            };
            setFunctions[typeof(double).TypeHandle] = (cPtr, dp, value) =>
            {
                Noesis_DependencySet_Double_(cPtr, dp, (double)value, false, false);
            };
            setFunctions[typeof(decimal).TypeHandle] = (cPtr, dp, value) =>
            {
                Noesis_DependencySet_Double_(cPtr, dp, (double)(decimal)value, false, false);
            };
            setFunctions[typeof(int).TypeHandle] = (cPtr, dp, value) =>
            {
                Noesis_DependencySet_Int_(cPtr, dp, (int)value, false, false);
            };
            setFunctions[typeof(long).TypeHandle] = (cPtr, dp, value) =>
            {
                Noesis_DependencySet_Int_(cPtr, dp, (int)(long)value, false, false);
            };
            setFunctions[typeof(uint).TypeHandle] = (cPtr, dp, value) =>
            {
                Noesis_DependencySet_UInt_(cPtr, dp, (uint)value, false, false);
            };
            setFunctions[typeof(ulong).TypeHandle] = (cPtr, dp, value) =>
            {
                Noesis_DependencySet_UInt_(cPtr, dp, (uint)(ulong)value, false, false);
            };
            setFunctions[typeof(char).TypeHandle] = (cPtr, dp, value) =>
            {
                Noesis_DependencySet_UInt_(cPtr, dp, (uint)(char)value, false, false);
            };
            setFunctions[typeof(short).TypeHandle] = (cPtr, dp, value) =>
            {
                Noesis_DependencySet_Short_(cPtr, dp, (short)value, false, false);
            };
            setFunctions[typeof(sbyte).TypeHandle] = (cPtr, dp, value) =>
            {
                Noesis_DependencySet_Short_(cPtr, dp, (short)(sbyte)value, false, false);
            };
            setFunctions[typeof(ushort).TypeHandle] = (cPtr, dp, value) =>
            {
                Noesis_DependencySet_UShort_(cPtr, dp, (ushort)value, false, false);
            };
            setFunctions[typeof(byte).TypeHandle] = (cPtr, dp, value) =>
            {
                Noesis_DependencySet_UShort_(cPtr, dp, (ushort)(byte)value, false, false);
            };
            setFunctions[typeof(string).TypeHandle] = (cPtr, dp, value) =>
            {
                Noesis_DependencySet_String_(cPtr, dp, value == null ? string.Empty : value.ToString());
            };
            setFunctions[typeof(Noesis.Color).TypeHandle] = (cPtr, dp, value) =>
            {
                Noesis_DependencySet_Color_(cPtr, dp, (Noesis.Color)value, false, false);
            };
            setFunctions[typeof(Noesis.Point).TypeHandle] = (cPtr, dp, value) =>
            {
                Noesis_DependencySet_Point_(cPtr, dp, (Noesis.Point)value, false, false);
            };
            setFunctions[typeof(Noesis.Rect).TypeHandle] = (cPtr, dp, value) =>
            {
                Noesis_DependencySet_Rect_(cPtr, dp, (Noesis.Rect)value, false, false);
            };
            setFunctions[typeof(Noesis.Size).TypeHandle] = (cPtr, dp, value) =>
            {
                Noesis_DependencySet_Size_(cPtr, dp, (Noesis.Size)value, false, false);
            };
            setFunctions[typeof(Noesis.Thickness).TypeHandle] = (cPtr, dp, value) =>
            {
                Noesis_DependencySet_Thickness_(cPtr, dp, (Noesis.Thickness)value, false, false);
            };
            setFunctions[typeof(Noesis.CornerRadius).TypeHandle] = (cPtr, dp, value) =>
            {
                Noesis_DependencySet_CornerRadius_(cPtr, dp, (Noesis.CornerRadius)value, false, false);
            };
            setFunctions[typeof(System.TimeSpan).TypeHandle] = (cPtr, dp, value) =>
            {
                Noesis_DependencySet_TimeSpan_(cPtr, dp, (Noesis.TimeSpanStruct)((System.TimeSpan)value), false, false);
            };
            setFunctions[typeof(Noesis.Duration).TypeHandle] = (cPtr, dp, value) =>
            {
                Noesis_DependencySet_Duration_(cPtr, dp, (Noesis.Duration)value, false, false);
            };
            setFunctions[typeof(Noesis.KeyTime).TypeHandle] = (cPtr, dp, value) =>
            {
                Noesis_DependencySet_KeyTime_(cPtr, dp, (Noesis.KeyTime)value, false, false);
            };
            setFunctions[typeof(bool?).TypeHandle] = (cPtr, dp, value) =>
            {
                if (value == null)
                {
                    Noesis_DependencySet_Bool_(cPtr, dp, default(bool), true, true);
                }
                else
                {
                    Noesis_DependencySet_Bool_(cPtr, dp, (bool)value, true, false);
                }
            };
            setFunctions[typeof(float?).TypeHandle] = (cPtr, dp, value) =>
            {
                if (value == null)
                {
                    Noesis_DependencySet_Float_(cPtr, dp, default(float), true, true);
                }
                else
                {
                    Noesis_DependencySet_Float_(cPtr, dp, (float)value, true, false);
                }
            };
            setFunctions[typeof(double?).TypeHandle] = (cPtr, dp, value) =>
            {
                if (value == null)
                {
                    Noesis_DependencySet_Double_(cPtr, dp, default(double), true, true);
                }
                else
                {
                    Noesis_DependencySet_Double_(cPtr, dp, (double)value, true, false);
                }
            };
            setFunctions[typeof(decimal?).TypeHandle] = (cPtr, dp, value) =>
            {
                if (value == null)
                {
                    Noesis_DependencySet_Double_(cPtr, dp, default(double), true, true);
                }
                else
                {
                    Noesis_DependencySet_Double_(cPtr, dp, (double)(decimal)value, true, false);
                }
            };
            setFunctions[typeof(int?).TypeHandle] = (cPtr, dp, value) =>
            {
                if (value == null)
                {
                    Noesis_DependencySet_Int_(cPtr, dp, default(int), true, true);
                }
                else
                {
                    Noesis_DependencySet_Int_(cPtr, dp, (int)value, true, false);
                }
            };
            setFunctions[typeof(long?).TypeHandle] = (cPtr, dp, value) =>
            {
                if (value == null)
                {
                    Noesis_DependencySet_Int_(cPtr, dp, default(int), true, true);
                }
                else
                {
                    Noesis_DependencySet_Int_(cPtr, dp, (int)(long)value, true, false);
                }
            };
            setFunctions[typeof(uint?).TypeHandle] = (cPtr, dp, value) =>
            {
                if (value == null)
                {
                    Noesis_DependencySet_UInt_(cPtr, dp, default(uint), true, true);
                }
                else
                {
                    Noesis_DependencySet_UInt_(cPtr, dp, (uint)value, true, false);
                }
            };
            setFunctions[typeof(ulong?).TypeHandle] = (cPtr, dp, value) =>
            {
                if (value == null)
                {
                    Noesis_DependencySet_UInt_(cPtr, dp, default(uint), true, true);
                }
                else
                {
                    Noesis_DependencySet_UInt_(cPtr, dp, (uint)(ulong)value, true, false);
                }
            };
            setFunctions[typeof(char?).TypeHandle] = (cPtr, dp, value) =>
            {
                if (value == null)
                {
                    Noesis_DependencySet_UInt_(cPtr, dp, default(uint), true, true);
                }
                else
                {
                    Noesis_DependencySet_UInt_(cPtr, dp, (uint)(char)value, true, false);
                }
            };
            setFunctions[typeof(short?).TypeHandle] = (cPtr, dp, value) =>
            {
                if (value == null)
                {
                    Noesis_DependencySet_Short_(cPtr, dp, default(short), true, true);
                }
                else
                {
                    Noesis_DependencySet_Short_(cPtr, dp, (short)value, true, false);
                }
            };
            setFunctions[typeof(sbyte?).TypeHandle] = (cPtr, dp, value) =>
            {
                if (value == null)
                {
                    Noesis_DependencySet_Short_(cPtr, dp, default(short), true, true);
                }
                else
                {
                    Noesis_DependencySet_Short_(cPtr, dp, (short)(sbyte)value, true, false);
                }
            };
            setFunctions[typeof(ushort?).TypeHandle] = (cPtr, dp, value) =>
            {
                if (value == null)
                {
                    Noesis_DependencySet_UShort_(cPtr, dp, default(ushort), true, true);
                }
                else
                {
                    Noesis_DependencySet_UShort_(cPtr, dp, (ushort)value, true, false);
                }
            };
            setFunctions[typeof(byte?).TypeHandle] = (cPtr, dp, value) =>
            {
                if (value == null)
                {
                    Noesis_DependencySet_UShort_(cPtr, dp, default(ushort), true, true);
                }
                else
                {
                    Noesis_DependencySet_UShort_(cPtr, dp, (ushort)(byte)value, true, false);
                }
            };
            setFunctions[typeof(Noesis.Color?).TypeHandle] = (cPtr, dp, value) =>
            {
                if (value == null)
                {
                    Noesis_DependencySet_Color_(cPtr, dp, default(Noesis.Color), true, true);
                }
                else
                {
                    Noesis_DependencySet_Color_(cPtr, dp, (Noesis.Color)value, true, false);
                }
            };
            setFunctions[typeof(Noesis.Point?).TypeHandle] = (cPtr, dp, value) =>
            {
                if (value == null)
                {
                    Noesis_DependencySet_Point_(cPtr, dp, default(Noesis.Point), true, true);
                }
                else
                {
                    Noesis_DependencySet_Point_(cPtr, dp, (Noesis.Point)value, true, false);
                }
            };
            setFunctions[typeof(Noesis.Rect?).TypeHandle] = (cPtr, dp, value) =>
            {
                if (value == null)
                {
                    Noesis_DependencySet_Rect_(cPtr, dp, default(Noesis.Rect), true, true);
                }
                else
                {
                    Noesis_DependencySet_Rect_(cPtr, dp, (Noesis.Rect)value, true, false);
                }
            };
            setFunctions[typeof(Noesis.Size?).TypeHandle] = (cPtr, dp, value) =>
            {
                if (value == null)
                {
                    Noesis_DependencySet_Size_(cPtr, dp, default(Noesis.Size), true, true);
                }
                else
                {
                    Noesis_DependencySet_Size_(cPtr, dp, (Noesis.Size)value, true, false);
                }
            };
            setFunctions[typeof(Noesis.Thickness?).TypeHandle] = (cPtr, dp, value) =>
            {
                if (value == null)
                {
                    Noesis_DependencySet_Thickness_(cPtr, dp, default(Noesis.Thickness), true, true);
                }
                else
                {
                    Noesis_DependencySet_Thickness_(cPtr, dp, (Noesis.Thickness)value, true, false);
                }
            };
            setFunctions[typeof(Noesis.CornerRadius?).TypeHandle] = (cPtr, dp, value) =>
            {
                if (value == null)
                {
                    Noesis_DependencySet_CornerRadius_(cPtr, dp, default(Noesis.CornerRadius), true, true);
                }
                else
                {
                    Noesis_DependencySet_CornerRadius_(cPtr, dp, (Noesis.CornerRadius)value, true, false);
                }
            };
            setFunctions[typeof(System.TimeSpan?).TypeHandle] = (cPtr, dp, value) =>
            {
                if (value == null)
                {
                    Noesis_DependencySet_TimeSpan_(cPtr, dp, default(Noesis.TimeSpanStruct), true, true);
                }
                else
                {
                    Noesis_DependencySet_TimeSpan_(cPtr, dp, (Noesis.TimeSpanStruct)((System.TimeSpan)value), true, false);
                }
            };
            setFunctions[typeof(Noesis.Duration?).TypeHandle] = (cPtr, dp, value) =>
            {
                if (value == null)
                {
                    Noesis_DependencySet_Duration_(cPtr, dp, default(Noesis.Duration), true, true);
                }
                else
                {
                    Noesis_DependencySet_Duration_(cPtr, dp, (Noesis.Duration)value, true, false);
                }
            };
            setFunctions[typeof(Noesis.KeyTime?).TypeHandle] = (cPtr, dp, value) =>
            {
                if (value == null)
                {
                    Noesis_DependencySet_KeyTime_(cPtr, dp, default(Noesis.KeyTime), true, true);
                }
                else
                {
                    Noesis_DependencySet_KeyTime_(cPtr, dp, (Noesis.KeyTime)value, true, false);
                }
            };
            setFunctions[typeof(Type).TypeHandle] = (cPtr, dp, value) =>
            {
                ResourceKeyType key = Noesis.Extend.GetResourceKeyType((Type)value);
                Noesis_DependencySet_BaseComponent_(cPtr, dp, Noesis.Extend.GetInstanceHandle(key).Handle);
            };

            return setFunctions;
        }

        #endregion

        #region Imports

        private static void CheckProperty(IntPtr dependencyObject, IntPtr dependencyProperty,
            string msg)
        {
            if (dependencyObject == IntPtr.Zero)
            {
                throw new Exception("Can't " + msg + " value, DependencyObject is null");
            }

            if (dependencyProperty == IntPtr.Zero)
            {
                throw new Exception("Can't " + msg + " value, DependencyProperty is null");
            }
        }

        private static bool Noesis_DependencyGet_Bool_(IntPtr dependencyObject, IntPtr dependencyProperty,
            bool isNullable, [MarshalAs(UnmanagedType.U1)]out bool isNull)
        {
            CheckProperty(dependencyObject, dependencyProperty, "get");
            bool result = Noesis_DependencyGet_Bool(dependencyObject, dependencyProperty, isNullable, out isNull);
            #if UNITY_EDITOR || NOESIS_API
            Error.Check();
            #endif
            return result;
        }

        private static float Noesis_DependencyGet_Float_(IntPtr dependencyObject, IntPtr dependencyProperty,
            bool isNullable, [MarshalAs(UnmanagedType.U1)]out bool isNull)
        {
            CheckProperty(dependencyObject, dependencyProperty, "get");
            float result = Noesis_DependencyGet_Float(dependencyObject, dependencyProperty, isNullable, out isNull);
            #if UNITY_EDITOR || NOESIS_API
            Error.Check();
            #endif
            return result;
        }

        private static double Noesis_DependencyGet_Double_(IntPtr dependencyObject, IntPtr dependencyProperty,
            bool isNullable, [MarshalAs(UnmanagedType.U1)]out bool isNull)
        {
            CheckProperty(dependencyObject, dependencyProperty, "get");
            double result = Noesis_DependencyGet_Double(dependencyObject, dependencyProperty, isNullable, out isNull);
            #if UNITY_EDITOR || NOESIS_API
            Error.Check();
            #endif
            return result;
        }

        private static int Noesis_DependencyGet_Int_(IntPtr dependencyObject, IntPtr dependencyProperty,
            bool isNullable, [MarshalAs(UnmanagedType.U1)]out bool isNull)
        {
            CheckProperty(dependencyObject, dependencyProperty, "get");
            int result = Noesis_DependencyGet_Int(dependencyObject, dependencyProperty, isNullable, out isNull);
            #if UNITY_EDITOR || NOESIS_API
            Error.Check();
            #endif
            return result;
        }

        private static uint Noesis_DependencyGet_UInt_(IntPtr dependencyObject, IntPtr dependencyProperty,
            bool isNullable, [MarshalAs(UnmanagedType.U1)]out bool isNull)
        {
            CheckProperty(dependencyObject, dependencyProperty, "get");
            uint result = Noesis_DependencyGet_UInt(dependencyObject, dependencyProperty, isNullable, out isNull);
            #if UNITY_EDITOR || NOESIS_API
            Error.Check();
            #endif
            return result;
        }

        private static short Noesis_DependencyGet_Short_(IntPtr dependencyObject, IntPtr dependencyProperty,
            bool isNullable, [MarshalAs(UnmanagedType.U1)]out bool isNull)
        {
            CheckProperty(dependencyObject, dependencyProperty, "get");
            short result = Noesis_DependencyGet_Short(dependencyObject, dependencyProperty, isNullable, out isNull);
            #if UNITY_EDITOR || NOESIS_API
            Error.Check();
            #endif
            return result;
        }

        private static ushort Noesis_DependencyGet_UShort_(IntPtr dependencyObject, IntPtr dependencyProperty,
            bool isNullable, [MarshalAs(UnmanagedType.U1)]out bool isNull)
        {
            CheckProperty(dependencyObject, dependencyProperty, "get");
            ushort result = Noesis_DependencyGet_UShort(dependencyObject, dependencyProperty, isNullable, out isNull);
            #if UNITY_EDITOR || NOESIS_API
            Error.Check();
            #endif
            return result;
        }

        private static IntPtr Noesis_DependencyGet_String_(IntPtr dependencyObject, IntPtr dependencyProperty)
        {
            CheckProperty(dependencyObject, dependencyProperty, "get");
            IntPtr result = Noesis_DependencyGet_String(dependencyObject, dependencyProperty);
            #if UNITY_EDITOR || NOESIS_API
            Error.Check();
            #endif
            return result;
        }

        private static IntPtr Noesis_DependencyGet_Color_(IntPtr dependencyObject, IntPtr dependencyProperty,
            bool isNullable, [MarshalAs(UnmanagedType.U1)]out bool isNull)
        {
            CheckProperty(dependencyObject, dependencyProperty, "get");
            IntPtr result = Noesis_DependencyGet_Color(dependencyObject, dependencyProperty, isNullable, out isNull);
            #if UNITY_EDITOR || NOESIS_API
            Error.Check();
            #endif
            return result;
        }

        private static IntPtr Noesis_DependencyGet_Point_(IntPtr dependencyObject, IntPtr dependencyProperty,
            bool isNullable, [MarshalAs(UnmanagedType.U1)]out bool isNull)
        {
            CheckProperty(dependencyObject, dependencyProperty, "get");
            IntPtr result = Noesis_DependencyGet_Point(dependencyObject, dependencyProperty, isNullable, out isNull);
            #if UNITY_EDITOR || NOESIS_API
            Error.Check();
            #endif
            return result;
        }

        private static IntPtr Noesis_DependencyGet_Rect_(IntPtr dependencyObject, IntPtr dependencyProperty,
            bool isNullable, [MarshalAs(UnmanagedType.U1)]out bool isNull)
        {
            CheckProperty(dependencyObject, dependencyProperty, "get");
            IntPtr result = Noesis_DependencyGet_Rect(dependencyObject, dependencyProperty, isNullable, out isNull);
            #if UNITY_EDITOR || NOESIS_API
            Error.Check();
            #endif
            return result;
        }

        private static IntPtr Noesis_DependencyGet_Size_(IntPtr dependencyObject, IntPtr dependencyProperty,
            bool isNullable, [MarshalAs(UnmanagedType.U1)]out bool isNull)
        {
            CheckProperty(dependencyObject, dependencyProperty, "get");
            IntPtr result = Noesis_DependencyGet_Size(dependencyObject, dependencyProperty, isNullable, out isNull);
            #if UNITY_EDITOR || NOESIS_API
            Error.Check();
            #endif
            return result;
        }

        private static IntPtr Noesis_DependencyGet_Thickness_(IntPtr dependencyObject, IntPtr dependencyProperty,
            bool isNullable, [MarshalAs(UnmanagedType.U1)]out bool isNull)
        {
            CheckProperty(dependencyObject, dependencyProperty, "get");
            IntPtr result = Noesis_DependencyGet_Thickness(dependencyObject, dependencyProperty, isNullable, out isNull);
            #if UNITY_EDITOR || NOESIS_API
            Error.Check();
            #endif
            return result;
        }

        private static IntPtr Noesis_DependencyGet_CornerRadius_(IntPtr dependencyObject, IntPtr dependencyProperty,
            bool isNullable, [MarshalAs(UnmanagedType.U1)]out bool isNull)
        {
            CheckProperty(dependencyObject, dependencyProperty, "get");
            IntPtr result = Noesis_DependencyGet_CornerRadius(dependencyObject, dependencyProperty, isNullable, out isNull);
            #if UNITY_EDITOR || NOESIS_API
            Error.Check();
            #endif
            return result;
        }

        private static IntPtr Noesis_DependencyGet_TimeSpan_(IntPtr dependencyObject, IntPtr dependencyProperty,
            bool isNullable, [MarshalAs(UnmanagedType.U1)]out bool isNull)
        {
            CheckProperty(dependencyObject, dependencyProperty, "get");
            IntPtr result = Noesis_DependencyGet_TimeSpan(dependencyObject, dependencyProperty, isNullable, out isNull);
            #if UNITY_EDITOR || NOESIS_API
            Error.Check();
            #endif
            return result;
        }

        private static IntPtr Noesis_DependencyGet_Duration_(IntPtr dependencyObject, IntPtr dependencyProperty,
            bool isNullable, [MarshalAs(UnmanagedType.U1)]out bool isNull)
        {
            CheckProperty(dependencyObject, dependencyProperty, "get");
            IntPtr result = Noesis_DependencyGet_Duration(dependencyObject, dependencyProperty, isNullable, out isNull);
            #if UNITY_EDITOR || NOESIS_API
            Error.Check();
            #endif
            return result;
        }

        private static IntPtr Noesis_DependencyGet_KeyTime_(IntPtr dependencyObject, IntPtr dependencyProperty,
            bool isNullable, [MarshalAs(UnmanagedType.U1)]out bool isNull)
        {
            CheckProperty(dependencyObject, dependencyProperty, "get");
            IntPtr result = Noesis_DependencyGet_KeyTime(dependencyObject, dependencyProperty, isNullable, out isNull);
            #if UNITY_EDITOR || NOESIS_API
            Error.Check();
            #endif
            return result;
        }

        private static IntPtr Noesis_DependencyGet_BaseComponent_(IntPtr dependencyObject, IntPtr dependencyProperty)
        {
            CheckProperty(dependencyObject, dependencyProperty, "get");
            IntPtr result = Noesis_DependencyGet_BaseComponent(dependencyObject, dependencyProperty);
            #if UNITY_EDITOR || NOESIS_API
            Error.Check();
            #endif
            return result;
        }

        private static void Noesis_DependencySet_Bool_(IntPtr dependencyObject, IntPtr dependencyProperty,
            bool val, bool isNullable, bool isNull)
        {
            CheckProperty(dependencyObject, dependencyProperty, "set");
            Noesis_DependencySet_Bool(dependencyObject, dependencyProperty, val, isNullable, isNull);
            #if UNITY_EDITOR || NOESIS_API
            Error.Check();
            #endif
        }

        private static void Noesis_DependencySet_Float_(IntPtr dependencyObject, IntPtr dependencyProperty,
            float val, bool isNullable, bool isNull)
        {
            CheckProperty(dependencyObject, dependencyProperty, "set");
            Noesis_DependencySet_Float(dependencyObject, dependencyProperty, val, isNullable, isNull);
            #if UNITY_EDITOR || NOESIS_API
            Error.Check();
            #endif
        }

        private static void Noesis_DependencySet_Double_(IntPtr dependencyObject, IntPtr dependencyProperty,
            double val, bool isNullable, bool isNull)
        {
            CheckProperty(dependencyObject, dependencyProperty, "set");
            Noesis_DependencySet_Double(dependencyObject, dependencyProperty, val, isNullable, isNull);
            #if UNITY_EDITOR || NOESIS_API
            Error.Check();
            #endif
        }

        private static void Noesis_DependencySet_Int_(IntPtr dependencyObject, IntPtr dependencyProperty,
            int val, bool isNullable, bool isNull)
        {
            CheckProperty(dependencyObject, dependencyProperty, "set");
            Noesis_DependencySet_Int(dependencyObject, dependencyProperty, val, isNullable, isNull);
            #if UNITY_EDITOR || NOESIS_API
            Error.Check();
            #endif
        }

        private static void Noesis_DependencySet_UInt_(IntPtr dependencyObject, IntPtr dependencyProperty,
            uint val, bool isNullable, bool isNull)
        {
            CheckProperty(dependencyObject, dependencyProperty, "set");
            Noesis_DependencySet_UInt(dependencyObject, dependencyProperty, val, isNullable, isNull);
            #if UNITY_EDITOR || NOESIS_API
            Error.Check();
            #endif
        }

        private static void Noesis_DependencySet_Short_(IntPtr dependencyObject, IntPtr dependencyProperty,
            short val, bool isNullable, bool isNull)
        {
            CheckProperty(dependencyObject, dependencyProperty, "set");
            Noesis_DependencySet_Short(dependencyObject, dependencyProperty, val, isNullable, isNull);
            #if UNITY_EDITOR || NOESIS_API
            Error.Check();
            #endif
        }

        private static void Noesis_DependencySet_UShort_(IntPtr dependencyObject, IntPtr dependencyProperty,
            ushort val, bool isNullable, bool isNull)
        {
            CheckProperty(dependencyObject, dependencyProperty, "set");
            Noesis_DependencySet_UShort(dependencyObject, dependencyProperty, val, isNullable, isNull);
            #if UNITY_EDITOR || NOESIS_API
            Error.Check();
            #endif
        }

        private static void Noesis_DependencySet_String_(IntPtr dependencyObject, IntPtr dependencyProperty,
            string val)
        {
            CheckProperty(dependencyObject, dependencyProperty, "set");
            Noesis_DependencySet_String(dependencyObject, dependencyProperty, val);
            #if UNITY_EDITOR || NOESIS_API
            Error.Check();
            #endif
        }

        private static void Noesis_DependencySet_Color_(IntPtr dependencyObject, IntPtr dependencyProperty,
            Noesis.Color val, bool isNullable, bool isNull)
        {
            CheckProperty(dependencyObject, dependencyProperty, "set");
            Noesis_DependencySet_Color(dependencyObject, dependencyProperty, ref val, isNullable, isNull);
            #if UNITY_EDITOR || NOESIS_API
            Error.Check();
            #endif
        }

        private static void Noesis_DependencySet_Point_(IntPtr dependencyObject, IntPtr dependencyProperty,
            Noesis.Point val, bool isNullable, bool isNull)
        {
            CheckProperty(dependencyObject, dependencyProperty, "set");
            Noesis_DependencySet_Point(dependencyObject, dependencyProperty, ref val, isNullable, isNull);
            #if UNITY_EDITOR || NOESIS_API
            Error.Check();
            #endif
        }

        private static void Noesis_DependencySet_Rect_(IntPtr dependencyObject, IntPtr dependencyProperty,
            Noesis.Rect val, bool isNullable, bool isNull)
        {
            CheckProperty(dependencyObject, dependencyProperty, "set");
            Noesis_DependencySet_Rect(dependencyObject, dependencyProperty, ref val, isNullable, isNull);
            #if UNITY_EDITOR || NOESIS_API
            Error.Check();
            #endif
        }

        private static void Noesis_DependencySet_Size_(IntPtr dependencyObject, IntPtr dependencyProperty,
            Noesis.Size val, bool isNullable, bool isNull)
        {
            CheckProperty(dependencyObject, dependencyProperty, "set");
            Noesis_DependencySet_Size(dependencyObject, dependencyProperty, ref val, isNullable, isNull);
            #if UNITY_EDITOR || NOESIS_API
            Error.Check();
            #endif
        }

        private static void Noesis_DependencySet_Thickness_(IntPtr dependencyObject, IntPtr dependencyProperty,
            Noesis.Thickness val, bool isNullable, bool isNull)
        {
            CheckProperty(dependencyObject, dependencyProperty, "set");
            Noesis_DependencySet_Thickness(dependencyObject, dependencyProperty, ref val, isNullable, isNull);
            #if UNITY_EDITOR || NOESIS_API
            Error.Check();
            #endif
        }

        private static void Noesis_DependencySet_CornerRadius_(IntPtr dependencyObject, IntPtr dependencyProperty,
            Noesis.CornerRadius val, bool isNullable, bool isNull)
        {
            CheckProperty(dependencyObject, dependencyProperty, "set");
            Noesis_DependencySet_CornerRadius(dependencyObject, dependencyProperty, ref val, isNullable, isNull);
            #if UNITY_EDITOR || NOESIS_API
            Error.Check();
            #endif
        }

        private static void Noesis_DependencySet_TimeSpan_(IntPtr dependencyObject, IntPtr dependencyProperty,
            Noesis.TimeSpanStruct val, bool isNullable, bool isNull)
        {
            CheckProperty(dependencyObject, dependencyProperty, "set");
            Noesis_DependencySet_TimeSpan(dependencyObject, dependencyProperty, ref val, isNullable, isNull);
            #if UNITY_EDITOR || NOESIS_API
            Error.Check();
            #endif
        }

        private static void Noesis_DependencySet_Duration_(IntPtr dependencyObject, IntPtr dependencyProperty,
            Noesis.Duration val, bool isNullable, bool isNull)
        {
            CheckProperty(dependencyObject, dependencyProperty, "set");
            Noesis_DependencySet_Duration(dependencyObject, dependencyProperty, ref val, isNullable, isNull);
            #if UNITY_EDITOR || NOESIS_API
            Error.Check();
            #endif
        }

        private static void Noesis_DependencySet_KeyTime_(IntPtr dependencyObject, IntPtr dependencyProperty,
            Noesis.KeyTime val, bool isNullable, bool isNull)
        {
            CheckProperty(dependencyObject, dependencyProperty, "set");
            Noesis_DependencySet_KeyTime(dependencyObject, dependencyProperty, ref val, isNullable, isNull);
            #if UNITY_EDITOR || NOESIS_API
            Error.Check();
            #endif
        }

        private static void Noesis_DependencySet_BaseComponent_(IntPtr dependencyObject, IntPtr dependencyProperty,
            IntPtr val)
        {
            CheckProperty(dependencyObject, dependencyProperty, "set");
            Noesis_DependencySet_BaseComponent(dependencyObject, dependencyProperty, val);
            #if UNITY_EDITOR || NOESIS_API
            Error.Check();
            #endif
        }

    #if UNITY_EDITOR

        ////////////////////////////////////////////////////////////////////////////////////////////////
        public static void RegisterFunctions(Library lib)
        {
            // DependencyObject Get/Set
            _DependencyGet_Bool = lib.Find<DependencyGet_BoolDelegate>(
                "Noesis_DependencyGet_Bool");
            _DependencyGet_Float = lib.Find<DependencyGet_FloatDelegate>(
                "Noesis_DependencyGet_Float");
            _DependencyGet_Double = lib.Find<DependencyGet_DoubleDelegate>(
                "Noesis_DependencyGet_Double");
            _DependencyGet_Int = lib.Find<DependencyGet_IntDelegate>(
                "Noesis_DependencyGet_Int");
            _DependencyGet_UInt = lib.Find<DependencyGet_UIntDelegate>(
                "Noesis_DependencyGet_UInt");
            _DependencyGet_Short = lib.Find<DependencyGet_ShortDelegate>(
                "Noesis_DependencyGet_Short");
            _DependencyGet_UShort = lib.Find<DependencyGet_UShortDelegate>(
                "Noesis_DependencyGet_UShort");
            _DependencyGet_String = lib.Find<DependencyGet_StringDelegate>(
                "Noesis_DependencyGet_String");
            _DependencyGet_Color = lib.Find<DependencyGet_ColorDelegate>(
                "Noesis_DependencyGet_Color");
            _DependencyGet_Point = lib.Find<DependencyGet_PointDelegate>(
                "Noesis_DependencyGet_Point");
            _DependencyGet_Rect = lib.Find<DependencyGet_RectDelegate>(
                "Noesis_DependencyGet_Rect");
            _DependencyGet_Size = lib.Find<DependencyGet_SizeDelegate>(
                "Noesis_DependencyGet_Size");
            _DependencyGet_Thickness = lib.Find<DependencyGet_ThicknessDelegate>(
                "Noesis_DependencyGet_Thickness");
            _DependencyGet_CornerRadius = lib.Find<DependencyGet_CornerRadiusDelegate>(
                "Noesis_DependencyGet_CornerRadius");
            _DependencyGet_TimeSpan = lib.Find<DependencyGet_TimeSpanDelegate>(
                "Noesis_DependencyGet_TimeSpan");
            _DependencyGet_Duration = lib.Find<DependencyGet_DurationDelegate>(
                "Noesis_DependencyGet_Duration");
            _DependencyGet_KeyTime = lib.Find<DependencyGet_KeyTimeDelegate>(
                "Noesis_DependencyGet_KeyTime");
            _DependencyGet_BaseComponent = lib.Find<DependencyGet_BaseComponentDelegate>(
                "Noesis_DependencyGet_BaseComponent");

            _DependencySet_Bool = lib.Find<DependencySet_BoolDelegate>(
                "Noesis_DependencySet_Bool");
            _DependencySet_Float = lib.Find<DependencySet_FloatDelegate>(
                "Noesis_DependencySet_Float");
            _DependencySet_Double = lib.Find<DependencySet_DoubleDelegate>(
                "Noesis_DependencySet_Double");
            _DependencySet_Int = lib.Find<DependencySet_IntDelegate>(
                "Noesis_DependencySet_Int");
            _DependencySet_UInt = lib.Find<DependencySet_UIntDelegate>(
                "Noesis_DependencySet_UInt");
            _DependencySet_Short = lib.Find<DependencySet_ShortDelegate>(
                "Noesis_DependencySet_Short");
            _DependencySet_UShort = lib.Find<DependencySet_UShortDelegate>(
                "Noesis_DependencySet_UShort");
            _DependencySet_String = lib.Find<DependencySet_StringDelegate>(
                "Noesis_DependencySet_String");
            _DependencySet_Color = lib.Find<DependencySet_ColorDelegate>(
                "Noesis_DependencySet_Color");
            _DependencySet_Point = lib.Find<DependencySet_PointDelegate>(
                "Noesis_DependencySet_Point");
            _DependencySet_Rect = lib.Find<DependencySet_RectDelegate>(
                "Noesis_DependencySet_Rect");
            _DependencySet_Size = lib.Find<DependencySet_SizeDelegate>(
                "Noesis_DependencySet_Size");
            _DependencySet_Thickness = lib.Find<DependencySet_ThicknessDelegate>(
                "Noesis_DependencySet_Thickness");
            _DependencySet_CornerRadius = lib.Find<DependencySet_CornerRadiusDelegate>(
                "Noesis_DependencySet_CornerRadius");
            _DependencySet_TimeSpan = lib.Find<DependencySet_TimeSpanDelegate>(
                "Noesis_DependencySet_TimeSpan");
            _DependencySet_Duration = lib.Find<DependencySet_DurationDelegate>(
                "Noesis_DependencySet_Duration");
            _DependencySet_KeyTime = lib.Find<DependencySet_KeyTimeDelegate>(
                "Noesis_DependencySet_KeyTime");
            _DependencySet_BaseComponent = lib.Find<DependencySet_BaseComponentDelegate>(
                "Noesis_DependencySet_BaseComponent");
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////
        public static void UnregisterFunctions()
        {
            // DependencyObject Get/Set
            _DependencyGet_Bool = null;
            _DependencyGet_Float = null;
            _DependencyGet_Double = null;
            _DependencyGet_Int = null;
            _DependencyGet_UInt = null;
            _DependencyGet_Short = null;
            _DependencyGet_UShort = null;
            _DependencyGet_String = null;
            _DependencyGet_Color = null;
            _DependencyGet_Point = null;
            _DependencyGet_Rect = null;
            _DependencyGet_Size = null;
            _DependencyGet_Thickness = null;
            _DependencyGet_CornerRadius = null;
            _DependencyGet_TimeSpan = null;
            _DependencyGet_Duration = null;
            _DependencyGet_KeyTime = null;
            _DependencyGet_BaseComponent = null;

            _DependencySet_Bool = null;
            _DependencySet_Float = null;
            _DependencySet_Double = null;
            _DependencySet_Int = null;
            _DependencySet_UInt = null;
            _DependencySet_Short = null;
            _DependencySet_UShort = null;
            _DependencySet_String = null;
            _DependencySet_Color = null;
            _DependencySet_Point = null;
            _DependencySet_Rect = null;
            _DependencySet_Size = null;
            _DependencySet_Thickness = null;
            _DependencySet_CornerRadius = null;
            _DependencySet_TimeSpan = null;
            _DependencySet_Duration = null;
            _DependencySet_KeyTime = null;
            _DependencySet_BaseComponent = null;
        }


        ////////////////////////////////////////////////////////////////////////////////////////////////
        [return: MarshalAs(UnmanagedType.U1)]
        delegate bool DependencyGet_BoolDelegate(IntPtr dependencyObject, IntPtr dependencyProperty,
            bool isNullable, [MarshalAs(UnmanagedType.U1)]out bool isNull);
        static DependencyGet_BoolDelegate _DependencyGet_Bool;
        private static bool Noesis_DependencyGet_Bool(IntPtr dependencyObject, IntPtr dependencyProperty,
            bool isNullable, out bool isNull)
        {
            return _DependencyGet_Bool(dependencyObject, dependencyProperty, isNullable, out isNull);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////
        delegate float DependencyGet_FloatDelegate(IntPtr dependencyObject, IntPtr dependencyProperty,
            bool isNullable, [MarshalAs(UnmanagedType.U1)]out bool isNull);
        static DependencyGet_FloatDelegate _DependencyGet_Float;
        private static float Noesis_DependencyGet_Float(IntPtr dependencyObject, IntPtr dependencyProperty,
            bool isNullable, out bool isNull)
        {
            return _DependencyGet_Float(dependencyObject, dependencyProperty, isNullable, out isNull);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////
        delegate double DependencyGet_DoubleDelegate(IntPtr dependencyObject, IntPtr dependencyProperty,
            bool isNullable, [MarshalAs(UnmanagedType.U1)]out bool isNull);
        static DependencyGet_DoubleDelegate _DependencyGet_Double;
        private static double Noesis_DependencyGet_Double(IntPtr dependencyObject, IntPtr dependencyProperty,
            bool isNullable, out bool isNull)
        {
            return _DependencyGet_Double(dependencyObject, dependencyProperty, isNullable, out isNull);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////
        delegate int DependencyGet_IntDelegate(IntPtr dependencyObject, IntPtr dependencyProperty,
            bool isNullable, [MarshalAs(UnmanagedType.U1)]out bool isNull);
        static DependencyGet_IntDelegate _DependencyGet_Int;
        private static int Noesis_DependencyGet_Int(IntPtr dependencyObject, IntPtr dependencyProperty,
            bool isNullable, out bool isNull)
        {
            return _DependencyGet_Int(dependencyObject, dependencyProperty, isNullable, out isNull);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////
        delegate uint DependencyGet_UIntDelegate(IntPtr dependencyObject, IntPtr dependencyProperty,
            bool isNullable, [MarshalAs(UnmanagedType.U1)]out bool isNull);
        static DependencyGet_UIntDelegate _DependencyGet_UInt;
        private static uint Noesis_DependencyGet_UInt(IntPtr dependencyObject, IntPtr dependencyProperty,
            bool isNullable, out bool isNull)
        {
            return _DependencyGet_UInt(dependencyObject, dependencyProperty, isNullable, out isNull);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////
        delegate short DependencyGet_ShortDelegate(IntPtr dependencyObject, IntPtr dependencyProperty,
            bool isNullable, [MarshalAs(UnmanagedType.U1)]out bool isNull);
        static DependencyGet_ShortDelegate _DependencyGet_Short;
        private static short Noesis_DependencyGet_Short(IntPtr dependencyObject, IntPtr dependencyProperty,
            bool isNullable, out bool isNull)
        {
            return _DependencyGet_Short(dependencyObject, dependencyProperty, isNullable, out isNull);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////
        delegate ushort DependencyGet_UShortDelegate(IntPtr dependencyObject, IntPtr dependencyProperty,
            bool isNullable, [MarshalAs(UnmanagedType.U1)]out bool isNull);
        static DependencyGet_UShortDelegate _DependencyGet_UShort;
        private static ushort Noesis_DependencyGet_UShort(IntPtr dependencyObject, IntPtr dependencyProperty,
            bool isNullable, out bool isNull)
        {
            return _DependencyGet_UShort(dependencyObject, dependencyProperty, isNullable, out isNull);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////
        delegate IntPtr DependencyGet_StringDelegate(IntPtr dependencyObject, IntPtr dependencyProperty);
        static DependencyGet_StringDelegate _DependencyGet_String;
        private static IntPtr Noesis_DependencyGet_String(IntPtr dependencyObject, IntPtr dependencyProperty)
        {
            return _DependencyGet_String(dependencyObject, dependencyProperty);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////
        delegate IntPtr DependencyGet_ColorDelegate(IntPtr dependencyObject, IntPtr dependencyProperty,
            bool isNullable, [MarshalAs(UnmanagedType.U1)]out bool isNull);
        static DependencyGet_ColorDelegate _DependencyGet_Color;
        private static IntPtr Noesis_DependencyGet_Color(IntPtr dependencyObject, IntPtr dependencyProperty,
            bool isNullable, out bool isNull)
        {
            return _DependencyGet_Color(dependencyObject, dependencyProperty, isNullable, out isNull);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////
        delegate IntPtr DependencyGet_PointDelegate(IntPtr dependencyObject, IntPtr dependencyProperty,
            bool isNullable, [MarshalAs(UnmanagedType.U1)]out bool isNull);
        static DependencyGet_PointDelegate _DependencyGet_Point;
        private static IntPtr Noesis_DependencyGet_Point(IntPtr dependencyObject, IntPtr dependencyProperty,
            bool isNullable, out bool isNull)
        {
            return _DependencyGet_Point(dependencyObject, dependencyProperty, isNullable, out isNull);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////
        delegate IntPtr DependencyGet_RectDelegate(IntPtr dependencyObject, IntPtr dependencyProperty,
            bool isNullable, [MarshalAs(UnmanagedType.U1)]out bool isNull);
        static DependencyGet_RectDelegate _DependencyGet_Rect;
        private static IntPtr Noesis_DependencyGet_Rect(IntPtr dependencyObject, IntPtr dependencyProperty,
            bool isNullable, out bool isNull)
        {
            return _DependencyGet_Rect(dependencyObject, dependencyProperty, isNullable, out isNull);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////
        delegate IntPtr DependencyGet_SizeDelegate(IntPtr dependencyObject, IntPtr dependencyProperty,
            bool isNullable, [MarshalAs(UnmanagedType.U1)]out bool isNull);
        static DependencyGet_SizeDelegate _DependencyGet_Size;
        private static IntPtr Noesis_DependencyGet_Size(IntPtr dependencyObject, IntPtr dependencyProperty,
            bool isNullable, out bool isNull)
        {
            return _DependencyGet_Size(dependencyObject, dependencyProperty, isNullable, out isNull);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////
        delegate IntPtr DependencyGet_ThicknessDelegate(IntPtr dependencyObject, IntPtr dependencyProperty,
            bool isNullable, [MarshalAs(UnmanagedType.U1)]out bool isNull);
        static DependencyGet_ThicknessDelegate _DependencyGet_Thickness;
        private static IntPtr Noesis_DependencyGet_Thickness(IntPtr dependencyObject, IntPtr dependencyProperty,
            bool isNullable, out bool isNull)
        {
            return _DependencyGet_Thickness(dependencyObject, dependencyProperty, isNullable, out isNull);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////
        delegate IntPtr DependencyGet_CornerRadiusDelegate(IntPtr dependencyObject, IntPtr dependencyProperty,
            bool isNullable, [MarshalAs(UnmanagedType.U1)]out bool isNull);
        static DependencyGet_CornerRadiusDelegate _DependencyGet_CornerRadius;
        private static IntPtr Noesis_DependencyGet_CornerRadius(IntPtr dependencyObject, IntPtr dependencyProperty,
            bool isNullable, out bool isNull)
        {
            return _DependencyGet_CornerRadius(dependencyObject, dependencyProperty, isNullable, out isNull);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////
        delegate IntPtr DependencyGet_TimeSpanDelegate(IntPtr dependencyObject, IntPtr dependencyProperty,
            bool isNullable, [MarshalAs(UnmanagedType.U1)]out bool isNull);
        static DependencyGet_TimeSpanDelegate _DependencyGet_TimeSpan;
        private static IntPtr Noesis_DependencyGet_TimeSpan(IntPtr dependencyObject, IntPtr dependencyProperty,
            bool isNullable, out bool isNull)
        {
            return _DependencyGet_TimeSpan(dependencyObject, dependencyProperty, isNullable, out isNull);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////
        delegate IntPtr DependencyGet_DurationDelegate(IntPtr dependencyObject, IntPtr dependencyProperty,
            bool isNullable, [MarshalAs(UnmanagedType.U1)]out bool isNull);
        static DependencyGet_DurationDelegate _DependencyGet_Duration;
        private static IntPtr Noesis_DependencyGet_Duration(IntPtr dependencyObject, IntPtr dependencyProperty,
            bool isNullable, out bool isNull)
        {
            return _DependencyGet_Duration(dependencyObject, dependencyProperty, isNullable, out isNull);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////
        delegate IntPtr DependencyGet_KeyTimeDelegate(IntPtr dependencyObject, IntPtr dependencyProperty,
            bool isNullable, [MarshalAs(UnmanagedType.U1)]out bool isNull);
        static DependencyGet_KeyTimeDelegate _DependencyGet_KeyTime;
        private static IntPtr Noesis_DependencyGet_KeyTime(IntPtr dependencyObject, IntPtr dependencyProperty,
            bool isNullable, out bool isNull)
        {
            return _DependencyGet_KeyTime(dependencyObject, dependencyProperty, isNullable, out isNull);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////
        delegate IntPtr DependencyGet_BaseComponentDelegate(IntPtr dependencyObject, IntPtr dependencyProperty);
        static DependencyGet_BaseComponentDelegate _DependencyGet_BaseComponent;
        private static IntPtr Noesis_DependencyGet_BaseComponent(IntPtr dependencyObject, IntPtr dependencyProperty)
        {
            return _DependencyGet_BaseComponent(dependencyObject, dependencyProperty);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////
        delegate void DependencySet_BoolDelegate(IntPtr dependencyObject, IntPtr dependencyProperty,
            bool val, bool isNullable, bool isNull);
        static DependencySet_BoolDelegate _DependencySet_Bool;
        private static void Noesis_DependencySet_Bool(IntPtr dependencyObject, IntPtr dependencyProperty,
            bool val, bool isNullable, bool isNull)
        {
            _DependencySet_Bool(dependencyObject, dependencyProperty, val, isNullable, isNull);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////
        delegate void DependencySet_FloatDelegate(IntPtr dependencyObject, IntPtr dependencyProperty,
            float val, bool isNullable, bool isNull);
        static DependencySet_FloatDelegate _DependencySet_Float;
        private static void Noesis_DependencySet_Float(IntPtr dependencyObject, IntPtr dependencyProperty,
            float val, bool isNullable, bool isNull)
        {
            _DependencySet_Float(dependencyObject, dependencyProperty, val, isNullable, isNull);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////
        delegate void DependencySet_DoubleDelegate(IntPtr dependencyObject, IntPtr dependencyProperty,
            double val, bool isNullable, bool isNull);
        static DependencySet_DoubleDelegate _DependencySet_Double;
        private static void Noesis_DependencySet_Double(IntPtr dependencyObject, IntPtr dependencyProperty,
            double val, bool isNullable, bool isNull)
        {
            _DependencySet_Double(dependencyObject, dependencyProperty, val, isNullable, isNull);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////
        delegate void DependencySet_IntDelegate(IntPtr dependencyObject, IntPtr dependencyProperty,
            int val, bool isNullable, bool isNull);
        static DependencySet_IntDelegate _DependencySet_Int;
        private static void Noesis_DependencySet_Int(IntPtr dependencyObject, IntPtr dependencyProperty,
            int val, bool isNullable, bool isNull)
        {
            _DependencySet_Int(dependencyObject, dependencyProperty, val, isNullable, isNull);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////
        delegate void DependencySet_UIntDelegate(IntPtr dependencyObject, IntPtr dependencyProperty,
            uint val, bool isNullable, bool isNull);
        static DependencySet_UIntDelegate _DependencySet_UInt;
        private static void Noesis_DependencySet_UInt(IntPtr dependencyObject, IntPtr dependencyProperty,
            uint val, bool isNullable, bool isNull)
        {
            _DependencySet_UInt(dependencyObject, dependencyProperty, val, isNullable, isNull);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////
        delegate void DependencySet_ShortDelegate(IntPtr dependencyObject, IntPtr dependencyProperty,
            short val, bool isNullable, bool isNull);
        static DependencySet_ShortDelegate _DependencySet_Short;
        private static void Noesis_DependencySet_Short(IntPtr dependencyObject, IntPtr dependencyProperty,
            short val, bool isNullable, bool isNull)
        {
            _DependencySet_Short(dependencyObject, dependencyProperty, val, isNullable, isNull);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////
        delegate void DependencySet_UShortDelegate(IntPtr dependencyObject, IntPtr dependencyProperty,
            ushort val, bool isNullable, bool isNull);
        static DependencySet_UShortDelegate _DependencySet_UShort;
        private static void Noesis_DependencySet_UShort(IntPtr dependencyObject, IntPtr dependencyProperty,
            ushort val, bool isNullable, bool isNull)
        {
            _DependencySet_UShort(dependencyObject, dependencyProperty, val, isNullable, isNull);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////
        delegate void DependencySet_StringDelegate(IntPtr dependencyObject, IntPtr dependencyProperty,
            string val);
        static DependencySet_StringDelegate _DependencySet_String;
        private static void Noesis_DependencySet_String(IntPtr dependencyObject, IntPtr dependencyProperty,
            string val)
        {
            _DependencySet_String(dependencyObject, dependencyProperty, val);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////
        delegate void DependencySet_ColorDelegate(IntPtr dependencyObject, IntPtr dependencyProperty,
            ref Noesis.Color val, bool isNullable, bool isNull);
        static DependencySet_ColorDelegate _DependencySet_Color;
        private static void Noesis_DependencySet_Color(IntPtr dependencyObject, IntPtr dependencyProperty,
            ref Noesis.Color val, bool isNullable, bool isNull)
        {
            _DependencySet_Color(dependencyObject, dependencyProperty, ref val, isNullable, isNull);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////
        delegate void DependencySet_PointDelegate(IntPtr dependencyObject, IntPtr dependencyProperty,
            ref Noesis.Point val, bool isNullable, bool isNull);
        static DependencySet_PointDelegate _DependencySet_Point;
        private static void Noesis_DependencySet_Point(IntPtr dependencyObject, IntPtr dependencyProperty,
            ref Noesis.Point val, bool isNullable, bool isNull)
        {
            _DependencySet_Point(dependencyObject, dependencyProperty, ref val, isNullable, isNull);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////
        delegate void DependencySet_RectDelegate(IntPtr dependencyObject, IntPtr dependencyProperty,
            ref Noesis.Rect val, bool isNullable, bool isNull);
        static DependencySet_RectDelegate _DependencySet_Rect;
        private static void Noesis_DependencySet_Rect(IntPtr dependencyObject, IntPtr dependencyProperty,
            ref Noesis.Rect val, bool isNullable, bool isNull)
        {
            _DependencySet_Rect(dependencyObject, dependencyProperty, ref val, isNullable, isNull);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////
        delegate void DependencySet_SizeDelegate(IntPtr dependencyObject, IntPtr dependencyProperty,
            ref Noesis.Size val, bool isNullable, bool isNull);
        static DependencySet_SizeDelegate _DependencySet_Size;
        private static void Noesis_DependencySet_Size(IntPtr dependencyObject, IntPtr dependencyProperty,
            ref Noesis.Size val, bool isNullable, bool isNull)
        {
            _DependencySet_Size(dependencyObject, dependencyProperty, ref val, isNullable, isNull);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////
        delegate void DependencySet_ThicknessDelegate(IntPtr dependencyObject, IntPtr dependencyProperty,
            ref Noesis.Thickness val, bool isNullable, bool isNull);
        static DependencySet_ThicknessDelegate _DependencySet_Thickness;
        private static void Noesis_DependencySet_Thickness(IntPtr dependencyObject, IntPtr dependencyProperty,
            ref Noesis.Thickness val, bool isNullable, bool isNull)
        {
            _DependencySet_Thickness(dependencyObject, dependencyProperty, ref val, isNullable, isNull);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////
        delegate void DependencySet_CornerRadiusDelegate(IntPtr dependencyObject, IntPtr dependencyProperty,
            ref Noesis.CornerRadius val, bool isNullable, bool isNull);
        static DependencySet_CornerRadiusDelegate _DependencySet_CornerRadius;
        private static void Noesis_DependencySet_CornerRadius(IntPtr dependencyObject, IntPtr dependencyProperty,
            ref Noesis.CornerRadius val, bool isNullable, bool isNull)
        {
            _DependencySet_CornerRadius(dependencyObject, dependencyProperty, ref val, isNullable, isNull);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////
        delegate void DependencySet_TimeSpanDelegate(IntPtr dependencyObject, IntPtr dependencyProperty,
            ref Noesis.TimeSpanStruct val, bool isNullable, bool isNull);
        static DependencySet_TimeSpanDelegate _DependencySet_TimeSpan;
        private static void Noesis_DependencySet_TimeSpan(IntPtr dependencyObject, IntPtr dependencyProperty,
            ref Noesis.TimeSpanStruct val, bool isNullable, bool isNull)
        {
            _DependencySet_TimeSpan(dependencyObject, dependencyProperty, ref val, isNullable, isNull);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////
        delegate void DependencySet_DurationDelegate(IntPtr dependencyObject, IntPtr dependencyProperty,
            ref Noesis.Duration val, bool isNullable, bool isNull);
        static DependencySet_DurationDelegate _DependencySet_Duration;
        private static void Noesis_DependencySet_Duration(IntPtr dependencyObject, IntPtr dependencyProperty,
            ref Noesis.Duration val, bool isNullable, bool isNull)
        {
            _DependencySet_Duration(dependencyObject, dependencyProperty, ref val, isNullable, isNull);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////
        delegate void DependencySet_KeyTimeDelegate(IntPtr dependencyObject, IntPtr dependencyProperty,
            ref Noesis.KeyTime val, bool isNullable, bool isNull);
        static DependencySet_KeyTimeDelegate _DependencySet_KeyTime;
        private static void Noesis_DependencySet_KeyTime(IntPtr dependencyObject, IntPtr dependencyProperty,
            ref Noesis.KeyTime val, bool isNullable, bool isNull)
        {
            _DependencySet_KeyTime(dependencyObject, dependencyProperty, ref val, isNullable, isNull);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////
        delegate void DependencySet_BaseComponentDelegate(IntPtr dependencyObject, IntPtr dependencyProperty,
            IntPtr val);
        static DependencySet_BaseComponentDelegate _DependencySet_BaseComponent;
        private static void Noesis_DependencySet_BaseComponent(IntPtr dependencyObject, IntPtr dependencyProperty,
            IntPtr val)
        {
            _DependencySet_BaseComponent(dependencyObject, dependencyProperty, val);
        }

    #else

        ////////////////////////////////////////////////////////////////////////////////////////////////
        #if UNITY_IPHONE || UNITY_XBOX360
        [DllImport("__Internal", EntryPoint="Noesis_DependencyGet_Bool")]
        #else
        [DllImport("Noesis", EntryPoint = "Noesis_DependencyGet_Bool")]
        #endif
        [return: MarshalAs(UnmanagedType.U1)]
        private static extern bool Noesis_DependencyGet_Bool(IntPtr dependencyObject, IntPtr dependencyProperty,
            bool isNullable, [MarshalAs(UnmanagedType.U1)]out bool isNull);

        ////////////////////////////////////////////////////////////////////////////////////////////////
        #if UNITY_IPHONE || UNITY_XBOX360
        [DllImport("__Internal", EntryPoint="Noesis_DependencyGet_Float")]
        #else
        [DllImport("Noesis", EntryPoint = "Noesis_DependencyGet_Float")]
        #endif
        private static extern float Noesis_DependencyGet_Float(IntPtr dependencyObject, IntPtr dependencyProperty,
            bool isNullable, [MarshalAs(UnmanagedType.U1)]out bool isNull);

        ////////////////////////////////////////////////////////////////////////////////////////////////
        #if UNITY_IPHONE || UNITY_XBOX360
        [DllImport("__Internal", EntryPoint="Noesis_DependencyGet_Double")]
        #else
        [DllImport("Noesis", EntryPoint = "Noesis_DependencyGet_Double")]
        #endif
        private static extern double Noesis_DependencyGet_Double(IntPtr dependencyObject, IntPtr dependencyProperty,
            bool isNullable, [MarshalAs(UnmanagedType.U1)]out bool isNull);

        ////////////////////////////////////////////////////////////////////////////////////////////////
        #if UNITY_IPHONE || UNITY_XBOX360
        [DllImport("__Internal", EntryPoint="Noesis_DependencyGet_Int")]
        #else
        [DllImport("Noesis", EntryPoint = "Noesis_DependencyGet_Int")]
        #endif
        private static extern int Noesis_DependencyGet_Int(IntPtr dependencyObject, IntPtr dependencyProperty,
            bool isNullable, [MarshalAs(UnmanagedType.U1)]out bool isNull);

        ////////////////////////////////////////////////////////////////////////////////////////////////
        #if UNITY_IPHONE || UNITY_XBOX360
        [DllImport("__Internal", EntryPoint="Noesis_DependencyGet_UInt")]
        #else
        [DllImport("Noesis", EntryPoint = "Noesis_DependencyGet_UInt")]
        #endif
        private static extern uint Noesis_DependencyGet_UInt(IntPtr dependencyObject, IntPtr dependencyProperty,
            bool isNullable, [MarshalAs(UnmanagedType.U1)]out bool isNull);

        ////////////////////////////////////////////////////////////////////////////////////////////////
        #if UNITY_IPHONE || UNITY_XBOX360
        [DllImport("__Internal", EntryPoint="Noesis_DependencyGet_Short")]
        #else
        [DllImport("Noesis", EntryPoint = "Noesis_DependencyGet_Short")]
        #endif
        private static extern short Noesis_DependencyGet_Short(IntPtr dependencyObject, IntPtr dependencyProperty,
            bool isNullable, [MarshalAs(UnmanagedType.U1)]out bool isNull);

        ////////////////////////////////////////////////////////////////////////////////////////////////
        #if UNITY_IPHONE || UNITY_XBOX360
        [DllImport("__Internal", EntryPoint="Noesis_DependencyGet_UShort")]
        #else
        [DllImport("Noesis", EntryPoint = "Noesis_DependencyGet_UShort")]
        #endif
        private static extern ushort Noesis_DependencyGet_UShort(IntPtr dependencyObject, IntPtr dependencyProperty,
            bool isNullable, [MarshalAs(UnmanagedType.U1)]out bool isNull);

        ////////////////////////////////////////////////////////////////////////////////////////////////
        #if UNITY_IPHONE || UNITY_XBOX360
        [DllImport("__Internal", EntryPoint="Noesis_DependencyGet_String")]
        #else
        [DllImport("Noesis", EntryPoint = "Noesis_DependencyGet_String")]
        #endif
        private static extern IntPtr Noesis_DependencyGet_String(IntPtr dependencyObject, IntPtr dependencyProperty);

        ////////////////////////////////////////////////////////////////////////////////////////////////
        #if UNITY_IPHONE || UNITY_XBOX360
        [DllImport("__Internal", EntryPoint="Noesis_DependencyGet_Color")]
        #else
        [DllImport("Noesis", EntryPoint = "Noesis_DependencyGet_Color")]
        #endif
        private static extern IntPtr Noesis_DependencyGet_Color(IntPtr dependencyObject, IntPtr dependencyProperty,
            bool isNullable, [MarshalAs(UnmanagedType.U1)]out bool isNull);

        ////////////////////////////////////////////////////////////////////////////////////////////////
        #if UNITY_IPHONE || UNITY_XBOX360
        [DllImport("__Internal", EntryPoint="Noesis_DependencyGet_Point")]
        #else
        [DllImport("Noesis", EntryPoint = "Noesis_DependencyGet_Point")]
        #endif
        private static extern IntPtr Noesis_DependencyGet_Point(IntPtr dependencyObject, IntPtr dependencyProperty,
            bool isNullable, [MarshalAs(UnmanagedType.U1)]out bool isNull);

        ////////////////////////////////////////////////////////////////////////////////////////////////
        #if UNITY_IPHONE || UNITY_XBOX360
        [DllImport("__Internal", EntryPoint="Noesis_DependencyGet_Rect")]
        #else
        [DllImport("Noesis", EntryPoint = "Noesis_DependencyGet_Rect")]
        #endif
        private static extern IntPtr Noesis_DependencyGet_Rect(IntPtr dependencyObject, IntPtr dependencyProperty,
            bool isNullable, [MarshalAs(UnmanagedType.U1)]out bool isNull);

        ////////////////////////////////////////////////////////////////////////////////////////////////
        #if UNITY_IPHONE || UNITY_XBOX360
        [DllImport("__Internal", EntryPoint="Noesis_DependencyGet_Size")]
        #else
        [DllImport("Noesis", EntryPoint = "Noesis_DependencyGet_Size")]
        #endif
        private static extern IntPtr Noesis_DependencyGet_Size(IntPtr dependencyObject, IntPtr dependencyProperty,
            bool isNullable, [MarshalAs(UnmanagedType.U1)]out bool isNull);

        ////////////////////////////////////////////////////////////////////////////////////////////////
        #if UNITY_IPHONE || UNITY_XBOX360
        [DllImport("__Internal", EntryPoint="Noesis_DependencyGet_Thickness")]
        #else
        [DllImport("Noesis", EntryPoint = "Noesis_DependencyGet_Thickness")]
        #endif
        private static extern IntPtr Noesis_DependencyGet_Thickness(IntPtr dependencyObject, IntPtr dependencyProperty,
            bool isNullable, [MarshalAs(UnmanagedType.U1)]out bool isNull);

        ////////////////////////////////////////////////////////////////////////////////////////////////
        #if UNITY_IPHONE || UNITY_XBOX360
        [DllImport("__Internal", EntryPoint="Noesis_DependencyGet_CornerRadius")]
        #else
        [DllImport("Noesis", EntryPoint = "Noesis_DependencyGet_CornerRadius")]
        #endif
        private static extern IntPtr Noesis_DependencyGet_CornerRadius(IntPtr dependencyObject, IntPtr dependencyProperty,
            bool isNullable, [MarshalAs(UnmanagedType.U1)]out bool isNull);

        ////////////////////////////////////////////////////////////////////////////////////////////////
        #if UNITY_IPHONE || UNITY_XBOX360
        [DllImport("__Internal", EntryPoint="Noesis_DependencyGet_TimeSpan")]
        #else
        [DllImport("Noesis", EntryPoint = "Noesis_DependencyGet_TimeSpan")]
        #endif
        private static extern IntPtr Noesis_DependencyGet_TimeSpan(IntPtr dependencyObject, IntPtr dependencyProperty,
            bool isNullable, [MarshalAs(UnmanagedType.U1)]out bool isNull);

        ////////////////////////////////////////////////////////////////////////////////////////////////
        #if UNITY_IPHONE || UNITY_XBOX360
        [DllImport("__Internal", EntryPoint="Noesis_DependencyGet_Duration")]
        #else
        [DllImport("Noesis", EntryPoint = "Noesis_DependencyGet_Duration")]
        #endif
        private static extern IntPtr Noesis_DependencyGet_Duration(IntPtr dependencyObject, IntPtr dependencyProperty,
            bool isNullable, [MarshalAs(UnmanagedType.U1)]out bool isNull);

        ////////////////////////////////////////////////////////////////////////////////////////////////
        #if UNITY_IPHONE || UNITY_XBOX360
        [DllImport("__Internal", EntryPoint="Noesis_DependencyGet_KeyTime")]
        #else
        [DllImport("Noesis", EntryPoint = "Noesis_DependencyGet_KeyTime")]
        #endif
        private static extern IntPtr Noesis_DependencyGet_KeyTime(IntPtr dependencyObject, IntPtr dependencyProperty,
            bool isNullable, [MarshalAs(UnmanagedType.U1)]out bool isNull);

        ////////////////////////////////////////////////////////////////////////////////////////////////
        #if UNITY_IPHONE || UNITY_XBOX360
        [DllImport("__Internal", EntryPoint="Noesis_DependencyGet_BaseComponent")]
        #else
        [DllImport("Noesis", EntryPoint = "Noesis_DependencyGet_BaseComponent")]
        #endif
        private static extern IntPtr Noesis_DependencyGet_BaseComponent(IntPtr dependencyObject, IntPtr dependencyProperty);

        ////////////////////////////////////////////////////////////////////////////////////////////////
        #if UNITY_IPHONE || UNITY_XBOX360
        [DllImport("__Internal", EntryPoint="Noesis_DependencySet_Bool")]
        #else
        [DllImport("Noesis", EntryPoint = "Noesis_DependencySet_Bool")]
        #endif
        private static extern void Noesis_DependencySet_Bool(IntPtr dependencyObject, IntPtr dependencyProperty,
            [MarshalAs(UnmanagedType.U1)] bool val, bool isNullable, bool isNull);

        ////////////////////////////////////////////////////////////////////////////////////////////////
        #if UNITY_IPHONE || UNITY_XBOX360
        [DllImport("__Internal", EntryPoint="Noesis_DependencySet_Float")]
        #else
        [DllImport("Noesis", EntryPoint = "Noesis_DependencySet_Float")]
        #endif
        private static extern void Noesis_DependencySet_Float(IntPtr dependencyObject, IntPtr dependencyProperty,
            float val, bool isNullable, bool isNull);

        ////////////////////////////////////////////////////////////////////////////////////////////////
        #if UNITY_IPHONE || UNITY_XBOX360
        [DllImport("__Internal", EntryPoint="Noesis_DependencySet_Double")]
        #else
        [DllImport("Noesis", EntryPoint = "Noesis_DependencySet_Double")]
        #endif
        private static extern void Noesis_DependencySet_Double(IntPtr dependencyObject, IntPtr dependencyProperty,
            double val, bool isNullable, bool isNull);

        ////////////////////////////////////////////////////////////////////////////////////////////////
        #if UNITY_IPHONE || UNITY_XBOX360
        [DllImport("__Internal", EntryPoint="Noesis_DependencySet_Int")]
        #else
        [DllImport("Noesis", EntryPoint = "Noesis_DependencySet_Int")]
        #endif
        private static extern void Noesis_DependencySet_Int(IntPtr dependencyObject, IntPtr dependencyProperty,
            int val, bool isNullable, bool isNull);

        ////////////////////////////////////////////////////////////////////////////////////////////////
        #if UNITY_IPHONE || UNITY_XBOX360
        [DllImport("__Internal", EntryPoint="Noesis_DependencySet_UInt")]
        #else
        [DllImport("Noesis", EntryPoint = "Noesis_DependencySet_UInt")]
        #endif
        private static extern void Noesis_DependencySet_UInt(IntPtr dependencyObject, IntPtr dependencyProperty,
            uint val, bool isNullable, bool isNull);

        ////////////////////////////////////////////////////////////////////////////////////////////////
        #if UNITY_IPHONE || UNITY_XBOX360
        [DllImport("__Internal", EntryPoint="Noesis_DependencySet_Short")]
        #else
        [DllImport("Noesis", EntryPoint = "Noesis_DependencySet_Short")]
        #endif
        private static extern void Noesis_DependencySet_Short(IntPtr dependencyObject, IntPtr dependencyProperty,
            short val, bool isNullable, bool isNull);

        ////////////////////////////////////////////////////////////////////////////////////////////////
        #if UNITY_IPHONE || UNITY_XBOX360
        [DllImport("__Internal", EntryPoint="Noesis_DependencySet_UShort")]
        #else
        [DllImport("Noesis", EntryPoint = "Noesis_DependencySet_UShort")]
        #endif
        private static extern void Noesis_DependencySet_UShort(IntPtr dependencyObject, IntPtr dependencyProperty,
            ushort val, bool isNullable, bool isNull);

        ////////////////////////////////////////////////////////////////////////////////////////////////
        #if UNITY_IPHONE || UNITY_XBOX360
        [DllImport("__Internal", EntryPoint="Noesis_DependencySet_String")]
        #else
        [DllImport("Noesis", EntryPoint = "Noesis_DependencySet_String")]
        #endif
        private static extern void Noesis_DependencySet_String(IntPtr dependencyObject, IntPtr dependencyProperty,
            string val);

        ////////////////////////////////////////////////////////////////////////////////////////////////
        #if UNITY_IPHONE || UNITY_XBOX360
        [DllImport("__Internal", EntryPoint="Noesis_DependencySet_Color")]
        #else
        [DllImport("Noesis", EntryPoint = "Noesis_DependencySet_Color")]
        #endif
        private static extern void Noesis_DependencySet_Color(IntPtr dependencyObject, IntPtr dependencyProperty,
            ref Noesis.Color val, bool isNullable, bool isNull);

        ////////////////////////////////////////////////////////////////////////////////////////////////
        #if UNITY_IPHONE || UNITY_XBOX360
        [DllImport("__Internal", EntryPoint="Noesis_DependencySet_Point")]
        #else
        [DllImport("Noesis", EntryPoint = "Noesis_DependencySet_Point")]
        #endif
        private static extern void Noesis_DependencySet_Point(IntPtr dependencyObject, IntPtr dependencyProperty,
            ref Noesis.Point val, bool isNullable, bool isNull);

        ////////////////////////////////////////////////////////////////////////////////////////////////
        #if UNITY_IPHONE || UNITY_XBOX360
        [DllImport("__Internal", EntryPoint="Noesis_DependencySet_Rect")]
        #else
        [DllImport("Noesis", EntryPoint = "Noesis_DependencySet_Rect")]
        #endif
        private static extern void Noesis_DependencySet_Rect(IntPtr dependencyObject, IntPtr dependencyProperty,
            ref Noesis.Rect val, bool isNullable, bool isNull);

        ////////////////////////////////////////////////////////////////////////////////////////////////
        #if UNITY_IPHONE || UNITY_XBOX360
        [DllImport("__Internal", EntryPoint="Noesis_DependencySet_Size")]
        #else
        [DllImport("Noesis", EntryPoint = "Noesis_DependencySet_Size")]
        #endif
        private static extern void Noesis_DependencySet_Size(IntPtr dependencyObject, IntPtr dependencyProperty,
            ref Noesis.Size val, bool isNullable, bool isNull);

        ////////////////////////////////////////////////////////////////////////////////////////////////
        #if UNITY_IPHONE || UNITY_XBOX360
        [DllImport("__Internal", EntryPoint="Noesis_DependencySet_Thickness")]
        #else
        [DllImport("Noesis", EntryPoint = "Noesis_DependencySet_Thickness")]
        #endif
        private static extern void Noesis_DependencySet_Thickness(IntPtr dependencyObject, IntPtr dependencyProperty,
            ref Noesis.Thickness val, bool isNullable, bool isNull);

        ////////////////////////////////////////////////////////////////////////////////////////////////
        #if UNITY_IPHONE || UNITY_XBOX360
        [DllImport("__Internal", EntryPoint="Noesis_DependencySet_CornerRadius")]
        #else
        [DllImport("Noesis", EntryPoint = "Noesis_DependencySet_CornerRadius")]
        #endif
        private static extern void Noesis_DependencySet_CornerRadius(IntPtr dependencyObject, IntPtr dependencyProperty,
            ref Noesis.CornerRadius val, bool isNullable, bool isNull);

        ////////////////////////////////////////////////////////////////////////////////////////////////
        #if UNITY_IPHONE || UNITY_XBOX360
        [DllImport("__Internal", EntryPoint="Noesis_DependencySet_TimeSpan")]
        #else
        [DllImport("Noesis", EntryPoint = "Noesis_DependencySet_TimeSpan")]
        #endif
        private static extern void Noesis_DependencySet_TimeSpan(IntPtr dependencyObject, IntPtr dependencyProperty,
            ref Noesis.TimeSpanStruct val, bool isNullable, bool isNull);

        ////////////////////////////////////////////////////////////////////////////////////////////////
        #if UNITY_IPHONE || UNITY_XBOX360
        [DllImport("__Internal", EntryPoint="Noesis_DependencySet_Duration")]
        #else
        [DllImport("Noesis", EntryPoint = "Noesis_DependencySet_Duration")]
        #endif
        private static extern void Noesis_DependencySet_Duration(IntPtr dependencyObject, IntPtr dependencyProperty,
            ref Noesis.Duration val, bool isNullable, bool isNull);

        ////////////////////////////////////////////////////////////////////////////////////////////////
        #if UNITY_IPHONE || UNITY_XBOX360
        [DllImport("__Internal", EntryPoint="Noesis_DependencySet_KeyTime")]
        #else
        [DllImport("Noesis", EntryPoint = "Noesis_DependencySet_KeyTime")]
        #endif
        private static extern void Noesis_DependencySet_KeyTime(IntPtr dependencyObject, IntPtr dependencyProperty,
            ref Noesis.KeyTime val, bool isNullable, bool isNull);

        ////////////////////////////////////////////////////////////////////////////////////////////////
        #if UNITY_IPHONE || UNITY_XBOX360
        [DllImport("__Internal", EntryPoint="Noesis_DependencySet_BaseComponent")]
        #else
        [DllImport("Noesis", EntryPoint = "Noesis_DependencySet_BaseComponent")]
        #endif
        private static extern void Noesis_DependencySet_BaseComponent(IntPtr dependencyObject, IntPtr dependencyProperty,
            IntPtr val);

    #endif

        #endregion
    }

}
