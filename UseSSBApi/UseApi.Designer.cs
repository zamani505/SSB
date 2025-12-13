namespace UseSSBApi
{
    partial class UseApi
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.txtBaseUrl = new System.Windows.Forms.TextBox();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.btnLogin = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.txtToken = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.txtPassword = new System.Windows.Forms.TextBox();
            this.txtUsername = new System.Windows.Forms.TextBox();
            this.grpMessage = new System.Windows.Forms.GroupBox();
            this.label5 = new System.Windows.Forms.Label();
            this.txtMessage = new System.Windows.Forms.TextBox();
            this.lblMobile = new System.Windows.Forms.Label();
            this.txtMobile = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.txtLineNumber = new System.Windows.Forms.TextBox();
            this.tabSMSApi = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.button16 = new System.Windows.Forms.Button();
            this.button5 = new System.Windows.Forms.Button();
            this.button4 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.button7 = new System.Windows.Forms.Button();
            this.button8 = new System.Windows.Forms.Button();
            this.button9 = new System.Windows.Forms.Button();
            this.button10 = new System.Windows.Forms.Button();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.button14 = new System.Windows.Forms.Button();
            this.button15 = new System.Windows.Forms.Button();
            this.tabPage4 = new System.Windows.Forms.TabPage();
            this.button6 = new System.Windows.Forms.Button();
            this.button11 = new System.Windows.Forms.Button();
            this.btnSuccess = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.btnFailed = new System.Windows.Forms.Button();
            this.txtResult = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.grpMessage.SuspendLayout();
            this.tabSMSApi.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.tabPage3.SuspendLayout();
            this.tabPage4.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.BackColor = System.Drawing.Color.Transparent;
            this.groupBox1.Controls.Add(this.txtBaseUrl);
            this.groupBox1.Location = new System.Drawing.Point(5, 3);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.groupBox1.Size = new System.Drawing.Size(679, 59);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "BaseUrl";
            // 
            // txtBaseUrl
            // 
            this.txtBaseUrl.Location = new System.Drawing.Point(75, 19);
            this.txtBaseUrl.Name = "txtBaseUrl";
            this.txtBaseUrl.Size = new System.Drawing.Size(308, 21);
            this.txtBaseUrl.TabIndex = 0;
            this.txtBaseUrl.Text = "http://localhost:57476/";
            // 
            // groupBox5
            // 
            this.groupBox5.BackColor = System.Drawing.Color.Transparent;
            this.groupBox5.Controls.Add(this.btnLogin);
            this.groupBox5.Controls.Add(this.label3);
            this.groupBox5.Controls.Add(this.txtToken);
            this.groupBox5.Controls.Add(this.label2);
            this.groupBox5.Controls.Add(this.label1);
            this.groupBox5.Controls.Add(this.txtPassword);
            this.groupBox5.Controls.Add(this.txtUsername);
            this.groupBox5.Location = new System.Drawing.Point(6, 67);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.groupBox5.Size = new System.Drawing.Size(678, 79);
            this.groupBox5.TabIndex = 4;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "Login SMS API";
            // 
            // btnLogin
            // 
            this.btnLogin.Image = global::UseSSBApi.Properties.Resources.login_Small;
            this.btnLogin.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnLogin.Location = new System.Drawing.Point(408, 46);
            this.btnLogin.Name = "btnLogin";
            this.btnLogin.Size = new System.Drawing.Size(129, 27);
            this.btnLogin.TabIndex = 6;
            this.btnLogin.Text = "Login";
            this.btnLogin.UseVisualStyleBackColor = true;
            this.btnLogin.Click += new System.EventHandler(this.btnLogin_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(313, 23);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(43, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "Token :";
            // 
            // txtToken
            // 
            this.txtToken.Enabled = false;
            this.txtToken.Location = new System.Drawing.Point(354, 20);
            this.txtToken.Name = "txtToken";
            this.txtToken.Size = new System.Drawing.Size(249, 21);
            this.txtToken.TabIndex = 4;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(15, 48);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(60, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Password :";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 23);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(62, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Username :";
            // 
            // txtPassword
            // 
            this.txtPassword.Location = new System.Drawing.Point(74, 45);
            this.txtPassword.Name = "txtPassword";
            this.txtPassword.Size = new System.Drawing.Size(146, 21);
            this.txtPassword.TabIndex = 1;
            this.txtPassword.Text = "zamani123";
            // 
            // txtUsername
            // 
            this.txtUsername.Location = new System.Drawing.Point(74, 19);
            this.txtUsername.Name = "txtUsername";
            this.txtUsername.Size = new System.Drawing.Size(146, 21);
            this.txtUsername.TabIndex = 0;
            this.txtUsername.Text = "zamani";
            // 
            // grpMessage
            // 
            this.grpMessage.BackColor = System.Drawing.Color.Transparent;
            this.grpMessage.Controls.Add(this.label5);
            this.grpMessage.Controls.Add(this.txtMessage);
            this.grpMessage.Controls.Add(this.lblMobile);
            this.grpMessage.Controls.Add(this.txtMobile);
            this.grpMessage.Controls.Add(this.label4);
            this.grpMessage.Controls.Add(this.txtLineNumber);
            this.grpMessage.Enabled = false;
            this.grpMessage.Location = new System.Drawing.Point(6, 151);
            this.grpMessage.Name = "grpMessage";
            this.grpMessage.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.grpMessage.Size = new System.Drawing.Size(678, 98);
            this.grpMessage.TabIndex = 1;
            this.grpMessage.TabStop = false;
            this.grpMessage.Text = "Message";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(314, 43);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(36, 13);
            this.label5.TabIndex = 11;
            this.label5.Text = "Text :";
            // 
            // txtMessage
            // 
            this.txtMessage.Location = new System.Drawing.Point(354, 29);
            this.txtMessage.Multiline = true;
            this.txtMessage.Name = "txtMessage";
            this.txtMessage.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtMessage.Size = new System.Drawing.Size(249, 45);
            this.txtMessage.TabIndex = 10;
            this.txtMessage.Text = "Test";
            this.txtMessage.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtMessage.TextChanged += new System.EventHandler(this.textBox2_TextChanged);
            // 
            // lblMobile
            // 
            this.lblMobile.AutoSize = true;
            this.lblMobile.Location = new System.Drawing.Point(30, 57);
            this.lblMobile.Name = "lblMobile";
            this.lblMobile.Size = new System.Drawing.Size(44, 13);
            this.lblMobile.TabIndex = 9;
            this.lblMobile.Text = "Mobile :";
            // 
            // txtMobile
            // 
            this.txtMobile.Location = new System.Drawing.Point(74, 54);
            this.txtMobile.Name = "txtMobile";
            this.txtMobile.Size = new System.Drawing.Size(146, 21);
            this.txtMobile.TabIndex = 8;
            this.txtMobile.Text = "09127698738";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(4, 29);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(70, 13);
            this.label4.TabIndex = 7;
            this.label4.Text = "LineNumber :";
            // 
            // txtLineNumber
            // 
            this.txtLineNumber.Location = new System.Drawing.Point(74, 26);
            this.txtLineNumber.Name = "txtLineNumber";
            this.txtLineNumber.Size = new System.Drawing.Size(146, 21);
            this.txtLineNumber.TabIndex = 0;
            this.txtLineNumber.Text = "+983000";
            // 
            // tabSMSApi
            // 
            this.tabSMSApi.Controls.Add(this.tabPage1);
            this.tabSMSApi.Controls.Add(this.tabPage2);
            this.tabSMSApi.Controls.Add(this.tabPage3);
            this.tabSMSApi.Controls.Add(this.tabPage4);
            this.tabSMSApi.Enabled = false;
            this.tabSMSApi.Location = new System.Drawing.Point(6, 256);
            this.tabSMSApi.Name = "tabSMSApi";
            this.tabSMSApi.SelectedIndex = 0;
            this.tabSMSApi.Size = new System.Drawing.Size(682, 130);
            this.tabSMSApi.TabIndex = 6;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.button16);
            this.tabPage1.Controls.Add(this.button5);
            this.tabPage1.Controls.Add(this.button4);
            this.tabPage1.Controls.Add(this.button3);
            this.tabPage1.Controls.Add(this.button2);
            this.tabPage1.Controls.Add(this.button1);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(674, 104);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "SenSMSApi";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // button16
            // 
            this.button16.Image = global::UseSSBApi.Properties.Resources.Array_Send;
            this.button16.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.button16.Location = new System.Drawing.Point(574, 15);
            this.button16.Name = "button16";
            this.button16.Size = new System.Drawing.Size(91, 72);
            this.button16.TabIndex = 5;
            this.button16.Text = "ArraySend";
            this.button16.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.button16.UseVisualStyleBackColor = true;
            this.button16.Click += new System.EventHandler(this.button16_Click);
            // 
            // button5
            // 
            this.button5.Image = global::UseSSBApi.Properties.Resources.Send;
            this.button5.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.button5.Location = new System.Drawing.Point(502, 15);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(69, 72);
            this.button5.TabIndex = 4;
            this.button5.Text = "Send";
            this.button5.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.button5.UseVisualStyleBackColor = true;
            this.button5.Click += new System.EventHandler(this.button5_Click);
            // 
            // button4
            // 
            this.button4.Image = global::UseSSBApi.Properties.Resources.Send_Queue;
            this.button4.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.button4.Location = new System.Drawing.Point(407, 15);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(94, 72);
            this.button4.TabIndex = 3;
            this.button4.Text = "SendQeue";
            this.button4.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // button3
            // 
            this.button3.Image = global::UseSSBApi.Properties.Resources.Send_Id;
            this.button3.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.button3.Location = new System.Drawing.Point(246, 15);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(160, 72);
            this.button3.TabIndex = 2;
            this.button3.Text = "ArraySendQeueWithId";
            this.button3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // button2
            // 
            this.button2.Image = global::UseSSBApi.Properties.Resources.Array_Send_Queue;
            this.button2.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.button2.Location = new System.Drawing.Point(115, 15);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(130, 72);
            this.button2.TabIndex = 1;
            this.button2.Text = "ArraySendQeue";
            this.button2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // button1
            // 
            this.button1.Image = global::UseSSBApi.Properties.Resources.Send_Url;
            this.button1.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.button1.Location = new System.Drawing.Point(8, 15);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(103, 72);
            this.button1.TabIndex = 0;
            this.button1.Text = "SendFromUrl ";
            this.button1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.button7);
            this.tabPage2.Controls.Add(this.button8);
            this.tabPage2.Controls.Add(this.button9);
            this.tabPage2.Controls.Add(this.button10);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(674, 104);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "RecieveSMSApi";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // button7
            // 
            this.button7.Image = global::UseSSBApi.Properties.Resources.UnreadMessage_Username;
            this.button7.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.button7.Location = new System.Drawing.Point(414, 16);
            this.button7.Name = "button7";
            this.button7.Size = new System.Drawing.Size(216, 72);
            this.button7.TabIndex = 8;
            this.button7.Text = "GetUnreadMessgeseWithUsername";
            this.button7.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.button7.UseVisualStyleBackColor = true;
            this.button7.Click += new System.EventHandler(this.button7_Click);
            // 
            // button8
            // 
            this.button8.Image = global::UseSSBApi.Properties.Resources.UnreadMessage;
            this.button8.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.button8.Location = new System.Drawing.Point(261, 16);
            this.button8.Name = "button8";
            this.button8.Size = new System.Drawing.Size(147, 72);
            this.button8.TabIndex = 7;
            this.button8.Text = "GetUnreadMessagese";
            this.button8.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.button8.UseVisualStyleBackColor = true;
            this.button8.Click += new System.EventHandler(this.button8_Click);
            // 
            // button9
            // 
            this.button9.Image = global::UseSSBApi.Properties.Resources.Recieve_Id;
            this.button9.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.button9.Location = new System.Drawing.Point(129, 16);
            this.button9.Name = "button9";
            this.button9.Size = new System.Drawing.Size(126, 72);
            this.button9.TabIndex = 6;
            this.button9.Text = "RecieveSMSById";
            this.button9.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.button9.UseVisualStyleBackColor = true;
            this.button9.Click += new System.EventHandler(this.button9_Click);
            // 
            // button10
            // 
            this.button10.Image = global::UseSSBApi.Properties.Resources.Recieve1;
            this.button10.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.button10.Location = new System.Drawing.Point(14, 16);
            this.button10.Name = "button10";
            this.button10.Size = new System.Drawing.Size(109, 72);
            this.button10.TabIndex = 5;
            this.button10.Text = "RecieveSMS";
            this.button10.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.button10.UseVisualStyleBackColor = true;
            this.button10.Click += new System.EventHandler(this.button10_Click);
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.button14);
            this.tabPage3.Controls.Add(this.button15);
            this.tabPage3.Location = new System.Drawing.Point(4, 22);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Size = new System.Drawing.Size(674, 104);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "StatusSMSApi";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // button14
            // 
            this.button14.Image = global::UseSSBApi.Properties.Resources.Status_Queue;
            this.button14.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.button14.Location = new System.Drawing.Point(155, 16);
            this.button14.Name = "button14";
            this.button14.Size = new System.Drawing.Size(169, 72);
            this.button14.TabIndex = 6;
            this.button14.Text = "GetQueueMessageStatus";
            this.button14.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.button14.UseVisualStyleBackColor = true;
            this.button14.Click += new System.EventHandler(this.button14_Click);
            // 
            // button15
            // 
            this.button15.Image = global::UseSSBApi.Properties.Resources.Status;
            this.button15.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.button15.Location = new System.Drawing.Point(14, 16);
            this.button15.Name = "button15";
            this.button15.Size = new System.Drawing.Size(135, 72);
            this.button15.TabIndex = 5;
            this.button15.Text = "GetMessageStatus";
            this.button15.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.button15.UseVisualStyleBackColor = true;
            // 
            // tabPage4
            // 
            this.tabPage4.Controls.Add(this.button6);
            this.tabPage4.Controls.Add(this.button11);
            this.tabPage4.Location = new System.Drawing.Point(4, 22);
            this.tabPage4.Name = "tabPage4";
            this.tabPage4.Size = new System.Drawing.Size(674, 104);
            this.tabPage4.TabIndex = 3;
            this.tabPage4.Text = "CreditSMSApi";
            this.tabPage4.UseVisualStyleBackColor = true;
            // 
            // button6
            // 
            this.button6.FlatAppearance.BorderColor = System.Drawing.Color.DeepSkyBlue;
            this.button6.FlatAppearance.BorderSize = 2;
            this.button6.Image = global::UseSSBApi.Properties.Resources.Chek_Credit;
            this.button6.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.button6.Location = new System.Drawing.Point(132, 16);
            this.button6.Name = "button6";
            this.button6.Size = new System.Drawing.Size(102, 72);
            this.button6.TabIndex = 8;
            this.button6.Text = "CheckCredit";
            this.button6.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.button6.UseVisualStyleBackColor = true;
            // 
            // button11
            // 
            this.button11.FlatAppearance.BorderColor = System.Drawing.Color.DeepSkyBlue;
            this.button11.Image = global::UseSSBApi.Properties.Resources.Cedit;
            this.button11.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.button11.Location = new System.Drawing.Point(12, 16);
            this.button11.Name = "button11";
            this.button11.Size = new System.Drawing.Size(114, 72);
            this.button11.TabIndex = 7;
            this.button11.Text = "GetUserCredit";
            this.button11.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.button11.UseVisualStyleBackColor = true;
            // 
            // btnSuccess
            // 
            this.btnSuccess.FlatAppearance.BorderColor = System.Drawing.Color.Lime;
            this.btnSuccess.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSuccess.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSuccess.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(0)))));
            this.btnSuccess.Image = global::UseSSBApi.Properties.Resources.Success;
            this.btnSuccess.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnSuccess.Location = new System.Drawing.Point(88, -5);
            this.btnSuccess.Name = "btnSuccess";
            this.btnSuccess.Size = new System.Drawing.Size(137, 48);
            this.btnSuccess.TabIndex = 7;
            this.btnSuccess.Text = "SUCCESS";
            this.btnSuccess.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnSuccess.UseVisualStyleBackColor = true;
            this.btnSuccess.Visible = false;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.btnFailed);
            this.groupBox2.Controls.Add(this.txtResult);
            this.groupBox2.Controls.Add(this.btnSuccess);
            this.groupBox2.Location = new System.Drawing.Point(690, 20);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(323, 360);
            this.groupBox2.TabIndex = 8;
            this.groupBox2.TabStop = false;
            // 
            // btnFailed
            // 
            this.btnFailed.FlatAppearance.BorderColor = System.Drawing.Color.Red;
            this.btnFailed.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnFailed.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnFailed.ForeColor = System.Drawing.Color.Red;
            this.btnFailed.Image = global::UseSSBApi.Properties.Resources.Failed;
            this.btnFailed.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnFailed.Location = new System.Drawing.Point(88, 0);
            this.btnFailed.Name = "btnFailed";
            this.btnFailed.Size = new System.Drawing.Size(137, 48);
            this.btnFailed.TabIndex = 9;
            this.btnFailed.Text = "FAILED";
            this.btnFailed.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnFailed.UseVisualStyleBackColor = true;
            this.btnFailed.Visible = false;
            // 
            // txtResult
            // 
            this.txtResult.Location = new System.Drawing.Point(6, 55);
            this.txtResult.Multiline = true;
            this.txtResult.Name = "txtResult";
            this.txtResult.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.txtResult.Size = new System.Drawing.Size(311, 299);
            this.txtResult.TabIndex = 8;
            this.txtResult.Visible = false;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(690, 6);
            this.label6.Name = "label6";
            this.label6.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.label6.Size = new System.Drawing.Size(57, 16);
            this.label6.TabIndex = 12;
            this.label6.Text = "Result :";
            // 
            // UseApi
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(1025, 394);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.tabSMSApi);
            this.Controls.Add(this.grpMessage);
            this.Controls.Add(this.groupBox5);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.groupBox2);
            this.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ForeColor = System.Drawing.SystemColors.ControlText;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "UseApi";
            this.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.Text = "سهند سامانه برتر";
            this.Load += new System.EventHandler(this.UseApi_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            this.grpMessage.ResumeLayout(false);
            this.grpMessage.PerformLayout();
            this.tabSMSApi.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.tabPage3.ResumeLayout(false);
            this.tabPage4.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox txtBaseUrl;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtPassword;
        private System.Windows.Forms.TextBox txtUsername;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtToken;
        private System.Windows.Forms.Button btnLogin;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.GroupBox grpMessage;
        private System.Windows.Forms.TextBox txtLineNumber;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label lblMobile;
        private System.Windows.Forms.TextBox txtMobile;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txtMessage;
        private System.Windows.Forms.TabControl tabSMSApi;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.Button button16;
        private System.Windows.Forms.Button button5;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button7;
        private System.Windows.Forms.Button button8;
        private System.Windows.Forms.Button button9;
        private System.Windows.Forms.Button button10;
        private System.Windows.Forms.Button button14;
        private System.Windows.Forms.Button button15;
        private System.Windows.Forms.TabPage tabPage4;
        private System.Windows.Forms.Button button6;
        private System.Windows.Forms.Button button11;
        private System.Windows.Forms.Button btnSuccess;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.TextBox txtResult;
        private System.Windows.Forms.Button btnFailed;
        private System.Windows.Forms.Label label6;
    }
}