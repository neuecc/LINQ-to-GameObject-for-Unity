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
            Origin.Parent().name.Is("Root");
        }

        [Test]
        public void Children()
        {
            Origin.Children().Select(x => x.name)
                .IsCollection("Sphere_A", "Sphere_B", "Group", "Sphere_A", "Sphere_B");
        }

    }
}
