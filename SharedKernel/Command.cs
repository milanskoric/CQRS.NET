using Microsoft.Practices.ServiceLocation;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedKernel
{
    // Marker interface to signify a command - all command will implement this
    public interface ICommandMessage : IMessage
    {
        
    }

    //The interface ICommandMessage is now just using for IoC purpose to hook 
    //corresponding Command Handler object.


    [Serializable]
    public abstract class CommandMessage : ICommandMessage
    {
        public CommandMessage()
        {
            this.TimeStamp = TimeProvider.Now();
            this.CorrelationId = IdentityGenerator.GenerateUniqueTickIdentifer();
        }

        public DateTime TimeStamp { get; protected set; }

        public string CorrelationId { get; protected set;}
    }

    public interface IValidationHandler<in TParameter> where TParameter : ICommandMessage
    {
        IEnumerable<ValidationResult> Validate(TParameter command);

    }

    // Interface for command handlers - has a type parameters for the command
    public interface ICommandHandler<in TParameter> : ICommand , IDisposable
        where TParameter : ICommandMessage
    {
        
    }

    public interface ICommand
    {
        InvocationContext Context { get; set; }

        IMessage Input { get; set; }

        void Execute();

        Result Result { get; }
    }
 

    public abstract class CommandHandler<TParameter> : ICommandHandler<TParameter> 
        where TParameter : ICommandMessage
    {
        public IUnitOfWork UnitOfWork { get; protected set; }

        public CommandHandler()
        {
            this.Result = new Result();
        }

        public CommandHandler(IUnitOfWork repository)
            : this()
        {
            this.UnitOfWork = repository;
        }

        public Result Result { get; protected set; }

        public IMessage Input { get; set; }

        public InvocationContext Context { get; set; }

        protected abstract void Handle(TParameter cmd, InvocationContext context);

        public void Execute()
        {
            this.Result = Action((TParameter)this.Input, this.Context);
        }

        protected virtual Result Action(TParameter command, InvocationContext context)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();

            Result output = new Result();
            output.Success = false;
            try
            {
                this.Handle(command, context);

                output = this.Result;
            }
            catch (Exception ex)
            {
                OnError(ex);

                output.Message = ex.ToString();
            }
            finally
            {
                sw.Stop();

                output.ExecuteTime = sw.Elapsed.ToReadableHourString();
            }

            return output;
        }

        protected virtual void OnError(Exception ex)
        {
             
        }

        public virtual void Dispose()
        {
            if (this.UnitOfWork != null)
                this.UnitOfWork.Dispose();
        }
    }

 

    // Describes the result of a validation of a potential change through a business service
    public class ValidationResult : Result
    {
        public ValidationResult(int code, string target, string message, string correlationId)
        {
            this.Code = code;
            this.Target = target;
            this.ObjectId = correlationId;
            this.Success = false;
        }

        // Gets or sets the code of the validation error
        public int Code { get; set; }

        // Gets or sets the name of the target member 
        public string Target { get; set; }
    }
 

    // Interface for the command dispatcher itself
    public interface ICommandDispatcher : IDisposable
    {
        Result Dispatch<TParameter>(TParameter command, InvocationContext context) where TParameter : ICommandMessage;
    }

    // Implementation of the command dispatcher - selects and executes the appropriate command 
    public class CommandDispatcher : ICommandDispatcher
    {
        IServiceLocator locator;

        public CommandDispatcher(IServiceLocator locator)
        {
            if (locator == null)
            {
                throw new ArgumentNullException("locator");
            }

            this.locator = locator;
        }
 

        public Result Dispatch<TParameter>(TParameter command, InvocationContext context) where TParameter : ICommandMessage
        {
            Type handlerType = typeof(ICommandHandler<>).MakeGenericType(command.GetType());

            // Find the appropriate handler to call from those registered with Unity based on the type of the command parameter
            var handler = locator.TryGet(handlerType) as ICommand;

            if (handler == null)
                return Result.GetUnknowResult();

            handler.Context = context;

            handler.Input = command;

            handler.Execute();

            return handler.Result;

        }


        public void Dispose()
        {
             
        }
    }


}
