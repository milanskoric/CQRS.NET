using SharedKernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Comments.Core 
{
    public interface ICommentReadRepository : IRepository
    {

    }

    public interface ICommentWriteRepository : IRepository
    {
        
    }

    public interface ICommentUnitOfWork : IUnitOfWork
    {

    }
}
