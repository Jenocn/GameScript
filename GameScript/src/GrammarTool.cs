/*
 * By Jenocn
 * https://jenocn.github.io/
 */

using System.Collections.Generic;

namespace gs.compiler.tool {
	public static class GrammarTool {

		public static List<string> SplitParams(string src) {
			var ret = new List<string>();
			if (string.IsNullOrEmpty(src)) {
				return ret;
			}
			var index = 0;
			var length = src.Length;
			while (index < length) {
				var startPos = index;
				index = length;

				// find param
				var fpsPos = src.IndexOf(Grammar.FPS, startPos);
				if (fpsPos != -1) {
					index = fpsPos;
				}

				// find method
				var fpbPos = src.IndexOf(Grammar.FPB, startPos);
				if (fpbPos != -1 && fpbPos < fpsPos) {
					var fpePos = ReadPairSignPos(src, fpbPos + 1, Grammar.FPB, Grammar.FPE);
					if (fpePos > fpsPos) {
						index = fpePos + 1;
						var methodEnd = src.IndexOf(Grammar.FPS, index);
						if (methodEnd != -1) {
							index = methodEnd;
						}
					}
				}

				var argStr = src.Substring(startPos, index - startPos).Trim();
				ret.Add(argStr);
				++index;
			}

			return ret;
		}

		public static int ReadPairSignPos(string src, int start, char left, char right, bool ignoreSS = true) {
			if (start >= src.Length) { return -1; }
			int count = 0;
			bool findSS = false;
			for (int i = start; i < src.Length; ++i) {
				char ch = src[i];
				if (ch == Grammar.SS) {
					findSS = !findSS;
				}
				if (ignoreSS && findSS) { continue; }
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

		public static int ReadSingleSignPos(string src, string sign, bool ignoreSS = true) {
			var spaceChar = new HashSet<char> { ' ', '\t', '\n' };
			int pos = 0;
			while (pos != -1) {
				var findPos = src.IndexOf(sign, pos, System.StringComparison.Ordinal);
				if (findPos == -1) {
					return -1;
				}
				pos = findPos + sign.Length;
				if (findPos > 0 && !spaceChar.Contains(src[findPos - 1])) {
					continue;
				}
				if (findPos + sign.Length < src.Length && !spaceChar.Contains(src[findPos + sign.Length])) {
					continue;
				}
				return findPos;
			}
			return -1;
		}

		public static int CountSign(string src, char sign) {
			int count = 0;
			for (int i = 0; i < src.Length; ++i) {
				if (src[i] == sign) {
					++count;
				}
			}
			return count;
		}

		public static string CutComments(string src) {
			if (string.IsNullOrEmpty(src)) { return src; }
			int pos = 0;
			string ret = "";

			while (true) {
				if (pos >= src.Length) { break; }
				var commentPos = src.IndexOf(Grammar.COMMENT, pos);
				if (commentPos != -1) {
					ret += src.Substring(pos, commentPos - pos);

					var rnPos = src.IndexOf('\n', commentPos + Grammar.COMMENT.Length);
					if (rnPos == -1) {
						rnPos = src.Length - 1;
					}
					pos = rnPos + 1;
				} else {
					ret += src.Substring(pos);
					break;
				}
			}
			return ret;
		}
	}
}