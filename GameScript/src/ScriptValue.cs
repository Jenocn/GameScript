/*
 * By Jenocn
 * https://jenocn.github.io/
 */

using System.Collections.Generic;

namespace gs.compiler {
	public enum ScriptValueType {
		Null = 0,
		Number,
		String,
		Bool,
		List,
	}

	public sealed class ScriptValue {
		private ScriptValueType _type = ScriptValueType.Null;
		private object _value = null;

		public static ScriptValue NULL { get { return new ScriptValue(); } }

		public static ScriptValue Create(object src) {
			ScriptValue ret = NULL;
			if (src == null) {
				return ret;
			}
			TryParse(src.ToString(), null, out ret);
			return ret;
		}

		public static bool TryParse(string src, ScriptMethod space, out ScriptValue ret) {
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
			if (checkCh == Grammar.ARRB) {
				if (tempSrc.Length >= 2 && tempSrc[tempSrc.Length - 1] == Grammar.ARRE) {
					ret = new ScriptValue();
					ret._type = ScriptValueType.List;
					var tempSplit = tempSrc.Substring(1, tempSrc.Length - 2).Split(',');
					var tempList = new List<ScriptValue>();
					foreach (var item in tempSplit) {
						var tempStr = item.Trim();
						if (string.IsNullOrEmpty(tempStr)) {
							return false;
						}
						ScriptValue tempValue = null;
						if (space != null) {
							var findObj = space.FindObject(tempStr);
							if (findObj != null) {
								tempValue = findObj.GetValue();
								tempList.Add(tempValue);
								continue;
							}
						}

						if (!TryParse(tempStr, null, out tempValue)) {
							return false;
						}
						tempList.Add(tempValue);
					}
					ret._value = tempList;
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
				return (string) _value;
			case ScriptValueType.Bool:
				return _value.ToString().ToLower();
			case ScriptValueType.List:
				var ret = "[";
				var tempList = (List<ScriptValue>) _value;
				for (int i = 0; i < tempList.Count; ++i) {
					var tempValue = tempList[i];
					if (tempValue._type == ScriptValueType.String) {
						ret += "\"" + tempValue.ToString() + "\"";
					} else {
						ret += tempValue.ToString();
					}
					if (i != tempList.Count - 1) {
						ret += ", ";
					}
				}
				return ret + ']';
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
				return System.Math.Abs((double) a.GetValue() - (double) b.GetValue()) < double.Epsilon;
			case ScriptValueType.String:
				return (string) a.GetValue() == (string) b.GetValue();
			case ScriptValueType.Bool:
				return (bool) a.GetValue() == (bool) b.GetValue();
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
				return (double) a.GetValue() < (double) b.GetValue();
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
				return (double) a.GetValue() > (double) b.GetValue();
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
				return (double) a.GetValue() <= (double) b.GetValue();
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
				return (double) a.GetValue() >= (double) b.GetValue();
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