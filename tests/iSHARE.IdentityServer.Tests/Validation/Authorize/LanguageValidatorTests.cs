using iSHARE.IdentityServer.Validation.Authorize;
using Shouldly;
using Xunit;

namespace iSHARE.IdentityServer.Tests.Validation.Authorize
{
    public class LanguageValidatorTests
    {
        [Theory]
        [InlineData("lt", true)]
        [InlineData("aa", true)]
        [InlineData("zu", true)]
        [InlineData("LT", false)]
        [InlineData("LTU", false)]
        public void IsValid_VariousLanguageCodes_ReturnsBool(string languageCode, bool expectedResult)
        {
            var result = LanguageValidator.IsValid(languageCode);

            result.ShouldBe(expectedResult);
        }
    }
}
