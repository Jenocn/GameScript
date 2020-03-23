/*
 * By Jenocn
 * https://jenocn.github.io/
 */

using gs.compiler;

namespace gs {
	public sealed class VMValue {

		public static VMValue TRUE { get { return new VMValue(true); } }
		public static VMValue FALSE { get { return new VMValue(false); } }
		public static VMValue NULL { get { return new VMValue(); } }

		private ScriptValue _scriptValue = null;

		public VMValue() {
			_scriptValue = ScriptValue.NULL;
		}
		public VMValue(object value) {
			_scriptValue = ScriptValue.NULL;
			SetValue(value);
		}
		public VMValue(ScriptValue value) {
			_scriptValue = value;
		}

		public VMValue(System.Collections.Generic.List<VMValue> arg) {
			_scriptValue = ScriptValue.NULL;
			SetValue(arg);
		}

		public void SetValue(object value) {
			if (value is string) {
				value = "\"" + value + "\"";
			}

			_scriptValue = ScriptValue.Create(value);
		}

		public void SetValue(System.Collections.Generic.List<VMValue> args) {
			if (args == null) {
				args = new System.Collections.Generic.List<VMValue>();
			}
			var tempList = new System.Collections.Generic.List<ScriptValue>();
			foreach (var item in args) {
				tempList.Add(item.GetMetadata());
			}
			_scriptValue = ScriptValue.CreateList(tempList);
		}

		public override string ToString() {
			return _scriptValue.ToString();
		}

		public bool IsNumber() {
			return _scriptValue.GetValueType() == ScriptValueType.Number;
		}
		public bool IsString() {
			return _scriptValue.GetValueType() == ScriptValueType.String;
		}
		public bool IsBool() {
			return _scriptValue.GetValueType() == ScriptValueType.Bool;
		}
		public bool IsNull() {
			return _scriptValue.GetValueType() == ScriptValueType.Null;
		}
		public bool IsList() {
			return _scriptValue.GetValueType() == ScriptValueType.List;
		}

		public object GetValue() {
			return _scriptValue.GetValue();
		}
		public double GetNumber() {
			if (IsNumber()) {
				return (double) _scriptValue.GetValue();
			}
			return 0;
		}
		public int GetInt() {
			return (int) GetNumber();
		}
		public float GetFloat() {
			return (float) GetNumber();
		}
		public string GetString() {
			if (IsString()) {
				return (string) _scriptValue.GetValue();
			}
			return "";
		}
		public bool GetBool() {
			if (IsBool()) {
				return (bool) _scriptValue.GetValue();
			}
			return false;
		}

		public System.Collections.Generic.List<ScriptValue> GetList() {
			if (IsList()) {
				return (System.Collections.Generic.List<ScriptValue>) _scriptValue.GetValue();
			}
			return new System.Collections.Generic.List<ScriptValue>();
		}

		public ScriptValue GetMetadata() {
			return _scriptValue;
		}
	}
}