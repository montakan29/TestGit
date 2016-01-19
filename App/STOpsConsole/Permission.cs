using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using Newtonsoft.Json;
using ThomsonReuters.Eikon.Toolkit.Interfaces;
using Wcf.Routing;
using ThomsonReuters.Eikon.STOpsConsole.UserInfoService;
using TR.AppServer.Common.Interfaces;
using TR.AppServer.Logging;

namespace ThomsonReuters.Eikon.STOpsConsole
{
    public class PermissionSetting
    {
        [JsonProperty("writeaccess")]
        public List<string> WriteAccess { get; set; }
        [JsonProperty("readaccess")]
        public List<string> ReadAccess { get; set; }
        [JsonProperty("disable")]
        public List<string> DisableTests { get; set; }

        public PermissionSetting()
        {
            WriteAccess = new List<string>();
            ReadAccess = new List<string>();
            DisableTests = new List<string>();
        }
    }

    public class LocationScope
    {
        public KeyValuePair<FindLocationFilter, string> TopLocationScope { get; set; }
    }

    internal static class Permission
    {
        private static MemoryCache Cache = MemoryCache.Default;
        private const string PermsCacheKey = "OpsConsole-UserPerms-{0}";
        private const string LocsCacheKey = "OpsConsole-UserLocaitons-{0}";
        private const int PermsCacheTimeoutSecs = 3 * 60;

        private static readonly ILogger _Logger = Logger.Default;
        
        static internal class Envs
        {
            static internal readonly string Local = "local";
            static internal readonly string Dev = "dev";
            static internal readonly string Alpha = "alpha";
            static internal readonly string Beta = "beta";
            static internal readonly string Prod = "prod";
        }

        static internal class DataCenters
        {
            internal const string Local = "local";
            internal const string Dev = "dev";
            internal const  string Alpha = "alpha";
            internal const string Beta = "beta";
            internal const string HDCP = "hdcp";
            internal const string NTCP = "ntcp";
            internal const string DTCP = "dtcp";
            internal const string STCP = "stcp";
        }

        static internal class Products
        {
            internal const string All = "all";
            internal const string Eikon = "ekn";
            internal const string FXT = "fxt";
            internal const string EST = "est";
        }

        private static string GetCurrentPlatform()
        {
            string asEnv = System.Environment.GetEnvironmentVariable("AS_ENV") ?? "";
            string currentEnv;
            switch (asEnv.ToLower())
            {
                case "dev":
                    currentEnv = Envs.Dev;
                    break;
                case "alpha":
                    currentEnv = Envs.Alpha;
                    break;
                case "ppe1":
                    currentEnv = Envs.Beta;
                    break;
                case "hdcp":
                case "ntcp":
                case "dtcp":
                case "stcp":
                    currentEnv = Envs.Prod;
                    break;
                case "local":
                case "appengine":
                case "temp":
                    currentEnv = Envs.Local;
                    break;
                default:
                    currentEnv = "Unknown";
                    break;
            }
            return currentEnv;
        }

        internal static bool IsAllowToUploadMetadata(IAaaUser user, List<string> prodList = null)
        {
            if (GetCurrentPlatform() == Envs.Local)
            {
                return true;
            }

            return IsAllowToUploadMetadataAaa(user, prodList);
        }

        internal static bool IsAllowToGetStats(IAaaUser user, List<string> prodList = null)
        {
            if (GetCurrentPlatform() == Envs.Local || IsUserInternal(user))
            {
                return true;
            }

            return IsAllowToGetStatsAaa(user, prodList);
        }

        internal static List<string> GetDisableTests(IAaaUser user)
        {
            return GetUserPermission(user).DisableTests;
        }

        internal static bool IsUserInScope(IAaaUser user, string searchUuid)
        {
            if (IsUserInternal(user))
            {
                return true;
            }
            return string.IsNullOrEmpty(searchUuid) ? true : IsUserInScopeAaa(user.UUID, searchUuid);
        }

