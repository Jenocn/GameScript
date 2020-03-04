﻿using System;
using gs.compiler;

namespace GameScriptConsole {
    class MainClass {

        private static void ExecuteFile(string path) {
            var fileExecuter = new FileExecuter();
            if (System.IO.File.Exists(path)) {
                var file = System.IO.File.OpenText(path);
                var src = file.ReadToEnd();
                file.Close();
                fileExecuter.Execute(src);
                return;
            }
            Console.WriteLine("\"" + path + "\"" + "is not a \"GameScript\" file!");

        }
        public static void Main(string[] args) {

            Console.WriteLine("GameScript:");

            if (args.Length > 0) {
                ExecuteFile(args[0]);
            }

            var lineExecuter = new LineExecuter();

            while (true) {
                Console.Write(">");
                var line = Console.ReadLine();
                line = line.Trim();
                if (line == "cls") {
                    Console.Clear();
                    lineExecuter.Clear();
                    Console.WriteLine("GameScript:");
                    continue;
                }
                if (line == "quit") {
                    break;
                }
                if (line == "src") {
                    var src = lineExecuter.GetSrc();
                    if (!string.IsNullOrEmpty(src)) {
                        Console.Write(src);
                        if (src[src.Length - 1] != '\n') {
                            Console.WriteLine();
                        }
                    }
                    continue;
                }
                var openPos = line.IndexOf("open ");
                if (!string.IsNullOrEmpty(line) && openPos == 0) {
                    ExecuteFile(line.Substring(5, line.Length - 5).Trim());
                    continue;
                }

                var result = lineExecuter.Execute(line);
                if (!string.IsNullOrEmpty(result)) {
                    Console.WriteLine(result.ToString());
                }
            }
        }
    }
}