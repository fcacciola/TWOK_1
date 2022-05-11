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

    private void samplesList_SelectedValueChanged(object sender, EventArgs e)
    {
      Context.InputSample = $"{Context.Params.SamplesFolder}\\{samplesList.SelectedItem.ToString()}";
      LoadInput();
    }


    private void scriptsList_SelectedIndexChanged(object sender, EventArgs e)
    {
      Context.ScriptFile = $"{Context.Params.ScriptsFolder}\\{scriptsList.SelectedItem.ToString()}.txt";

      if ( File.Exists(Context.ScriptFile) )
      {
        var lSC = File.ReadAllText(Context.ScriptFile);
        scriptBox.Text = lSC ;
      }
    }

    private void button1_Click(object sender, EventArgs e)
    {
      Start();
    }

    void LoadInput()
    {
      if ( File.Exists(Context.InputSample) )
      {
        var lASource = new FileAudioSource(Context.InputSample);
        Context.InputSignal = lASource.CreateSignal();
        Context.InputSignal.Render();
      }
    }

    void Start()
    {
      Context.ScriptDriver.Run(Context.ScriptFile);
    }
  }
}
