using System;
using System.Globalization;

namespace PahlBit
{
    /// <summary>
    /// 스펙 테이블 데이터에서 초기값 + (스텝 * 포인트) 형태의 데이터 포멧을 하나의 타입으로 다루기 편하게 하기 위함
    /// </summary>
    public struct BaseStepPair<T> where T : struct, IConvertible
    {
        public T mBase;
        public T mStep;

        public BaseStepPair(T _base, T _step)
        {
            mBase = _base;
            mStep = _step;
        }

        // 입력 예시: "100+20", "15.5+3.2"
        public static BaseStepPair<T> Parse(string str)
        {
            if (string.IsNullOrWhiteSpace(str))
                throw new ArgumentException("Input string is null or whitespace.");

            // 공백 제거 (전체적으로 깔끔하게)
            str = str.Trim();

            // '+' 기준으로 분리 (중간 공백 제거 허용: e.g. "100 + 20")
            string[] parts = str.Split('+');

            // 정확히 2개일 때만 허용
            if (parts.Length != 2)
                throw new FormatException($"Invalid format: '{str}'. Expected format 'Base+Step'.");

            // 각 파트 앞뒤 공백 제거
            string basePart = parts[0].Trim();
            string stepPart = parts[1].Trim();

            if (basePart.Length == 0 || stepPart.Length == 0)
                throw new FormatException($"Invalid format: '{str}'. Empty base or step value.");

            T baseValue;
            T stepValue;

            try
            {
                baseValue = ConvertToT(basePart);
                stepValue = ConvertToT(stepPart);
            }
            catch (Exception ex)
            {
                throw new FormatException($"Failed to parse base value '{basePart}','{stepPart}' as type {typeof(T).Name}.", ex);
            }

            return new BaseStepPair<T>(baseValue, stepValue);
        }

        // 문자열을 T 타입으로 변환
        private static T ConvertToT(string s)
        {
            // s를 double로 파싱한 뒤 T로 변환
            double val = double.Parse(s, CultureInfo.InvariantCulture);
            return (T)Convert.ChangeType(val, typeof(T), CultureInfo.InvariantCulture);
        }

        public override string ToString()
        {
            return $"{mBase}+{mStep}";
        }

        // Base + Step * points
        public T GetValue(int points)
        {
            double baseVal = mBase.ToDouble(CultureInfo.InvariantCulture);
            double stepVal = mStep.ToDouble(CultureInfo.InvariantCulture);

            double result = baseVal + (stepVal * points);

            return (T)Convert.ChangeType(result, typeof(T), CultureInfo.InvariantCulture);
        }
    }
}