/*
 * By Jenocn
 * https://jenocn.github.io/
 */

using System.Collections.Generic;

namespace gs.compiler {
	public static class UsingMemory {
		private static Dictionary<string, ScriptUsing> _memoryList = new Dictionary<string, ScriptUsing>();

		public static void Add(string name, ScriptUsing scriptUsing) {
			if (_memoryList.ContainsKey(name)) {
				return;
			}
			_memoryList.Add(name, scriptUsing);
		}

		public static void Remove(string name) {
			_memoryList.Remove(name);
		}

		public static ScriptUsing Get(string name) {
			ScriptUsing ret = null;
			if (_memoryList.TryGetValue(name, out ret)) {
				return ret;
			}
			return null;
		}
	}
}