        internal static bool IsLocationInScope(IAaaUser user, string searchLocationID)
        {
            if (IsUserInternal(user)) return true;
            //location xx is hardcoded for getting min interval
            if (string.Compare(user.LocationAccountId, searchLocationID, true) == 0 || searchLocationID == "xx") return true;

            if (string.IsNullOrEmpty(user.LocationAccountId) || string.IsNullOrEmpty(searchLocationID))
            {
                _Logger.LogWarn("STOpsConsole-IsLocationInScope - empty location user {0}, [User-{1}/Search-{2}]", user.UUID, user.LocationAccountId,searchLocationID);
                return false;
            }

            using (var uisCilent = new UserInfoServiceClient(RouterBindings.Local, RouterAddresses.Local.RequestReply))
            {
                var locs = new List<LocationInfoRequest>();
                locs.Add(new LocationInfoRequest { LocationAccountId = searchLocationID });
                locs.Add(new LocationInfoRequest { LocationAccountId = user.LocationAccountId });
                var locResp = uisCilent.GetLocations(locs);
                var userULT = locResp.Where(x => x.LocationId == user.LocationAccountId).Select(y => y.UltimateParentId).SingleOrDefault();
                var srchULT = locResp.Where(x => x.LocationId == searchLocationID).Select(y => y.UltimateParentId).SingleOrDefault();

                if (string.Compare(userULT, srchULT, true) == 0)
                {
                    _Logger.LogInfo("STOpsConsole-IsLocationInScope - ULT location matched for user {0}, [User-{1}:{3}/Search-{2}:{4}]", 
                        user.UUID, user.LocationAccountId, searchLocationID,userULT,srchULT);
                    return true;
                }

                _Logger.LogInfo("STOpsConsole-IsLocationInScope - ULT location not matched for user {0}, [User-{1}:{3}/Search-{2}:{4}]",
                        user.UUID, user.LocationAccountId, searchLocationID, userULT, srchULT);
            }
            return false;
        }

        internal static KeyValuePair<FindLocationFilter, string> GetTopLocationScope(IAaaUser user)
        {
            var key = default(KeyValuePair<FindLocationFilter, string>);
            
            try
            {
                var locationScope = GetLocationScopeFromCache(user.UUID);

                if (locationScope == null)
                {
                    using (
                        var uisCilent = new UserInfoServiceClient(RouterBindings.Local,
                            RouterAddresses.Local.RequestReply))
                    {
                        var locResp = uisCilent.GetUserInfoReq2(new UserInfoReq
                        {
                            uuid = user.UUID,
                            fields = new List<string>
                            {
                                "LocationAccountId",
                                "NearestLegalEntityId",
                                "UltimateParentId"
                            }
                        });

                        if (!locResp.OperationSuccessful)
                        {
                            _Logger.LogWarn("STOpsConsole-GetTopLocationScope - Failed response with {0} - {1}", locResp.ResponseCode,
                                locResp.ResponseMessage);
                            return key;
                        }

                        var locs = new Dictionary<string, string>();

                        locs.Add("lo",locResp.UserInfo.UserDetails.First(x => x.Key == "LocationAccountId").Value);
                        locs.Add("le",locResp.UserInfo.UserDetails.First(x => x.Key == "NearestLegalEntityId").Value);
                        locs.Add("up",locResp.UserInfo.UserDetails.First(x => x.Key == "UltimateParentId").Value);

                        var response = uisCilent.GetUserScope(new CheckUserScopeRequest
                        {
                            LoginUUID = user.UUID,
                            CheckLocationUUID = null,
                            LocationID = new List<string> { locs["up"], locs["le"], locs["lo"] },
                            AAAServiceCode = "CPAP_SNAPIN_MANAGE_ST_IRS"
                        });

                        if (!response.Success)
                        {
                            _Logger.LogWarn("STOpsConsole-GetTopLocationScope - Failed response with {0}", response.Message);
                            return key;
                        }

                        foreach (var scope in response.UserScope)
                        {
                            if (locs["up"] == scope.locationAccountIdField && scope.isInScopeField)
                            {
                                key = new KeyValuePair<FindLocationFilter, string>(FindLocationFilter.ULT, locs["up"]);
                                break;
                            }
                            if (locs["le"] == scope.locationAccountIdField && scope.isInScopeField)
                            {
                                key = new KeyValuePair<FindLocationFilter, string>(FindLocationFilter.LGL, locs["le"]);
                                break;
                            }
                            if (locs["lo"] == scope.locationAccountIdField && scope.isInScopeField)
                            {
                                key = new KeyValuePair<FindLocationFilter, string>(FindLocationFilter.LOC, locs["lo"]);
                                break;
                            }
                        }

                        AddLocationScopeToCache(user.UUID, new LocationScope
                        {
                            TopLocationScope = key
                        });
                    }
                }
                else
                {
                    key = locationScope.TopLocationScope;
                }

                _Logger.LogInfo("STOpsConsole-GetTopLocationScope - User {0}, Key {1}, Value {2}", user.UUID, key.Key.ToString(), key.Value);
                return key;
            }
            catch (Exception ex)
            {
                _Logger.LogError("STOpsConsole-GetTopLocationScope: Error get user scope from AAA service: {0}", ex.Message);
                return key;
            }

        }

