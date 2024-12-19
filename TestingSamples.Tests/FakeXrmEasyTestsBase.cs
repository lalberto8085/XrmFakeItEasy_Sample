using FakeXrmEasy.Abstractions;
using FakeXrmEasy.Abstractions.Enums;
using FakeXrmEasy.Middleware;
using FakeXrmEasy.Middleware.Crud;
using FakeXrmEasy.Middleware.Messages;
using FakeXrmEasy.Middleware.Pipeline;
using FakeXrmEasy.Plugins;
using Microsoft.Xrm.Sdk;
using System.ComponentModel;
using TestingSamples.Plugins;

namespace TestingSamples.Tests
{
    public class FakeXrmEasyTestsBase
    {
        protected readonly IXrmFakedContext _context;
        protected readonly IOrganizationService _service;

        public FakeXrmEasyTestsBase()
        {
            _context = MiddlewareBuilder
                        .New()

                        // Add middleware
                        .AddCrud()
                        .AddFakeMessageExecutors()
                        .AddPipelineSimulation(new PipelineOptions
                        {
                            UsePluginStepRegistrationValidation = true
                        })

                        .UsePipelineSimulation()
                        .UseMessages()
                        .UseCrud()

                        // Build the context
                        .SetLicense(FakeXrmEasyLicense.NonCommercial)
                        .Build();

            _service = _context.GetOrganizationService();
        }

        protected ILocalPluginContext BuildLocalPluginContext(XrmFakedPluginExecutionContext context)
        {
            var properties = _context.GetPluginContextProperties();
            var serviceProvider = properties.GetServiceProvider(context);

            return new LocalPluginContext(serviceProvider);
        }

        protected ILocalPluginContext BuildLocalPluginContext() => BuildLocalPluginContext(_context.GetDefaultPluginContext());
    }
}