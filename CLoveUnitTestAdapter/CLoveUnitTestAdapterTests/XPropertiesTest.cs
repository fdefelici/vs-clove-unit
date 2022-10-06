using Microsoft.VisualStudio.TestTools.UnitTesting;
using CLoveUnitTestAdapter.Core;

namespace CLoveUnitTestAdapterTests
{
    [TestClass]
    public class XPropertiesTest
    {
        [TestMethod]
        public void BoolPropConversion()
        {
            XProperties props = new XProperties();
            props.Set("mybool", true);

            Assert.AreEqual("True", props.Get("mybool", null));
            Assert.AreEqual(true,   props.Get("mybool", false));
        }

        [TestMethod]
        public void SettingTwiceSamePropOverrideValue()
        {
            XProperties props = new XProperties();
            props.Set("myint", 10);
            props.Set("myint", 5);

            Assert.AreEqual(1, props.Count);
            Assert.AreEqual(5, props.Get("myint", 0));
        }
    }
}
