using CleanArchitectureBase.Domain.Enums;
using FluentAssertions;
using NUnit.Framework;

namespace CleanArchitectureBase.Domain.UnitTests.ValueObjects
{
    public class EntityExtendedAttributeTypeTests
    {
        [Test]
        public void ShouldHaveCorrectMembers()
        {
            typeof(EntityExtendedAttributeType).GetFields().Should().HaveCount(5);
        }
    }
}
