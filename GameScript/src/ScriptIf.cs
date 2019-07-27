using System.Collections.Generic;

namespace gs.compiler {
	public static class ScriptIf {

		public static bool Execute(string src, ScriptMethod space, out bool result) {

			result = false;

			int fpbPos = src.IndexOf(Grammar.FPB);
			int fpePos = src.LastIndexOf(Grammar.FPE);
			if (fpbPos == -1 || fpbPos >= fpePos) {
				Logger.Error(src);
				return false;
			}
			var nameSrc = src.Substring(0, fpbPos).Trim();
			if (nameSrc != Grammar.IF) {
				Logger.Error(src);
				return false;
			}

			var srcCondition = src.Substring(fpbPos + 1, fpePos - fpbPos - 1).Trim();
			if (string.IsNullOrEmpty(srcCondition)) {
				Logger.Error(src);
				return false;
			}

			ScriptValue tempValue = new ScriptValue();
			if (!ScriptExpression.Execute(srcCondition, space, out tempValue)) {
				Logger.Error(src);
				return false;
			}
			if (tempValue.GetValueType() != ScriptValueType.Bool) {
				Logger.Error(src);
				return false;
			}
			result = (bool)tempValue.GetValue();
			return true;
		}
	}
}
