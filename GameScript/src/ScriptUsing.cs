/*
 * By Jenocn
 * https://jenocn.github.io/
 */

using System.Collections.Generic;

namespace gs.compiler {
	public class ScriptUsing {
		private ScriptMethod _method = new ScriptMethod("");

		public static ScriptUsing Create(string src) {
			return Create<ScriptUsing>(src);
		}
		public static ScriptUsing Create() {
			return new ScriptUsing();
		}
		public static T Create<T>(string src) where T : ScriptUsing, new() {
			var scriptUsing = new T();
			scriptUsing.SetScriptMethod(src);
			return scriptUsing;
		}
		public static T Create<T>() where T : ScriptUsing, new() {
			return new T();
		}

		public ScriptUsing Clone() {
			return Clone<ScriptUsing>();
		}
		public T Clone<T>() where T : ScriptUsing, new() {
			var ret = ScriptMethod.NewMethodForUsing(_method);
			if (ret != null) {
				var su = new T();
				su.SetScriptMethod(ret);
				return su;
			}
			return null;
		}

		public void SetScriptMethod(string src) {
			_method = new ScriptMethod(src);
		}
		public void SetScriptMethod(ScriptMethod method) {
			_method = method;
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