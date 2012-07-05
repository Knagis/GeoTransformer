/*
 * This file is part of GeoTransformer project (http://geotransformer.codeplex.com/).
 * It is licensed under Microsoft Reciprocal License (Ms-RL).
 */
 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using GeoTransformer.Extensions;

namespace GeoTransformer.Transformers.Translator
{
    /// <summary>
    /// Transformer that uses Bing Translator to automatically translate descriptions, hints and logs.
    /// </summary>
    public class Translator : TransformerBase, IConfigurable, ILocalStorage
    {
        /// <summary>
        /// A cached instance of the configuration data, valid while the processing is happening.
        /// </summary>
        private ConfigurationData _processConfigCache;

        /// <summary>
        /// Gets the title of the transformer to display to the user.
        /// </summary>
        public override string Title
        {
            get { return "Translate information"; }
        }

        /// <summary>
        /// Gets the required execution for this transformer. Smaller values indicate that the transformer has to be executed earlier.
        /// The values are not limited to the enumeration.
        /// </summary>
        public override ExecutionOrder ExecutionOrder
        {
            get 
            { 
                // +100 means that the translation will be done after most transformers have done changing the values.
                return Transformers.ExecutionOrder.Process + 100;
            }
        }

        private static string RetrieveAzureToken(string clientId, string clientSecret)
        {
            // the parameters can be retrieved after registering the app on https://datamarket.azure.com/developer/applications

            var url = "https://datamarket.accesscontrol.windows.net/v2/OAuth2-13";
            var body = string.Format("grant_type=client_credentials&client_id={0}&client_secret={1}&scope=http://api.microsofttranslator.com", System.Uri.EscapeDataString(clientId), System.Uri.EscapeDataString(clientSecret));

            var wc = new System.Net.WebClient();
            string result;
            try
            {
                result = wc.UploadString(url, "POST", body);
            }
            catch (System.Net.WebException wex)
            {
                throw new ArgumentException("The specified Client ID or Client Secret are invalid or the authentication server cannot be reached.", wex);
            }

            //{"access_token":"http%3a%2f%2fschemas.xmlsoap.org%2fws%2f2005%2f05%2fidentity%2fclaims%2fnameidentifier=GeoTransformer&http%3a%2f%2fschemas.microsoft.com%2faccesscontrolservice%2f2010%2f07%2fclaims%2fidentityprovider=https%3a%2f%2fdatamarket.accesscontrol.windows.net%2f&Audience=http%3a%2f%2fapi.microsofttranslator.com&ExpiresOn=1320872565&Issuer=https%3a%2f%2fdatamarket.accesscontrol.windows.net%2f&HMACSHA256=K2fAgbGeAANi%2fKGFcx4FJEWKNuyBXJl5Lzv%2f4hsyCYg%3d","token_type":"http://schemas.xmlsoap.org/ws/2009/11/swt-token-profile-1.0","expires_in":"600","scope":"http://api.microsofttranslator.com"}
            var match = new System.Text.RegularExpressions.Regex("\"access_token\":\"(.*?)\"").Match(result);
            if (!match.Success)
                throw new InvalidOperationException("Unable to retrieve the access token from the Azure server.");

            return match.Groups[1].Value;
        }


        private class AuthorizationExtension : System.ServiceModel.Dispatcher.IClientMessageInspector
        {
            public string Token;

            public void AfterReceiveReply(ref System.ServiceModel.Channels.Message reply, object correlationState)
            {
            }

            public object BeforeSendRequest(ref System.ServiceModel.Channels.Message request, System.ServiceModel.IClientChannel channel)
            {
                System.ServiceModel.Channels.HttpRequestMessageProperty header;
                object temp;
                if (!request.Properties.TryGetValue(System.ServiceModel.Channels.HttpRequestMessageProperty.Name, out temp))
                {
                    header = new System.ServiceModel.Channels.HttpRequestMessageProperty();
                    request.Properties.Add(System.ServiceModel.Channels.HttpRequestMessageProperty.Name, header);
                }
                else
                {
                    header = (System.ServiceModel.Channels.HttpRequestMessageProperty)temp;
                }

                header.Headers["Authorization"] = "Bearer " + this.Token;
                return null;
            }
        }
        private class AuthorizationBehavior : System.ServiceModel.Description.IEndpointBehavior
        {
            public string Token;

            public void AddBindingParameters(System.ServiceModel.Description.ServiceEndpoint endpoint, System.ServiceModel.Channels.BindingParameterCollection bindingParameters)
            {
            }

            public void ApplyClientBehavior(System.ServiceModel.Description.ServiceEndpoint endpoint, System.ServiceModel.Dispatcher.ClientRuntime clientRuntime)
            {
                var ext = new AuthorizationExtension() { Token = this.Token };
                clientRuntime.MessageInspectors.Add(ext);
            }

