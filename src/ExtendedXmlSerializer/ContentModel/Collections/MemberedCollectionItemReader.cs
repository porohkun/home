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

using System.Collections;
using System.Collections.Generic;
using ExtendedXmlSerialization.ContentModel.Members;
using ExtendedXmlSerialization.ContentModel.Xml;
using ExtendedXmlSerialization.Core;

namespace ExtendedXmlSerialization.ContentModel.Collections
{
	class MemberedCollectionItemReader : ICollectionItemReader
	{
		readonly ICollectionItemReader _item;
		readonly IDictionary<string, IMember> _members;

		public MemberedCollectionItemReader(ICollectionItemReader item, IDictionary<string, IMember> members)
		{
			_item = item;
			_members = members;
		}

		public void Read(ContentReading reading, object instance, IList list)
		{
			if (reading.IsMember())
			{
				var reader = reading.Owner;
				var member = _members.Get(reader.Name);
				if (member != null)
				{
					member.Assign(instance, ((IReader) member).Get(reader));
					return;
				}
			}
			_item.Read(reading, instance, list);
		}
	}
}