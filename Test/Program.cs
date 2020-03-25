using System.Collections.Generic;
using System.IO;
using gs;


namespace Test {
	class Program {
		static void Main(string[] args) {
            VM.AddModule(new gs.std.StandardModule());

            var text = File.ReadAllText("../../main3.gs");
            VM.Load(text).Execute();


            // var text = File.ReadAllText("../../tmptest.gs");
            // var func = gs.VM.Load(text);
            // func.Execute();

			//var text = File.ReadAllText("../../student.gs");
			//gs.VM.AddUsing("student", text);
			//var mainFunc = gs.VM.Load(File.ReadAllText("../../main.gs"));
			//var mainFunc2 = gs.VM.Load(File.ReadAllText("../../main2.gs"));
   //         mainFunc.Execute();
   //         mainFunc2.Execute();

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
