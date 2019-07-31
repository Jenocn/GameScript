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

		public readonly static ScriptValue NULL = new ScriptValue();

		public static ScriptValue Create(object src) {
			ScriptValue ret = NULL;
			if (src == null) {
				return ret;
			}
			TryParse(src.ToString(), out ret);
			return ret;
		}

		public static bool TryParse(string src, out ScriptValue ret) {
			ret = NULL;
			var tempSrc = src.Trim();
			if (string.IsNullOrEmpty(tempSrc)) {
				return false;
			}
			char checkCh = tempSrc[0];
			if (checkCh == 'n' && tempSrc == "null") {
				ret = new ScriptValue();
				ret._type = ScriptValueType.Null;
				ret._value = null;
				return true;
			}
			if (checkCh == 't' || checkCh == 'T') {
				if (tempSrc == "true" || tempSrc == "True") {
					ret = new ScriptValue();
					ret._type = ScriptValueType.Bool;
					ret._value = true;
					return true;
				}
			}
			if (checkCh == 'f' || checkCh == 'F') {
				if (tempSrc == "false" || tempSrc == "False") {
					ret = new ScriptValue();
					ret._type = ScriptValueType.Bool;
					ret._value = false;
					return true;
				}
			}
			if (checkCh == Grammar.SS) {
				if (tempSrc.Length >= 2 && tempSrc[tempSrc.Length - 1] == Grammar.SS) {
					ret = new ScriptValue();
					ret._type = ScriptValueType.String;
					ret._value = tempSrc.Substring(1, tempSrc.Length - 2);
					return true;
				}
			}
			double tempDouble = 0;
			if (double.TryParse(tempSrc, out tempDouble)) {
				ret = new ScriptValue();
				ret._type = ScriptValueType.Number;
				ret._value = tempDouble;
				return true;
			}
			return false;
		}

		private ScriptValue() { }

		public override string ToString() {
			switch (_type) {
			case ScriptValueType.Number:
				return _value.ToString();
			case ScriptValueType.String:
				return (string)_value;
			case ScriptValueType.Bool:
				return _value.ToString().ToLower();
			}
			return "null";
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

		public ScriptValueType GetValueType() {
			return _type;
		}

		public object GetValue() {
			return _value;
		}
	}
}
