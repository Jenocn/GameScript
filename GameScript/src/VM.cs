/*
 * By Jenocn
 * https://jenocn.github.io/
*/

using System;
using gs.compiler;

namespace gs {
	public static class VM {

		public static VMFunction Load(string src, VMFunction parent = null) {
			return new VMFunction(src, parent);
		}

		public static void SetLogger(Action<string> logCall) {
			Logger.SetLoggerFunc(logCall);
		}

		public static void Using(string name, string src) {
			UsingMemory.Add(name, new ScriptMethod(src));
		}
	}
}