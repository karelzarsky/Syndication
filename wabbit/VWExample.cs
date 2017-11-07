using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wabbit
{
    /// <summary>
    /// VW supports 2 input formats: plain text (this page) and JSON. 
    /// The raw (plain text) input data for VW should have one example per line. Each example should be formatted as follows.
    /// [Label] [Importance] [Base] [Tag]|Namespace Features |Namespace Features... |Namespace Features
    /// </summary>
    public class VWExample
    {
        /// <summary>
        /// Label is the real number that we are trying to predict for this example. If the label is omitted, then no training will be performed
        /// with the corresponding example, although VW will still compute a prediction.
        /// </summary>
        public double Label;

        /// <summary>
        /// Importance (importance weight) is a non-negative real number indicating the relative importance of this example over the others.
        /// Omitting this gives a default importance of 1 to the example.
        /// </summary>
        public double Importance = 1.0;

        /// <summary>
        /// Base is used for residual regression. It is added to the prediction before computing an update. The default value is 0.
        /// </summary>
        public double Base = 0.0;

        /// <summary>
        /// Tag is a string that serves as an identifier for the example.It is reported back when predictions are made. It doesn't have to be unique.
        /// The default value if it is not provided is the empty string. If you provide a tag without a weight you need to disambiguate: either make
        /// the tag touch the | (no trailing spaces) or mark it with a leading single-quote '. If you don't provide a tag, you need to have a space before the |.
        /// </summary>
        public string Tag;

        /// <summary>
        /// Namespace is an identifier of a source of information for the example optionally followed by a float (e.g., MetricFeatures:3.28),
        /// which acts as a global scaling of all the values of the features in this namespace. If value is omitted, the default is 1.
        /// It is important that the namespace not have a space between the separator | as otherwise it is interpreted as a feature.
        /// </summary>
        public List<Namespace> Namespaces = new List<Namespace>();

        public override string ToString() =>
            Label.ToString()
                + (Importance == 1.0 && Base == 0.0 ? string.Empty : " " + Importance.ToString())
                + (Base == 0.0 ? string.Empty : " " + Base.ToString())
                + (string.IsNullOrWhiteSpace(Tag) ? string.Empty : " '" + Tag)
                + string.Join(" ", Namespaces);
    }

    // 1 1.0 |MetricFeatures:3.28 height:1.5 length:2.0 |Says black with white stripes |OtherFeatures NumberOfLegs:4.0 HasStripes
    // 1 1.0 zebra|MetricFeatures:3.28 height:1.5 length:2.0 |Says black with white stripes |OtherFeatures NumberOfLegs:4.0 HasStripes
}
