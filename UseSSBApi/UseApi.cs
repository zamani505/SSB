using Newtonsoft.Json;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using UseSSBApi.Models;

namespace UseSSBApi
{
    public partial class UseApi : Form
    {
        public readonly string _prefixUrl = "v1/api/";
        public UseApi()
        {
            InitializeComponent();
        }
       
        private T GetClient<T>(object body, string url)where T:class
        {
            T response;
            using (var client = new HttpClient())
            {
                var content = new StringContent(JsonConvert.SerializeObject(body), Encoding.UTF8, "application/json");
                var resClient = client.PostAsync(url,content).Result;
                resClient.EnsureSuccessStatusCode();
                response = JsonConvert.DeserializeObject<T>(resClient.Content.ReadAsStringAsync().Result);
            }
            return response;
        }
        private void btnLogin_Click(object sender, EventArgs e)
        {
            var body = new { Username = txtUsername, Password = txtPassword.Text };
            var response = GetClient<LoginModel>(body, "SendSMS/Login");
            if (response.Code != "0")
                MessageBox.Show($"Api error is :{response.Message}");
            else
                txtToken.Text = response.SSBToken;
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void button14_Click(object sender, EventArgs e)
        {

        }
    }
}
