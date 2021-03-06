﻿using System;
using System.Collections.Generic;
using System.Linq;
using GraphiQl;
using GraphQL.EntityFramework;
using GraphQL;
using GraphQL.Server;
using GraphQL.Types;
using GraphQL.Utilities;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        GraphTypeTypeRegistry.Register<Employee, EmployeeGraph>();
        GraphTypeTypeRegistry.Register<EmployeeSummary, EmployeeSummaryGraph>();
        GraphTypeTypeRegistry.Register<Company, CompanyGraph>();
        EfGraphQLConventions.RegisterInContainer<SampleDbContext>(
            services,
            model: SampleDbContext.StaticModel);
        EfGraphQLConventions.RegisterConnectionTypesInContainer(services);

        foreach (var type in GetGraphQlTypes())
        {
            services.AddSingleton(type);
        }

        var graphQl = services.AddGraphQL(
            options => options.ExposeExceptions = true);
        graphQl.AddWebSockets();

        var dbContextBuilder = new DbContextBuilder();
        services.AddSingleton<IHostedService>(dbContextBuilder);
        services.AddSingleton<Func<SampleDbContext>>(provider => dbContextBuilder.BuildDbContext);
        services.AddScoped(provider => dbContextBuilder.BuildDbContext());
        services.AddSingleton<IDocumentExecuter, EfDocumentExecuter>();
        services.AddSingleton<IDependencyResolver>(
            provider => new FuncDependencyResolver(provider.GetRequiredService!));
        services.AddSingleton<ISchema, Schema>();
        var mvc = services.AddMvc(option => option.EnableEndpointRouting = false);
        mvc.SetCompatibilityVersion(CompatibilityVersion.Latest);
        mvc.AddNewtonsoftJson();
    }

    static IEnumerable<Type> GetGraphQlTypes()
    {
        return typeof(Startup).Assembly
            .GetTypes()
            .Where(x => !x.IsAbstract &&
                        (typeof(IObjectGraphType).IsAssignableFrom(x) ||
                         typeof(IInputObjectGraphType).IsAssignableFrom(x)));
    }

    public void Configure(IApplicationBuilder builder)
    {
        builder.UseWebSockets();
        builder.UseGraphQLWebSockets<ISchema>();
        builder.UseGraphiQl("/graphiql", "/graphql");
        builder.UseMvc();
    }
}