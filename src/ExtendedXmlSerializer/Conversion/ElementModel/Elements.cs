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

using System.Linq;
using System.Reflection;
using ExtendedXmlSerialization.Core.Sources;

namespace ExtendedXmlSerialization.Conversion.ElementModel
{
    /*class TypeResolver : IParameterizedSource<XElement, IElement>
    {
        private readonly INamedTypes _types;
        private readonly IElements _elements;

        public TypeResolver(INamedTypes types, IElements elements)
        {
            _types = types;
            _elements = elements;
        }

        public IElement Get(XElement parameter)
        {
            var type = _types.Get(parameter);
            var result = _elements.Get(type);
            return result;
        }
    }*/

    public class Elements : Selector<TypeInfo, IElement>, IElements
    {
        public static Elements Default { get; } = new Elements();
        Elements() : this(new ElementCandidates().ToArray()) {}

        public Elements(params ICandidate<TypeInfo, IElement>[] candidates) : base(candidates) {}
    }
}