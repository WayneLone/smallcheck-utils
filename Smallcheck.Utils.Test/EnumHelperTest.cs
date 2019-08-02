using Microsoft.VisualStudio.TestTools.UnitTesting;
using Smallcheck.Utils.Common;
using Smallcheck.Utils.Test.Models;
using System.Collections.Generic;

namespace Smallcheck.Utils.Test
{
    /// <summary>
    /// 枚举测试
    /// </summary>
    [TestClass]
    public class EnumHelperTest
    {
        [TestMethod]
        public void TestEnumDescription()
        {
            string result = EnumHelper.GetDescription(Permission.Create);
            Assert.AreEqual("创建", result);
        }

        [TestMethod]
        public void TestEnumDic()
        {
            Dictionary<string, int> expectedDic = new Dictionary<string, int>
            {
                { "查询", 1 },
                { "创建", 2 },
                { "修改", 4 },
                { "删除", 8 },
            };
            Dictionary<string, int> resultDic = EnumHelper.GetEnumDic(typeof(Permission));
            CollectionAssert.AreEqual(expectedDic, resultDic);
        }

        [TestMethod]
        public void TestFlagEnum()
        {
            Dictionary<int, string> expectedDic = new Dictionary<int, string>
            {
                { 1, "查询" },
                { 2, "创建" },
                { 4, "修改" }
            };
            Dictionary<int, string> resultDic = EnumHelper.GetFlagEnumDic((Permission)7);
            CollectionAssert.AreEqual(expectedDic, resultDic);
        }
    }
}
