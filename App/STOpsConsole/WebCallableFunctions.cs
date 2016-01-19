// -----------------------------------------------------------------------
// <copyright file="$className$.cs" company="Thomson Reuters">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.IO;
using System.Linq;
using System.Runtime.Caching;
using System.ServiceModel.Channels;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ThomsonReuters.Eikon.Toolkit.Interfaces;
using TR.AppServer.Logging;
using Wcf.Routing;
using System.Collections.Generic;
using System.Web;
using System.Text;
using TR.AppServer.Common.Interfaces;
using ThomsonReuters.Eikon.STOpsConsole.OpsConsoleService;
using ThomsonReuters.Eikon.STOpsConsole.UserInfoService;

namespace ThomsonReuters.Eikon.STOpsConsole
{
    public class FindMachineInstallRequest
    {
        [JsonProperty("searchString")]
        public string SearchString { get; set; }

        [JsonProperty("filter")]
        public bool Filter { get; set; }

        [JsonProperty("product")]
        public string Product { get; set; }
    }

    public class FindMachInstResponse
    {
        [JsonProperty("items")]
        public IList<FindMachInstInfoItem> Items { get; set; }

        [JsonProperty("product")]
        public string Product { get; set; }
    }

    public class FindMachInstInfoItem
    {
        [JsonProperty("uuid")]
        public string UUID { get; set; }

        [JsonProperty("firstName")]
        public string FirstName { get; set; }

        [JsonProperty("lastName")]
        public string LastName { get; set; }

        [JsonProperty("email")]
        public string EmailAddress { get; set; }

        /*[JsonProperty("machInstInfoList")]
        public IList<MachInstInfo> MachInstInfoList { get; set; }*/
    }

    public class UserPermission
    {
        public string IsAllowToUploadMetadata { get; set; }
        public string IsAllowToGetStats { get; set; }
        public string IsInternal { get; set; }
        public string IsUserInScope { get; set; }
    }

    // AppServer: match the app name to namespace above, and the class name below in the UiToolkitApp attribute.
    [UiToolkitApp("ThomsonReuters.Eikon.OpsConsole.WebCallableFunctions")]
    public class WebCallableFunctions
    {
        private MemoryCache _Cache = MemoryCache.Default;

        public string GetStats(string query, string body, IAppServerServices services)
        {
            var req = JObject.Parse(body);
            string uuid = null, mid = null, iid = null, prod = null;
            int testid = 0;
            try
            {
                uuid = (string) req["uuid"];
                prod = (string) req["product"];
                mid = (string) req["machineGuid"];
                iid = (string) req["installGuid"];
                testid = (int) req["testid"];
            }
            catch (Exception ex)
            {
                services.Logger.LogInfo("STOpsConsole - GetStats - by UUID extract query parameter exception {0}", ex.Message);
            }

            if (testid > 0 && (string.IsNullOrEmpty(uuid) || string.IsNullOrEmpty(mid) || string.IsNullOrEmpty(iid)))
            {
                services.Logger.LogInfo("STOpsConsole - GetStats - by test id {0}", testid);
            }
            else if (string.IsNullOrEmpty(mid) && string.IsNullOrEmpty(iid)) // If request has only uuid specify, try to get the stat of the latest machine id and installation id.
            {
                services.Logger.LogInfo("STOpsConsole - GetStats - by UUID");
                using (var opsConsoleSvc = new OpsConsoleServiceClient(RouterBindings.Local,RouterAddresses.Local.RequestReply))
                {
                    try
                    {
                        var machInstReq = new MachInstInfoRequest()
                        {
                            uuids = new List<string> {uuid},
                            product = prod,
                            filter = true
                        };
                        var response = opsConsoleSvc.GetMachineInstallInfo(machInstReq);
                        var latest = response.Items.FirstOrDefault()
                                        .machInstInfoList.OrderByDescending(y => DateTime.Parse(y.dateLastSeen))
                                        .First();
                        req.Add(new JProperty("installGuid", latest.instGUID));
                        req.Add(new JProperty("machineGuid", latest.machGUID));
                        body = JsonConvert.SerializeObject(req);
                    }
                    catch (Exception ex)
                    {
                        services.Logger.LogError("STOpsConsole - GetStats:GetMachineInstallInfo - Caught exception: " + ex.Message);
                    }
                }
            }
            else if (string.IsNullOrEmpty(mid) || string.IsNullOrEmpty(iid))
            {
                services.Logger.LogWarn(string.IsNullOrEmpty(mid)
                    ? "STOpsConsole - GetStats:GetMachineInstallInfo - machine id is null or empty"
                    : "STOpsConsole - GetStats:GetMachineInstallInfo - installation id is null or empty");
                return "{}";
            }

            var getStatsRequest = JsonConvert.DeserializeObject<GetStatsRequest>(body);
            getStatsRequest.product = "est";
            var product = new List<string> { "est" };
            if (Permission.IsAllowToGetStats(services.UserContext, product) && Permission.IsUserInScope(services.UserContext, getStatsRequest.uuid)){
                using (var opsConsoleSvc = new OpsConsoleServiceClient(RouterBindings.Local, RouterAddresses.Local.RequestReply))
                {
                    try
                    {
                        var response = opsConsoleSvc.GetStats(getStatsRequest);
                        if (response == null || String.IsNullOrEmpty(response.uuid))
                        {
                            return "{}";
                        }
                        response.firstName = services.UserSearch.GetUserByUuid(response.uuid).FirstName;
                        response.lastName = services.UserSearch.GetUserByUuid(response.uuid).LastName;
                        response.email = services.UserSearch.GetUserByUuid(response.uuid).EmailAddress;
                        return JsonConvert.SerializeObject(response);
                    }
                    catch (Exception ex)
                    {
                        services.Logger.LogError("Error while invoking GetStats: {0}", ex.Message);
                        return "{}";
                    }
                }
            }
            else
            {
                return "{}";
            }
        }

