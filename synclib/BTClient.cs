using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using POCCamera.Models;
using RestSharp;

namespace Synclib
{
    public class BTClient
    {
        readonly RestClient client;

        static RestRequest CreateDefault(string res)
        {
            return new RestRequest
            {
                Resource = res,
                Method = Method.GET
            };
        }

        public BTClient(string baseURL, string username, string password)
        {
            client = new RestClient(baseURL)
            {
                Authenticator = new HttpBasicAuthenticator(username, password)
            };
        }

        public async Task<T> Execute<T>(RestRequest request, HttpStatusCode expectedResponseCode) where T : new()
        {
            var response = await client.ExecuteGetTaskAsync<T>(request);

            if (response.ResponseStatus != ResponseStatus.Completed || response.ErrorException != null)
            {
                Console.WriteLine("RestSharp response status: {0} - HTTP response: {1} - {2} - {3}",
                    response.ResponseStatus, response.StatusCode, response.StatusDescription, response.Content);
                return default(T);
            }
            else
                return response.Data;
        }

        public async Task<Response> AddFolder(string directory, string secret)
        {
            var request = CreateDefault(string.Format("/api?method=add_folder&dir={0}&secret={1}", directory, secret));

            return await Execute<Response>(request, HttpStatusCode.OK);
        }

        public async Task<List<Folder>> GetFolders(string secret)
        {
            var request = CreateDefault("/api?method=get_folders");
            if (secret.Length > 0)
                request.Resource += "&secret=" + secret;

            return await Execute<List<Folder>>(request, HttpStatusCode.OK);
        }

        public async Task<Response> RemoveFolder(string secret)
        {
            var request = CreateDefault(string.Format("/api?method=remove_folder&secret={0}", secret));
            return await Execute<Response>(request, HttpStatusCode.OK);
        }

        public async Task<Secrets> GetSecrets()
        {
            var request = CreateDefault("/api?method=get_secrets");
            return await Execute<Secrets>(request, HttpStatusCode.OK);
        }

        public async Task<List<FolderItem>> GetFiles(string secret, string path)
        {
            var request = CreateDefault(string.Format("/api?method=get_files&secret={0}", secret));
            if (path.Length > 0)
                request.Resource += "&path=" + path;

            return await Execute<List<FolderItem>>(request, HttpStatusCode.OK);
        }

        public async Task<Preferences> GetPreferences()
        {
            var request = CreateDefault("/api?method=get_prefs");
            return await Execute<Preferences>(request, HttpStatusCode.OK);
        }

        public async Task<OSNameResponse> GetOSName()
        {
            var request = CreateDefault("/api?method=get_os");

            return await Execute<OSNameResponse>(request, HttpStatusCode.OK);
        }

        public async Task<ClientVersion> GetVersion()
        {
            var request = CreateDefault("/api?method=get_version");

            return await Execute<ClientVersion>(request, HttpStatusCode.OK);
        }

        public async Task<SpeedResponse> GetSpeed()
        {
            var request = CreateDefault("/api?method=get_speed");

            return await Execute<SpeedResponse>(request, HttpStatusCode.OK);
        }

        public async Task<Response> Shutdown()
        {
            var request = CreateDefault("/api?method=shutdown");

            return await Execute<Response>(request, HttpStatusCode.OK);
        }

        public async Task<Response> SetFilePreferences(string secret, string path, bool download)
        {
            var request = CreateDefault(string.Format("/api?method=set_file_prefs&secret={0}&path={1}&download=", secret, path));
            request.Resource += download ? "1" : "0";

            return await Execute<Response>(request, HttpStatusCode.OK);
        }

        public async Task<List<FolderPeer>> GetFolderPeers(string secret)
        {
            var request = CreateDefault(string.Format("/api?method=get_folder_peers&secret={0}", secret));

            return await Execute<List<FolderPeer>>(request, HttpStatusCode.OK);
        }

        public async Task<FolderPreferences> GetFolderPreferences(string secret)
        {
            var request = CreateDefault(string.Format("/api?method=get_folder_prefs&secret={0}", secret));

            return await Execute<FolderPreferences>(request, HttpStatusCode.OK);
        }

        public async Task<Response> SetFolderPreferences(string secret, FolderPreferences folderPreferences)
        {
            var request = CreateDefault(string.Format("/api?method=set_folder_prefs&secret={0}", secret));
            request.AddParameter(new Parameter()
                {
                    Name = "search_lan",
                    Value = folderPreferences.search_lan,
                    Type = ParameterType.GetOrPost
                });
            request.AddParameter(new Parameter()
                {
                    Name = "use_dht",
                    Value = folderPreferences.use_dht,
                    Type = ParameterType.GetOrPost
                });
            request.AddParameter(new Parameter()
                {
                    Name = "use_hosts",
                    Value = folderPreferences.use_hosts,
                    Type = ParameterType.GetOrPost
                });
            request.AddParameter(new Parameter()
                {
                    Name = "use_relay_server",
                    Value = folderPreferences.use_relay_server,
                    Type = ParameterType.GetOrPost
                });
            request.AddParameter(new Parameter()
                {
                    Name = "use_sync_trash",
                    Value = folderPreferences.use_sync_trash,
                    Type = ParameterType.GetOrPost
                });
            request.AddParameter(new Parameter()
                {
                    Name = "use_tracker",
                    Value = folderPreferences.use_tracker,
                    Type = ParameterType.GetOrPost
                });

            return await Execute<Response>(request, HttpStatusCode.OK);
        }

