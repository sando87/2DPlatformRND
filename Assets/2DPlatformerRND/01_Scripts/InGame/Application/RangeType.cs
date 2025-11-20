using System;
using DG.Tweening;
using DG.Tweening.Core.Easing;
using UnityEngine;

namespace PahlBit
{
    /// <summary>
    /// 두 값 사이의 값을 랜덤하게 반환하는 타입
    /// </summary>
    public class RangeType
    {
        private double mMin = 0;
        private double mMax = 0;
        private Ease mEase = Ease.Linear;
        private Func<double, double> mCustomEase;

        private static System.Random ran = new System.Random();

        public void SetSeed(int seed) { ran = new System.Random(seed); }

        public RangeType(double min, double max, Ease ease = Ease.Linear)
        {
            mMin = min;
            mMax = max;
            mEase = ease;
            mCustomEase = null;
        }
        public RangeType(double min, double max, Func<double, double> func)
        {
            mMin = min;
            mMax = max;
            mEase = Ease.Unset;
            mCustomEase = func;
        }

        public double GetDouble()
        {
            double t = ran.NextDouble(); // 0~1
            t = TransferTime(t);
            return Lerp(mMin, mMax, t);
        }

        public int GetInt()
        {
            int intMin = (int)Math.Ceiling(mMin);
            int intMax = (int)Math.Floor(mMax);
            double t = ran.NextDouble(); // 0~1
            t = TransferTime(t);
            int ret = (int)Math.Round(Lerp(mMin - 0.5, mMax + 0.5, t));
            return Math.Clamp(ret, intMin, intMax);
        }

        // // 덧셈 연산자
        // public static RangeType operator +(RangeType a, RangeType b) => new RangeType(a.mMin + b.mMin, a.mMax + b.mMax);
        // // 뺄셈 연산자
        // public static RangeType operator -(RangeType a, RangeType b) => new RangeType(a.mMin - b.mMax, a.mMax - b.mMin);

        private double Lerp(double a, double b, double t) => a + (b - a) * t;

        private double TransferTime(double time)
        {
            if (mEase != Ease.Unset)
            {
                return EaseManager.Evaluate(mEase, null, (float)time, 1, 1, 1);
            }
            else if (mCustomEase != null)
            {
                return mCustomEase.Invoke(time);
            }
            else
            {
                return time;
            }
        }

        // 0.02%
        public static double EaseInOutQuad_Inverse(double time)
        {
            return EaseInOutQuad_Inverse(time, 1, 1, 1);
        }
        private static double EaseInOutQuad_Inverse(double t, float d, float a, float p)
        {
            if (t < 0.5f)
                return Math.Sqrt(t / 2f);
            else
                return 1f - Math.Sqrt((1f - t) / 2f);
        }

        // 0.0007%
        public static double EaseInOutQuart_Inverse(double time)
        {
            return EaseInOutQuart_Inverse(time, 1, 1, 1);
        }
        private static double EaseInOutQuart_Inverse(double x, float d, float a, float p)
        {
            if (x < 0.5f)
                return Math.Pow(x / 8f, 0.25f);
            else
                return 1f - Math.Pow((1f - x) / 8f, 0.25f);
        }
    }
}