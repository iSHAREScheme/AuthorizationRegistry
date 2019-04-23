using System.Collections.Generic;
using System.Security.Claims;
using iSHARE.AuthorizationRegistry.Core.Api;
using iSHARE.AuthorizationRegistry.Core.Models;
using iSHARE.Models;
using Moq;
using Shouldly;
using Xunit;

namespace iSHARE.AuthorizationRegistry.Core.Tests
{
    public class DelegationValidationServiceTests
    {
        [Fact]
        public void ValidateCopy_WhenPartyIdAndAccessSubjectEmpty_ReturnsNotValid()
        {
            //Arrange
            var policyJson = @"
            {
                ""delegationEvidence"":
                {
                    ""notBefore"": 1509633681,
                    ""notOnOrAfter"": 1509633741,
                    ""policyIssuer"": """",
                    ""target"":
                    {
                        ""accessSubject"": """"
                    }
                }
            }";
            var claims = new List<Claim>
            {
                new Claim("partyId", "EU.EORI.NL812972715")
            };
            var identity = new ClaimsIdentity(claims, "TestAuthType");
            var claimsPrincipal = new ClaimsPrincipal(identity);

            var delegationServiceMock = new Mock<IDelegationService>();
            var sut = new DelegationValidationService(delegationServiceMock.Object);

            //Act
            var result = sut.ValidateCopy(policyJson, claimsPrincipal);

            //Assert
            result.Error.ShouldBe("Policy issuer and access subject are required.");
        }

        [Fact]
        public void ValidateCopy_ForNotMatchingPartyIdWithEPRole_ReturnsNotValid()
        {
            //Arrange
            var policyJson = @"
            {
                ""delegationEvidence"":
                {
                    ""notBefore"": 1509633681,
                    ""notOnOrAfter"": 1509633741,
                    ""policyIssuer"": ""EU.EORI.NL812972713"",
                    ""target"":
                    {
                        ""accessSubject"": ""EU.EORI.NL000000001""
                    }
                }
            }";
            var claims = new List<Claim>
            {
                new Claim("partyId", "EU.EORI.NL812972715")
            };
            var identity = new ClaimsIdentity(claims, "TestAuthType");
            var claimsPrincipal = new ClaimsPrincipal(identity);

            var delegationServiceMock = new Mock<IDelegationService>();
            var sut = new DelegationValidationService(delegationServiceMock.Object);

            //Act
            var result = sut.ValidateCopy(policyJson, claimsPrincipal);

            //Assert
            result.Error.ShouldBe("Policy issuer must be equal to your party id.");
        }

        [Fact]
        public void ValidateCopy_ForPolicyIssuerAndAccessSubjectNotEmptyOnMatchinEPRole_ReturnsValid()
        {
            //Arrange
            var policyJson = @"
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
            var claims = new List<Claim>
            {
                new Claim("partyId", "EU.EORI.NL812972715")
            };
            var identity = new ClaimsIdentity(claims, "TestAuthType");
            var claimsPrincipal = new ClaimsPrincipal(identity);

            var delegationServiceMock = new Mock<IDelegationService>();
            var sut = new DelegationValidationService(delegationServiceMock.Object);

            //Act
            var result = sut.ValidateCopy(policyJson, claimsPrincipal);

            //Assert
            result.Success.ShouldBe(true);
        }

        [Fact]
        public void ValidateCreate_WhenPartyIdAndAccessSubjectEmpty_ReturnsNotValid()
        {
            //Arrange
            var policyJson = @"
            {
                ""delegationEvidence"":
                {
                    ""notBefore"": 1509633681,
                    ""notOnOrAfter"": 1509633741,
                    ""policyIssuer"": """",
                    ""target"":
                    {
                        ""accessSubject"": """"
                    }
                }
            }";
            var claims = new List<Claim>
            {
                new Claim("partyId", "EU.EORI.NL812972715")
            };
            var identity = new ClaimsIdentity(claims, "TestAuthType");
            var claimsPrincipal = new ClaimsPrincipal(identity);

            var delegationServiceMock = new Mock<IDelegationService>();
            var sut = new DelegationValidationService(delegationServiceMock.Object);

            //Act
            var result = sut.ValidateCreate(policyJson, claimsPrincipal).Result;

