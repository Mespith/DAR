namespace DARpracticum
{
    partial class Form1
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
            this.createButton = new System.Windows.Forms.Button();
            this.fillButton = new System.Windows.Forms.Button();
            this.createMetaData = new System.Windows.Forms.Button();
            this.fillMetaDatabase = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // createButton
            // 
            this.createButton.Location = new System.Drawing.Point(85, 48);
            this.createButton.Name = "createButton";
            this.createButton.Size = new System.Drawing.Size(108, 23);
            this.createButton.TabIndex = 0;
            this.createButton.Text = "Create database";
            this.createButton.UseVisualStyleBackColor = true;
            this.createButton.Click += new System.EventHandler(this.createButton_Click);
            // 
            // fillButton
            // 
            this.fillButton.Location = new System.Drawing.Point(85, 78);
            this.fillButton.Name = "fillButton";
            this.fillButton.Size = new System.Drawing.Size(108, 23);
            this.fillButton.TabIndex = 1;
            this.fillButton.Text = "Fill database";
            this.fillButton.UseVisualStyleBackColor = true;
            this.fillButton.Click += new System.EventHandler(this.fillButton_Click);
            // 
            // createMetaData
            // 
            this.createMetaData.Location = new System.Drawing.Point(85, 145);
            this.createMetaData.Name = "createMetaData";
            this.createMetaData.Size = new System.Drawing.Size(108, 35);
            this.createMetaData.TabIndex = 2;
            this.createMetaData.Text = "Create Meta Database";
            this.createMetaData.UseVisualStyleBackColor = true;
            this.createMetaData.Click += new System.EventHandler(this.createMetaData_Click);
            // 
            // fillMetaDatabase
            // 
            this.fillMetaDatabase.Location = new System.Drawing.Point(85, 187);
            this.fillMetaDatabase.Name = "fillMetaDatabase";
            this.fillMetaDatabase.Size = new System.Drawing.Size(108, 23);
            this.fillMetaDatabase.TabIndex = 3;
            this.fillMetaDatabase.Text = "Fill meta database";
            this.fillMetaDatabase.UseVisualStyleBackColor = true;
            this.fillMetaDatabase.Click += new System.EventHandler(this.fillMetaDatabase_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 261);
            this.Controls.Add(this.fillMetaDatabase);
            this.Controls.Add(this.createMetaData);
            this.Controls.Add(this.fillButton);
            this.Controls.Add(this.createButton);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button createButton;
        private System.Windows.Forms.Button fillButton;
        private System.Windows.Forms.Button createMetaData;
        private System.Windows.Forms.Button fillMetaDatabase;
    }
}

