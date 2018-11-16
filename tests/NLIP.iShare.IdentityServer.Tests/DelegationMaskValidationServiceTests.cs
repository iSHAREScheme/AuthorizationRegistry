using Newtonsoft.Json;
using NLIP.iShare.IdentityServer.Delegation;
using NLIP.iShare.Models.DelegationMask;
using Xunit;

namespace NLIP.iShare.IdentityServer.Tests
{
    public class DelegationMaskValidationServiceTests
    {
        [Fact]
        public void Validate_WhenNoRulePermit_ReturnsError()
        {
            //Arrange
            var mask = @"
            {
                'delegationRequest': {
                    'policyIssuer': 'EU.EORI.NL000000003',
                    'target': { 'accessSubject': 'EU.EORI.NL000000001' },
                    'policySets': [
                        {
                            'policies': [
                                {
                                    'target': {
                                        'resource': {
                                            'type': 'CONTAINER.DATA',
                                            'identifiers': [ '180621.ABC123' ],
                                            'attributes': [ 'CONTAINER.DATA.ATTRIBUTE.ETA' ]
                                        },
                                        'actions': ['ISHARE.READ']
                                    },
                                    'rules': [
                                        { 'effect': 'Deny' }
                                    ]
                                }
                            ]
                        }
                    ]
                }
            }";
            var sut = new DelegationMaskValidationService();

            //Act
            var response = sut.Validate(JsonConvert.DeserializeObject<DelegationMask>(mask));

            //Assert
            Assert.False(response.Success);
        }

        [Fact]
        public void Validate_WhenNoRulePermit_ReturnsSuccess()
        {
            //Arrange
            var mask = @"
            {
                'delegationRequest': {
                    'policyIssuer': 'EU.EORI.NL000000003',
                    'target': { 'accessSubject': 'EU.EORI.NL000000001' },
                    'policySets': [
                        {
                            'policies': [
                                {
                                    'target': {
                                        'resource': {
                                            'type': 'CONTAINER.DATA',
                                            'identifiers': [ '180621.ABC123' ],
                                            'attributes': [ 'CONTAINER.DATA.ATTRIBUTE.ETA' ]
                                        },
                                        'actions': ['ISHARE.READ']
                                    },
                                    'rules': [
                                        { 'effect': 'Permit' }
                                    ]
                                }
                            ]
                        }
                    ]
                }
            }";
            var sut = new DelegationMaskValidationService();

            //Act
            var response = sut.Validate(JsonConvert.DeserializeObject<DelegationMask>(mask));

            //Assert
            Assert.True(response.Success);
        }
    }
}
