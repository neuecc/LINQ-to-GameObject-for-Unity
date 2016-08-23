using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading;
using NUnit.Framework;
using UnityEngine;
using Unity.Linq;

namespace UnityTest
{
    [TestFixture]
    internal class TraverseTest
    {
        GameObject Origin
        {
            get { return GameObject.Find("Origin"); }
        }

        [Test]
        public void Parent()
        {
            Origin.Parent().name.Is("Container");
            Origin.Parent().Parent().name.Is("Root");
            Origin.transform.root.name.Is("Root");
        }

        [Test]
        public void Child()
        {
            Origin.Child("Group").ChildrenAndSelf()
                .Where(x => x.name == "Group")
                .Select(x => x.name)
                .IsCollection("Group", "Group");

            var c2 = Origin.Parent().Child("C2");
            c2.IsNotNull();
            c2.activeInHierarchy.IsFalse();
            c2.name.Is("C2");
        }

        [Test]
        public void Children()
        {
            {
                Origin.Children().Select(x => x.name)
                    .IsCollection("Sphere_A", "Sphere_B", "Group", "Sphere_A", "Sphere_B");

                Origin.Children()
                    .Where(x => x.name == "Sphere_B")
                    .Select(x => x.name)
                    .IsCollection("Sphere_B", "Sphere_B");

                Origin.ChildrenAndSelf().Select(x => x.name)
                    .IsCollection("Origin", "Sphere_A", "Sphere_B", "Group", "Sphere_A", "Sphere_B");

                Origin.ChildrenAndSelf()
                    .Where(x => x.name == "Sphere_A")
                    .Select(x => x.name)
                    .IsCollection("Sphere_A", "Sphere_A");
            }
            {
                Origin.Children().ToArray().Select(x => x.name)
                    .IsCollection("Sphere_A", "Sphere_B", "Group", "Sphere_A", "Sphere_B");

                Origin.ChildrenAndSelf().ToArray().Select(x => x.name)
                    .IsCollection("Origin", "Sphere_A", "Sphere_B", "Group", "Sphere_A", "Sphere_B");
            }
        }

        [Test]
        public void Ancestors()
        {
            Origin.Ancestors().Select(x => x.name).IsCollection("Container", "Root");

            var group = Origin.Child("Group");
            group.Ancestors().Select(x => x.name).IsCollection("Origin", "Container", "Root");
            group.AncestorsAndSelf().Select(x => x.name).IsCollection("Group", "Origin", "Container", "Root");

            group.Child("Group").AncestorsAndSelf().Where(x => x.name == "Group").Select(x => x.name).IsCollection("Group", "Group");
        }

        [Test]
        public void Descendants()
        {
            Origin.Descendants().Select(x => x.name)
                .IsCollection("Sphere_A", "Sphere_B", "Group", "P1", "Group", "Sphere_B", "P2", "Sphere_A", "Sphere_B");

            Origin.Descendants().Where(x => x.name == "Sphere_B").Select(x => x.name).IsCollection("Sphere_B", "Sphere_B", "Sphere_B");

            Origin.DescendantsAndSelf().Select(x => x.name)
                .IsCollection("Origin", "Sphere_A", "Sphere_B", "Group", "P1", "Group", "Sphere_B", "P2", "Sphere_A", "Sphere_B");


            Origin.DescendantsAndSelf().Where(x => x.name == "Sphere_B").Select(x => x.name)
                .IsCollection("Sphere_B", "Sphere_B", "Sphere_B");

            Origin.Child("Group").DescendantsAndSelf().Where(x => x.name == "Group").Select(x => x.name)
                .IsCollection("Group", "Group");
        }

        [Test]
        public void BeforeSelf()
        {
            Origin.BeforeSelf().Select(x => x.name)
                .IsCollection("C1", "C2");

            Origin.BeforeSelf().Where(x => x.name == "C2").Select(x => x.name)
                .IsCollection("C2");

            Origin.BeforeSelfAndSelf().Select(x => x.name)
                .IsCollection("C1", "C2", "Origin");

            Origin.Child("Sphere_B").BeforeSelfAndSelf().Select(x => x.name)
                .IsCollection("Sphere_A", "Sphere_B");

            Origin.Children().Where(x => x.name == "Sphere_B").Last().BeforeSelfAndSelf().Where(x => x.name == "Sphere_B").Select(x => x.name)
                .IsCollection("Sphere_B", "Sphere_B");
        }

        [Test]
        public void AfterSelf()
        {
            Origin.AfterSelf().Select(x => x.name)
                .IsCollection("C3", "C4");

            Origin.AfterSelf().Where(x => x.name == "C3").Select(x => x.name)
                .IsCollection("C3");

            Origin.AfterSelfAndSelf().Select(x => x.name)
                .IsCollection("Origin", "C3", "C4");

            Origin.Child("Sphere_B").AfterSelfAndSelf().Select(x => x.name)
                .IsCollection("Sphere_B", "Group", "Sphere_A", "Sphere_B");

            Origin.Child("Sphere_B").AfterSelfAndSelf().Where(x => x.name == "Sphere_B").Select(x => x.name)
                .IsCollection("Sphere_B", "Sphere_B");
        }

