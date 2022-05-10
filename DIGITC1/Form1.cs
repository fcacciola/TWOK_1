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

      Context.Params.SamplesFolder = @"C:\Users\User\Dropbox\ECB\ITC\Digital ITC\Samples" ;
      Context.Params.ScriptsFolder = @"C:\Users\User\Dropbox\ECB\ITC\Digital ITC\Scripts" ;

      LoadSamples();
      LoadScripts();
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
      foreach ( var lEntry in Directory.EnumerateFiles(Context.Params.ScriptsFolder, "*.dcs") )
      {
        scriptsList.Items.Add( Path.GetFileNameWithoutExtension(lEntry) );
      }
      scriptsList.EndUpdate();
    }

    private void samplesList_SelectedValueChanged(object sender, EventArgs e)
    {
      Context.Params.InputSample = $"{Context.Params.SamplesFolder}\\{samplesList.SelectedItem.ToString()}";
      LoadInput();
    }


    private void scriptsList_SelectedIndexChanged(object sender, EventArgs e)
    {
      Context.Params.ScriptFile = $"{Context.Params.ScriptsFolder}\\{scriptsList.SelectedItem.ToString()}.dcs";

      if ( File.Exists(Context.Params.ScriptFile) )
      {
        scriptBox.Text = File.ReadAllText(Context.Params.ScriptFile);
      }


    }

    private void button1_Click(object sender, EventArgs e)
    {
      Start();
    }

    void LoadInput()
    {
      if ( File.Exists(Context.Params.InputSample) )
      {
        var lASource = new FileAudioSource(Context.Params.InputSample);
        Context.Params.InputSignal = lASource.CreateSignal();
        Context.Params.InputSignal.Render();
      }
    }

    void Start()
    {
    }
  }
}
