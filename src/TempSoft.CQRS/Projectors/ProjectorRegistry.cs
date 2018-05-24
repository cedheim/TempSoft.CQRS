using System.Collections.Generic;
using System.Linq;
using TempSoft.CQRS.Events;
using TempSoft.CQRS.Exceptions;

namespace TempSoft.CQRS.Projectors
{
    public class ProjectorRegistry : IProjectorRegistry
    {
        private readonly HashSet<ProjectorDefinition> _projectorDefinitions = new HashSet<ProjectorDefinition>();
        
        public void Register(ProjectorDefinition definition)
        {
            var success = _projectorDefinitions.Add(definition);
            if(!success)
                throw new DuplicateProjectorDefinitionException($"Unable to add definition with name {definition.Name}");
        }

        public IEnumerable<ProjectorDefinition> ListDefinitionsByEvent(IEvent @event)
        {
            return _projectorDefinitions.Where(pd => pd.Matches(@event));
        }
    }
}