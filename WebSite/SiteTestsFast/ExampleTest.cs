using System;
using NUnit.Framework;

namespace SiteTests
{
    public class ExampleTest
    {
        public ExampleTest()
        {
            System.Console.WriteLine("Constructor called");
        }

        [SetUp]
        public void Setup()
        {
            System.Console.WriteLine("Setup called");
        }

        [TearDown]
        public void TearDown()
        {
            System.Console.WriteLine("TearDown called");
        }

        [Test()]
        public void Test1()
        {
            System.Console.WriteLine("Test 1 called");
        }

        [Test()]
        public void Test2()
        {
            System.Console.WriteLine("Test 2 called");
        }

        [Test()]
        public void Test3()
        {
            System.Console.WriteLine("Test 3 called");
        }

    }
}

