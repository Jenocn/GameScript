using gs.compiler;

public class LineExecuter {
	private ScriptMethod method = new ScriptMethod("");
	private string _src = "";

	public string GetSrc() {
		return _src;
	}

	public string Execute(string line) {
		ScriptValue result = ScriptValue.NULL;
		if (ScriptExpression.Execute(line, method, out result)) {
			return result.ToString();
		}

		string tempSrc = _src;
		if (!string.IsNullOrEmpty(line)) {
			tempSrc += (line + '\n');
		}
		var child = new ScriptMethod(tempSrc, method);
		bool bReturn = false;
		if (child.Execute(null, out bReturn, out result)) {
			_src = tempSrc;
		}
		return "";
	}
	public void Clear() {
		_src = "";
		method = new ScriptMethod("");
	}
}