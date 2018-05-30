using System;

namespace TempSoft.CQRS.Projectors
{
    public interface IProjection
    {
        string Id { get; }

        string ProjectorId { get; }
    }
}