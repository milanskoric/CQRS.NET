using Comments.Core;
using SharedKernel;
using SharedKernel.Infrastructure;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Comments.Infrastructure
{
    internal class CommentsContext : DataContext
    {
        public static readonly string Identifier = "DefaultConnection";
        public static readonly string SchemaName = "comment";

        public CommentsContext()
            : base("name=" + CommentsContext.Identifier, CommentsContext.SchemaName)
        {
            Database.SetInitializer<CommentsContext>(new DropCreateDatabaseIfModelChanges<CommentsContext>());
        }

        public DbSet<Comments.Domain.Comment> Comments { get; set; }
    }

    public class CommentRepository : Repository, ICommentRepository
    {
        public static readonly string Identifier = "CommentsRepositoryWrite";
        public CommentRepository()
            :base(new CommentsContext())
        { 
        
        }

        public IQueryable<Comments.Domain.Comment> Comments
        {
            get { return (this.dataContext as CommentsContext).Comments; }
        }


 
    }

 
}
