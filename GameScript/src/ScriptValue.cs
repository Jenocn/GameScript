/*
 * By Jenocn
 * https://jenocn.github.io/
*/

namespace gs.compiler {
	public enum ScriptValueType {
		Null = 0,
		Number,
		String,
		Bool,
	}

	public sealed class ScriptValue {
		private ScriptValueType _type = ScriptValueType.Null;
		private object _value = null;

		public ScriptValue() { }
		public ScriptValue(object src) {
			SetValue(src);
		}

		public static bool Compare(ScriptValue a, ScriptValue b) {
			if (a == null && b == null) {
				return true;
			}
			if (a == null || b == null) {
				return false;
			}
			if (a.GetValueType() != b.GetValueType()) {
				return false;
			}
			var type = a.GetValueType();
			switch (type) {
			case ScriptValueType.Null:
				return true;
			case ScriptValueType.Number:
				return (double)a.GetValue() == (double)b.GetValue();
			case ScriptValueType.String:
				return (string)a.GetValue() == (string)b.GetValue();
			case ScriptValueType.Bool:
				return (bool)a.GetValue() == (bool)b.GetValue();
			}
			return false;
		}

		public static bool Less(ScriptValue a, ScriptValue b) {
			if (a == null || b == null) {
				return false;
			}
			if (a.GetValueType() != b.GetValueType()) {
				return false;
			}
			var type = a.GetValueType();
			switch (type) {
			case ScriptValueType.Number:
				return (double)a.GetValue() < (double)b.GetValue();
			}
			return false;
		}

		public static bool More(ScriptValue a, ScriptValue b) {
			if (a == null || b == null) {
				return false;
			}
			if (a.GetValueType() != b.GetValueType()) {
				return false;
			}
			var type = a.GetValueType();
			switch (type) {
			case ScriptValueType.Number:
				return (double)a.GetValue() > (double)b.GetValue();
			}
			return false;
		}

		public static bool LessEqual(ScriptValue a, ScriptValue b) {
			if (a == null || b == null) {
				return false;
			}
			if (a.GetValueType() != b.GetValueType()) {
				return false;
			}
			var type = a.GetValueType();
			switch (type) {
			case ScriptValueType.Number:
				return (double)a.GetValue() <= (double)b.GetValue();
			}
			return false;
		}

		public static bool MoreEqual(ScriptValue a, ScriptValue b) {
			if (a == null || b == null) {
				return false;
			}
			if (a.GetValueType() != b.GetValueType()) {
				return false;
			}
			var type = a.GetValueType();
			switch (type) {
			case ScriptValueType.Number:
				return (double)a.GetValue() >= (double)b.GetValue();
			}
			return false;
		}

		public void SetValue(object src) {
			if (src == null) {
				_Parse("null");
			} else {
				_Parse(src.ToString().ToLower());
			}
		}

		private void _Parse(string srcStr) {
			var tempSrc = srcStr.Trim();
			if (string.IsNullOrEmpty(tempSrc)) {
				Logger.Error("", "empty value!");
				return;
			}
			if (tempSrc == "null") {
				_type = ScriptValueType.Null;
				return;
			}
			if (tempSrc == "true" || tempSrc == "false") {
				_type = ScriptValueType.Bool;
				_value = bool.Parse(tempSrc);
				return;
			}
			if (tempSrc.Length >= 2 && tempSrc[0] == Grammar.SS && tempSrc[tempSrc.Length - 1] == Grammar.SS) {
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

			_type = ScriptValueType.Null;
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
