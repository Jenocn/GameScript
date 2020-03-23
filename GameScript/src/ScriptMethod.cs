/*
 * By Jenocn
 * https://jenocn.github.io/
 */

using System.Collections.Generic;

namespace gs.compiler {
	public sealed class ScriptMethod {

		public enum ScriptMethodType {
			None = 0,
			Loop,
			Condition,
		}

		private string _srcBody = "";
		private string _name = "";
		private List<string> _params = new List<string>();

		private ScriptMethod _parent = null;
		// temp space will clear when execute end
		private Dictionary<string, ScriptMethod> _methods = new Dictionary<string, ScriptMethod>();
		private Dictionary<string, ScriptUsing> _usings = new Dictionary<string, ScriptUsing>();
		private Dictionary<string, ScriptObject> _objects = new Dictionary<string, ScriptObject>();

		// always
		private Dictionary<string, ScriptObject> _registerObjects = new Dictionary<string, ScriptObject>();
		private Dictionary<string, ScriptObject> _strings = new Dictionary<string, ScriptObject>();
		private System.Func<List<ScriptValue>, ScriptValue> _func = null;
		private MethodPool _methodPool = new MethodPool();
		private ScriptMethodType _scriptMethodType = ScriptMethodType.None;
		private bool _bExecuted = false;

		public ScriptMethod(System.Func<List<ScriptValue>, ScriptValue> func) {
			_func = func;
		}

		public ScriptMethod(string srcHeader, string srcBody, ScriptMethod parent = null) {
			_ParseHeader(srcHeader);
			_srcBody = srcBody;
			_parent = parent;
			_ParseSrcbody();
		}

		public ScriptMethod(string srcBody, ScriptMethod parent = null, ScriptMethodType type = ScriptMethodType.None) {
			_srcBody = srcBody;
			_parent = parent;
			_scriptMethodType = type;
			_ParseSrcbody();
		}

		public static ScriptMethod NewMethodForUsing(ScriptMethod method) {
			var ret = new ScriptMethod("");
			ret._strings = method._strings;
			ret._srcBody = method._srcBody;
			return ret;
		}

		private void _ParseSrcbody() {
			_srcBody = tool.GrammarTool.CutComments(_srcBody).Trim();
			bool bSS = false;
			int ssBP = 0;
			int previousPos = 0;
			string newStr = "";
			for (int i = 0; i < _srcBody.Length; ++i) {
				char ch = _srcBody[i];
				if (ch != Grammar.SS) { continue; }
				bSS = !bSS;
				if (bSS) {
					ssBP = i;
				} else {
					var str = _srcBody.Substring(ssBP, i - ssBP + 1);
					var name = "##_" + _strings.Count + "_##";
					_strings.Add(name, new ScriptObject(name, ScriptValue.Create(str)));
					newStr += _srcBody.Substring(previousPos, ssBP - previousPos);
					newStr += name;
					previousPos = i + 1;
				}
			}
			if (previousPos < _srcBody.Length) {
				newStr += _srcBody.Substring(previousPos);
			}
			_srcBody = newStr;
		}

		private void _ParseHeader(string srcHeader) {
			int fpbPos = srcHeader.IndexOf(Grammar.FPB);
			int fpePos = tool.GrammarTool.ReadPairSignPos(srcHeader, fpbPos + 1, Grammar.FPB, Grammar.FPE);
			if (fpbPos == -1 || fpbPos >= fpePos) {
				Logger.Error(srcHeader);
				return;
			}
			var nameSrc = srcHeader.Substring(0, fpbPos).Trim();
			if (string.IsNullOrEmpty(nameSrc)) {
				Logger.Error(srcHeader);
				return;
			}
			if (nameSrc.IndexOfAny(Grammar.SPECIAL_CHAR) != -1) {
				Logger.Error(srcHeader);
				return;
			}

			var paramSrc = srcHeader.Substring(fpbPos + 1, fpePos - fpbPos - 1).Trim();
			if (!string.IsNullOrEmpty(paramSrc)) {
				var paramsSrcList = paramSrc.Split(Grammar.FPS);
				foreach (var str in paramsSrcList) {
					string paramName = str.Trim();
					if (string.IsNullOrEmpty(paramName)) {
						Logger.Error(srcHeader);
						return;
					}
					if (paramName.IndexOfAny(Grammar.SPECIAL_CHAR) != -1) {
						Logger.Error(srcHeader);
						return;
					}
					if (_params.Contains(paramName)) {
						Logger.Error(srcHeader);
						return;
					}
					_params.Add(paramName);
				}
			}

			_name = nameSrc;
		}