        public string GetSystemTestSummary(string query, string body, IAppServerServices services)
        {
            var getMultiStatsRequest = JsonConvert.DeserializeObject<GetMultiStatsRequest>(body);
            if (Permission.IsAllowToGetStats(services.UserContext, getMultiStatsRequest.products))
            {
                using (var opsConsoleSvc = new OpsConsoleServiceClient(RouterBindings.Local, RouterAddresses.Local.RequestReply))
                {
                    try
                    {
                        var response = opsConsoleSvc.GetMultiStats(getMultiStatsRequest);
                        response.firstName = services.UserSearch.GetUserByUuid(getMultiStatsRequest.uuid).FirstName;
                        response.lastName = services.UserSearch.GetUserByUuid(getMultiStatsRequest.uuid).LastName;
                        response.email = services.UserSearch.GetUserByUuid(getMultiStatsRequest.uuid).EmailAddress;
                        return response == null ? "{}" : JsonConvert.SerializeObject(response);
                    }
                    catch (Exception ex)
                    {
                        services.Logger.LogError("Error while invoking GetMultiStats: {0}", ex.Message);
                        return "{}";
                    }
                }
            }
            else
            {
                return "{}";
            }
        }

        public string IsAllowToUploadMetadata(string query, string body, IAppServerServices services)
        {
            var values = JsonConvert.DeserializeObject<Dictionary<string, string>>(body);
            //var product = values != null && values.ContainsKey("product") ? new List<string>() { values["product"] } : null;
            var product = new List<string> { "est" };
            return Permission.IsAllowToUploadMetadata(services.UserContext, product) ? "{\"isAllowToUploadMetadata\":true}" : "{\"isAllowToUploadMetadata\":false}";
        }

        public string IsAllowToGetStats(string query, string body, IAppServerServices services)
        {
            var values = JsonConvert.DeserializeObject<Dictionary<string, string>>(body);
            //var product = values != null && values.ContainsKey("product") ? new List<string>() { values["product"] } : null;
            var product = new List<string> { "est" };
            return Permission.IsAllowToGetStats(services.UserContext, product) ? "{\"isAllowToGetStats\":true}" : "{\"isAllowToGetStats\":false}";
        }

        public string GetUserPermissions(string query, string body, IAppServerServices services)
        {
            var values = JsonConvert.DeserializeObject<Dictionary<string, string>>(body);
            var product = new List<string> { "est" };
            var enterUuid = values != null && values.ContainsKey("uuid") ? values["uuid"] : null;
            var fromCache = false;

            var cacheKey = string.Format("{0}_{1}_PEMISSION", services.UserContext.UUID, enterUuid ?? "INITIAL");
            UserPermission userPermission;
            if (_Cache.Contains(cacheKey))
            {
                userPermission = _Cache.Get(cacheKey) as UserPermission;
                fromCache = true;
            }
            else
            {
                userPermission = new UserPermission
                {
                    IsAllowToUploadMetadata = Permission.IsAllowToUploadMetadata(services.UserContext, product) ? "true" : "false",
                    IsAllowToGetStats = Permission.IsAllowToGetStats(services.UserContext, product) ? "true" : "false",
                    IsInternal = Permission.IsUserInternal(services.UserContext) ? "true" : "false",
                    IsUserInScope = string.IsNullOrEmpty(enterUuid) ? "true" : Permission.IsUserInScope(services.UserContext, enterUuid) ? "true" : "false"
                };
                _Cache.Set(cacheKey, userPermission, DateTimeOffset.UtcNow.AddMinutes(1));
            }

            return String.Format("{{\"isAllowToUploadMetadata\":{0},\"isAllowToGetStats\":{1},\"isInternal\":{2},\"isUserInScope\":{3},\"cache\":{4},\"scope\":{5}}}", 
                userPermission.IsAllowToUploadMetadata, 
                userPermission.IsAllowToGetStats,
                userPermission.IsInternal,
                userPermission.IsUserInScope,
                fromCache?"true":"false",
                Permission.GetUserInScopeAaaDetails(services.UserContext.UUID, enterUuid)
                );
        }

