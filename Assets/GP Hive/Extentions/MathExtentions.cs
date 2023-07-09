using UnityEngine;


public static class MathExtentions
{
    /// <summary>
    /// Checks probablity over given percentage.
    /// </summary>
    /// <param name="percentage">Percentage</param>
    /// <returns></returns>
    public static bool Probability(int percentage)
    {
        int _random = Random.Range(0, 100);

        return _random <= percentage;
    }

    /// <summary>
    /// Checks probablity over given percentage.
    /// </summary>
    /// <param name="percentage">Percentage</param>
    /// <returns></returns>
    public static bool Probability(float percentage)
    {
        float _random = Random.Range(0, 100);

        return _random <= percentage;
    }
    
        private static string[] _suffix = { "", "K", "M", "B", "T", "Q", "QU", "S" };
    
        public static string ConvertToKBM(this float value)
        {
            var _count = 0;
            while (value >= 1000f)
            {
                _count++;
                value /= 1000f;
            }
    
            return value < .01f && value != 0 ? $"{value:0.000}{_suffix[_count]}" : $"{value:F}{_suffix[_count]}";
        }
}