        private static bool IsAllowToUploadMetadataAaa(IAaaUser user, List<string> prodList = null)
        {
            var valid = false;

            // if not provide product list check permission for Eikon Product as default
            if (prodList == null)
            {
                prodList = new List<string>{Products.EST};
            }

            prodList.Add(Products.All);

            foreach (var prod in prodList)
            {
                valid = GetUserPermission(user).WriteAccess.Contains(prod);
                if(valid) break;
            }

            return valid;
        }

        private static bool IsAllowToGetStatsAaa(IAaaUser user, List<string> prodList = null)
        {
            var valid = false;

            // if not provide product list check permission for Eikon Product as default
            if (prodList == null)
            {
                prodList = new List<string> { Products.EST };
            }

            prodList.Add(Products.All);

            foreach (var prod in prodList)
            {
                valid = GetUserPermission(user).ReadAccess.Contains(prod);
                if (valid) break;
            }

            _Logger.LogDebug("STOpsConsole-IsAllowToGetStatsAaa: {0} - {1}",user.UUID, valid);

            return valid;
        }

        internal static bool IsUserInternal(IAaaUser user)
        {
            var emailAddress = user.EmailAddress;            
            _Logger.LogDebug("STOpsConsole-IsUserInternal: {0} - {1} - {2}", emailAddress, user.UserId, user.UUID);
            if (string.IsNullOrWhiteSpace(emailAddress))
            {
                _Logger.LogDebug("STOpsConsole-IsUserInternal no email address treat as external: {0} - {1}", emailAddress, user.UUID);
                return false;
            }
            return emailAddress.EndsWith("@thomsonreuters.com", StringComparison.OrdinalIgnoreCase)
                            || emailAddress.EndsWith("@thomson.com", StringComparison.OrdinalIgnoreCase)
                            || emailAddress.EndsWith("@reuters.com", StringComparison.OrdinalIgnoreCase)
                            || emailAddress.EndsWith("@apac.reuters.com", StringComparison.OrdinalIgnoreCase)
                            || emailAddress.EndsWith("@fxall.com", StringComparison.OrdinalIgnoreCase)
                            || emailAddress.EndsWith("@tradeweb.com", StringComparison.OrdinalIgnoreCase);
        }

        private static bool IsUserInScopeAaa(string userUuid, string searchUuid)
        {
            try
            {
                using (var uisCilent = new UserInfoServiceClient(RouterBindings.Local, RouterAddresses.Local.RequestReply))
                {
                    var response = uisCilent.GetUserScope(new CheckUserScopeRequest
                    {
                        LoginUUID = userUuid,
                        CheckLocationUUID = new List<string> { searchUuid },
                        LocationID = null,
                        AAAServiceCode = "CPAP_SNAPIN_MANAGE_ST_IRS"
                    });

                    if (!response.Success)
                    {
                        _Logger.LogWarn("STOpsConsole-IsUserInScopeAaa - Failed response with {0}", response.Message);
                        return false;
                    }

                    var isUserInScope = false;
                    foreach (var scope in response.UserScope)
                    {
                        isUserInScope |= scope.isInScopeField;
                    }

                    _Logger.LogDebug("STOpsConsole-IsUserInScopeAaa: {0} - {1}", userUuid, isUserInScope);

                    return isUserInScope;
                }
            }
            catch (Exception ex)
            {
                _Logger.LogError("STOpsConsole-Error get user scope from AAA service: {0}", ex.Message);
                return false;
            }
        }

