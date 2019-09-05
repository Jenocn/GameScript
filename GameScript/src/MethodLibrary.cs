/*
 * By Jenocn
 * https://jenocn.github.io/
 */

using System.Collections.Generic;

namespace gs.compiler {
	public static class MethodLibrary {

		private static MethodPool _methodPool = new MethodPool();

		static MethodLibrary() {
			_methodPool.AddMethod("print", _std_print);
			_methodPool.AddMethod("strlen", _std_strlen);
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
			return _methodPool.AddMethod(name, func);
		}

		public static bool Contains(string name) {
			return _methodPool.Contains(name);
		}

		public static ScriptValue Execute(string name, List<ScriptValue> args) {
			return _methodPool.Execute(name, args);
		}
	}
}