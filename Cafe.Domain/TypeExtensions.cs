using System;
using System.Linq;
using CQRSTutorial.Core;

namespace Cafe.Domain
{
    public static class TypeExtensions
    {
        public static bool CanHandle(this Type type, ICommand command, int id)
        {
            var canHandleThisTypeOfCommand = type.GetInterfaces()
                .Any(interfaceType =>
                    interfaceType.IsGenericType
                    && interfaceType.GetGenericTypeDefinition() == typeof(ICommandHandler<>)
                    && interfaceType.GenericTypeArguments.Single() == command.GetType());

            return canHandleThisTypeOfCommand && ((ICommandWithAggregateId)command).AggregateId == id;
        }
    }
}