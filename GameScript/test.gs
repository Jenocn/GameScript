main() {

	var str = "";
	str = "Hello World!";

	print(str);

	str = "1234";

	func1() {
		str = "Hi";
	}

	func1();

	print(str);
}

main();