            //Assert
            result.Error.ShouldBe("Policy issuer and access subject are required.");
        }

        [Fact]
        public void ValidateCreate_ForNotMatchingPartyIdWithEPRole_ReturnsNotValid()
        {
            //Arrange
            var policyJson = @"
            {
                ""delegationEvidence"":
                {
                    ""notBefore"": 1509633681,
                    ""notOnOrAfter"": 1509633741,
                    ""policyIssuer"": ""EU.EORI.NL812972713"",
                    ""target"":
                    {
                        ""accessSubject"": ""EU.EORI.NL000000001""
                    }
                }
            }";
            var claims = new List<Claim>
            {
                new Claim("partyId", "EU.EORI.NL812972715")
            };
            var identity = new ClaimsIdentity(claims, "TestAuthType");
            var claimsPrincipal = new ClaimsPrincipal(identity);

            var delegationServiceMock = new Mock<IDelegationService>();
            var sut = new DelegationValidationService(delegationServiceMock.Object);

            //Act
            var result = sut.ValidateCreate(policyJson, claimsPrincipal).Result;

            //Assert
            result.Error.ShouldBe("Policy issuer must be equal to your party id.");
        }

        [Fact]
        public void ValidateCreate_WhenDelegationExists_ReturnsNotValid()
        {
            //Arrange
            var policyJson = @"
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
            var claims = new List<Claim>
            {
                new Claim("partyId", "EU.EORI.NL812972715")
            };
            var identity = new ClaimsIdentity(claims, "TestAuthType");
            var claimsPrincipal = new ClaimsPrincipal(identity);

            var delegationServiceMock = new Mock<IDelegationService>();

            delegationServiceMock
                .Setup(c => c.DelegationExists("EU.EORI.NL812972715", "EU.EORI.NL000000001"))
                .ReturnsAsync(true);

            var sut = new DelegationValidationService(delegationServiceMock.Object);

            //Act
            var result = sut.ValidateCreate(policyJson, claimsPrincipal).Result;

            //Assert
            result.Error.ShouldBe("The combination policyIssuer - accessSubject already exists.");
        }

        [Fact]
        public void ValidateCreate_ForPolicyIssuerAndAccessSubjectNotEmptyOnMatchinEPRole_ReturnsValid()
        {
            //Arrange
            var policyJson = @"
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
            var claims = new List<Claim>
            {
                new Claim("partyId", "EU.EORI.NL812972715")
            };
            var identity = new ClaimsIdentity(claims, "TestAuthType");
            var claimsPrincipal = new ClaimsPrincipal(identity);

            var delegationServiceMock = new Mock<IDelegationService>();
            var sut = new DelegationValidationService(delegationServiceMock.Object);

            //Act
            var result = sut.ValidateCreate(policyJson, claimsPrincipal).Result;

            //Assert
            result.Success.ShouldBe(true);
        }

        [Fact]
        public void ValidateEdit_WhenPolicyIssuerAndAccessSubjectEmpty_ReturnsNotValid()
        {
            //Arrange
            var policyJson = @"
            {
                ""delegationEvidence"":
                {
                    ""notBefore"": 1509633681,
                    ""notOnOrAfter"": 1509633741,
                    ""policyIssuer"": """",
                    ""target"":
                    {
                        ""accessSubject"": """"
                    }
                }
            }";
            var arId = "AR.1234567890";
            var claims = new List<Claim>
            {
                new Claim("partyId", "EU.EORI.NL812972715")
            };
            var identity = new ClaimsIdentity(claims, "TestAuthType");
            var claimsPrincipal = new ClaimsPrincipal(identity);

            var delegationServiceMock = new Mock<IDelegationService>();
            var sut = new DelegationValidationService(delegationServiceMock.Object);

            //Act
            var result = sut.ValidateEdit(arId, policyJson, claimsPrincipal).Result;

            //Assert
            result.Error.ShouldBe("Policy issuer and access subject are required.");
        }

        [Fact]
        public void ValidateEdit_WhenCombinationPolicyIssuerAccessSubjectIsModified_ReturnsNotValid()
        {
            //Arrange
            var policyJson = @"
            {
                ""delegationEvidence"":
                {
                    ""notBefore"": 1509633681,
                    ""notOnOrAfter"": 1509633741,
                    ""policyIssuer"": ""EU.EORI.NL812972711"",
                    ""target"":
                    {
                        ""accessSubject"": ""EU.EORI.NL000000002""
                    }
                }
            }";
            var arId = "AR.1234567890";
            var claims = new List<Claim>
            {
                new Claim("partyId", "EU.EORI.NL812972711")
            };
            var identity = new ClaimsIdentity(claims, "TestAuthType");
            var claimsPrincipal = new ClaimsPrincipal(identity);

            var delegationServiceMock = new Mock<IDelegationService>();
            delegationServiceMock
                .Setup(c => c.GetByPolicyId(arId, claimsPrincipal.GetPartyId()))
                .ReturnsAsync(new Delegation
                {
                    PolicyIssuer = "EU.EORI.NL812972715",
                    AccessSubject = "EU.EORI.NL000000001",
                });

            var sut = new DelegationValidationService(delegationServiceMock.Object);

            //Act
            var result = sut.ValidateEdit(arId, policyJson, claimsPrincipal).Result;

            //Assert
            result.Error.ShouldBe("The combination policyIssuer - accessSubject must remain unmodified.");
        }

        [Fact]
        public void ValidateEdit_ForPolicyIssuerAndAccessSubjectNotModified_ReturnsValid()
        {
            //Arrange
            var policyJson = @"
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
            var arId = "AR.1234567890";
            var identity = new ClaimsIdentity(new List<Claim>(), "TestAuthType");
            var claimsPrincipal = new ClaimsPrincipal(identity);

            var delegationServiceMock = new Mock<IDelegationService>();
            delegationServiceMock
                .Setup(c => c.GetByPolicyId(arId, claimsPrincipal.GetPartyId()))
                .ReturnsAsync(new Delegation
                {
                    PolicyIssuer = "EU.EORI.NL812972715",
                    AccessSubject = "EU.EORI.NL000000001",
                });

            var sut = new DelegationValidationService(delegationServiceMock.Object);

            //Act
            var result = sut.ValidateEdit(arId, policyJson, claimsPrincipal).Result;

            //Assert
            result.Success.ShouldBe(true);
        }
    }
}
