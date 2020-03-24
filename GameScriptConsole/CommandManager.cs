using System.Collections.Generic;
using System.IO;

public static class CommandManager {

	public enum Command {
		None = 0,
		Clear, // 清空控制台
		Quit, // 退出
		ShowSrc, // 显示当前源码
		Space, // 显示当前所有空间
		Execute, // 执行
		Reset, // 重置环境
		OpenFile, // 打开文件
		Change, // 改变当前脚本空间
		Remove, // 删除空间
		New, // 新建项目
	}

	public static readonly string ConsoleVersion = "1.2.0 beta";
	public static readonly string GSDesc = "GameScript " + gs.config.Config.Version;
	public static readonly string ConsoleDesc = "Console " + ConsoleVersion;

	private static Dictionary<string, Command> CMDS_NO_PARAMS = new Dictionary<string, Command>();
	private static Dictionary<string, Command> CMDS_PARAMS = new Dictionary<string, Command>();

	private static Dictionary<string, LineExecuter> _lineExecuterDict = new Dictionary<string, LineExecuter>();
	private static string _lineExecuterCurName = "default";

	private static bool _bStop = false;

	static CommandManager() {
		CMDS_NO_PARAMS.Add("cls", Command.Clear);
		CMDS_NO_PARAMS.Add("clear", Command.Clear);
		CMDS_NO_PARAMS.Add("quit", Command.Quit);
		CMDS_NO_PARAMS.Add("src", Command.ShowSrc);
		CMDS_NO_PARAMS.Add("reset", Command.Reset);
		CMDS_NO_PARAMS.Add("space", Command.Space);
		CMDS_NO_PARAMS.Add("execute", Command.Execute);
		CMDS_NO_PARAMS.Add("exec", Command.Execute);

		CMDS_PARAMS.Add("open", Command.OpenFile);
		CMDS_PARAMS.Add("change", Command.Change);
		CMDS_PARAMS.Add("remove", Command.Remove);
		CMDS_PARAMS.Add("new", Command.New);
	}

	public static void Run(params string[] args) {

		gs.VM.AddModule(new std.StandardModule());

		CommandManager.WriteTitleLine();
		if (args.Length > 0) {
			CommandManager.ExecuteFile(args[0]);
		}

		ExecuteChange("");

		while (true) {
			if (CommandManager.IsStop()) {
				break;
			}
			CommandManager.Execute();
		}
	}

	public static void Execute() {
		GSConsole.Write(">");
		var line = GSConsole.ReadLine().Trim();

		if (CMDS_NO_PARAMS.ContainsKey(line)) {
			ExecuteCommand(CMDS_NO_PARAMS[line]);
			return;
		}
		foreach (var item in CMDS_PARAMS) {
			var argStrArr = line.Split(new char[] { ' ', '\t' }, System.StringSplitOptions.RemoveEmptyEntries);
			if (argStrArr.Length < 2) {
				continue;
			}
			if (item.Key != argStrArr[0].Trim()) {
				continue;
			}
			var args = new string[argStrArr.Length - 1];
			for (int i = 1; i < argStrArr.Length; ++i) {
				args[i - 1] = argStrArr[i];
			}
			ExecuteCommand(item.Value, args);
			return;
		}
		ExecuteLine(line);
	}