        public string GetUserScope(string query, string body, IAppServerServices services)
        {
            var logger = services.Logger;
            try
            {
                var values = JsonConvert.DeserializeObject<Dictionary<string, string>>(body);
                var product = new List<string> { "est" };
                var loginUser = values != null && values.ContainsKey("loginUser") ? values["loginUser"] : null;
                var lookedUpUser = values != null && values.ContainsKey("lookedUpUser") ? values["lookedUpUser"] : null;
                var searchString = values != null && values.ContainsKey("searchString") && !string.IsNullOrEmpty(values["searchString"]) ? values["searchString"] : "*";

                var loginAaaUser = new AaaUser();
                loginAaaUser.SetUser(!String.IsNullOrEmpty(loginUser) ? loginUser : services.UserContext.UUID);
            
                var fromCache = false;

                var cacheKey = string.Format("{0}_{1}_PEMISSION", services.UserContext.UUID, lookedUpUser ?? "INITIAL");
                UserPermission userPermission;
                if (_Cache.Contains(cacheKey))
                {
                    userPermission = _Cache.Get(cacheKey) as UserPermission;
                    fromCache = true;
                }
                else
                {
                    userPermission = new UserPermission
                    {
                        IsAllowToUploadMetadata = Permission.IsAllowToUploadMetadata(loginAaaUser, product) ? "true" : "false",
                        IsAllowToGetStats = Permission.IsAllowToGetStats(loginAaaUser, product) ? "true" : "false",
                        IsInternal = Permission.IsUserInternal(loginAaaUser) ? "true" : "false",
                        IsUserInScope = string.IsNullOrEmpty(lookedUpUser) ? "n/a" : Permission.IsUserInScope(loginAaaUser, lookedUpUser) ? "true" : "false"
                    };
                    _Cache.Set(cacheKey, userPermission, DateTimeOffset.UtcNow.AddMinutes(1));
                }

                #region loginUser

                var userSettingString = "{}";
                var userPermissionSetting = Permission.GetUserPermission(loginAaaUser);
                if (userPermissionSetting != null)
                {
                    userSettingString = JsonConvert.SerializeObject(userPermissionSetting);
                }

                var permissionString = "{}";
                if (userPermission != null)
                {
                    permissionString = JsonConvert.SerializeObject(userPermission);
                }

                var loginUserPermission = String.Format("{{\"userPermission\":{0},\"setting\":{1},\"cache\":\"{2}\"}}",
                    permissionString,
                    userSettingString,
                    fromCache.ToString().ToLower()
                    );

                var loginUserLocation = String.Format("{{\"locationScope\":{0},\"topLocationScope\":\"{1}\"}}",
                    Permission.GetUserInScopeAaaDetails(loginUser, loginUser),
                    Permission.GetTopLocationScope(loginAaaUser)
                    );

                var loginUserResult = String.Format("{{\"Info\":{0},\"Permissions\":{1},\"Locations\":{2}}}",
                    loginAaaUser.GetJsonString(),
                    loginUserPermission,
                    loginUserLocation
                    );
                #endregion

                #region searchUser
                var req = new FindMachineInstallRequest()
                {
                    SearchString = searchString,
                    Product = "est",
                    Filter = true
                };
                var searchResult = FindMachineInstall(req,loginAaaUser,logger);
                #endregion

                #region lookupUser
                var lookupUserResult = String.Format("{{\"locationScope\":\"empty argument\"}}");
                var lookupAaaUser = new AaaUser();
                if (!String.IsNullOrEmpty(lookedUpUser))
                {
                    var lookupUserLocation = String.Format("{{\"locationScope\":{0}}}",
                    Permission.GetUserInScopeAaaDetails(loginUser, lookedUpUser)
                    );

                    lookupAaaUser.SetUser(lookedUpUser);
                    lookupUserResult = String.Format("{{\"Info\":{0},\"Locations\":{1}}}",
                        lookupAaaUser.GetJsonString(),
                        lookupUserLocation
                        );
                }
                #endregion

                var result = String.Format("{{\"LogInUser\":{0},\"LookUpUser\":{1},\"SearchResult\":{2},\"status\":\"{3}\"}}",
                    loginUserResult,
                    lookupUserResult,
                    searchResult,
                    "Done"
                    );

                return result;
            }
            catch (Exception ex)
            {
                logger.LogError("GetUserScope - error {0}", ex.Message);
                return String.Format("{{\"status\":\"{0}\"}}",
                "Error: " + ex.Message
                );
            }
        }

        public void UploadMetadata(IDictionary<string, object> context)
        {
            var user = ((IAaaUser)context["AS.Services.ThomsonReuters.Eikon.Toolkit.Interfaces.IAaaUser"]);
            MetadataResponse response = null;

            string body = (string)context["AS.RequestBody"];
            HttpFormFileContentPaarser httpParser = new HttpFormFileContentPaarser(body, "metadata");

            //var product = string.IsNullOrEmpty(httpParser.Product) ? null : new List<string>() { httpParser.Product };
            var product = new List<string> { "est" };

            if (!Permission.IsAllowToUploadMetadata(user, product))
            {
                response = new MetadataResponse { success = false, description = "Failed - no permission to upload the metadata" };
            }
            else
            {
                using (var opsConsoleSvc = new OpsConsoleServiceClient(RouterBindings.Local, RouterAddresses.Local.RequestReply))
                {
                    string data = httpParser.FileContents.Replace("\r\n", string.Empty).Replace("\t", string.Empty).Trim();

                    if (string.IsNullOrEmpty(data))
                    {
                        response = new MetadataResponse { success = false, description = "Failed - no metadata content" };
                    }
                    else if (data.Length > 4194304) // limit to 4MB
                    {
                        response = new MetadataResponse() { success = false, description = "Failed - the file is too big" };
                    }
                    else if (data[0] != '{' || data[data.Length - 1] != '}')
                    {
                        response = new MetadataResponse { success = false, description = "Failed - the content is invalid" };
                    }
                    else
                    {
                        var setMreq = new SetMetadataRequest();
                        setMreq.metadata = data;
                        //setMreq.product = httpParser.Product;
                        setMreq.product = "est";
                        response = opsConsoleSvc.SetMetadataEx(setMreq);
                        if (response == null)
                        {
                            response = new MetadataResponse() { success = false, description = "Failed - no response from the OpsConsole service" };
                        }
                    }
                }
            }
            string returnedResult = string.Empty;
            returnedResult = JsonConvert.SerializeObject(response);
            context["AS.ResponseBody"] = returnedResult;
        }