		public bool Execute(List<ScriptValue> args) {
			bool bReturn = false;
			ScriptValue ret = ScriptValue.NULL;
			return Execute(args, out bReturn, out ret);
		}

		public bool Execute(List<ScriptValue> args, out bool bMethodReturn, out ScriptValue methodReturnResult) {
			bool bBreak = false;
			bool bContinue = false;
			return Execute(args, out bMethodReturn, out methodReturnResult, out bBreak, out bContinue);
		}

		public bool Execute(List<ScriptValue> args, out bool bMethodReturn, out ScriptValue methodReturnResult, out bool bMethodBreak, out bool bMethodContinue) {

			_bExecuted = true;
			bMethodReturn = false;
			methodReturnResult = ScriptValue.NULL;
			bMethodBreak = false;
			bMethodContinue = false;
			_Clear();

			if (_func != null) {
				methodReturnResult = _func(args);
				return true;
			}

			// args
			if (args != null && args.Count > 0) {
				for (int i = 0; i < _params.Count; ++i) {
					string paramName = _params[i];
					ScriptValue paramValue = ScriptValue.NULL;
					if (i < args.Count) {
						paramValue = args[i];
					}
					_objects.Add(paramName, new ScriptObject(paramName, paramValue));
				}
			}

			int readPos = 0;
			while (true) {
				if (readPos >= _srcBody.Length) {
					break;
				}

				var overPos = _srcBody.IndexOf(Grammar.OVER, readPos);
				var fcbPos = _srcBody.IndexOf(Grammar.FCB, readPos);
				if (overPos == -1 && fcbPos == -1) {
					Logger.Error(_srcBody);
					return false;
				}

				// function, if, while
				if ((fcbPos != -1 && fcbPos < overPos) || overPos == -1) {
					var fcePos = tool.GrammarTool.ReadPairSignPos(_srcBody, fcbPos + 1, Grammar.FCB, Grammar.FCE);
					if (fcePos == -1) {
						Logger.Error(_srcBody);
						return false;
					}

					var srcNewHeader = _srcBody.Substring(readPos, fcbPos - readPos).Trim();
					var srcNewBody = _srcBody.Substring(fcbPos + 1, fcePos - fcbPos - 1);

					readPos = fcePos + 1;

					var ifPos = srcNewHeader.IndexOf(Grammar.IF);
					if (ifPos == 0) {
						// if
						var ifSrcList = new List<KeyValuePair<string, string>>();
						ifSrcList.Add(new KeyValuePair<string, string>(srcNewHeader, srcNewBody));

						string srcElseBody = "";

						// elseif
						while (true) {
							var elsePos = _srcBody.IndexOf(Grammar.ELSE, readPos); // else
							if (elsePos == -1) { break; }
							var tempSpaceSrc = _srcBody.Substring(readPos, elsePos - readPos).Trim();
							if (!string.IsNullOrEmpty(tempSpaceSrc)) {
								break;
							}

							var elseFcbPos = _srcBody.IndexOf(Grammar.FCB, readPos); // {
							if (elseFcbPos == -1) {
								Logger.Error(_srcBody);
								return false;
							}
							var elseFcePos = tool.GrammarTool.ReadPairSignPos(_srcBody, elseFcbPos + 1, Grammar.FCB, Grammar.FCE);
							if (elseFcePos == -1) {
								Logger.Error(_srcBody);
								return false;
							}

							var tempElseBody = _srcBody.Substring(elseFcbPos + 1, elseFcePos - elseFcbPos - 1);

							readPos = elseFcePos + 1;

							var tempSpaceHeaderSrc = _srcBody.Substring(elsePos + Grammar.ELSE.Length, elseFcbPos - elsePos - Grammar.ELSE.Length).Trim();
							if (tempSpaceHeaderSrc.Length == 0) {
								// else
								srcElseBody = tempElseBody;
								break;
							} else {
								// else if
								var elseifPos = tempSpaceHeaderSrc.IndexOf(Grammar.IF);
								if (elseifPos == -1) {
									Logger.Error(_srcBody);
									return false;
								}
								var elseifFpbPos = tempSpaceHeaderSrc.IndexOf(Grammar.FPB);
								var elseifFpePos = tempSpaceHeaderSrc.IndexOf(Grammar.FPE);
								if (elseifFpePos <= elseifFpbPos) {
									Logger.Error(_srcBody);
									return false;
								}
								ifSrcList.Add(new KeyValuePair<string, string>(tempSpaceHeaderSrc, tempElseBody));
							}
						}

						bool bCondition = false;
						foreach (var pair in ifSrcList) {
							var ifHeader = pair.Key;
							var ifBody = pair.Value;
							if (!ScriptIf.Execute(ifHeader, this, out bCondition)) {
								Logger.Error(ifHeader);
								return false;
							}
							if (bCondition) {
								var conditionExe = new ScriptMethod(ifBody, this, ScriptMethodType.Condition);
								ScriptValue conditionResult = ScriptValue.NULL;
								bool bConditionReturn = false;
								bool bConditionBreak = false;
								bool bConditionContinue = false;
								if (!conditionExe.Execute(null, out bConditionReturn, out conditionResult, out bConditionBreak, out bConditionContinue)) {
									Logger.Error(ifHeader);
									return false;
								}
								if (bConditionReturn) {
									methodReturnResult = conditionResult;
									return true;
								}
								if (bConditionBreak || bConditionContinue) {
									if (!FindScriptMethodType(ScriptMethodType.Loop)) {
										Logger.Error(ifHeader);
										return false;
									}
									bMethodBreak = bConditionBreak;
									bMethodContinue = bConditionContinue;
									return true;
								}
								break;
							}
						}
						if (!bCondition) {
							// else
							if (srcElseBody.Length == 0) { continue; }
							var elseExe = new ScriptMethod(srcElseBody, this, ScriptMethodType.Condition);
							ScriptValue conditionResult = ScriptValue.NULL;
							bool bConditionReturn = false;
							bool bConditionBreak = false;
							bool bConditionContinue = false;
							if (!elseExe.Execute(null, out bConditionReturn, out conditionResult, out bConditionBreak, out bConditionContinue)) {
								Logger.Error(srcElseBody);
								return false;
							}
							if (bConditionReturn) {
								methodReturnResult = conditionResult;
								return true;
							}
							if (bConditionBreak || bConditionContinue) {
								if (!FindScriptMethodType(ScriptMethodType.Loop)) {
									Logger.Error(srcElseBody);
									return false;
								}
								bMethodBreak = bConditionBreak;
								bMethodContinue = bConditionContinue;
								return true;
							}
						}
						continue;
					}

					// foreach
					var foreachPos = srcNewHeader.IndexOf(Grammar.FOREACH);
					if (foreachPos == 0) {
						var foreachParamStr = srcNewHeader.Substring(Grammar.FOREACH.Length).Trim();
						if (foreachParamStr.Length < Grammar.FOREACH_IN.Length + 6) {
							Logger.Error(srcNewHeader);
							return false;
						}
						if (foreachParamStr[0] != Grammar.FPB && foreachParamStr[foreachParamStr.Length - 1] != Grammar.FPE) {
							Logger.Error(srcNewHeader);
							return false;
						}
						var foreachInPos = tool.GrammarTool.ReadSingleSignPos(foreachParamStr, Grammar.FOREACH_IN);
						if (foreachInPos == -1) {
							Logger.Error(srcNewHeader);
							return false;
						}
						var tempParamNameStr = foreachParamStr.Substring(1, foreachInPos - 1).Trim();
						var tempListName = foreachParamStr.Substring(foreachInPos + Grammar.FOREACH_IN.Length, foreachParamStr.Length - foreachInPos - Grammar.FOREACH_IN.Length - 1).Trim();
						var obj = FindObject(tempListName);
						if (obj == null) {
							Logger.Error(srcNewHeader);
							return false;
						}
						var objValue = obj.GetValue();
						if (objValue.GetValueType() != ScriptValueType.List) {
							Logger.Error(srcNewHeader);
							return false;
						}

						var tempParamName = tempParamNameStr;
						var tempParamIndexName = "";
						var tempParamNameStrSplit = tempParamNameStr.Split(Grammar.FPS);
						if (tempParamNameStrSplit.Length > 1) {
							tempParamName = tempParamNameStrSplit[0].Trim();
							tempParamIndexName = tempParamNameStrSplit[1].Trim();
						}
						var tempList = (List<ScriptValue>) objValue.GetValue();
						int index = 0;
						foreach (var item in tempList) {
							var conditionExe = new ScriptMethod(srcNewBody, this, ScriptMethodType.Loop);
							conditionExe.RegisterObject(tempParamName, item);
							if (!string.IsNullOrEmpty(tempParamIndexName)) {
								conditionExe.RegisterObject(tempParamIndexName, ScriptValue.Create(index));
							}
							++index;
							var conditionResult = ScriptValue.NULL;
							bool bConditionReturn = false;
							bool bConditionBreak = false;
							bool bConditionContinue = false;
							if (!conditionExe.Execute(null, out bConditionReturn, out conditionResult, out bConditionBreak, out bConditionContinue)) {
								Logger.Error(srcNewHeader);
								return false;
							}
							if (bConditionReturn) {
								methodReturnResult = conditionResult;
								return true;
							}
							if (bConditionBreak) {
								break;
							}
							if (bConditionContinue) {
								continue;
							}
						}
						continue;
					}

					// while
					var whilePos = srcNewHeader.IndexOf(Grammar.WHILE);
					if (whilePos == 0) {
						var whileHeader = srcNewHeader.Replace(Grammar.WHILE, Grammar.IF);
						while (true) {
							bool bCondition = false;
							if (!ScriptIf.Execute(whileHeader, this, out bCondition)) {
								Logger.Error(srcNewHeader);
								return false;
							}
							if (!bCondition) {
								break;
							}
							var conditionExe = new ScriptMethod(srcNewBody, this, ScriptMethodType.Loop);
							ScriptValue conditionResult = ScriptValue.NULL;
							bool bConditionReturn = false;
							bool bConditionBreak = false;
							bool bConditionContinue = false;
							if (!conditionExe.Execute(null, out bConditionReturn, out conditionResult, out bConditionBreak, out bConditionContinue)) {
								Logger.Error(srcNewHeader);
								return false;
							}
							if (bConditionReturn) {
								methodReturnResult = conditionResult;
								return true;
							}
							if (bConditionBreak) {
								break;
							}
							if (bConditionContinue) {
								continue;
							}
						}
						continue;
					}

					// function
					var method = new ScriptMethod(srcNewHeader, srcNewBody, this);
					var methodName = method._name;
					if (!string.IsNullOrEmpty(methodName)) {
						if (_methods.ContainsKey(methodName)) {
							Logger.Error(srcNewHeader, methodName + " is exists!");
							continue;
						}
						_methods.Add(method._name, method);
					}
					continue;
				}

				// sentence
				var sentence = _srcBody.Substring(readPos, overPos - readPos).Trim();
				while (true) {
					int ssCount = tool.GrammarTool.CountSign(sentence, Grammar.SS);
					if (ssCount % 2 == 0) { break; }
					overPos = _srcBody.IndexOf(Grammar.OVER, overPos + 1);
					if (overPos == -1) {
						Logger.Error(_srcBody);
						return false;
					}
					sentence = _srcBody.Substring(readPos, overPos - readPos).Trim();
				}
				readPos = overPos + 1;

				// return
				var returnPos = sentence.IndexOf(Grammar.RETURN);
				if (returnPos == 0) {
					if (sentence.Length <= Grammar.RETURN.Length + 1) {
						Logger.Error(sentence);
						return false;
					}
					if (!string.IsNullOrEmpty(sentence.Substring(Grammar.RETURN.Length, 1).Trim())) {
						Logger.Error(sentence);
						return false;
					}
					bMethodReturn = true;
					ScriptValue result = ScriptValue.NULL;
					var returnValueStr = sentence.Substring(Grammar.RETURN.Length + 1).Trim();
					if (!ScriptMethodCall.Execute(returnValueStr, this, out result)) {
						if (!ScriptExpression.Execute(returnValueStr, this, out result)) {
							Logger.Error(sentence);
							return false;
						}
					}
					methodReturnResult = result;
					return true;
				}

				// array
				var arrayPos = sentence.IndexOf(Grammar.ARRAY);
				if (arrayPos == 0) {
					if (sentence.Length <= Grammar.ARRAY.Length + 1) {
						Logger.Error(sentence);
						return false;
					}
					if (!string.IsNullOrEmpty(sentence.Substring(Grammar.ARRAY.Length, 1).Trim())) {
						Logger.Error(sentence);
						return false;
					}
					var arrayRightStr = sentence.Substring(Grammar.USING.Length + 1).Trim();
					if (string.IsNullOrEmpty(arrayRightStr)) {
						Logger.Error(sentence);
						return false;
					}

					var arrbPos = arrayRightStr.IndexOf(Grammar.ARRB);
					if (arrbPos == -1) {
						Logger.Error(sentence);
						return false;
					}

					var arrayRightValueStr = arrayRightStr.Substring(arrbPos).Trim();
					if (arrayRightValueStr.Length < 3) {
						Logger.Error(sentence);
						return false;
					}

					var arrayRightNameStr = arrayRightStr.Substring(0, arrbPos).Trim();
					if (string.IsNullOrEmpty(arrayRightNameStr)) {
						Logger.Error(sentence);
						return false;
					}
					if (FindObjectFromSelf(arrayRightNameStr) != null) {
						Logger.Error(sentence, arrayRightNameStr + " is exists!");
						return false;
					}

					int arrayEndPos = tool.GrammarTool.ReadPairSignPos(arrayRightValueStr, 1, Grammar.ARRB, Grammar.ARRE);
					if (arrayEndPos <= 0 || arrayEndPos > arrayRightValueStr.Length - 1) {
						Logger.Error(sentence);
						return false;
					}
					var valueStr = arrayRightValueStr.Substring(1, arrayEndPos - 1);
					var valueStrSplitArr = valueStr.Split(Grammar.ARRAY_SPLIT);
					if (valueStrSplitArr.Length > 2) {
						Logger.Error(sentence);
						return false;
					}
					var arrBaseValueStr = valueStrSplitArr[0].Trim();
					if (string.IsNullOrEmpty(arrBaseValueStr)) {
						Logger.Error(sentence);
						return false;
					}
					int arrBaseValue = 0;
					if (!int.TryParse(arrBaseValueStr, out arrBaseValue)) {
						Logger.Error(sentence);
						return false;
					}

					int arrStarIndex = 0;
					int arrEndIndex = arrBaseValue - 1;

					if (valueStrSplitArr.Length == 2) {
						var arrNextValueStr = valueStrSplitArr[1].Trim();
						if (string.IsNullOrEmpty(arrNextValueStr)) {
							Logger.Error(sentence);
							return false;
						}
						int arrNextValue = 0;
						if (!int.TryParse(arrNextValueStr, out arrNextValue)) {
							Logger.Error(sentence);
							return false;
						}

						arrStarIndex = arrBaseValue;
						arrEndIndex = arrNextValue;
					}
					if (arrStarIndex >= arrEndIndex) {
						Logger.Error(sentence);
						return false;
					}
					for (int i = arrStarIndex; i <= arrEndIndex; ++i) {
						var leftName = arrayRightNameStr + Grammar.ARRB + i + Grammar.ARRE;
						if (FindObjectFromSelf(leftName) != null) {
							Logger.Error(sentence, leftName + " is exists!");
							return false;
						}
						_objects.Add(leftName, new ScriptObject(leftName, ScriptValue.NULL));
					}
					continue;
				}

				// using
				var usingPos = sentence.IndexOf(Grammar.USING);
				if (usingPos == 0) {
					if (sentence.Length <= Grammar.USING.Length + 1) {
						Logger.Error(sentence);
						return false;
					}
					if (!string.IsNullOrEmpty(sentence.Substring(Grammar.USING.Length, 1).Trim())) {
						Logger.Error(sentence);
						return false;
					}
					var usingSpaceName = sentence.Substring(Grammar.USING.Length + 1).Trim();
					if (string.IsNullOrEmpty(usingSpaceName)) {
						Logger.Error(sentence);
						return false;
					}
					var usingSpace = UsingMemory.Get(usingSpaceName);
					if (usingSpace == null) {
						Logger.Error(sentence);
						return false;
					}
					if (_usings.ContainsKey(usingSpaceName)) {
						Logger.Error(sentence);
						return false;
					}
					if (!usingSpace.ExecuteUsing()) {
						Logger.Error(sentence);
						return false;
					}
					_usings.Add(usingSpaceName, usingSpace);
					continue;
				}

				// new
				var newPos = sentence.IndexOf(Grammar.NEW);
				if (newPos == 0) {
					if (sentence.Length <= Grammar.NEW.Length + 1) {
						Logger.Error(sentence);
						return false;
					}
					if (!string.IsNullOrEmpty(sentence.Substring(Grammar.NEW.Length, 1).Trim())) {
						Logger.Error(sentence);
						return false;
					}
					var newSpaceName = sentence.Substring(Grammar.NEW.Length + 1).Trim();
					if (string.IsNullOrEmpty(newSpaceName)) {
						Logger.Error(sentence);
						return false;
					}
					var tempSpace = UsingMemory.Get(newSpaceName);
					if (tempSpace == null) {
						Logger.Error(sentence);
						return false;
					}
					if (_usings.ContainsKey(newSpaceName)) {
						Logger.Error(sentence);
						return false;
					}
					var newSpace = tempSpace.Clone();
					if ((newSpace == null) || (!newSpace.ExecuteUsing())) {
						Logger.Error(sentence);
						return false;
					}
					_usings.Add(newSpaceName, newSpace);
					continue;
				}

				// break;
				var breakPos = sentence.IndexOf(Grammar.BREAK);
				if (breakPos == 0) {
					bMethodBreak = true;
					if (!FindScriptMethodType(ScriptMethodType.Loop)) {
						Logger.Error(sentence);
						return false;
					}
					return true;
				}
				// continue;
				var continuePos = sentence.IndexOf(Grammar.CONTINUE);
				if (continuePos == 0) {
					bMethodContinue = true;
					if (!FindScriptMethodType(ScriptMethodType.Loop)) {
						Logger.Error(sentence);
						return false;
					}
					return true;
				}

				bool bAssign = false;
				do {
					var assignPos = sentence.IndexOf(Grammar.ASSIGN);
					if (assignPos == -1) { break; }
					if (sentence.IndexOf(Grammar.COMPARE_EQUIP) == assignPos) {
						break;
					}
					if (sentence.IndexOf(Grammar.COMPARE_LESS_EQUAL) == assignPos - 1) {
						break;
					}
					if (sentence.IndexOf(Grammar.COMPARE_MORE_EQUAL) == assignPos - 1) {
						break;
					}

					// assign sentence
					var srcLeft = sentence.Substring(0, assignPos).Trim();
					var srcRight = sentence.Substring(assignPos + 1).Trim();

					ScriptValue result = ScriptValue.NULL;
					bool bMethodCallSuccess = false;
					var rFcbPos = srcRight.IndexOf(Grammar.FPB);
					if (rFcbPos != -1) {
						// method call
						bMethodCallSuccess = ScriptMethodCall.Execute(srcRight, this, out result);
					}
					if (!bMethodCallSuccess) {
						// expression
						if (!ScriptExpression.Execute(srcRight, this, out result)) {
							Logger.Error(sentence);
							return false;
						}
					}

					var varBeginPos = srcLeft.IndexOf(Grammar.VAR);
					if (varBeginPos == 0) {
						// var new object
						var leftName = srcLeft.Substring(varBeginPos + Grammar.VAR.Length).Trim();
						if (leftName.IndexOfAny(Grammar.SPECIAL_CHAR) != -1) {
							Logger.Error(sentence);
							return false;
						}
						if (FindObjectFromSelf(leftName) != null) {
							Logger.Error(sentence, leftName + " is exists!");
							return false;
						}
						_objects.Add(leftName, new ScriptObject(leftName, result));
					} else {
						var leftName = srcLeft.Trim();
						if (leftName.IndexOfAny(Grammar.SPECIAL_CHAR) != -1) {
							Logger.Error(sentence);
							return false;
						}
						var obj = FindObject(leftName);
						if (obj == null) {
							Logger.Error(sentence);
							return false;
						}
						obj.SetValue(result);
					}
					bAssign = true;
				} while (false);

				if (!bAssign) {
					// method call
					ScriptValue tempRet = ScriptValue.NULL;
					if (!ScriptMethodCall.Execute(sentence, this, out tempRet)) {
						Logger.Error(sentence);
						return false;
					}
				}
			}

			return true;
		}

