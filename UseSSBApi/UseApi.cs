using Newtonsoft.Json;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices.ComTypes;
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
                client.BaseAddress = new Uri(txtBaseUrl.Text+_prefixUrl);
                if (!string.IsNullOrEmpty(txtToken.Text))
                    client.DefaultRequestHeaders.Add("SSBToken", txtToken.Text);
                var content = new StringContent(JsonConvert.SerializeObject(body), Encoding.UTF8, "application/json");
                var resClient = client.PostAsync(url,content).Result;
                resClient.EnsureSuccessStatusCode();
                response = JsonConvert.DeserializeObject<T>(resClient.Content.ReadAsStringAsync().Result);
            }
            return response;
        }
        private void btnLogin_Click(object sender, EventArgs e)
        {
            var body = new { Username = txtUsername.Text, Password = txtPassword.Text };
            var response = GetClient<LoginModel>(body, "SendSMS/Login");
            if (response.Code != "0")
            {
                btnFailed.Visible = true;
                txtResult.Text = $"Api error is :{(string.IsNullOrEmpty(response.Message) ? response.Code : response.Message)}";
            }
            else
            {
                txtToken.Text = response.SSBToken;
                grpMessage.Enabled = true;
                tabSMSApi.Enabled = true;
            }
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void button14_Click(object sender, EventArgs e)
        {

        }
        private void Success() {
            txtResult.Visible = true;
            btnFailed.Visible = false;
            btnSuccess.Visible = true;
        }
        private void Failed()
        {
            txtResult.Visible = true;
            btnFailed.Visible = true;
            btnSuccess.Visible = false;
        }
        private void SendSMSResult(SMSModel model) {
            if (model.Code != "0")
            {
                Failed();
                txtResult.Text = $"Api error is :{(string.IsNullOrEmpty(model.Message) ? model.Code : model.Message)}";
            }
            else
            {
                Success();
                foreach (var item in model.Result)
                    txtResult.Text += (item + Environment.NewLine);
            }
        }

        private void SendSMSResult2(SMSModel2 model)
        {
            if (model.Code != "0")
            {
                Failed();
                txtResult.Text = $"Api error is :{(string.IsNullOrEmpty(model.Message) ? model.Code : model.Message)}";
            }
            else
            {
                Success();
                foreach (var item in model.Result)
                    txtResult.Text += (item.ToString() + Environment.NewLine);
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            var body = new { Message = txtMessage.Text, FromNumber = txtLineNumber.Text, ToNumber=txtMobile.Text };
            var response = GetClient<SMSModel>(body, "SendSMS/SendFromUrl");
            SendSMSResult(response);
        }

        private void UseApi_Load(object sender, EventArgs e)
        {
           
        }
        
        private void button2_Click(object sender, EventArgs e)
        {
            var message = new string[] { txtMessage.Text };
            var toNumber = new string[] { txtMobile.Text };
            var fromNumber = new string[] { txtLineNumber.Text };
            var body = new { Messages = message, SenderNumbers = fromNumber, Mobiles = toNumber };
            var response = GetClient<SMSModel>(body, "SendSMS/ArraySendQeue");
            SendSMSResult(response);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            var message = new string[] { txtMessage.Text };
            var toNumber = new string[] { txtMobile.Text };
            var fromNumber = new string[] { txtLineNumber.Text };
            var ids = new Guid[] { Guid.NewGuid() };
            var body = new { Messages = message, SenderNumbers = fromNumber, Mobiles = toNumber, Ids=ids };
            var response = GetClient<SMSModel>(body, "SendSMS/ArraySendQeueWithId");
            SendSMSResult(response);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            var body = new { Message = txtMessage.Text, FromNumber = txtLineNumber.Text, ToNumber = new string[] { txtMobile.Text } };
            var response = GetClient<SMSModel>(body, "SendSMS/SendQeue");
            SendSMSResult(response);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            var body = new { Message = txtMessage.Text, FromNumber = txtLineNumber.Text, ToNumber = new string[] { txtMobile.Text } };
            var response = GetClient<SMSModel2>(body, "SendSMS/Send");
            SendSMSResult2(response);
        }

        private void button16_Click(object sender, EventArgs e)
        {
            var body = new { Message =new string[] { txtMessage.Text }, FromNumber = txtLineNumber.Text, ToNumber = new string[] { txtMobile.Text } };
            var response = GetClient<SMSModel2>(body, "SendSMS/ArraySend");
            SendSMSResult2(response);
        }
        private void RecieveSMSResult(RecieveModel model) {
            if (model.Code != "0")
            {
                Failed();
                txtResult.Text = $"Api error is :{(string.IsNullOrEmpty(model.Message) ? model.Code : model.Message)}";
            }
            else
            {
                Success();
                foreach (var item in model.Result)
                    txtResult.Text += $"SmsTex :{item.RcvSmsText} SmsFrom :{item.RcvSmsfrom} SmsTo :{item.RcvSmsTo}";
            }
        }
        private void button10_Click(object sender, EventArgs e)
        {
            var body = new { PhNo =  txtLineNumber.Text, StartDate="1404/01/01", EndDate= "1404/12/29" };
            var response = GetClient<RecieveModel>(body, "Recieve/RecieveSMS");
            RecieveSMSResult(response);
        }

        private void button9_Click(object sender, EventArgs e)
        {
            var body = new { PhNo = txtLineNumber.Text, Id = 10 };
            var response = GetClient<RecieveModel>(body, "Recieve/RecieveSMSById");
            RecieveSMSResult(response);
        }

        private void button8_Click(object sender, EventArgs e)
        {
            var body = new { ToNumber = txtMobile.Text };
            var response = GetClient<RecieveModel>(body, "Recieve/UnreadMessgese");
            RecieveSMSResult(response);
        }

        private void button7_Click(object sender, EventArgs e)
        {
            var body = new { Username = txtUsername.Text };
            var response = GetClient<RecieveModel>(body, "Recieve/UnreadMessgeseWithUsername");
            RecieveSMSResult(response);
        }
    }
}
