using gs.compiler;

public class LineExecuter {
	private ScriptMethod method = new ScriptMethod("");
	private string _src = "";

	public string GetSrc() {
		return _src;
	}

	public string ExecuteLineOrSaveLine(string line) {
		ScriptValue result = ScriptValue.NULL;
		if (ScriptExpression.Execute(line, method, out result)) {
			return result.ToString();
		}

		if (!string.IsNullOrEmpty(line)) {
			_src += (line + '\n');
		}
		return "";
	}

	public bool ExecuteSrc() {
		var child = new ScriptMethod(_src, method);
		bool bReturn = false;
		ScriptValue result = ScriptValue.NULL;
		return child.Execute(null, out bReturn, out result);
	}

	public void Clear() {
		_src = "";
		method = new ScriptMethod("");
	}
}