        public void DownloadStatData(IDictionary<string, object> context)
        {
            string body = (string)context["AS.RequestBody"];
            string encodedJson = body.Replace("json=", "");
            string json = HttpUtility.UrlDecode(encodedJson);

            // Load the incoming request into the request params object (for easy passing to the half dozen functions that need the parameters)
            var req = JsonConvert.DeserializeObject<GetStatsRequest>(json);
            req.product = "est";
            string fileName = string.Format("SystemReport-{0}-{1}.csv", req.getStatHistoryFor, DateTime.UtcNow.ToString("yyyyMMdd-HHmmss'Z'"));
            context["AS.ResponseContentType"] = "application/vnd.ms-excel";
            var headers = (Dictionary<string, string>)context["AS.ResponseHeaders"];
            headers.Add("Content-Disposition", string.Format("attachment; filename=\"{0}\"", fileName));
            headers.Add("Content-Type", "application/vnd.ms-excel");

            var user = ((IAaaUser)context["AS.Services.ThomsonReuters.Eikon.Toolkit.Interfaces.IAaaUser"]);
            //var product = string.IsNullOrEmpty(req.product) ? null : new List<string>() { req.product };
            var product = new List<string> { "est" };
            if (Permission.IsAllowToGetStats(user, product) && Permission.IsUserInScope(user, req.uuid))
            {
                GetStatsResponse statResponse = null;
                req.getAllStats = true;
                using (var opsConsoleSvc = new OpsConsoleServiceClient(RouterBindings.Local, RouterAddresses.Local.RequestReply))
                {
                    try
                    {
                        statResponse = opsConsoleSvc.GetStats(req);
                        if (statResponse != null)
                        {
                            StringBuilder data = new StringBuilder();
                            try
                            {
                                StatItem stat = statResponse.stats.Find(s => s.statName == req.getStatHistoryFor);
                                data.AppendFormat("Name,{0}\n", stat.statName);
                                data.AppendFormat("Display Name,{0}\n", stat.statDisplayName);
                                data.AppendFormat("Unit,{0}\n", stat.unit);
                                data.AppendFormat("Discard Incomming data,{0}\n", stat.discard ? "true" : "false");
                                data.AppendFormat("Hidden in UI,{0}\n", stat.hidden ? "true" : "false");
                            }
                            catch (Exception)
                            {
                                data.AppendLine("Name,N/A");
                                data.AppendFormat("Display Name,{0}\n", req.getStatHistoryFor);
                                data.AppendLine("Unit,N/A");
                                data.AppendLine("Discard Incomming data,N/A");
                                data.AppendLine("Hidden in UI,N/A");
                            }
                            data.AppendLine();
                            data.AppendLine("Timestamp,Value");
                            foreach (var row in statResponse.history.rows)
                            {
                                DateTime dt = DateTime.Parse(row.timeStamp);
                                data.AppendFormat("{0},{1}\n", dt.ToString("u"), row.statVal);
                            }
                            context["AS.ResponseBody"] = data.ToString();
                        }
                        else
                        {
                            context["AS.ResponseBody"] = "null";
                        }
                    }
                    catch (Exception)
                    {
                        context["AS.ResponseBody"] = "Error while invoking GetStats";
                    }
                }
            }
            else
            {
                context["AS.ResponseBody"] = "No permission to get stats";
            }
        }

