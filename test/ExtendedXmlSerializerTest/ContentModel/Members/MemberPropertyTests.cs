﻿// MIT License
// 
// Copyright (c) 2016 Wojciech Nagórski
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

using System.Collections.Generic;
using System.Reflection;
using ExtendedXmlSerialization.Configuration;
using ExtendedXmlSerialization.ContentModel.Converters;
using ExtendedXmlSerialization.ContentModel.Members;
using Xunit;

namespace ExtendedXmlSerialization.Test.ContentModel.Members
{
	public class MemberPropertyTests
	{
		[Fact]
		public void VerifyWrite()
		{
			var converters = new Dictionary<MemberInfo, IConverter>
			                 {
				                 {typeof(Subject).GetRuntimeProperty(nameof(Subject.Message)), StringConverter.Default},
								 {typeof(Subject).GetRuntimeProperty(nameof(Subject.Number)), IntegerConverter.Default}
			                 };
			var specifications = new Dictionary<MemberInfo, IRuntimeMemberSpecification>();
			var configuration = new ExtendedXmlConfiguration(ExtendedXmlSerializerFactory.Default, converters, specifications);
			
			var serializer = configuration.Create();

			var expected = new Subject { Message = "Hello World!", Number = 6776 };
			var data = serializer.Serialize(expected);
			var actual = serializer.Deserialize<Subject>(data);
			Assert.Equal(expected.Message, actual.Message);
			Assert.Equal(expected.Number, actual.Number);
		}

		class Subject
		{
			public string Message { get; set; }

			public int Number { get; set; }
		}
	}
}