		public bool RegisterMethod(string name, System.Func<List<ScriptValue>, ScriptValue> func) {
			bool bRet = _methodPool.AddMethod(name, func);
			if (!bRet) {
				Logger.Error(string.Format("The method named \"{0}\" is exists!", name));
			}
			return bRet;
		}

		public ScriptMethod FindMethod(string name) {
			ScriptMethod ret = null;
			if (_methods.TryGetValue(name, out ret)) {
				return ret;
			}
			ret = _methodPool.GetMethod(name);
			if (ret != null) {
				return ret;
			}
			foreach (var item in _usings) {
				ret = item.Value.GetScriptMethod().FindMethod(name);
				if (ret != null) {
					return ret;
				}
			}
			if (_parent != null) {
				ret = _parent.FindMethod(name);
				if (ret != null) {
					return ret;
				}
			}
			return null;
		}

		public bool RegisterObject(string name, ScriptValue value) {
			if (string.IsNullOrEmpty(name)) {
				return false;
			}
			if (FindObject(name) != null) {
				return false;
			}
			_registerObjects.Add(name, new ScriptObject(name, value));
			return true;
		}

		public ScriptObject FindObject(string name) {
			name = _ConvertObjectName(name);
			ScriptObject ret = null;
			if (_registerObjects.TryGetValue(name, out ret)) {
				return ret;
			}
			if (_objects.TryGetValue(name, out ret)) {
				return ret;
			}
			if (_strings.TryGetValue(name, out ret)) {
				return ret;
			}
			foreach (var item in _usings) {
				ret = item.Value.GetScriptMethod().FindObject(name);
				if (ret != null) {
					return ret;
				}
			}
			if (_parent != null) {
				ret = _parent.FindObject(name);
				if (ret != null) {
					return ret;
				}
			}
			return null;
		}

