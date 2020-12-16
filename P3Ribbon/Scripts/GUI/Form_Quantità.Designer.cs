
namespace P3Ribbon.Scripts.Form
{
    partial class Form_Quantità
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form_Quantità));
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.AbacoQuantità = new System.Windows.Forms.DataGridView();
            this.butt_DettagliQuantità = new System.Windows.Forms.Button();
            this.Materiale = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Kg = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.AbacoQuantità)).BeginInit();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Controls.Add(this.AbacoQuantità, 0, 0);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(10, 5);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(499, 393);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // AbacoQuantità
            // 
            this.AbacoQuantità.AllowUserToAddRows = false;
            this.AbacoQuantità.AllowUserToDeleteRows = false;
            this.AbacoQuantità.AllowUserToOrderColumns = true;
            this.AbacoQuantità.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            this.AbacoQuantità.BackgroundColor = System.Drawing.SystemColors.ButtonHighlight;
            this.AbacoQuantità.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            this.AbacoQuantità.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.AbacoQuantità.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Materiale,
            this.Kg});
            this.AbacoQuantità.Dock = System.Windows.Forms.DockStyle.Fill;
            this.AbacoQuantità.Location = new System.Drawing.Point(3, 3);
            this.AbacoQuantità.Name = "AbacoQuantità";
            this.AbacoQuantità.ReadOnly = true;
            this.AbacoQuantità.Size = new System.Drawing.Size(493, 387);
            this.AbacoQuantità.TabIndex = 0;
            // 
            // butt_DettagliQuantità
            // 
            this.butt_DettagliQuantità.Location = new System.Drawing.Point(336, 404);
            this.butt_DettagliQuantità.Name = "butt_DettagliQuantità";
            this.butt_DettagliQuantità.Size = new System.Drawing.Size(168, 25);
            this.butt_DettagliQuantità.TabIndex = 1;
            this.butt_DettagliQuantità.Text = "Dettagli";
            this.butt_DettagliQuantità.UseVisualStyleBackColor = true;
            this.butt_DettagliQuantità.Click += new System.EventHandler(this.butt_DettagliQuantità_Click);
            // 
            // Materiale
            // 
            this.Materiale.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.Materiale.DefaultCellStyle = dataGridViewCellStyle1;
            this.Materiale.HeaderText = "Materiale";
            this.Materiale.Name = "Materiale";
            this.Materiale.ReadOnly = true;
            // 
            // Kg
            // 
            this.Kg.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.Kg.HeaderText = "Tot Kg";
            this.Kg.Name = "Kg";
            this.Kg.ReadOnly = true;
            // 
            // Form_Quantità
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(516, 441);
            this.Controls.Add(this.butt_DettagliQuantità);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximumSize = new System.Drawing.Size(532, 480);
            this.MinimumSize = new System.Drawing.Size(532, 480);
            this.Name = "Form_Quantità";
            this.Text = "Form_Quantità";
            this.Load += new System.EventHandler(this.Form_Quantità_Load);
            this.tableLayoutPanel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.AbacoQuantità)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.DataGridView AbacoQuantità;
        private System.Windows.Forms.Button butt_DettagliQuantità;
        private System.Windows.Forms.DataGridViewTextBoxColumn Materiale;
        private System.Windows.Forms.DataGridViewTextBoxColumn Kg;
    }
}