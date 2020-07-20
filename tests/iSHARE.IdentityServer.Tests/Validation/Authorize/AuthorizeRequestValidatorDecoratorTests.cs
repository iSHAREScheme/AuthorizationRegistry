using System;
using System.Collections.Specialized;
using System.Security.Claims;
using System.Threading.Tasks;
using FluentAssertions;
using IdentityModel;
using IdentityServer4.Validation;
using iSHARE.Configuration;
using iSHARE.IdentityServer.Validation.Authorize;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace iSHARE.IdentityServer.Tests.Validation.Authorize
{
    public class AuthorizeRequestValidatorDecoratorTests
    {
        private readonly Mock<IAuthorizeRequestValidator> _decoratedMock;
        private readonly IAuthorizeRequestValidator _sut;

        public AuthorizeRequestValidatorDecoratorTests()
        {
            _decoratedMock = new Mock<IAuthorizeRequestValidator>();
            var innerValidatorMock = new Mock<Decorator<IAuthorizeRequestValidator>>(_decoratedMock.Object);
            var loggerMock = new Mock<ILogger<AuthorizeRequestValidatorDecorator>>();

            _sut = new AuthorizeRequestValidatorDecorator(innerValidatorMock.Object, loggerMock.Object);
        }

        [Fact]
        public void ValidateAsync_ParametersEqNull_Throws()
        {
            NameValueCollection parameters = null;

            Func<Task> act = async () => await _sut.ValidateAsync(parameters);

            act.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public async Task ValidateAsync_ScopeEqNull_ReturnsError()
        {
            var parameters = new NameValueCollection();

            var result = await _sut.ValidateAsync(parameters);

            var expected = CreateExpectedResult(parameters, "scope is missing");
            result.Should().BeEquivalentTo(expected);
        }

        [Theory]
        [InlineData("openid")]
        [InlineData("openid ishare")]
        [InlineData("openidiSHARE")]
        [InlineData("openId iSHARE")]
        [InlineData("iSHARE")]
        [InlineData("no scope 360")]
        public async Task ValidateAsync_ContainsInvalidScope_ReturnsError(string scope)
        {
            var parameters = new NameValueCollection(1) { { "scope", scope } };

            var result = await _sut.ValidateAsync(parameters);

            var expected = CreateExpectedResult(parameters, "Invalid scope");
            result.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public async Task ValidateAsync_ResponseTypeEqNull_ReturnsError()
        {
            var parameters = new NameValueCollection(1) { { "scope", "iSHARE openid" } };

            var result = await _sut.ValidateAsync(parameters);

            var expected = CreateExpectedResult(parameters, "response_type is missing");
            result.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public async Task ValidateAsync_ResponseTypeNeqCode_ReturnsError()
        {
            var parameters = new NameValueCollection(2)
            {
                {"scope", "iSHARE openid"},
                {"response_type", "decode"}
            };

            var result = await _sut.ValidateAsync(parameters);

            var expected = CreateExpectedResult(parameters, "Invalid response_type");
            result.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public async Task ValidateAsync_Valid_InvokesInnerValidator()
        {
            var parameters = new NameValueCollection(2)
            {
                {"scope", "iSHARE openid"},
                {"response_type", "code"}
            };

            await _sut.ValidateAsync(parameters);

            _decoratedMock.Verify(
                x => x.ValidateAsync(It.IsAny<NameValueCollection>(), It.IsAny<ClaimsPrincipal>()));
        }

        [Fact]
        public async Task ValidateAsync_ClientIdIsSpa_InvokesInnerValidator()
        {
            var parameters = new NameValueCollection(1) { { "client_id", "SPA" } };

            await _sut.ValidateAsync(parameters);

            _decoratedMock.Verify(
                x => x.ValidateAsync(It.IsAny<NameValueCollection>(), It.IsAny<ClaimsPrincipal>()), Times.Once);
        }

        private static AuthorizeRequestValidationResult CreateExpectedResult(
            NameValueCollection parameters,
            string description)
        {
            var request = new ValidatedAuthorizeRequest { Raw = parameters };

            return new AuthorizeRequestValidationResult(
                request,
                OidcConstants.AuthorizeErrors.InvalidRequest,
                description);
        }
    }
}
