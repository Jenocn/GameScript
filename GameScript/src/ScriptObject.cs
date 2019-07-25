
namespace gs.compiler {
	public sealed class ScriptObject {
		private string _src = "";
		private string _name = "";
		private ScriptValue _value = new ScriptValue();

		public ScriptObject(string name, ScriptValue value) {
			_name = name;
			_value = value;
		}
		public ScriptObject(string src) {
			_src = src;
			_Parse();
		}

		private void _Parse() {
			string tempSrc = _src.Trim();
			if (string.IsNullOrEmpty(tempSrc)) {
				Logger.Error(_src);
				return;
			}
			var varPos = tempSrc.IndexOf(Grammar.VAR);
			if (varPos != 0) {
				Logger.Error(_src);
				return;
			}
			var varEndPos = varPos + Grammar.VAR.Length;
			if (varEndPos >= tempSrc.Length) {
				Logger.Error(_src);
				return;
			}
			tempSrc = tempSrc.Substring(varEndPos, tempSrc.Length - varEndPos);
			var assignPos = tempSrc.IndexOf(Grammar.ASSIGN);
			if (assignPos == -1) {
				var tempName = tempSrc.Trim();
				if (tempName.IndexOfAny(Grammar.SPECIAL_CHAR) != -1) {
					Logger.Error(_src);
					return;
				}
				_name = tempName;
			} else {
				if (assignPos == tempSrc.Length - 1 || assignPos == 0) {
					Logger.Error(_src);
					return;
				}
				var tempNameStr = tempSrc.Substring(0, assignPos).Trim();
				var tempValueStr = tempSrc.Substring(assignPos + 1, tempSrc.Length - 1 - assignPos).Trim();
				if (tempNameStr.IndexOfAny(Grammar.SPECIAL_CHAR) != -1) {
					Logger.Error(_src);
					return;
				}
				if (tempValueStr.IndexOfAny(Grammar.SPECIAL_CHAR) != -1) {
					Logger.Error(_src);
					return;
				}
				_name = tempNameStr;
				_value.SetValue(tempValueStr);
			}
		}
	}
}
