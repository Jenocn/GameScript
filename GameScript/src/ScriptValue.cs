
namespace gs.compiler {
	public enum ScriptValueType {
		Null = 0,
		Number,
		String,
		Bool,
	}

	public sealed class ScriptValue {
		private string _src = "";
		private ScriptValueType _type = ScriptValueType.Null;

		private object _value = null;

		public ScriptValue() { }
		public ScriptValue(string src) {
			SetValue(src);
		}

		public void SetValue(string src) {
			_src = src;
			_Parse();
		}

		private void _Parse() {
			var tempSrc = _src.Trim();
			if (string.IsNullOrEmpty(tempSrc)) {
				Logger.Error(tempSrc);
				return;
			}
			if (tempSrc == "true" || tempSrc == "false") {
				_type = ScriptValueType.Bool;
				_value = bool.Parse(tempSrc);
				return;
			}
			if (tempSrc.Length > 2 && tempSrc[0] == Grammar.SS && tempSrc[tempSrc.Length - 1] == Grammar.SS) {
				_type = ScriptValueType.String;
				_value = tempSrc.Substring(1, tempSrc.Length - 2);
				return;
			}
			double tempDouble = 0;
			if (double.TryParse(tempSrc, out tempDouble)) {
				_type = ScriptValueType.Number;
				_value = tempDouble;
				return;
			}
			Logger.Error(tempSrc);
		}

		public ScriptValueType GetValueType() {
			return _type;
		}

		public object GetValue() {
			return _value;
		}
	}
}
