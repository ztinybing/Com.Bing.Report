using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Com.Bing.API
{
    class XmlDocumentEx : XmlDocument
    {
        public override void Load(string filename)
        {
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.IgnoreComments = true;
            using (XmlReader reader = XmlReader.Create(filename, settings))
            {
                this.Load(reader);
            }
        }
    }

}