        public void DownloadLocationStatData(IDictionary<string, object> context)
        {
            string body = (string)context["AS.RequestBody"];
            string encodedJson = body.Replace("json=", "");
            string json = HttpUtility.UrlDecode(encodedJson);

            // Load the incoming request into the request params object (for easy passing to the half dozen functions that need the parameters)
            var req = JsonConvert.DeserializeObject<GetLocationStatDumpRequest>(json);
            req.product = "est";
            string fileName = string.Format("SystemReportLoc-{0}-{1}.csv", req.locationid, DateTime.UtcNow.ToString("yyyyMMdd-HHmmss'Z'"));
            context["AS.ResponseContentType"] = "application/vnd.ms-excel";
            var headers = (Dictionary<string, string>)context["AS.ResponseHeaders"];
            headers.Add("Content-Disposition", string.Format("attachment; filename=\"{0}\"", fileName));
            headers.Add("Content-Type", "application/vnd.ms-excel");

            var user = ((IAaaUser)context["AS.Services.ThomsonReuters.Eikon.Toolkit.Interfaces.IAaaUser"]);            
            var product = new List<string> { "est" };
            if (Permission.IsAllowToGetStats(user, product) && Permission.IsLocationInScope(user, req.locationid))
            {
                using (var opsConsoleSvc = new OpsConsoleServiceClient(RouterBindings.Local, RouterAddresses.Local.RequestReply))
                {
                    try
                    {
                        GetLocationStatDumpResponse statResponse = opsConsoleSvc.GetLocationStatDump(req);
                        if (statResponse != null && statResponse.stats != null)
                        {
                            var data = new StringBuilder();
                            try
                            {
                                data.AppendFormat("Name,{0}\n", statResponse.locationInfo.name);
                                data.AppendFormat("Location ID,{0}\n", statResponse.locationInfo.locationID);
                                data.AppendFormat("Address,{0}\n", statResponse.locationInfo.address);
                                data.AppendFormat("City,{0}\n", statResponse.locationInfo.city);
                                data.AppendFormat("Country,{0}\n", statResponse.locationInfo.country);
                            }
                            catch (Exception)
                            {
                                data.AppendLine("Name,N/A");
                                data.AppendFormat("Location ID,{0}\n", statResponse.locationInfo.locationID);
                                data.AppendLine("Address,N/A");
                                data.AppendLine("City,N/A");
                                data.AppendLine("Country,N/A");
                            }
                            data.AppendLine();
                            data.AppendLine("Timestamp,UUID,Email,First Name, Last Name, Host Name, Stat Name, Stat Value");
                            foreach (var usr in statResponse.stats)
                            {
                                foreach (var stat in usr.stats)
                                {
                                    var dt = DateTime.Parse(stat.timeStamp);
                                    data.AppendFormat("{0},{1},{2},{3},{4},{5},{6},\"{7}\"\n", 
                                        dt.ToString("u"), usr.uuid, usr.email, usr.firstName, usr.lastName, usr.hostName, 
                                        stat.statDisplayName, stat.statLatestVal);
                                }
                            }
                            context["AS.ResponseBody"] = data.ToString();
                        }
                        else
                        {
                            context["AS.ResponseBody"] = "null";
                        }
                    }
                    catch (Exception ex)
                    {
                        context["AS.ResponseBody"] = "Error while invoking GetLocationStatDump";
                        Logger.Default.LogError("GetLocationStatDump: exception {0}", ex.Message);
                    }
                }
            }
            else
            {
                context["AS.ResponseBody"] = "No permission to get DownloadLocationStatData";
            }
        }

        public string EditStat(string query, string body, IAppServerServices services)
        {
            var req = JsonConvert.DeserializeObject<EditStatRequest>(body);
            req.product = "est";
            //var product = string.IsNullOrEmpty(req.product) ? null : new List<string>() { req.product };
            var product = new List<string> { "est" };
            if (Permission.IsAllowToUploadMetadata(services.UserContext, product))
            {
                using (var opsConsoleSvc = new OpsConsoleServiceClient(RouterBindings.Local, RouterAddresses.Local.RequestReply))
                {
                    try
                    {
                        var response = opsConsoleSvc.EditStat(req);

                        if (response == null)
                        {
                            services.Logger.LogError("STOpsConsole - EditStat returned null");
                            return "{}";
                        }
                        return JsonConvert.SerializeObject(response);
                    }
                    catch (Exception ex)
                    {
                        services.Logger.LogError("STOpsConsole - EditStat -  Caught exception: {0}", ex.Message);
                        return "{}";
                    }
                }
            }
            else
            {
                return JsonConvert.SerializeObject(new EditStatResponse { success = false });
            }
        }

        public string GetMetadata(string query, string body, IAppServerServices services)
        {
            var logger = services.Logger;
            try
            {
                using (var client = new OpsConsoleServiceClient(RouterBindings.Local, RouterAddresses.Local.RequestReply))
                {
                    var metadataReq = JsonConvert.DeserializeObject<GetMetadataRequest>(body) ?? new GetMetadataRequest();
                    metadataReq.product = "est";
                    metadataReq.disable = Permission.GetDisableTests(services.UserContext);
                    var metadata = client.GetMetadata(metadataReq);
                    return string.IsNullOrEmpty(metadata) ? "{}" : metadata;
                }
            }
            catch (Exception ex)
            {
                logger.LogError("Error while getting metadata: {0}", ex.Message);
                return "{}";
            }
        }

        public string GetMetadataVersion(string query, string body, IAppServerServices services)
        {
            var logger = services.Logger;
            try
            {
                using (var client = new OpsConsoleServiceClient(RouterBindings.Local, RouterAddresses.Local.RequestReply))
                {
                    var metadataVersionReq = JsonConvert.DeserializeObject<GetMetadataVersion>(body);
                    metadataVersionReq.product = "est";
                    var version = client.GetMetadataVersion(metadataVersionReq);
                    int returnVersion = 0;
                    if (version != null)
                    {
                        returnVersion = version.GetValueOrDefault();
                    }
                    return string.Format("{{\"version\": {0}}}", returnVersion);
                }
            }
            catch (Exception ex)
            {
                logger.LogError("Error while getting metadata: {0}", ex.Message);
                return "{}";
            }
        }