            public void ApplyDispatchBehavior(System.ServiceModel.Description.ServiceEndpoint endpoint, System.ServiceModel.Dispatcher.EndpointDispatcher endpointDispatcher)
            {
            }

            public void Validate(System.ServiceModel.Description.ServiceEndpoint endpoint)
            {
            }
        }

        internal static TranslatorService.LanguageServiceClient CreateServiceWithoutAuthentication()
        {
            var binding = new System.ServiceModel.BasicHttpBinding(System.ServiceModel.BasicHttpSecurityMode.None);
            binding.MaxReceivedMessageSize = 1048576;
            binding.ReaderQuotas.MaxStringContentLength = 1048576;

            var endpoint = new System.ServiceModel.EndpointAddress("http://api.microsofttranslator.com/V2/soap.svc");
            return new TranslatorService.LanguageServiceClient(binding, endpoint);
        }
        internal TranslatorService.LanguageServiceClient CreateService()
        {
            return CreateService(this._processConfigCache.ClientId, this._processConfigCache.ClientSecret);
        }
        internal static TranslatorService.LanguageServiceClient CreateService(string clientId, string clientSecret)
        {
            var binding = new System.ServiceModel.BasicHttpBinding(System.ServiceModel.BasicHttpSecurityMode.None);
            binding.MaxReceivedMessageSize = 1048576;
            binding.ReaderQuotas.MaxStringContentLength = 1048576;

            var token = RetrieveAzureToken(clientId, clientSecret);
            var endpoint = new System.ServiceModel.EndpointAddress(new Uri("http://api.microsofttranslator.com/V2/soap.svc"));
            
            var svc = new TranslatorService.LanguageServiceClient(binding, endpoint);
            svc.Endpoint.Behaviors.Add(new AuthorizationBehavior() { Token = token });

            return svc;
        }

        private string _errorLogFileName;
        private void LogError(string data)
        {
            if (this._errorLogFileName == null)
                this._errorLogFileName = System.IO.Path.Combine(this.LocalStoragePath, "ErrorLog." + DateTime.Now.ToString("yyyyMMdd.HHmmss") + ".txt");

            if (!data.EndsWith(Environment.NewLine))
                data += Environment.NewLine;
            System.IO.File.AppendAllText(this._errorLogFileName, data);
        }

        private void DetectLanguage(IEnumerable<TranslateData> data, TranslatorService.LanguageService service, TranslatorCacheSchema cache)
        {
            double c;
            while ((c = data.Count(o => !o.SourceLanguageCacheTested && o.SourceLanguage == null)) > 0)
            {
                this.ExecutionContext.ThrowIfCancellationPending();
                this.ExecutionContext.ReportStatus((1 - c / data.Count()).ToString("P2") + " of language detection done");

                var subset = data.Where(o => !o.SourceLanguageCacheTested && o.SourceLanguage == null).Take(50).ToList();
                var query = cache.LanguageDetects.Select();
                query.Select(o => o.HashCode, o => o.Language);
                foreach (var x in subset)
                {
                    x.SourceLanguageCacheTested = true;
                    query.Where(o => o.HashCode, x.Hash);
                }
                foreach (var r in query.Execute())
                {
                    subset.First(o => o.Hash == r.Value(x => x.HashCode)).SourceLanguage = r.Value(o => o.Language);
                }
            }

            while ((c = data.Count(o => o.SourceLanguage == null && !o.SourceLanguageError)) > 0)
            {
                this.ExecutionContext.ThrowIfCancellationPending();
                this.ExecutionContext.ReportStatus((1 - c / data.Count()).ToString("P2") + " of language detection done");

                var subset = data.Where(o => o.SourceLanguage == null && !o.SourceLanguageError).Take(5).ToList();
                string[] results;
                try
                {
                    // detect will fail for string longer than 10240. let's assume that first 10KB's will be enough to detect the language.
                    results = service.DetectArray(string.Empty, subset.Select(o => o.Text.Length <= 10240 ? o.Text : o.Text.Substring(0, 10240)).ToArray());
                }
                catch (Exception ex)
                {
                    subset.ForEach(o => o.SourceLanguageError = true);
                    System.Diagnostics.Debug.WriteLine("DetectArray returned error:" + Environment.NewLine + ex.ToString());
                    this.LogError("DetectArray: " + ex.Message);
                    continue;
                }
                using (var tran = cache.Database().BeginTransaction())
                {
                    for (int i = 0; i < results.Length; i++)
                    {
                        subset[i].SourceLanguage = results[i];
                        var uq = cache.LanguageDetects.Replace();
                        uq.Value(o => o.HashCode, subset[i].Hash);
                        uq.Value(o => o.Language, subset[i].SourceLanguage);
                        uq.Execute();
                    }
                    tran.Commit();
                }
            }
        }