        public static string GetUserInScopeAaaDetails(string userUuid, string searchUuid)
        {
            try
            {
                using (var uisCilent = new UserInfoServiceClient(RouterBindings.Local, RouterAddresses.Local.RequestReply))
                {
                    var response = uisCilent.GetUserScope(new CheckUserScopeRequest
                    {
                        LoginUUID = userUuid,
                        CheckLocationUUID = new List<string> { searchUuid },
                        LocationID = null,
                        AAAServiceCode = "CPAP_SNAPIN_MANAGE_ST_IRS"
                    });

                    if (!response.Success)
                    {
                        _Logger.LogWarn("STOpsConsole-IsUserInScopeAaa - Failed response with {0}", response.Message);
                        return "{}";
                    }

                    var isUserInScope = "user:"+userUuid+" search:"+searchUuid;
                    var scopeList = new List<string>();

                    foreach (var scope in response.UserScope)
                    {
                        isUserInScope += "("+scope.locationAccountIdField+"_"+scope.isInScopeField+")";
                        scopeList.Add(String.Format("{{\"location\":\"{0}\",\"isInScope\":\"{1}\"}}",
                        scope.locationAccountIdField,
                        scope.isInScopeField
                        )); 
                    }
                    _Logger.LogDebug("STOpsConsole-IsUserInScopeAaa: {0} - {1}", userUuid, isUserInScope);

                    var jsonresp = String.Format("{{\"user\":\"{0}\",\"search\":\"{1}\",\"scope\":[{2}]}}",
                                userUuid,
                                searchUuid,
                                string.Join(",", scopeList)
                                );
                    return jsonresp;
                }
            }
            catch (Exception ex)
            {
                _Logger.LogError("STOpsConsole-Error get user scope from AAA service: {0}", ex.Message);
                return "{}";
            }
        }

        public static PermissionSetting GetUserPermission(IAaaUser user)
        {
            try
            {
                var permission = GetUserPermissionFromCache(user.UUID);
                if (permission == null)
                {
                    var req = new UserPreferencesReq
                    {
                        uuid = user.UUID,
                        preferences = new List<preference>(1) { new preference { dactName = "APP.SYSTEMTEST.PERMISSION", prefName = "APP.SYSTEMTEST.PERMISSION" } }
                    };

                    using (var userInfoServiceclient = new UserInfoServiceClient(RouterBindings.Local, RouterAddresses.Local.RequestReply))
                    {
                        var svcResp = userInfoServiceclient.GetUserPreferencesReq(req);
                        if (svcResp != null)
                        {
                            var setting = svcResp.preferences.FirstOrDefault().value;
                            //setting = @"{""writeaccess"": [
                            //                                    """"
                            //                                ],
                            //                                ""readaccess"": [
                            //                                    """"
                            //                                ]
                            //                            }";

                            permission = JsonConvert.DeserializeObject<PermissionSetting>(setting);
                            AddUserPermissionToCache(user.UUID, permission);
                            return permission;
                        }
                        return new PermissionSetting();
                    }
                }
                return permission;

            }
            catch (Exception ex)
            {
                _Logger.LogError("STOpsConsole-Error getting user preference from UserInfoService: {0}", ex.Message);
                return new PermissionSetting();
            }
        }

        private static PermissionSetting GetUserPermissionFromCache(string uuid)
        {
            try
            {
                var key = string.Format(PermsCacheKey, uuid);
                var userPerms = Cache.Get(key) as PermissionSetting;
                return userPerms;
            }
            catch (Exception ex)
            {
                _Logger.LogError("STOpsConsole-Error getting user permission from cache: {0}", ex.Message);
            }

            return null;
        }

        private static void AddUserPermissionToCache(string uuid, PermissionSetting userPerms)
        {
            try
            {
                var key = string.Format(PermsCacheKey, uuid);
                Cache.Set(key, userPerms, DateTimeOffset.UtcNow.AddSeconds(PermsCacheTimeoutSecs));
            }
            catch (Exception ex)
            {
                _Logger.LogError("STOpsConsole-Error adding user permission to cache: {0}", ex.Message);
            }

        }

        private static LocationScope GetLocationScopeFromCache(string uuid)
        {
            try
            {
                var key = string.Format(LocsCacheKey, uuid);
                var locScope = Cache.Get(key) as LocationScope;
                return locScope;
            }
            catch (Exception ex)
            {
                _Logger.LogError("STOpsConsole-Error getting location scope from cache: {0}", ex.Message);
            }

            return null;
        }

        private static void AddLocationScopeToCache(string uuid, LocationScope locationScope)
        {
            try
            {
                var key = string.Format(LocsCacheKey, uuid);
                Cache.Set(key, locationScope, DateTimeOffset.UtcNow.AddSeconds(PermsCacheTimeoutSecs));
            }
            catch (Exception ex)
            {
                _Logger.LogError("STOpsConsole-Error adding location scope to cache: {0}", ex.Message);
            }

        }
    }
}
