using Newtonsoft.Json;
using NLIP.iShare.IdentityServer.Delegation;
using NLIP.iShare.Models;
using NLIP.iShare.Models.DelegationMask;
using Xunit;

namespace NLIP.iShare.IdentityServer.Tests
{
    public class DelegationTranslateServiceTests
    {
        [Fact]
        public void Translate_WhenWarehouse13DelegatesRightsToABCTruckingAndAllowsReadAccessToETAOfAllContainers_ReturnsEffectPermit()
        {
            //Arrange
            var delegationMask = @"DelegationTestCases\case1_mask.json".Read<DelegationMask>();

            var delegationResponse = @"DelegationTestCases\case1_evidence.json".Read<DelegationTranslationTestResponse>();

            var sut = new DelegationTranslateService();

            //Act
            var delegationEvidenceResponse = sut.Translate(delegationMask, @"DelegationTestCases\case1_policy.json".Read());

            //Assert
            Assert.Equal(JsonConvert.SerializeObject(delegationResponse), JsonConvert.SerializeObject(delegationEvidenceResponse));
        }

        [Fact]
        public void Translate_WhenWarehouse13DelegatesRightsToABCTruckingAndDeniesReadAccessToETAOfSomeContainers_ReturnsEffectDeny()
        {
            //Arrange
            var delegationMask = @"DelegationTestCases\case2_mask1.json".Read<DelegationMask>();

            var delegationResponse = @"DelegationTestCases\case2_evidence1.json".Read<DelegationTranslationTestResponse>();

            var sut = new DelegationTranslateService();

            //Act
            var delegationEvidenceResponse = sut.Translate(delegationMask, @"DelegationTestCases\case2_policy.json".Read());

            //Assert
            Assert.Equal(JsonConvert.SerializeObject(delegationResponse), JsonConvert.SerializeObject(delegationEvidenceResponse));
        }

        [Fact]
        public void Translate_WhenWarehouse13DelegatesRightsToABCTruckingAndDeniesReadAccessToETAOfSomeContainers_ReturnsEffectPermit()
        {
            //Arrange
            var delegationMask = @"DelegationTestCases\case2_mask2.json".Read<DelegationMask>();

            var delegationResponse = @"DelegationTestCases\case2_evidence2.json".Read<DelegationTranslationTestResponse>();

            var sut = new DelegationTranslateService();

            //Act
            var delegationEvidenceResponse = sut.Translate(delegationMask, @"DelegationTestCases\case2_policy.json".Read());

            //Assert
            Assert.Equal(JsonConvert.SerializeObject(delegationResponse), JsonConvert.SerializeObject(delegationEvidenceResponse));
        }
    }
}
