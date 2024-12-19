using FluentAssertions;
using FluentAssertions.Execution;
using FluentAssertions.Primitives;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestingSamples.Tests.Utils
{
    public class EntityAssertions : ReferenceTypeAssertions<Entity, EntityAssertions>
    {
        protected override string Identifier => "entity";

        public EntityAssertions(Entity entity)
            : base(entity)
        {
        }

        public AndConstraint<EntityAssertions> HaveAttribute(string attributeName, string because = "", params object[] becauseArgs)
        {
            AssertEntityNotNull();
            AssertEntityHasAttribute(attributeName, because, becauseArgs);
            return new AndConstraint<EntityAssertions>(this);
        }

        public AndConstraint<EntityAssertions> HaveAttributeWithValue<T>(string attributeName, T expectedValue, string because = "", params object[] becauseArgs)
        {
            AssertEntityNotNull();
            AssertEntityHasAttribute(attributeName, because, becauseArgs);

            Execute.Assertion
                .ForCondition(Subject.GetAttributeValue<T>(attributeName) is T actualValue && actualValue.Equals(expectedValue))
                .BecauseOf(because, becauseArgs)
                .FailWith("Expected entity {context:entity} to have attribute {0} with value {1}{reason}, but it does not.", attributeName, expectedValue);

            return new AndConstraint<EntityAssertions>(this);
        }

        private void AssertEntityNotNull()
        {
            Execute.Assertion
                .ForCondition(Subject != null)
                .FailWith("Expected entity {context:entity} to not be null.");
        }

        private void AssertEntityHasAttribute(string attributeName, string because = "", params object[] becauseArgs)
        {
            Execute.Assertion
                .ForCondition(Subject.Contains(attributeName))
                .BecauseOf(because, becauseArgs)
                .FailWith("Expected entity {context:entity} to have attribute {0}, but it does not.", attributeName);
        }
    }
}
