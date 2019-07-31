# GameScript

## 介绍 
- 函数式结构 
- 基于.net开发,跨平台,支持Unity 
- 嵌入式,接入方便,简单易用
- 类C语法 

## 函数式结构 
任何被执行的代码片段都看作为一个函数调用,因此一个脚本文件为一个函数,函数内可以嵌套定义函数,一个函数可看作为一个空间作用域,子作用域可以调用父作用域中的方法和变量

## 数据类型 
GameScript是动态类型,但本质上会根据值来动态确定类型
- null
- number 
- string
- bool

## 代码 
```csharp
// 定义一个变量
var value1; // 没有赋值,默认为null
var value2 = 10;
var value3 = "HelloWorld";
var value4 = true;
var value5 = false;
var value6 = null;

// 赋值
value1 = 1234;
value1 = "string";

// 表达式 
var num1 = 100 * (5 + 3);
var num2 = num1 / 100;

// 定义一个无参的函数
Method0() {
	// print为内置方法
	print("Hello World");
}

// 函数的调用 
Method0(); // print "Hello World"

// 带一个参数的函数
Method1(message) {
	print(message);
}

// 带返回值的函数
// 如果不写return语句,则默认返回null
Method2() {
	return 0;
}

// 实现一个返回最大数的函数
Max(a, b) {
	if (a >= b) {
		return a;
	}
	return b;
}

// 函数空间 
SpaceMethod1() {
	SpaceMethod2() {
		SpaceMethod3() {
			// ...
			// 可调用所有上层空间内的变量和方法
		}
	}
}
```

## 注册C#方法
目前可以通过调用`gs.VM.RegisterMethod`来注册,需要使用到`gs.compiler.ScriptValue`这个类型,接下来准备将再封装一层,不再需要外部调用`gs.compiler`空间下的类,使得注册C#方法更加简单方便
```csharp
/// <summary>
/// 注册C#方法的方法 RegisterMethod
/// </summary>
/// <param name="name">方法名</param>
/// <param name="func">回调</param>
public static bool RegisterMethod(string name, System.Func<List<ScriptValue>, ScriptValue> func);
```

```csharp
// 在C#中
// 例如注册一个求和方法 sum
gs.VM.RegisterMethod("sum", (List<ScriptValue> param) => {
    // 返回值为 ScriptValue 类型
    var ret = new ScriptValue(0);
    // 参数数组需要判空
    if (param == null) { return ret; }
    double value = 0;
    // for循环所有参数,表示支持无限参数
    foreach (var item in param) {
        // 获取参数类型是否为Number类型
        if (item.GetValueType() != ScriptValueType.Number) { continue; }
        // 取值相加
        var sv = (double)item.GetValue();
        value += sv;
    }
    // 设置结果
    ret.SetValue(value);
    // 返回
    return ret;
});
```

```csharp
// 在 GameScript 脚本中就可以使用了
var value = sum(10, 20, 30);
print(value);
```

## 调用脚本 
新建一个文本文件,例如 main.gs
```csharp
// C#代码中
// 读取脚本的全部内容
var src = File.ReadAllText("main.gs");
// Execute则执行整个脚本,返回值为整个脚本的return返回值,与方法类型
// 整个脚本可以看做是一个无参的匿名方法
gs.VM.Execute(src);
```

