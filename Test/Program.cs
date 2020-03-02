using System.Collections.Generic;
using System.IO;


namespace Test {
	class Program {
		static void Main(string[] args) {
			var text = File.ReadAllText("../../tmptest.gs");
			var func = gs.VM.Load(text);
			func.Execute();

			//gs.VM.RegisterFunction("sum", (List<gs.VMValue> tempArgs) => {
			//	double ret = 0;
			//	foreach (var num in tempArgs) {
			//		if (num.IsNumber()) {
			//			ret += num.GetNumber();
			//		}
			//	}
			//	return new gs.VMValue(ret);
			//});
			//gs.VM.Load("var value = sum(1, 2, 3); print(value);").Execute();

			// var tttArgs = gs.compiler.tool.GrammarTool.SplitParams("3 + 5 * 2 + (1 - 5), print(1, 2), 123, print(3, 4)");

			// foreach (var arg in tttArgs) {
			// 	System.Console.WriteLine(arg);
			// }
		}
	}
}
