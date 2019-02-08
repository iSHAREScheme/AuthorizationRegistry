using System;
using System.Collections.Generic;
using System.Linq;
using iSHARE.Models.DelegationEvidence;
using Shouldly;
using Xunit;

namespace iSHARE.IdentityServer.Tests
{
    public class PolicyTests
    {
        [Theory]
        [InlineData(PolicyTestData.Containers.Star, PolicyTestData.Containers.Allowed, true)]
        [InlineData(PolicyTestData.Containers.Allowed, PolicyTestData.Containers.Denied, false)]
        public void HasIdentifier_ReturnsAsExpected(string source, string identifier, bool expected)
        {
            //Arrange
            var sut = new PolicyTargetResource
            {
                Identifiers = new List<string> { source }
            };

            //Act
            var result = sut.HasIdentifier(identifier);

            //Assert
            result.ShouldBe(expected);
        }

        [Theory]
        [InlineData(new[] { PolicyTestData.Containers.Star }, new[] { PolicyTestData.Containers.Allowed }, true)]
        [InlineData(new[] { "180621.ABC125", "180621.ABC126", "180621.ABC127" }, new[] { "180621.ABC125", "180621.ABC126", "180621.ABC128" }, true)]
        [InlineData(new[] { "180621.ABC125", "180621.ABC126", "180621.ABC127" }, new[] { "180621.ABC128" }, false)]
        [InlineData(new[] { "180621.ABC125", "180621.ABC126", "180621.ABC127" }, new[] { "180621.ABC128", "180621.ABC129" }, false)]
        [InlineData(new string[] { }, new[] { "180621.ABC128", "180621.ABC129" }, false)]
        public void HasAnyIdentifiers_ReturnsAsExpected(IEnumerable<string> source, IEnumerable<string> identifiers, bool expected)
        {
            //Arrange
            var sut = new PolicyTargetResource
            {
                Identifiers = source?.ToList()
            };

            //Act
            var result = sut.HasAnyIdentifiers(identifiers.ToList());

            //Assert
            result.ShouldBe(expected);
        }

        [Fact]
        public void HasAnyIdentifiers_WhenSourceIsNull_ThrowsArgumentNullExceptions()
        {
            //Arrange
            var sut = new PolicyTargetResource
            {
                Identifiers = null
            };

            //Act
            Action act = () => sut.HasAnyIdentifiers(new[] { PolicyTestData.Containers.Allowed });

            //Assert
            act.ShouldThrow<ArgumentNullException>();
        }
    }
}
