using System;
using System.Collections.Generic;
using gs;

namespace std.console {
	public class ConsoleUsing : VMUsing<ConsoleUsing> {
		public override bool OnExecuteUsing() {
			bool ret = true;

			ret &= RegisterFunction("console.input", (List<VMValue> args) => {
				foreach (var item in args) {
					Console.WriteLine(item);
				}
				return new VMValue(Console.ReadLine());
			});
			ret &= RegisterFunction("console.inputNum", (List<VMValue> args) => {
				foreach (var item in args) {
					Console.WriteLine(item);
				}
				var value = Console.ReadLine();
				double result = 0;
				if (double.TryParse(value, out result)) {
					return new VMValue(result);
				}
				return new VMValue(0);
			});
			ret &= RegisterFunction("console.log", (List<VMValue> args) => {
				foreach (var item in args) {
					Console.WriteLine(item);
				}
			});

			return ret;
		}
	}
}