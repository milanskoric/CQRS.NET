using Comments.Domain;
using SharedKernel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharedKernel.Infrastructure;
using Comments.Core;

namespace Comments.Infrastructure
{
    public class CommentsReadUnitOfWork : UnitOfWork
    {
        public static readonly string Identifier = "CommentsUnitOfWorkRead";

        public CommentsReadUnitOfWork(ICommentReadRepository commentsReadRepository)
            : base(null, commentsReadRepository)
        {
            

        }

        private Collection<Comment> _Comments = new Collection<Comment>();

        public override void Set<TEntity>(TEntity entity, InvocationContext context)
        {
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
            }
        }

          

        public override Result<TEntity> Save<TEntity>(TEntity entity, InvocationContext context)
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

        public override void Commit()
        {
            foreach(var c in _Comments)
                InMemoryStore.Comments.Add(c);
        }
    }

    public class CommentsReadRepository : ICommentReadRepository
    {
        IEventStore _store = null;
        ICommandBus _bus = null;

        public CommentsReadRepository(IEventStore _store, ICommandBus _bus)
        {
            this._store = _store;
            this._bus = _bus;
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
                        _bus.Replay(change);
                    }
                }
            }
        }


        private bool disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    if (_store != null)
                        _store.Dispose();
                }
            }
            this.disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
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
            where TEntity : AggregateRoot
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


        public IQueryable<TEntity> BuildQuery<TEntity>(QueryOptions options)
           where TEntity : AggregateRoot
        {
            var q = this.GetQueryable<TEntity>();

            if (options != null)
            {
                if (options.HasLimit())
                {
                    q = q.Take(options.GetLimit());
                }
            }

            return q;
        }

        public IEnumerable<TEntity> ExecuteList<TEntity>(IQueryable<TEntity> query)
             where TEntity : AggregateRoot
        {
            return query.ToList();
        }

        public TEntity ExecuteSingle<TEntity>(IQueryable<TEntity> query)
            where TEntity : AggregateRoot
        {
            return query.SingleOrDefault();
        }
    }

    internal static class InMemoryStore
    {

      

       public static Collection<Comment> Comments = new Collection<Comment>();
       
    }
}


