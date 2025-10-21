using System.ComponentModel;

namespace Auxiliar.Enums
{
    public enum MailTipoEnum
    {
        [Description("SMTP")]
        SMTP = 1,
        [Description("SendGrid")]
        SendGrid = 2,
    }
}
