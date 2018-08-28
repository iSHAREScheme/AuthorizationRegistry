using Shouldly;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace NLIP.iShare.AuthorizationRegistry.Core.Tests
{
    public class AuthorizationRegistryIdGeneratorTests
    {
        [Fact]
        public void GenerateAuthorizationRegistryId_GeneratesDistinctValues()
        {
            //Act
            IList<string> ids = new List<string>();
            for (int i = 0; i < 10000; i++)
            {
                ids.Add(AuthorizationRegistryIdGenerator.New());
            }

            //Assert
            var expected = ids.Distinct().Count() == ids.Count();
            expected.ShouldBe(true);
        }

        [Fact]
        public void GenerateAuthorizationRegistryId_WhenGenerateId_MaximumLengthIs13()
        {
            //Act
            var result = AuthorizationRegistryIdGenerator.New();

            //Assert
            result.Length.ShouldBe(13);
        }
    }
}
