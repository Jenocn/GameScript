using System.Collections.Generic;
using System.IO;
using gs;

namespace std.io {
	public class IOUsing : VMUsing<IOUsing> {
		public override bool OnExecuteUsing() {

			bool ret = true;

			ret &= RegisterFunction("file.readText", (List<VMValue> args) => {
				if (args.Count == 0) {
					return null;
				}
				if (!args[0].IsString()) { return null; }
				return new VMValue(File.ReadAllText(args[0].ToString()));
			});
			
			ret &= RegisterFunction("file.writeText", (List<VMValue> args) => {
				if (args.Count < 2) {
					return VMValue.FALSE;
				}
				if (!args[0].IsString() || !args[1].IsString()) {
					return VMValue.FALSE;
				}
				File.WriteAllText(args[0].ToString(), args[1].ToString());
				return VMValue.TRUE;
			});

			ret &= RegisterFunction("file.copy", (List<VMValue> args) => {
				if (args.Count < 2) {
					return VMValue.FALSE;
				}
				if (!args[0].IsString() || !args[1].IsString()) {
					return VMValue.FALSE;
				}
				File.Copy(args[0].ToString(), args[1].ToString());
				return VMValue.TRUE;
			});

			ret &= RegisterFunction("file.move", (List<VMValue> args) => {
				if (args.Count < 2) {
					return VMValue.FALSE;
				}
				if (!args[0].IsString() || !args[1].IsString()) {
					return VMValue.FALSE;
				}
				File.Move(args[0].ToString(), args[1].ToString());
				return VMValue.TRUE;
			});

			ret &= RegisterFunction("file.delete", (List<VMValue> args) => {
				if (args.Count < 1) {
					return VMValue.FALSE;
				}
				if (!args[0].IsString()) {
					return VMValue.FALSE;
				}
				File.Delete(args[0].ToString());
				return VMValue.TRUE;
			});

			ret &= RegisterFunction("file.exists", (List<VMValue> args) => {
				if (args.Count < 1) {
					gs.compiler.Logger.Error("file.exists");
					return VMValue.NULL;
				}
				if (!args[0].IsString()) {
					gs.compiler.Logger.Error("file.exists");
					return VMValue.NULL;
				}
				return new VMValue(File.Exists(args[0].ToString()));
			});

			return ret;
		}
	}
}