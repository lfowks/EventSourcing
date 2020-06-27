using AutoMapper;
using EventStore.ClientAPI;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddAutoMapper(Assembly.GetExecutingAssembly());
            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
            services.AddMediatR(Assembly.GetExecutingAssembly());
            //services.AddTransient(typeof(IPipelineBehavior<,>), typeof(PerformanceBehaviour<,>));
            //services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
            //services.AddTransient(typeof(IPipelineBehavior<,>), typeof(UnhandledExceptionBehaviour<,>));

            var connectionSettings = ConnectionSettings.Create();
            //var eventStoreConnection = EventStoreConnection.Create(
            //    connectionString: "ConnectTo=tcp://127.0.0.1:1115;DefaultUserCredentials=admin:changeit;UseSslConnection=true;TargetHost=eventstore.org;ValidateServer=false",
            //    builder: connectionSettings,
            //    connectionName: "Task");
            var eventStoreConnection=EventStoreConnection.Create(
              "ConnectTo=tcp://127.0.0.1:1115;DefaultUserCredentials=admin:changeit;UseSslConnection=true;TargetHost=eventstore.org;ValidateServer=false",
              connectionSettings, "Task");

            eventStoreConnection.ConnectAsync().Wait();//Open Connection

            // eventStoreConnection.ConnectAsync().GetAwaiter().GetResult();//Open Connection

            services.AddSingleton(eventStoreConnection);



            return services;
        }
    }
}
