using System.Collections.Generic;

namespace gs.compiler {
	public static class ScriptMethodCall {
		public static bool Execute(string src, ScriptMethod space, out ScriptValue result) {
			result = new ScriptValue();

			var tempSrc = src.Trim();

			// check validity
			var fpbPos = tempSrc.IndexOf(Grammar.FPB);
			var fpePos = tempSrc.LastIndexOf(Grammar.FPE);
			if (fpbPos == -1 && fpePos == -1) {
				return false;
			}
			if (fpbPos != -1 && fpbPos >= fpePos) {
				return false;
			}
			if (fpePos != tempSrc.Length - 1) {
				return false;
			}

			var methodName = tempSrc.Substring(0, fpbPos).Trim();
			var scriptParams = new List<ScriptValue>();
			var srcArgs = tempSrc.Substring(fpbPos + 1, fpePos - fpbPos - 1).Trim();
			if (!string.IsNullOrEmpty(srcArgs)) {
				var argArr = srcArgs.Split(Grammar.FPS);
				for (int i = 0; i < argArr.Length; ++i) {
					var argStr = argArr[i].Trim();
					if (string.IsNullOrEmpty(argStr)) {
						Logger.Error(src);
						return false;
					}
					if (!ScriptExpression.Execute(argStr, space, out result)) {
						Logger.Error(src);
						return false;
					}
					scriptParams.Add(result);
				}
			}
			var findMethod = space.FindMethod(methodName);
			if (findMethod == null) {
				if (MethodLibrary.Container(methodName)) {
					result = MethodLibrary.Execute(methodName, scriptParams);
					return true;
				} else {
					Logger.Error(src);
					return false;
				}
			}
			bool bReturn = false;
			return findMethod.Execute(scriptParams, out bReturn, out result);
		}
	}
}
