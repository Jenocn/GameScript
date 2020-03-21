using System;
using System.Collections.Generic;
using System.IO;
using gs;
using gs.compiler.tool;
using std;

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
                    break;
                }

                if (args.Length > 0) {
                    config.main = args[0];
                }

                string mainSrc = "";
                if (!ReadSrc(config.main, out mainSrc)) {
                    Console.WriteLine("Not found '" + config.main + "'!");
                    break;
                }

                foreach (var item in config.list) {
                    string src = "";
                    if (ReadSrc(item, out src)) {
                        var itemSplits = item.Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
                        var name = itemSplits[0].Replace('/', '.').Replace('\\', '.');
                        VM.AddUsing(name, src);
                    }
                }

                LoadModule();
                LoadScript(mainSrc);
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
                var tempText = text.Substring(listPos + 5).Trim();
                var cbPos = tempText.IndexOf('[');
                if (cbPos != -1) {
                    var cePos = GrammarTool.ReadPairSignPos(tempText, cbPos + 1, '[', ']');
                    if (cePos > cbPos) {
                        listText = tempText.Substring(cbPos + 1, cePos - cbPos - 1);
                    }
                }
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

        private static void LoadModule() {
            VM.AddModule(new StandardModule());
        }

        private static void LoadScript(string src) {
            var method = VM.Load(src);
            App.Run(method);
            method.Execute();
        }
    }
}