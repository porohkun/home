// MIT License
// 
// Copyright (c) 2016 Wojciech Nag�rski
//                    Michael DeMond
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using System.Xml;

namespace ExtendedXmlSerialization.ContentModel.Xml
{
	public struct ContentReading
	{
		readonly System.Xml.XmlReader _reader;
		readonly int _targetDepth;

		public ContentReading(IXmlReader owner, System.Xml.XmlReader reader)
		{
			Owner = owner;
			_reader = reader;
			switch (reader.NodeType)
			{
				case XmlNodeType.Attribute:
					_reader.MoveToElement();
					break;
			}
			_targetDepth = _reader.Depth + 1;
		}

		public IXmlReader Owner { get; }

		public bool IsMember() => _reader.Prefix == string.Empty; // TODO: Might need a more reliable method for this.

		public bool Next() => _reader.Read() && _reader.IsStartElement() && _reader.Depth == _targetDepth;
	}
}