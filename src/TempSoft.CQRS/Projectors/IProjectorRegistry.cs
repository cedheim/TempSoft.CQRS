using System.Collections;
using System.Collections.Generic;
using TempSoft.CQRS.Events;

namespace TempSoft.CQRS.Projectors
{
    public interface IProjectorRegistry
    {
        void Register(ProjectorDefinition definition);

        IEnumerable<ProjectorDefinition> ListDefinitionsByEvent(IEvent @event);

        //IEnumerable<ProjectorDefinition> ListDefinitions();
    }
}