        private void Translate(IEnumerable<TranslateData> data, TranslatorService.LanguageService service, TranslatorCacheSchema cache)
        {
            double c;
            while ((c = data.Count(o => !o.SourceLanguageError && !o.TranslationCacheTested && o.Translation == null)) > 0)
            {
                this.ExecutionContext.ThrowIfCancellationPending();
                this.ExecutionContext.ReportStatus((1 - c / data.Count()).ToString("P2") + " of translation done");

                var subset = data.Where(o => !o.SourceLanguageError && !o.TranslationCacheTested && o.Translation == null).Take(50).ToList();
                var query = cache.Translations.Select();
                query.Select(o => o.HashCode, o => o.SourceLanguage, o => o.Translation);
                query.Where(o => o.TargetLanguage, this._processConfigCache.TargetLanguage);
                foreach (var x in subset)
                {
                    x.TranslationCacheTested = true;
                    query.Where(o => o.HashCode, x.Hash);
                }
                foreach (var r in query.Execute())
                {
                    var x = subset.First(o => o.Hash == r.Value(ro => ro.HashCode));
                    if (x.SourceLanguage == r.Value(ro => ro.SourceLanguage))
                        x.Translation = r.Value(o => o.Translation);

                    if (string.IsNullOrWhiteSpace(x.Translation))
                        x.Translation = null;
                }
            }

            foreach (var x in data.Where(o => o.SourceLanguage == this._processConfigCache.TargetLanguage))
                x.Translation = x.Text;

            while ((c = data.Count(o => o.Translation == null && !o.TranslationError && !o.SourceLanguageError)) > 0)
            {
                this.ExecutionContext.ThrowIfCancellationPending();
                this.ExecutionContext.ReportStatus((1 - c / data.Count()).ToString("P2") + " of translation done");

                var subset = data.Where(o => o.Translation == null && !o.TranslationError && !o.SourceLanguageError)
                                 .GroupBy(o => o.SourceLanguage + "-" + o.IsHtml)
                                 .First()
                                 .Take(5)
                                 .ToList();
                GeoTransformer.TranslatorService.TranslateArrayResponse[] results;
                try
                {
                    results = service.TranslateArray(string.Empty,
                                                    subset.Select(o => o.Text.Length <= 10240 ? o.Text : o.Text.Substring(0, 10240)).ToArray(),
                                                    subset[0].SourceLanguage,
                                                    this._processConfigCache.TargetLanguage,
                                                    new TranslatorService.TranslateOptions() { ContentType = subset[0].IsHtml ? "text/html" : "text/plain" });
                }
                catch (Exception ex)
                {
                    subset.ForEach(o => o.TranslationError = true);
                    System.Diagnostics.Debug.WriteLine("TranslateArray returned error:" + Environment.NewLine + ex.ToString());
                    this.LogError("TranslateArray: " + ex.Message);
                    continue;
                }
                using (var tran = cache.Database().BeginTransaction())
                {
                    for (int i = 0; i < results.Length; i++)
                    {
                        if (results[i].Error != null)
                        {
                            subset[i].TranslationError = true;
                            continue;
                        }

                        if (string.IsNullOrWhiteSpace(results[i].TranslatedText))
                            continue;

                        subset[i].Translation = results[i].TranslatedText;
                        var uq = cache.Translations.Replace();
                        uq.Value(o => o.HashCode, subset[i].Hash);
                        uq.Value(o => o.SourceLanguage, subset[i].SourceLanguage);
                        uq.Value(o => o.TargetLanguage, this._processConfigCache.TargetLanguage);
                        uq.Value(o => o.Translation, results[i].TranslatedText);
                        uq.Execute();
                    }
                    tran.Commit();
                }
            }
        }

        private IEnumerable<TranslateData> RetrieveWork(Gpx.GpxDocument document)
        {
            foreach (var wpt in document.Waypoints)
            {
                // create local variables to ensure closure scope
                var w = wpt;
                var g = wpt.Geocache;
                if (this._processConfigCache.TranslateHints)
                {
                    if (!string.IsNullOrWhiteSpace(g.Hints))
                        yield return new TranslateData(g.Hints, value => g.Hints = value);
                }
                if (this._processConfigCache.TranslateDescriptions)
                {
                    if (!string.IsNullOrWhiteSpace(w.Comment))
                        yield return new TranslateData(w.Comment, value => w.Comment = value);
                    if (!string.IsNullOrWhiteSpace(g.ShortDescription.Text))
                        yield return new TranslateData(g.ShortDescription.Text, g.ShortDescription.IsHtml, value => g.ShortDescription.Text = value);
                    if (!string.IsNullOrWhiteSpace(g.LongDescription.Text))
                        yield return new TranslateData(g.LongDescription.Text, g.LongDescription.IsHtml, value => g.LongDescription.Text = value);
                }
                if (this._processConfigCache.TranslateLogs)
                {
                    foreach (var log in g.Logs)
                    {
                        var l = log;
                        yield return new TranslateData(l.Text.Text, value => l.Text.Text = value);
                    }
                }
            }
        }

