using gs;
using System.IO;

public class FileExecuter {
	public string Execute(string path) {
		if (!File.Exists(path)) {
			GSConsole.WriteLine("\"" + path + "\"" + "is not a \"GameScript\" file!");
			return "";
		}
		var src = File.ReadAllText(path);
		if (string.IsNullOrEmpty(src)) {
			return "";
		}

		var vmFun = VM.Load(src);
		var result = vmFun.Execute();
		if (result != null) {
			return result.ToString();
		}
		return "";
	}
}