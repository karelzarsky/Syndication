using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Wabbit;

namespace UnitTests
{
    [TestClass]
    public class UnitTest_VW
    {
        [TestMethod]
        public void VW_ToString1()
        {
            // 1 1.0 |MetricFeatures:3.28 height:1.5 length:2.0 |Says black with white stripes |OtherFeatures NumberOfLegs:4.0 HasStripes
            // 1 1.0 zebra|MetricFeatures:3.28 height:1.5 length:2.0 |Says black with white stripes |OtherFeatures NumberOfLegs:4.0 HasStripes

            var exa = new VWExample {Label = 1, Importance = 1.0 };
            exa.Namespaces = new List<Namespace>();
            exa.Namespaces.Add(new Namespace { Name = "MetricFeatures", Value = 3.28 });
            exa.Namespaces[0].Features.Add(new Feature { Name = "height", Value = 1.5 });
            exa.Namespaces[0].Features.Add(new Feature { Name = "length", Value = 2.0 });
            exa.Namespaces.Add(new Namespace { Name = "Says"});
            exa.Namespaces[1].Features.Add(new Feature { Name = "black"});
            exa.Namespaces[1].Features.Add(new Feature { Name = "with" });
            exa.Namespaces[1].Features.Add(new Feature { Name = "white" });
            exa.Namespaces[1].Features.Add(new Feature { Name = "stripes" });
            exa.Namespaces.Add(new Namespace { Name = "OtherFeatures" });
            exa.Namespaces[2].Features.Add(new Feature { Name = "NumberOfLegs", Value=4.0 });
            exa.Namespaces[2].Features.Add(new Feature { Name = "HasStripes" });

            Assert.AreEqual("1|MetricFeatures:3.28 height:1.5 length:2 |Says black with white stripes |OtherFeatures NumberOfLegs:4 HasStripes", exa.ToString());
        }

        [TestMethod]
        public void VW_ToString2()
        {
            // 1 1.0 |MetricFeatures:3.28 height:1.5 length:2.0 |Says black with white stripes |OtherFeatures NumberOfLegs:4.0 HasStripes
            // 1 1.0 zebra|MetricFeatures:3.28 height:1.5 length:2.0 |Says black with white stripes |OtherFeatures NumberOfLegs:4.0 HasStripes

            var exa = new VWExample { Label = 1, Tag = "zebra" };
            exa.Namespaces = new List<Namespace>();
            exa.Namespaces.Add(new Namespace { Name = "MetricFeatures", Value = 3.28 });
            exa.Namespaces[0].Features.Add(new Feature { Name = "height", Value = 1.5 });
            exa.Namespaces[0].Features.Add(new Feature { Name = "length", Value = 2.0 });
            exa.Namespaces.Add(new Namespace { Name = "Says" });
            exa.Namespaces[1].Features.Add(new Feature { Name = "black" });
            exa.Namespaces[1].Features.Add(new Feature { Name = "with" });
            exa.Namespaces[1].Features.Add(new Feature { Name = "white" });
            exa.Namespaces[1].Features.Add(new Feature { Name = "stripes" });
            exa.Namespaces.Add(new Namespace { Name = "OtherFeatures" });
            exa.Namespaces[2].Features.Add(new Feature { Name = "NumberOfLegs", Value = 4.0 });
            exa.Namespaces[2].Features.Add(new Feature { Name = "HasStripes" });

            Assert.AreEqual("1 'zebra|MetricFeatures:3.28 height:1.5 length:2 |Says black with white stripes |OtherFeatures NumberOfLegs:4 HasStripes", exa.ToString());
        }
    }
}
