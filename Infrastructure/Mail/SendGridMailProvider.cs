using Domain.Interfaces;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Threading.Tasks;

namespace Infrastructure.Mail
{
    public class SendGridMailProvider : ISendMail<SendGridMailProvider>
    {
        private SendGridClient _sendGridClient;
        private SendGridMessage _sendGridMessage;
        private bool _isBodyHTML = true;
        private List<string> _emailsAdicionados = new List<string>();

        public SendGridMailProvider Prepare(string connectionString)
        {//"emailFrom;nomeFrom;hostSendGrid;version;apiToken"
            try
            {
                var configs = connectionString.Split(';');
                if (configs.Length != 5)
                    throw new Exception("Erro no formato da ConnectionString do SendGrid");
                _sendGridClient = new SendGridClient(configs[4], configs[2], null, configs[3]);
                _sendGridMessage = new SendGridMessage()
                {
                    From = new EmailAddress(configs[0], configs[1])
                };
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return this;
        }

        public SendGridMailProvider From(MailAddress from)
        {
            _sendGridMessage.From = new EmailAddress(from.Address, from.DisplayName);
            return this;
        }

        public SendGridMailProvider AddTo(MailAddress to)
        {
            if (!EhEmailDuplicado(to.Address))
                _sendGridMessage.AddTo(MailAdressToEmailAdress(to));
            return this;
        }

        public SendGridMailProvider AddTos(List<MailAddress> tos)
        {
            foreach (var email in tos)
                AddTo(email);
            return this;
        }

        public SendGridMailProvider AddCC(MailAddress cc)
        {
            if (!EhEmailDuplicado(cc.Address))
                _sendGridMessage.AddCc(MailAdressToEmailAdress(cc));
            return this;
        }

        public SendGridMailProvider AddCCs(List<MailAddress> ccs)
        {
            foreach (var email in ccs)
                AddCC(email);
            return this;
        }

        public SendGridMailProvider AddBCC(MailAddress bcc)
        {
            if (!EhEmailDuplicado(bcc.Address))
                _sendGridMessage.AddBcc(MailAdressToEmailAdress(bcc));
            return this;
        }

        public SendGridMailProvider AddBCCs(List<MailAddress> bccs)
        {
            foreach (var email in bccs)
                AddBCC(email);
            return this;
        }

        public SendGridMailProvider Subject(string subject)
        {
            _sendGridMessage.Subject = subject;
            return this;
        }

        public SendGridMailProvider IsBodyHtml(bool isHTML)
        {
            _isBodyHTML = isHTML;
            return this;
        }

        public SendGridMailProvider Body(string body)
        {
            if (_isBodyHTML)
                _sendGridMessage.HtmlContent = body;
            else
                _sendGridMessage.PlainTextContent = body;
            return this;
        }

        public async Task<bool> Send()
        {
            var response = await _sendGridClient.SendEmailAsync(_sendGridMessage);
            return response.IsSuccessStatusCode;
        }

        private EmailAddress MailAdressToEmailAdress(MailAddress mail)
        {
            var email = new EmailAddress(mail.Address, mail.DisplayName);
            if (string.IsNullOrEmpty(mail.DisplayName))
                email.Name = mail.Address.Split('@')[0];
            return email;
        }

        private bool EhEmailDuplicado(string email)
        {
            email = email.ToLower();
            var contem = _emailsAdicionados.Contains(email);
            if (!contem)
                _emailsAdicionados.Add(email);
            return contem;
        }
    }
}
