namespace StandAloneGFXDKC1
{
    partial class CreateSpriteSheet
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
            this.richTextBox_spriteList = new System.Windows.Forms.RichTextBox();
            this.button_extractExport = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // richTextBox_spriteList
            // 
            this.richTextBox_spriteList.Dock = System.Windows.Forms.DockStyle.Top;
            this.richTextBox_spriteList.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.richTextBox_spriteList.Location = new System.Drawing.Point(0, 0);
            this.richTextBox_spriteList.Name = "richTextBox_spriteList";
            this.richTextBox_spriteList.Size = new System.Drawing.Size(228, 220);
            this.richTextBox_spriteList.TabIndex = 0;
            this.richTextBox_spriteList.Text = "";
            // 
            // button_extractExport
            // 
            this.button_extractExport.Location = new System.Drawing.Point(70, 255);
            this.button_extractExport.Name = "button_extractExport";
            this.button_extractExport.Size = new System.Drawing.Size(90, 23);
            this.button_extractExport.TabIndex = 1;
            this.button_extractExport.Text = "Extract/Export";
            this.button_extractExport.UseVisualStyleBackColor = true;
            this.button_extractExport.Click += new System.EventHandler(this.button_extractExport_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(49, 223);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(129, 26);
            this.label1.TabIndex = 2;
            this.label1.Text = "Press F7 to add + newline\r\nPress F8 to add + -";
            // 
            // CreateSpriteSheet
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(228, 290);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.button_extractExport);
            this.Controls.Add(this.richTextBox_spriteList);
            this.Name = "CreateSpriteSheet";
            this.Text = "CreateSpriteSheet";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RichTextBox richTextBox_spriteList;
        private System.Windows.Forms.Button button_extractExport;
        private System.Windows.Forms.Label label1;
    }
}