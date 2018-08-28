using NLIP.iShare.Abstractions.Email;
using Shouldly;
using System;
using NLIP.iShare.Abstractions;
using Xunit;

namespace NLIP.iShare.EmailClient.Tests
{
    public class EmailValidatorTests
    {
        [Fact]
        public void NotNullOrEmpty_WhenParameterIsEmpty_ThrowsArgumentNullException()
        {
            //Arrange
            var value = string.Empty;

            //Act
            Action act = () => value.NotNullOrEmpty(nameof(value));

            //Assert
            Should.Throw<ArgumentNullException>(act);
        }

        [Fact]
        public void NotNullOrEmpty_WhenParameterIsNull_ThrowsArgumentNullException()
        {
            //Arrange
            string value = null;

            //Act
            Action act = () => value.NotNullOrEmpty(nameof(value));

            //Assert
            Should.Throw<ArgumentNullException>(act);
        }

        [Fact]
        public void NotNullOrEmpty_WhenTheValueIsValid_DoesNotThrowException()
        {
            //Arrange
            var value = "value";

            //Act
            Action act = () => value.NotNullOrEmpty(nameof(value));

            //Assert
            Should.NotThrow(act);
        }

        [Fact]
        public void CreateNewInstanceOfEmailAddress_WhenTheAddresParameterIsValid_DoesNotThrowException()
        {
            //Arrange
            EmailAddress value;

            //Act
            Action act = () => value = new EmailAddress("example_email@mailinator.com",null);

            //Assert
            Should.NotThrow(act);
        }

        [Fact]
        public void CreateNewInstanceOfEmailAddress_WhenTheAddresParameterIsInvalid_ThrowsArgumentNullException()
        {
            //Arrange
            EmailAddress value;

            //Act
            Action act = () => value = new EmailAddress(null, null);

            //Assert
            Should.Throw<ArgumentNullException>(act);
        }
    }
}
