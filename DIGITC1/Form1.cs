using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DIGITC1
{
  public partial class Form1 : Form
  {
    public Form1()
    {
      InitializeComponent();
    }

    
    private void Form1_Load(object sender, EventArgs e)
    {
      Context.Create(this);

      Context.Params.SamplesFolder = @".\Input\Samples" ;
      Context.Params.ScriptsFolder = @".\Input\Scripts" ;

      LoadSamples();
      LoadScripts();

      Context.BaseScriptContents = File.ReadAllText(@".\Input\Scripts\BaseScript.cs");
    }

    void LoadSamples()
    {
      samplesList.BeginUpdate();
      foreach ( var lEntry in Directory.EnumerateFiles(Context.Params.SamplesFolder, "*.wav") )
      {
        samplesList.Items.Add( Path.GetFileName(lEntry) );
      }
      samplesList.EndUpdate();
    }

    void LoadScripts()
    {
      scriptsList.BeginUpdate();
      foreach ( var lEntry in Directory.EnumerateFiles(Context.Params.ScriptsFolder, "*.txt") )
      {
        scriptsList.Items.Add( Path.GetFileNameWithoutExtension(lEntry) );
      }
      scriptsList.EndUpdate();
    }

    public void ClearRenderModules()
    {
      renderListBox1.BeginUpdate();
      renderListBox1.Items.Clear();
      renderListBox1.Items.Add( Context.InputSignal.Name );  
      renderListBox1.SetItemChecked( renderListBox1.Items.Count - 1, true );
      renderListBox1.EndUpdate();
    }

    public void AddRenderModule( string aModule )
    {
      renderListBox1.BeginUpdate();
      renderListBox1.Items.Add( aModule );  
      renderListBox1.SetItemChecked( renderListBox1.Items.Count - 1, true );
      renderListBox1.EndUpdate();
    }

    public bool IsRenderModuleChecked( string aName )
    {
      int lIdx = -1 ;
      for ( int i = 0 ; i < renderListBox1.Items.Count ; i++ )
      {
        if ( renderListBox1.GetItemText(renderListBox1.Items[i]) == aName )
        {
          lIdx = i ; 
          break ;
        }
      }
      return lIdx != -1 ? renderListBox1.GetItemCheckState(lIdx) == System.Windows.Forms.CheckState.Checked : false ;
    }

    private void samplesList_SelectedValueChanged(object sender, EventArgs e)
    {
      if ( samplesList.SelectedItem != null )
      {
        Context.InputSample = $"{Context.Params.SamplesFolder}\\{samplesList.SelectedItem.ToString()}";
        outputBox.Text = "" ;
        LoadInput();
        Context.Pipeline.Clear(); 
      }
    }


    private void scriptsList_SelectedIndexChanged(object sender, EventArgs e)
    {
      if ( scriptsList.SelectedItem != null )
      {
        Context.ScriptFile = $"{Context.Params.ScriptsFolder}\\{scriptsList.SelectedItem.ToString()}.txt";

        if ( File.Exists(Context.ScriptFile) )
        {
          var lSC = File.ReadAllText(Context.ScriptFile);
          scriptBox.Text = lSC ;
          outputBox.Text = "" ;
          Context.Pipeline.Clear(); 
          Context.Log($"Script [{Path.GetFileNameWithoutExtension(Context.ScriptFile)}] loaded.");
        }
      }
    }

    private void button1_Click(object sender, EventArgs e)
    {
      outputBox.Text = "" ;
      ClearRenderModules();
      Start();
    }

    void LoadInput()
    {
      if ( File.Exists(Context.InputSample) )
      {
        var lASource = new FileAudioSource(Context.InputSample);

        var lISignal = lASource.CreateSignal();

        signalPlot1.Gain   = 100 ;
        signalPlot1.Stride = (int)Math.Ceiling(lISignal.Rep.Duration) * 10 ;

        lISignal.Name = "Input";

        Context.InputSignal = lISignal;

        ClearRenderModules();
        //AddRenderModule(Context.InputSignal.Name);

        Context.InputSignal.Render();

        Context.LogSessionStart(sessionNameBox.Text,"");
        Context.Log($"Sample [{Path.GetFileName(Context.InputSample)}] loaded.");
      }
    }

    void Start()
    {
      if ( Context.InputSignal != null )
      {
        Context.Log("Running Script..." + Environment.NewLine);
        Context.Pipeline.Start();
        Context.ScriptDriver.Run(Context.Pipeline, scriptBox.Text);
      }
    }

    private void button3_Click(object sender, EventArgs e)
    {
      outputBox.Text = "" ;
    }

    private void renderListBox1_ItemCheck(object sender, ItemCheckEventArgs e)
    {
    }

    private void renderListBox1_SelectedIndexChanged(object sender, EventArgs e)
    {
      signalPlot1.LayersChanged();
    }
  }
}
