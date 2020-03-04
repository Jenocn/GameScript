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

		var tempSrc = _src + line + '\n';
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