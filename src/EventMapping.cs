using System;
using Reventuous;
using static Reventuous.Sample.Domain.Events;

namespace Reventuous.Sample
{
    internal static class EventMapping
    {
        public static void MapEventTypes()
        {
            TypeMap.AddType<V1.AccountCreated>("AccountCreated");
            TypeMap.AddType<V1.AccountCredited>("AccountCredited");
            TypeMap.AddType<V1.AccountDebited>("AccountDebited");
        }
    }
}