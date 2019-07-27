using System;
using System.Collections.Generic;
using gs.compiler;

namespace gs {
	public static class VM {
		public static ScriptValue Execute(string src) {
			var scriptMethod = new ScriptMethod(src);
			bool bReturn = false;
			ScriptValue returnValue = new ScriptValue();
			scriptMethod.Execute(null, out bReturn, out returnValue);
			return returnValue;
		}

		public static bool RegisterMethod(string name, Func<List<ScriptValue>, ScriptValue> func) {
			return MethodLibrary.RegisterMethod(name, func);
		}

		public static void SetLogger(Action<string> logCall) {
			Logger.SetLoggerFunc(logCall);
		}
	}
}