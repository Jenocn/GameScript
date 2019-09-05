/*
 * By Jenocn
 * https://jenocn.github.io/
 */

using System;
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
				foreach (var arg in args) {
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

		public bool RegisterFunction(string name, Func<List<VMValue>, VMValue> func) {
			Func<List<ScriptValue>, ScriptValue> libFunc = (List<ScriptValue> args) => {
				var retArgs = new List<VMValue>();
				if (args != null) {
					foreach (var arg in args) {
						retArgs.Add(new VMValue(arg));
					}
				}
				var obj = func(retArgs);
				if (obj != null) {
					return obj.GetMetadata();
				} else {
					return null;
				}
			};
			return _scriptMethod.RegisterMethod(name, libFunc);
		}
		public bool RegisterFunction(string name, Action<List<VMValue>> func) {
			return RegisterFunction(name, (List<VMValue> args) => {
				func(args);
				return null;
			});
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