        public string GetMachineInstallInfo(string query, string body, IAppServerServices services)
        {
            var logger = services.Logger;
            using (var client = new OpsConsoleServiceClient(RouterBindings.Local, RouterAddresses.Local.RequestReply))
            {
                try
                {
                    var machInstReq = JsonConvert.DeserializeObject<MachInstInfoRequest>(body);
                    machInstReq.product = "est";
                    var response = client.GetMachineInstallInfo(machInstReq);

                    return response == null ? "{ \"items\":[]}" : JsonConvert.SerializeObject(response);
                }
                catch (Exception ex)
                {
                    logger.LogError("STOpsConsole - GetMachineInstallInfo - Caught exception: " + ex.Message);
                    return "{}";
                }
            }
        }

        public string FindUser(string query, string body, IAppServerServices services)
        {
            var logger = services.Logger;
            try
            {
                // Load the incoming request into the request params object (for easy passing to the half dozen functions that need the parameters)
                var req = JsonConvert.DeserializeObject<FindUserRequest>(body);
                req.Filter = FindUserFilter.All;

                using (var userInfoServiceclient = new UserInfoServiceClient(RouterBindings.Local, RouterAddresses.Local.RequestReply))
                {
                    var svcResp = userInfoServiceclient.FindUser(req);

                    return (svcResp == null) ? "{ \"Users\":[]}" : JsonConvert.SerializeObject(svcResp);
                }
            }
            catch (Exception ex)
            {
                logger.LogError("STOpsConsole - FindUser - Caught exception: " + ex.Message);
                return "{ }";
            }
        }

        public string FindMachineInstall(string query, string body, IAppServerServices services)
        {
            IDictionary<string, FindUserEntity> userDetailDic = new Dictionary<string, FindUserEntity>();
            var logger = services.Logger;
            try
            {
                // Load the incoming request into the request params object (for easy passing to the half dozen functions that need the parameters)
                var req = JsonConvert.DeserializeObject<FindMachineInstallRequest>(body);
                return FindMachineInstall(req, services.UserContext, logger);

            }
            catch (Exception ex)
            {
                logger.LogError("STOpsConsole - FindMachineInstall - Caught exception: " + ex.Message);
                return "{ }";
            }
        }

        private string FindMachineInstall(FindMachineInstallRequest req, IAaaUser aaaUser, ILogger logger)
        {
            IDictionary<string, FindUserEntity> userDetailDic = new Dictionary<string, FindUserEntity>();
            req.Product = "est";
            var findUserReq = new FindUserRequest
            {
                Filter = FindUserFilter.All,
                SearchString = req.SearchString,
            };


            // For external user, the auto suggest will only show the users under the user's location scope
            var canOnlySeeYourOwnAccount = false;
            if (!Permission.IsUserInternal(aaaUser))
            {
                var scope = Permission.GetTopLocationScope(aaaUser);
                if (!scope.Equals(default(KeyValuePair<FindLocationFilter, string>)))
                {
                    findUserReq.LocationScope = scope;
                }
                else if (aaaUser.UserId.Contains(req.SearchString) ||
                        aaaUser.EmailAddress.Contains(req.SearchString) ||
                        aaaUser.UUID.Contains(req.SearchString) ||
                        aaaUser.FullName.Contains(req.SearchString))
                {
                    canOnlySeeYourOwnAccount = true;
                }
                else
                {
                    logger.LogWarn("STOpsConsole - FindMachineInstall - external user {0} has no eligible scope.", aaaUser.UserId);
                    return "{ \"items\":[]}";
                }
            }

            using (var userInfoServiceclient = new UserInfoServiceClient(RouterBindings.Local, RouterAddresses.Local.RequestReply))
            {
                var svcResp = userInfoServiceclient.FindUser(findUserReq);

                if (svcResp == null || svcResp.Users.Count == 0)
                {
                    return "{ \"items\":[]}";
                }

                var machInstReq = new MachInstInfoRequest
                {
                    uuids = new List<string>(),
                    filter = req.Filter,
                    product = req.Product
                };

                //If user can see only his own account due to the scope. Will filter out the list of find user.
                if (canOnlySeeYourOwnAccount)
                {
                    var user = svcResp.Users.SingleOrDefault(x => x.Uuid == aaaUser.UUID);
                    if (user != null)
                    {
                        userDetailDic[user.Uuid] = user;
                        machInstReq.uuids.Add(user.Uuid);
                    }
                    else
                    {
                        return "{ \"items\":[]}";
                    }
                }
                else
                {
                    foreach (FindUserEntity user in svcResp.Users)
                    {
                        userDetailDic[user.Uuid] = user;
                        machInstReq.uuids.Add(user.Uuid);
                    }
                }

                FindMachInstResponse findMachInstResponse = new FindMachInstResponse() { Items = new List<FindMachInstInfoItem>() };
                using (var opsConsoleServiceClient = new OpsConsoleServiceClient(RouterBindings.Local, RouterAddresses.Local.RequestReply))
                {
                    MachInstInfoResponse machInstResp = opsConsoleServiceClient.GetMachineInstallInfo(machInstReq);
                    foreach (MachInstInfoItem machInsInfo in machInstResp.Items)
                    {

                        FindMachInstInfoItem findMachInstInfoItem = new FindMachInstInfoItem
                        {
                            UUID = machInsInfo.uuid,
                            FirstName = userDetailDic[machInsInfo.uuid].FirstName,
                            LastName = userDetailDic[machInsInfo.uuid].LastName,
                            EmailAddress = userDetailDic[machInsInfo.uuid].Email
                            //MachInstInfoList = machInsInfo.machInstInfoList
                        };
                        findMachInstResponse.Items.Add(findMachInstInfoItem);
                        findMachInstResponse.Product = machInstResp.product;

                    }
                }
                return JsonConvert.SerializeObject(findMachInstResponse) ?? "{}";
            }
        }

