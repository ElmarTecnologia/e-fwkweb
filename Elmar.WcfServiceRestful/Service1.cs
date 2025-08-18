using Elmar.WebServiceRest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace Elmar.WcfServiceRestful
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in both code and config file together.
    public class Service1 : IService1
    {
        public List<SmsMobile> GetSmsJson(int value)
        {
            //return new SMSController().Get(value);
            return new List<SmsMobile>();
        }

        public List<SmsMobile> GetSmsJson()
        {
            //return new SMSController().Get();
            return new List<SmsMobile>();
        }
    }
}
