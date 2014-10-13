﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Infrastructure.Model;
using Infrastructure;


namespace UnitTest
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            var fo = new FileOperation();
            var output = fo.ParseToXml(@"(?<http>\w+):\/\/(?<dbpedia>[\w@][\w.:@]+)\/?[\w\.?=%&=\-@/$,]*", @"C:\D\Skola\STUBA\2roc\VI\Project\\DBpediaSearcher\Files\sample.txt");
            Assert.IsTrue(!string.IsNullOrEmpty(output));
        }
    }
}
