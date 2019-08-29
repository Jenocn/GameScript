/*
 * By Jenocn
 * https://jenocn.github.io/
*/

using System;
using System.Collections.Generic;
using gs.compiler;

namespace gs {
	public static class VM {

		public static VMFunction Load(string src, VMFunction parent = null) {
			return new VMFunction(src, parent);
		}

		public static bool RegisterFunction(string name, Func<List<VMValue>, VMValue> func) {
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
			return MethodLibrary.RegisterMethod(name, libFunc);
		}
		public static bool RegisterFunction(string name, Action<List<VMValue>> func) {
			return RegisterFunction(name, (List<VMValue> args)=> {
				func(args);
				return null;
			});
		}

		public static void SetLogger(Action<string> logCall) {
			Logger.SetLoggerFunc(logCall);
		}
	}
}