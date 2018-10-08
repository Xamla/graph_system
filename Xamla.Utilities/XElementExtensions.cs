using System;
using System.IO;
using System.Xml.Linq;

namespace Xamla.Utilities
{
    public static class XElementExtensions
    {
        public static void ReplaceChild(this XContainer parent, XName childName, XElement replacement)
        {
            var child = parent.Element(childName);
            if (child != null)
            {
                if (replacement != null)
                    child.ReplaceWith(replacement);
                else
                    child.Remove();
            }
            else
            {
                if (replacement != null)
                    parent.Add(replacement);
            }
        }

        public static string GetChildText(this XElement parent, XName childName)
        {
            var child = parent.Element(childName);
            if (child == null)
                return null;
            return child.Value;
        }

        public static void SetChildText(this XElement parent, XName childName, string value)
        {
            var child = parent.Element(childName);
            if (child != null)
            {
                if (value != null)
                    child.Value = value;
                else
                    child.Remove();
            }
            else
            {
                if (value != null)
                    parent.Add(new XElement(childName, value));
            }
        }
    }
}
