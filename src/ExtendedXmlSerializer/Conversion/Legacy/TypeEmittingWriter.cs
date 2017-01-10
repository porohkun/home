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

using System.Reflection;
using ExtendedXmlSerialization.Conversion.ElementModel;
using ExtendedXmlSerialization.Conversion.Write;
using ExtendedXmlSerialization.Core;
using ExtendedXmlSerialization.Core.Specifications;

namespace ExtendedXmlSerialization.Conversion.Legacy
{
    class TypeEmittingWriter : DecoratedWriter
    {
        readonly private static ISpecification<TypeInfo> Specification = AlwaysSpecification<TypeInfo>.Default;

        private readonly ISpecification<TypeInfo> _specification;

        public TypeEmittingWriter(IWriter writer) : this(Specification, writer) {}

        public TypeEmittingWriter(ISpecification<TypeInfo> specification, IWriter writer) : base(writer)
        {
            _specification = specification;
        }

        public override void Write(IWriteContext context, object instance)
        {
            var type = instance.GetType();
            if (_specification.IsSatisfiedBy(type.GetTypeInfo()) && context.Get<EmittedTypes>().Add(instance))
            {
                context.Write(TypeProperty.Default, LegacyTypeFormatter.Default.Format(type));
            }

            base.Write(context, instance);
        }
    }
}