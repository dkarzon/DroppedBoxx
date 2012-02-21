using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using DroppedBoxx.Web;
using System.Text.RegularExpressions;
using DroppedBoxx.Models;
using System.IO;
using DroppedBoxx.Code.Responses;
using System.Net;
using System.Windows.Forms;
using DroppedBoxx.Code.Exceptions;

namespace DroppedBoxx.Code
{
    public class DropBox
    {
#warning Put your API Key here...
        private string _apiKey = "API_KEY";
        private string _appsecret = "API_SECRET";

        public UserLogin UserLogin;
        private AccountInfo _accountInfo;
        private DateTime _lastAccountInfo = DateTime.MinValue;

        public long BytesSent { get; set; }
        public long BytesRecieved { get; set; }

        public bool LoggedIn { get; set; }

        public bool Login(string email, string password)
        {
            Http http = new Http("get");
            http.Url = new Uri("https://api.dropbox.com/0/token");

            http.Parameters.Add(new HttpParameter { Name = "oauth_consumer_key", Value = _apiKey });

            http.Parameters.Add(new HttpParameter { Name = "email", Value = email });
            http.Parameters.Add(new HttpParameter { Name = "password", Value = password });

            //add oauth
            OAuthAuthenticator oAuth = new OAuthAuthenticator(http.Url.ToString(), _apiKey, _appsecret, "", "");
            oAuth.Authenticate(http);

            try
            {
                http.Get();

                if (http.Response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    //Wrong Username/Password
                    throw new DropException("Incorrect Username/Password.");
                }

                BytesRecieved += http.Response.Content.Length;

                JsonDeserializer deserializer = new JsonDeserializer();

                UserLogin = deserializer.Deserialize<UserLogin>(http.Response.Content);

                if (!string.IsNullOrEmpty(UserLogin.Error))
                {
                    //Some sort of dropbox error
                    throw new DropException(UserLogin.Error);
                }

                LoggedIn = true;

                return LoggedIn;
            }
            catch (DropException dx)
            {
                throw dx;
            }
            catch (Exception ex)
            {
                return false;
            }

        }

