namespace prawnbot
{
    partial class Message
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Message));
            this.availableGuilds = new System.Windows.Forms.ComboBox();
            this.messageContent = new System.Windows.Forms.RichTextBox();
            this.textChannels = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.findTextChannels = new System.Windows.Forms.Button();
            this.sendButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // availableGuilds
            // 
            this.availableGuilds.FormattingEnabled = true;
            this.availableGuilds.Location = new System.Drawing.Point(84, 12);
            this.availableGuilds.Name = "availableGuilds";
            this.availableGuilds.Size = new System.Drawing.Size(121, 21);
            this.availableGuilds.TabIndex = 1;
            this.availableGuilds.SelectedIndexChanged += new System.EventHandler(this.availableGuilds_SelectedIndexChanged);
            // 
            // messageContent
            // 
            this.messageContent.Location = new System.Drawing.Point(84, 65);
            this.messageContent.Name = "messageContent";
            this.messageContent.Size = new System.Drawing.Size(704, 201);
            this.messageContent.TabIndex = 2;
            this.messageContent.Text = "";
            // 
            // textChannels
            // 
            this.textChannels.FormattingEnabled = true;
            this.textChannels.Location = new System.Drawing.Point(84, 39);
            this.textChannels.Name = "textChannels";
            this.textChannels.Size = new System.Drawing.Size(121, 21);
            this.textChannels.TabIndex = 3;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(47, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(31, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "Guild";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(8, 42);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(70, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "Text Channel";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(28, 68);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(50, 13);
            this.label3.TabIndex = 6;
            this.label3.Text = "Message";
            // 
            // findTextChannels
            // 
            this.findTextChannels.Location = new System.Drawing.Point(211, 39);
            this.findTextChannels.Name = "findTextChannels";
            this.findTextChannels.Size = new System.Drawing.Size(51, 21);
            this.findTextChannels.TabIndex = 7;
            this.findTextChannels.Text = "Find";
            this.findTextChannels.UseVisualStyleBackColor = true;
            this.findTextChannels.Click += new System.EventHandler(this.findTextChannels_Click);
            // 
            // sendButton
            // 
            this.sendButton.Location = new System.Drawing.Point(713, 272);
            this.sendButton.Name = "sendButton";
            this.sendButton.Size = new System.Drawing.Size(75, 23);
            this.sendButton.TabIndex = 8;
            this.sendButton.Text = "Send";
            this.sendButton.UseVisualStyleBackColor = true;
            this.sendButton.Click += new System.EventHandler(this.sendButton_Click);
            // 
            // Message
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 304);
            this.Controls.Add(this.sendButton);
            this.Controls.Add(this.findTextChannels);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.textChannels);
            this.Controls.Add(this.messageContent);
            this.Controls.Add(this.availableGuilds);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Message";
            this.Text = "Message";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.ComboBox availableGuilds;
        private System.Windows.Forms.RichTextBox messageContent;
        private System.Windows.Forms.ComboBox textChannels;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button findTextChannels;
        private System.Windows.Forms.Button sendButton;
    }
}