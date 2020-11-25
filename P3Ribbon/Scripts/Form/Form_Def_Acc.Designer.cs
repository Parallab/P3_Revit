namespace P3Ribbon.Scripts
{
    partial class Form_Def_Acc
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form_Def_Acc));
            this.label1 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.float_acc = new System.Windows.Forms.TextBox();
            this.z4 = new System.Windows.Forms.Button();
            this.z3 = new System.Windows.Forms.Button();
            this.z2 = new System.Windows.Forms.Button();
            this.z1 = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.b_annulla = new System.Windows.Forms.Button();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.label11 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.c_1 = new System.Windows.Forms.Button();
            this.c_2 = new System.Windows.Forms.Button();
            this.c_3 = new System.Windows.Forms.Button();
            this.c_4 = new System.Windows.Forms.Button();
            this.label12 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.b_ok = new System.Windows.Forms.Button();
            this.label8 = new System.Windows.Forms.Label();
            this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            this.tableLayoutPanel1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Arial", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(24, 11);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(100, 16);
            this.label1.TabIndex = 5;
            this.label1.Text = "α = αg/g\t";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label7
            // 
            this.label7.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(137, 208);
            this.label7.Margin = new System.Windows.Forms.Padding(11, 0, 0, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(207, 13);
            this.label7.TabIndex = 14;
            this.label7.Text = "Sisimicità bassa ( α  ≤ 0.05) ";
            // 
            // label6
            // 
            this.label6.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(137, 168);
            this.label6.Margin = new System.Windows.Forms.Padding(11, 0, 0, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(207, 13);
            this.label6.TabIndex = 13;
            this.label6.Text = "Sisimicità  medio-bassa (0.05 < α  ≤ 0.15) ";
            // 
            // label3
            // 
            this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(137, 129);
            this.label3.Margin = new System.Windows.Forms.Padding(11, 0, 0, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(207, 13);
            this.label3.TabIndex = 12;
            this.label3.Text = "Sisimicità medio-alta (0.15 < α  ≤ 0.25) ";
            // 
            // label5
            // 
            this.label5.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(137, 90);
            this.label5.Margin = new System.Windows.Forms.Padding(11, 0, 0, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(207, 13);
            this.label5.TabIndex = 11;
            this.label5.Text = "Sisimicità alta (0.25 < α  ≤ 0.35) ";
            // 
            // float_acc
            // 
            this.float_acc.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.float_acc.Location = new System.Drawing.Point(128, 9);
            this.float_acc.Margin = new System.Windows.Forms.Padding(2);
            this.float_acc.Name = "float_acc";
            this.float_acc.Size = new System.Drawing.Size(214, 20);
            this.float_acc.TabIndex = 0;
            this.float_acc.TextChanged += new System.EventHandler(this.float_acc_TextChanged);
            // 
            // z4
            // 
            this.z4.BackColor = System.Drawing.Color.LightSteelBlue;
            this.z4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.z4.Font = new System.Drawing.Font("Arial", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.z4.Location = new System.Drawing.Point(22, 197);
            this.z4.Margin = new System.Windows.Forms.Padding(0, 3, 0, 3);
            this.z4.Name = "z4";
            this.z4.Size = new System.Drawing.Size(104, 36);
            this.z4.TabIndex = 9;
            this.z4.Text = "ZONA 4";
            this.z4.UseVisualStyleBackColor = false;
            this.z4.Click += new System.EventHandler(this.z4_Click);
            // 
            // z3
            // 
            this.z3.BackColor = System.Drawing.Color.LightSteelBlue;
            this.z3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.z3.Font = new System.Drawing.Font("Arial", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.z3.Location = new System.Drawing.Point(22, 158);
            this.z3.Margin = new System.Windows.Forms.Padding(0, 3, 0, 3);
            this.z3.Name = "z3";
            this.z3.Size = new System.Drawing.Size(104, 33);
            this.z3.TabIndex = 8;
            this.z3.Text = "ZONA 3";
            this.z3.UseVisualStyleBackColor = false;
            this.z3.Click += new System.EventHandler(this.z3_Click);
            // 
            // z2
            // 
            this.z2.BackColor = System.Drawing.Color.LightSteelBlue;
            this.z2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.z2.Font = new System.Drawing.Font("Arial", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.z2.Location = new System.Drawing.Point(22, 119);
            this.z2.Margin = new System.Windows.Forms.Padding(0, 3, 0, 3);
            this.z2.Name = "z2";
            this.z2.Size = new System.Drawing.Size(104, 33);
            this.z2.TabIndex = 7;
            this.z2.Text = "ZONA 2";
            this.z2.UseVisualStyleBackColor = false;
            this.z2.Click += new System.EventHandler(this.z2_Click);
            // 
            // z1
            // 
            this.z1.BackColor = System.Drawing.Color.LightSteelBlue;
            this.z1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.z1.Font = new System.Drawing.Font("Arial", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.z1.Location = new System.Drawing.Point(22, 80);
            this.z1.Margin = new System.Windows.Forms.Padding(0, 3, 0, 3);
            this.z1.Name = "z1";
            this.z1.Size = new System.Drawing.Size(104, 33);
            this.z1.TabIndex = 6;
            this.z1.Text = "ZONA 1";
            this.z1.UseVisualStyleBackColor = false;
            this.z1.Click += new System.EventHandler(this.z1_Click);
            // 
            // label4
            // 
            this.label4.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Arial", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(43, 41);
            this.label4.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(62, 32);
            this.label4.TabIndex = 5;
            this.label4.Text = "ZONA SISMICA";
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel1.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.tableLayoutPanel1.ColumnCount = 4;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 17.79141F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 82.20859F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 218F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 21F));
            this.tableLayoutPanel1.Controls.Add(this.label4, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.z1, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.z2, 1, 3);
            this.tableLayoutPanel1.Controls.Add(this.z3, 1, 4);
            this.tableLayoutPanel1.Controls.Add(this.z4, 1, 5);
            this.tableLayoutPanel1.Controls.Add(this.label1, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.label3, 2, 3);
            this.tableLayoutPanel1.Controls.Add(this.label6, 2, 4);
            this.tableLayoutPanel1.Controls.Add(this.label7, 2, 5);
            this.tableLayoutPanel1.Controls.Add(this.float_acc, 2, 0);
            this.tableLayoutPanel1.Controls.Add(this.label5, 2, 2);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(9, 40);
            this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 7;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 38F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 39F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 39F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 39F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 39F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 42F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(366, 260);
            this.tableLayoutPanel1.TabIndex = 3;
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.panel1.Controls.Add(this.b_annulla);
            this.panel1.Controls.Add(this.tableLayoutPanel2);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.b_ok);
            this.panel1.Controls.Add(this.label8);
            this.panel1.Controls.Add(this.tableLayoutPanel1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(712, 354);
            this.panel1.TabIndex = 4;
            // 
            // b_annulla
            // 
            this.b_annulla.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.b_annulla.Location = new System.Drawing.Point(579, 312);
            this.b_annulla.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.b_annulla.Name = "b_annulla";
            this.b_annulla.Size = new System.Drawing.Size(58, 21);
            this.b_annulla.TabIndex = 16;
            this.b_annulla.Text = "Annulla";
            this.b_annulla.UseVisualStyleBackColor = false;
            this.b_annulla.Click += new System.EventHandler(this.b_annulla_Click);
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel2.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.tableLayoutPanel2.ColumnCount = 4;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 17.79141F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 82.20859F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 218F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 21F));
            this.tableLayoutPanel2.Controls.Add(this.label11, 2, 3);
            this.tableLayoutPanel2.Controls.Add(this.label10, 2, 2);
            this.tableLayoutPanel2.Controls.Add(this.label9, 2, 1);
            this.tableLayoutPanel2.Controls.Add(this.c_1, 1, 1);
            this.tableLayoutPanel2.Controls.Add(this.c_2, 1, 2);
            this.tableLayoutPanel2.Controls.Add(this.c_3, 1, 3);
            this.tableLayoutPanel2.Controls.Add(this.c_4, 1, 4);
            this.tableLayoutPanel2.Controls.Add(this.label12, 2, 4);
            this.tableLayoutPanel2.Location = new System.Drawing.Point(393, 40);
            this.tableLayoutPanel2.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.Padding = new System.Windows.Forms.Padding(2);
            this.tableLayoutPanel2.RowCount = 5;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 37F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 37F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 74F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 45F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 46F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 16F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(310, 260);
            this.tableLayoutPanel2.TabIndex = 15;
            // 
            // label11
            // 
            this.label11.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(70, 153);
            this.label11.Margin = new System.Windows.Forms.Padding(2, 2, 0, 2);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(216, 39);
            this.label11.TabIndex = 15;
            this.label11.Text = "Costruzioni il cui uso preveda affollamenti signifficativi. Industrie con attivit" +
    "à pericolose per l\'ambiente | Centri commerciali";
            this.label11.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label10
            // 
            this.label10.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.label10.AutoSize = true;
            this.label10.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.label10.Location = new System.Drawing.Point(70, 80);
            this.label10.Margin = new System.Windows.Forms.Padding(2, 2, 0, 2);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(216, 65);
            this.label10.TabIndex = 15;
            this.label10.Text = "Costruzioni cui uso preveda normali affollamenti senza contenuti pericolosi per l" +
    "\'ambiente e senza funzioni pubbliche e sociali essenziali. Industie con attività" +
    " non pericolose per l\'ambiente | Uffici";
            this.label10.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label9
            // 
            this.label9.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(70, 44);
            this.label9.Margin = new System.Windows.Forms.Padding(2, 2, 0, 2);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(216, 26);
            this.label9.TabIndex = 15;
            this.label9.Text = "Costruzioni con presenza solo occasionale di persone | Edifici agricoli";
            this.label9.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // c_1
            // 
            this.c_1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.c_1.BackColor = System.Drawing.Color.LightSteelBlue;
            this.c_1.Font = new System.Drawing.Font("Arial", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.c_1.Location = new System.Drawing.Point(13, 46);
            this.c_1.Margin = new System.Windows.Forms.Padding(0, 3, 0, 3);
            this.c_1.Name = "c_1";
            this.c_1.Size = new System.Drawing.Size(55, 23);
            this.c_1.TabIndex = 6;
            this.c_1.Text = "I";
            this.c_1.UseVisualStyleBackColor = false;
            this.c_1.Click += new System.EventHandler(this.c_1_Click);
            // 
            // c_2
            // 
            this.c_2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.c_2.BackColor = System.Drawing.Color.LightSteelBlue;
            this.c_2.Font = new System.Drawing.Font("Arial", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.c_2.Location = new System.Drawing.Point(13, 103);
            this.c_2.Margin = new System.Windows.Forms.Padding(0, 3, 0, 3);
            this.c_2.Name = "c_2";
            this.c_2.Size = new System.Drawing.Size(55, 20);
            this.c_2.TabIndex = 7;
            this.c_2.Text = "II";
            this.c_2.UseVisualStyleBackColor = false;
            this.c_2.Click += new System.EventHandler(this.c_2_Click);
            // 
            // c_3
            // 
            this.c_3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.c_3.BackColor = System.Drawing.Color.LightSteelBlue;
            this.c_3.Font = new System.Drawing.Font("Arial", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.c_3.Location = new System.Drawing.Point(13, 162);
            this.c_3.Margin = new System.Windows.Forms.Padding(0, 3, 0, 3);
            this.c_3.Name = "c_3";
            this.c_3.Size = new System.Drawing.Size(55, 20);
            this.c_3.TabIndex = 8;
            this.c_3.Text = "III";
            this.c_3.UseVisualStyleBackColor = false;
            this.c_3.Click += new System.EventHandler(this.c_3_Click);
            // 
            // c_4
            // 
            this.c_4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.c_4.BackColor = System.Drawing.Color.LightSteelBlue;
            this.c_4.Font = new System.Drawing.Font("Arial", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.c_4.Location = new System.Drawing.Point(13, 217);
            this.c_4.Margin = new System.Windows.Forms.Padding(0, 3, 0, 3);
            this.c_4.Name = "c_4";
            this.c_4.Size = new System.Drawing.Size(55, 18);
            this.c_4.TabIndex = 9;
            this.c_4.Text = "IV";
            this.c_4.UseVisualStyleBackColor = false;
            this.c_4.Click += new System.EventHandler(this.c_4_Click);
            // 
            // label12
            // 
            this.label12.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.label12.AutoSize = true;
            this.label12.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.label12.Location = new System.Drawing.Point(70, 197);
            this.label12.Margin = new System.Windows.Forms.Padding(2, 2, 0, 2);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(216, 59);
            this.label12.TabIndex = 16;
            this.label12.Text = resources.GetString("label12.Text");
            this.label12.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Arial", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(11, 20);
            this.label2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(290, 16);
            this.label2.TabIndex = 13;
            this.label2.Text = "DEFINIZIONE ACCELLERAZIONI SISMICHE";
            // 
            // b_ok
            // 
            this.b_ok.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.b_ok.Location = new System.Drawing.Point(642, 312);
            this.b_ok.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.b_ok.Name = "b_ok";
            this.b_ok.Size = new System.Drawing.Size(58, 21);
            this.b_ok.TabIndex = 5;
            this.b_ok.Text = "OK";
            this.b_ok.UseVisualStyleBackColor = false;
            this.b_ok.Click += new System.EventHandler(this.b_ok_Click);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Arial", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.Location = new System.Drawing.Point(390, 22);
            this.label8.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(201, 16);
            this.label8.TabIndex = 12;
            this.label8.Text = "DEFINIZIONE CLASSE D\'USO";
            // 
            // Form_Def_Acc
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(712, 354);
            this.Controls.Add(this.panel1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.MaximumSize = new System.Drawing.Size(728, 393);
            this.MinimumSize = new System.Drawing.Size(728, 393);
            this.Name = "Form_Def_Acc";
            this.Text = "Compilazione parametri sismici";
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox float_acc;
        private System.Windows.Forms.Button z4;
        private System.Windows.Forms.Button z3;
        private System.Windows.Forms.Button z2;
        private System.Windows.Forms.Button z1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Panel panel1;
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
        private System.Windows.Forms.Button b_ok;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button b_annulla;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Button c_1;
        private System.Windows.Forms.Button c_2;
        private System.Windows.Forms.Button c_3;
        private System.Windows.Forms.Button c_4;
        private System.Windows.Forms.Label label12;
    }
}