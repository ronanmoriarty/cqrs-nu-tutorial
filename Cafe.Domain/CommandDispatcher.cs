﻿using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace Cafe.Domain
{
    public class CommandDispatcher
    {
        private readonly IEventPublisher _eventPublisher;
        private Dictionary<Type, object> _commandHandlerMappings;

        public CommandDispatcher(IEventPublisher eventPublisher, object[] commandHandlers)
        {
            _eventPublisher = eventPublisher;
            MapCommandTypesToCommandHandlerInstance(commandHandlers);
        }

        public void Dispatch(params object[] commands)
        {
            foreach (var command in commands)
            {
                var commandType = command.GetType();
                var handler = _commandHandlerMappings[commandType];
                var handleMethod = handler.FindMethodTakingSingleArgument(GetHandleMethodName(), commandType);
                try
                {
                    var events = (IEnumerable<IEvent>)handleMethod.Invoke(handler, new[] { command });
                    _eventPublisher.Publish(events);
                }
                catch (TargetInvocationException exception)
                {
                    throw exception.InnerException; // allow any actual exceptions to bubble up, rather than wrapping up the original exception in the reflection-specific TargetInvocationException.
                }
            }
        }

        private string GetHandleMethodName()
        {
            Expression<Action> objectExpression = () => ((ICommandHandler<IEvent>)null).Handle(null); // done this way instead of just returning "Handle" to facilitate any potential future refactoring / renaming.
            return ((MethodCallExpression)objectExpression.Body).Method.Name;
        }

        private void MapCommandTypesToCommandHandlerInstance(object[] commandHandlers)
        {
            _commandHandlerMappings = new Dictionary<Type, object>();
            foreach (var commandHandler in commandHandlers)
            {
                foreach (var interfaceType in commandHandler.GetType().GetInterfaces())
                {
                    if (interfaceType.IsGenericType && interfaceType.GetGenericTypeDefinition() == typeof(ICommandHandler<>))
                    {
                        var commandType = interfaceType.GenericTypeArguments[0];
                        if (_commandHandlerMappings.ContainsKey(commandType))
                        {
                            throw new ArgumentException($"More than one handler found for {commandType.FullName}");
                        }

                        _commandHandlerMappings.Add(commandType, commandHandler);
                    }
                }
            }
        }
    }
}