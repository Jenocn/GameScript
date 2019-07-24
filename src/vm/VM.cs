namespace gs {
	public class VM {
		public void Execute(string src) {
			var trans = gs.Compiler.Translate(src);
			_Script(trans);
		}

		private void _Script(string trans) {
			
		}
	}
}