using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace WindowsFormsApp1
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
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.numLabel = new System.Windows.Forms.Label();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.retrieve = new System.Windows.Forms.Button();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.Column1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column4 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clearance = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Picture = new System.Windows.Forms.DataGridViewLinkColumn();
            this.Folder = new System.Windows.Forms.Button();
            this.parsetable = new System.Windows.Forms.Button();
            this.dataGridView2 = new System.Windows.Forms.DataGridView();
            this.Meat = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Image = new System.Windows.Forms.DataGridViewLinkColumn();
            this.loadsaved = new System.Windows.Forms.Button();
            this.DateLabel = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView2)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(66, 16);
            this.label1.TabIndex = 0;
            this.label1.Text = "Input Link:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(12, 42);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(47, 16);
            this.label2.TabIndex = 1;
            this.label2.Text = "Dates:";
            // 
            // numLabel
            // 
            this.numLabel.AutoSize = true;
            this.numLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.numLabel.Location = new System.Drawing.Point(219, 42);
            this.numLabel.Name = "numLabel";
            this.numLabel.Size = new System.Drawing.Size(139, 16);
            this.numLabel.TabIndex = 2;
            this.numLabel.Text = "Number of Sale Items:";
            // 
            // textBox1
            // 
            this.textBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox1.Location = new System.Drawing.Point(78, 6);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(629, 22);
            this.textBox1.TabIndex = 3;
            // 
            // retrieve
            // 
            this.retrieve.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.retrieve.Location = new System.Drawing.Point(713, 6);
            this.retrieve.Name = "retrieve";
            this.retrieve.Size = new System.Drawing.Size(84, 23);
            this.retrieve.TabIndex = 4;
            this.retrieve.Text = "Retrieve";
            this.retrieve.UseVisualStyleBackColor = true;
            this.retrieve.Click += new System.EventHandler(this.retrieve_Click);
            // 
            // dataGridView1
            // 
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Column1,
            this.Column2,
            this.Column3,
            this.Column4,
            this.clearance,
            this.Picture});
            this.dataGridView1.Location = new System.Drawing.Point(2, 61);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.Size = new System.Drawing.Size(915, 407);
            this.dataGridView1.TabIndex = 5;
            this.dataGridView1.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_CellContentClick);
            this.dataGridView1.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_CellValueChanged);
            // 
            // Column1
            // 
            this.Column1.HeaderText = "Name";
            this.Column1.Name = "Column1";
            // 
            // Column2
            // 
            this.Column2.HeaderText = "Original Price";
            this.Column2.Name = "Column2";
            // 
            // Column3
            // 
            this.Column3.HeaderText = "Sale Price";
            this.Column3.Name = "Column3";
            // 
            // Column4
            // 
            this.Column4.HeaderText = "Discount %";
            this.Column4.Name = "Column4";
            // 
            // clearance
            // 
            this.clearance.HeaderText = "Clearance";
            this.clearance.Name = "clearance";
            // 
            // Picture
            // 
            this.Picture.HeaderText = "Picture";
            this.Picture.Name = "Picture";
            // 
            // Folder
            // 
            this.Folder.Location = new System.Drawing.Point(713, 32);
            this.Folder.Name = "Folder";
            this.Folder.Size = new System.Drawing.Size(84, 27);
            this.Folder.TabIndex = 6;
            this.Folder.Text = "Open Folder";
            this.Folder.UseVisualStyleBackColor = true;
            this.Folder.Click += new System.EventHandler(this.Folder_Click);
            // 
            // parsetable
            // 
            this.parsetable.Location = new System.Drawing.Point(803, 6);
            this.parsetable.Name = "parsetable";
            this.parsetable.Size = new System.Drawing.Size(104, 24);
            this.parsetable.TabIndex = 7;
            this.parsetable.Text = "Populate Table";
            this.parsetable.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.parsetable.UseVisualStyleBackColor = true;
            this.parsetable.Click += new System.EventHandler(this.parsetable_Click);
            // 
            // dataGridView2
            // 
            this.dataGridView2.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView2.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Meat,
            this.Image});
            this.dataGridView2.Location = new System.Drawing.Point(2, 474);
            this.dataGridView2.Name = "dataGridView2";
            this.dataGridView2.Size = new System.Drawing.Size(915, 212);
            this.dataGridView2.TabIndex = 8;
            // 
            // Meat
            // 
            this.Meat.HeaderText = "Meat";
            this.Meat.Name = "Meat";
            this.Meat.ReadOnly = true;
            this.Meat.Width = 800;
            // 
            // Image
            // 
            this.Image.HeaderText = "Image";
            this.Image.Name = "Image";
            // 
            // loadsaved
            // 
            this.loadsaved.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F);
            this.loadsaved.Location = new System.Drawing.Point(803, 32);
            this.loadsaved.Name = "loadsaved";
            this.loadsaved.Size = new System.Drawing.Size(104, 27);
            this.loadsaved.TabIndex = 10;
            this.loadsaved.Text = "Load Saved Table";
            this.loadsaved.UseVisualStyleBackColor = true;
            this.loadsaved.Click += new System.EventHandler(this.loadsaved_Click);
            // 
            // DateLabel
            // 
            this.DateLabel.AutoSize = true;
            this.DateLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.DateLabel.Location = new System.Drawing.Point(440, 41);
            this.DateLabel.Name = "DateLabel";
            this.DateLabel.Size = new System.Drawing.Size(39, 15);
            this.DateLabel.TabIndex = 11;
            this.DateLabel.Text = "Date: ";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(919, 688);
            this.Controls.Add(this.DateLabel);
            this.Controls.Add(this.loadsaved);
            this.Controls.Add(this.dataGridView2);
            this.Controls.Add(this.parsetable);
            this.Controls.Add(this.Folder);
            this.Controls.Add(this.dataGridView1);
            this.Controls.Add(this.retrieve);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.numLabel);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView2)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        //only works upon extracting all images. would not work if you start at node 10 to 20 for example. 
        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dataGridView1.CurrentCell.ColumnIndex.Equals(5) && e.RowIndex != -1)
            {
                if (dataGridView1.CurrentCell != null && dataGridView1.CurrentCell.Value != null)
                {
                    string path = dataGridView1.CurrentCell.Value.ToString();
                    Process.Start(path);
                    Console.WriteLine(path.ToString());
                }
            }

        }

        private void dataGridView2_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dataGridView2.CurrentCell.ColumnIndex.Equals(1) && e.RowIndex != -1)
            {
                if (dataGridView1.CurrentCell != null && dataGridView1.CurrentCell.Value != null)
                {
                    string row = (dataGridView2.CurrentCell.RowIndex + 1).ToString();
                    string path = "C:\\Users\\Judy\\source\\repos\\judy-chen\\CostcoHelper\\WindowsFormsApp1\\bin\\Debug\\" + row + ".png";
                    Process.Start(path);
                    Console.WriteLine(path.ToString());

                }
            }

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label numLabel;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Button retrieve;
        private System.Windows.Forms.DataGridView dataGridView1;
        private Button Folder;
        private Button parsetable;
        private DataGridView dataGridView2;
        private DataGridViewTextBoxColumn Meat;
        private DataGridViewLinkColumn Image;
        private DataGridViewTextBoxColumn Column1;
        private DataGridViewTextBoxColumn Column2;
        private DataGridViewTextBoxColumn Column3;
        private DataGridViewTextBoxColumn Column4;
        private DataGridViewTextBoxColumn clearance;
        private DataGridViewLinkColumn Picture;
        private Button loadsaved;
        private Label DateLabel;
    }
}

