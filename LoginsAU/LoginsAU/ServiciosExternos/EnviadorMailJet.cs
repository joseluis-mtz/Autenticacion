using Microsoft.AspNetCore.Identity.UI.Services;

namespace LoginsAU.ServiciosExternos
{
    public class EnviadorMailJet : IEmailSender
    {
        private readonly IConfiguration _config;
        public OpcionesMailJet _opcionesMailJet;

        public EnviadorMailJet(IConfiguration config)
        {
            _config = config;
        }

        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            _opcionesMailJet = _config.GetSection("MailJet").Get<OpcionesMailJet>();
            throw new NotImplementedException();
            // Agregar codigo de MailJet para enviar correo
        }
    }
}
