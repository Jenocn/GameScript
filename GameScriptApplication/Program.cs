using System;
using System.Collections.Generic;
using System.IO;
using gs;

namespace GameScriptApplication {
    public class SourcesConfig {
        public string main = "";
        public LinkedList<string> list = new LinkedList<string>();
    }
    class MainClass {
        private static readonly string CONF_SOURCES = "sources.conf";

        public static void Main(string[] args) {
            do {
                // using
                var config = GetConfg();
                if (config == null) {
                    Console.WriteLine("The 'sources.conf' error!");
                    return;
                }

                if (args.Length > 0) {
                    config.main = args[0];
                }

                string mainSrc = "";
                if (!ReadSrc(config.main, out mainSrc)) {
                    Console.WriteLine("Not found '" + config.main + "'!");
                    return;
                }

                foreach (var item in config.list) {
                    string src = "";
                    if (ReadSrc(item, out src)) {
                        var itemSplits = item.Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
                        var name = itemSplits[0].Replace('/', '.').Replace('\\', '.');
                        VM.Using(name, src);
                    }
                }

                VM.Load(mainSrc).Execute();
            } while (false);

            Console.Write("Press any key to continue...");
            Console.ReadLine();
        }

        private static SourcesConfig GetConfg() {
            if (!File.Exists(CONF_SOURCES)) {
                return null;
            }
            var text = File.ReadAllText(CONF_SOURCES);
            var mainPos = text.IndexOf("main:", StringComparison.Ordinal);
            if (mainPos == -1) {
                return null;
            }
            var listPos = text.IndexOf("list:", StringComparison.Ordinal);
            var mainText = "";
            var listText = "";

            if (listPos == -1) {
                mainText = text.Substring(mainPos + 5).Trim();
            } else {
                mainText = text.Substring(mainPos + 5, listPos - mainPos - 5).Trim();
                listText = text.Substring(listPos + 5).Trim();
            }
            if (string.IsNullOrEmpty(mainText)) {
                return null;
            }
            var config = new SourcesConfig();
            config.main = mainText;

            if (!string.IsNullOrEmpty(listText)) {
                var listArrs = listText.Split(new char[] { ',', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var item in listArrs) {
                    config.list.AddLast(item.Trim());
                }
            }

            return config;
        }

        private static bool ReadSrc(string filename, out string src) {
            src = "";
            if (!File.Exists(filename)) {
                return false;
            }
            src = File.ReadAllText(filename);
            return true;
        }
    }
}