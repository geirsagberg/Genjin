﻿/*
* -----------------------------------------------------------------------------
* Original code from SlimMath project. http://code.google.com/p/slimmath/
* Greetings to SlimDX Group. Original code published with the following license:
* -----------------------------------------------------------------------------
* 
* Copyright (c) 2007-2010 SlimDX Group
* 
* Permission is hereby granted, free of charge, to any person obtaining a copy
* of this software and associated documentation files (the "Software"), to deal
* in the Software without restriction, including without limitation the rights
* to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
* copies of the Software, and to permit persons to whom the Software is
* furnished to do so, subject to the following conditions:
* 
* The above copyright notice and this permission notice shall be included in
* all copies or substantial portions of the Software.
* 
* THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
* IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
* FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
* AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
* LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
* OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
* THE SOFTWARE.
*/

using System.Diagnostics;
using System.Numerics;
using System.Runtime.Serialization;

namespace Genjin.Core.Primitives;

public enum AngleType : byte {
    Radian = 0,
    Degree,
    Revolution, //or Turn / cycle
    Gradian // or Gon
}

[DataContract]
[DebuggerDisplay("{ToString(),nq}")]
public struct Angle : IComparable<Angle>, IEquatable<Angle> {
    private const float TauInv = (float) (1.0 / Math.Tau);
    private const float DegreeRadian = (float) (Math.PI / 180.0);
    private const float RadianDegree = (float) (180.0 / Math.PI);
    private const float GradianRadian = (float) (Math.PI / 200.0);
    private const float RadianGradian = (float) (200.0 / Math.PI);

    [DataMember]
    public float Radians { get; set; }

    public float Degrees {
        get => Radians * RadianDegree;
        set => Radians = value * DegreeRadian;
    }

    public float Gradians {
        get => Radians * RadianGradian;
        set => Radians = value * GradianRadian;
    }

    public float Revolutions {
        get => Radians * TauInv;
        set => Radians = value * MathF.Tau;
    }

    public Angle(float value, AngleType angleType = AngleType.Radian) {
        switch (angleType) {
            default:
                Radians = 0f;
                break;
            case AngleType.Radian:
                Radians = value;
                break;
            case AngleType.Degree:
                Radians = value * DegreeRadian;
                break;
            case AngleType.Revolution:
                Radians = value * MathF.Tau;
                break;
            case AngleType.Gradian:
                Radians = value * GradianRadian;
                break;
        }
    }

    public float GetValue(AngleType angleType) {
        switch (angleType) {
            default:
                return 0f;
            case AngleType.Radian:
                return Radians;
            case AngleType.Degree:
                return Degrees;
            case AngleType.Revolution:
                return Revolutions;
            case AngleType.Gradian:
                return Gradians;
        }
    }

    public void Wrap() {
        var angle = Radians % MathF.Tau;
        if (angle <= Math.PI) angle += MathF.Tau;
        if (angle > Math.PI) angle -= MathF.Tau;
        Radians = angle;
    }

    public void WrapPositive() {
        Radians %= MathF.Tau;
        if (Radians < 0d) Radians += MathF.Tau;
        Radians = Radians;
    }

    public static Angle FromVector(Vector2 vector) {
        return new Angle((float) Math.Atan2(-vector.Y, vector.X));
    }

    public Vector2 ToUnitVector() => ToVector(1);

    public Vector2 ToVector(float length) {
        return new Vector2(length * (float) Math.Cos(Radians), -length * (float) Math.Sin(Radians));
    }

    public static bool IsBetween(Angle value, Angle min, Angle end) {
        return end < min
            ? value >= min || value <= end
            : value >= min && value <= end;
    }

    public int CompareTo(Angle other) {
        WrapPositive();
        other.WrapPositive();
        return Radians.CompareTo(other.Radians);
    }

    public bool Equals(Angle other) {
        WrapPositive();
        other.WrapPositive();
        return Radians.Equals(other.Radians);
    }

    public override bool Equals(object obj) {
        if (ReferenceEquals(null, obj)) return false;
        return obj is Angle a && Equals(a);
    }

    public override int GetHashCode() {
        // ReSharper disable once NonReadonlyMemberInGetHashCode
        return Radians.GetHashCode();
    }

    public static implicit operator float(Angle angle) {
        return angle.Radians;
    }

    public static explicit operator Angle(float angle) {
        return new Angle(angle);
    }

    public static Angle operator -(Angle angle) {
        return new Angle(-angle.Radians);
    }

    public static bool operator ==(Angle a, Angle b) {
        return a.Equals(b);
    }

    public static bool operator !=(Angle a, Angle b) {
        return !a.Equals(b);
    }

    public static Angle operator -(Angle left, Angle right) {
        return new Angle(left.Radians - right.Radians);
    }

    public static Angle operator *(Angle left, float right) {
        return new Angle(left.Radians * right);
    }

    public static Angle operator *(float left, Angle right) {
        return new Angle(right.Radians * left);
    }

    public static Angle operator +(Angle left, Angle right) {
        return new Angle(left.Radians + right.Radians);
    }

    public override string ToString() {
        return $"{Radians} Radians";
    }
}
