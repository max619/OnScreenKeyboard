using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Serialization;

namespace OnScreenKeyboard.Models
{
    public enum ScreenUnitType
    {
        Pixels,
        Percent,
    }

    public struct ScreenUnit
    {
        [XmlAttribute("Value")]
        public float Value { get; set; }
        [XmlAttribute("Type")]
        public ScreenUnitType UnitType { get; set; }

        public ScreenUnit(float value, ScreenUnitType type)
        {
            Value = value;
            UnitType = type;
        }

        public float TranslateToPixelPoint(float offset, float len)
        {
            float translatedValue = 0;
            switch(UnitType)
            {
                case ScreenUnitType.Percent:
                    translatedValue = TranslateFromPercents(len);
                    break;
                case ScreenUnitType.Pixels:
                    translatedValue = TranslateFromPixels(len);
                    break;
                default:
                    throw new InvalidOperationException("Not suuported pixel type");
            }

            return offset + translatedValue;
        }

        private float TranslateFromPercents(float len)
        {
            return len * Value / 100.0f;
        }

        private float TranslateFromPixels(float len)
        {
            return Value;
        }
    }
}
