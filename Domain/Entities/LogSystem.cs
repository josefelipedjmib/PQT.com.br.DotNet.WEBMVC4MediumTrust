using Domain.Attributes;
using System;

namespace Domain.Entities
{
    [Table("logsystem")]
    public class LogSystem
    {
        public int id{ get; set; }
        public string usuario { get; set; }
        public string acao { get; set; }
        public string status { get; set; }
        public DateTime datahora { get; set; }
        public int ipa { get; set; }
        public int ipb { get; set; }
        public int ipc { get; set; }
        public int ipd { get; set; }
        public bool sucesso { get; set; }
    }
}
