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
    public class ProjectorDefinition
    {
        private static readonly Regex ParseRegex = new Regex("^((?<constant>[^\\{]+)|(?<field>\\{[^\\}]+\\}))*$");

        private Token[] _tokens;

        private ProjectorDefinition()
        {
            IdentifiedBy = string.Empty;
            EventGroups = new string[0];
            EventTypes = new Type[0];

            _tokens = new Token[0];
        }

        public ProjectorDefinition(string identifiedBy, IEnumerable<Type> eventTypes, IEnumerable<string> eventGroups)
        {
            IdentifiedBy = identifiedBy;
            EventGroups = eventGroups?.ToArray() ?? new string[0];
            EventTypes = eventTypes?.ToArray() ?? new Type[0];

            _tokens = ParseIdentifier().ToArray();
        }

        public string IdentifiedBy { get; private set; }

        public Type[] EventTypes { get; private set; }

        public string[] EventGroups { get; private set; }

        public string GenerateIdentifierFor(IEvent @event)
        {
            return _tokens.Aggregate(string.Empty, (s, token) => s + (token.GenerateToken(@event) ?? string.Empty));
        }

        private IEnumerable<Token> ParseIdentifier()
        {
            if (string.IsNullOrEmpty(IdentifiedBy))
            {
                throw new ProjectionIdentifierException("Identifier can not be empty.");
            }

            var match = ParseRegex.Match(IdentifiedBy);

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
                if (property == default(PropertyInfo))
                {
                    return default(string);
                }

                return property.GetMethod.Invoke(@event, new object[0])?.ToString();
            }

            private PropertyInfo GetPropertyForType(Type type)
            {
                return type.GetProperty(_field);

            }
        }
    }
}