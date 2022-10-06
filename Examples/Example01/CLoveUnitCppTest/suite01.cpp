#define CLOVE_SUITE_NAME Suite01
#include "clove-unit.h"

CLOVE_TEST(Test01) {
	CLOVE_PASS();
}

CLOVE_TEST(Test02) {
	CLOVE_FAIL();
}

CLOVE_TEST(Test03) {
	//skipped
}

CLOVE_TEST(Test04) {
	puts("Hello Line1");
	puts("Hello Line2");
	puts("Hello Line3");
	CLOVE_PASS();
}

CLOVE_TEST(Test05) {
	int a = 1;
	int b = 2;
	int sum = a + b;
	CLOVE_INT_EQ(3, sum);
}


