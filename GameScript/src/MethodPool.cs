/*
 * By Jenocn
 * https://jenocn.github.io/
 */

using System.Collections.Generic;

namespace gs.compiler {

	public class MethodPool {
		private Dictionary<string, ScriptMethod> _methods = new Dictionary<string, ScriptMethod>();
		public bool AddMethod(string name, System.Func<List<ScriptValue>, ScriptValue> func) {
			if (Contains(name)) {
				return false;
			}
			_methods.Add(name, new ScriptMethod(func));
			return true;
		}

		public bool Contains(string name) {
			return _methods.ContainsKey(name);
		}

		public ScriptMethod GetMethod(string name) {
			ScriptMethod value = null;
			_methods.TryGetValue(name, out value);
			return value;
		}

		public ScriptValue Execute(string name, List<ScriptValue> args) {
			ScriptMethod method = null;
			if (_methods.TryGetValue(name, out method)) {
				ScriptValue result = null;
				bool bResult = true;
				method.Execute(args, out bResult, out result);
				if (result != null) {
					return result;
				}
			}
			return ScriptValue.NULL;
		}
	}
}