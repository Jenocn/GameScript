using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using gs.compiler;

namespace Test {
	class Program {
		static void Main(string[] args) {

			MethodLibrary.RegisterMethod("sum", (List<ScriptValue> param) => {
				ScriptValue ret = new ScriptValue(0);
				if (param == null) { return ret; }
				double value = 0;
				foreach (var item in param) {
					if (item == null) { continue; }
					if (item.GetValueType() != ScriptValueType.Number) { continue; }
					var sv = (double)item.GetValue();
					value += sv;
				}
				ret.SetValue(value);
				return ret;
			});

			var text = File.ReadAllText(@"D:\JenocnDocument\temp\GameScript\GameScript\test.gs");
			var method = new ScriptMethod(text);
			method.Execute(null);
		}
	}
}
