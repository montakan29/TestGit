using System;
using System.Collections.Generic;
using System.Linq;
using ThomsonReuters.Eikon.STOpsConsole.UserInfoService;
using ThomsonReuters.Eikon.Toolkit.Interfaces;
using TR.AppServer.Common.Interfaces;
using TR.AppServer.Logging;
using Wcf.Routing;

namespace ThomsonReuters.Eikon.STOpsConsole
{
    class AaaUser: IAaaUser
    {
        private static readonly ILogger Logger = TR.AppServer.Logging.Logger.Default;

        public bool IsUserInRole(string role)
        {
            throw new NotImplementedException();
        }

        public string FullName { get; private set; }
        public string Title { get; private set; }
        public string FirstName { get; private set; }
        public string MiddleName { get; private set; }
        public string LastName { get; private set; }
        public string Suffix { get; private set; }
        public string CompanyName { get; private set; }
        public string[] UserRoles { get; private set; }
        public string JobRole { get; private set; }
        public string PreferredLanguage { get; private set; }
        public string GeographicalFocus { get; private set; }
        public string EmailAddress { get; private set; }
        public string MessagingId { get; private set; }
        public string PhoneNumber { get; private set; }
        public string Address { get; private set; }
        public string Country { get; private set; }
        public string JobRoleCode { get; private set; }
        public string GeographicalFocusCode { get; private set; }
        public string LocationAccountId { get; private set; }
        public string NearestLegalEntityId { get; private set; }
        public string UltimateParentId { get; private set; }
        public string FirmId { get; private set; }
        public string UserId { get; private set; }
        public bool IsExpressLogin { get; private set; }
        public string UUID { get; private set; }
        public string LastUpdateGCAP { get; private set; }
        public string LocalLastName { get; private set; }
        public string LegalConsentFlag { get; private set; }
        public string AccountDomain { get; private set; }
        public string LocalDACSId { get; private set; }
        public string LocalFirstName { get; private set; }
        public string AccountType { get; private set; }
        public string City { get; private set; }
        public string HomeSite { get; private set; }
        public string LegalConsentFlagSetTime { get; private set; }
        public string LastUpdateAAA { get; private set; }
        public string ContactTitle { get; private set; }
        public string AccountName { get; private set; }
        public string ParentAccountId { get; private set; }
        public IDictionary<string, string> Preferences { get; private set; }

        public void SetUser(string uuid)
        {
            using (var uisCilent = new UserInfoServiceClient(RouterBindings.Local,RouterAddresses.Local.RequestReply))
            {
                var locResp = uisCilent.GetUserInfoReq2(new UserInfoReq
                {
                    uuid = uuid,
                    fields = new List<string>
                    {
                        "LocationAccountId",
                        "EmailAddress",
                        "UserId",
                        "FullName",
                        "AccountName",
                        "NearestLegalEntityId",
                        "UltimateParentId"
                    }
                });

                if (!locResp.OperationSuccessful)
                {
                    Logger.LogWarn("STOpsConsole-AaaUser - Failed response with {0} - {1}", locResp.ResponseCode,
                        locResp.ResponseMessage);
                    return;
                }

                UUID = uuid;
                LocationAccountId = locResp.UserInfo.UserDetails.First(x => x.Key == "LocationAccountId").Value;
                EmailAddress = locResp.UserInfo.UserDetails.First(x => x.Key == "EmailAddress").Value;
                UserId = locResp.UserInfo.UserDetails.First(x => x.Key == "UserId").Value;
                FullName = locResp.UserInfo.UserDetails.First(x => x.Key == "FullName").Value;
                CompanyName = locResp.UserInfo.UserDetails.First(x => x.Key == "AccountName").Value;
                NearestLegalEntityId = locResp.UserInfo.UserDetails.First(x => x.Key == "NearestLegalEntityId").Value;
                UltimateParentId = locResp.UserInfo.UserDetails.First(x => x.Key == "UltimateParentId").Value;
            }
        }

        public string GetJsonString()
        {
            return String.Format("{{\"UUID\":\"{0}\",\"UserID\":\"{1}\",\"Email\":\"{2}\",\"CompanyName\":\"{3}\",\"LOID\":\"{4}\",\"LEID\":\"{5}\",\"UPID\":\"{6}\"}}",
                    this.UUID,
                    this.UserId,
                    this.EmailAddress,
                    this.CompanyName,
                    this.LocationAccountId,
                    this.NearestLegalEntityId,
                    this.UltimateParentId
                    );
        }
    }
}
