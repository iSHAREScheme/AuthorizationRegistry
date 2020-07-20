using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using FluentAssertions;
using IdentityModel;
using iSHARE.Abstractions;
using iSHARE.IdentityServer.Helpers.Interfaces;
using iSHARE.IdentityServer.Validation;
using iSHARE.IdentityServer.Validation.Interfaces;
using iSHARE.Tests.Common;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Moq;
using Xunit;

namespace iSHARE.IdentityServer.Tests.Validation
{
    public class DefaultJwtValidatorTests
    {
        private const string ClientId = TestData.Warehouse13.ClientId;
        private const string PrivateKey = TestData.Warehouse13.PrivateKey;
        private const string PublicKey = TestData.Warehouse13.PublicKey;

        private readonly Mock<ILogger<DefaultJwtValidator>> _loggerMock;
        private readonly Mock<IKeysExtractor> _keysExtractorMock;
        private readonly IDefaultJwtValidator _sut;

        public DefaultJwtValidatorTests()
        {
            _loggerMock = new Mock<ILogger<DefaultJwtValidator>>();
            _keysExtractorMock = new Mock<IKeysExtractor>();

            _keysExtractorMock
                .Setup(x => x.ExtractSecurityKeys(It.IsAny<string>()))
                .Returns(new List<SecurityKey> { new X509SecurityKey(PublicKey.ConvertToX509Certificate2()) });

            _sut = new DefaultJwtValidator(_loggerMock.Object, _keysExtractorMock.Object);
        }

        [Theory]
        [InlineData("PS256")]
        [InlineData("RS384")]
        [InlineData("RS512")]
        public void IsValid_JwtHeaderAlgInvalid_ReturnsFalse(string signingAlgorithm)
        {
            var jwt = CreateClientAssertionJwt(signingAlgorithm);

            var result = _sut.IsValid(jwt, ClientId, TestData.SchemeOwner.ClientId);

            result.Should().BeFalse();
        }

        private static string CreateClientAssertionJwt(string signingAlgorithm)
        {
            var now = DateTime.UtcNow;

            var token = new JwtSecurityToken(
                ClientId,
                ClientId,
                new[]
                {
                    new Claim(JwtClaimTypes.Subject, ClientId),
                    new Claim(JwtClaimTypes.Audience, TestData.SchemeOwner.ClientId),
                    new Claim(JwtClaimTypes.JwtId, Guid.NewGuid().ToString()),
                    new Claim(JwtClaimTypes.IssuedAt, now.ToEpoch()),
                    new Claim(JwtClaimTypes.Expiration, now.AddSeconds(30).ToEpoch())
                },
                now,
                now.AddSeconds(30),
                JwtStringFactory.CreateSigningCredentials(PrivateKey, signingAlgorithm)
            ).AddPublicKeyHeader(PublicKey);

            return JwtStringFactory.CreateToken(token);
        }
    }
}
