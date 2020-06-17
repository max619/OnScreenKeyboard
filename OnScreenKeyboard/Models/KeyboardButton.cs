using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Xml.Serialization;

namespace OnScreenKeyboard.Models
{
    public class KeyboardButton
    {
        private string _displayChar;

        [XmlAttribute("DisplayChar")]
        public string DisplayChar { get => string.IsNullOrEmpty(_displayChar) ? Char : _displayChar; set => _displayChar = value; }
        [XmlAttribute("Char")]
        public string Char { get; set; }
        [XmlAttribute("KeyCode")]
        public int KeyCode { get; set; }
        [XmlAttribute("PredefinedKey")]
        public string PredefinedKey { get; set; }
        [XmlAttribute("Width")]
        public double Width { get; set; } = double.NaN;
        [XmlAttribute("Height")]
        public double Height { get; set; } = double.NaN;
        [XmlIgnore]
        public ICommand Command { get; set; }
    }
}
