namespace Business.Services.EmailService;

public interface IEmailService : IBusinessService
{
    Task SendForgetPassword(string email, string link);
}