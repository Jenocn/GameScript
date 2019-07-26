using System;
using System.Collections.Generic;
using gs.compiler;

namespace gs {
	public static class VM {
		public static ScriptValue Execute(string src) {
			var scriptMethod = new ScriptMethod(src);
			return scriptMethod.Execute(null);
		}

		public static bool RegisterMethod(string name, Func<List<ScriptValue>, ScriptValue> func) {
			return MethodLibrary.RegisterMethod(name, func);
		}

		public static void SetLogger(Action<string> logCall) {
			Logger.SetLoggerFunc(logCall);
		}
	}
}