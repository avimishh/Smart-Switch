using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using BackEnd.Models;
using Autofac;
using Microsoft.EntityFrameworkCore;
using BackEnd.Models.Websockets;

namespace BackEnd
{
    public class Program
    {
        public static IContainer Container { get; set; }

        public static void Main(string[] args)
        {
            ContainerBuilder builder = new ContainerBuilder();
            builder.RegisterType<WebsocketsServer>().As<IWebsocketsServer>().SingleInstance();
            builder.Register(c =>
            {
                var opt = new DbContextOptionsBuilder<SmartSwitchDbContext>();
                opt.UseSqlServer(Startup.SmartSwitchSqlConnectionString);
                return new SmartSwitchDbContext(opt.Options);
            }).AsSelf().InstancePerLifetimeScope();
            Container = builder.Build();

            using (ILifetimeScope scope = Container.BeginLifetimeScope()) scope.Resolve<IWebsocketsServer>().Start();

            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>();
    }
}
