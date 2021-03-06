﻿using System.Reflection;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Autofac;
using Autofac.Integration.Mvc;
using ENode;
using ENode.Autofac;
using ENode.Infrastructure;
using ENode.JsonNet;
using ENode.Log4Net;
using ENode.Mongo;
using Forum.Domain;
using Forum.Repository;

namespace Forum.Web
{
    public class MvcApplication : HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            AuthConfig.RegisterAuth();

            var eventdbConnectionString = "mongodb://localhost/ForumEventDB";
            var querydbConnectionString = "Data Source=.;Initial Catalog=EventDB;Integrated Security=True;Connect Timeout=30;Min Pool Size=10;Max Pool Size=100";
            var assemblies = new Assembly[] {
                Assembly.Load("Forum.Domain"),
                Assembly.Load("Forum.Repository"),
                Assembly.Load("Forum.Application"),
                Assembly.Load("Forum.Denormalizer"),
                Assembly.Load("Forum.Query"),
                Assembly.GetExecutingAssembly()
            };
            Configuration
                .Create()
                .UseAutofac()
                .RegisterFrameworkComponents()
                .RegisterBusinessComponents(assemblies)
                .UseLog4Net()
                .UseJsonNet()
                .UseMongo(eventdbConnectionString)
                .UseDefaultSqlQueryDbConnectionFactory(querydbConnectionString)
                .CreateAllDefaultProcessors()
                .Initialize(assemblies)
                .Start();

            var container = (ObjectContainer.Current as AutofacObjectContainer).Container;
            RegisterControllers(container);
            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));
        }

        private void RegisterControllers(IContainer container) {
            var containerBuilder = new ContainerBuilder();
            containerBuilder.RegisterControllers(typeof(MvcApplication).Assembly);
            containerBuilder.Update(container);
        }
    }
}