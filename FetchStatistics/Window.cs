﻿/*
 * This file is part of GeoTransformer project (http://geotransformer.codeplex.com/).
 * It is licensed under Microsoft Reciprocal License (Ms-RL).
 */
 
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace FetchStatistics
{
    public partial class Window : UserControl
    {
        private Queue<Guid> _toDo = new Queue<Guid>();
        private Dictionary<Guid, StatisticsData> _data = new Dictionary<Guid, StatisticsData>();
        private Random _random = new Random();
        private int _counter = 0;

        public Window()
        {
            InitializeComponent();

            this.webBrowser1.Navigate("https://www.geocaching.com/");
            this.webBrowser1.Navigated += WebBrowser1_Navigated;
        }

        void WebBrowser1_Navigated(object sender, WebBrowserNavigatedEventArgs e)
        {
            var url = e.Url.ToString(); //this.webBrowser1.Url.ToString();
            if ((!url.StartsWith("https://www.geocaching.com/profile/")
                || url.StartsWith("https://www.geocaching.com/profile/profilecontent.html")))
            {
                System.Diagnostics.Debugger.Log(0, "StatisticsParser", "Ignoring URL: " + url + Environment.NewLine);
                return;
            }

            System.Diagnostics.Debugger.Log(0, "StatisticsParser", "Loaded URL: " + url + Environment.NewLine);

            this.QueueProcessDocument();
        }

        private void QueueProcessDocument(bool delay = false)
        {
            System.Threading.ThreadPool.QueueUserWorkItem(state => {
                if (delay)
                    System.Threading.Thread.Sleep(_random.Next(500, 2500));

                this.Invoke((ProcessDocumentDelegate)this.ProcessDocument);
            });
        }

        private delegate void ProcessDocumentDelegate();
        private void ProcessDocument()
        {
            var url = this.webBrowser1.Url.ToString();

            while (this.webBrowser1.IsBusy || this.webBrowser1.Document == null || this.webBrowser1.Document.GetElementById("fancybox-loading") == null)
            {
                this.QueueProcessDocument(true);
                return;
            }

            try
            {
                var doc = this.webBrowser1.Document;

                var id = Guid.Parse(url.Substring(url.IndexOf("?guid=") + 6));
                if (!this._data.ContainsKey(id))
                    return;

                var obj = this._data[id];

                if (!doc.GetElementsByClassName("a", "SignedInProfileLink").Any())
                {
                    MessageBox.Show("Nepieciešams ielogoties pirms statistikas lejupielādes sākšanas!", "LV statistika", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.EnableButtons(true);
                    return;
                }

                // if needed, switch to English
                var currentLanguage = doc.GetElementsByClassName("div", "LocaleList").FirstOrDefault();
                if (currentLanguage == null || !currentLanguage.InnerText.Trim().StartsWith("English", StringComparison.Ordinal))
                {
                    doc.GetElementById("ctl00_uxLocaleList_uxLocaleList_ctl00_uxLocaleItem").InvokeMember("click");
                    return;
                }

                var links = doc.GetElementById("ProfileTabs").GetElementsByTagName("a").Cast<HtmlElement>();

                if (doc.GetElementById("ctl00_ContentBody_ProfilePanel1_pnlProfile") != null)
                {
                    Parser.ParseMainPage(doc, obj);
                    links.First(o => o.Id.EndsWith("lnkUserStats")).InvokeMember("click");
                }
                else if (doc.GetElementById("ctl00_ContentBody_ProfilePanel1_pnlStats") != null)
                {
                    Parser.ParseStatPage(doc, obj);
                    links.First(o => o.Id.EndsWith("lnkStatistics")).InvokeMember("click");
                }
                else if (doc.GetElementById("ctl00_ContentBody_ProfilePanel1_pnlStatistics") != null)
                {
                    Parser.ParseStatistics(doc, obj);

                    this.SubmitWork(obj);

                    this.StartNext();
                }
                else
                {
                    throw new Exception("Could not identify the currently open profile page.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Jūsu izmantotā versija ir novecojusi un nespēj vairs apstrādāt geocaching.com lapu vai arī geocaching.com lapa šobrīd nav pieejama." + Environment.NewLine + Environment.NewLine + ex.ToString(), "Kļūda", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }

        private void btnRun_Click(object sender, EventArgs e)
        {
            StartBulk();
        }

        private void StartBulk(bool continuing = false)
        {
            this.EnableButtons(false);
            this._toDo.Clear();
            this._data.Clear();

            if (!continuing)
            {
                this._counter = 0;
            }
            else
            {
                if (++this._counter > 4)
                {
                    MessageBox.Show("Labs darbs padarīts!", "LV statistika", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.EnableButtons(true);
                    return;
                }
            }

            using (var service = ServiceProxy.CreateServiceClient())
            {
                foreach (var id in service.RetrieveWork())
                    this._toDo.Enqueue(id);
            }

            if (this._toDo.Count == 0)
            {
                if (continuing)
                    MessageBox.Show("Labs darbs padarīts!", "LV statistika", MessageBoxButtons.OK, MessageBoxIcon.Information);
                else
                    MessageBox.Show("Šobrīd nav nepieciešama statistikas lejupielāde, bet paldies par piedāvājumu!", "LV statistika", MessageBoxButtons.OK, MessageBoxIcon.Information);

                this.EnableButtons(true);
                return;
            }

            this.StartNext();
        }

        private void SubmitWork(StatisticsData data)
        {
            using (var service = ServiceProxy.CreateServiceClient())
            {
                service.SubmitWork(data);
            }
        }

        private void StartNext()
        {
            if (this._toDo.Count == 0)
            {
                this.StartBulk(true);
                return;
            }

            var id = this._toDo.Dequeue();
            this._data[id] = new StatisticsData() { Version = 2, UserId = id };

            this.webBrowser1.Navigate("https://www.geocaching.com/profile/?guid=" + id.ToString("D").ToLowerInvariant());
        }

        private void EnableButtons(bool enabled)
        {
            if (enabled)
            {
                this._toDo.Clear();
                this._data.Clear();
            }

            this.btnRun.Enabled = enabled;
            this.btnRunSpecifics.Enabled = enabled;
            this.textBoxIDs.Enabled = enabled;
        }

        private void btnRunSpecifics_Click(object sender, EventArgs e)
        {
            this.EnableButtons(false);
            this._toDo.Clear();
            this._data.Clear();
            try
            {
                foreach (var id in this.textBoxIDs.Text.Split(',', ';'))
                {
                    if (string.IsNullOrWhiteSpace(id))
                        continue;

                    this._toDo.Enqueue(Guid.Parse(id));
                }

                if (this._toDo.Count == 0)
                {
                    this.EnableButtons(true);
                    MessageBox.Show("Ievadiet slēpņotāju ID kodus teksta laukā. Vairākus kodus var atdalīt ar komatiem.", "LV statistika", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
            }
            catch
            {
                MessageBox.Show("Nav iespējams atpazīt ievadītos ID kodus.", "LV statistika", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.EnableButtons(true);
                return;
            }

            this.StartNext();
        }
    }
}
