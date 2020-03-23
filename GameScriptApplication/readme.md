## GameScriptApplication
用于创建独立应用的工程模板

### 配置  
`sources.conf`为项目配置
格式:
```
main: main.gs
list: [
  a.gs,
  b.gs,
  c.gs,
  src/d.gs,
  ...
]
```
`mian:`配置脚本入口
`list:`配置所有需要用到的脚本路径

之后在脚本代码中使用`using a;`的方式引用`a.gs`的脚本
多路径下的脚本引用使用例如:`using src.d;`


### App
在App的`Run`方法中注册模块等扩展操作