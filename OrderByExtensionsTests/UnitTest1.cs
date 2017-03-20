using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OrderByExtensions;
using static OrderByExtensions.QueryableExtensions;
using System.Reflection;
using System.Linq.Expressions;

namespace OrderByExtensionsText
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void EnumerableTestMethod1()
        {
            var x = new List<MyClass> {
                new MyClass{ Prop1 = "C", Prop2 = "F"},
                new MyClass{ Prop1 = "C", Prop2 = "G"},
                new MyClass{ Prop1 = "C", Prop2 = "H"},
                new MyClass{ Prop1 = "B", Prop2 = "A"},
                new MyClass{ Prop1 = "A", Prop2 = "A"},
            };

            var results = new[]{
                x.OrderBy("Prop1", "Prop2 desc").ToArray(),
                x.OrderByDescending("Prop1 asc", "Prop2 desc").ToArray(),
                x.OrderBy("Prop1").ThenBy("Prop2 desc").ToArray(),
                x.OrderBy("Prop1, Prop2 desc").ToArray(),
            };

            foreach (var result in results)
            {
                Assert.AreEqual("A", result[0].Prop1);
                Assert.AreEqual("B", result[1].Prop1);
                Assert.AreEqual("C", result[2].Prop1);
                Assert.AreEqual("C", result[3].Prop1);
                Assert.AreEqual("C", result[4].Prop1);

                Assert.AreEqual("A", result[0].Prop2);
                Assert.AreEqual("A", result[1].Prop2);
                Assert.AreEqual("H", result[2].Prop2);
                Assert.AreEqual("G", result[3].Prop2);
                Assert.AreEqual("F", result[4].Prop2);
            }
        }

        [TestMethod]
        public void EnumerableTestMethodDescending()
        {
            var x = new List<MyClass> {
                new MyClass{ Prop1 = "C", Prop2 = "F"},
                new MyClass{ Prop1 = "C", Prop2 = "G"},
                new MyClass{ Prop1 = "C", Prop2 = "H"},
                new MyClass{ Prop1 = "B", Prop2 = "A"},
                new MyClass{ Prop1 = "A", Prop2 = "A"},
            };

            var results = new[]{
                x.OrderBy("Prop1 desc", "Prop2 asc").ToArray(),
                x.OrderByDescending("Prop1", "Prop2 asc").ToArray(),
                x.OrderBy("Prop1 desc, Prop2 asc").ToArray(),
                x.OrderBy("Prop1 desc").ThenBy("Prop2 asc").ToArray(),
                x.OrderBy("Prop1 desc").ThenByDescending("Prop2 asc").ToArray()
            };

            foreach (var result in results)
            {
                Assert.AreEqual("C", result[0].Prop1);
                Assert.AreEqual("C", result[1].Prop1);
                Assert.AreEqual("C", result[2].Prop1);
                Assert.AreEqual("B", result[3].Prop1);
                Assert.AreEqual("A", result[4].Prop1);

                Assert.AreEqual("F", result[0].Prop2);
                Assert.AreEqual("G", result[1].Prop2);
                Assert.AreEqual("H", result[2].Prop2);
                Assert.AreEqual("A", result[3].Prop2);
                Assert.AreEqual("A", result[4].Prop2);
            }
        }

        [TestMethod]
        public void QueryableTestMethod1()
        {
            var x = new List<MyClass> {
                new MyClass{ Prop1 = "C", Prop2 = "F"},
                new MyClass{ Prop1 = "C", Prop2 = "G"},
                new MyClass{ Prop1 = "C", Prop2 = "H"},
                new MyClass{ Prop1 = "B", Prop2 = "A"},
                new MyClass{ Prop1 = "A", Prop2 = "A"},
            }.AsQueryable();

            var results = new[]{
                x.OrderBy("Prop1", "Prop2 desc").ToArray(),
                x.OrderByDescending("Prop1 asc", "Prop2 desc").ToArray(),
                x.OrderBy("Prop1").ThenBy("Prop2 desc").ToArray(),
                x.OrderBy("Prop1, Prop2 desc").ToArray(),
                x.OrderBy("Prop1").ThenByDescending("Prop2").ToArray(),
            };

            foreach (var result in results)
            {
                Assert.AreEqual("A", result[0].Prop1);
                Assert.AreEqual("B", result[1].Prop1);
                Assert.AreEqual("C", result[2].Prop1);
                Assert.AreEqual("C", result[3].Prop1);
                Assert.AreEqual("C", result[4].Prop1);

                Assert.AreEqual("A", result[0].Prop2);
                Assert.AreEqual("A", result[1].Prop2);
                Assert.AreEqual("H", result[2].Prop2);
                Assert.AreEqual("G", result[3].Prop2);
                Assert.AreEqual("F", result[4].Prop2);
            }
        }

        [TestMethod]
        public void QueryableTestMethodDescending()
        {
            var x = new List<MyClass> {
                new MyClass{ Prop1 = "C", Prop2 = "F"},
                new MyClass{ Prop1 = "C", Prop2 = "G"},
                new MyClass{ Prop1 = "C", Prop2 = "H"},
                new MyClass{ Prop1 = "B", Prop2 = "A"},
                new MyClass{ Prop1 = "A", Prop2 = "A"},
            }.AsQueryable();

            var results = new[]{
                x.OrderBy("Prop1 desc", "Prop2 asc").ToArray(),
                x.OrderByDescending("Prop1", "Prop2 asc").ToArray(),
                x.OrderBy("Prop1 desc, Prop2 asc").ToArray(),
                x.OrderBy("Prop1 desc").ThenBy("Prop2 asc").ToArray(),
                x.OrderBy("Prop1 desc").ThenByDescending("Prop2 asc").ToArray()
            };

            foreach (var result in results)
            {
                Assert.AreEqual("C", result[0].Prop1);
                Assert.AreEqual("C", result[1].Prop1);
                Assert.AreEqual("C", result[2].Prop1);
                Assert.AreEqual("B", result[3].Prop1);
                Assert.AreEqual("A", result[4].Prop1);

                Assert.AreEqual("F", result[0].Prop2);
                Assert.AreEqual("G", result[1].Prop2);
                Assert.AreEqual("H", result[2].Prop2);
                Assert.AreEqual("A", result[3].Prop2);
                Assert.AreEqual("A", result[4].Prop2);
            }
        }

        class MyClass
        {
            public string Prop1 { get; set; }

            public string Prop2 { get; set; }

            public override string ToString()
            {
                return Prop1.ToString();
            }
        }
    }
}
