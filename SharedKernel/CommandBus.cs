using Microsoft.Practices.ServiceLocation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedKernel
{
    public class CommandBus : IDisposable, ICommandBus
    {
        IEventStore _store = null;
        ICommandDispatcher _commandDispatcher = null;


        public CommandBus(IEventStore store, ICommandDispatcher cmmandDispatcher)
        {
            this._store = store;

            this._commandDispatcher = cmmandDispatcher;
            
        }


   
        public Result Send(ICommandMessage command, InvocationContext context)
        {
            Result result = command.StopwatchWraper((x) =>
            {
                Result output = new Result(command);

                IEnumerable<ValidationResult> vResults = new List<ValidationResult>();

                output.Success = this.IsValid(command, out vResults);//validate the command before submitting

                output.ValidationResults = vResults;

                if (output.Success)
                {
                    CommandOptions option = context.Options as CommandOptions;

                    //hook the right command handler with command message and execute the command operation
                    if (option != null && option.IsAsync)
                    {
                        var task = Task.Factory.StartNew(() => this.Dispach(command, context));
                    }
                    else
                    {
                        output = this.Dispach(command, context);
                    }
                }

                return output;
            });

            return result;
        }

        public void Replay(IDomainEvent change)
        {
            if (change == null)
                return;

            Task.Factory.StartNew(() => OnEventReceved(change, null));
        }

        public void Publish(IDomainEvent change, InvocationContext context)
        {
            if (change == null)
                return;

            if (_store != null) 
                _store.SaveEvents(change);

            Task.Factory.StartNew(() => OnEventReceved(change, context));
        }



        private void OnEventReceved(IDomainEvent change, InvocationContext context)
        {
            Type handlerType = typeof(IEventHandler<>).MakeGenericType(change.GetType());

            IEvent handler = ServiceLocator.Current.TryGet(handlerType) as IEvent;

            if (handler == null)
                return;

            handler.Input = change;

            handler.Execute();
        }

        public IEnumerable<ValidationResult> Validate(ICommandMessage command)
        {
            List<ValidationResult> output = new List<ValidationResult>();

            Type t = command.GetType();

            var handlers = ServiceLocator.Current.GetAllInstances(t);

            if (handlers != null)
            {
                foreach (var h in handlers)
                {
                    var x = h as IValidationHandler<ICommandMessage>;

                    if (x != null)
                    {
                        var result = x.Validate(command);

                        if (result != null && result.Count() > 0)
                            output.AddRange(result);
                    }
                }
            }

            return output;
        }

        protected bool IsValid(ICommandMessage msg, out IEnumerable<ValidationResult> output)
        {
            output = new List<ValidationResult>();

            output = this.Validate(msg);

            return output == null || output.Count() == 0;
        }


        protected Result Dispach(ICommandMessage message, InvocationContext context)
        {
           Result output = _commandDispatcher.Dispatch<ICommandMessage>(message, context);

           if (output != null)
                PublishChanges(output.Data as AggregateRoot, message, context);

           return output;
        }

        protected void PublishChanges(AggregateRoot aggregateRoot, ICommandMessage message, InvocationContext context)
        {
            if (aggregateRoot == null)
                return;

            if (aggregateRoot.HasChanges())
            {
                foreach (var change in aggregateRoot.Changes)
                {
                    if (change.CorrelationId == aggregateRoot.CorrelationId)
                        change.AggregateRootID = aggregateRoot.Id;

                    change.SourceName = this.GetType().Name;
                    change.Payload = message;

                    this.Publish(change, context);
                }
            }
        }

        public void Dispose()
        {
            if (_store != null)
                _store.Dispose();

            if (_commandDispatcher!=null)
                _commandDispatcher.Dispose();
        }
    }

    public class InvocationContext
    {
        public string ParticipantId { get; set; }

        public OperationOptions Options { get;set;}
 
    }


}
