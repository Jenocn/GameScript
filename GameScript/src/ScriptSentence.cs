/*
namespace gs.compiler {
	public static class ScriptSentence {
		public static void Execute(string src) {

			var temp = src.Trim();
			if (string.IsNullOrEmpty(temp)) { return; }
			int fcbPos = temp.IndexOf(Grammar.FCB);
			if (fcbPos != -1) {
				int fcePos = temp.LastIndexOf(Grammar.FCE);
				if (fcePos == -1 || fcbPos >= fcePos) {
					Logger.Error(src);
					return;
				}
				// function head
				string head = temp.Substring(0, fcbPos);
				// function body
				string body = temp.Substring(fcbPos + 1, fcePos - fcbPos - 1);

				return;
			}

			if (temp[temp.Length - 1] != Grammar.OVER) {
				Logger.Error(src);
				return;
			}
			temp = temp.Substring(0, temp.Length - 1);

			int signPos = temp.IndexOf('=');
			if (signPos != -1) {
				if (signPos + 1 >= temp.Length - 1) {
					Logger.Error(src);
					return;
				}
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

		private static void _ExecuteVariable(string left, string right) {
		}

		private static void _ExecuteCall(string src) {
			ScriptCall.Execute(src);
		}
	}
}
*/