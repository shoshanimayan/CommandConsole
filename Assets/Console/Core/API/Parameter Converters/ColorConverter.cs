using UnityEngine;

namespace Console.Internal.ParameterConverters
{
    [ParameterConverter(typeof(Color))]
    public class ColorConverter : IParameterConverter
    {
        public object Convert(string userValue)
        {
            if (userValue == null) return null;
            Color color;
            if (ColorUtility.TryParseHtmlString(userValue, out color)) return color;
            else if (ColorUtility.TryParseHtmlString($"#{userValue}", out color)) return color;
            else return null;
        }
    }
}