namespace gs.compiler {
	public static class ScriptVariable {
		public static void Execute(string left, string right) {
			var tempLeft = left.Trim();
			var tempRight = right.Trim();
			var varPos = tempLeft.IndexOf(ScriptGrammar.VAR);
			if (varPos == -1) {
				Logger.Error(tempLeft);
				return;
			}
			if (string.IsNullOrEmpty(tempRight)) {
				Logger.Error(tempRight);
				return;
			}

			var varEndPos = varPos + ScriptGrammar.VAR.Length;
			if (varEndPos >= tempLeft.Length - 1) {
				Logger.Error(tempRight);
				return;
			}
			string key = tempLeft.Substring(varEndPos, tempLeft.Length - varEndPos).Trim();
			string value = ScriptExpression.Execute(tempRight);

		}
	}
}