
namespace gs.compiler {
	public sealed class ScriptObject {
		private string _name = "";
		private ScriptValue _value = new ScriptValue();

		public ScriptObject(string name, ScriptValue value) {
			_name = name;
			_value = value;
		}

		public ScriptValue GetValue() {
			return _value;
		}

		public void SetValue(ScriptValue value) {
			_value = value;
		}
	}
}
