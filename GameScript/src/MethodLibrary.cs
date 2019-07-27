/*
 * By Jenocn
 * https://jenocn.github.io/
*/

using System.Collections.Generic;

namespace gs.compiler {
	public static class MethodLibrary {
		private static Dictionary<string, System.Func<List<ScriptValue>, ScriptValue>> _methods = new Dictionary<string, System.Func<List<ScriptValue>, ScriptValue>>();

		static MethodLibrary() {
			_methods.Add("print", _std_print);
		}

		private static ScriptValue _std_print(List<ScriptValue> args) {
			string message = "";
			if (args != null && args.Count > 0) {
				if (args[0] != null) {
					var value = args[0].GetValue();
					if (value != null) {
						message = value.ToString();
					}
				}
			}
			Logger.Log(message);
			return null;
		}

		public static bool RegisterMethod(string name, System.Func<List<ScriptValue>, ScriptValue> func) {
			if (Container(name)) {
				return false;
			}
			_methods.Add(name, func);
			return true;
		}

		public static bool Container(string name) {
			return _methods.ContainsKey(name);
		}

		public static ScriptValue Execute(string name, List<ScriptValue> args) {
			System.Func<List<ScriptValue>, ScriptValue> method = null;
			if (_methods.TryGetValue(name, out method)) {
				return method.Invoke(args);
			}
			return null;
		}
	}
}