        [Test]
        public void DescendantsVariation()
        {
            {
                Origin.Descendants().ToArray().Select(x => x.name)
                    .IsCollection("Sphere_A", "Sphere_B", "Group", "P1", "Group", "Sphere_B", "P2", "Sphere_A", "Sphere_B");

                Origin.Descendants().ToArray(x => x.name)
                    .IsCollection("Sphere_A", "Sphere_B", "Group", "P1", "Group", "Sphere_B", "P2", "Sphere_A", "Sphere_B");

                Origin.Descendants().ToArray(x => x.name == "Sphere_B").Select(x => x.name).IsCollection("Sphere_B", "Sphere_B", "Sphere_B");

                Origin.Descendants().ToArray(x => x.name == "Sphere_B", x => x.name).IsCollection("Sphere_B", "Sphere_B", "Sphere_B");

                Origin.Descendants().ToArray(x => x.name, x => x == "Sphere_B", x => x).IsCollection("Sphere_B", "Sphere_B", "Sphere_B");


                var l = new List<string>();
                Origin.Descendants().ForEach(x => l.Add(x.name));
                l.IsCollection("Sphere_A", "Sphere_B", "Group", "P1", "Group", "Sphere_B", "P2", "Sphere_A", "Sphere_B");
            }
            {
                Origin.Descendants().OfComponent<Transform>().ToArray().Select(x => x.name)
    .IsCollection("Sphere_A", "Sphere_B", "Group", "P1", "Group", "Sphere_B", "P2", "Sphere_A", "Sphere_B");


                var l = new List<string>();
                Origin.Descendants().OfComponent<Transform>().ForEach(x => l.Add(x.name));
                l.IsCollection("Sphere_A", "Sphere_B", "Group", "P1", "Group", "Sphere_B", "P2", "Sphere_A", "Sphere_B");
            }
        }



        [Test]
        public void DescendantsDescendIntoChildren()
        {
            {
                Origin.Descendants(_ => true).Select(x => x.name)
                    .IsCollection("Sphere_A", "Sphere_B", "Group", "P1", "Group", "Sphere_B", "P2", "Sphere_A", "Sphere_B");

                Origin.DescendantsAndSelf(_ => true).Select(x => x.name)
                    .IsCollection("Origin", "Sphere_A", "Sphere_B", "Group", "P1", "Group", "Sphere_B", "P2", "Sphere_A", "Sphere_B");

                Origin.Descendants(_ => false).Select(x => x.name)
                    .Count().Is(0);

                Origin.DescendantsAndSelf(_ => false).Select(x => x.name)
                    .IsCollection("Origin");

                Origin.Descendants(x => x.name != "Group").Select(x => x.name)
                    .IsCollection("Sphere_A", "Sphere_B", "Group", "Sphere_A", "Sphere_B");

                Origin.DescendantsAndSelf(x => x.name != "Group").Select(x => x.name)
                    .IsCollection("Origin", "Sphere_A", "Sphere_B", "Group", "Sphere_A", "Sphere_B");
            }
            {
                // ToArray optimized path check
                Origin.Descendants(_ => true).ToArray().Select(x => x.name)
                    .IsCollection("Sphere_A", "Sphere_B", "Group", "P1", "Group", "Sphere_B", "P2", "Sphere_A", "Sphere_B");

                Origin.DescendantsAndSelf(_ => true).ToArray().Select(x => x.name)
                    .IsCollection("Origin", "Sphere_A", "Sphere_B", "Group", "P1", "Group", "Sphere_B", "P2", "Sphere_A", "Sphere_B");

                Origin.Descendants(_ => false).ToArray().Select(x => x.name)
                    .Count().Is(0);

                Origin.DescendantsAndSelf(_ => false).ToArray().Select(x => x.name)
                    .IsCollection("Origin");

                Origin.Descendants(x => x.name != "Group").ToArray().Select(x => x.name)
                    .IsCollection("Sphere_A", "Sphere_B", "Group", "Sphere_A", "Sphere_B");

                Origin.DescendantsAndSelf(x => x.name != "Group").ToArray().Select(x => x.name)
                    .IsCollection("Origin", "Sphere_A", "Sphere_B", "Group", "Sphere_A", "Sphere_B");
            }
            {
                // OfComponent Optimized path check
                Origin.Descendants(_ => true).OfComponent<Transform>().ToArray().Select(x => x.name)
                    .IsCollection("Sphere_A", "Sphere_B", "Group", "P1", "Group", "Sphere_B", "P2", "Sphere_A", "Sphere_B");

                Origin.DescendantsAndSelf(_ => true).OfComponent<Transform>().ToArray().Select(x => x.name)
                    .IsCollection("Origin", "Sphere_A", "Sphere_B", "Group", "P1", "Group", "Sphere_B", "P2", "Sphere_A", "Sphere_B");

                Origin.Descendants(_ => false).OfComponent<Transform>().ToArray().Select(x => x.name)
                    .Count().Is(0);

                Origin.DescendantsAndSelf(_ => false).OfComponent<Transform>().ToArray().Select(x => x.name)
                    .IsCollection("Origin");

                Origin.Descendants(x => x.name != "Group").OfComponent<Transform>().ToArray().Select(x => x.name)
                    .IsCollection("Sphere_A", "Sphere_B", "Group", "Sphere_A", "Sphere_B");

                Origin.DescendantsAndSelf(x => x.name != "Group").OfComponent<Transform>().ToArray().Select(x => x.name)
                    .IsCollection("Origin", "Sphere_A", "Sphere_B", "Group", "Sphere_A", "Sphere_B");
            }
        }
    }
}
