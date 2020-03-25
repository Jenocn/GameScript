## GameScript 1.3.0 beta  
(+) 增加`std`模块,目前包含`std.io`和`std.file`两个模块  
(+) 增加`ScriptUsing`和`VMUsing`类用于方便扩展新的模块  
(+) 增加`VMValue.TRUE`和`VMValue.FALSE`两个值方便代码书写  
(!) 修复在返回语句中有括号的表达式会报错的问题  
(!) 修复`ScriptValue.NULL`可能导致使用不当发生值改变的错误问题  
(!) 修复调用函数时,参数解析出错的bug  
(!) 修复字符串做`+`运算时,可能导致报错的bug  

#### GameScriptConsole 1.1.0 beta  
(+) `Console`接入`std`  

#### GameScriptApplication  
(+) 增加`GameScriptApplication`工程,作为模板,用于创建应用  

[github:https://github.com/Jenocn/GameScript](https://github.com/Jenocn/GameScript)  
