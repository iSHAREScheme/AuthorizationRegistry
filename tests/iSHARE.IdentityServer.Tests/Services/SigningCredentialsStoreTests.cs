using System.Threading.Tasks;
using FluentAssertions;
using iSHARE.IdentityServer.Services;
using iSHARE.Tests.Common;
using Microsoft.IdentityModel.Tokens;
using Moq;
using Xunit;

namespace iSHARE.IdentityServer.Tests.Services
{
    public class SigningCredentialsStoreTests
    {
        [Fact]
        public async Task GetSigningCredentialsAsync_MethodInvoked_ReturnsSigningCredentials()
        {
            var privateKeyVaultMock = new Mock<IPrivateKeyVault>();
            privateKeyVaultMock.Setup(x => x.GetRsaPrivateKey()).ReturnsAsync(TestData.SchemeOwner.PrivateKey);
            var sut = new SigningCredentialsStore(privateKeyVaultMock.Object);

            var result = await sut.GetSigningCredentialsAsync();

            result.Key.Should().NotBeNull();
            result.Algorithm.Should().BeEquivalentTo(SecurityAlgorithms.RsaSha256);
        }
    }
}
