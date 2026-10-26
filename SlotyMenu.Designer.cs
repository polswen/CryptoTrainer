namespace CryptoTrainer
{
    partial class SlotsMenu
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
            Slot1Btn = new Button();
            Slot2Btn = new Button();
            Slot3Btn = new Button();
            progressBar1 = new ProgressBar();
            progressBar2 = new ProgressBar();
            progressBar3 = new ProgressBar();
            label1 = new Label();
            label2 = new Label();
            label3 = new Label();
            clearSlot1Btn = new Button();
            clearSlot2Btn = new Button();
            clearSlot3Btn = new Button();
            SuspendLayout();
            // 
            // Slot1Btn
            // 
            Slot1Btn.BackColor = SystemColors.ActiveCaption;
            Slot1Btn.Location = new Point(256, 175);
            Slot1Btn.Name = "Slot1Btn";
            Slot1Btn.Size = new Size(185, 84);
            Slot1Btn.TabIndex = 0;
            Slot1Btn.Text = "Slot 1";
            Slot1Btn.UseVisualStyleBackColor = false;
            Slot1Btn.Click += Slot1Btn_Click;
            // 
            // Slot2Btn
            // 
            Slot2Btn.BackColor = SystemColors.ActiveCaption;
            Slot2Btn.Location = new Point(256, 271);
            Slot2Btn.Name = "Slot2Btn";
            Slot2Btn.Size = new Size(185, 84);
            Slot2Btn.TabIndex = 1;
            Slot2Btn.Text = "Slot 2";
            Slot2Btn.UseVisualStyleBackColor = false;
            // 
            // Slot3Btn
            // 
            Slot3Btn.BackColor = SystemColors.ActiveCaption;
            Slot3Btn.Location = new Point(256, 365);
            Slot3Btn.Name = "Slot3Btn";
            Slot3Btn.Size = new Size(185, 84);
            Slot3Btn.TabIndex = 2;
            Slot3Btn.Text = "Slot 3";
            Slot3Btn.UseVisualStyleBackColor = false;
            // 
            // progressBar1
            // 
            progressBar1.BackColor = SystemColors.ActiveCaption;
            progressBar1.Location = new Point(472, 175);
            progressBar1.Maximum = 15;
            progressBar1.Name = "progressBar1";
            progressBar1.Size = new Size(145, 84);
            progressBar1.Step = 1;
            progressBar1.TabIndex = 3;
            // 
            // progressBar2
            // 
            progressBar2.BackColor = SystemColors.ActiveCaption;
            progressBar2.Location = new Point(472, 271);
            progressBar2.Maximum = 15;
            progressBar2.Name = "progressBar2";
            progressBar2.Size = new Size(145, 84);
            progressBar2.Step = 1;
            progressBar2.TabIndex = 4;
            // 
            // progressBar3
            // 
            progressBar3.BackColor = SystemColors.ActiveCaption;
            progressBar3.Location = new Point(472, 365);
            progressBar3.Maximum = 15;
            progressBar3.Name = "progressBar3";
            progressBar3.Size = new Size(145, 84);
            progressBar3.Step = 1;
            progressBar3.TabIndex = 5;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.BackColor = SystemColors.ActiveCaption;
            label1.Location = new Point(623, 175);
            label1.Name = "label1";
            label1.Size = new Size(46, 15);
            label1.TabIndex = 6;
            label1.Text = "0/100%";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.BackColor = SystemColors.ActiveCaption;
            label2.Location = new Point(623, 271);
            label2.Name = "label2";
            label2.Size = new Size(46, 15);
            label2.TabIndex = 7;
            label2.Text = "0/100%";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.BackColor = SystemColors.ActiveCaption;
            label3.Location = new Point(623, 365);
            label3.Name = "label3";
            label3.Size = new Size(46, 15);
            label3.TabIndex = 8;
            label3.Text = "0/100%";
            label3.Click += label3_Click;
            // 
            // clearSlot1Btn
            // 
            clearSlot1Btn.BackColor = SystemColors.ActiveCaption;
            clearSlot1Btn.Location = new Point(623, 236);
            clearSlot1Btn.Name = "clearSlot1Btn";
            clearSlot1Btn.Size = new Size(117, 23);
            clearSlot1Btn.TabIndex = 9;
            clearSlot1Btn.Text = "Wyczyść slot 1";
            clearSlot1Btn.UseVisualStyleBackColor = false;
            // 
            // clearSlot2Btn
            // 
            clearSlot2Btn.BackColor = SystemColors.ActiveCaption;
            clearSlot2Btn.Location = new Point(623, 332);
            clearSlot2Btn.Name = "clearSlot2Btn";
            clearSlot2Btn.Size = new Size(117, 23);
            clearSlot2Btn.TabIndex = 10;
            clearSlot2Btn.Text = "Wyczyść slot 2";
            clearSlot2Btn.UseVisualStyleBackColor = false;
            clearSlot2Btn.Click += button5_Click;
            // 
            // clearSlot3Btn
            // 
            clearSlot3Btn.BackColor = SystemColors.ActiveCaption;
            clearSlot3Btn.Location = new Point(623, 426);
            clearSlot3Btn.Name = "clearSlot3Btn";
            clearSlot3Btn.Size = new Size(117, 23);
            clearSlot3Btn.TabIndex = 11;
            clearSlot3Btn.Text = "Wyczyść slot 3";
            clearSlot3Btn.UseVisualStyleBackColor = false;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackgroundImage = Properties.Resources._25336;
            ClientSize = new Size(976, 697);
            Controls.Add(clearSlot3Btn);
            Controls.Add(clearSlot2Btn);
            Controls.Add(clearSlot1Btn);
            Controls.Add(label3);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(progressBar3);
            Controls.Add(progressBar2);
            Controls.Add(progressBar1);
            Controls.Add(Slot3Btn);
            Controls.Add(Slot2Btn);
            Controls.Add(Slot1Btn);
            Name = "Form1";
            Text = "Form1";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button Slot1Btn;
        private Button Slot2Btn;
        private Button Slot3Btn;
        private ProgressBar progressBar1;
        private ProgressBar progressBar2;
        private ProgressBar progressBar3;
        private Label label1;
        private Label label2;
        private Label label3;
        private Button clearSlot1Btn;
        private Button clearSlot2Btn;
        private Button clearSlot3Btn;
    }
}