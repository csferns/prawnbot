namespace prawnbotWF
{
    partial class DiscordBot
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DiscordBot));
            this.label1 = new System.Windows.Forms.Label();
            this.token_tb = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.rpdropdown = new System.Windows.Forms.ComboBox();
            this.rptextbox = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.consoleoutput = new System.Windows.Forms.RichTextBox();
            this.connect_btn = new System.Windows.Forms.Button();
            this.rpbutton = new System.Windows.Forms.Button();
            this.disconnectbtn = new System.Windows.Forms.Button();
            this.multirp = new System.Windows.Forms.CheckBox();
            this.rpdropdown2 = new System.Windows.Forms.ComboBox();
            this.rpdropdown3 = new System.Windows.Forms.ComboBox();
            this.rpdropdown4 = new System.Windows.Forms.ComboBox();
            this.defaultrplabel = new System.Windows.Forms.Label();
            this.rp1label = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.rptextbox2 = new System.Windows.Forms.TextBox();
            this.rptextbox3 = new System.Windows.Forms.TextBox();
            this.rptextbox4 = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.streamurl = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(10, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(38, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Token";
            // 
            // token_tb
            // 
            this.token_tb.Location = new System.Drawing.Point(88, 12);
            this.token_tb.Name = "token_tb";
            this.token_tb.Size = new System.Drawing.Size(471, 20);
            this.token_tb.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 373);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(22, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "RP";
            // 
            // rpdropdown
            // 
            this.rpdropdown.FormattingEnabled = true;
            this.rpdropdown.Location = new System.Drawing.Point(88, 415);
            this.rpdropdown.Name = "rpdropdown";
            this.rpdropdown.Size = new System.Drawing.Size(121, 21);
            this.rpdropdown.TabIndex = 3;
            // 
            // rptextbox
            // 
            this.rptextbox.Location = new System.Drawing.Point(213, 416);
            this.rptextbox.Name = "rptextbox";
            this.rptextbox.Size = new System.Drawing.Size(346, 20);
            this.rptextbox.TabIndex = 4;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 41);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(45, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "Console";
            // 
            // consoleoutput
            // 
            this.consoleoutput.Location = new System.Drawing.Point(88, 38);
            this.consoleoutput.Name = "consoleoutput";
            this.consoleoutput.Size = new System.Drawing.Size(471, 262);
            this.consoleoutput.TabIndex = 6;
            this.consoleoutput.Text = "";
            // 
            // connect_btn
            // 
            this.connect_btn.BackColor = System.Drawing.Color.LightGreen;
            this.connect_btn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.connect_btn.ForeColor = System.Drawing.SystemColors.ControlText;
            this.connect_btn.Location = new System.Drawing.Point(88, 306);
            this.connect_btn.Name = "connect_btn";
            this.connect_btn.Size = new System.Drawing.Size(240, 34);
            this.connect_btn.TabIndex = 7;
            this.connect_btn.Text = "Connect";
            this.connect_btn.UseVisualStyleBackColor = false;
            this.connect_btn.Click += new System.EventHandler(this.connect_btn_Click);
            // 
            // rpbutton
            // 
            this.rpbutton.BackColor = System.Drawing.Color.MediumTurquoise;
            this.rpbutton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.rpbutton.Location = new System.Drawing.Point(334, 512);
            this.rpbutton.Name = "rpbutton";
            this.rpbutton.Size = new System.Drawing.Size(225, 34);
            this.rpbutton.TabIndex = 8;
            this.rpbutton.Text = "Update Rich Presence";
            this.rpbutton.UseVisualStyleBackColor = false;
            this.rpbutton.Click += new System.EventHandler(this.rpbutton_Click);
            // 
            // disconnectbtn
            // 
            this.disconnectbtn.BackColor = System.Drawing.Color.OrangeRed;
            this.disconnectbtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.disconnectbtn.ForeColor = System.Drawing.SystemColors.ControlText;
            this.disconnectbtn.Location = new System.Drawing.Point(334, 306);
            this.disconnectbtn.Name = "disconnectbtn";
            this.disconnectbtn.Size = new System.Drawing.Size(225, 34);
            this.disconnectbtn.TabIndex = 9;
            this.disconnectbtn.Text = "Disconnect";
            this.disconnectbtn.UseVisualStyleBackColor = false;
            this.disconnectbtn.Click += new System.EventHandler(this.disconnectbtn_Click);
            // 
            // multirp
            // 
            this.multirp.AutoSize = true;
            this.multirp.Location = new System.Drawing.Point(88, 373);
            this.multirp.Name = "multirp";
            this.multirp.Size = new System.Drawing.Size(182, 17);
            this.multirp.TabIndex = 10;
            this.multirp.Text = "Multiple Rich presence statuses?";
            this.multirp.UseVisualStyleBackColor = true;
            this.multirp.CheckedChanged += new System.EventHandler(this.multirp_CheckedChanged);
            // 
            // rpdropdown2
            // 
            this.rpdropdown2.FormattingEnabled = true;
            this.rpdropdown2.Location = new System.Drawing.Point(88, 437);
            this.rpdropdown2.Name = "rpdropdown2";
            this.rpdropdown2.Size = new System.Drawing.Size(121, 21);
            this.rpdropdown2.TabIndex = 11;
            // 
            // rpdropdown3
            // 
            this.rpdropdown3.FormattingEnabled = true;
            this.rpdropdown3.Location = new System.Drawing.Point(88, 460);
            this.rpdropdown3.Name = "rpdropdown3";
            this.rpdropdown3.Size = new System.Drawing.Size(121, 21);
            this.rpdropdown3.TabIndex = 12;
            // 
            // rpdropdown4
            // 
            this.rpdropdown4.FormattingEnabled = true;
            this.rpdropdown4.Location = new System.Drawing.Point(88, 485);
            this.rpdropdown4.Name = "rpdropdown4";
            this.rpdropdown4.Size = new System.Drawing.Size(121, 21);
            this.rpdropdown4.TabIndex = 13;
            // 
            // defaultrplabel
            // 
            this.defaultrplabel.AutoSize = true;
            this.defaultrplabel.Location = new System.Drawing.Point(10, 418);
            this.defaultrplabel.Name = "defaultrplabel";
            this.defaultrplabel.Size = new System.Drawing.Size(41, 13);
            this.defaultrplabel.TabIndex = 14;
            this.defaultrplabel.Text = "Default";
            // 
            // rp1label
            // 
            this.rp1label.AutoSize = true;
            this.rp1label.Location = new System.Drawing.Point(10, 440);
            this.rp1label.Name = "rp1label";
            this.rp1label.Size = new System.Drawing.Size(46, 13);
            this.rp1label.TabIndex = 15;
            this.rp1label.Text = "Status 2";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(10, 464);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(46, 13);
            this.label6.TabIndex = 16;
            this.label6.Text = "Status 3";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(10, 489);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(46, 13);
            this.label7.TabIndex = 17;
            this.label7.Text = "Status 4";
            // 
            // rptextbox2
            // 
            this.rptextbox2.Location = new System.Drawing.Point(213, 438);
            this.rptextbox2.Name = "rptextbox2";
            this.rptextbox2.Size = new System.Drawing.Size(346, 20);
            this.rptextbox2.TabIndex = 4;
            // 
            // rptextbox3
            // 
            this.rptextbox3.Location = new System.Drawing.Point(213, 461);
            this.rptextbox3.Name = "rptextbox3";
            this.rptextbox3.Size = new System.Drawing.Size(346, 20);
            this.rptextbox3.TabIndex = 4;
            // 
            // rptextbox4
            // 
            this.rptextbox4.Location = new System.Drawing.Point(213, 486);
            this.rptextbox4.Name = "rptextbox4";
            this.rptextbox4.Size = new System.Drawing.Size(346, 20);
            this.rptextbox4.TabIndex = 4;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(10, 396);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(65, 13);
            this.label4.TabIndex = 18;
            this.label4.Text = "Stream URL";
            // 
            // streamurl
            // 
            this.streamurl.Location = new System.Drawing.Point(88, 393);
            this.streamurl.Name = "streamurl";
            this.streamurl.Size = new System.Drawing.Size(471, 20);
            this.streamurl.TabIndex = 19;
            // 
            // DiscordBot
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(569, 558);
            this.Controls.Add(this.streamurl);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.rp1label);
            this.Controls.Add(this.defaultrplabel);
            this.Controls.Add(this.rpdropdown4);
            this.Controls.Add(this.rpdropdown3);
            this.Controls.Add(this.rpdropdown2);
            this.Controls.Add(this.multirp);
            this.Controls.Add(this.disconnectbtn);
            this.Controls.Add(this.rpbutton);
            this.Controls.Add(this.connect_btn);
            this.Controls.Add(this.consoleoutput);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.rptextbox4);
            this.Controls.Add(this.rptextbox3);
            this.Controls.Add(this.rptextbox2);
            this.Controls.Add(this.rptextbox);
            this.Controls.Add(this.rpdropdown);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.token_tb);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "DiscordBot";
            this.Text = "Discord Bot";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox token_tb;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox rpdropdown;
        private System.Windows.Forms.TextBox rptextbox;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.RichTextBox consoleoutput;
        private System.Windows.Forms.Button connect_btn;
        private System.Windows.Forms.Button rpbutton;
        private System.Windows.Forms.Button disconnectbtn;
        private System.Windows.Forms.CheckBox multirp;
        private System.Windows.Forms.ComboBox rpdropdown2;
        private System.Windows.Forms.ComboBox rpdropdown3;
        private System.Windows.Forms.ComboBox rpdropdown4;
        private System.Windows.Forms.Label defaultrplabel;
        private System.Windows.Forms.Label rp1label;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox rptextbox2;
        private System.Windows.Forms.TextBox rptextbox3;
        private System.Windows.Forms.TextBox rptextbox4;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox streamurl;
    }
}

