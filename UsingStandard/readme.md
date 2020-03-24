

# UsingStandard API  

--

## `std.console` 控制台操作模块

#### `console.input(...)`  
功能:控制台获取用户输入
参数:支持无参/多参,参数为打印的提示信息
返回:用户输入的字符串

#### `console.inputNum(...)`
功能:`console.input`的数值版本,只能获取数值类型的输入
参数:同`console.input`
返回:用户输入的数值

#### `console.log(...)`
功能:打印信息
参数:多参支持,以行为单位打印

#### `console.clear()`
功能:清空控制台屏幕

---

## `std.file` 文件操作模块

#### `file.readText(file)`
功能:从文件读取文本内容
参数:`file`文件名
返回:返回文件的所有文本内容,失败则返回null

#### `file.writeText(path, text)`
功能:写入文本内容到文件中
参数:`path`文件路径, `text`为写入内容

#### `file.copy(file1, file2)`
功能:将`file1`的文件拷贝到`file2`中
返回:是否成功

#### `file.move(file1, file2)`
功能:将`file1`的文件移动到`file2`中
返回:是否成功

#### `file.delete(file1)`
功能:删除文件`file1`
返回:是否成功

#### `file.exists(file1)`
功能:判断`file1`文件是否存在
返回:是否存在

#### `file.list(path = "./", searchPattern = "*", bDeep = false)`
功能:遍历目录下的文件
参数:
  `path`指定目录,默认为"./"当前目录  
  `searchPattern`遍历的查询语句,默认为"*"所有文件  
  `bDeep`是否深度搜索,默认为false  
返回:包含所有查询到的文件名列表  

---

### `std.list` 列表操作模块

#### `list.get(list, index)`
功能:获取`list`列表第`index`的值

#### `list.find(list, value)`
功能:从`list`列表中查询`value`并返回下标

#### `list.contains(list, value)`
功能:从`list`列表中查询`value`是否存在

#### `list.add(list, value...)`
功能:从`list`列表末尾添加`value`
参数:支持多参数,表示同时插入多个数据

#### `list.remove(list, value...)`
功能:从`list`列表中删除所有`value`对应的项
参数:支持多参数,表示删除多个

#### `list.removeAt(list, index)`
功能:从`list`列表中删除`index`位置的项

#### `list.clear(list...)`
功能:清空指定列表
参数:支持多参数

---

## `std.module` 脚本module操作模块

#### `module.addUsing(name, src)`
功能:新增一个引用using,可通过using/new来引用空间
参数:
`name`:引用的名称
`src`:脚本代码
返回:是否增加成功

---

## `std.string` 字符串操作模块

#### `string.split(str, separator)`
功能:分割`str`字符串
参数:
`str`:待分割的字符串
`separator`:分割界限符号
返回:切割后的字符串列表

#### `string.substr(str, start, length)`
功能:切割字符串
参数:
`str`:待切割字符串
`start`:切割起始位置
`length`:切割长度
返回:切割后的字符串

#### `string.replace(str, src, dest)`
功能:替换字符串中指定的内容
参数:
`str`:目标字符串
`src`:要替换的内容
`dest`:替换的新内容
返回:替换后的字符串

#### `string.contains(str, con)`
功能:查找字符串中是否包含`con`

#### `string.trim(str)`
功能:返回一个排除首尾空格的字符串



