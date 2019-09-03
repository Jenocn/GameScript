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
			_methods.Add("strlen", _std_strlen);
		}

		private static ScriptValue _std_print(List<ScriptValue> args) {
			if (args.Count > 0) {
				Logger.Log(args[0].ToString());
			}
			return null;
		}

		private static ScriptValue _std_strlen(List<ScriptValue> args) {
			if (args.Count > 0) {
				return ScriptValue.Create(args[0].ToString().Length);
			}
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
				var result = method.Invoke(args);
				if (result != null) {
					return result;
				}
			}
			return ScriptValue.NULL;
		}
	}
}
