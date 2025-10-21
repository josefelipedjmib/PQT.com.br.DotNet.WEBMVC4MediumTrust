using Auxiliar.Enums;
using Auxiliar.Extensions;
using Domain.Interfaces;
using Infrastructure.Mail;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace Service.Mail
{
    public class Mail
    {
        private string _connectionString = "";
        private Dictionary<string, string> _config;
        private MailTipoEnum _mailTipo = MailTipoEnum.SMTP;
        private bool _enviarEmail = false;
        private string _telaSecao = "";
        private string _subject;
        private bool _isBodyHTML = false;
        private string _body = "";
        private MailAddress _from;
        private List<MailAddress> _tos = new List<MailAddress>();
        private List<MailAddress> _ccs = new List<MailAddress>();
        private List<MailAddress> _bccs = new List<MailAddress>();

        public Mail()
        {
        }

        public Mail Prepare(Dictionary<string, string> config)
        {
            try
            {
                _config = config;
                var emUso = _config["Email:EmUso"];
                //emUso = BancoDadosConexao.PegarParametro("EmailEmUso", emUso);
                if (emUso.ToUpper().Contains("SENDGRID"))
                    _mailTipo = MailTipoEnum.SendGrid;
                _connectionString = _config[emUso].Decrypt();
                //_connectionString = BancoDadosConexao.PegarParametro(emUso, _connectionString).Decrypt();
                if (string.IsNullOrEmpty(_connectionString))
                    throw new Exception("Problema encontrado na configuração do preparo de e-mail.");
                bool.TryParse(_config["Email:EnviarReal"], out _enviarEmail);
                //bool.TryParse(BancoDadosConexao.PegarParametro("EnviarReal", _enviarEmail.ToString()), out _enviarEmail);
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao preparar o envio de e-mail: " + ex.Message);
            }
            return this;
        }

        public Mail From(MailAddress from)
        {
            _from = from;
            return this;
        }

        public Mail AddTo(MailAddress to)
        {
            _tos.Add(to);
            return this;
        }

        public Mail AddCC(MailAddress cc)
        {
            _ccs.Add(cc);
            return this;
        }

        public Mail AddBCC(MailAddress bcc)
        {
            _bccs.Add(bcc);
            return this;
        }

        public Mail Subject(string subject)
        {
            _subject = subject;
            return this;
        }

        public Mail IsBodyHtml(bool isHTML)
        {
            _isBodyHTML = isHTML;
            return this;
        }

        public Mail Body(string body)
        {
            _body = body;
            return this;
        }

        public async Task<bool> Send()
        {
            try
            {
                if (string.IsNullOrEmpty(_connectionString)) throw new Exception("método Prepare não chamado corretamente.");
                switch (_mailTipo)
                {
                    case MailTipoEnum.SMTP:
                        return await EnviarEmail<SMTPMailProvider>(new SMTPMailProvider());
                        break;
                    case MailTipoEnum.SendGrid:
                        return await EnviarEmail<SendGridMailProvider>(new SendGridMailProvider());
                        break;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao enviar e-mail: " + ex.Message);
            }
            return false;
        }

        public Mail BodyObterDoArquivo(string caminhoDoArquivo, IEnumerable<KeyValuePair<string, string>> variaveis)
        {
            _body = ObterTextoCompletoDoArquivo(caminhoDoArquivo, variaveis);
            return this;
        }

        public Mail DefinirTelaSecao(string telaSecao)
        {
            _telaSecao = telaSecao;
            return this;
        }

        public Mail AdicionarDestinatarios(string emailsPara)
        {
            string parametro;
            if (_enviarEmail)
            {
                AddEmailsDestinatarios(AddTo, emailsPara);
            }
            else
            {
                var emailsDevs = _config["Email:EmailsDevs"];
                //emailsDevs = BancoDadosConexao.PegarParametro("EmailsDevs", emailsDevs);
                AddEmailsDestinatarios(AddTo, emailsDevs);
            }

            var emails = _config["Email:EmailsCC"];
            //emails = BancoDadosConexao.PegarParametro("EmailsCC", emails);
            AddEmailsDestinatarios(AddCC, emails);
            emails = _config["Email:EmailsBCC"];
            //emails = BancoDadosConexao.PegarParametro("EmailsBCC", emails);
            AddEmailsDestinatarios(AddBCC, emails);
            if (!string.IsNullOrEmpty(_telaSecao))
            {
                try
                {
                    emails = _config["Email:Emails" + _telaSecao];
                    //emails = BancoDadosConexao.PegarParametro("EmailsBCC" + _telaSecao, emails);
                    AddEmailsDestinatarios(AddBCC, emails);
                }
                catch (Exception ex) { }
            }

            return this;
        }

        private string ObterTextoCompletoDoArquivo(string caminhoDoArquivo,
            IEnumerable<KeyValuePair<string, string>> variaveis)
        {
            try
            {
                caminhoDoArquivo = Path.Combine(AppContext.BaseDirectory, "bin\\Mail\\Templates\\", caminhoDoArquivo);
                var templateTexto = File.ReadAllText(caminhoDoArquivo, Encoding.UTF8);
                return PreencherInformacoesDoEmail(templateTexto, variaveis);
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao tratar TemplatesEmail");
            }
        }

        private string PreencherInformacoesDoEmail(string mensagem,
            IEnumerable<KeyValuePair<string, string>> variaveis)
        {
            return variaveis.Aggregate(mensagem, (atual, variavel) => atual.Replace(variavel.Key, variavel.Value));
        }

        private void AddEmailsDestinatarios(Func<MailAddress, Mail> addEmails, string emails = "")
        {
            try
            {
                if (!string.IsNullOrEmpty(emails))
                {
                    emails = emails.Replace(',', ';');
                    foreach (var email in emails.Split(';'))
                    {
                        addEmails(new MailAddress(email));
                    };
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Não é possível enviar e-mail com endereço inválido: " + emails);
            }
        }
        private async Task<bool> EnviarEmail<T>(ISendMail<T> sendMail) where T : ISendMail<T>
        {
            if (_enviarEmail)
                return await sendMail
                    .Prepare(_connectionString)
                    .Subject(_subject)
                    .IsBodyHtml(_isBodyHTML)
                    .Body(_body)
                    .AddTos(_tos)
                    .AddCCs(_ccs)
                    .AddBCCs(_bccs)
                    .Send();
            return _enviarEmail;
        }

    }
}
