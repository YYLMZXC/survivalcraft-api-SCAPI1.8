﻿using System.Xml.Linq;
namespace Game.IContentReader
{
	public class XmlReader : IContentReader
	{
		public override string Type => "System.Xml.Linq.XElement";
		public override string[] DefaultSuffix => new string[] { "xml", "xdb" };
		public override object Get(ContentInfo[] contents)
		{
			return XElement.Load(contents[0].Duplicate());
		}
	}
}
