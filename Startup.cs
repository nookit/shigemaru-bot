using System.Collections.Concurrent;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Bot.Schema;
using Microsoft.Bot.Builder.Azure;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Integration.AspNet.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Uls.Shigemaru.Bots;
using Uls.Shigemaru.Dialog;

namespace Uls.Shigemaru
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            services.AddSingleton<IBotFrameworkHttpAdapter, AdapterWithErrorHandler>();

            services.AddSingleton<IBotServices, BotServices>();

            services.AddSingleton<IStorage, MemoryStorage>();

            services.AddSingleton<UserState>();

            services.AddSingleton<ConversationState>();

            services.AddSingleton<RootDialog>();

            services.AddTransient<IBot, QnABot<RootDialog>>();

            string storageAccount = Configuration.GetValue<string>("StorageAccount");
            string storageContainerName = Configuration.GetValue<string>("StorageContainerName");
            AzureBlobStorage storage = new AzureBlobStorage(storageAccount, storageContainerName);
            services.AddSingleton<IStorage>(storage);

            services.AddSingleton<ConversationState>();

            services.AddSingleton<ConcurrentDictionary<string, ConversationReference>>();

            services.AddBot<QnABot<RootDialog>>(options => {
                options.Middleware.Add(new ContextLogger());

            });

            services.AddSingleton<NotificationDialog>();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseDefaultFiles();
            app.UseStaticFiles();

            app.UseWebSockets();
            app.UseMvc();
        }
    }
}
