using CLZ.BLLService;
using CLZ.Model;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLZ.NUnitTest
{
    [TestFixture]

    public class LifeCycleContractFixture
    {


        [TestFixtureSetUp]

        public void FixtureSetUp()
        {

            Console.Out.WriteLine("FixtureSetUp");

        }



        [TestFixtureTearDown]

        public void FixtureTearDown()
        {

            Console.Out.WriteLine("FixtureTearDown");

        }



        [SetUp]

        public void SetUp()
        {

            Console.Out.WriteLine("SetUp");

        }



        [TearDown]

        public void TearDown()
        {

            Console.Out.WriteLine("TearDown");

        }



        [Test]

        public void Test1()
        {
            int a = 1;
            int b = 2;
            Assert.AreEqual(a, b);
            Console.Out.WriteLine("Test 1");

        }



        [Test]

        public void Test2()
        {

            Console.Out.WriteLine("Test 2");

        }

        [Test]
        public void getAllSamplesTest()
        {
            try
            {
                ICommonService ics = new CommonService();
                List<SysSample> sampleList = ics.getAllSamples();
                int expectedCount = 16;
                int actualCount = sampleList.Count;
                Assert.AreEqual(expectedCount, actualCount);
            }
            catch (Exception ex)
            {
                Console.Out.WriteLine(ex.ToString());
                throw ex;
            }

        }


        [Test]
        public void TestDemo() 
        {
            try
            {
                ICommonService ics = new CommonService();
                int num = 0;
                string actualResult = ics.test(num);
                string expectedResult = "白日依山尽";
                Assert.AreEqual(expectedResult, actualResult);
            }
            catch (Exception ex)
            {
                Console.Out.WriteLine(ex.ToString());
                throw ex;
            }
        }
    }


}
