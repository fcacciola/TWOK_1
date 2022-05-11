using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NWaves.Audio;
using NWaves.Operations;
using NWaves.Signals;

namespace DIGITC1
{
  public abstract class Signal : ICloneable
  {
    protected Signal()
    {
    }

    public override string ToString() => "";

    public void Render() 
    {
      if ( Context.RenderThisSegment(this) )
        DoRender();
    }

    public abstract void DoRender() ;

    public abstract List<Signal> Segment( float aWindowSizeInSeconds ) ;

    public abstract object Clone();

    public void Assign( Signal aRHS )
    {
      Idx                 = aRHS.Idx                 ;
      Name                = aRHS.Name                ;
      RenderFillColor     = aRHS.RenderFillColor     ;
      RenderLineColor     = aRHS.RenderLineColor     ;
      RenderLineThickness = aRHS.RenderLineThickness ;
      RenderTopLine       = aRHS.RenderTopLine       ;
      RenderBottomLine    = aRHS.RenderBottomLine    ;
      SegmentIdx          = aRHS.SegmentIdx          ;
    }

    public int    Idx                 = 0 ;
    public string Name                = "";
    public Color  RenderFillColor     = Color.Empty ;
    public Color  RenderLineColor     = Color.Empty ;
    public int    RenderLineThickness = 2 ;  
    public bool   RenderTopLine       = false ;
    public bool   RenderBottomLine    = false ;
    public int    SegmentIdx          = 0 ;

  }
  
  public class WaveSignal : Signal
  {
    public WaveSignal( DiscreteSignal aRep ) : base()
    { 
      Rep = aRep ;
    }

    public DiscreteSignal Rep ;
    
    public WaveSignal CopyWith( DiscreteSignal aDS )
    {
      WaveSignal rCopy = new WaveSignal(aDS);
      rCopy.Assign(this); 
      return rCopy ;
    }

    public WaveSignal Copy() => CopyWith(Rep.Copy()); 

    public override object Clone() => Copy();

    public override string ToString()
    {
      return $"Duiration:{Rep.Duration} SampleRate:{Rep.SamplingRate}{Environment.NewLine}Samples:[{Utils.ToStr(Rep.Samples)}]";
    }

    public override void DoRender()
    {
      Context.RenderWaveForm(Rep, Idx, Name, RenderFillColor, RenderLineColor, RenderLineThickness, RenderTopLine, RenderBottomLine);
    }

    public float ComputeMax() => Rep.Samples.Max();

    public WaveSignal Transform( Func<float,float> Transformation ) 
    {
      float[] lTransformedSamples = new float[Rep.Samples.Length];
      for (int i = 0; i < Rep.Samples.Length; i++)  
        lTransformedSamples[i] = Transformation(Rep.Samples[i]);
      return CopyWith( new DiscreteSignal(Rep.SamplingRate, lTransformedSamples) );
    }

    public override List<Signal> Segment( float aWindowSizeInSeconds ) 
    {
      List<Signal> rList = new List<Signal> ();

      if ( aWindowSizeInSeconds > 0 )
      {
        int lOriginalLength = Rep.Samples.Length;
        int lSegmentLength  = (int)(Rep.SamplingRate * aWindowSizeInSeconds);

        int k = 0 ;
        do
        {
          float[] lSegmentSamples = new float[lSegmentLength];  
          for (int i = 0; i < lSegmentLength && k < lOriginalLength ; i++, k++)
            lSegmentSamples[i] = Rep.Samples[k];  

          var lSignal = CopyWith( new DiscreteSignal(Rep.SamplingRate,lSegmentSamples) );

          lSignal.SegmentIdx = rList.Count; 

          rList.Add (lSignal);
        }
        while ( k < lOriginalLength );  
      }
      else
      {
        rList.Add(this);
      }

      return rList;
    }
  }

}
