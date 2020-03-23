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
			_methodPool.AddMethod("len", _std_len);
		}

		private static ScriptValue _std_print(List<ScriptValue> args) {
			if (args.Count > 0) {
				Logger.Log(args[0].ToString());
			}
			return null;
		}

		private static ScriptValue _std_strlen(List<ScriptValue> args) {
			if (args.Count == 0) {
				return null;
			}
			if (args[0].GetValueType() != ScriptValueType.String) {
				return null;
			}
			return ScriptValue.Create(args[0].ToString().Length);
		}

		private static ScriptValue _std_len(List<ScriptValue> args) {
			if (args.Count == 0) {
				return null;
			}
			if (args[0].GetValueType() != ScriptValueType.List) {
				return null;
			}
			var tempList = (List<ScriptValue>) args[0].GetValue();
			return ScriptValue.Create(tempList.Count);
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