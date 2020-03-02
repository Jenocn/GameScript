/*
 * By Jenocn
 * https://jenocn.github.io/
 */

namespace gs.compiler {
	public static class ScriptIf {

		public static bool Execute(string ifSrc, ScriptMethod space, out bool result) {

			result = false;

			var tempSrc = ifSrc.Trim();

			int fpbPos = tempSrc.IndexOf(Grammar.FPB);
			int fpePos = tool.GrammarTool.ReadPairSignPos(tempSrc, fpbPos + 1, Grammar.FPB, Grammar.FPE);

			if (fpbPos == -1 || fpbPos >= fpePos) {
				Logger.Error(tempSrc);
				return false;
			}
			var nameSrc = tempSrc.Substring(0, fpbPos).Trim();
			if (nameSrc != Grammar.IF) {
				Logger.Error(tempSrc);
				return false;
			}

			var srcCondition = tempSrc.Substring(fpbPos + 1, fpePos - fpbPos - 1).Trim();
			if (string.IsNullOrEmpty(srcCondition)) {
				Logger.Error(tempSrc);
				return false;
			}

			ScriptValue tempValue = null;
			if (!ScriptMethodCall.Execute(srcCondition, space, out tempValue)) {
				if (!ScriptExpression.Execute(srcCondition, space, out tempValue)) {
					Logger.Error(tempSrc);
					return false;
				}
			}
			if (tempValue.GetValueType() != ScriptValueType.Bool) {
				Logger.Error(tempSrc);
				return false;
			}
			result = (bool) tempValue.GetValue();
			return true;
		}
	}
}