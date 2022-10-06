namespace CLoveUnitTestAdapter.Core
{
    public static class CloveConfig
    {
        public const string TestExecutorUri = "executor://CLoveUnitTestExecutor";
        public const string TestFileExtension = ".exe";
        public static XVersion SupportedCloveVersion = new XVersion(2, 3, 0);
        public const string BinaryMagicStringRaw = "https://github.com/fdefelici/clove-unit";
        //public const string DotVsDirName = ".vs";
        public const string CacheBaseDirName  = ".cloveunit";
        public const string CacheFileNameSuffix = ".cloveunit";
        public const string CacheProp_IsCloveBinary = "is_cloveunit_binary";
        public const string CacheProp_DiscoveredTests = "discovered_tests";

        public const string RunTestsFileNameSuffix = "_runtests.json";
    }
}
