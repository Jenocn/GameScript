using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using gs.compiler;

namespace Test {
	class Program {
		static void Main(string[] args) {
			ScriptMethod sm = new ScriptMethod("print(msg, arg1, arg2)", "");

			ScriptObject so = new ScriptObject("var value");
		}
	}
}
