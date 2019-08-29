/*
 * By Jenocn
 * https://jenocn.github.io/
*/

using System.Collections.Generic;

using gs.compiler;

namespace gs {
	public class VMFunction {
		private ScriptMethod _scriptMethod = null;

		public VMFunction(string src, VMFunction parent = null) {
			if (parent != null) {
				_scriptMethod = new ScriptMethod(src, parent._scriptMethod);
			} else {
				_scriptMethod = new ScriptMethod(src);
			}
		}
		public VMFunction(ScriptMethod method) {
			_scriptMethod = method;
		}

		public VMValue Execute(List<VMValue> args = null) {
			var scriptArgs = new List<ScriptValue>();
			if (args != null) {
				foreach(var arg in args) {
					scriptArgs.Add(arg.GetMetadata());
				}
			}
			bool bReturn = false;
			ScriptValue retValue = null;
			if (_scriptMethod.Execute(scriptArgs, out bReturn, out retValue)) {
				return new VMValue(retValue);
			}
			return null;
		}

		public VMFunction GetFunction(string name) {
			var method = _scriptMethod.FindMethod(name);
			if (method != null) {
				return new VMFunction(method);
			}
			return null;
		}
	}
}
