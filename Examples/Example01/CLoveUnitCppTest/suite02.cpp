#define CLOVE_SUITE_NAME Suite02
#include "clove-unit.h"

#ifdef _WIN32
#include <windows.h>
#define sleep_secs(sec) Sleep(sec * 1000)
#else
#include <unistd.h>
#define sleep_secs(sec) sleep(sec)
#endif
CLOVE_TEST(TimeElapsedAfterSleep1Second) {
    sleep_secs(1);
    CLOVE_PASS();
}


CLOVE_TEST(FailingStringEqual) {
    CLOVE_STRING_EQ("hello", "world");
}


CLOVE_TEST(FailingStringNotEqual) {
    CLOVE_STRING_NE("hello", "hello");
}


CLOVE_TEST(FailingStringEncodedEqual) {
    CLOVE_STRING_EQ("", "abc\ndef \"ghi\"?");
}

CLOVE_TEST(EqualInt) {
    CLOVE_INT_EQ(10, 10);
}