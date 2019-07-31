/*
 * By Jenocn
 * https://jenocn.github.io/
*/

using gs.compiler;

namespace gs {
	public sealed class VMValue {
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

		public void SetValue(object value) {
			_scriptValue = ScriptValue.Create(value);
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

		public object GetValue() {
			return _scriptValue.GetValue();
		}
		public double GetNumber() {
			if (_scriptValue.GetValueType() == ScriptValueType.Number) {
				return (double)_scriptValue.GetValue();
			}
			return 0;
		}
		public string GetString() {
			if (_scriptValue.GetValueType() == ScriptValueType.String) {
				return (string)_scriptValue.GetValue();
			}
			return "";
		}
		public bool GetBool() {
			if (_scriptValue.GetValueType() == ScriptValueType.Bool) {
				return (bool)_scriptValue.GetValue();
			}
			return false;
		}

		public ScriptValue GetMetadata() {
			return _scriptValue;
		}
	}
}
