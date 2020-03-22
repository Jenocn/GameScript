using System.Collections.Generic;
using gs;

namespace std.module {
	public class ModuleUsing : VMUsing<ModuleUsing> {
		public override bool OnExecuteUsing() {
			bool ret = true;

			ret &= RegisterFunction("module.addUsing", (List<VMValue> args) => {
				if (args.Count < 2) {
					return VMValue.FALSE;
				}
				if (!args[0].IsString() || !args[1].IsString()) {
					return VMValue.FALSE;
				}
				if (string.IsNullOrEmpty(args[0].ToString())) {
					return VMValue.FALSE;
				}
				VM.AddUsing(args[0].ToString(), args[1].ToString());
				return VMValue.TRUE;
			});

			return ret;
		}
	}
}