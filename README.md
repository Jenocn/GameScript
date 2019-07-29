# GameScript

## 语法 
我很喜欢类C语法,看起来很舒服,所以GameScript使用类C语法

新增:经过周六一天的奋斗,对代码整体进行了修改,然后增加了算术表达式的支持

```csharp
// 变量和赋值
// 就跟C#一样,只不过GameScript中类型是动态的
var value = 10; // 定义一个value变量,赋值为10,类型为Number
value = "hello"; // 修改value的值,类型变为String
```

```csharp
// 方法
// 声明一个方法,无参或带N个参数,参数无类型限制
MethodName(arg1, arg2, argN) {
    return null; // 可以不写return,默认返回null
}
// 调用
MethodName();

// 方法支持嵌套,嵌套方法作用域仅限于方法内
MethodName() {
    func1() {
        test1() {
            // todo...
        }

        test1(); // call
    }
    func2() {
        // todo...
    }
    func2(); // call
}
```

```csharp
// 条件语句
var value1 = 10;
var value2 = 15;
if (value1 == value2) {
    // todo...
}
```

```csharp
// 内置的print方法,默认情况下只会在C#控制台输出
print("Hello");
```

## 注册C#方法
**`MethodLibrary 类`**
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
        // 具体参数也需要判空
        if (item == null) { continue; }
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
