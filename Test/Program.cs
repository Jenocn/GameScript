using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using gs.compiler;

namespace Test {
	class Program {
		static void Main(string[] args) {
			var text = File.ReadAllText(@"D:\JenocnDocument\temp\GameScript\GameScript\test.gs");
			var method = new ScriptMethod(text);
			method.Execute(null);
		}
	}
}
