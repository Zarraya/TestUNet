using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Reflection;

namespace Noesis
{

    public partial class BaseComponent
    {
        protected BaseComponent()
        {
            Type type = this.GetType();

            if (Noesis.Extend.NeedsCreateCPtr(type))
            {
                // Instance created from C#, we need to create C++ native object
                bool registerExtend;
                IntPtr cPtr = CreateCPtr(type, out registerExtend);
                Init(cPtr, true, registerExtend);
            }
            else
            {
                // Extended instance created from C++, where native object is already created
                bool registerExtend = true;
                IntPtr cPtr = Noesis.Extend.GetCPtr(this, type);
                Init(cPtr, false, registerExtend);
            }
        }

        private void Init(System.IntPtr cPtr, bool cMemoryOwn, bool registerExtend)
        {
            swigCPtr = new HandleRef(this, cPtr);

            if (registerExtend)
            {
                // NOTE: Instance added to the Extend Table before AddReference is called, so when
                // instance is grabbed table entry can be transformed into a strong reference
                Noesis.Extend.RegisterExtendInstance(this);
            }
            else
            {
                Noesis.Extend.AddProxy(this);
            }

            if (cPtr != IntPtr.Zero && !cMemoryOwn)
            {
                AddReference();
            }

            if (registerExtend)
            {
                Noesis.Extend.RegisterInterfaces(this);
            }
        }

        protected virtual System.IntPtr CreateCPtr(System.Type type, out bool registerExtend)
        {
            return CreateExtendCPtr(type, out registerExtend);
        }

        protected System.IntPtr CreateExtendCPtr(System.Type type, out bool registerExtend)
        {
            registerExtend = true;
            return Noesis.Extend.NewCPtr(type, this);
        }

        public static bool operator ==(BaseComponent a, BaseComponent b)
        {
            // If both are null, or both are the same instance, return true.
            if (System.Object.ReferenceEquals(a, b))
            {
                return true;
            }

            // If one is null, but not both, return false.
            if ((object)a == null || (object)b == null)
            {
                return false;
            }

            // Return true if wrapped c++ objects match:
            return a.swigCPtr.Handle == b.swigCPtr.Handle;
        }

        public static bool operator !=(BaseComponent a, BaseComponent b)
        {
            return !(a == b);
        }

        public override bool Equals(object o)
        {
            return this == o as BaseComponent;
        }

        public override int GetHashCode()
        {
            return swigCPtr.Handle.GetHashCode();
        }
    }

}
