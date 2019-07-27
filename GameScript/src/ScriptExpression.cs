
namespace gs.compiler {
	public static class ScriptExpression {

		public static bool Execute(string src, ScriptMethod space, out ScriptValue result) {
			result = new ScriptValue();

			var tempSrc = src.Trim();

			// check validity
			var fpbPos = tempSrc.IndexOf(Grammar.FPB);
			var fpePos = tempSrc.LastIndexOf(Grammar.FPE);
			if (fpbPos == -1 && fpePos != -1) {
				return false;
			}
			if (fpbPos != -1 && fpbPos >= fpePos) {
				return false;
			}

			// object
			var findObj = space.FindObject(tempSrc);
			if (findObj != null) {
				result = findObj.GetValue();
				return true;
			}

			// logic expression
			do {
				var srcCondition = tempSrc;
				var comparePos = srcCondition.IndexOf(Grammar.COMPARE_EQUIP);
				if (comparePos != -1) {
					var left = srcCondition.Substring(0, comparePos).Trim();
					var right = srcCondition.Substring(comparePos + Grammar.COMPARE_EQUIP.Length, srcCondition.Length - comparePos - Grammar.COMPARE_EQUIP.Length).Trim();
					if (string.IsNullOrEmpty(left) || string.IsNullOrEmpty(right)) {
						return false;
					}

					ScriptValue leftValue = null;
					ScriptValue rightValue = null;
					if (!Execute(left, space, out leftValue)) {
						return false;
					}
					if (!Execute(right, space, out rightValue)) {
						return false;
					}
					if (leftValue.GetValueType() != rightValue.GetValueType()) {
						return false;
					}
					bool bCondition = ScriptValue.Compare(leftValue, rightValue);
					result = new ScriptValue(bCondition);
					return true;
				}


				comparePos = srcCondition.IndexOf(Grammar.COMPARE_LESS_EQUAL);
				if (comparePos != -1) {
					var left = srcCondition.Substring(0, comparePos).Trim();
					var right = srcCondition.Substring(comparePos + Grammar.COMPARE_EQUIP.Length, srcCondition.Length - comparePos - Grammar.COMPARE_EQUIP.Length).Trim();
					if (string.IsNullOrEmpty(left) || string.IsNullOrEmpty(right)) {
						return false;
					}

					ScriptValue leftValue = null;
					ScriptValue rightValue = null;
					if (!Execute(left, space, out leftValue)) {
						return false;
					}
					if (!Execute(right, space, out rightValue)) {
						return false;
					}
					if (leftValue.GetValueType() != rightValue.GetValueType()) {
						return false;
					}
					bool bCondition = ScriptValue.LessEqual(leftValue, rightValue);
					result = new ScriptValue(bCondition);
					return true;
				}

				comparePos = srcCondition.IndexOf(Grammar.COMPARE_MORE_EQUAL);
				if (comparePos != -1) {
					var left = srcCondition.Substring(0, comparePos).Trim();
					var right = srcCondition.Substring(comparePos + Grammar.COMPARE_EQUIP.Length, srcCondition.Length - comparePos - Grammar.COMPARE_EQUIP.Length).Trim();
					if (string.IsNullOrEmpty(left) || string.IsNullOrEmpty(right)) {
						return false;
					}

					ScriptValue leftValue = null;
					ScriptValue rightValue = null;
					if (!Execute(left, space, out leftValue)) {
						return false;
					}
					if (!Execute(right, space, out rightValue)) {
						return false;
					}
					if (leftValue.GetValueType() != rightValue.GetValueType()) {
						return false;
					}
					bool bCondition = ScriptValue.MoreEqual(leftValue, rightValue);
					result = new ScriptValue(bCondition);
					return true;
				}

				comparePos = srcCondition.IndexOf(Grammar.COMPARE_LESS);
				if (comparePos != -1) {
					var left = srcCondition.Substring(0, comparePos).Trim();
					var right = srcCondition.Substring(comparePos + Grammar.COMPARE_EQUIP.Length, srcCondition.Length - comparePos - Grammar.COMPARE_EQUIP.Length).Trim();
					if (string.IsNullOrEmpty(left) || string.IsNullOrEmpty(right)) {
						return false;
					}

					ScriptValue leftValue = null;
					ScriptValue rightValue = null;
					if (!Execute(left, space, out leftValue)) {
						return false;
					}
					if (!Execute(right, space, out rightValue)) {
						return false;
					}
					if (leftValue.GetValueType() != rightValue.GetValueType()) {
						return false;
					}
					bool bCondition = ScriptValue.Less(leftValue, rightValue);
					result = new ScriptValue(bCondition);
					return true;
				}

				comparePos = srcCondition.IndexOf(Grammar.COMPARE_MORE);
				if (comparePos != -1) {
					var left = srcCondition.Substring(0, comparePos).Trim();
					var right = srcCondition.Substring(comparePos + Grammar.COMPARE_EQUIP.Length, srcCondition.Length - comparePos - Grammar.COMPARE_EQUIP.Length).Trim();
					if (string.IsNullOrEmpty(left) || string.IsNullOrEmpty(right)) {
						return false;
					}

					ScriptValue leftValue = null;
					ScriptValue rightValue = null;
					if (!Execute(left, space, out leftValue)) {
						return false;
					}
					if (!Execute(right, space, out rightValue)) {
						return false;
					}
					if (leftValue.GetValueType() != rightValue.GetValueType()) {
						return false;
					}
					bool bCondition = ScriptValue.More(leftValue, rightValue);
					result = new ScriptValue(bCondition);
					return true;
				}
			} while (false);

			// math expression
			do {


			} while (false);

			// value
			result = new ScriptValue(tempSrc);
			return true;
		}
	}
}
