using FakeXrmEasy.Abstractions;
using FakeXrmEasy.Abstractions.Enums;
using FakeXrmEasy.Middleware;
using FakeXrmEasy.Middleware.Crud;
using FakeXrmEasy.Middleware.Messages;
using FakeXrmEasy.Middleware.Pipeline;
using Microsoft.Xrm.Sdk;

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
                        .AddPipelineSimulation()

                        // Build the context
                        .UsePipelineSimulation()
                        .UseCrud()
                        .UseMessages()

                        .SetLicense(FakeXrmEasyLicense.NonCommercial)
                        .Build();

            _service = _context.GetOrganizationService();
        }
    }
}