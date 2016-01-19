using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThomsonReuters.Eikon.STOpsConsole
{
    class StubString
    {
        public static string GetStubJson()
        {
            return "{\"uuid\":\"PAXTRA0000\",\"installGuid\":\"3d025b7e-c0be-497c-8717-c9fb2b10758e\",\"stats\":[{\"statName\":\"RTNEWS\",\"statDisplayName\":\"Response time to retrieve news\",\"statLatestVal\":\"200\"},{\"statName\":\"RTVIEWS\",\"statDisplayName\":\"Response time to retrieve views\",\"statLatestVal\":\"230\"},{\"statName\":\"RTQUOTE\",\"statDisplayName\":\"Response time to retrieve Quote\",\"statLatestVal\":\"100\"},{\"statName\":\"RTHBEAT\",\"statDisplayName\":\"Response time to retrieve Heartbeat\",\"statLatestVal\":\"100\"},{\"statName\":\"RTELEK\",\"statDisplayName\":\"Response time to retrieve Elektron\",\"statLatestVal\":\"10\"},{\"statName\":\"RTGWAY\",\"statDisplayName\":\"Response time to retrieve Gateway\",\"statLatestVal\":\"320\"},{\"statName\":\"RTLOGIN\",\"statDisplayName\":\"Response time to Login\",\"statLatestVal\":\"120\"}],\"history\":{\"statName\":\"RTNEWS\",\"rows\":[{\"timeStamp\":\"2014-12-23T00:00:00.96786Z\",\"statVal\":\"200\"},{\"timeStamp\":\"2014-12-23T00:30:00.96786Z\",\"statVal\":\"300\"},{\"timeStamp\":\"2014-12-23T01:00:00.96786Z\",\"statVal\":\"300\"},{\"timeStamp\":\"2014-12-23T01:30:00.96786Z\",\"statVal\":\"400\"},{\"timeStamp\":\"2014-12-23T02:00:00.96786Z\",\"statVal\":\"450\"},{\"timeStamp\":\"2014-12-23T02:30:00.96786Z\",\"statVal\":\"400\"},{\"timeStamp\":\"2014-12-23T03:00:00.96786Z\",\"statVal\":\"600\"},{\"timeStamp\":\"2014-12-23T03:30:00.96786Z\",\"statVal\":\"455\"}]}}";
        }
    }
}
