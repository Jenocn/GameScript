// 定义一个变量,变量必须要赋初值
var value1 = null;
var value2 = 10;
var value3 = "HelloWorld";
var value4 = true;
var value5 = false;

// 赋值
value1 = 1234;
value1 = "string";

// 表达式 
var num1 = 100 * (5 + 3);
var num2 = num1 / 100;

// 字符串拼接
var str1 = "abc" + "123";
var str2 = str1 + " yes" + " no " + str1;

// 内置输出方法
print(str2);

// 求字符串长度的方法
var length = strlen(str2);
print(length);

// 条件语句
if (num2 > 1000) {
    print("1000");
} else if (num2 > 100) {
    print("100");
} else {
    print("no");
}

// 循环语句
var x = 3;
while(x > 0) {
    print(x);
    x = x - 1;
}

// 键值对变量
var pair[0] = "pair 0";
var pair[1] = "pair 1";
var pair[2] = "pair 2";
var keyIndex = 0;
while (keyIndex < 3) {
    print(pair[keyIndex]);
    keyIndex = keyIndex + 1;
}

// 定义一个无参的函数
Method0() {
    // print为内置方法
    print("Hello World");
}

// 带一个参数的函数
Method1(message) {
    print(message);
}

// 函数的调用 
Method0(); // print "Hello World"
Method1("Hello Method1");
Method1(100 * 5);

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