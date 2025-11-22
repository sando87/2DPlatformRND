using System;
using System.Globalization;

namespace PahlBit
{
    /// <summary>
    /// 스펙 테이블 데이터에서 초기값 + (스텝 * 포인트) 형태의 데이터 포멧을 하나의 타입으로 다루기 편하게 하기 위함
    /// </summary>
    public struct BaseStepPair
    {
        public double mBase;
        public double mStep;
        public bool IsPercent { get; private set; }

        public BaseStepPair(double _base, double _step, bool _isPercent = false)
        {
            mBase = _base;
            mStep = _step;
            IsPercent = _isPercent;
        }

        // 입력 예시: "100+20", "15.5+3.2"
        public static BaseStepPair Parse(string str)
        {
            if (string.IsNullOrWhiteSpace(str))
                throw new ArgumentException("Input string is null or whitespace.");

            // 공백 제거 (전체적으로 깔끔하게)
            bool isPercent = str.Contains('%');
            str = str.Trim();
            str = str.Replace("%", "");

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

            double baseValue;
            double stepValue;

            try
            {
                baseValue = ConvertToDouble(basePart);
                stepValue = ConvertToDouble(stepPart);
            }
            catch (Exception ex)
            {
                throw new FormatException($"Failed to parse base value '{basePart}','{stepPart}'.", ex);
            }

            return new BaseStepPair(baseValue, stepValue, isPercent);
        }

        // 문자열을 T 타입으로 변환
        private static double ConvertToDouble(string s)
        {
            // s를 double로 파싱한 뒤 T로 변환
            return double.Parse(s, CultureInfo.InvariantCulture);
            // return (T)Convert.ChangeType(val, typeof(T), CultureInfo.InvariantCulture);
        }

        public override string ToString()
        {
            return $"{mBase}+{mStep}";
        }

        // Base + Step * points
        public double GetValue(int points)
        {
            double result = mBase + (mStep * points);
            return result;
        }
    }
}