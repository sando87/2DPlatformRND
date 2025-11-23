using System;
using System.Globalization;
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

        public bool IsPercent { get; private set; } = false;

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

        // 예시입력 스트링 "10 ~ 20", "5.5 ~ 15.5", "30% ~ 50%"
        public static RangeType Parse(string str)
        {
            if (string.IsNullOrWhiteSpace(str))
                throw new ArgumentException("Input string is null or whitespace.");

            // 공백 제거 (전체적으로 깔끔하게)
            bool isPercent = str.Contains('%');
            str = str.Trim();
            str = str.Replace("%", "");

            // '~' 기준으로 분리
            string[] parts = str.Split('~');

            // 정확히 2개일 때만 허용
            if (parts.Length != 2)
                throw new FormatException($"Invalid format: '{str}'. Expected format 'Base+Step'.");

            // 각 파트 앞뒤 공백 제거
            string basePart = parts[0].Trim();
            string stepPart = parts[1].Trim();

            if (basePart.Length == 0 || stepPart.Length == 0)
                throw new FormatException($"Invalid format: '{str}'. Empty base or step value.");

            double baseValue;
            double stepValue;

            try
            {
                baseValue = double.Parse(basePart, CultureInfo.InvariantCulture);
                stepValue = double.Parse(stepPart, CultureInfo.InvariantCulture);
            }
            catch (Exception ex)
            {
                throw new FormatException($"Failed to parse base value '{basePart}','{stepPart}'.", ex);
            }

            RangeType ret = new RangeType(baseValue, stepValue);
            ret.IsPercent = isPercent;
            return ret;
        }

        public double GetDouble(double normalizedPos)
        {
            double t = TransferTime(normalizedPos);
            return Lerp(mMin, mMax, t);
        }

        public int GetInt(double normalizedPos)
        {
            int intMin = (int)Math.Ceiling(mMin);
            int intMax = (int)Math.Floor(mMax);
            double t = TransferTime(normalizedPos);
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