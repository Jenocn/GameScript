
namespace gs.compiler {
	public class ScriptObject {
		private string _name = "";
		private ScriptValue _value = null;

		public ScriptObject(string name, ScriptValue value) {
			_name = name;
			_value = value;
		}
	}
}
