using System;
using System.Collections.Generic;
using System.Text;

namespace ProjectManagement.Domain.Common
{
    public abstract class AggregateRoot<TId> : Entity<TId> where TId : notnull
    {
        protected AggregateRoot() : base() { }
        protected AggregateRoot(TId id) : base(id) { }
    }
}
