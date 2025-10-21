using System.Collections.Generic;
using System.Net.Mail;
using System.Threading.Tasks;

namespace Domain.Interfaces
{
    public interface ISendMail<T>
    {
        T Prepare(string connectionString);
        T From(MailAddress from);
        T AddTo(MailAddress to);
        T AddTos(List<MailAddress> tos);
        T AddCC(MailAddress cc);
        T AddCCs(List<MailAddress> ccs);
        T AddBCC(MailAddress bcc);
        T AddBCCs(List<MailAddress> bccs);
        T Subject(string subject);
        T IsBodyHtml(bool isHTML);
        T Body(string body);
        Task<bool> Send();
    }
}
