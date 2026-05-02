using GodsEye.API.Features.Camera;

namespace GodsEye.Tests.Validators
{
    public class CreateNotificationGroupValidatorTests
    {
        private readonly CreateNotificationGroupValidator _validator = new();

        [Fact]
        public async Task QuandoNameVazio_DeveRetornarErroDeValidacao()
        {
            var request = new CreateNotificationGroupRequest("", ["email@teste.com"]);
            var result = await _validator.ValidateAsync(request);
            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.PropertyName == "name");
        }

        [Fact]
        public async Task QuandoEmailsVazio_DeveRetornarErroDeValidacao()
        {
            var request = new CreateNotificationGroupRequest("Grupo", []);
            var result = await _validator.ValidateAsync(request);
            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.PropertyName == "emails");
        }

        [Fact]
        public async Task QuandoDadosValidos_DevePassarNaValidacao()
        {
            var request = new CreateNotificationGroupRequest("Grupo", ["email@test.com"]);
            var result = await _validator.ValidateAsync(request);
            Assert.True(result.IsValid);
        }
    }
}
