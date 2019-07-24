namespace gs {
	public static class ScriptVariable {
		public static void Execute(string left, string right) {
			var tempLeft = left.Trim();
			var tempRight = right.Trim();
			var varPos = tempLeft.IndexOf(ScriptGrammar.VAR);
			if (varPos == -1) { return; }
			if (string.IsNullOrEmpty(tempRight)) { return; }

			var varEndPos = varPos + ScriptGrammar.VAR.Length;
			if (varEndPos >= tempLeft.Length - 1) { return; }
			string key = tempLeft.Substring(varEndPos, tempLeft.Length - varEndPos).Trim();
			string value = ScriptExpression.Execute(tempRight);

			//Debug.Log(key + ":" + value);
		}
	}
}