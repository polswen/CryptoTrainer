namespace CryptoTrainer
{
    partial class Menu
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            graj = new Button();
            wyjdz = new Button();
            SuspendLayout();
            // 
            // graj
            // 
            graj.BackColor = SystemColors.ActiveCaption;
            graj.Location = new Point(378, 157);
            graj.Name = "graj";
            graj.Size = new Size(185, 84);
            graj.TabIndex = 0;
            graj.Text = "Graj";
            graj.UseVisualStyleBackColor = false;
            graj.Click += graj_Click_1;
            // 
            // wyjdz
            // 
            wyjdz.BackColor = SystemColors.ActiveCaption;
            wyjdz.Location = new Point(378, 377);
            wyjdz.Name = "wyjdz";
            wyjdz.Size = new Size(185, 84);
            wyjdz.TabIndex = 1;
            wyjdz.Text = "Wyjdź";
            wyjdz.UseVisualStyleBackColor = false;
            wyjdz.Click += wyjdz_Click;
            // 
            // Menu
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackgroundImage = Properties.Resources._25336;
            ClientSize = new Size(976, 697);
            Controls.Add(wyjdz);
            Controls.Add(graj);
            Name = "Menu";
            Text = "Form1";
            Load += Form1_Load;
            ResumeLayout(false);
        }

        #endregion

        private Button graj;
        private Button wyjdz;
    }
}
