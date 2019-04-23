using System.Threading.Tasks;
using IdentityServer4.Validation;
using iSHARE.IdentityServer.Validation;
using Microsoft.Extensions.Logging;
using Moq;
using Shouldly;
using Xunit;

namespace iSHARE.IdentityServer.Tests
{
    public class AllOrNothingTokenRequestValidatorTests
    {
        [Theory]
        [InlineData(true, true, true)]
        [InlineData(true, false, true)]
        [InlineData(false, false, false)]
        public async Task ValidateAsync_ForDifferentIsErrorCombinations_ExpectedIsError(
                bool firstValidator, bool secondValidator, bool expected)
        {
            //Arrange
            var context = new CustomTokenRequestValidationContext()
            {
                Result = new TokenRequestValidationResult(new ValidatedTokenRequest())
            };
            var validator1Mock = new Mock<ICustomTokenRequestValidator>();
            validator1Mock
                .Setup(c => c.ValidateAsync(It.IsAny<CustomTokenRequestValidationContext>()))
                .Returns(Task.CompletedTask)
                .Callback(() => context.Result.IsError = firstValidator);
            var validator2Mock = new Mock<ICustomTokenRequestValidator>();
            validator2Mock
                .Setup(c => c.ValidateAsync(It.IsAny<CustomTokenRequestValidationContext>()))
                .Returns(Task.CompletedTask)
                .Callback(() => context.Result.IsError = secondValidator);

            var sut = new AllOrNothingTokenRequestValidator(new[]
            {
                validator1Mock.Object,
                validator2Mock.Object
            }, new LoggerFactory().CreateLogger<AllOrNothingTokenRequestValidator>());

            //Act
            await sut.ValidateAsync(context);

            //Assert
            context.Result.IsError.ShouldBe(expected);
        }
    }
}
