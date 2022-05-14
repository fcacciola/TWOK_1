using System;
using System.Collections;
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
  using BinarySignal = GenericLexicalSignal<BitSymbol>;
  using BytesSignal = GenericLexicalSignal<ByteSymbol>;
  using TextSignal = GenericLexicalSignal<TextSymbol>;

  public abstract class Signal : ICloneable
  {
    protected Signal()
    {
    }

    public override string ToString() => "";

    public void Render() 
    {
      if ( Context.ShouldRender(Name) )
        DoRender();
    }

    public abstract void DoRender() ;

    public virtual List<Signal> Segment( float aWindowSizeInSeconds ) { return new List<Signal>(){this} ; }

    public static Signal Merge( List<Signal> aSegments )
    {
      if ( aSegments.Count == 0 ) return null;

      switch ( aSegments[0] )
      {
        case WaveSignal         lS : return WaveSignal        .Merge(aSegments.Cast<WaveSignal        >().ToList());
        case GatedLexicalSignal lS : return GatedLexicalSignal.Merge(aSegments.Cast<GatedLexicalSignal>().ToList());
        case BinarySignal       lS : return BinarySignal      .Merge(aSegments.Cast<BinarySignal      >().ToList());
        case TextSignal         lS : return TextSignal        .Merge(aSegments.Cast<TextSignal        >().ToList());
      }

      return null ;
    }

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
    
    public double  Duration     => Rep.Duration ;
    public int     SamplingRate => Rep.SamplingRate ;
    public float[] Samples      => Rep.Samples ; 

    public WaveSignal CopyWith( DiscreteSignal aDS )
    {
      WaveSignal rCopy = new WaveSignal(aDS);
      rCopy.Assign(this); 
      return rCopy ;
    }

    public WaveSignal Copy() => CopyWith(Rep.Copy()); 

    public static WaveSignal Merge( List<WaveSignal> aSegments )
    { 
      if ( aSegments.Count == 0 ) return null ; 

      List<float> lAllSamples = new List<float> ();
      foreach( WaveSignal aSegment in aSegments ) 
        lAllSamples.AddRange(aSegment.Samples);

      return new WaveSignal ( new DiscreteSignal(aSegments[0].SamplingRate, lAllSamples) ); 
    }

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
      float[] lTransformedSamples = new float[Samples.Length];
      for (int i = 0; i < Samples.Length; i++)  
        lTransformedSamples[i] = Transformation(Samples[i]);
      return CopyWith( new DiscreteSignal(SamplingRate, lTransformedSamples) );
    }

    public override List<Signal> Segment( float aWindowSizeInSeconds ) 
    {
      List<Signal> rList = new List<Signal> ();

      if ( aWindowSizeInSeconds > 0 )
      {
        int lOriginalLength = Samples.Length;
        int lSegmentLength  = (int)(SamplingRate * aWindowSizeInSeconds);

        int k = 0 ;
        do
        {
          float[] lSegmentSamples = new float[lSegmentLength];  
          for (int i = 0; i < lSegmentLength && k < lOriginalLength ; i++, k++)
            lSegmentSamples[i] = Samples[k];  

          var lSignal = CopyWith( new DiscreteSignal(SamplingRate,lSegmentSamples) );

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

  public abstract class Symbol : ICloneable
  {
    public Symbol( int aIdx ) { Idx = aIdx ; }

    public abstract object Clone() ;  

    public int Idx ;

    public virtual string Meaning => ToString();
  }

  public class GatedSymbol : Symbol
  {
    public GatedSymbol( int aIdx, float aAmplitud, int aSamplingRate, int aPos, int aLength ) : base(aIdx)
    {
      Amplitude    = aAmplitud;
      SamplingRate = aSamplingRate; 
      Pos          = aPos; 
      Length       = aLength;
    }

    public double Duration => (double)Length / (double)SamplingRate;

//    public override string ToString() => $"[{Duration:F2} s {(IsGap ? "Gap" : "Sym")} ({Length}|{Pos})]{Environment.NewLine}" ;

    public override string ToString() => $"[{Duration:F2} at {(double)Pos/(double)SamplingRate:F2})]{Environment.NewLine}" ;

    public override object Clone() {  return new GatedSymbol( Idx, Amplitude, SamplingRate, Pos, Length ); }  

    public bool IsGap => Amplitude == 0 ;

    public void DumpSamples( List<float> aSamples )
    {
      int lC = aSamples.Count ;
      for( int i = lC ; i < Pos ; i++ )
        aSamples.Add(0);

      for( int i = 0; i < Length; i++ ) 
        aSamples.Add(Amplitude);
    }

    public float Amplitude ;
    public int   SamplingRate ;
    public int   Pos ;
    public int   Length ; 
  }

  public class BitSymbol : Symbol
  {
    public BitSymbol( int aIdx, bool aOne, GatedSymbol aView ) : base(aIdx) { One = aOne ; View = aView ; }

    public override object Clone() { return new BitSymbol( Idx, One, View?.Clone()  as GatedSymbol ); }  

    public override string ToString() => One ? "1" : "0" ;

    public bool One ;

    public GatedSymbol View ;
  }

  public class ByteSymbol : Symbol
  {
    public ByteSymbol( int aIdx, byte aByte ) : base(aIdx) { Byte = aByte ; }

    public override object Clone() { return new ByteSymbol( Idx, Byte ); }  

    public override string ToString() => $"[{Byte.ToString():x}]" ;

    public byte Byte ;
  }

  public class TextSymbol : Symbol
  {
    public TextSymbol( int aIdx, string aText ) : base(aIdx) { Text = aText ; }

    public override object Clone() { return new TextSymbol( Idx, Text ); }  

    public override string ToString() => Text ;

    public string Text ;
  }

  public abstract class LexicalSignal : Signal
  {
    public override string ToString()
    {
      List<string> lAll = new List<string>();

      foreach( Symbol lSymbol in EnumSymbols )
        lAll.Add(lSymbol.ToString() );  

      return String.Join( "", lAll );
    }

    public override void DoRender() {}

    public abstract IEnumerable<Symbol> EnumSymbols { get ; }
  }

  public class GatedLexicalSignal : LexicalSignal
  {
    public GatedLexicalSignal( IEnumerable<GatedSymbol> aSymbols )
    {
      Symbols.AddRange(aSymbols);
    }

    public override object Clone()
    {
      return new GatedLexicalSignal( Symbols.ConvertAll( s => s.Clone() as GatedSymbol ) ) ;
    }

    public static GatedLexicalSignal Merge( List<GatedLexicalSignal> aSegments )
    { 
      if ( aSegments.Count == 0 ) return null ; 

      List<GatedSymbol> lAllSymbos = new List<GatedSymbol> ();
      foreach( GatedLexicalSignal aSegment in aSegments ) 
        lAllSymbos.AddRange(aSegment.Symbols);

      return new GatedLexicalSignal (lAllSymbos ); 
    }

    public WaveSignal ConvertToWave()
    { 
      List<float> lSamples = new List<float>();  
      foreach( GatedSymbol lSymbol in Symbols ) 
        lSymbol.DumpSamples(lSamples);
     
      var rSignal = new WaveSignal( new DiscreteSignal( Symbols[0].SamplingRate, lSamples.ToArray()) ) ;

      rSignal.Idx  = Idx ;
      rSignal.Name = Name ;

      return rSignal ;
    }

    public override IEnumerable<Symbol> EnumSymbols => Symbols ;

    public List<GatedSymbol> Symbols = new List<GatedSymbol>();
  }

  public class GenericLexicalSignal<SYM> : LexicalSignal
  {
    public GenericLexicalSignal( IEnumerable<SYM> aSymbols )
    {
      Symbols.AddRange(aSymbols);
    }

    public override object Clone()
    {
      return null ; //new GenericLexicalSignal<SYM>( Symbols.ConvertAll( s => s.Clone() as SYM ) ) ;
    }

    public static GenericLexicalSignal<SYM> Merge( List<GenericLexicalSignal<SYM>> aSegments )
    { 
      if ( aSegments.Count == 0 ) return null ; 

      List<SYM> lAllSymbos = new List<SYM> ();
      foreach( GenericLexicalSignal<SYM> aSegment in aSegments ) 
        lAllSymbos.AddRange(aSegment.Symbols);

      return new GenericLexicalSignal<SYM>(lAllSymbos ); 
    }

    public override IEnumerable<Symbol> EnumSymbols => Symbols.Cast<Symbol>() ;

    public List<SYM> Symbols = new List<SYM>();

  }
}
