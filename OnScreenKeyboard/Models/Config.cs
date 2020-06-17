using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace OnScreenKeyboard.Models
{
    public class Config
    {
        private static readonly XmlSerializer xmlSerializer = new XmlSerializer(typeof(Config));

        public ScreenUnit? Top { get; set; }
        public ScreenUnit? Bottom { get; set; }
        public ScreenUnit? Left { get; set; }
        public ScreenUnit? Right { get; set; }
        public ScreenUnit? Height { get; set; }
        public ScreenUnit? Width { get; set; }

        public bool DockAtBottom { get; set; }
        public bool BlurBackground { get; set; } = true;
        public double WindowOpacity { get; set; } = 0.8;
        public string DefaultLayoutName { get; set; }

        public List<string> IncludeLayouts { get; set; }
        [XmlIgnore]
        public List<KeyboardLayout> Layouts { get; set; }
        [XmlIgnore]
        public KeyboardLayout DefaultLayout => Layouts.FirstOrDefault(x => x.LanguageCode == DefaultLayoutName);

        public Config()
        {
            IncludeLayouts = new List<string>();
            Layouts = new List<KeyboardLayout>();
        }

        public static Config Load(string path)
        {
            using (var stream = File.OpenRead(path))
            {
                var config = (Config)xmlSerializer.Deserialize(stream);
                if (config.IncludeLayouts != null)
                {
                    foreach (var includePath in config.IncludeLayouts)
                    {
                        var fullpath = includePath;
                        if (!Path.IsPathRooted(includePath))
                            fullpath = Path.Combine(Path.GetDirectoryName(path), fullpath);
                        config.Layouts.Add(KeyboardLayout.Load(fullpath));
                    }
                }

                return config;
            }
        }
    }
}
