using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.ServiceModel.Syndication;
using System.Xml;

namespace SyndicateLogic
{
    public class SyndicationFeedXmlReader : XmlTextReader
    {
        private static readonly DateTimeFormatInfo dtfi = CultureInfo.GetCultureInfo("en-US").DateTimeFormat;
        private readonly string[] Atom10DateTimeHints = {"updated", "published", "lastBuildDate"};
        private readonly string[] Rss20DateTimeHints = {"pubDate"};
        private bool isAtomDateTime;
        private bool isRss2DateTime;

        public SyndicationFeedXmlReader(Stream stream) : base(stream)
        {
        }

        public override bool IsStartElement(string localname, string ns)
        {
            isRss2DateTime = false;
            isAtomDateTime = false;

            if (Rss20DateTimeHints.Contains(localname)) isRss2DateTime = true;
            if (Atom10DateTimeHints.Contains(localname)) isAtomDateTime = true;

            return base.IsStartElement(localname, ns);
        }

        public override string ReadString()
        {
            string dateVal = "";
            try
            {
                dateVal = base.ReadString();
                if (isRss2DateTime)
                {
                    var objMethod = typeof(Rss20FeedFormatter).GetMethod("DateFromString", BindingFlags.NonPublic | BindingFlags.Static);
                    Debug.Assert(objMethod != null);
                    objMethod.Invoke(null, new object[] {dateVal, this});
                }
                if (isAtomDateTime)
                {
                    var objMethod = typeof(Atom10FeedFormatter).GetMethod("DateFromString", BindingFlags.NonPublic | BindingFlags.Instance);
                    Debug.Assert(objMethod != null);
                    objMethod.Invoke(new Atom10FeedFormatter(), new object[] {dateVal, this});
                }
            }
            catch (TargetInvocationException)
            {
                try
                {
                    return DateTime.Parse(dateVal).ToString(dtfi.RFC1123Pattern);
                }
                catch (FormatException)
                {
                    return DateTimeOffset.UtcNow.ToString(dtfi.RFC1123Pattern);
                }
            }
            catch (Exception e)
            {
                DataLayer.LogMessage(LogLevel.FeedError, "Cannot parse XML feed.");
                DataLayer.LogException(e);
            }
            return dateVal;
        }
    }
}