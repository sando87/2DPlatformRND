using System;
using System.Globalization;
using DG.Tweening;
using DG.Tweening.Core.Easing;

namespace PahlBit
{
    /// <summary>
    /// 스펙 테이블 데이터에서 범위 초기값 + (스텝 * 포인트) 형태의 데이터 포멧을 하나의 타입으로 다루기 편하게 하기 위함
    /// </summary>
    public class ParseValue
    {
        public float mBaseMin;
        public float mBaseMax;
        public Ease mEase = Ease.Linear;
        public float mStepByPoint;
        public float mStepByLevel;

        public ParseValue(float _min, float _max, float _stepByPoint, float _stepByLevel, Ease _ease)
        {
            mBaseMin = _min;
            mBaseMax = _max;
            mStepByPoint = _stepByPoint;
            mStepByLevel = _stepByLevel;
            mEase = _ease;
        }

        // 입력 string 예시: "3~5@InOutQuad+0.2"
        public static ParseValue Parse(string str)
        {
            float min = 0f, max = 0f, stepByPoint = 0f, stepByLevel = 0f;
            Ease ease = Ease.Linear;

            string work = str;

            // -------------------------------
            // 1) STEP 파싱: "+0.2" or "-0.2"
            // -------------------------------
            while (true)
            {
                int plusIdx = work.IndexOf('+');
                if (plusIdx >= 0)
                {
                    string stepStr = work.Substring(plusIdx + 1);
                    if (stepStr.Contains("L"))
                        stepByLevel = float.Parse(stepStr.Replace("L", "").Trim(), CultureInfo.InvariantCulture);
                    else
                        stepByPoint = float.Parse(stepStr, CultureInfo.InvariantCulture);

                    work = work.Substring(0, plusIdx); // 나머지 부분만 유지
                    continue;
                }
                int minusIdx = work.IndexOf('-');
                if (minusIdx >= 0)
                {
                    string stepStr = work.Substring(minusIdx + 1);
                    if (stepStr.Contains("L"))
                        stepByLevel = -float.Parse(stepStr.Replace("L", "").Trim(), CultureInfo.InvariantCulture);
                    else
                        stepByPoint = -float.Parse(stepStr, CultureInfo.InvariantCulture);

                    work = work.Substring(0, minusIdx); // 나머지 부분만 유지
                    continue;
                }

                break;
            }

            // -------------------------------
            // 2) EASE 파싱: "@InOutQuad"
            // -------------------------------
            int atIdx = work.IndexOf('@');
            if (atIdx >= 0)
            {
                string easeStr = work.Substring(atIdx + 1);
                if (Enum.TryParse(easeStr, out Ease parsedEase))
                    ease = parsedEase;

                work = work.Substring(0, atIdx); // 범위 부분만 남기기
            }

            // -------------------------------
            // 3) RANGE / SINGLE VALUE 파싱
            // -------------------------------
            if (work.Contains("~"))
            {
                var parts = work.Split('~');
                if (parts.Length != 2)
                    throw new FormatException("잘못된 범위 형식입니다.");

                min = float.Parse(parts[0], CultureInfo.InvariantCulture);
                max = float.Parse(parts[1], CultureInfo.InvariantCulture);
            }
            else
            {
                // 범위가 없으면 단일값
                min = float.Parse(work, CultureInfo.InvariantCulture);
                max = min;
            }

            return new ParseValue(min, max, stepByPoint, stepByLevel, ease);
        }

        public override string ToString()
        {
            return base.ToString();
        }

        public float GetValue()
        {
            return mBaseMin;
        }
        public float GetValueByPoint(int points)
        {
            float result = mBaseMin + (mStepByPoint * points);
            return result;
        }
        public float GetValueByLevel(int level)
        {
            float result = mBaseMin + (mStepByLevel * level);
            return result;
        }
        public float GetValueByBoth(int point, int level)
        {
            float result = mBaseMin + (mStepByPoint * point) + (mStepByLevel * level);
            return result;
        }
        public float GetValueInRange(float normalizedTime)
        {
            float t = TransferTime(normalizedTime);
            return Lerp(mBaseMin, mBaseMax, t);
        }
        private float Lerp(float a, float b, float t) => a + (b - a) * t;

        private float TransferTime(float time)
        {
            if (mEase != Ease.Unset)
            {
                return EaseManager.Evaluate(mEase, null, (float)time, 1, 1, 1);
            }
            else
            {
                return time;
            }
        }
    }
}