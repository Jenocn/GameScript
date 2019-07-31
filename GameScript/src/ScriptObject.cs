/*
 * By Jenocn
 * https://jenocn.github.io/
*/

namespace gs.compiler {
	public class ScriptObject {
		private string _name = "";
		private ScriptValue _value = ScriptValue.NULL;

		public ScriptObject(string name, ScriptValue value) {
			_name = name;
			SetValue(value);
		}

		public string GetName() {
			return _name;
		}

		public ScriptValue GetValue() {
			return _value;
		}

		public void SetValue(ScriptValue value) {
			if (value == null) {
				_value = ScriptValue.NULL;
			} else {
				_value = value;
			}
		}
	}
}