        public string GetLocationStatDetail(string query, string body, IAppServerServices services)
        {
            var req = JsonConvert.DeserializeObject<GetLocationStatDetailRequest>(body);
            req.product = "est";
            //var product = string.IsNullOrEmpty(req.product) ? null : new List<string>() { req.product };
            var product = new List<string> { "est" };
            if (Permission.IsAllowToGetStats(services.UserContext, product) && Permission.IsLocationInScope(services.UserContext, req.locationID))
            {
                using (var opsConsoleSvc = new OpsConsoleServiceClient(RouterBindings.Local, RouterAddresses.Local.RequestReply))
                {
                    try
                    {
                        var response = opsConsoleSvc.GetLocationStatDetail(req);
                        if (response == null)
                        {
                            services.Logger.LogError("WebCallableFunctions.GetLocationStatDetail() - null is returned");
                            return "{}";
                        }
                        return JsonConvert.SerializeObject(response);
                    }
                    catch (Exception ex)
                    {
                        services.Logger.LogError("WebCallableFunctions.GetLocationStatDetail() - Exception: {0}", ex.Message);
                        return "{}";
                    }
                }
            }
            else
            {
                return "{}";
            }
        }

        public string GetTop100LocationAtRisk(string query, string body, IAppServerServices services)
        {
            var req = JsonConvert.DeserializeObject<GetTop100LocationAtRiskRequest>(body);
            req.product = "est";
            //var product = string.IsNullOrEmpty(req.product) ? null : new List<string>() { req.product };
            var product = new List<string> { "est" };


            // For external user, the location dashboard will only show the users under the user's location scope
            if (!Permission.IsUserInternal(services.UserContext))
            {
                var scope = Permission.GetTopLocationScope(services.UserContext);

                if (!scope.Equals(default(KeyValuePair<FindLocationFilter, string>)))
                {
                    var temp = 0;
                    switch (scope.Key)
                    {
                        case FindLocationFilter.LOC:
                            temp = 1;
                            break;
                        case FindLocationFilter.LGL:
                            temp = 2;
                            break;
                        case FindLocationFilter.ULT:
                            temp = 3;
                            break;
                    }
                    req.locationScope = new KeyValuePair<int, string>(temp, scope.Value);
                }
                else
                {
                    services.Logger.LogWarn("WebCallableFunctions.GetTop100LocationAtRisk() - External user without scope {0} - {1}", services.UserContext.UUID, scope.ToString());
                    return "{}";
                }
            }

            if (Permission.IsAllowToGetStats(services.UserContext, product))
            {
                using (var opsConsoleSvc = new OpsConsoleServiceClient(RouterBindings.Local, RouterAddresses.Local.RequestReply))
                {
                    try
                    {
                        var response = opsConsoleSvc.GetTop100LocationAtRisk(req);
                        if (response == null)
                        {
                            services.Logger.LogError("WebCallableFunctions.GetTop100LocationAtRisk() - null is returned");
                            return "{}";
                        }
                        return JsonConvert.SerializeObject(response);
                    }
                    catch (Exception ex)
                    {
                        services.Logger.LogError("WebCallableFunctions.GetTop100LocationAtRisk() - Exception: {0}", ex.Message);
                        return "{}";
                    }
                }
            }
            else
            {
                return "{}";
            }
        }

        public string GetLocationStatus(string query, string body, IAppServerServices services)
        {
            var req = JsonConvert.DeserializeObject<GetLocationStatusRequest>(body);
            req.product = "est";
            //var product = string.IsNullOrEmpty(req.product) ? null : new List<string>() { req.product };
            var product = new List<string> { "est" };
            if (Permission.IsAllowToGetStats(services.UserContext, product))
            {
                using (var opsConsoleSvc = new OpsConsoleServiceClient(RouterBindings.Local, RouterAddresses.Local.RequestReply))
                {
                    try
                    {
                        var response = opsConsoleSvc.GetLocationStatus(req);
                        if (response == null)
                        {
                            services.Logger.LogError("WebCallableFunctions.GetLocationStatus() - null is returned");
                            return "{}";
                        }
                        return JsonConvert.SerializeObject(response);
                    }
                    catch (Exception ex)
                    {
                        services.Logger.LogError("WebCallableFunctions.GetLocationStatus() - Exception: {0}", ex.Message);
                        return "{}";
                    }
                }
            }
            else
            {
                return "{}";
            }
        }

