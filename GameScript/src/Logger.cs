using System;

namespace gs.compiler {
	public static class Logger {

		private static Action<string> _func = null;

		public static void SetLoggerFunc(Action<string> func) {
			_func = func;
		}

		public static void Log(string message) {
			if (_func != null) {
				_func(message);
			} else {
				Console.WriteLine(message);
			}
		}

		public static void Error(string src, string message = "") {
			if (_func != null) {
				_func(" ");
				_func(string.Format("> Error:{0}", message));
				_func(src);
				_func("--------------------------------");
			} else {
				Console.WriteLine(" ");
				Console.WriteLine("> Error:{0}", message);
				Console.WriteLine(src);
				Console.WriteLine("--------------------------------");
			}
		}
	}
}
