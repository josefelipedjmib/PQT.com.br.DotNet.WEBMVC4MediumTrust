using Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace Infrastructure.Mail
{
    public class SMTPMailProvider : ISendMail<SMTPMailProvider>
    {
        private readonly MailMessage _email;
        private readonly SmtpClient _clientSMTP;

        public SMTPMailProvider()
        {
            _email = new MailMessage();
            _clientSMTP = new SmtpClient();
        }

        public SMTPMailProvider Prepare(string connectionString)
        {//"emailFrom;nomeFrom;hostSMTP;true;587;user|password|domain"
            try
            {
                var configs = connectionString.Split(';');
                if (configs.Length != 6)
                    throw new Exception("Erro no formato da ConnectionString do EmailSMTP");
                var porta = 0;
                int.TryParse(configs[4], out porta);
                var enableSSL = configs[3].ToLower() == "true" ? true : false;
                _email.From = new MailAddress(configs[0], configs[1]);
                _clientSMTP.Host = configs[2];
                _clientSMTP.EnableSsl = enableSSL;
                _clientSMTP.Port = porta;
                _clientSMTP.DeliveryMethod = SmtpDeliveryMethod.Network;
                _clientSMTP.UseDefaultCredentials = false;
                var credentialsConfig = configs[5].Split('|');
                var credentials = new NetworkCredential();
                if (credentialsConfig.Length > 1)
                {
                    credentials.UserName = credentialsConfig[0];
                    credentials.Password = credentialsConfig[1];
                    if (credentialsConfig.Length > 2)
                        credentials.Domain = credentialsConfig[2];
                }
                _clientSMTP.Credentials = credentials;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return this;
        }

        public SMTPMailProvider From(MailAddress from)
        {
            _email.From = from;
            return this;
        }

        public SMTPMailProvider AddTo(MailAddress to)
        {
            _email.To.Add(to);
            return this;
        }

        public SMTPMailProvider AddTos(List<MailAddress> tos)
        {
            foreach (var email in tos)
                _email.To.Add(email);
            return this;
        }

        public SMTPMailProvider AddCC(MailAddress cc)
        {
            _email.CC.Add(cc);
            return this;
        }

        public SMTPMailProvider AddCCs(List<MailAddress> ccs)
        {
            foreach (var email in ccs)
                _email.CC.Add(email);
            return this;
        }

        public SMTPMailProvider AddBCC(MailAddress bcc)
        {
            _email.Bcc.Add(bcc);
            return this;
        }

        public SMTPMailProvider AddBCCs(List<MailAddress> bccs)
        {
            foreach (var email in bccs)
                _email.Bcc.Add(email);
            return this;
        }

        public SMTPMailProvider Subject(string subject)
        {
            _email.Subject = subject;
            return this;
        }

        public SMTPMailProvider IsBodyHtml(bool isHTML)
        {
            _email.IsBodyHtml = isHTML;
            return this;
        }

        public SMTPMailProvider Body(string body)
        {
            _email.Body = body;
            return this;
        }

        public async Task<bool> Send()
        {
            await _clientSMTP.SendMailAsync(_email);
            return true;
        }
    }
}
