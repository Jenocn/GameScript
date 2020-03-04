using gs;

public class FileExecuter {
	public string Execute(string src) {
		var vmFun = VM.Load(src);
		var result = vmFun.Execute();
		if (result != null) {
			return result.ToString();
		}
		return "";
	}
}