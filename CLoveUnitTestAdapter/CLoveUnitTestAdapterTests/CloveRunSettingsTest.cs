using CLoveUnitTestAdapter.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace CLoveUnitTestAdapterTests
{
    [TestClass]
    public class CloveRunSettingsTest
    {
        [TestMethod]
        public void Simple()
        {
            CloveRunSettings sets = CloveRunSettings.From(SETTINGS_01, null, new NullXLogger());
            Assert.IsNotNull(sets);

            Assert.AreEqual("Res\\Base\\Path\\.cloveunit", sets.CachePath);
            Assert.AreEqual(true, sets.AdapterEnabled);
            Assert.AreEqual(true, sets.DebugEnabled);
            Assert.AreEqual(".+Test", sets.ExecNameRegex.ToString());
        }

        //Note: RunConfiguration/SolutionDirectory is added by default by the Test Framework
        //Note2: Looking at official doc, SolutionDirectory seems to NOT EXISTS....
        //       Instead is declared to exists ResultsDirectory (but is declared to be relative path to .runsettings, instead during MSTest execution, return an absolute path)
        //       https://learn.microsoft.com/en-us/visualstudio/test/configure-unit-tests-by-using-a-dot-runsettings-file?view=vs-2022
        private const string SETTINGS_01 = @"<?xml version=""1.0"" encoding=""utf-8""?>
    <RunSettings>
      <RunConfiguration>
        <SolutionDirectory>Sln\Base\Path</SolutionDirectory>
        <ResultsDirectory>Res\Base\Path</ResultsDirectory>
      </RunConfiguration>
      
      <CLoveUnit>
        <AdapterEnabled>true</AdapterEnabled>
        <DebugEnabled>true</DebugEnabled>
        <ExecNameRegex>.+Test</ExecNameRegex>
      </CLoveUnit>
    </RunSettings>";
    }
}
