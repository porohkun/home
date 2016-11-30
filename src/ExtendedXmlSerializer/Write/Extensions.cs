﻿using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Reflection;
using ExtendedXmlSerialization.Common;

namespace ExtendedXmlSerialization.Write
{
    public interface IWriterExtension
    {
        bool Starting(IWritingServices services);
        void Finished(IWritingServices services);
    }

    public class DefaultWriteExtensions : CompositeExtension
    {
        public DefaultWriteExtensions(ISerializationToolsFactory factory) : base(
            new ObjectReferencesExtension(factory),
            new VersionExtension(factory)
        ) {}
    }

    public class CompositeExtension : IWriterExtension
    {
        private readonly IEnumerable<IWriterExtension> _extensions;

        public CompositeExtension(params IWriterExtension[] extensions) : this(extensions.ToImmutableList()) {}

        public CompositeExtension(IEnumerable<IWriterExtension> extensions)
        {
            _extensions = extensions;
        }

        public bool Starting(IWritingServices services)
        {
            foreach (var extension in _extensions)
            {
                if (!extension.Starting(services))
                {
                    return false;
                }
            }
            return true;
        }

        public void Finished(IWritingServices services)
        {
            foreach (var extension in _extensions)
            {
                extension.Finished(services);
            }
        }
    }

    /*class ExtensionInstruction : DecoratedWriteInstruction
    {
        protected ExtensionInstruction(IInstruction instruction) : base(instruction) {}

        protected override void Execute(IWriteServices services)
        {
            if (services.Starting(services))
            {
                base.Execute(services);
            }
            services.Finished(services);
        }
    }*/

    class ToolingObjectSerializer : IObjectSerializer
    {
        public static ToolingObjectSerializer Default { get; } = new ToolingObjectSerializer();
        ToolingObjectSerializer() {}

        public string Serialize(object instance) => instance as string ?? PrimitiveValueTools.SetPrimitiveValue(instance);
    }

    public class DefaultWritingExtension : WritingExtensionBase
    {
        public static DefaultWritingExtension Default { get; } = new DefaultWritingExtension();
        DefaultWritingExtension() {}
    }

    public abstract class WritingExtensionBase : IWriterExtension
    {
        public virtual bool Starting(IWritingServices services)
        {
            var current = services.Current;
            if (current.Content != null)
            {
                return StartingContent(services, current.Instance, current.Member, current.Content);
            }

            if (current.Member != null)
            {
                return StartingMember(services, current.Instance, current.Member);
            }

            if (current.Members != null)
            {
                return StartingMembers(services, current.Instance, current.Members);
            }

            var result = current.Instance != null
                ? StartingInstance(services, current.Instance)
                : Initializing(services);
            return result;
        }

        protected virtual bool Initializing(IWritingServices services) => true;
        protected virtual bool StartingInstance(IWritingServices services, object instance) => true;
        protected virtual bool StartingMembers(IWritingServices services, object instance, IImmutableList<MemberInfo> members) => true;
        protected virtual bool StartingMember(IWritingServices services, object instance, MemberInfo member) => true;
        protected virtual bool StartingContent(IWritingServices services, object instance, MemberInfo member, string content) => true;

        public virtual void Finished(IWritingServices services)
        {
            var current = services.Current;
            if (current.Content != null)
            {
                FinishedContent(services, current.Instance, current.Member, current.Content);
            }
            else if (current.Member != null)
            {
                FinishedMember(services, current.Instance, current.Member);
            }
            else if (current.Members != null)
            {
                FinishedMembers(services, current.Instance, current.Members);
            }
            else if (current.Instance != null)
            {
                FinishedInstance(services, current.Instance);
            }
            else
            {
                Completed(services);
            }
        }

        protected virtual void FinishedInstance(IWritingServices services, object instance) {}
        protected virtual void FinishedMembers(IWritingServices services, object instance, IImmutableList<MemberInfo> members) {}
        protected virtual void FinishedMember(IWritingServices services, object instance, MemberInfo member) {}
        protected virtual void FinishedContent(IWritingServices services, object instance, MemberInfo member, string content) {}
        protected virtual void Completed(IWritingServices services) {}
    }

    public class VersionExtension : WritingExtensionBase
    {
        private readonly ISerializationToolsFactory _factory;

        public VersionExtension(ISerializationToolsFactory factory)
        {
            _factory = factory;
        }

        protected override bool StartingMembers(IWritingServices services, object instance, IImmutableList<MemberInfo> members)
        {
            var type = instance.GetType();
            var version = _factory
                .GetConfiguration(type)?
                .Version;

            if (version != null && version > 0)
            {
                services.Attach(new VersionProperty(version.Value));
            }
            return base.StartingMembers(services, instance, members);
        }
    }

    class VersionProperty : AttachedPropertyBase
    {
        public VersionProperty(int version) : base(ExtendedXmlSerializer.Version, version) {}
    }

    class ObjectReferenceProperty : AttachedPropertyBase
    {
        public ObjectReferenceProperty(string value) : base(ExtendedXmlSerializer.Ref, value) {}
    }

    class ObjectIdProperty : AttachedPropertyBase
    {
        public ObjectIdProperty(string value) : base(ExtendedXmlSerializer.Id, value) {}
    }

    public class ObjectReferencesExtension : WritingExtensionBase
    {
        private readonly IDictionary<string, object>
            _references = new ConcurrentDictionary<string, object>();

        private readonly ISerializationToolsFactory _factory;

        public ObjectReferencesExtension(ISerializationToolsFactory factory)
        {
            _factory = factory;
        }

        protected override bool StartingMembers(IWritingServices services, object instance, IImmutableList<MemberInfo> members)
        {
            var type = instance.GetType();
            var configuration = _factory.GetConfiguration(type);

            if (configuration?.IsObjectReference ?? false)
            {
                var objectId = configuration.GetObjectId(instance);
                var key = $"{type.FullName}{ExtendedXmlSerializer.Underscore}{objectId}";
                var contains = _references.ContainsKey(key);
                var property = contains ? (IAttachedProperty)new ObjectReferenceProperty(objectId) : new ObjectIdProperty(objectId);
                var result = !contains;
                services.Property(property);
                if (result)
                {
                    _references.Add(key, instance);
                }
                return result;
            }
            return base.StartingMembers(services, instance, members);
        }

        protected override bool Initializing(IWritingServices services)
        {
            _references.Clear();
            return base.Initializing(services);
        }

        protected override void Completed(IWritingServices services) => _references.Clear();
    }
}