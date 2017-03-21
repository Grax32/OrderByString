using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OrderByExtensions;
using static OrderByExtensions.QueryableExtensions;
using System.Reflection;
using System.Linq.Expressions;
using System.Diagnostics;

namespace OrderByExtensionsText
{
    [TestClass]
    public class UnitTest1
    {
        public TestContext TestContext { get; set; }

        [TestInitialize]
        public void InitTest()
        {
            Debug.WriteLine("TEST: " + TestContext.TestName);
        }

        [TestMethod]
        public void EnumerableTestMethod1()
        {
            Debug.WriteLine("TEST: " + MethodBase.GetCurrentMethod().Name);
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

        [TestMethod]
        public void QueryableTestMethodDotted()
        {
            var x = new List<MyClass> {
                new MyClass{ Prop1 = "One",   Prop3 = new MyPropClass { SubProp1 = 1, SubProp2 = 1 } },
                new MyClass{ Prop1 = "Two",   Prop3 = new MyPropClass { SubProp1 = 2, SubProp2 = 2 } },
                new MyClass{ Prop1 = "Three", Prop3 = new MyPropClass { SubProp1 = 3, SubProp2 = 3 } },
                new MyClass{ Prop1 = "Four",  Prop3 = new MyPropClass { SubProp1 = 3, SubProp2 = 4 } },
                new MyClass{ Prop1 = "Five",  Prop3 = new MyPropClass { SubProp1 = 3, SubProp2 = 5 } }
            }.AsQueryable();

            var results = new[]{
                x.OrderBy("Prop3.SubProp1", "Prop3.SubProp2 desc").ToArray(),
                x.OrderByDescending("Prop3.SubProp1 asc", "Prop3.SubProp2 desc").ToArray(),
                x.OrderBy("Prop3.SubProp1").ThenBy("Prop3.SubProp2 desc").ToArray(),
                x.OrderBy("Prop3.SubProp1, Prop3.SubProp2 desc").ToArray(),
                x.OrderBy("Prop3.SubProp1").ThenByDescending("Prop3.SubProp2").ToArray(),
            };

            foreach (var result in results)
            {
                Assert.AreEqual("One", result[0].Prop1);
                Assert.AreEqual("Two", result[1].Prop1);
                Assert.AreEqual("Five", result[2].Prop1);
                Assert.AreEqual("Four", result[3].Prop1);
                Assert.AreEqual("Three", result[4].Prop1);
            }
        }


        [TestMethod]
        public void EnumerableTestMethodDotted()
        {
            var x = new List<MyClass> {
                new MyClass{ Prop1 = "One",   Prop3 = new MyPropClass { SubProp1 = 1, SubProp2 = 1 } },
                new MyClass{ Prop1 = "Two",   Prop3 = new MyPropClass { SubProp1 = 2, SubProp2 = 2 } },
                new MyClass{ Prop1 = "Three", Prop3 = new MyPropClass { SubProp1 = 3, SubProp2 = 3 } },
                new MyClass{ Prop1 = "Four",  Prop3 = new MyPropClass { SubProp1 = 3, SubProp2 = 4 } },
                new MyClass{ Prop1 = "Five",  Prop3 = new MyPropClass { SubProp1 = 3, SubProp2 = 5 } }
            }.AsEnumerable();

            var results = new[]{
                x.OrderBy("Prop3.SubProp1", "Prop3.SubProp2 desc").ToArray(),
                x.OrderByDescending("Prop3.SubProp1 asc", "Prop3.SubProp2 desc").ToArray(),
                x.OrderBy("Prop3.SubProp1").ThenBy("Prop3.SubProp2 desc").ToArray(),
                x.OrderBy("Prop3.SubProp1, Prop3.SubProp2 desc").ToArray(),
                x.OrderBy("Prop3.SubProp1").ThenByDescending("Prop3.SubProp2").ToArray(),
            };

            foreach (var result in results)
            {
                Assert.AreEqual("One", result[0].Prop1);
                Assert.AreEqual("Two", result[1].Prop1);
                Assert.AreEqual("Five", result[2].Prop1);
                Assert.AreEqual("Four", result[3].Prop1);
                Assert.AreEqual("Three", result[4].Prop1);
            }
        }


        class MyClass
        {
            public string Prop1 { get; set; }

            public string Prop2 { get; set; }

            public MyPropClass Prop3 { get; set; }

            public override string ToString()
            {
                return Prop1 + " - " + Prop2 + " - " + (Prop3?.SubProp1.ToString() ?? "NULL") + "-" + (Prop3?.SubProp2.ToString() ?? "NULL");
            }
        }

        class MyPropClass
        {
            public int SubProp1 { get; set; }
            public int SubProp2 { get; set; }

        }
    }
}
