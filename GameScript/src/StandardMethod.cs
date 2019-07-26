using System.Collections.Generic;

namespace gs.compiler {
	public static class StandardMethod {
		private static Dictionary<string, System.Func<List<ScriptValue>, ScriptValue>> _methods = new Dictionary<string, System.Func<List<ScriptValue>, ScriptValue>>();

		static StandardMethod() {
			_methods.Add("print", _std_print);
		}

		private static ScriptValue _std_print(List<ScriptValue> args) {
			string message = "";
			if (args != null && args.Count > 0) {
				var value = args[0].GetValue();
				if (value != null) {
					message = value.ToString();
				}
			}
			Logger.Log(message);
			return null;
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
