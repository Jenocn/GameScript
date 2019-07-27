using System;
using System.Collections.Generic;
using gs.compiler;

namespace gs {
	public static class VM {

		private static ScriptMethod _vmSpace = null;

		public static ScriptValue ExecuteMethod(string name, List<ScriptValue> args = null) {
			if (_vmSpace != null) {
				var method = _vmSpace.FindMethod(name);
				if (method != null) {
					bool bReturn = false;
					ScriptValue returnValue = null;
					method.Execute(args, out bReturn, out returnValue);
					return returnValue;
				}
			}
			return new ScriptValue();
		}

		public static void ExecuteCache(string src) {
			if (_vmSpace == null) {
				_vmSpace = new ScriptMethod(src);
			} else {
				_vmSpace.Parse(src);
			}
			bool bReturn = false;
			ScriptValue returnValue = new ScriptValue();
			_vmSpace.Execute(null, out bReturn, out returnValue);
		}

		public static void ClearCache() {
			_vmSpace = null;
		}

		public static void Execute(string src) {
			var scriptMethod = new ScriptMethod(src, _vmSpace);
			bool bReturn = false;
			ScriptValue returnValue = new ScriptValue();
			scriptMethod.Execute(null, out bReturn, out returnValue);
		}

		public static bool RegisterMethod(string name, Func<List<ScriptValue>, ScriptValue> func) {
			return MethodLibrary.RegisterMethod(name, func);
		}

		public static void SetLogger(Action<string> logCall) {
			Logger.SetLoggerFunc(logCall);
		}
	}
}