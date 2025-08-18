using Elmar.WebServiceRest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace Elmar.WcfServiceRestful
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IService1" in both code and config file together.
    [ServiceContract]
    public interface IService1
    {
        [OperationContract]
        List<SmsMobile> GetSmsJson(int value);
        string GetSmsJson();

        // TODO: Add your service operations here
    }
}
