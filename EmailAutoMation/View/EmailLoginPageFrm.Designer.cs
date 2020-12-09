namespace TradeWolfRobot.EmailAutomation.View
{
    partial class EmailLoginPageFrm
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
            this.LoginPage = new System.Windows.Forms.Panel();
            this.NoticeBrowserPanel = new System.Windows.Forms.Panel();
            this.LoginPage.SuspendLayout();
            this.SuspendLayout();
            // 
            // LoginPage
            // 
            this.LoginPage.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.LoginPage.Controls.Add(this.NoticeBrowserPanel);
            this.LoginPage.Location = new System.Drawing.Point(12, 1);
            this.LoginPage.Name = "LoginPage";
            this.LoginPage.Size = new System.Drawing.Size(1276, 679);
            this.LoginPage.TabIndex = 0;
            this.LoginPage.Paint += new System.Windows.Forms.PaintEventHandler(this.LoginPage_Paint);
            // 
            // NoticeBrowserPanel
            // 
            this.NoticeBrowserPanel.Location = new System.Drawing.Point(404, 3);
            this.NoticeBrowserPanel.Name = "NoticeBrowserPanel";
            this.NoticeBrowserPanel.Size = new System.Drawing.Size(421, 35);
            this.NoticeBrowserPanel.TabIndex = 0;
            // 
            // EmailLoginPageFrm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1300, 692);
            this.Controls.Add(this.LoginPage);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "EmailLoginPageFrm";
            this.Text = "EmailLoginPageFrm";
            this.Load += new System.EventHandler(this.EmailLoginPageFrm_Load);
            this.LoginPage.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        public System.Windows.Forms.Panel LoginPage;
        public System.Windows.Forms.Panel NoticeBrowserPanel;
    }
}