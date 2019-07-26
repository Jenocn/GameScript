using System.Collections.Generic;

namespace gs.compiler {
	public class ScriptIf {

		private bool _condition = false;

		public ScriptIf(string src, ScriptMethod space) {
			_Parse(src, space);
		}

		public bool Condition() {
			return _condition;
		}

		private void _Parse(string src, ScriptMethod space) {
			int fpbPos = src.IndexOf(Grammar.FPB);
			int fpePos = src.LastIndexOf(Grammar.FPE);
			if (fpbPos == -1 || fpbPos >= fpePos) {
				Logger.Error(src);
				return;
			}
			var nameSrc = src.Substring(0, fpbPos).Trim();
			if (nameSrc != Grammar.IF) {
				Logger.Error(src);
				return;
			}

			var srcCondition = src.Substring(fpbPos + 1, fpePos - fpbPos - 1).Trim();
			if (string.IsNullOrEmpty(srcCondition)) {
				Logger.Error(src);
				return;
			}

			var comparePos = srcCondition.IndexOf(Grammar.COMPARE_EQUIP);
			if (comparePos != -1) {
				var left = srcCondition.Substring(0, comparePos).Trim();
				var right = srcCondition.Substring(comparePos + Grammar.COMPARE_EQUIP.Length, srcCondition.Length - comparePos - Grammar.COMPARE_EQUIP.Length).Trim();
				if (string.IsNullOrEmpty(left) || string.IsNullOrEmpty(right)) {
					Logger.Error(src);
					return;
				}

				var leftValue = _CalcValue(left, space);
				var rightValue = _CalcValue(right, space);
				_condition = ScriptValue.Compare(leftValue, rightValue);
				return;
			}

			comparePos = srcCondition.IndexOf(Grammar.COMPARE_LESS_EQUAL);
			if (comparePos != -1) {
				var left = srcCondition.Substring(0, comparePos).Trim();
				var right = srcCondition.Substring(comparePos + Grammar.COMPARE_EQUIP.Length, srcCondition.Length - comparePos - Grammar.COMPARE_EQUIP.Length).Trim();
				if (string.IsNullOrEmpty(left) || string.IsNullOrEmpty(right)) {
					Logger.Error(src);
					return;
				}

				var leftValue = _CalcValue(left, space);
				var rightValue = _CalcValue(right, space);
				_condition = ScriptValue.LessEqual(leftValue, rightValue);
				return;
			}

			comparePos = srcCondition.IndexOf(Grammar.COMPARE_MORE_EQUAL);
			if (comparePos != -1) {
				var left = srcCondition.Substring(0, comparePos).Trim();
				var right = srcCondition.Substring(comparePos + Grammar.COMPARE_EQUIP.Length, srcCondition.Length - comparePos - Grammar.COMPARE_EQUIP.Length).Trim();
				if (string.IsNullOrEmpty(left) || string.IsNullOrEmpty(right)) {
					Logger.Error(src);
					return;
				}

				var leftValue = _CalcValue(left, space);
				var rightValue = _CalcValue(right, space);
				_condition = ScriptValue.MoreEqual(leftValue, rightValue);
				return;
			}

			comparePos = srcCondition.IndexOf(Grammar.COMPARE_LESS);
			if (comparePos != -1) {
				var left = srcCondition.Substring(0, comparePos).Trim();
				var right = srcCondition.Substring(comparePos + Grammar.COMPARE_EQUIP.Length, srcCondition.Length - comparePos - Grammar.COMPARE_EQUIP.Length).Trim();
				if (string.IsNullOrEmpty(left) || string.IsNullOrEmpty(right)) {
					Logger.Error(src);
					return;
				}

				var leftValue = _CalcValue(left, space);
				var rightValue = _CalcValue(right, space);
				_condition = ScriptValue.Less(leftValue, rightValue);
				return;
			}

			comparePos = srcCondition.IndexOf(Grammar.COMPARE_MORE);
			if (comparePos != -1) {
				var left = srcCondition.Substring(0, comparePos).Trim();
				var right = srcCondition.Substring(comparePos + Grammar.COMPARE_EQUIP.Length, srcCondition.Length - comparePos - Grammar.COMPARE_EQUIP.Length).Trim();
				if (string.IsNullOrEmpty(left) || string.IsNullOrEmpty(right)) {
					Logger.Error(src);
					return;
				}

				var leftValue = _CalcValue(left, space);
				var rightValue = _CalcValue(right, space);
				_condition = ScriptValue.More(leftValue, rightValue);
				return;
			}


			// temp
			// todo...

			// expression
			// todo....
		}

		private ScriptValue _CalcValue(string src, ScriptMethod space) {

			var fpbPos = src.IndexOf(Grammar.FPB);
			if (fpbPos != -1) {
				var fpePos = src.IndexOf(Grammar.FPE);
				if (fpbPos >= fpePos) {
					Logger.Error(src);
					return null;
				}
				// method
				var args = new List<ScriptValue>();
				var srcArgs = src.Substring(fpbPos + 1, fpePos - fpbPos - 1).Trim();
				if (!string.IsNullOrEmpty(srcArgs)) {
					var argsStrArr = srcArgs.Split(Grammar.FPS);
					for (int i = 0; i < argsStrArr.Length; ++i) {
						var argName = argsStrArr[i].Trim();
						if (argName.IndexOfAny(Grammar.SPECIAL_CHAR) != -1) {
							Logger.Error(src);
							return null;
						}
						var findObj = space.FindObject(argName);
						if (findObj != null) {
							args.Add(findObj.GetValue());
						} else {
							args.Add(new ScriptValue(argName));
						}
					}
				}
				var methodName = src.Substring(0, fpbPos).Trim();
				var method = space.FindMethod(methodName);
				if (method == null) {
					if (MethodLibrary.Container(methodName)) {
						return MethodLibrary.Execute(methodName, args);
					}
					Logger.Error(src);
					return null;
				}
				return method.Execute(args);
			}

			var obj = space.FindObject(src);
			if (obj != null) {
				return obj.GetValue();
			}
			return new ScriptValue(src);
		}
	}
}
