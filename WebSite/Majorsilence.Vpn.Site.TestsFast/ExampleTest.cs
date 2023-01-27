using System;
using NUnit.Framework;

namespace SiteTests;

public class ExampleTest
{
    public ExampleTest()
    {
        Console.WriteLine("Constructor called");
    }

    [SetUp]
    public void Setup()
    {
        Console.WriteLine("Setup called");
    }

    [TearDown]
    public void TearDown()
    {
        Console.WriteLine("TearDown called");
    }

    [Test()]
    public void Test1()
    {
        Console.WriteLine("Test 1 called");
    }

    [Test()]
    public void Test2()
    {
        Console.WriteLine("Test 2 called");
    }

    [Test()]
    public void Test3()
    {
        Console.WriteLine("Test 3 called");
    }
}