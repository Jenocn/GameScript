namespace gs {
	public static class ScriptSentence {
		public static void Execute(string src) {
			var temp = src.Trim();
			if (string.IsNullOrEmpty(temp)) { return; }
			int fcbPos = temp.IndexOf(ScriptGrammar.FCB);
			if (fcbPos != -1) {
				int fcePos = temp.LastIndexOf(ScriptGrammar.FCE);
				if (fcePos == -1) { return; }
				if (fcbPos >= fcePos) { return; }
				// function head
				string head = temp.Substring(0, fcbPos);
				// function body
				string body = temp.Substring(fcbPos + 1, fcePos - fcbPos - 1);
				ScriptFunction.Execute(head, body);
				return;
			}

			if (temp[temp.Length - 1] != ScriptGrammar.OVER) { return; }
			temp = temp.Substring(0, temp.Length - 1);

			int overPos = temp.LastIndexOf(ScriptGrammar.OVER);
			if (overPos == -1) { return; }
			int signPos = temp.IndexOf('=');
			if (sign != -1) {
				if (signPos + 1 >= temp.Length - 1) { return; }
				// variable left value
				string left = temp.Substring(0, signPos);
				// variable right value
				string right = temp.Substring(signPos + 1, temp.Length - signPos - 1);
				_ExecuteVariable(left, right);		
				return;
			}

			// call
			_ExecuteCall(temp);
		}

		private void _ExecuteVariable(string left, string right) {
			ScriptVariable.Execute(left, right);
		}

		private void _ExecuteCall(string src) {
			ScriptCall.Execute(src);
		}
	}
}