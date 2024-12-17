using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestingSamples.Tests.Utils
{
    public static class EntityAssertionsExtensions
    {
        public static EntityAssertions Should(this Entity entity)
        {
            return new EntityAssertions(entity);
        }
    }
}
