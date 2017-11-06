using System.Collections.Generic;

namespace Wabbit
{
    /// <summary>
    /// Namespace is an identifier of a source of information for the example optionally followed by a float
    /// (e.g., MetricFeatures:3.28), which acts as a global scaling of all the values of the features in this namespace.
    /// If value is omitted, the default is 1. It is important that the namespace not have a space between
    /// the separator | as otherwise it is interpreted as a feature.
    /// Namespace=String[:Value]
    /// </summary>
    public class Namespace
    {
        public string Name;
        public double Value = 1.0;
        public List<Feature> Features = new List<Feature>();

        public override string ToString() => Name + (Value == 1.0 ? "" : Value.ToString()) + " " + string.Join(" ", Features);
    }
}