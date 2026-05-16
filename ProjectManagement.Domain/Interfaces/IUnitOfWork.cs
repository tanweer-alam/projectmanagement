using System;
using System.Collections.Generic;
using System.Text;

namespace ProjectManagement.Domain.Interfaces
{
    public interface IUnitOfWork
    {
        //returns number of state entries written to the database
        //better for unit testing
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
