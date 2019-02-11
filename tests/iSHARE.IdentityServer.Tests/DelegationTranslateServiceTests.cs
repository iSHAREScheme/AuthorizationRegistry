using Newtonsoft.Json;
using iSHARE.IdentityServer.Delegation;
using iSHARE.Models;
using iSHARE.Models.DelegationMask;
using Xunit;

namespace iSHARE.IdentityServer.Tests
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

        [Fact]
        public void Translate_WhenBananaAndCoDelegatesRightsToABCTruckingAndDeniesReadAccessOnAContainerWhenRequestIsAll_ReturnsEffectDeny()
        {
            //Arrange
            var delegationMask = @"DelegationTestCases\case3_mask.json".Read<DelegationMask>();

            var delegationResponse = @"DelegationTestCases\case3_evidence.json".Read<DelegationTranslationTestResponse>();

            var sut = new DelegationTranslateService();

            //Act
            var delegationEvidenceResponse = sut.Translate(delegationMask, @"DelegationTestCases\case3_policy.json".Read());

            //Assert
            Assert.Equal(JsonConvert.SerializeObject(delegationResponse), JsonConvert.SerializeObject(delegationEvidenceResponse));
        }


        [Fact]
        public void Translate_Case4_ReturnsExpected()
        {
            //Arrange
            var delegationMask = @"DelegationTestCases\case4_mask.json".Read<DelegationMask>();

            var delegationResponse = @"DelegationTestCases\case4_evidence.json".Read<DelegationTranslationTestResponse>();

            var sut = new DelegationTranslateService();

            //Act
            var result = sut.Translate(delegationMask, @"DelegationTestCases\case4_policy.json".Read());

            //Assert
            Assert.Equal(JsonConvert.SerializeObject(delegationResponse), JsonConvert.SerializeObject(result));
        }

        [Fact]
        public void Translate_WhenMaskHasMultipleAtreibuteRequestForSameAction_ReturnsExpected()
        {
            //Arrange
            var delegationMask = @"DelegationTestCases\case5_mask.json".Read<DelegationMask>();

            var delegationResponse = @"DelegationTestCases\case5_evidence.json".Read<DelegationTranslationTestResponse>();

            var sut = new DelegationTranslateService();

            //Act
            var result = sut.Translate(delegationMask, @"DelegationTestCases\case5_policy.json".Read());

            //Assert
            Assert.Equal(JsonConvert.SerializeObject(delegationResponse), JsonConvert.SerializeObject(result));
        }
    }
}
