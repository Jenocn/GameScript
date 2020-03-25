using System.Collections.Generic;
using gs.compiler;

namespace gs.std.str {
	public class StringUsing : VMUsing<StringUsing> {
		public override bool OnExecuteUsing() {
			bool ret = true;

			ret &= RegisterFunction("string.split", (List<VMValue> args) => {
				if (args.Count < 2) {
					Logger.Error("string.split");
					return VMValue.NULL;
				}
				if (!args[0].IsString() || !args[1].IsString()) {
					Logger.Error("string.split");
					return VMValue.NULL;
				}
				var retList = args[0].ToString().Split(new string[] { args[1].ToString() },
					System.StringSplitOptions.None);
				List<VMValue> tempList = new List<VMValue>();
				foreach (var item in retList) {
					tempList.Add(new VMValue(item));
				}
				return new VMValue(tempList);
			});
			ret &= RegisterFunction("string.substr", (List<VMValue> args) => {
				if (args.Count < 3) {
					Logger.Error("string.substr");
					return VMValue.NULL;
				}
				if (!args[0].IsString() || !args[1].IsNumber() || !args[2].IsNumber()) {
					Logger.Error("string.substr");
					return VMValue.NULL;
				}
				var str = args[0].ToString();
				int start = args[1].GetInt();
				if (start < 0) {
					start = 0;
				}
				int length = args[2].GetInt();
				if (start + length > str.Length) {
					length = str.Length - start;
				}
				return new VMValue(str.Substring(start, length));
			});
			ret &= RegisterFunction("string.replace", (List<VMValue> args) => {
				if (args.Count < 3) {
					Logger.Error("string.replace");
					return VMValue.NULL;
				}
				if (!args[0].IsString() || !args[1].IsString() || !args[2].IsString()) {
					Logger.Error("string.replace");
					return VMValue.NULL;
				}
				return new VMValue(args[0].ToString().Replace(args[1].ToString(), args[2].ToString()));
			});
			ret &= RegisterFunction("string.contains", (List<VMValue> args) => {
				if (args.Count < 2) {
					Logger.Error("string.contains");
					return VMValue.NULL;
				}
				if (!args[0].IsString() || !args[1].IsString()) {
					Logger.Error("string.contains");
					return VMValue.NULL;
				}
				return new VMValue(args[0].ToString().Contains(args[1].ToString()));
			});
			ret &= RegisterFunction("string.trim", (List<VMValue> args) => {
				if (args.Count < 1) {
					Logger.Error("string.trim");
					return VMValue.NULL;
				}
				if (!args[0].IsString()) {
					Logger.Error("string.trim");
					return VMValue.NULL;
				}
				return new VMValue(args[0].ToString().Trim());
			});

			return ret;
		}
	}
}