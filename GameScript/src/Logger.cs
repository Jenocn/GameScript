/*
 * By Jenocn
 * https://jenocn.github.io/
 */

using System;

namespace gs.compiler {
	public static class Logger {

		private static Action<string, bool> _logFunc = null;

		static Logger() {
			SetLoggerFunc(null);
		}

		public static void SetLoggerFunc(Action<string, bool> func) {
			if (_logFunc != null) {
				_logFunc = func;
			} else {
				_logFunc = (string message, bool bError) => {
					if (bError) {
						Console.ForegroundColor = ConsoleColor.Red;
					} else {
						Console.ForegroundColor = ConsoleColor.Yellow;
					}
					Console.WriteLine(message);
					Console.ResetColor();
				};
			}
		}

		public static void Log(string message) {
			_logFunc(message, false);
		}

		public static void Error(string src, string message = "") {
			_logFunc(" ", true);
			_logFunc(string.Format("> Error:{0}", message), true);
			_logFunc(src, true);
			_logFunc("--------------------------------", true);
		}
	}
}