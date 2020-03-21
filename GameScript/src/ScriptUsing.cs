/*
 * By Jenocn
 * https://jenocn.github.io/
 */

namespace gs.compiler {
	public class ScriptUsing {
		private ScriptMethod _method = new ScriptMethod("");

		public static ScriptUsing Create(string src) {
			var scriptUsing = new ScriptUsing();
			scriptUsing.SetScriptMethod(src);
			return scriptUsing;
		}

		public ScriptUsing Clone() {
			var ret = ScriptMethod.NewMethodForUsing(_method);
			if (ret != null) {
				var su = New();
				su.SetScriptMethod(ret);
				return su;
			}
			return null;
		}

		public virtual ScriptUsing New() {
			return new ScriptUsing();
		}
		public virtual bool OnExecuteUsing() { return true; }

		public void SetScriptMethod(string src) {
			_method = new ScriptMethod(src);
		}
		public void SetScriptMethod(ScriptMethod method) {
			_method = method;
		}

		public bool ExecuteUsing() {
			if (_method.IsExecuted()) {
				return true;
			}
			if (!_method.Execute(null)) {
				return false;
			}
			return OnExecuteUsing();
		}

		public ScriptMethod GetScriptMethod() {
			return _method;
		}
	}
}