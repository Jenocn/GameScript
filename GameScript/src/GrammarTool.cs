/*
 * By Jenocn
 * https://jenocn.github.io/
*/

namespace gs.compiler.tool {
	public static class GrammarTool {
		public static int ReadPairSignPos(string src, int start, char left, char right) {
			if (start >= src.Length) { return -1; }
			int count = 0;
			for (int i = start; i < src.Length; ++i) {
				char ch = src[i];
				if (ch == left) {
					++count;
				} else if (ch == right) {
					--count;
				}
				if (count == -1) {
					return i;
				}
			}
			return -1;
		}
	}
}
