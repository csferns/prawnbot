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
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(38, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Token";
            // 
            // token_tb
            // 
            this.token_tb.Location = new System.Drawing.Point(54, 10);
            this.token_tb.Name = "token_tb";
            this.token_tb.Size = new System.Drawing.Size(734, 20);
            this.token_tb.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(26, 46);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(22, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "RP";
            // 
            // rpdropdown
            // 
            this.rpdropdown.FormattingEnabled = true;
            this.rpdropdown.Location = new System.Drawing.Point(54, 43);
            this.rpdropdown.Name = "rpdropdown";
            this.rpdropdown.Size = new System.Drawing.Size(121, 21);
            this.rpdropdown.TabIndex = 3;
            // 
            // rptextbox
            // 
            this.rptextbox.Location = new System.Drawing.Point(181, 44);
            this.rptextbox.Name = "rptextbox";
            this.rptextbox.Size = new System.Drawing.Size(607, 20);
            this.rptextbox.TabIndex = 4;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 75);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(45, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "Console";
            // 
            // consoleoutput
            // 
            this.consoleoutput.Location = new System.Drawing.Point(55, 75);
            this.consoleoutput.Name = "consoleoutput";
            this.consoleoutput.Size = new System.Drawing.Size(733, 322);
            this.consoleoutput.TabIndex = 6;
            this.consoleoutput.Text = "";
            // 
            // connect_btn
            // 
            this.connect_btn.BackColor = System.Drawing.Color.LightGreen;
            this.connect_btn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.connect_btn.ForeColor = System.Drawing.SystemColors.ControlText;
            this.connect_btn.Location = new System.Drawing.Point(55, 404);
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
            this.rpbutton.Location = new System.Drawing.Point(547, 404);
            this.rpbutton.Name = "rpbutton";
            this.rpbutton.Size = new System.Drawing.Size(240, 34);
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
            this.disconnectbtn.Location = new System.Drawing.Point(301, 404);
            this.disconnectbtn.Name = "disconnectbtn";
            this.disconnectbtn.Size = new System.Drawing.Size(240, 34);
            this.disconnectbtn.TabIndex = 9;
            this.disconnectbtn.Text = "Disconnect";
            this.disconnectbtn.UseVisualStyleBackColor = false;
            this.disconnectbtn.Click += new System.EventHandler(this.disconnectbtn_Click);
            // 
            // DiscordBot
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.disconnectbtn);
            this.Controls.Add(this.rpbutton);
            this.Controls.Add(this.connect_btn);
            this.Controls.Add(this.consoleoutput);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.rptextbox);
            this.Controls.Add(this.rpdropdown);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.token_tb);
            this.Controls.Add(this.label1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
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
    }
}

