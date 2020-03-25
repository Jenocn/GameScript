/*
 * By Jenocn
 * https://jenocn.github.io/
*/

using System;
using gs.compiler;

namespace gs {
	public static class VM {

		/// <summary>
		/// 加载脚本
		/// </summary>
		/// <param name="src">脚本源码</param>
		/// <param name="parent">父空间,默认为空</param>
		/// <returns>可执行函数对象VMFunction</returns>
		public static VMFunction Load(string src, VMFunction parent = null) {
			return new VMFunction(src, parent);
		}

		/// <summary>
		/// 重设日志器
		/// </summary>
		/// <param name="logCall">日志回调函数参数:(消息, 是否为错误信息)</param>
		public static void ResetLogger(Action<string, bool> logCall = null) {
			Logger.SetLoggerFunc(logCall);
		}

		/// <summary>
		/// 添加新的引用
		/// <para>脚本中使用using或new关键词引用</para>
		/// </summary>
		/// <param name="name">引用名称</param>
		/// <param name="src">引用的脚本源码</param>
		public static void AddUsing(string name, string src) {
			UsingMemory.Add(name, ScriptUsing.Create(src));
		}

		/// <summary>
		/// 添加新的引用
		/// <para>脚本中使用using或new关键词引用</para>
		/// </summary>
		/// <param name="name">引用名称</param>
		/// <param name="value">引用类</param>
		public static void AddUsing<T>(string name, VMUsing<T> value) where T : VMUsing<T>, new() {
			UsingMemory.Add(name, value);
		}

		/// <summary>
		/// 添加模块
		/// </summary>
		/// <param name="module">模块</param>
		public static void AddModule(VMModuleBase module) {
			module.OnModuleLoad();
		}
	}
}