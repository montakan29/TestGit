using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;

namespace SystemTestService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "ISystemTestService" in both code and config file together.
    [ServiceContract]
    public interface ISystemTestService
    {
        [OperationContract]
        [WebInvoke(Method = "GET", UriTemplate = "/EDW/GetData")]
        string GetData();

        [OperationContract]
        [WebInvoke(Method = "GET", UriTemplate = "/EDW/DumpAndUploadRSTStat?start={start}&end={end}")]
        string DumpAndUploadRSTStatInterval(string start, string end);
    }    
}
