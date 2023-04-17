#define CLOVE_SUITE_NAME CSuite01
#include "clove-unit.h"

CLOVE_TEST(Test01) {
	CLOVE_PASS();
}

CLOVE_TEST(Test02) {
	CLOVE_INT_GT(2, 1);
}

CLOVE_TEST(Test03) {
	CLOVE_INT_GT(1, 2);
}