        public AccountInfo GetAccountInfo()
        {
            if (_accountInfo != null)
            {
                var lastDiff = DateTime.Now - _lastAccountInfo;
                //May need Tweeking
                if (lastDiff.TotalSeconds < 100) return _accountInfo;
            }

            Http http = new Http("get");
            http.Url = new Uri("https://api.dropbox.com/0/account/info");

            OAuthAuthenticator oAuth = new OAuthAuthenticator(http.Url.ToString(), _apiKey, _appsecret, UserLogin.Token, UserLogin.Secret);

            oAuth.Authenticate(http);

            try
            {
                http.Get();

                BytesRecieved += http.Response.Content.Length;

                JsonDeserializer deserializer = new JsonDeserializer();

                _accountInfo = deserializer.Deserialize<AccountInfo>(http.Response.Content);

                return _accountInfo;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public MetaData GetItems()
        {
            return GetItems(new MetaData { Path = "" });
        }

        public MetaData GetItems(MetaData remoteDir)
        {
            Http http = new Http("get");
            http.Url = new Uri("https://api.dropbox.com/0/metadata/dropbox" + remoteDir.Path);

            OAuthAuthenticator oAuth = new OAuthAuthenticator(http.Url.ToString(), _apiKey, _appsecret, UserLogin.Token, UserLogin.Secret);

            oAuth.Authenticate(http);

            try
            {
                http.Get();

                BytesRecieved += http.Response.Content.Length;

                JsonDeserializer deserializer = new JsonDeserializer();

                var returnMeta = deserializer.Deserialize<MetaData>(http.Response.Content);

                if (string.IsNullOrEmpty(returnMeta.error))
                {
                    if (returnMeta.Contents == null) returnMeta.Contents = new List<MetaData>();
                    return returnMeta;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public bool Delete(MetaData remoteFile)
        {
            Http http = new Http("get");
            http.Url = new Uri("https://api.dropbox.com/0/fileops/delete");

            OAuthAuthenticator oAuth = new OAuthAuthenticator(http.Url.ToString(), _apiKey, _appsecret, UserLogin.Token, UserLogin.Secret);

            http.Parameters.Add(new HttpParameter { Name = "path", Value = remoteFile.Path });
            http.Parameters.Add(new HttpParameter { Name = "root", Value = "dropbox" });

            oAuth.Authenticate(http);

            try
            {
                http.Get();

                BytesRecieved += http.Response.Content.Length;

                return http.Response.StatusCode == HttpStatusCode.OK;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool CopyFile(MetaData fromFile, MetaData toDir)
        {
            Http http = new Http("get");
            http.Url = new Uri("https://api.dropbox.com/0/fileops/copy");

            OAuthAuthenticator oAuth = new OAuthAuthenticator(http.Url.ToString(), _apiKey, _appsecret, UserLogin.Token, UserLogin.Secret);

            http.Parameters.Add(new HttpParameter { Name = "root", Value = "dropbox" });
            http.Parameters.Add(new HttpParameter { Name = "from_path", Value = fromFile.Path });
            http.Parameters.Add(new HttpParameter { Name = "to_path", Value = string.Format("{0}/{1}", toDir.Path, fromFile.Name) });

            oAuth.Authenticate(http);

            try
            {
                http.Get();

                BytesRecieved += http.Response.Content.Length;

                return http.Response.StatusCode == HttpStatusCode.OK;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool MoveFile(MetaData fromFile, MetaData toDir)
        {
            Http http = new Http("get");
            http.Url = new Uri("https://api.dropbox.com/0/fileops/move");

            OAuthAuthenticator oAuth = new OAuthAuthenticator(http.Url.ToString(), _apiKey, _appsecret, UserLogin.Token, UserLogin.Secret);

            http.Parameters.Add(new HttpParameter { Name = "root", Value = "dropbox" });
            http.Parameters.Add(new HttpParameter { Name = "from_path", Value = fromFile.Path });
            http.Parameters.Add(new HttpParameter { Name = "to_path", Value = string.Format("{0}/{1}", toDir.Path, fromFile.Name) });

            oAuth.Authenticate(http);

            try
            {
                http.Get();

                BytesRecieved += http.Response.Content.Length;

                return http.Response.StatusCode == HttpStatusCode.OK;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public DropboxFile GetFile(MetaData remoteFile)
        {
            Http http = new Http("get");
            http.Url = new Uri("https://api-content.dropbox.com/0/files/dropbox" + remoteFile.Path);

            OAuthAuthenticator oAuth = new OAuthAuthenticator(http.Url.ToString(), _apiKey, _appsecret, UserLogin.Token, UserLogin.Secret);

            oAuth.Authenticate(http);

            try
            {
                var localFileInfo = new FileInfo(Settings.Instance.TempDirectory + remoteFile.Name);

                if (!localFileInfo.Directory.Exists) localFileInfo.Directory.Create();

                http.GetAndSaveFile(localFileInfo.FullName);

                BytesRecieved += localFileInfo.Length;
                
                //Check for OK status ;)
                if (http.Response.StatusCode != System.Net.HttpStatusCode.OK) return null;

                return new DropboxFile { Name = remoteFile.Name, Path = remoteFile.Path, LocalFileInfo = localFileInfo };
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public bool UploadFile(FileInfo localFile, MetaData remoteDir)
        {
            var path = remoteDir.Path;
            if (!path.StartsWith("/")) path = "/" + path;

            byte[] buff = null;
            FileStream fs = new FileStream(localFile.FullName, FileMode.Open, FileAccess.Read);
            BinaryReader br = new BinaryReader(fs);
            long numBytes = localFile.Length;
            buff = br.ReadBytes((int)numBytes);

            Http http = new Http("post");
            http.Url = new Uri("https://api-content.dropbox.com/0/files/dropbox" + path);

            http.Parameters.Add(new HttpParameter { Name = "file", Value = localFile.Name });

            http.Files.Add(new HttpFile { Parameter = "file", FileName = localFile.Name, ContentType = localFile.Extension, Data = buff });

            OAuthAuthenticator oAuth = new OAuthAuthenticator(http.Url.ToString(), _apiKey, _appsecret, UserLogin.Token, UserLogin.Secret);

            oAuth.Authenticate(http);

            try
            {
                http.Post();

                BytesRecieved += http.Response.Content.Length;
                BytesSent += localFile.Length;

                return http.Response.StatusCode == HttpStatusCode.OK;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public MetaData CreateFolder(MetaData parent, string folderName)
        {
            Http http = new Http("get");
            http.Url = new Uri("https://api.dropbox.com/0/fileops/create_folder");

            OAuthAuthenticator oAuth = new OAuthAuthenticator(http.Url.ToString(), _apiKey, _appsecret, UserLogin.Token, UserLogin.Secret);

            http.Parameters.Add(new HttpParameter { Name = "path", Value = string.Format("{0}/{1}", parent.Path, folderName) });
            http.Parameters.Add(new HttpParameter { Name = "root", Value = "dropbox" });

            oAuth.Authenticate(http);

            try
            {
                http.Get();

                BytesRecieved += http.Response.Content.Length;

                JsonDeserializer deserializer = new JsonDeserializer();

                var returnMeta = deserializer.Deserialize<MetaData>(http.Response.Content);

                if (string.IsNullOrEmpty(returnMeta.error))
                {
                    if (returnMeta.Contents == null) returnMeta.Contents = new List<MetaData>();
                    return returnMeta;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

    }
}