using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;

using Reventuous;
using Reventuous.Subscriptions;
using Reventuous.Redis;
using Reventuous.Subscriptions.Redis;
using StackExchange.Redis;


using Reventuous.Sample.Infrastructure;
using Reventuous.Sample.Application.Reactions;
using Reventuous.Sample.Domain;
using Reventuous.Sample.Application;

namespace Reventuous.Sample
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Reventuous.Sample", Version = "v1" });
            });

            services
                .Configure<SendgridConfiguration>(Configuration.GetSection("Sendgrid"))
                .AddRedisEventStore(Configuration["RedisConnectionString"])
                .AddCustomServices()
                .AddReactions()
            ;
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Reventuous.Sample v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }

    public static class StartupExtensions
    {

        public static IServiceCollection AddCustomServices(
            this IServiceCollection services
        )
        {
            services
                .AddSingleton<AccountService>()
                .AddSingleton<ISendEmailService, SendGridService>();
            return services;
        }

        public static IServiceCollection AddRedisEventStore(
            this IServiceCollection services,
            string redisConnectionString
        )
        {
            EventMapping.MapEventTypes();
            
            ConnectionMultiplexer redis = ConnectionMultiplexer.Connect(redisConnectionString);
            IDatabase db = redis.GetDatabase();
            services.AddSingleton<IDatabase>(db);

            var eventStore = new RedisEventStore(db);
            services.AddSingleton(eventStore);
            var aggregateStore = new AggregateStore(eventStore, DefaultEventSerializer.Instance);
            services.AddSingleton<IAggregateStore>(aggregateStore);
            return services;
        }

        public static IServiceCollection AddReactions(
            this IServiceCollection services)
        {
            services
                .AddSingleton<IHostedService, StreamPersistentSubscription>( provider => {
                    var subscriptionId = "bankaccount.reactions";
                    var stream = "$by-category:account";
                    var consumerId = "consumer1";
                    var loggerFactory = provider.GetLoggerFactory();

                    return new StreamPersistentSubscription(
                        provider.GetRedisDatabase(),
                        stream,
                        subscriptionId,
                        consumerId,
                        new NoOpCheckpointStore(),
                        new IEventHandler[] { 
                            new AccountReactor(
                                subscriptionId, 
                                provider.GetRequiredService<ISendEmailService>()
                            )
                        },
                        DefaultEventSerializer.Instance,
                        loggerFactory
                    );
                });
            return services;
        }

        public static ILoggerFactory GetLoggerFactory(this IServiceProvider provider)
            => provider.GetRequiredService<ILoggerFactory>();
        public static IDatabase GetRedisDatabase(this IServiceProvider provider)
            => provider.GetRequiredService<IDatabase>();
        public static IAggregateStore GetAggregateStore(this IServiceProvider provider)
            => provider.GetRequiredService<IAggregateStore>();

    }
}
