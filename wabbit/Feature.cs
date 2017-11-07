namespace Wabbit
{
    /// <summary>
    /// Features is a sequence of whitespace separated strings, each of which is optionally followed by a float
    /// (e.g., NumberOfLegs:4.0 HasStripes). Each string is a feature and the value is the feature value for that
    /// example. Omitting a feature means that its value is zero. Including a feature but omitting its value
    /// means that its value is 1.
    /// Features=(String[:Value] )*
    /// </summary>
    public class Feature
    {
        public string Name;
        public double Value = 1.0;

        public override string ToString() => Name + (Value == 1.0 ? "" : ":" + Value.ToString());
    }
}