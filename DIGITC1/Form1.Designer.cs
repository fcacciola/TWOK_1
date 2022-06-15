namespace DIGITC1
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
      this.menuStrip1 = new System.Windows.Forms.MenuStrip();
      this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.samplesList = new System.Windows.Forms.ListBox();
      this.scriptsList = new System.Windows.Forms.ListBox();
      this.scriptBox = new System.Windows.Forms.TextBox();
      this.outputBox = new System.Windows.Forms.RichTextBox();
      this.button1 = new System.Windows.Forms.Button();
      this.button2 = new System.Windows.Forms.Button();
      this.button3 = new System.Windows.Forms.Button();
      this.renderListBox1 = new System.Windows.Forms.CheckedListBox();
      this.signalPlot1 = new NWaves.DemoForms.UserControls.SignalPlot();
      this.sessionNameBox = new System.Windows.Forms.TextBox();
      this.menuStrip1.SuspendLayout();
      this.SuspendLayout();
      // 
      // menuStrip1
      // 
      this.menuStrip1.GripMargin = new System.Windows.Forms.Padding(2, 2, 0, 2);
      this.menuStrip1.ImageScalingSize = new System.Drawing.Size(24, 24);
      this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem});
      this.menuStrip1.Location = new System.Drawing.Point(0, 0);
      this.menuStrip1.Name = "menuStrip1";
      this.menuStrip1.Size = new System.Drawing.Size(3690, 24);
      this.menuStrip1.TabIndex = 0;
      this.menuStrip1.Text = "menuStrip1";
      // 
      // fileToolStripMenuItem
      // 
      this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
      this.fileToolStripMenuItem.Size = new System.Drawing.Size(16, 20);
      // 
      // samplesList
      // 
      this.samplesList.FormattingEnabled = true;
      this.samplesList.ItemHeight = 20;
      this.samplesList.Location = new System.Drawing.Point(25, 51);
      this.samplesList.Name = "samplesList";
      this.samplesList.Size = new System.Drawing.Size(434, 304);
      this.samplesList.TabIndex = 5;
      this.samplesList.SelectedValueChanged += new System.EventHandler(this.samplesList_SelectedValueChanged);
      // 
      // scriptsList
      // 
      this.scriptsList.FormattingEnabled = true;
      this.scriptsList.ItemHeight = 20;
      this.scriptsList.Location = new System.Drawing.Point(25, 437);
      this.scriptsList.Name = "scriptsList";
      this.scriptsList.Size = new System.Drawing.Size(370, 1204);
      this.scriptsList.TabIndex = 6;
      this.scriptsList.SelectedIndexChanged += new System.EventHandler(this.scriptsList_SelectedIndexChanged);
      // 
      // scriptBox
      // 
      this.scriptBox.Font = new System.Drawing.Font("Courier New", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.scriptBox.Location = new System.Drawing.Point(401, 437);
      this.scriptBox.Multiline = true;
      this.scriptBox.Name = "scriptBox";
      this.scriptBox.Size = new System.Drawing.Size(1510, 1201);
      this.scriptBox.TabIndex = 7;
      // 
      // outputBox
      // 
      this.outputBox.BackColor = System.Drawing.SystemColors.ButtonHighlight;
      this.outputBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
      this.outputBox.Location = new System.Drawing.Point(1917, 437);
      this.outputBox.Name = "outputBox";
      this.outputBox.Size = new System.Drawing.Size(1761, 1201);
      this.outputBox.TabIndex = 8;
      this.outputBox.Text = "";
      // 
      // button1
      // 
      this.button1.Location = new System.Drawing.Point(25, 391);
      this.button1.Name = "button1";
      this.button1.Size = new System.Drawing.Size(116, 48);
      this.button1.TabIndex = 9;
      this.button1.Text = "Run";
      this.button1.UseVisualStyleBackColor = true;
      this.button1.Click += new System.EventHandler(this.button1_Click);
      // 
      // button2
      // 
      this.button2.Location = new System.Drawing.Point(299, 391);
      this.button2.Name = "button2";
      this.button2.Size = new System.Drawing.Size(136, 40);
      this.button2.TabIndex = 10;
      this.button2.Text = "Save";
      this.button2.UseVisualStyleBackColor = true;
      // 
      // button3
      // 
      this.button3.Location = new System.Drawing.Point(1917, 395);
      this.button3.Name = "button3";
      this.button3.Size = new System.Drawing.Size(116, 36);
      this.button3.TabIndex = 11;
      this.button3.Text = "Clear";
      this.button3.UseVisualStyleBackColor = true;
      this.button3.Click += new System.EventHandler(this.button3_Click);
      // 
      // renderListBox1
      // 
      this.renderListBox1.CheckOnClick = true;
      this.renderListBox1.FormattingEnabled = true;
      this.renderListBox1.Location = new System.Drawing.Point(465, 51);
      this.renderListBox1.Name = "renderListBox1";
      this.renderListBox1.Size = new System.Drawing.Size(263, 303);
      this.renderListBox1.TabIndex = 12;
      this.renderListBox1.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.renderListBox1_ItemCheck);
      this.renderListBox1.SelectedIndexChanged += new System.EventHandler(this.renderListBox1_SelectedIndexChanged);
      // 
      // signalPlot1
      // 
      this.signalPlot1.AutoScroll = true;
      this.signalPlot1.BackColor = System.Drawing.Color.White;
      this.signalPlot1.ForeColor = System.Drawing.Color.Blue;
      this.signalPlot1.Location = new System.Drawing.Point(734, 51);
      this.signalPlot1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
      this.signalPlot1.Name = "signalPlot1";
      this.signalPlot1.PaddingX = 30;
      this.signalPlot1.PaddingY = 6;
      this.signalPlot1.Size = new System.Drawing.Size(2944, 305);
      this.signalPlot1.SamplesPerPixel = 500;
      this.signalPlot1.TabIndex = 4;
      // 
      // sessionNameBox
      // 
      this.sessionNameBox.Location = new System.Drawing.Point(2049, 397);
      this.sessionNameBox.Name = "sessionNameBox";
      this.sessionNameBox.Size = new System.Drawing.Size(432, 26);
      this.sessionNameBox.TabIndex = 13;
      this.sessionNameBox.Text = "DEFAULT";
      // 
      // Form1
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(3690, 1650);
      this.Controls.Add(this.sessionNameBox);
      this.Controls.Add(this.renderListBox1);
      this.Controls.Add(this.button3);
      this.Controls.Add(this.button2);
      this.Controls.Add(this.button1);
      this.Controls.Add(this.outputBox);
      this.Controls.Add(this.scriptBox);
      this.Controls.Add(this.scriptsList);
      this.Controls.Add(this.samplesList);
      this.Controls.Add(this.signalPlot1);
      this.Controls.Add(this.menuStrip1);
      this.MainMenuStrip = this.menuStrip1;
      this.Name = "Form1";
      this.Text = "Form1";
      this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
      this.Load += new System.EventHandler(this.Form1_Load);
      this.menuStrip1.ResumeLayout(false);
      this.menuStrip1.PerformLayout();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.MenuStrip menuStrip1;
    private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
    public NWaves.DemoForms.UserControls.SignalPlot signalPlot1;
    public System.Windows.Forms.ListBox samplesList;
    public System.Windows.Forms.ListBox scriptsList;
    public System.Windows.Forms.TextBox scriptBox;
    public System.Windows.Forms.RichTextBox outputBox;
    private System.Windows.Forms.Button button1;
    private System.Windows.Forms.Button button2;
    private System.Windows.Forms.Button button3;
    public System.Windows.Forms.CheckedListBox renderListBox1;
    public System.Windows.Forms.TextBox sessionNameBox;
  }
}

