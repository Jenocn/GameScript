using System.Collections.Generic;
using gs;
using gs.compiler;

namespace std.list {

	public class ListUsing : VMUsing<ListUsing> {
		public override bool OnExecuteUsing() {

			bool ret = true;

			ret &= RegisterFunction("list.get", (List<VMValue> args) => {
				if (args.Count < 2) {
					Logger.Error("list.get");
					return VMValue.NULL;
				}
				if (!args[0].IsList()) {
					Logger.Error("list.get");
					return VMValue.NULL;
				}
				if (!args[1].IsNumber()) {
					Logger.Error("list.get");
					return VMValue.NULL;
				}
				var list = args[0].GetList();
				var i = args[1].GetInt();
				if (i >= 0 && i < list.Count) {
					return new VMValue(list[i]);
				}
				return VMValue.NULL;
			});
			ret &= RegisterFunction("list.find", (List<VMValue> args) => {
				if (args.Count < 2) {
					Logger.Error("list.find");
					return VMValue.NULL;
				}
				if (!args[0].IsList()) {
					Logger.Error("list.find");
					return VMValue.NULL;
				}
				var list = args[0].GetList();
				for (int i = 0; i < list.Count; ++i) {
					if (ScriptValue.Compare(list[i], args[1].GetMetadata())) {
						return new VMValue(i);
					}
				}
				return new VMValue(0);
			});
			ret &= RegisterFunction("list.contains", (List<VMValue> args) => {
				if (args.Count < 2) {
					Logger.Error("list.contains");
					return VMValue.NULL;
				}
				if (!args[0].IsList()) {
					Logger.Error("list.contains");
					return VMValue.NULL;
				}
				var list = args[0].GetList();
				for (int i = 0; i < list.Count; ++i) {
					if (ScriptValue.Compare(list[i], args[1].GetMetadata())) {
						return VMValue.TRUE;
					}
				}
				return VMValue.FALSE;
			});
			ret &= RegisterFunction("list.add", (List<VMValue> args) => {
				if (args.Count < 2) {
					Logger.Error("list.add");
					return;
				}
				if (!args[0].IsList()) {
					Logger.Error("list.add");
					return;
				}
				var list = args[0].GetList();
				for (int i = 1; i < args.Count; ++i) {
					list.Add(args[i].GetMetadata());
				}
			});
			ret &= RegisterFunction("list.remove", (List<VMValue> args) => {
				if (args.Count < 2) {
					Logger.Error("list.remove");
					return;
				}
				if (!args[0].IsList()) {
					Logger.Error("list.remove");
					return;
				}
				var list = args[0].GetList();
				for (int i = 1; i < args.Count; ++i) {
					list.RemoveAll((ScriptValue value) => {
						return ScriptValue.Compare(value, args[i].GetMetadata());
					});
				}
			});
			ret &= RegisterFunction("list.removeAt", (List<VMValue> args) => {
				if (args.Count < 2) {
					Logger.Error("list.removeAt");
					return;
				}
				if (!args[0].IsList()) {
					Logger.Error("list.removeAt");
					return;
				}
				if (!args[1].IsNumber()) {
					Logger.Error("list.removeAt");
					return;
				}
				var list = args[0].GetList();
				var index = args[1].GetInt();
				if (index >= 0 && index < list.Count) {
					list.RemoveAt(index);
				}
			});
			ret &= RegisterFunction("list.clear", (List<VMValue> args) => {
				foreach (var item in args) {
					if (!item.IsList()) {
						Logger.Error("list.clear");
						return;
					}
					item.GetList().Clear();
				}
			});

			return ret;
		}
	}
}