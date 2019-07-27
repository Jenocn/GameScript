using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using gs.compiler;

namespace Test {
	class Program {
		static void Main(string[] args) {
			var text = File.ReadAllText("../../test.gs");
			gs.VM.Execute(text);
		}
	}
}
