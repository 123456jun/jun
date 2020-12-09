using RobotForeignWorkflow.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TradeWolfRobot.EmailAutomation.FrontEnd
{
    public partial class SharpToJs
    {
        RobotForeignDal _robotForeignDal = null;
        public SharpToJs()
        {
            _robotForeignDal = new RobotForeignDal();
        }
    }
}