	public static bool ExecuteCommand(Command command, params string[] args) {
		switch (command) {
		case Command.Clear:
			GSConsole.Clear();
			_GetCurExecuter().Clear();
			WriteTitleLine();
			break;
		case Command.Quit:
			_bStop = true;
			break;
		case Command.ShowSrc:
			var src = _GetCurExecuter().GetSrc();
			if (!string.IsNullOrEmpty(src)) {
				GSConsole.Write(src);
				if (src[src.Length - 1] != '\n') {
					GSConsole.WriteLine();
				}
			}
			break;
		case Command.Space:
			foreach (var item in _lineExecuterDict) {
				bool bSelected = item.Key == _lineExecuterCurName;
				GSConsole.WriteLine(item.Key + (bSelected ? " (cur)" : ""));
			}
			break;
		case Command.Execute:
			ExecuteSrc();
			break;
		case Command.Reset:
			GSConsole.Clear();
			_RemoveAllExecuter();
			WriteTitleLine();
			break;
		case Command.OpenFile:
			if (args.Length == 0) {
				return false;
			}
			ExecuteFile(args[0]);
			break;
		case Command.Change:
			if (args.Length == 0) {
				return false;
			}
			ExecuteChange(args[0]);
			break;
		case Command.Remove:
			ExecuteRemoveSpace(args);
			break;
		case Command.New:
			ExecuteNewProject(args);
			break;
		default:
			return false;
		}
		return true;
	}

	public static void ExecuteLine(string src) {
		var result = _GetCurExecuter().ExecuteLineOrSaveLine(src);
		if (!string.IsNullOrEmpty(result)) {
			GSConsole.WriteLine(result.ToString());
		}
	}

	public static void ExecuteSrc() {
		_GetCurExecuter().ExecuteSrc();
	}

	public static void ExecuteFile(string path) {
		var fileExecuter = new FileExecuter();
		fileExecuter.Execute(path);
	}

	public static void ExecuteChange(string name) {
		if (string.IsNullOrEmpty(name)) {
			name = "default";
		}
		_lineExecuterCurName = name;
		_GetExecuter(name);
	}

	public static void ExecuteRemoveSpace(params string[] name) {
		if (name.Length == 0) {
			_RemoveExecuter(_lineExecuterCurName);
			ExecuteChange("");
		} else {
			bool bCur = false;
			foreach (var item in name) {
				_RemoveExecuter(item);
				if (item == _lineExecuterCurName) {
					bCur = true;
				}
			}
			if (bCur) {
				ExecuteChange("");
			}
		}
	}

	public static void ExecuteNewProject(params string[] args) {
		string path = "";
		if (args.Length > 0) {
			path = args[0].Trim();
		}

		if (!Directory.Exists(path)) {
			Directory.CreateDirectory(path);
		} else {
			GSConsole.WriteLine("[NO] Project: " + path + " is exists!");
			return;
		}

		string name = path;
		var chPos = path.LastIndexOfAny(new char[] { '/', '\\' });
		if (chPos != -1) {
			name = path.Substring(chPos + 1).Trim();
		}

		File.Copy("GameScriptApplication.exe", path + "/" + name + ".exe");
		File.Copy("sources.conf", path + "/sources.conf");
		string templateSrc = "main() {\n\tWelcome() {\n\t\tprint(\"Welcome to GameScript!\");\n\t}\n\tWelcome();\n}\nmain();";
		File.WriteAllText(path + "/main.gs", templateSrc);

		GSConsole.WriteLine("[YES] Project: " + path + " is created!");
	}

	public static bool IsStop() {
		return _bStop;
	}

	public static void WriteTitleLine() {
		GSConsole.WriteLine("---------------------------");
		GSConsole.WriteLine(GSDesc);
		GSConsole.WriteLine(ConsoleDesc);
		GSConsole.WriteLine("---------------------------");
	}

	private static LineExecuter _GetCurExecuter() {
		return _GetExecuter(_lineExecuterCurName);
	}
	private static LineExecuter _GetExecuter(string name) {
		LineExecuter ret = null;
		if (!_lineExecuterDict.TryGetValue(name, out ret)) {
			ret = new LineExecuter();
			_lineExecuterDict.Add(name, ret);
		}
		return ret;
	}
	private static void _RemoveExecuter(string name) {
		_lineExecuterDict.Remove(name);
	}
	private static void _RemoveAllExecuter() {
		_lineExecuterDict.Clear();
		ExecuteChange("");
	}
}