        public string GetCriticalUsers(string query, string body, IAppServerServices services)
        {
            var req = JsonConvert.DeserializeObject<GetCriticalUsersRequest>(body);
            req.product = "est";
            var product = new List<string> { "est" };
            if (Permission.IsAllowToGetStats(services.UserContext, product) && Permission.IsUserInScope(services.UserContext, req.uuid))
            {
                using (var opsConsoleSvc = new OpsConsoleServiceClient(RouterBindings.Local, RouterAddresses.Local.RequestReply))
                {
                    try
                    {
                        var response = opsConsoleSvc.GetCriticalUsers(req);
                        if (response == null)
                        {
                            services.Logger.LogError("WebCallableFunctions.GetCriticalUsers() - null is returned");
                            return "{}";
                        }
                        return JsonConvert.SerializeObject(response);
                    }
                    catch (Exception ex)
                    {
                        services.Logger.LogError("WebCallableFunctions.GetCriticalUsers() - Exception: {0}", ex.Message);
                        return "{}";
                    }
                }
            }
            else
            {
                return "{}";
            }
        }

        // Temp method
        public string GetUserInfo(string query, string body, IAppServerServices services)
        {
            var logger = services.Logger;
            try
            {
                // Load the incoming request into the request params object (for easy passing to the half dozen functions that need the parameters)
                var req = JsonConvert.DeserializeObject<GetUserInfoRequest>(body);
                using (var client = new OpsConsoleServiceClient(RouterBindings.Local, RouterAddresses.Local.RequestReply))
                {
                    var res = client.GetUserInfo(req);
                    return JsonConvert.SerializeObject(res);
                }
            }
            catch (Exception ex)
            {
                logger.LogError("WebCallableFunctions.GetUserInfo() - Exception: {0}" + ex.Message);
                return "{ }";
            }
        }

        // Temp method
        public string GetLocationInfo(string query, string body, IAppServerServices services)
        {
            var logger = services.Logger;
            try
            {
                // Load the incoming request into the request params object (for easy passing to the half dozen functions that need the parameters)
                var req = JsonConvert.DeserializeObject<GetLocationInfoRequest>(body);
                using (var client = new OpsConsoleServiceClient(RouterBindings.Local, RouterAddresses.Local.RequestReply))
                {
                    var res = client.GetLocationInfo(req);
                    return JsonConvert.SerializeObject(res);
                }
            }
            catch (Exception ex)
            {
                logger.LogError("WebCallableFunctions.GetLocationInfo() - Exception: {0}" + ex.Message);
                return "{ }";
            }
        }

        public string GetAggStatMetadata(string query, string body, IAppServerServices services)
        {
            var logger = services.Logger;
            var req = JsonConvert.DeserializeObject<GetAggStatMetadataRequest>(body);
            req.product = "est";
            try
            {
                using (var client = new OpsConsoleServiceClient(RouterBindings.Local, RouterAddresses.Local.RequestReply))
                {
                    //var metadata = client.GetAggStatMetadata(new GetAggStatMetadataRequest());
                    var metadata = client.GetAggStatMetadata(req);
                    return (metadata != null) ? JsonConvert.SerializeObject(metadata, Formatting.None, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }) : "{}";
                }
            }
            catch (Exception ex)
            {
                logger.LogError("WebCallableFunctions.GetAggStatMetadata() - Exception: {0}", ex.Message);
                return "{}";
            }
        }

        public void UploadAggStatMetadata(IDictionary<string, object> context)
        {
            var user = ((IAaaUser)context["AS.Services.ThomsonReuters.Eikon.Toolkit.Interfaces.IAaaUser"]);
            MetadataResponse response = null;

            string body = (string)context["AS.RequestBody"];
            HttpFormFileContentPaarser httpParser = new HttpFormFileContentPaarser(body, "metadata");

            //var product = string.IsNullOrEmpty(httpParser.Product) ? null : new List<string>() { httpParser.Product };
            var product = new List<string> { "est" };

            if (!Permission.IsAllowToUploadMetadata(user, product))
            {
                response = new MetadataResponse { success = false, description = "Failed - no permission to upload the metadata" };
            }
            else
            {
                using (var opsConsoleSvc = new OpsConsoleServiceClient(RouterBindings.Local, RouterAddresses.Local.RequestReply))
                {
                    string data = httpParser.FileContents.Replace("\r\n", string.Empty).Replace("\t", string.Empty).Trim();
                    if (string.IsNullOrEmpty(data))
                    {
                        response = new MetadataResponse { success = false, description = "Failed - no metadata content" };
                    }
                    else
                    {
                        try
                        {
                            var req = JsonConvert.DeserializeObject<SetAggStatMetadataRequest>(data);
                            //req.product = httpParser.Product;
                            req.product = "est";
                            response = opsConsoleSvc.SetAggStatMetadata(req);
                        }
                        catch (Exception ex)
                        {
                            response = new MetadataResponse { success = false, description = "Failed - " + ex.Message };
                        }
                    }
                }
            }
            string returnedResult = string.Empty;
            returnedResult = JsonConvert.SerializeObject(response);
            context["AS.ResponseBody"] = returnedResult;
        }
    }
}