        public async Task<FolderHosts> GetFolderHosts(string secret, FolderHosts hosts)
        {
            var request = CreateDefault(string.Format("/api?method=get_folder_hosts&secret={0}", secret));

            return await Execute<FolderHosts>(request, HttpStatusCode.OK);
        }

        public async Task<Response> SetFolderHosts(string secret, FolderHosts hosts)
        {
            var request = CreateDefault(string.Format("/api?method=set_folder_hosts&secret={0}&hosts=", secret));
            bool first = true;
            foreach (string hostEntry in hosts.hosts)
            {
                if (first)
                {
                    request.Resource += ",";
                    first = false;
                }
                request.Resource += hostEntry;
            }

            return await Execute<Response>(request, HttpStatusCode.OK);
        }

        public async Task<Response> SetPreferences(string secret, Preferences prefs)
        {
            var request = CreateDefault(string.Format("/api?method=set_prefs&secret={0}", secret));
            request.AddParameter(new Parameter()
                {
                    Name = "device_name",
                    Value = prefs.device_name,
                    Type = ParameterType.GetOrPost
                });
            request.AddParameter(new Parameter()
                {
                    Name = "disk_low_priority",
                    Value = prefs.disk_low_priority,
                    Type = ParameterType.GetOrPost
                });
            request.AddParameter(new Parameter()
                {
                    Name = "download_limit",
                    Value = prefs.download_limit,
                    Type = ParameterType.GetOrPost
                });
            request.AddParameter(new Parameter()
                {
                    Name = "folder_rescan_interval",
                    Value = prefs.folder_rescan_interval,
                    Type = ParameterType.GetOrPost
                });
            request.AddParameter(new Parameter()
                {
                    Name = "lan_encrypt_data",
                    Value = prefs.lan_encrypt_data,
                    Type = ParameterType.GetOrPost
                });
            request.AddParameter(new Parameter()
                {
                    Name = "lan_use_tcp",
                    Value = prefs.lan_use_tcp,
                    Type = ParameterType.GetOrPost
                });
            request.AddParameter(new Parameter()
                {
                    Name = "lang",
                    Value = prefs.lang,
                    Type = ParameterType.GetOrPost
                });
            request.AddParameter(new Parameter()
                {
                    Name = "listening_port",
                    Value = prefs.listening_port,
                    Type = ParameterType.GetOrPost
                });
            request.AddParameter(new Parameter()
                {
                    Name = "max_file_size_diff_for_patching",
                    Value = prefs.max_file_size_diff_for_patching,
                    Type = ParameterType.GetOrPost
                });
            request.AddParameter(new Parameter()
                {
                    Name = "max_file_size_for_versioning",
                    Value = prefs.max_file_size_for_versioning,
                    Type = ParameterType.GetOrPost
                });
            request.AddParameter(new Parameter()
                {
                    Name = "rate_limit_local_peers",
                    Value = prefs.rate_limit_local_peers,
                    Type = ParameterType.GetOrPost
                });
            request.AddParameter(new Parameter()
                {
                    Name = "recv_buf_size",
                    Value = prefs.recv_buf_size,
                    Type = ParameterType.GetOrPost
                });
            request.AddParameter(new Parameter()
                {
                    Name = "send_buf_size",
                    Value = prefs.send_buf_size,
                    Type = ParameterType.GetOrPost
                });
            request.AddParameter(new Parameter()
                {
                    Name = "sync_max_time_diff",
                    Value = prefs.sync_max_time_diff,
                    Type = ParameterType.GetOrPost
                });
            request.AddParameter(new Parameter()
                {
                    Name = "sync_trash_ttl",
                    Value = prefs.sync_trash_ttl,
                    Type = ParameterType.GetOrPost
                });
            request.AddParameter(new Parameter()
                {
                    Name = "upload_limit",
                    Value = prefs.upload_limit,
                    Type = ParameterType.GetOrPost
                });
            request.AddParameter(new Parameter()
                {
                    Name = "use_upnp",
                    Value = prefs.use_upnp,
                    Type = ParameterType.GetOrPost
                });

            return await Execute<Response>(request, HttpStatusCode.OK);
        }
    }
}
