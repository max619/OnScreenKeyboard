using OnScreenKeyboard.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Serialization;

namespace OnScreenKeyboard.Models
{
    public class KeyboardLayout : DependencyObject
    {
        private static readonly XmlSerializer xmlSerializer = new XmlSerializer(typeof(KeyboardLayout));
        [XmlAttribute("LanguageCode")]
        public string LanguageCode { get; set; }
        [XmlAttribute("LanguageNameShort")]
        public string LanguageNameShort { get; set; }
        [XmlAttribute("LanguageName")]
        public string LanguageName { get; set; }
        [XmlArrayItem("Row")]
        public List<List<KeyboardButton>> KeyboardRows { get; set; }

        public void InitButtons(IEnumerable<IKeyboardButtonInitializer> initializers)
        {
            var buttons = KeyboardRows.SelectMany(x => x);
           
            foreach(var button in buttons)
            {
                foreach(var initializer in initializers)
                {
                    if(initializer.CanInitialize(button))
                    {
                        initializer.Initialize(button);
                        break;
                    }
                }
            }
        }

        public static KeyboardLayout Load(string path)
        {
            using (var stream = File.OpenRead(path))
            {
                return (KeyboardLayout)xmlSerializer.Deserialize(stream);
            }
        }
    }
}
