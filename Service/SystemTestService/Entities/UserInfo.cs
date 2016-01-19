using System.Linq;
using SystemTestService.UserInfoService;
using TR.AppServer.Common.Interfaces;
using TR.AppServer.Logging;
using Wcf.Routing;

namespace SystemTestService.Utility
{
    public class UserInfo
    {
        private static readonly ILogger ALogger = Logger.Default;

        public static UserDetails GetUserDetails(string uuid)
        {
            var userDetails = new UserDetails();
            using (var uisCilent = new UserInfoServiceClient(RouterBindings.Local, RouterAddresses.Local.RequestReply))
            {
                var userResp = uisCilent.GetUserInfoReq2(new UserInfoReq
                {
                    uuid = uuid,
                    fields = new[]
                    {
                        "UserId",
                        "EmailAddress",
                        "LocationAccountId"
                    }
                });

                if (!userResp.OperationSuccessful)
                {
                    ALogger.LogWarn("STOpsConsole-GetTopLocationScope - Failed response with {0} - {1}",
                        userResp.ResponseCode,
                        userResp.ResponseMessage);
                    return null;
                }

                var userID = userResp.UserInfo.UserDetails.First(x => x.Key == "UserId").Value;
                var email = userResp.UserInfo.UserDetails.First(x => x.Key == "EmailAddress").Value;
                userDetails.UserID = userID;
                userDetails.Email = email;
                var locAccID = userResp.UserInfo.UserDetails.First(x => x.Key == "LocationAccountId").Value;

                if (string.IsNullOrEmpty(locAccID)) return userDetails;

                var locResp = uisCilent.GetLocation(new LocationInfoRequest
                {
                    LocationAccountId = locAccID
                });
                if (locResp != null)
                {
                    userDetails.Country = locResp.Country;
                }
                return userDetails;
            }
        }
    }

    public class UserDetails
    {
        public string UserID { get; set; }
        public string Email { get; set; }
        public string Country { get; set; }
    }
}
