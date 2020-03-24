# GameScriptConsole  

## 表达式执行  
直接输入数学或逻辑表达式运算  

## GameScript代码缓存  
非表达式和非命令的语句被当做一般的脚本代码缓存在当前空间下,待`exec`命令调用时会执行,使用`src`命令也可以显示当前缓存区的所有代码  

## Command命令  
#### `open filename`  
执行脚本文件,`filename`文件名  

#### `clear`,`cls`  
清空当前屏幕和当前空间缓存代码  

#### `src`  
显示当前缓存的脚本代码  

#### `execute`,`exec`  
执行缓存区的代码  

#### `new name`  
创建项目  

#### `change name`  
切换到新的代码空间(分支),没有则新建,`name`参数为名称

#### `remove name`  
删除分支  

#### `space`  
显示所有分支  

#### `quit`  
退出控制台  

#### `reset`  
初始化整个控制台  

## 拖入文件执行脚本  
将GameScript脚本文件直接拖到控制台exe上可以直接运行  