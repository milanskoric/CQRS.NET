using Comments.Domain;
using SharedKernel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Comments.Infrastructure
{
    public class CommentsReadRepository  : IRepository 
    {
        IEventStore _store = null;

        public CommentsReadRepository(IEventStore _store)
        {
            this._store = _store;
        }

        public static readonly string Identifier = "CommentsRepositoryRead";

        public void Build()
        {
            if (_store != null )
            {
                var changes = _store.GetAllEvents();

                if (changes != null && changes.Count() > 0)
                {
                    foreach (var change in changes)
                    {
                        CommandBus.ReplayEvent(change);
                    }
                }
            }
        }


        public Result<TEntity> Save<TEntity>(TEntity entity, InvocationContext context) where TEntity : CoreObject
        {
            Result<TEntity> output = new Result<TEntity>();
            output.Success = false;
            if (entity is Comment)
            {
                Comment commnet = entity as Comment;

                if (commnet.Id == 0)
                {
                    long id = 0;

                    if (_Comments.Count > 0)
                        id = _Comments.Max(t => t.Id);
                    id++;
                    commnet.Id = id;
                }
                commnet.Guid = Guid.NewGuid();

                _Comments.Add(commnet);

                output.Success = true;
                output.ObjectId = commnet.Id.ToString();
                output.Data = commnet as TEntity;
            }

            return output;
        }

        public void Dispose()
        {
            if (_store != null)
                _store.Dispose();
        }

        public IQueryable GetQueryable(Type t)
        {
            if (t == typeof(Comment))
            {
                return _Comments.AsQueryable();
            }

            return null;
        }

        public IQueryable<TEntity> GetQueryable<TEntity>()
            where TEntity : CoreObject
        {
            if (typeof(TEntity) == typeof(Comment))
            {
                return _Comments.OfType<TEntity>().AsQueryable();
            }

            return null;
        }

        private Collection<Comment> _Comments
        {
            get {
                return InMemoryStore.Comments;
            }
        }
    }

    internal static class InMemoryStore
    {

      

       public static Collection<Comment> Comments = new Collection<Comment>();
       
    }
}


