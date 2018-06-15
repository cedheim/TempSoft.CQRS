using System;

namespace TempSoft.CQRS.Domain
{
    public interface IEntity
    {
        string Id { get; set; }
    }
}