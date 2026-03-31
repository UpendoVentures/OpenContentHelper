using DotNetNuke.Common.Utilities;
using Upendo.OpenContentHelper.Features.Events.Data;
using Upendo.OpenContentHelper.Features.Events.Services;
using System;

namespace Upendo.OpenContentHelper.Features.Events
{
    public static class EventServiceFactory
    {
        public static IEventService Create()
        {
            var connectionString = Config.GetConnectionString();

            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new InvalidOperationException("Unable to resolve the default database connection string.");
            }

            var repository = new SqlEventRepository(connectionString);
            return new EventService(repository);
        }
    }
}