﻿/*
 * By Jenocn
 * https://jenocn.github.io/
*/

namespace gs.compiler {
	public class Grammar {

		// sentence over
		public static readonly char OVER = ';';

		// return
		public static readonly string RETURN = "return";

		// function content begin
		public static readonly char FCB = '{';
		// function content end
		public static readonly char FCE = '}';

		// function param begin
		public static readonly char FPB = '(';
		// function param end
		public static readonly char FPE = ')';
		// function param split
		public static readonly char FPS = ',';

		// string sign
		public static readonly char SS = '\"';

		// comment sign
		public static readonly string COMMENT = "//";

		// if sign
		public static readonly string IF = "if";
		public static readonly string ELSE = "else";

		// compare
		public static readonly string COMPARE_EQUIP = "==";
		public static readonly string COMPARE_LESS = "<";
		public static readonly string COMPARE_MORE = ">";
		public static readonly string COMPARE_LESS_EQUAL = "<=";
		public static readonly string COMPARE_MORE_EQUAL = ">=";

		// assign sign
		public static readonly char ASSIGN = '=';

		// var
		public static readonly string VAR = "var";

		// special char of name
		public static readonly char[] SPECIAL_CHAR = new char[] { ' ', '\n' };
	}
}