using System;
using System.Xml.Linq;

namespace Xamla.Utilities
{
    public class ElementExpectedException
        : Exception
    {
        public ElementExpectedException(XName expected)
            : base(string.Format("{0} element expected.", new XElement(expected.LocalName)))
        {
        }
    }
}
