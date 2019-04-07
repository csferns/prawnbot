namespace prawnbot
{
    partial class BotLog
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
            this.messagelog = new System.Windows.Forms.RichTextBox();
            this.eventlog = new System.Windows.Forms.RichTextBox();
            this.SuspendLayout();
            // 
            // messagelog
            // 
            this.messagelog.Location = new System.Drawing.Point(13, 13);
            this.messagelog.Name = "messagelog";
            this.messagelog.Size = new System.Drawing.Size(391, 425);
            this.messagelog.TabIndex = 0;
            this.messagelog.Text = "";
            // 
            // eventlog
            // 
            this.eventlog.Location = new System.Drawing.Point(411, 13);
            this.eventlog.Name = "eventlog";
            this.eventlog.Size = new System.Drawing.Size(377, 425);
            this.eventlog.TabIndex = 1;
            this.eventlog.Text = "";
            // 
            // BotLog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.eventlog);
            this.Controls.Add(this.messagelog);
            this.Name = "BotLog";
            this.Text = "BotLog";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.RichTextBox messagelog;
        private System.Windows.Forms.RichTextBox eventlog;
    }
}