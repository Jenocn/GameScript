using System.Collections.Generic;

namespace gs.compiler {
	public static class UsingMemory {
		private static Dictionary<string, ScriptMethod> _memoryList = new Dictionary<string, ScriptMethod>();
		public static void Add(string name, ScriptMethod space) {
			if (_memoryList.ContainsKey(name)) {
			}
			_memoryList.Add(name, space);
		}

		public static void Remove(string name) {
			_memoryList.Remove(name);
		}

		public static ScriptMethod Get(string name) {
			ScriptMethod ret = null;
			if (_memoryList.TryGetValue(name, out ret)) {
				return ret;
			}
			return null;
		}
	}
}