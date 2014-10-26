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
            Origin.Child("Group").ChildrenAndSelf("Group")
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
            Origin.Children().Select(x => x.name)
                .IsCollection("Sphere_A", "Sphere_B", "Group", "Sphere_A", "Sphere_B");

            Origin.Children("Sphere_B").Select(x => x.name)
                .IsCollection("Sphere_B", "Sphere_B");

            Origin.ChildrenAndSelf().Select(x => x.name)
                .IsCollection("Origin", "Sphere_A", "Sphere_B", "Group", "Sphere_A", "Sphere_B");

            Origin.ChildrenAndSelf("Sphere_A").Select(x => x.name)
                .IsCollection("Sphere_A", "Sphere_A");
        }

        [Test]
        public void Ancestors()
        {
            Origin.Ancestors().Select(x => x.name).IsCollection("Container", "Root");

            var group = Origin.Child("Group");
            group.Ancestors().Select(x => x.name).IsCollection("Origin", "Container", "Root");
            group.AncestorsAndSelf().Select(x => x.name).IsCollection("Group", "Origin", "Container", "Root");

            group.Child("Group").AncestorsAndSelf("Group").Select(x => x.name).IsCollection("Group", "Group");
        }

        [Test]
        public void Descendants()
        {
            Origin.Descendants().Select(x => x.name)
                .IsCollection("Sphere_A", "Sphere_B", "Group", "P1", "Group", "Sphere_B", "P2", "Sphere_A", "Sphere_B");

            Origin.Descendants("Sphere_B").Select(x => x.name).IsCollection("Sphere_B", "Sphere_B", "Sphere_B");

            Origin.DescendantsAndSelf().Select(x => x.name)
                .IsCollection("Origin", "Sphere_A", "Sphere_B", "Group", "P1", "Group", "Sphere_B", "P2", "Sphere_A", "Sphere_B");


            Origin.DescendantsAndSelf("Sphere_B").Select(x => x.name)
                .IsCollection("Sphere_B", "Sphere_B", "Sphere_B");

            Origin.Child("Group").DescendantsAndSelf("Group").Select(x => x.name)
                .IsCollection("Group", "Group");
        }

        [Test]
        public void ObjectsBeforeSelf()
        {
            Origin.ObjectsBeforeSelf().Select(x => x.name)
                .IsCollection("C1", "C2");

            Origin.ObjectsBeforeSelf("C2").Select(x => x.name)
                .IsCollection("C2");

            Origin.ObjectsBeforeSelfAndSelf().Select(x => x.name)
                .IsCollection("C1", "C2", "Origin");

            Origin.Child("Sphere_B").ObjectsBeforeSelfAndSelf().Select(x => x.name)
                .IsCollection("Sphere_A", "Sphere_B");

            Origin.Children("Sphere_B").Last().ObjectsBeforeSelfAndSelf("Sphere_B").Select(x => x.name)
                .IsCollection("Sphere_B", "Sphere_B");
        }

        [Test]
        public void ObjectsAfterSelf()
        {
            Origin.ObjectsAfterSelf().Select(x => x.name)
                .IsCollection("C3", "C4");
            
            Origin.ObjectsAfterSelf("C3").Select(x => x.name)
                .IsCollection("C3");

            Origin.ObjectsAfterSelfAndSelf().Select(x => x.name)
                .IsCollection("Origin", "C3", "C4");

            Origin.Child("Sphere_B").ObjectsAfterSelfAndSelf().Select(x => x.name)
                .IsCollection("Sphere_B", "Group", "Sphere_A", "Sphere_B");

            Origin.Child("Sphere_B").ObjectsAfterSelfAndSelf("Sphere_B").Select(x => x.name)
                .IsCollection("Sphere_B", "Sphere_B");
        }
    }
}
