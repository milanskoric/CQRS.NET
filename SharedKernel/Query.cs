﻿using Microsoft.Practices.ServiceLocation;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedKernel
{
    // Marker interface to signify a query - all queries will implement this
    public interface IQueryMessage : IMessage
    {
    }

    //// Marker interface to signify a query result - all view models will implement this 
    //public interface IQueryResult
    //{
    //}

    // Interface for query handlers - has two type parameters for the query and the query result
    public interface IQueryHandler<TParameter> : ICommand, IDisposable
        where TParameter : IQueryMessage
    {
        
    }

 

    public abstract class QueryHandler<TParameter> : IQueryHandler<TParameter> 
        where TParameter : IQueryMessage
    {
        public QueryHandler(IRepository repository)
        {
            this.Repository = repository;
        }

        protected abstract Result Retrieve(TParameter query, InvocationContext context);

        public IRepository Repository { get; protected set; }
        public InvocationContext Context { get;set;}

        public IMessage Input
        {
            get;
            set;
        }

        public void Execute()
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            try
            {
                this.Result = Retrieve((TParameter)this.Input, Context);
            }
            catch (Exception ex)
            {
                this.Result = new Result();
                this.Result.Success = false;
                this.Result.Message = ex.ToString();
            }
            finally
            {
                sw.Stop();

                this.Result.ExecuteTime = sw.Elapsed.ToReadableHourString();
            }
        }

        public Result Result
        {
            get;
            protected set;
        }

        public virtual void Dispose()
        {
            if (this.Repository != null)
                this.Repository.Dispose();
        }
    }

    //  Interface for the query dispatcher itself
    //  The query dispacther takes query definition with options 
    //  and context as argument and resturns the result
    public interface IQueryDispatcher
    {
        Result Dispatch<TParameter>(TParameter query, InvocationContext ctx)
            where TParameter : IQueryMessage;

        Task<Result> DispatchAsync<TParameter>(TParameter query, InvocationContext ctx)
            where TParameter : IQueryMessage;
    }




    public class QueryDispatcher : IQueryDispatcher
    {
        IServiceLocator locator;

        public QueryDispatcher(IServiceLocator locator)
        {
            if (locator == null)
            {
                throw new ArgumentNullException("locator");
            }

            this.locator = locator;
        }

        public Result Dispatch<TParameter>(TParameter query, InvocationContext context) where TParameter : IQueryMessage
        {
            Type handlerType = typeof(IQueryHandler<>).MakeGenericType(query.GetType());

            // Find the appropriate handler to call from those registered with Unity based on the type of the command parameter
            var handler = locator.TryGet(handlerType) as ICommand;

            if (handler == null)
                return Result.GetUnknowResult();

            handler.Input = query;

            handler.Context = context;

            handler.Execute();

            return handler.Result;
        }

        public async Task<Result> DispatchAsync<TParameter>(TParameter query, InvocationContext context) where TParameter : IQueryMessage
        {
            Task<Result> output = Task.Factory.StartNew(() =>
            {
                return Dispatch<TParameter>(query, context);
            });

            return await output;
        }

        private bool disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
 
                    
                }
            }
            this.disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
