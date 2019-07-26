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

		private void _ParseHeader(string srcHeader) {
			int fpbPos = srcHeader.IndexOf(Grammar.FPB);
			int fpePos = srcHeader.IndexOf(Grammar.FPE);
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
					_params.Add(paramName);
				}
			}

			_name = nameSrc;
		}

		public ScriptValue Execute(List<ScriptValue> args) {
			_objects.Clear();

			// args
			if (args != null && args.Count > 0) {
				for (int i = 0; i < _params.Count; ++i) {
					string paramName = _params[i];
					ScriptValue paramValue = null;
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
					return null;
				}
				// function
				if ((fcbPos != -1 && fcbPos < overPos) || overPos == -1) {

					var fcePos = _ReadNextFCE(_srcBody, fcbPos + 1);
					if (fcePos == -1) {
						Logger.Error(_srcBody);
						return null;
					}

					var srcNewHeader = _srcBody.Substring(readPos, fcbPos - readPos);
					var srcNewBody = _srcBody.Substring(fcbPos + 1, fcePos - fcbPos - 1);
					readPos = fcePos + 1;

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
				var sentence = _srcBody.Substring(readPos, overPos - readPos);
				readPos = overPos + 1;

				var assignPos = sentence.IndexOf(Grammar.ASSIGN);
				if (assignPos != -1) {
					// assign sentence
					var srcLeft = sentence.Substring(0, assignPos);
					var srcRight = sentence.Substring(assignPos + 1).Trim();

					ScriptValue result = null;
					var rFcbPos = srcRight.IndexOf(Grammar.FPB);
					if (rFcbPos != -1) {
						// method call
						var rFcePos = srcRight.IndexOf(Grammar.FPE);
						if (rFcbPos >= rFcePos) {
							Logger.Error(sentence);
							continue;
						}
						result = _SrcMethodCall(srcRight);
					} else {
						// expression

						// =====================
						// temp todo.....
						result = new ScriptValue(srcRight);
						// =====================
					}

					var varBeginPos = srcLeft.IndexOf(Grammar.VAR);
					if (varBeginPos != -1) {
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
					_SrcMethodCall(sentence);
				}
			}
			return null;
		}

		private ScriptValue _SrcMethodCall(string sentence) {
			var fpbPos = sentence.IndexOf(Grammar.FPB);
			var fpePos = sentence.IndexOf(Grammar.FPE);
			if (fpbPos == -1 || fpbPos >= fpePos) {
				Logger.Error(sentence);
				return null;
			}
			var methodName = sentence.Substring(0, fpbPos).Trim();
			if (methodName.IndexOfAny(Grammar.SPECIAL_CHAR) != -1) {
				Logger.Error(sentence);
				return null;
			}
			var scriptParam = new List<ScriptValue>();
			var methodArgs = sentence.Substring(fpbPos + 1, fpePos - fpbPos - 1).Trim();
			if (!string.IsNullOrEmpty(methodArgs)) {
				var argArr = methodArgs.Split(Grammar.FPS);
				for (int i = 0; i < argArr.Length; ++i) {
					var argStr = argArr[i].Trim();
					if (string.IsNullOrEmpty(argStr)) {
						Logger.Log(sentence);
						break;
					}
					ScriptValue scriptValue = null;
					var obj = FindObject(argStr);
					if (obj != null) {
						scriptValue = obj.GetValue();
					} else {
						scriptValue = new ScriptValue(argStr);
					}
					scriptParam.Add(scriptValue);
				}
			}

			var method = FindMethod(methodName);
			if (method == null) {
				if (MethodLibrary.Container(methodName)) {
					return MethodLibrary.Execute(methodName, scriptParam);
				} else {
					Logger.Error(sentence);
					return null;
				}
			}
			return method.Execute(scriptParam);
		}

		private int _ReadNextFCE(string src, int start) {
			if (start >= src.Length) { return -1; }
			int findFCB = 0;
			for (int i = start; i < src.Length; ++i) {
				char ch = src[i];
				if (ch == Grammar.FCB) {
					++findFCB;
				} else if (ch == Grammar.FCE) {
					--findFCB;
				}
				if (findFCB == -1) {
					return i;
				}
			}
			return -1;
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
