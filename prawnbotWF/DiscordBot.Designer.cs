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
            this.consoleoutput = new System.Windows.Forms.RichTextBox();
            this.consolegroup = new System.Windows.Forms.GroupBox();
            this.tokengroup = new System.Windows.Forms.GroupBox();
            this.tokenconnectbutton = new System.Windows.Forms.Button();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.messageToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.sendToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.richPresenceToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.disconnectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.consolegroup.SuspendLayout();
            this.tokengroup.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // token_tb
            // 
            this.token_tb.Location = new System.Drawing.Point(9, 13);
            this.token_tb.Name = "token_tb";
            this.token_tb.Size = new System.Drawing.Size(718, 20);
            this.token_tb.TabIndex = 1;
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
            // consolegroup
            // 
            this.consolegroup.Controls.Add(this.consoleoutput);
            this.consolegroup.Location = new System.Drawing.Point(12, 73);
            this.consolegroup.Name = "consolegroup";
            this.consolegroup.Size = new System.Drawing.Size(814, 508);
            this.consolegroup.TabIndex = 27;
            this.consolegroup.TabStop = false;
            this.consolegroup.Text = "Console";
            // 
            // tokengroup
            // 
            this.tokengroup.Controls.Add(this.tokenconnectbutton);
            this.tokengroup.Controls.Add(this.token_tb);
            this.tokengroup.Location = new System.Drawing.Point(12, 27);
            this.tokengroup.Name = "tokengroup";
            this.tokengroup.Size = new System.Drawing.Size(814, 40);
            this.tokengroup.TabIndex = 28;
            this.tokengroup.TabStop = false;
            this.tokengroup.Text = "Token";
            // 
            // tokenconnectbutton
            // 
            this.tokenconnectbutton.Location = new System.Drawing.Point(733, 11);
            this.tokenconnectbutton.Name = "tokenconnectbutton";
            this.tokenconnectbutton.Size = new System.Drawing.Size(75, 23);
            this.tokenconnectbutton.TabIndex = 2;
            this.tokenconnectbutton.Text = "Connect";
            this.tokenconnectbutton.UseVisualStyleBackColor = true;
            this.tokenconnectbutton.Click += new System.EventHandler(this.tokenconnectbutton_Click);
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.messageToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(836, 24);
            this.menuStrip1.TabIndex = 29;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // messageToolStripMenuItem
            // 
            this.messageToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.sendToolStripMenuItem,
            this.richPresenceToolStripMenuItem,
            this.disconnectToolStripMenuItem});
            this.messageToolStripMenuItem.Name = "messageToolStripMenuItem";
            this.messageToolStripMenuItem.Size = new System.Drawing.Size(50, 20);
            this.messageToolStripMenuItem.Text = "Menu";
            // 
            // sendToolStripMenuItem
            // 
            this.sendToolStripMenuItem.Name = "sendToolStripMenuItem";
            this.sendToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.sendToolStripMenuItem.Text = "Send Message";
            this.sendToolStripMenuItem.Click += new System.EventHandler(this.sendToolStripMenuItem_Click);
            // 
            // richPresenceToolStripMenuItem
            // 
            this.richPresenceToolStripMenuItem.Name = "richPresenceToolStripMenuItem";
            this.richPresenceToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.richPresenceToolStripMenuItem.Text = "Rich Presence";
            this.richPresenceToolStripMenuItem.Click += new System.EventHandler(this.richPresenceToolStripMenuItem_Click);
            // 
            // disconnectToolStripMenuItem
            // 
            this.disconnectToolStripMenuItem.Name = "disconnectToolStripMenuItem";
            this.disconnectToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.disconnectToolStripMenuItem.Text = "Disconnect";
            this.disconnectToolStripMenuItem.Click += new System.EventHandler(this.disconnectToolStripMenuItem_Click);
            // 
            // DiscordBot
            // 
            this.AcceptButton = this.tokenconnectbutton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(836, 590);
            this.Controls.Add(this.tokengroup);
            this.Controls.Add(this.consolegroup);
            this.Controls.Add(this.menuStrip1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "DiscordBot";
            this.Text = "Discord Bot";
            this.consolegroup.ResumeLayout(false);
            this.tokengroup.ResumeLayout(false);
            this.tokengroup.PerformLayout();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.TextBox token_tb;
        private System.Windows.Forms.RichTextBox consoleoutput;
        private System.Windows.Forms.GroupBox consolegroup;
        private System.Windows.Forms.GroupBox tokengroup;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem messageToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem sendToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem richPresenceToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem disconnectToolStripMenuItem;
        private System.Windows.Forms.Button tokenconnectbutton;
    }
}

