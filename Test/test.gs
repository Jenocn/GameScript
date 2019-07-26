// GameScript

main() {

	max(a, b) {
		var result = b;
		if (a > b) {
			result = a;
		}
		return result;
	}

	var ret = max(5, 9);
	print(ret);
}

main();