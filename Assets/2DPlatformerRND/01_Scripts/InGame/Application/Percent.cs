using System;
using UnityEngine;

namespace PahlBit
{
    /// <summary>
    /// 퍼센트(%)단위로 관리
    /// 계산방식은 값이 100%이면 2배, -100%이면 0.5배
    /// == 사용법 예시 ==
    /// Percent a = new Percent(100);
    /// double b = 10 * a.Rate; // b는 20
    /// Percent c = new Percent(-100);
    /// double d = 10 * c.Rate; // d는 5
    /// </summary>
    public struct Percent : IComparable<Percent>
    {
        private readonly double mPercentVal; // [%]

        public double PercentValue => mPercentVal;

        // 양수면 배수를 그대로 반환하지만 음수이면 그 역수를 반환한다
        // 예) 70%이면 1.7를 반환, -70%이면 1/(1.7)를 반환
        public double Rate => mPercentVal >= 0 ? (1 + (mPercentVal * 0.01)) : (1 / (1 + (Math.Abs(mPercentVal) * 0.01)));

        public Percent(double percent)
        {
            mPercentVal = percent;
        }

        // ToString
        public override string ToString()
        {
            return $"{PercentValue}%";
        }

        // ---- 암시적 변환 ----
        // public static implicit operator double(Percent p) => p.mPercentVal;
        // public static implicit operator Percent(double v) => new Percent(v);
        // public static implicit operator Percent(float v) => new Percent(v);
        // public static implicit operator Percent(int v) => new Percent(v);

        // ---- Percent끼리 연산 ----
        public static Percent operator +(Percent a, Percent b) => new Percent(a.mPercentVal + b.mPercentVal);
        public static Percent operator -(Percent a, Percent b) => new Percent(a.mPercentVal - b.mPercentVal);
        // public static Percent operator *(Percent a, Percent b) => new Percent(a.mPercentVal * b.mPercentVal);
        // public static Percent operator /(Percent a, Percent b) => new Percent(a.mPercentVal / b.mPercentVal);

        // ---- 숫자와 Percent 연산 (양방향) ----
        // public static Percent operator +(Percent a, double b) => new Percent(a.mPercentVal + b);
        // public static Percent operator -(Percent a, double b) => new Percent(a.mPercentVal - b);
        public static Percent operator *(Percent a, double b) => new Percent(a.Rate * b);
        // public static Percent operator /(Percent a, double b) => new Percent(a.mPercentVal / b);

        // public static Percent operator +(double a, Percent b) => new Percent(a + b.mPercentVal);
        // public static Percent operator -(double a, Percent b) => new Percent(a - b.mPercentVal);
        public static Percent operator *(double a, Percent b) => new Percent(a * b.Rate);
        //public static Percent operator /(double a, Percent b) => new Percent(a / b.mPercentVal);

        // ---- 비교 연산 ----
        // public static bool operator ==(Percent a, Percent b) => a.mPercentVal == b.mPercentVal;
        // public static bool operator !=(Percent a, Percent b) => a.mPercentVal != b.mPercentVal;
        public static bool operator >(Percent a, Percent b) => a.mPercentVal > b.mPercentVal;
        public static bool operator <(Percent a, Percent b) => a.mPercentVal < b.mPercentVal;
        public static bool operator >=(Percent a, Percent b) => a.mPercentVal >= b.mPercentVal;
        public static bool operator <=(Percent a, Percent b) => a.mPercentVal <= b.mPercentVal;

        public override int GetHashCode() => mPercentVal.GetHashCode();

        public int CompareTo(Percent other) => mPercentVal.CompareTo(other.mPercentVal);
    }
}