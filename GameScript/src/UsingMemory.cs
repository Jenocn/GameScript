using System.Collections.Generic;

namespace gs.compiler {
	public static class UsingMemory {
		private static Dictionary<string, string> _memoryList = new Dictionary<string, string>();
		public static void Add(string name, string space) {
			if (_memoryList.ContainsKey(name)) {
			}
			_memoryList.Add(name, space);
		}

		public static void Remove(string name) {
			_memoryList.Remove(name);
		}

		public static ScriptMethod Get(string name) {
			string ret = null;
			if (_memoryList.TryGetValue(name, out ret)) {
				return new ScriptMethod(ret);
			}
			return null;
		}
	}
}