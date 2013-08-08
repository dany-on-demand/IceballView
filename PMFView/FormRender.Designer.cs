namespace IceballView
{
    partial class FormRender
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
            this.renderPictureBox = new System.Windows.Forms.PictureBox();
            this.instructionsLabel = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.renderPictureBox)).BeginInit();
            this.SuspendLayout();
            // 
            // renderPictureBox
            // 
            this.renderPictureBox.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.renderPictureBox.BackgroundImage = global::IceballView.Properties.Resources.checker;
            this.renderPictureBox.Location = new System.Drawing.Point(99, 30);
            this.renderPictureBox.Margin = new System.Windows.Forms.Padding(0);
            this.renderPictureBox.MaximumSize = new System.Drawing.Size(512, 512);
            this.renderPictureBox.MinimumSize = new System.Drawing.Size(512, 512);
            this.renderPictureBox.Name = "renderPictureBox";
            this.renderPictureBox.Size = new System.Drawing.Size(512, 512);
            this.renderPictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.renderPictureBox.TabIndex = 0;
            this.renderPictureBox.TabStop = false;
            // 
            // instructionsLabel
            // 
            this.instructionsLabel.AutoSize = true;
            this.instructionsLabel.Font = new System.Drawing.Font("Segoe Script", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.instructionsLabel.ForeColor = System.Drawing.Color.White;
            this.instructionsLabel.Location = new System.Drawing.Point(14, 547);
            this.instructionsLabel.Name = "instructionsLabel";
            this.instructionsLabel.Size = new System.Drawing.Size(679, 22);
            this.instructionsLabel.TabIndex = 1;
            this.instructionsLabel.Text = "Instructions: Drag&&Drop a PMF/VXL/ICEMAP file onto the window to see a preview o" +
    "f it.";
            // 
            // FormRender
            // 
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Black;
            this.ClientSize = new System.Drawing.Size(705, 578);
            this.Controls.Add(this.instructionsLabel);
            this.Controls.Add(this.renderPictureBox);
            this.Name = "FormRender";
            this.Text = "nade (64x64)";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.DragDrop += new System.Windows.Forms.DragEventHandler(this.Form1_DragDrop);
            this.DragEnter += new System.Windows.Forms.DragEventHandler(this.Form1_DragEnter);
            ((System.ComponentModel.ISupportInitialize)(this.renderPictureBox)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox renderPictureBox;
        private System.Windows.Forms.Label instructionsLabel;
    }
}

