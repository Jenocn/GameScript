/*
 * By Jenocn
 * https://jenocn.github.io/
 */

using System;

namespace gs.compiler {
	public static class Logger {

		private static Action<string> _logFunc = null;

		static Logger() {
			SetLoggerFunc(null);
		}

		public static void SetLoggerFunc(Action<string> func) {
			if (_logFunc != null) {
				_logFunc = func;
			} else {
				_logFunc = (string message) => {
					Console.WriteLine(message);
				};
			}
		}

		public static void Log(string message) {
			_logFunc(message);
		}

		public static void Error(string src, string message = "") {
			_logFunc(" ");
			_logFunc(string.Format("> Error:{0}", message));
			_logFunc(src);
			_logFunc("--------------------------------");
		}
	}
}