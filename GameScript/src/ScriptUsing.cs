/*
 * By Jenocn
 * https://jenocn.github.io/
 */

using System.Collections.Generic;

namespace gs.compiler {
	public class ScriptUsing {
		private ScriptMethod _method = null;

		public static ScriptUsing Create(ScriptUsing value) {
			if (value == null) {
				return null;
			}

			var ret = ScriptMethod.NewMethodForUsing(value._method);
			if (ret == null) {
				return null;
			}
			
			return new ScriptUsing(ret);
		}

		public ScriptUsing(string src) {
			_method = new ScriptMethod(src);
		}

		private ScriptUsing(ScriptMethod method) {
			_method = method;
		}

		public ScriptUsing() {
			_method = new ScriptMethod("");
		}

		public bool ExecuteUsing() {
			if (!_method.IsExecuted()) {
				return _method.Execute(null);
			}
			return true;
		}

		public ScriptMethod GetScriptMethod() {
			return _method;
		}

		public bool RegisterMethod(string name, System.Func<List<ScriptValue>, ScriptValue> func) {
			return _method.RegisterMethod(name, func);
		}
	}
}