﻿using Application.Features.Users.Rules;
using DataAccess.Abstract.Core;
using MimeKit;
using OCK.Core.Mailing;
using System.Reflection;
using System.Text;

namespace Application.Services.EmailService;

public class EmailManager(IUserDal userDal, IMailService mailService) : IEmailService
{
    private static string _forgetPasswordTemplate = string.Empty;

    public async Task SendForgetPassword(string email, string link)
    {
        if (!email.IsValidEmail()) return;

        var user = await userDal.GetAsync(predicate: x => x.Email == email, enableTracking: false);
        await UserRules.UserShouldExistsAndActive(user);

        var emailList = new List<MailboxAddress>
        {
            new(Encoding.UTF8,$"{user.Name} {user.Surname}",user.Email)
        };

        if (_forgetPasswordTemplate.IsEmpty())
        {
            var templatePath = Path.Combine(Directory.GetParent(Assembly.GetExecutingAssembly().Location)!.FullName, "Templates", "ForgetPassword.html");
            _forgetPasswordTemplate = new StreamReader(templatePath).ReadToEnd().HTMLMinifer();
        }

        var emailHtmlBody = _forgetPasswordTemplate.Replace("[[link]]", link.ToLowerInvariant());
        var emailTextBody = $"Şifrenizi yenilemek için bu linke tıklayın. -> {link.ToLowerInvariant()}";

        var emails = new Mail
        {
            ToList = emailList,
            Attachments = new AttachmentCollection(),
            Subject = "Şifre Yenileme",
            TextBody = emailTextBody,
            HtmlBody = emailHtmlBody,
            UnscribeLink = "https://seduss.com"
        };

        await mailService.SendEmailAsync(emails);
    }
}