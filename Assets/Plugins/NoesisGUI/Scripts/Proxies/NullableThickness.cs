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

[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
internal struct NullableThickness {

  [MarshalAs(UnmanagedType.U1)]
  private bool _hasValue;
  private Thickness _value;

  public bool HasValue { get { return this._hasValue; } }

  public Thickness Value {
    get {
      if (!HasValue) {
        throw new InvalidOperationException("Nullable does not have a value");
      }
      return this._value;
    }
  }

  public NullableThickness(Thickness v) {
    this._hasValue = true;
    this._value = v;
  }

  public static explicit operator Thickness(NullableThickness n) {
    if (!n.HasValue) {
      throw new InvalidOperationException("Nullable does not have a value");
    }
    return n.Value;
  }

  public static implicit operator NullableThickness(Thickness v) {
    return new NullableThickness(v);
  }

  public static implicit operator System.Nullable<Thickness>(NullableThickness n) {
    return n.HasValue ? new System.Nullable<Thickness>(n.Value) : new System.Nullable<Thickness>();
  }

  public static implicit operator NullableThickness(System.Nullable<Thickness> n) {
    return n.HasValue ? new NullableThickness(n.Value) : new NullableThickness();
  }

  public static bool operator==(NullableThickness n, Thickness v) {
    return n.HasValue && n.Value == v;
  }

  public static bool operator!=(NullableThickness n, Thickness v) {
    return !(n == v);
  }

  public static bool operator==(Thickness v, NullableThickness n) {
    return n == v;
  }
  
  public static bool operator!=(Thickness v, NullableThickness n) {
    return n != v;
  }

  public static bool operator==(NullableThickness n0, NullableThickness n1) {
    return n0.HasValue && n1.HasValue ? n0.Value == n1.Value : n0.HasValue == n1.HasValue;
  }

  public static bool operator!=(NullableThickness n0, NullableThickness n1) {
    return !(n0 == n1);
  }

  public override bool Equals(System.Object obj) {
    return obj is NullableThickness && this == (NullableThickness)obj;
  }

  public bool Equals(NullableThickness n) {
    return this == n;
  }

  public override int GetHashCode() {
    return HasValue ? Value.GetHashCode() : 0;
  }

}

}

