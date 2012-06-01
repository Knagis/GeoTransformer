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

        public Window()
        {
            InitializeComponent();

            this.webBrowser1.Navigate("http://www.geocaching.com/");
            this.webBrowser1.DocumentCompleted += webBrowser1_DocumentCompleted;
        }

        void webBrowser1_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            var url = e.Url.ToString();
            if (!url.Contains("/profile/"))
                return;

            System.Threading.Thread.Sleep(_random.Next(1000, 5000));

            try
            {
                var doc = this.webBrowser1.Document;
                var links = doc.GetElementById("ProfileTabs").GetElementsByTagName("a").Cast<HtmlElement>();

                var id = Guid.Parse(url.Substring(url.IndexOf("?guid=") + 6));
                var obj = this._data[id];

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
            }
            catch (Exception ex)
            {
                MessageBox.Show("Jūsu izmantotā versija ir novecojusi un nespēj vairs apstrādāt geocaching.com lapu vai arī geocaching.com lapa šobrīd nav pieejama." + Environment.NewLine + Environment.NewLine + ex.ToString(), "Kļūda", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }

        private void btnRun_Click(object sender, EventArgs e)
        {
            this.EnableButtons(false);
            this._toDo.Clear();
            this._data.Clear();

            using (var service = ServiceProxy.CreateServiceClient())
            {
                foreach (var id in service.RetrieveWork())
                    this._toDo.Enqueue(id);
            }

            if (this._toDo.Count == 0)
            {
                this.EnableButtons(true);
                MessageBox.Show("Šobrīd nav nepieciešama statistikas lejupielāde, bet paldies par piedāvājumu!", "LV statistika", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
                MessageBox.Show("Labs darbs padarīts!", "LV statistika", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.EnableButtons(true);
                return;
            }

            var id = this._toDo.Dequeue();
            this._data[id] = new StatisticsData() { UserId = id };

            this.webBrowser1.Navigate("http://www.geocaching.com/profile/?guid=" + id.ToString("D").ToLowerInvariant());
        }

        private void EnableButtons(bool enabled)
        {
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