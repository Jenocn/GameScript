namespace gs.compiler {
	public class Grammar {

		public static readonly char OVER = ';';

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

		// assign sign
		public static readonly char ASSIGN = '=';

		// var
		public static readonly string VAR = "var";

		public static readonly char[] SPECIAL_CHAR = new char[] { ' ', '\n' };
	}
}