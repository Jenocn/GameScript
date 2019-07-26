main() {

	var str = "Hello World!";

	func1() {
		print(str);
	}

	func2() {
		print("2");
	}

	func1();
	func2();
}

main();

print(str); // error