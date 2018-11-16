using NLIP.iShare.IdentityServer.Delegation;
using Shouldly;
using Xunit;

namespace NLIP.iShare.IdentityServer.Tests
{
    public class DelegationPolicyJsonParserTests
    {
        [Fact]
        public void PolicyIssuer_WithValidPolicyIssuer_ReturnsPolicyIssuer()
        {
            //Arrange
            var policyString = @"
            {
                ""delegationEvidence"":
                {
                    ""notBefore"": 1509633681,
                    ""notOnOrAfter"": 1509633741,
                    ""policyIssuer"": ""EU.EORI.NL812972715"",
                    ""target"":
                    {
                        ""accessSubject"": ""EU.EORI.NL000000001""
                    }
                }
            }";

            //Act
            var policyJson = new DelegationPolicyJsonParser(policyString);
            var policyIssuer = policyJson.PolicyIssuer;

            //Assert
            policyIssuer.ShouldBe("EU.EORI.NL812972715");
        }

        [Fact]
        public void AccessSubject_WithValidAccessSubject_ReturnsAccessSubject()
        {
            //Arrange
            var policyString = @"
            {
                ""delegationEvidence"":
                {
                    ""notBefore"": 1509633681,
                    ""notOnOrAfter"": 1509633741,
                    ""policyIssuer"": ""EU.EORI.NL812972715"",
                    ""target"":
                    {
                        ""accessSubject"": ""EU.EORI.NL000000001""
                    }
                }
            }";

            //Act
            var policyJson = new DelegationPolicyJsonParser(policyString);
            var accessSubject = policyJson.AccessSubject;

            //Assert
            accessSubject.ShouldBe("EU.EORI.NL000000001");
        }

        [Fact]        
        public void DelegationPolicyJsonParser_ForEmptyJson_ThrowsDelegationPolicyFormatException()
        {
            //Arrange
            var policyString = @"
            {
    
            }";

            //Act

            //Assert
            Assert.Throws<DelegationPolicyFormatException>(
                () => new DelegationPolicyJsonParser(policyString));
        }

        [Fact]
        public void DelegationPolicyJsonParser_ForInvalidJson_ThrowsDelegationPolicyFormatException()
        {
            //Arrange
            var policyString = @"
            """"
            ";

            //Act

            //Assert
            Assert.Throws<DelegationPolicyFormatException>(
                () => new DelegationPolicyJsonParser(policyString));
        }

        [Fact]
        public void PolicyIssuer_ForPolicyIssuerMissing_ReturnsNull()
        {
            //Arrange
            var policyString = @"
            {
                ""delegationEvidence"":
                {
                    ""notBefore"": 1509633681,
                    ""notOnOrAfter"": 1509633741,
                    ""target"":
                    {
                        ""accessSubject"": ""EU.EORI.NL000000001""
                    }     
                }
            }";

            //Act
            var policyJson = new DelegationPolicyJsonParser(policyString);

            //Assert
            policyJson.PolicyIssuer.ShouldBe(null);
        }

        [Fact]
        public void AccessSubject_ForAccessSubjectMissing_ReturnsNull()
        {
            //Arrange
            var policyString = @"
            {
                ""delegationEvidence"":
                {
                    ""notBefore"": 1509633681,
                    ""notOnOrAfter"": 1509633741,
                    ""policyIssuer"": ""EU.EORI.NL812972715"",
                    ""target"":
                    {
            
                    }     
                }
            }";

            //Act
            var policyJson = new DelegationPolicyJsonParser(policyString);

            //Assert
            policyJson.AccessSubject.ShouldBe(null);
        }
    }
}
