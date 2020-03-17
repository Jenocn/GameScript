using System;

public static class GSConsole {
	public static void WriteLine() {
		Console.WriteLine();
	}
	public static void WriteLine(object value) {
		Console.WriteLine(value);
	}
	public static void Write(object value) {
		Console.Write(value);
	}
	public static string ReadLine() {
		return Console.ReadLine();
	}
	public static void Clear() {
		Console.Clear();
	}
}