using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using TempSoft.CQRS.Events;
using TempSoft.CQRS.Exceptions;

namespace TempSoft.CQRS.Projectors
{
    public class ProjectorDefinition : IComparable, IComparable<ProjectorDefinition>, IEquatable<ProjectorDefinition>
    {
        private static readonly Regex ParseRegex = new Regex("^((?<constant>[^\\{]+)|(?<field>\\{[^\\}]+\\}))*$");

        private readonly Token[] _tokens;
        
        public ProjectorDefinition(string name, string identifiedBy, Type projectorType, IEnumerable<Type> eventTypes, IEnumerable<string> eventGroups)
        {
            Name = name;
            ProjectorType = projectorType;
            EventGroups = eventGroups?.ToArray() ?? new string[0];
            EventTypes = eventTypes?.ToArray() ?? new Type[0];

            _tokens = ParseIdentifier(identifiedBy).ToArray();
        }

        public string Name { get; }

        public Type ProjectorType { get; }
        
        public Type[] EventTypes { get; }

        public string[] EventGroups { get; }

        public bool Matches(IEvent @event)
        {
            var matchesEventGroup = (EventGroups?.Length ?? 0) == 0 || EventGroups.Contains(@event.EventGroup);
            var matchesEventTypes = (EventTypes?.Length ?? 0) == 0 || EventTypes.Contains(@event.GetType());

            return matchesEventTypes && matchesEventGroup;
        }

        public string GenerateIdentifierFor(IEvent @event)
        {
            return _tokens.Aggregate(string.Empty, (s, token) => s + (token.GenerateToken(@event) ?? string.Empty));
        }

        #region Comparison

        public int CompareTo(object obj)
        {
            if (obj is ProjectorDefinition other)
            {
                return this.CompareTo(other);
            }

            return -1;
        }

        public int CompareTo(ProjectorDefinition other)
        {
            return string.Compare(this.Name, other.Name, StringComparison.Ordinal);
        }

        public bool Equals(ProjectorDefinition other)
        {
            return this.CompareTo(other) == 0;
        }

        public override bool Equals(object obj)
        {
            return this.CompareTo(obj) == 0;
        }

        public override int GetHashCode()
        {
            return this.Name.GetHashCode();
        }
        #endregion

        #region Identifier parsing

        private static IEnumerable<Token> ParseIdentifier(string identifiedBy)
        {
            if (string.IsNullOrEmpty(identifiedBy))
            {
                throw new ProjectionIdentifierException("Identifier can not be empty.");
            }

            var match = ParseRegex.Match(identifiedBy);

            if (!match.Success)
            {
                throw new ProjectionIdentifierException("Unable to parse projection identifier");
            }

            return match.Groups["constant"].Captures.Cast<Capture>().Select(c => (Token)new ConstantToken(c.Index, c.Value))
                .Union(match.Groups["field"].Captures.Cast<Capture>().Select(c => (Token)new FieldToken(c.Index, c.Value.Substring(1, c.Value.Length - 2))))
                .OrderBy(t => t.Index);
        }

        private abstract class Token
        {
            protected Token(int index)
            {
                Index = index;
            }

            public int Index { get; private set; }

            public abstract string GenerateToken(IEvent @event);
        }

        private class ConstantToken : Token
        {
            private readonly string _constant;

            public ConstantToken(int index, string constant)
                : base(index)
            {
                _constant = constant;
            }

            public override string GenerateToken(IEvent @event)
            {
                return _constant;
            }
        }

        private class FieldToken : Token
        {
            private readonly string _field;

            public FieldToken(int index, string field)
                : base(index)
            {
                _field = field;
            }

            public override string GenerateToken(IEvent @event)
            {
                var type = @event.GetType();

                var property = GetPropertyForType(type);

                return property?.GetMethod.Invoke(@event, new object[0])?.ToString();
            }

            private PropertyInfo GetPropertyForType(Type type)
            {
                return type.GetProperty(_field);
            }
        }

        #endregion

    }
}