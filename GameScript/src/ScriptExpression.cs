/*
 * By Jenocn
 * https://jenocn.github.io/
 */

using System;

namespace gs.compiler {
	public static class ScriptExpression {

		public static bool Execute(string src, ScriptMethod space, out ScriptValue result) {
			result = ScriptValue.NULL;

			var tempSrc = src.Trim();

			// value
			if (ScriptValue.TryParse(tempSrc, out result)) {
				return true;
			}

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

				int comparePos = -1;
				string compareSign = "";
				foreach (var tempCmpSign in Grammar.COMPARE_SIGNS) {
					comparePos = srcCondition.IndexOf(tempCmpSign);
					if (comparePos != -1) {
						compareSign = tempCmpSign;
						break;
					}
				}

				if (comparePos == -1) { break; }

				var left = srcCondition.Substring(0, comparePos).Trim();
				var right = srcCondition.Substring(comparePos + compareSign.Length, srcCondition.Length - comparePos - compareSign.Length).Trim();
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
				bool bCondition = _Compare(leftValue, rightValue, compareSign);
				result = ScriptValue.Create(bCondition);
				return true;
			} while (false);

			// math expression
			do {
				var calcSrc = tempSrc;
				var mathSign = new char[] { '+', '-', '*', '/', '(', ')', '%', '^' };
				var highSign = new char[] { '*', '/', '%', '^' };
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
					calcSrc = calcSrc.Insert(rightBracket + 1, bracketValue.ToString());
					calcSrc = calcSrc.Remove(leftBracket, rightBracket - leftBracket + 1);
				}
				while (true) {
					var highSignPos = calcSrc.IndexOfAny(highSign);
					if (highSignPos == -1) { break; }
					if (highSignPos == 0) {
						return false;
					}
					char tempSign = calcSrc[highSignPos];
					var findOL = calcSrc.LastIndexOfAny(mathSign, highSignPos - 1);
					int startPosOL = 0;
					int endPosOR = 0;
					string srcOL = "";
					if (findOL == -1) {
						srcOL = calcSrc.Substring(0, highSignPos);
						startPosOL = 0;
					} else {
						if (calcSrc[findOL] == '-') {
							if (findOL == 0) {
								--findOL;
							} else {
								var checkSignPos = calcSrc.IndexOfAny(mathSign, 0, findOL);
								if (checkSignPos != -1) {
									var checkStr = calcSrc.Substring(checkSignPos + 1, findOL).Trim();
									if (string.IsNullOrEmpty(checkStr)) {
										findOL = checkSignPos;
									}
								}
							}
						}
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
						if (string.IsNullOrEmpty(srcOR.Trim())) {
							if (calcSrc[findOR] == '-') {
								var checkSignPos = calcSrc.IndexOfAny(mathSign, findOR + 1);
								if (checkSignPos != -1) {
									findOR = checkSignPos;
								} else {
									findOR = calcSrc.Length;
								}
								srcOR = calcSrc.Substring(highSignPos + 1, findOR - highSignPos - 1);
							}
						}
						endPosOR = findOR;
					}
					ScriptValue valueOR = null;
					if (!Execute(srcOR, space, out valueOR)) {
						return false;
					}
					if (valueOR.GetValueType() != ScriptValueType.Number) {
						return false;
					}
					var valueResult = _Calc((double) valueOL.GetValue(), (double) valueOR.GetValue(), tempSign);
					calcSrc = calcSrc.Insert(endPosOR, valueResult.ToString());
					calcSrc = calcSrc.Remove(startPosOL, endPosOR - startPosOL);
				}
				while (true) {
					var lowSignPos = calcSrc.IndexOfAny(lowSign);
					if (lowSignPos == -1) { break; }
					if (lowSignPos == 0) {
						if (calcSrc[lowSignPos] == '-') {
							lowSignPos = calcSrc.IndexOfAny(lowSign, lowSignPos + 1);
							if (lowSignPos == -1) { break; }
						} else {
							return false;
						}
					}

					char tempSign = calcSrc[lowSignPos];
					var findOL = calcSrc.LastIndexOfAny(mathSign, lowSignPos - 1);
					int startPosOL = 0;
					int endPosOR = 0;
					string srcOL = "";
					if (findOL == -1) {
						srcOL = calcSrc.Substring(0, lowSignPos);
						startPosOL = 0;
					} else {
						if (calcSrc[findOL] == '-') {
							if (findOL == 0) {
								--findOL;
							} else {
								var checkSignPos = calcSrc.IndexOfAny(mathSign, 0, findOL);
								if (checkSignPos != -1) {
									var checkStr = calcSrc.Substring(checkSignPos + 1, findOL).Trim();
									if (string.IsNullOrEmpty(checkStr)) {
										findOL = checkSignPos;
									}
								}
							}
						}
						srcOL = calcSrc.Substring(findOL + 1, lowSignPos - findOL - 1);
						startPosOL = findOL + 1;
					}
					ScriptValue valueOL = null;
					if (!Execute(srcOL, space, out valueOL)) {
						return false;
					}

					var findOR = calcSrc.IndexOfAny(mathSign, lowSignPos + 1);
					string srcOR = "";
					if (findOR == -1) {
						srcOR = calcSrc.Substring(lowSignPos + 1);
						endPosOR = calcSrc.Length;
					} else {
						srcOR = calcSrc.Substring(lowSignPos + 1, findOR - lowSignPos - 1);
						if (string.IsNullOrEmpty(srcOR.Trim())) {
							if (calcSrc[findOR] == '-') {
								var checkSignPos = calcSrc.IndexOfAny(mathSign, findOR + 1);
								if (checkSignPos != -1) {
									findOR = checkSignPos;
								} else {
									findOR = calcSrc.Length;
								}
								srcOR = calcSrc.Substring(lowSignPos + 1, findOR - lowSignPos - 1);
							}
						}
						endPosOR = findOR;
						endPosOR = findOR;
					}
					ScriptValue valueOR = null;
					if (!Execute(srcOR, space, out valueOR)) {
						return false;
					}
					object valueResult = null;
					do {
						if (tempSign == '+') {
							if ((valueOL.GetValueType() == ScriptValueType.String) || (valueOR.GetValueType() == ScriptValueType.String)) {
								valueResult = "\"" + valueOL.ToString() + valueOR.ToString() + "\"";
								break;
							}
						}
						if (valueOL.GetValueType() != ScriptValueType.Number) {
							return false;
						}
						if (valueOR.GetValueType() != ScriptValueType.Number) {
							return false;
						}
						valueResult = _Calc((double) valueOL.GetValue(), (double) valueOR.GetValue(), tempSign);
					} while (false);
					calcSrc = calcSrc.Insert(endPosOR, valueResult.ToString());
					calcSrc = calcSrc.Remove(startPosOL, endPosOR - startPosOL);
				}
				if (!Execute(calcSrc, space, out result)) {
					return false;
				}
				return true;
			} while (false);

			return false;
		}

		private static double _Calc(double value1, double value2, char sign) {
			switch (sign) {
			case '+':
				return value1 + value2;
			case '-':
				return value1 - value2;
			case '*':
				return value1 * value2;
			case '/':
				return value1 / value2;
			case '%':
				return value1 % value2;
			case '^':
				return Math.Pow(value1, value2);
			}
			return 0;
		}

		private static bool _Compare(ScriptValue value1, ScriptValue value2, string sign) {
			if (sign == Grammar.COMPARE_EQUIP) {
				return ScriptValue.Compare(value1, value2);
			} else if (sign == Grammar.COMPARE_NOT_EQUIP) {
				return !ScriptValue.Compare(value1, value2);
			} else if (sign == Grammar.COMPARE_LESS) {
				return ScriptValue.Less(value1, value2);
			} else if (sign == Grammar.COMPARE_LESS_EQUAL) {
				return ScriptValue.LessEqual(value1, value2);
			} else if (sign == Grammar.COMPARE_MORE) {
				return ScriptValue.More(value1, value2);
			} else if (sign == Grammar.COMPARE_MORE_EQUAL) {
				return ScriptValue.MoreEqual(value1, value2);
			}
			return false;
		}
	}
}