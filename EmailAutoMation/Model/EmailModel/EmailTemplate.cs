using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TradeWolfRobot.EmailAutomation.Model.EmailModel
{
  public  class EmailTemplate
    {
        public int Id { get; set; }
       public string MailTheme { get; set; }
       public string MailBody { get; set; }
       public int IsChoose { get; set; }
    }
}
