using System;

namespace TempSoft.CQRS.Domain
{
    public interface IEntity
    {
        Guid Id { get; set; }
    }
}