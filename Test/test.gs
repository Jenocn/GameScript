var value1 = null;
var value2 = 10;
var value3 = "HelloWorld";
var value4 = true;
var value5 = false;

value1 = 1234;
value1 = "string";

var num1 = 100 * (5 + 3);
var num2 = num1 / 100;

Method0() {
    print("Hello World");
}

Method1(message) {
    print(message);
}

Method0();
Method1("Hello Method1");
Method1(100 * 5);

Method2() {
    return 0;
}

Max(a, b) {
    if (a >= b) {
        return a;
    }
    return b;
}

SpaceMethod1() {
    SpaceMethod2() {
        SpaceMethod3() {
        }
    }
}