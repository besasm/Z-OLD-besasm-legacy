﻿namespace SystemsAnalysis.Grid.GridAnalysis
{
    partial class FormServerDatabaseUserIDPasswordDomainEntry
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
            this.labelDatabase = new System.Windows.Forms.Label();
            this.labelServer = new System.Windows.Forms.Label();
            this.labelUserID = new System.Windows.Forms.Label();
            this.labelPassword = new System.Windows.Forms.Label();
            this.labelDomain = new System.Windows.Forms.Label();
            this.textBoxServer = new System.Windows.Forms.TextBox();
            this.textBoxDatabase = new System.Windows.Forms.TextBox();
            this.textBoxUserID = new System.Windows.Forms.TextBox();
            this.textBoxPassword = new System.Windows.Forms.TextBox();
            this.textBoxDomain = new System.Windows.Forms.TextBox();
            this.buttonOK = new System.Windows.Forms.Button();
            this.checkBoxUseTrustedConnection = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // labelDatabase
            // 
            this.labelDatabase.AutoSize = true;
            this.labelDatabase.Location = new System.Drawing.Point(40, 45);
            this.labelDatabase.Name = "labelDatabase";
            this.labelDatabase.Size = new System.Drawing.Size(56, 13);
            this.labelDatabase.TabIndex = 0;
            this.labelDatabase.Text = "Database:";
            // 
            // labelServer
            // 
            this.labelServer.AutoSize = true;
            this.labelServer.Location = new System.Drawing.Point(55, 15);
            this.labelServer.Name = "labelServer";
            this.labelServer.Size = new System.Drawing.Size(41, 13);
            this.labelServer.TabIndex = 1;
            this.labelServer.Text = "Server:";
            // 
            // labelUserID
            // 
            this.labelUserID.AutoSize = true;
            this.labelUserID.Location = new System.Drawing.Point(50, 75);
            this.labelUserID.Name = "labelUserID";
            this.labelUserID.Size = new System.Drawing.Size(46, 13);
            this.labelUserID.TabIndex = 2;
            this.labelUserID.Text = "User ID:";
            // 
            // labelPassword
            // 
            this.labelPassword.AutoSize = true;
            this.labelPassword.Location = new System.Drawing.Point(40, 105);
            this.labelPassword.Name = "labelPassword";
            this.labelPassword.Size = new System.Drawing.Size(56, 13);
            this.labelPassword.TabIndex = 3;
            this.labelPassword.Text = "Password:";
            // 
            // labelDomain
            // 
            this.labelDomain.AutoSize = true;
            this.labelDomain.Location = new System.Drawing.Point(50, 135);
            this.labelDomain.Name = "labelDomain";
            this.labelDomain.Size = new System.Drawing.Size(46, 13);
            this.labelDomain.TabIndex = 4;
            this.labelDomain.Text = "Domain:";
            // 
            // textBoxServer
            // 
            this.textBoxServer.Location = new System.Drawing.Point(102, 12);
            this.textBoxServer.Name = "textBoxServer";
            this.textBoxServer.Size = new System.Drawing.Size(167, 20);
            this.textBoxServer.TabIndex = 5;
            // 
            // textBoxDatabase
            // 
            this.textBoxDatabase.Location = new System.Drawing.Point(102, 42);
            this.textBoxDatabase.Name = "textBoxDatabase";
            this.textBoxDatabase.Size = new System.Drawing.Size(167, 20);
            this.textBoxDatabase.TabIndex = 6;
            // 
            // textBoxUserID
            // 
            this.textBoxUserID.Location = new System.Drawing.Point(102, 72);
            this.textBoxUserID.Name = "textBoxUserID";
            this.textBoxUserID.Size = new System.Drawing.Size(167, 20);
            this.textBoxUserID.TabIndex = 7;
            // 
            // textBoxPassword
            // 
            this.textBoxPassword.Location = new System.Drawing.Point(102, 102);
            this.textBoxPassword.Name = "textBoxPassword";
            this.textBoxPassword.Size = new System.Drawing.Size(167, 20);
            this.textBoxPassword.TabIndex = 8;
            // 
            // textBoxDomain
            // 
            this.textBoxDomain.Location = new System.Drawing.Point(102, 132);
            this.textBoxDomain.Name = "textBoxDomain";
            this.textBoxDomain.Size = new System.Drawing.Size(167, 20);
            this.textBoxDomain.TabIndex = 9;
            // 
            // buttonOK
            // 
            this.buttonOK.Location = new System.Drawing.Point(223, 195);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(82, 32);
            this.buttonOK.TabIndex = 10;
            this.buttonOK.Text = "OK";
            this.buttonOK.UseVisualStyleBackColor = true;
            this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click_1);
            // 
            // checkBoxUseTrustedConnection
            // 
            this.checkBoxUseTrustedConnection.AutoSize = true;
            this.checkBoxUseTrustedConnection.Location = new System.Drawing.Point(128, 172);
            this.checkBoxUseTrustedConnection.Name = "checkBoxUseTrustedConnection";
            this.checkBoxUseTrustedConnection.Size = new System.Drawing.Size(141, 17);
            this.checkBoxUseTrustedConnection.TabIndex = 11;
            this.checkBoxUseTrustedConnection.Text = "Use Trusted Connection";
            this.checkBoxUseTrustedConnection.UseVisualStyleBackColor = true;
            // 
            // FormServerDatabaseUserIDPasswordDomainEntry
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(317, 239);
            this.Controls.Add(this.checkBoxUseTrustedConnection);
            this.Controls.Add(this.buttonOK);
            this.Controls.Add(this.textBoxDomain);
            this.Controls.Add(this.textBoxPassword);
            this.Controls.Add(this.textBoxUserID);
            this.Controls.Add(this.textBoxDatabase);
            this.Controls.Add(this.textBoxServer);
            this.Controls.Add(this.labelDomain);
            this.Controls.Add(this.labelPassword);
            this.Controls.Add(this.labelUserID);
            this.Controls.Add(this.labelServer);
            this.Controls.Add(this.labelDatabase);
            this.Name = "FormServerDatabaseUserIDPasswordDomainEntry";
            this.Text = "FormServerDatabaseUserIDPasswordDomainEntry";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormServerDatabaseUserIDPasswordDomainEntry_FormClosing);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label labelDatabase;
        private System.Windows.Forms.Label labelServer;
        private System.Windows.Forms.Label labelUserID;
        private System.Windows.Forms.Label labelPassword;
        private System.Windows.Forms.Label labelDomain;
        private System.Windows.Forms.TextBox textBoxServer;
        private System.Windows.Forms.TextBox textBoxDatabase;
        private System.Windows.Forms.TextBox textBoxUserID;
        private System.Windows.Forms.TextBox textBoxPassword;
        private System.Windows.Forms.TextBox textBoxDomain;
        private System.Windows.Forms.Button buttonOK;
        private System.Windows.Forms.CheckBox checkBoxUseTrustedConnection;
    }
}