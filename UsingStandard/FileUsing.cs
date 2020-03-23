using System.Collections.Generic;
using System.IO;
using gs;

namespace std.file {
	public class FileUsing : VMUsing<FileUsing> {
		public override bool OnExecuteUsing() {

			bool ret = true;

			ret &= RegisterFunction("file.readText", (List<VMValue> args) => {
				if (args.Count == 0) {
					return null;
				}
				if (!args[0].IsString()) { return null; }
				if (!File.Exists(args[0].ToString())) {
					return null;
				}
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
				if (!File.Exists(args[0].ToString()) || File.Exists(args[1].ToString())) {
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
				if (!File.Exists(args[0].ToString()) || File.Exists(args[1].ToString())) {
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
				if (!File.Exists(args[0].ToString())) {
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

			ret &= RegisterFunction("file.list", (List<VMValue> args) => {
				if (args.Count < 1) {
					gs.compiler.Logger.Error("file.list");
					return VMValue.NULL;
				}
				string path = "./";
				string searchPattern = "*";
				bool bDeep = false;
				if (args.Count >= 1 && args[0].IsString()) {
					path = args[0].ToString();
				}
				if (args.Count >= 2 && args[1].IsString()) {
					searchPattern = args[1].ToString();
				}
				if (args.Count >= 3 && args[2].IsBool()) {
					bDeep = args[2].GetBool();
				}
				var tempList = Directory.GetFiles(args[0].ToString(), searchPattern, bDeep ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);
				var retList = new List<VMValue>();
				foreach (var item in tempList) {
					retList.Add(new VMValue(item));
				}
				return new VMValue(retList);
			});

			return ret;
		}
	}
}