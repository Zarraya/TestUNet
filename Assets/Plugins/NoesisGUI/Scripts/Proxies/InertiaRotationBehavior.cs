/* ----------------------------------------------------------------------------
 * This file was automatically generated by SWIG (http://www.swig.org).
 * Version 2.0.4
 *
 * Do not make changes to this file unless you know what you are doing--modify
 * the SWIG interface file instead.
 * ----------------------------------------------------------------------------- */


using System;
using System.Runtime.InteropServices;

namespace Noesis
{

public class InertiaRotationBehavior : IDisposable {
  private HandleRef swigCPtr;
  protected bool swigCMemOwn;

  internal InertiaRotationBehavior(IntPtr cPtr, bool cMemoryOwn) {
    swigCMemOwn = cMemoryOwn;
    swigCPtr = new HandleRef(this, cPtr);
  }

  internal static HandleRef getCPtr(InertiaRotationBehavior obj) {
    return (obj == null) ? new HandleRef(null, IntPtr.Zero) : obj.swigCPtr;
  }

  ~InertiaRotationBehavior() {
    Dispose();
  }

  public virtual void Dispose() {
    lock(this) {
      if (swigCPtr.Handle != IntPtr.Zero) {
        if (swigCMemOwn) {
          swigCMemOwn = false;
          if (Noesis.Extend.Initialized) { NoesisGUI_PINVOKE.delete_InertiaRotationBehavior(swigCPtr);}
        }
        swigCPtr = new HandleRef(null, IntPtr.Zero);
      }
      GC.SuppressFinalize(this);
    }
  }

  public float DesiredDeceleration {
    get {
      return GetDesiredDecelerationHelper();
    }
    set {
      SetDesiredDecelerationHelper(value);
    }
  }

  private float GetDesiredDecelerationHelper() {
    float ret = NoesisGUI_PINVOKE.InertiaRotationBehavior_GetDesiredDecelerationHelper(swigCPtr);
    #if UNITY_EDITOR || NOESIS_API
    if (NoesisGUI_PINVOKE.SWIGPendingException.Pending) throw NoesisGUI_PINVOKE.SWIGPendingException.Retrieve();
    #endif
    return ret;
  }

  private void SetDesiredDecelerationHelper(float v) {
    NoesisGUI_PINVOKE.InertiaRotationBehavior_SetDesiredDecelerationHelper(swigCPtr, v);
    #if UNITY_EDITOR || NOESIS_API
    if (NoesisGUI_PINVOKE.SWIGPendingException.Pending) throw NoesisGUI_PINVOKE.SWIGPendingException.Retrieve();
    #endif
  }

  public InertiaRotationBehavior() : this(NoesisGUI_PINVOKE.new_InertiaRotationBehavior(), true) {
    #if UNITY_EDITOR || NOESIS_API
    if (NoesisGUI_PINVOKE.SWIGPendingException.Pending) throw NoesisGUI_PINVOKE.SWIGPendingException.Retrieve();
    #endif
  }

}

}
