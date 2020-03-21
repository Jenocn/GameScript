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

		public static void AddUsing(string name, string src) {
			UsingMemory.Add(name, ScriptUsing.Create(src));
		}

		public static void AddUsing<T>(string name, VMUsing<T> value) where T : VMUsing<T>, new() {
			UsingMemory.Add(name, value);
		}

		public static void AddModule(VMModuleBase module) {
			module.OnModuleLoad();
		}
	}
}