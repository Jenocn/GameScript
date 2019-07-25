using System.Collections.Generic;

namespace gs.compiler {
	public sealed class ScriptMethod {

		private string _srcHeader = "";
		private string _srcBody = "";
		private string _name = "";
		private List<string> _params = new List<string>();

		private ScriptMethod _parent = null;
		private Dictionary<string, ScriptMethod> _methods = new Dictionary<string, ScriptMethod>();
		private Dictionary<string, ScriptObject> _objects = new Dictionary<string, ScriptObject>();

		public ScriptMethod(string srcHeader, string srcBody, ScriptMethod parent = null) {
			_srcHeader = srcHeader;
			_srcBody = srcBody;
			_parent = parent;
			_ParseHeader();
			_ParseMethod();
		}

		private void _ParseHeader() {
			int fpbPos = _srcHeader.IndexOf(Grammar.FPB);
			int fpePos = _srcHeader.IndexOf(Grammar.FPE);
			if (fpbPos == -1 || fpbPos >= fpePos) {
				Logger.Error(_srcHeader);
				return;
			}
			var nameSrc = _srcHeader.Substring(0, fpbPos).Trim();
			if (string.IsNullOrEmpty(nameSrc)) {
				Logger.Error(_srcHeader);
				return;
			}
			if (nameSrc.IndexOfAny(Grammar.SPECIAL_CHAR) != -1) {
				Logger.Error(_srcHeader);
				return;
			}
			_name = nameSrc;

			var paramSrc = _srcHeader.Substring(fpbPos + 1, fpePos - fpbPos - 1).Trim();
			if (string.IsNullOrEmpty(paramSrc)) {
				Logger.Error(_srcHeader);
				return;
			}

			var paramsSrcList = paramSrc.Split(Grammar.FPS);
			foreach (var str in paramsSrcList) {
				string paramName = str.Trim();
				if (string.IsNullOrEmpty(paramName)) {
					Logger.Error(_srcHeader);
					return;
				}
				if (paramName.IndexOfAny(Grammar.SPECIAL_CHAR) != -1) {
					Logger.Error(_srcHeader);
					return;
				}
				_params.Add(paramName);
			}
		}

		private void _ParseMethod() {

		}

		public ScriptValue Execute(List<ScriptValue> args) {
			_objects.Clear();

			for (int i = 0; i < _params.Count; ++i) {
				string paramName = _params[i];
				ScriptValue paramValue = null;
				if (i < args.Count) {
					paramValue = args[i];
				}
				_objects.Add(paramName, new ScriptObject(paramName, paramValue));
			}

			return null;
		}

		public ScriptMethod FindMethod(string name) {
			ScriptMethod ret = null;
			if (_methods.TryGetValue(name, out ret)) {
				if (_parent != null) {
					return _parent.FindMethod(name);
				}
			}
			return ret;
		}
	}
}
