using FluentAssertions;
using System;
using System.Linq;
using Xunit;

namespace TestingSamples.Tests
{
    public class SanityTests
    {
        [Fact]
        public void SanityCheck()
        {
        }

        [Fact]
        public void AssertionCheck()
        {
            Assert.True(true);
        }

        [Fact]
        public void FluentAssertionCheck()
        {
            var numbers = new[] { 1, 2, 3, 4, 5 };
            numbers.Should().Contain(3);
        }
    }
}