		private ScriptObject FindObjectFromSelf(string name) {
			name = _ConvertObjectName(name);
			ScriptObject ret = null;
			if (_objects.TryGetValue(name, out ret)) {
				return ret;
			}
			if (_strings.TryGetValue(name, out ret)) {
				return ret;
			}
			return null;
		}

		public ScriptMethodType GetScriptMethodType() {
			return _scriptMethodType;
		}
		public bool FindScriptMethodType(ScriptMethodType type) {
			if (_scriptMethodType == type) {
				return true;
			}
			if (_parent != null) {
				return _parent.FindScriptMethodType(type);
			}
			return false;
		}

		public bool IsExecuted() {
			return _bExecuted;
		}

		private string _ConvertObjectName(string name) {
			var arrb = name.IndexOf(Grammar.ARRB);
			if (arrb == -1) { return name; }
			var arre = gs.compiler.tool.GrammarTool.ReadPairSignPos(name, arrb + 1, Grammar.ARRB, Grammar.ARRE);
			if (arre == -1) { return name; }
			var sign = name.Substring(arrb + 1, arre - arrb - 1);
			var findObj = FindObject(sign.Trim());
			if (findObj == null) {
				return name;
			}
			return name.Replace(sign, findObj.GetValue().ToString());
		}

		private void _Clear() {
			_methods.Clear();
			_objects.Clear();
			_usings.Clear();
		}
	}
}