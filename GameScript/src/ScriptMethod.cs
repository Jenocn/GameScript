﻿/*
 * By Jenocn
 * https://jenocn.github.io/
*/

using System.Collections.Generic;

namespace gs.compiler {
	public sealed class ScriptMethod {

		private string _srcBody = "";
		private string _name = "";
		private List<string> _params = new List<string>();

		private ScriptMethod _parent = null;
		private Dictionary<string, ScriptMethod> _methods = new Dictionary<string, ScriptMethod>();
		private Dictionary<string, ScriptObject> _objects = new Dictionary<string, ScriptObject>();
		private Dictionary<string, ScriptObject> _strings = new Dictionary<string, ScriptObject>();

		public ScriptMethod(string srcHeader, string srcBody, ScriptMethod parent = null) {
			_srcBody = srcBody;
			_parent = parent;
			_ParseHeader(srcHeader);
			_ParseSrcbody();
		}

		public ScriptMethod(string srcBody, ScriptMethod parent = null) {
			_srcBody = srcBody;
			_parent = parent;
			_ParseSrcbody();
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

		public bool Execute(List<ScriptValue> args, out bool bMethodReturn, out ScriptValue methodReturnResult) {

			bMethodReturn = false;
			methodReturnResult = ScriptValue.NULL;
			_objects.Clear();

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
								var conditionExe = new ScriptMethod(ifBody, this);
								ScriptValue conditionResult = ScriptValue.NULL;
								bool bConditionReturn = false;
								if (!conditionExe.Execute(null, out bConditionReturn, out conditionResult)) {
									Logger.Error(ifHeader);
									return false;
								}
								if (bConditionReturn) {
									methodReturnResult = conditionResult;
									return true;
								}
								break;
							}
						}
						if (!bCondition) {
							// else
							if (srcElseBody.Length == 0) { continue; }
							var elseExe = new ScriptMethod(srcElseBody, this);
							ScriptValue conditionResult = ScriptValue.NULL;
							bool bConditionReturn = false;
							if (!elseExe.Execute(null, out bConditionReturn, out conditionResult)) {
								Logger.Error(srcElseBody);
								return false;
							}
							if (bConditionReturn) {
								methodReturnResult = conditionResult;
								return true;
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
							var conditionExe = new ScriptMethod(srcNewBody, this);
							ScriptValue conditionResult = ScriptValue.NULL;
							bool bConditionReturn = false;
							if (!conditionExe.Execute(null, out bConditionReturn, out conditionResult)) {
								Logger.Error(srcNewHeader);
								return false;
							}
							if (bConditionReturn) {
								methodReturnResult = conditionResult;
								return true;
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
					bMethodReturn = true;
					ScriptValue result = ScriptValue.NULL;
					var returnValueStr = sentence.Substring(Grammar.RETURN.Length).Trim();
					var returnFpbPos = returnValueStr.IndexOf(Grammar.FPB);
					if (returnFpbPos != -1) {
						// method
						if (!ScriptMethodCall.Execute(returnValueStr, this, out result)) {
							Logger.Error(sentence);
							return false;
						}
					} else {

						if (!ScriptExpression.Execute(returnValueStr, this, out result)) {
							Logger.Error(sentence);
							return false;
						}
					}
					methodReturnResult = result;
					return true;
				}

				var assignPos = sentence.IndexOf(Grammar.ASSIGN);
				if (assignPos != -1) {
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
							continue;
						}
					}

					var varBeginPos = srcLeft.IndexOf(Grammar.VAR);
					if (varBeginPos == 0) {
						// var new object
						var leftName = srcLeft.Substring(varBeginPos + Grammar.VAR.Length).Trim();
						if (leftName.IndexOfAny(Grammar.SPECIAL_CHAR) != -1) {
							Logger.Error(sentence);
							continue;
						}
						if (FindObject(leftName) != null) {
							Logger.Error(sentence, leftName + " is exists!");
							continue;
						}
						_objects.Add(leftName, new ScriptObject(leftName, result));
					} else {
						var leftName = srcLeft.Trim();
						if (leftName.IndexOfAny(Grammar.SPECIAL_CHAR) != -1) {
							Logger.Error(sentence);
							continue;
						}
						var obj = FindObject(leftName);
						if (obj == null) {
							Logger.Error(sentence);
							continue;
						}
						obj.SetValue(result);
					}

				} else {
					// method call
					ScriptValue tempRet = ScriptValue.NULL;
					if (!ScriptMethodCall.Execute(sentence, this, out tempRet)) {
						Logger.Error(sentence);
						continue;
					}
				}
			}
			return true;
		}

		public ScriptMethod FindMethod(string name) {
			ScriptMethod ret = null;
			if (!_methods.TryGetValue(name, out ret)) {
				if (_parent != null) {
					return _parent.FindMethod(name);
				}
			}
			return ret;
		}

		public ScriptObject FindObject(string name) {
			name = _ConvertObjectName(name);
			ScriptObject ret = null;
			if (!_objects.TryGetValue(name, out ret)) {
				if (!_strings.TryGetValue(name, out ret)) {
					if (_parent != null) {
						return _parent.FindObject(name);
					}
				}
			}
			return ret;
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
	}
}
