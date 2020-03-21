using System;
using System.Collections.Generic;
using gs.compiler;

namespace gs {
	public abstract class VMUsing<T> : ScriptUsing where T : VMUsing<T>, new() {

		public abstract override bool OnExecuteUsing();

		public override ScriptUsing New() {
			return new T();
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
				}
				return null;
			};
			return GetScriptMethod().RegisterMethod(name, libFunc);
		}

		public bool RegisterFunction(string name, Action<List<VMValue>> func) {
			return RegisterFunction(name, (List<VMValue> args) => {
				func(args);
				return null;
			});
		}
	}
}