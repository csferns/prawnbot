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
            this.token_tb = new System.Windows.Forms.TextBox();
            this.rpdropdown = new System.Windows.Forms.ComboBox();
            this.rptextbox = new System.Windows.Forms.TextBox();
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
            this.label5 = new System.Windows.Forms.Label();
            this.delaytime = new System.Windows.Forms.NumericUpDown();
            this.statusdropdown = new System.Windows.Forms.ComboBox();
            this.statuslabel = new System.Windows.Forms.Label();
            this.statusbutton = new System.Windows.Forms.Button();
            this.rpgroup = new System.Windows.Forms.GroupBox();
            this.controlsgroup = new System.Windows.Forms.GroupBox();
            this.consolegroup = new System.Windows.Forms.GroupBox();
            this.tokengroup = new System.Windows.Forms.GroupBox();
            ((System.ComponentModel.ISupportInitialize)(this.delaytime)).BeginInit();
            this.rpgroup.SuspendLayout();
            this.controlsgroup.SuspendLayout();
            this.consolegroup.SuspendLayout();
            this.tokengroup.SuspendLayout();
            this.SuspendLayout();
            // 
            // token_tb
            // 
            this.token_tb.Location = new System.Drawing.Point(9, 14);
            this.token_tb.Name = "token_tb";
            this.token_tb.Size = new System.Drawing.Size(799, 20);
            this.token_tb.TabIndex = 1;
            // 
            // rpdropdown
            // 
            this.rpdropdown.FormattingEnabled = true;
            this.rpdropdown.Location = new System.Drawing.Point(77, 81);
            this.rpdropdown.Name = "rpdropdown";
            this.rpdropdown.Size = new System.Drawing.Size(121, 21);
            this.rpdropdown.TabIndex = 3;
            // 
            // rptextbox
            // 
            this.rptextbox.Location = new System.Drawing.Point(204, 82);
            this.rptextbox.Name = "rptextbox";
            this.rptextbox.Size = new System.Drawing.Size(344, 20);
            this.rptextbox.TabIndex = 4;
            // 
            // consoleoutput
            // 
            this.consoleoutput.Location = new System.Drawing.Point(6, 19);
            this.consoleoutput.Name = "consoleoutput";
            this.consoleoutput.ReadOnly = true;
            this.consoleoutput.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
            this.consoleoutput.Size = new System.Drawing.Size(802, 479);
            this.consoleoutput.TabIndex = 6;
            this.consoleoutput.TabStop = false;
            this.consoleoutput.Text = "";
            // 
            // connect_btn
            // 
            this.connect_btn.BackColor = System.Drawing.Color.LightGreen;
            this.connect_btn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.connect_btn.ForeColor = System.Drawing.SystemColors.ControlText;
            this.connect_btn.Location = new System.Drawing.Point(8, 22);
            this.connect_btn.Name = "connect_btn";
            this.connect_btn.Size = new System.Drawing.Size(225, 34);
            this.connect_btn.TabIndex = 7;
            this.connect_btn.Text = "Connect";
            this.connect_btn.UseVisualStyleBackColor = false;
            this.connect_btn.Click += new System.EventHandler(this.connect_btn_Click);
            // 
            // rpbutton
            // 
            this.rpbutton.BackColor = System.Drawing.Color.Gray;
            this.rpbutton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.rpbutton.Location = new System.Drawing.Point(8, 102);
            this.rpbutton.Name = "rpbutton";
            this.rpbutton.Size = new System.Drawing.Size(225, 34);
            this.rpbutton.TabIndex = 8;
            this.rpbutton.Text = "Update Rich Presence";
            this.rpbutton.UseVisualStyleBackColor = false;
            this.rpbutton.Click += new System.EventHandler(this.rpbutton_Click);
            // 
            // disconnectbtn
            // 
            this.disconnectbtn.BackColor = System.Drawing.Color.IndianRed;
            this.disconnectbtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.disconnectbtn.ForeColor = System.Drawing.SystemColors.ControlText;
            this.disconnectbtn.Location = new System.Drawing.Point(8, 62);
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
            this.multirp.Location = new System.Drawing.Point(373, 18);
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
            this.rpdropdown2.Location = new System.Drawing.Point(77, 108);
            this.rpdropdown2.Name = "rpdropdown2";
            this.rpdropdown2.Size = new System.Drawing.Size(121, 21);
            this.rpdropdown2.TabIndex = 11;
            // 
            // rpdropdown3
            // 
            this.rpdropdown3.FormattingEnabled = true;
            this.rpdropdown3.Location = new System.Drawing.Point(77, 134);
            this.rpdropdown3.Name = "rpdropdown3";
            this.rpdropdown3.Size = new System.Drawing.Size(121, 21);
            this.rpdropdown3.TabIndex = 12;
            // 
            // rpdropdown4
            // 
            this.rpdropdown4.FormattingEnabled = true;
            this.rpdropdown4.Location = new System.Drawing.Point(77, 161);
            this.rpdropdown4.Name = "rpdropdown4";
            this.rpdropdown4.Size = new System.Drawing.Size(121, 21);
            this.rpdropdown4.TabIndex = 13;
            // 
            // defaultrplabel
            // 
            this.defaultrplabel.AutoSize = true;
            this.defaultrplabel.Location = new System.Drawing.Point(30, 84);
            this.defaultrplabel.Name = "defaultrplabel";
            this.defaultrplabel.Size = new System.Drawing.Size(41, 13);
            this.defaultrplabel.TabIndex = 14;
            this.defaultrplabel.Text = "Default";
            // 
            // rp1label
            // 
            this.rp1label.AutoSize = true;
            this.rp1label.Location = new System.Drawing.Point(25, 111);
            this.rp1label.Name = "rp1label";
            this.rp1label.Size = new System.Drawing.Size(46, 13);
            this.rp1label.TabIndex = 15;
            this.rp1label.Text = "Status 2";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(25, 137);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(46, 13);
            this.label6.TabIndex = 16;
            this.label6.Text = "Status 3";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(25, 164);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(46, 13);
            this.label7.TabIndex = 17;
            this.label7.Text = "Status 4";
            // 
            // rptextbox2
            // 
            this.rptextbox2.Location = new System.Drawing.Point(204, 109);
            this.rptextbox2.Name = "rptextbox2";
            this.rptextbox2.Size = new System.Drawing.Size(344, 20);
            this.rptextbox2.TabIndex = 4;
            // 
            // rptextbox3
            // 
            this.rptextbox3.Location = new System.Drawing.Point(204, 135);
            this.rptextbox3.Name = "rptextbox3";
            this.rptextbox3.Size = new System.Drawing.Size(344, 20);
            this.rptextbox3.TabIndex = 4;
            // 
            // rptextbox4
            // 
            this.rptextbox4.Location = new System.Drawing.Point(204, 162);
            this.rptextbox4.Name = "rptextbox4";
            this.rptextbox4.Size = new System.Drawing.Size(344, 20);
            this.rptextbox4.TabIndex = 4;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 53);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(65, 13);
            this.label4.TabIndex = 18;
            this.label4.Text = "Stream URL";
            // 
            // streamurl
            // 
            this.streamurl.Location = new System.Drawing.Point(77, 50);
            this.streamurl.Name = "streamurl";
            this.streamurl.Size = new System.Drawing.Size(471, 20);
            this.streamurl.TabIndex = 19;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(15, 192);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(56, 13);
            this.label5.TabIndex = 20;
            this.label5.Text = "Delay time";
            // 
            // delaytime
            // 
            this.delaytime.Location = new System.Drawing.Point(77, 190);
            this.delaytime.Name = "delaytime";
            this.delaytime.Size = new System.Drawing.Size(121, 20);
            this.delaytime.TabIndex = 21;
            // 
            // statusdropdown
            // 
            this.statusdropdown.FormattingEnabled = true;
            this.statusdropdown.Location = new System.Drawing.Point(77, 19);
            this.statusdropdown.Name = "statusdropdown";
            this.statusdropdown.Size = new System.Drawing.Size(121, 21);
            this.statusdropdown.TabIndex = 22;
            // 
            // statuslabel
            // 
            this.statuslabel.AutoSize = true;
            this.statuslabel.Location = new System.Drawing.Point(34, 22);
            this.statuslabel.Name = "statuslabel";
            this.statuslabel.Size = new System.Drawing.Size(37, 13);
            this.statuslabel.TabIndex = 23;
            this.statuslabel.Text = "Status";
            // 
            // statusbutton
            // 
            this.statusbutton.BackColor = System.Drawing.Color.Gray;
            this.statusbutton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.statusbutton.Location = new System.Drawing.Point(8, 142);
            this.statusbutton.Name = "statusbutton";
            this.statusbutton.Size = new System.Drawing.Size(225, 34);
            this.statusbutton.TabIndex = 24;
            this.statusbutton.Text = "Change status";
            this.statusbutton.UseVisualStyleBackColor = false;
            this.statusbutton.Click += new System.EventHandler(this.Statusbutton_Click);
            // 
            // rpgroup
            // 
            this.rpgroup.Controls.Add(this.streamurl);
            this.rpgroup.Controls.Add(this.label4);
            this.rpgroup.Controls.Add(this.statuslabel);
            this.rpgroup.Controls.Add(this.rpdropdown);
            this.rpgroup.Controls.Add(this.defaultrplabel);
            this.rpgroup.Controls.Add(this.statusdropdown);
            this.rpgroup.Controls.Add(this.multirp);
            this.rpgroup.Controls.Add(this.delaytime);
            this.rpgroup.Controls.Add(this.rptextbox);
            this.rpgroup.Controls.Add(this.label5);
            this.rpgroup.Controls.Add(this.rptextbox2);
            this.rpgroup.Controls.Add(this.label7);
            this.rpgroup.Controls.Add(this.rpdropdown2);
            this.rpgroup.Controls.Add(this.rpdropdown4);
            this.rpgroup.Controls.Add(this.label6);
            this.rpgroup.Controls.Add(this.rpdropdown3);
            this.rpgroup.Controls.Add(this.rp1label);
            this.rpgroup.Controls.Add(this.rptextbox4);
            this.rpgroup.Controls.Add(this.rptextbox3);
            this.rpgroup.Location = new System.Drawing.Point(12, 571);
            this.rpgroup.Name = "rpgroup";
            this.rpgroup.Size = new System.Drawing.Size(561, 219);
            this.rpgroup.TabIndex = 25;
            this.rpgroup.TabStop = false;
            this.rpgroup.Text = "Rich Presence";
            // 
            // controlsgroup
            // 
            this.controlsgroup.Controls.Add(this.connect_btn);
            this.controlsgroup.Controls.Add(this.disconnectbtn);
            this.controlsgroup.Controls.Add(this.statusbutton);
            this.controlsgroup.Controls.Add(this.rpbutton);
            this.controlsgroup.Location = new System.Drawing.Point(587, 571);
            this.controlsgroup.Name = "controlsgroup";
            this.controlsgroup.Size = new System.Drawing.Size(239, 219);
            this.controlsgroup.TabIndex = 26;
            this.controlsgroup.TabStop = false;
            this.controlsgroup.Text = "Controls";
            // 
            // consolegroup
            // 
            this.consolegroup.Controls.Add(this.consoleoutput);
            this.consolegroup.Location = new System.Drawing.Point(12, 57);
            this.consolegroup.Name = "consolegroup";
            this.consolegroup.Size = new System.Drawing.Size(814, 508);
            this.consolegroup.TabIndex = 27;
            this.consolegroup.TabStop = false;
            this.consolegroup.Text = "Console";
            // 
            // tokengroup
            // 
            this.tokengroup.Controls.Add(this.token_tb);
            this.tokengroup.Location = new System.Drawing.Point(12, 11);
            this.tokengroup.Name = "tokengroup";
            this.tokengroup.Size = new System.Drawing.Size(814, 40);
            this.tokengroup.TabIndex = 28;
            this.tokengroup.TabStop = false;
            this.tokengroup.Text = "Token";
            // 
            // DiscordBot
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(833, 802);
            this.Controls.Add(this.tokengroup);
            this.Controls.Add(this.consolegroup);
            this.Controls.Add(this.rpgroup);
            this.Controls.Add(this.controlsgroup);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "DiscordBot";
            this.Text = "Discord Bot";
            ((System.ComponentModel.ISupportInitialize)(this.delaytime)).EndInit();
            this.rpgroup.ResumeLayout(false);
            this.rpgroup.PerformLayout();
            this.controlsgroup.ResumeLayout(false);
            this.consolegroup.ResumeLayout(false);
            this.tokengroup.ResumeLayout(false);
            this.tokengroup.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.TextBox token_tb;
        private System.Windows.Forms.ComboBox rpdropdown;
        private System.Windows.Forms.TextBox rptextbox;
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
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.NumericUpDown delaytime;
        private System.Windows.Forms.ComboBox statusdropdown;
        private System.Windows.Forms.Label statuslabel;
        private System.Windows.Forms.Button statusbutton;
        private System.Windows.Forms.GroupBox rpgroup;
        private System.Windows.Forms.GroupBox controlsgroup;
        private System.Windows.Forms.GroupBox consolegroup;
        private System.Windows.Forms.GroupBox tokengroup;
    }
}

