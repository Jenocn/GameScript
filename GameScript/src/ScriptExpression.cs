﻿using System.Collections.Generic;

namespace gs.compiler {
	public static class ScriptExpression {

		public static bool Execute(string src, ScriptMethod space, out ScriptValue result) {
			result = new ScriptValue();

			var tempSrc = src.Trim();

			// check validity
			var fpbPos = tempSrc.IndexOf(Grammar.FPB);
			var fpePos = tool.GrammarTool.ReadPairSignPos(tempSrc, fpbPos + 1, Grammar.FPB, Grammar.FPE);
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
				var calcSrc = tempSrc;
				var mathSign = new char[] {'+', '-', '*', '/', '(', ')', '%' };
				var highSign = new char[] { '*', '/', '%' };
				var lowSign = new char[] { '+', '-' };
				if (calcSrc.IndexOfAny(mathSign) == -1) {
					break;
				}
				while (true) {
					var leftBracket = calcSrc.IndexOf('(');
					if (leftBracket == -1) { break; }
					var rightBracket = tool.GrammarTool.ReadPairSignPos(calcSrc, leftBracket + 1, '(', ')');
					if (rightBracket == -1) {
						return false;
					}
					var srcBracket = calcSrc.Substring(leftBracket + 1, rightBracket - leftBracket - 1);
					ScriptValue bracketValue = null;
					if (!Execute(srcBracket, space, out bracketValue)) {
						return false;
					}
					if (bracketValue.GetValueType() != ScriptValueType.Number) {
						return false;
					}
					calcSrc = calcSrc.Insert(rightBracket + 1, bracketValue.GetValue().ToString());
					calcSrc = calcSrc.Remove(leftBracket, rightBracket - leftBracket + 1);
				}
				while (true) {
					var highSignPos = calcSrc.IndexOfAny(highSign);
					if (highSignPos == -1) { break; }
					char tempSign = calcSrc[highSignPos];
					var findOL = calcSrc.IndexOfAny(mathSign, 0, highSignPos);
					int startPosOL = 0;
					int endPosOR = 0;
					string srcOL = "";
					if (findOL == -1) {
						srcOL = calcSrc.Substring(0, highSignPos);
						startPosOL = 0;
					} else {
						srcOL = calcSrc.Substring(findOL + 1, highSignPos - findOL - 1);
						startPosOL = findOL + 1;
					}
					ScriptValue valueOL = null;
					if (!Execute(srcOL, space, out valueOL)) {
						return false;
					}
					if (valueOL.GetValueType() != ScriptValueType.Number) {
						return false;
					}

					var findOR = calcSrc.IndexOfAny(mathSign, highSignPos + 1);
					string srcOR = "";
					if (findOR == -1) {
						srcOR = calcSrc.Substring(highSignPos + 1);
						endPosOR = calcSrc.Length;
					} else {
						srcOR = calcSrc.Substring(highSignPos + 1, findOR - highSignPos - 1);
						endPosOR = findOR;
					}
					ScriptValue valueOR = null;
					if (!Execute(srcOR, space, out valueOR)) {
						return false;
					}
					if (valueOR.GetValueType() != ScriptValueType.Number) {
						return false;
					}
					var valueResult = _Calc((double)valueOL.GetValue(), (double)valueOR.GetValue(), tempSign);
					calcSrc = calcSrc.Insert(endPosOR, valueResult.ToString());
					calcSrc = calcSrc.Remove(startPosOL, endPosOR - startPosOL);
				}
				while (true) {
					var lowSignPos = calcSrc.IndexOfAny(lowSign);
					if (lowSignPos == -1) { break; }
					char tempSign = calcSrc[lowSignPos];
					var findOL = calcSrc.IndexOfAny(mathSign, 0, lowSignPos);
					int startPosOL = 0;
					int endPosOR = 0;
					string srcOL = "";
					if (findOL == -1) {
						srcOL = calcSrc.Substring(0, lowSignPos);
						startPosOL = 0;
					} else {
						srcOL = calcSrc.Substring(findOL + 1, lowSignPos - findOL - 1);
						startPosOL = findOL + 1;
					}
					ScriptValue valueOL = null;
					if (!Execute(srcOL, space, out valueOL)) {
						return false;
					}
					if (valueOL.GetValueType() != ScriptValueType.Number) {
						return false;
					}

					var findOR = calcSrc.IndexOfAny(mathSign, lowSignPos + 1);
					string srcOR = "";
					if (findOR == -1) {
						srcOR = calcSrc.Substring(lowSignPos + 1);
						endPosOR = calcSrc.Length;
					} else {
						srcOR = calcSrc.Substring(lowSignPos + 1, findOR - lowSignPos - 1);
						endPosOR = findOR;
					}
					ScriptValue valueOR = null;
					if (!Execute(srcOR, space, out valueOR)) {
						return false;
					}
					if (valueOR.GetValueType() != ScriptValueType.Number) {
						return false;
					}
					var valueResult = _Calc((double)valueOL.GetValue(), (double)valueOR.GetValue(), tempSign);
					calcSrc = calcSrc.Insert(endPosOR, valueResult.ToString());
					calcSrc = calcSrc.Remove(startPosOL, endPosOR - startPosOL);
				}
				tempSrc = calcSrc;
			} while (false);

			// value
			result = new ScriptValue(tempSrc);
			return true;
		}

		private static double _Calc(double value1, double value2, char sign) {
			if(sign == '+') {
				return value1 + value2;
			}
			if (sign == '-') {
				return value1 - value2;
			}
			if (sign == '*') {
				return value1 * value2;
			}
			if (sign == '/') {
				return value1 / value2;
			}
			if (sign == '%') {
				return value1 % value2;
			}
			return 0;
		}
	}
}