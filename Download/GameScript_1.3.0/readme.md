## GameScript 1.3.0 beta  
(+) 增加array语法,用于创建一组对象,`array[size]`用于创建一个size大小的数组 or `array[start, end]`用于创建从start到end(包括end)的数组  

(+) 新增using语法`using space`,用于引用预先加载的其他文件,预先加载使用`gs.VM.Using()`方法,using语法会引用空间,所以空间对象的状态时公用的    

(+) 新增new语法`new space`,用于从预先加载文件中拷贝一份新的空间,区别于using语法的是,new语法会创建一份新的空间,空间对象的状态是独立的  

(!) 修复函数多次执行时因没有清空上一次执行的临时数据而导致报错的问题      

(!) 修复空间重名会导致出错的问题,规则改为空间重名时,使用就近原则  

---

## GameScriptConsole 1.1.0 beta
(+) 增加space命令,用于查看当前创建并且存在的空间,现在默认为default空间   

(+) 增加change命令,用于创建/切换空间作用域,空间作用域可以在控制台中同时管理多份src   

(+) 增加clear命令,与cls命令功能相同  

(+) 增加remove命令,用于删除指定空间  

(+) 增加reset命令,删除所有空间并清理控制台  

[github:https://github.com/Jenocn/GameScript](https://github.com/Jenocn/GameScript)  
