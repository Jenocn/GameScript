// GameScript

compare_more(a, b) {
	return a > b;
}

max(a, b) {
	if (a > b) {
		return a;
	}
	return b;
}

var v = compare_more(10, 15);

print(v);

var f = max(10, 9);
print(f);

f = max(100, 101);
print(f);