        /// <summary>
        /// Processes the specified GPX documents. If the method is not overriden in the derived class,
        /// calls <see cref="TransformerBase.Process(Gpx.GpxDocument, Transformers.TransformerOptions)"/> for each document in the list.
        /// </summary>
        /// <param name="documents">A list of GPX documents. The list may be modified as a result of the execution.</param>
        /// <param name="options">The options that instruct how the transformer should proceed.</param>
        public override void Process(IList<Gpx.GpxDocument> documents, TransformerOptions options)
        {
            if ((options & TransformerOptions.LoadingViewerCache) == TransformerOptions.LoadingViewerCache)
            {
                this.ExecutionContext.ReportStatus("Translation is only done when publishing.");
                return;
            }

            this._errorLogFileName = null; // this will force creation of a new file.
            this._processConfigCache = this.Configuration.Data;

            List<TranslateData> data = new List<TranslateData>();
            foreach (var gpx in documents)
                data.AddRange(this.RetrieveWork(gpx));

            int ignored = 0;

            using (var schema = new TranslatorCacheSchema(System.IO.Path.Combine(this.LocalStoragePath, "Translator.data")))
            {
                using (var service = this.CreateService())
                    this.DetectLanguage(data, service, schema);

                for (int i = data.Count - 1; i >= 0; i--)
                {
                    var d = data[i];
                    if (this._processConfigCache.IgnoreLanguages.Contains(d.SourceLanguage) || this._processConfigCache.TargetLanguage == d.SourceLanguage)
                    {
                        ignored++;
                        data.RemoveAt(i);
                    }
                }

                // recreating the service just in case the security token expired
                using (var service = this.CreateService())
                    this.Translate(data, service, schema);
            }

            for (int i = 0; i < data.Count; i++)
            {
                this.ExecutionContext.ThrowIfCancellationPending();
                if (i % 10 == 0)
                    this.ExecutionContext.ReportStatus(((double)i / data.Count).ToString("P2") + " of values updated");

                var d = data[i];
                if (!string.IsNullOrWhiteSpace(d.Translation) && !string.Equals(d.Translation, d.Text, StringComparison.OrdinalIgnoreCase))
                    d.UpdateValue(d.Translation + (d.IsHtml ? "<hr/>" : Environment.NewLine + "-------------" + Environment.NewLine) + d.Text);
            }

            var errors = data.Count(o => o.SourceLanguageError || o.TranslationError);
            this.ExecutionContext.ReportStatus("Translation complete (" + (data.Count - errors) + " values translated, " + ignored + " skipped, " + errors + " errors)");

            this._processConfigCache = null;
        }

        #region [ IConfigurable ]

        private ConfigurationControl Configuration;

        /// <summary>
        /// Initializes the extension with the specified current configuration (can be <c>null</c> if the extension is initialized for the very first time) and
        /// returns the configuration UI control (can return <c>null</c> if the user interface is not needed).
        /// </summary>
        /// <param name="currentConfiguration">The current configuration.</param>
        /// <returns>
        /// The configuration UI control.
        /// </returns>
        public System.Windows.Forms.Control Initialize(byte[] currentConfiguration)
        {
            this.Configuration = new ConfigurationControl();
            this.Configuration.Data = ConfigurationData.Deserialize(currentConfiguration);
            return this.Configuration;
        }

        /// <summary>
        /// Retrieves the configuration from the extension's configuration UI control.
        /// </summary>
        /// <returns>
        /// The serialized configuration data.
        /// </returns>
        public byte[] SerializeConfiguration()
        {
            return this.Configuration.Data.Serialize();
        }

        /// <summary>
        /// Gets a value indicating whether the this extension should be executed.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this extension is enabled; otherwise, <c>false</c>.
        /// </value>
        public bool IsEnabled
        {
            get { return this.Configuration.Data.IsEnabled; }
        }

        /// <summary>
        /// Gets the category of the extension.
        /// </summary>
        Category IHasCategory.Category { get { return Category.Transformers; } }

        #endregion

        /// <summary>
        /// Gets or sets the local storage path where the extension can store its cache if needed. The value is set by the main engine once the extension instance is created.
        /// </summary>
        public string LocalStoragePath { get; set; }
    }
}
