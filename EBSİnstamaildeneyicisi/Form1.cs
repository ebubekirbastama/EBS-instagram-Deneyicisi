using EbubekirBastamatxtokuma;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;

namespace EBSİnstamaildeneyicisi
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            CheckForIllegalCrossThreadCalls = false;
        }
        private static string GetMesaj = "";
        private static string durum = "";
        private void button1_Click(object sender, EventArgs e)
        {
            EBSClass.op = new OpenFileDialog();
            if (EBSClass.op.ShowDialog() == DialogResult.OK)
            {

                EBSClass.th = new Thread(Basla);
                EBSClass.th.Start();
            }
        }

        async void Basla()
        {
            try
            {

                int beklemesüresi = int.Parse(textBox1.Text);
                BekraTxtOkuma.Txtİmport(EBSClass.op.FileName, listBox1, false);
                listBox4.Items.Add(listBox1.Items.Count.ToString());
                for (int i = 0; i < listBox1.Items.Count; i++)
                {

                    #region instarequest


                    var handler = new HttpClientHandler();
                git:
                    handler = new HttpClientHandler
                    {
                       
                        UseProxy = true,
                        PreAuthenticate = true,
                        UseDefaultCredentials = false,
                        AutomaticDecompression = ~DecompressionMethods.None
                    };

                    using (var httpClient = new HttpClient(handler))
                    {
                        using (var request = new HttpRequestMessage(new HttpMethod("POST"), "https://www.instagram.com/accounts/account_recovery_send_ajax/"))
                        {
                            request.Headers.TryAddWithoutValidation("authority", "www.instagram.com");
                            request.Headers.TryAddWithoutValidation("pragma", "no-cache");
                            request.Headers.TryAddWithoutValidation("cache-control", "no-cache");
                            request.Headers.TryAddWithoutValidation("sec-ch-ua", "\" Not A;Brand\";v=\"99\", \"Chromium\";v=\"96\", \"Google Chrome\";v=\"96\"");
                            request.Headers.TryAddWithoutValidation("x-ig-app-id", "936619743392459");
                            request.Headers.TryAddWithoutValidation("x-ig-www-claim", "");
                            request.Headers.TryAddWithoutValidation("sec-ch-ua-mobile", "?0");
                            request.Headers.TryAddWithoutValidation("x-instagram-ajax", "");
                            request.Headers.TryAddWithoutValidation("accept", "*/*");
                            request.Headers.TryAddWithoutValidation("x-requested-with", "");
                            request.Headers.TryAddWithoutValidation("x-asbd-id", "198387");
                            request.Headers.TryAddWithoutValidation("user-agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/96.0.4664.110 Safari/537.36");
                            request.Headers.TryAddWithoutValidation("x-csrftoken", "");
                            request.Headers.TryAddWithoutValidation("sec-ch-ua-platform", "\"Windows\"");
                            request.Headers.TryAddWithoutValidation("origin", "https://www.instagram.com");
                            request.Headers.TryAddWithoutValidation("sec-fetch-site", "same-origin");
                            request.Headers.TryAddWithoutValidation("sec-fetch-mode", "cors");
                            request.Headers.TryAddWithoutValidation("sec-fetch-dest", "empty");
                            request.Headers.TryAddWithoutValidation("referer", "https://www.instagram.com/accounts/password/reset/");
                            request.Headers.TryAddWithoutValidation("accept-language", "tr-TR,tr;q=0.9,en-US;q=0.8,en;q=0.7");
                            request.Headers.TryAddWithoutValidation("cookie", "csrftoken=; mid=; ig_did=----; ig_nrcb=1");

                            request.Content = new StringContent("email_or_username=" + listBox1.Items[i].ToString() + "&recaptcha_challenge_field=&flow=&app_id=&source_account_id=");
                            request.Content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/x-www-form-urlencoded");

                            var response = await httpClient.SendAsync(request);
                            EBSParseJSON(response.Content.ReadAsStringAsync().Result.ToString());
                        }
                    }
                    #endregion


                    if (GetMesaj.IndexOf("bulunamadı") != -1)
                    {
                        listBox2.Items.Add(listBox1.Items[i].ToString() + " : " + GetMesaj);
                        int count = listBox2.Items.Count + listBox3.Items.Count;
                        this.Text = "Toplam Çekilen Mail :" + listBox1.Items.Count.ToString() + "/" + count;
                    }
                    else if (GetMesaj.IndexOf("bekle") != -1)
                    {
                        listBox4.Items.Add(textBox1.Text.ToString() + ": Sn. Bekleniyor... Mailadresi: " + listBox1.Items[i].ToString());
                        int count = listBox2.Items.Count + listBox3.Items.Count;
                        this.Text = "Toplam Çekilen Mail :" + listBox1.Items.Count.ToString() + "/" + count;
                        Thread.Sleep(beklemesüresi);

                        goto git;
                    }
                    else if (GetMesaj.IndexOf("feedback_required") != -1)
                    {
                        listBox4.Items.Add(textBox1.Text.ToString() + ": Sn. Bekleniyor... Mailadresi: " + listBox1.Items[i].ToString());
                        int count = listBox2.Items.Count + listBox3.Items.Count;
                        this.Text = "Toplam Çekilen Mail :" + listBox1.Items.Count.ToString() + "/" + count;
                        Thread.Sleep(beklemesüresi);

                        goto git;
                    }
                    else
                    {
                        int count = listBox2.Items.Count + listBox3.Items.Count;
                        this.Text = "Toplam Çekilen Mail :" + listBox1.Items.Count.ToString() + "/" + count;
                        listBox3.Items.Add(listBox1.Items[i].ToString() + " : " + GetMesaj);
                    }

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "EBS Time");
            }
        }

        public static void EBSParseJSON(string s)
        {
            ArrayList ary = new ArrayList();
            Regex r = new Regex("\"(?<Key>[\\w]*)\":\"?(?<Value>([\\s\\w\\d\\.\\\\\\-/:_\\+]+(,[,\\s\\w\\d\\.\\\\\\-/:_\\+]*)?)*)\"?");

            MatchCollection mc = r.Matches(s);

            Dictionary<string, string> json = new Dictionary<string, string>();


            foreach (Match k in mc)
            {

                if (k.Groups["Key"].Value == "message")
                {
                    GetMesaj = k.Groups["Value"].Value;

                }
                if (k.Groups["Key"].Value == "status")
                {
                    durum = k.Groups["Value"].Value;
                    break;
                }

            }

        }

    }
}