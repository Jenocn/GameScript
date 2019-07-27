using System.Collections.Generic;

namespace gs.compiler {
	public sealed class ScriptMethod {

		private string _srcBody = "";
		private string _name = "";
		private List<string> _params = new List<string>();

		private ScriptMethod _parent = null;
		private Dictionary<string, ScriptMethod> _methods = new Dictionary<string, ScriptMethod>();
		private Dictionary<string, ScriptObject> _objects = new Dictionary<string, ScriptObject>();

		public ScriptMethod(string srcHeader, string srcBody, ScriptMethod parent = null) {
			_srcBody = srcBody.Trim();
			_parent = parent;
			_ParseHeader(srcHeader);
		}

		public ScriptMethod(string srcBody, ScriptMethod parent = null) {
			_srcBody = srcBody.Trim();
			_parent = parent;
		}

		public void Parse(string srcBody) {
			_srcBody = srcBody;
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
			methodReturnResult = new ScriptValue();
			_objects.Clear();

			// args
			if (args != null && args.Count > 0) {
				for (int i = 0; i < _params.Count; ++i) {
					string paramName = _params[i];
					ScriptValue paramValue = new ScriptValue();
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
				var commentPos = _srcBody.IndexOf(Grammar.COMMENT, readPos);
				if (overPos == -1 && fcbPos == -1) {
					if (commentPos != -1) {
						var tailPos = _srcBody.IndexOf('\n', commentPos);
						if (tailPos == -1) {
							tailPos = _srcBody.Length;
						}
						readPos = tailPos + 1;
						continue;
					}
					Logger.Error(_srcBody);
					return false;
				}

				// function, if
				if ((fcbPos != -1 && fcbPos < overPos) || overPos == -1) {

					if (commentPos != -1) {
						if (commentPos < fcbPos) {
							var tailPos = _srcBody.IndexOf('\n', commentPos);
							if (tailPos == -1) {
								tailPos = _srcBody.Length;
							}
							readPos = tailPos + 1;
							continue;
						}
					}

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
						bool bCondition = false;
						if (!ScriptIf.Execute(srcNewHeader, this, out bCondition)) {
							Logger.Error(srcNewHeader);
							continue;
						}

						if (!bCondition) {
							// else
							// todo...
							continue;
						}
						var conditionExe = new ScriptMethod(srcNewBody, this);
						ScriptValue conditionResult = new ScriptValue();
						bool bConditionReturn = false;
						if (!conditionExe.Execute(null, out bConditionReturn, out conditionResult)) {
							Logger.Error(srcNewHeader);
							continue;
						}
						if (bConditionReturn) {
							methodReturnResult = conditionResult;
							return true;
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
				if (commentPos != -1) {
					if (commentPos < overPos) {
						var tailPos = _srcBody.IndexOf('\n', commentPos);
						if (tailPos == -1) {
							tailPos = _srcBody.Length;
						}
						readPos = tailPos + 1;
						continue;
					}
				}

				var sentence = _srcBody.Substring(readPos, overPos - readPos).Trim();
				readPos = overPos + 1;

				// return
				var returnPos = sentence.IndexOf(Grammar.RETURN);
				if (returnPos == 0) {
					bMethodReturn = true;
					ScriptValue result = new ScriptValue();
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

					ScriptValue result = new ScriptValue();
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
						var obj = FindObject(leftName);
						if (obj != null) {
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
					ScriptValue tempRet = new ScriptValue();
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
			ScriptObject ret = null;
			if (!_objects.TryGetValue(name, out ret)) {
				if (_parent != null) {
					return _parent.FindObject(name);
				}
			}
			return ret;
		}
	}
}
