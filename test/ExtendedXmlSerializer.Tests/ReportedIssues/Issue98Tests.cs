﻿using ExtendedXmlSerializer.Configuration;
using ExtendedXmlSerializer.Core;
using ExtendedXmlSerializer.ExtensionModel.Xml;
using ExtendedXmlSerializer.Tests.Support;
using FluentAssertions;
using System.Collections.Generic;
using Xunit;

namespace ExtendedXmlSerializer.Tests.ReportedIssues
{
	public sealed class Issue98Tests
	{
		[Fact]
		void Verify()
		{
			var instance = new Foo {Bar = new Bar()};
			instance.Bar.Foos.Add(instance);

			var serializer = new ConfigurationContainer().EnableReferences()
			                                             .Create();

			var cycled = serializer.Cycle(instance);

			cycled.Should()
			      .BeSameAs(cycled.Bar.Foos.Only());

			cycled.Bar.Should()
			      .BeSameAs(cycled.Bar.Foos.Only().Bar);
		}

		public class Foo
		{
			public virtual Bar Bar { get; set; }
		}

		public class Bar
		{
			public virtual ICollection<Foo> Foos { get; set; } = new List<Foo>();
		}
	}
}