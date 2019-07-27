/*
 * By Jenocn
 * https://jenocn.github.io/
*/

using System.Collections.Generic;

namespace gs.compiler {
	public static class ScriptMethodCall {
		public static bool Execute(string src, ScriptMethod space, out ScriptValue result) {
			result = new ScriptValue();

			var tempSrc = src.Trim();

			// check validity
			var fpbPos = tempSrc.IndexOf(Grammar.FPB);
			int fpePos = tool.GrammarTool.ReadPairSignPos(tempSrc, fpbPos + 1, Grammar.FPB, Grammar.FPE);
			if (fpbPos == -1 || fpbPos >= fpePos) {
				return false;
			}
			if (fpePos != tempSrc.Length - 1) {
				return false;
			}

			var methodName = tempSrc.Substring(0, fpbPos).Trim();
			var findMethod = space.FindMethod(methodName);
			bool bMethod = (findMethod != null) || MethodLibrary.Container(methodName);
			if (!bMethod) {
				return false;
			}
			var scriptParams = new List<ScriptValue>();
			var srcArgs = tempSrc.Substring(fpbPos + 1, fpePos - fpbPos - 1).Trim();
			if (!string.IsNullOrEmpty(srcArgs)) {
				var argArr = srcArgs.Split(Grammar.FPS);
				for (int i = 0; i < argArr.Length; ++i) {
					var argStr = argArr[i].Trim();
					if (string.IsNullOrEmpty(argStr)) {
						return false;
					}
					if (!ScriptExpression.Execute(argStr, space, out result)) {
						return false;
					}
					scriptParams.Add(result);
				}
			}
			if (findMethod == null) {
				result = MethodLibrary.Execute(methodName, scriptParams);
				return true;
			}
			bool bReturn = false;
			return findMethod.Execute(scriptParams, out bReturn, out result);
		}
	}
}
