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
  using BitsSignal = GenericLexicalSignal<BitSymbol>;

  public class BinarizeByDuration : LexicalModule
  {
    public BinarizeByDuration( double aThreshold ) : base() { mThreshold = aThreshold ; }

     public override ModuleSignature GetSignature() { return new ModuleSignature( GetType().Name, mThreshold); }

     protected override Signal ProcessLexicalSignal ( int aSegmentIdx, int aStep, LexicalSignal aInput )
     {
       GatedLexicalSignal lGatedInput = aInput as GatedLexicalSignal ;
       if ( lGatedInput == null )
         throw new ArgumentException("Input Signal must be a Gated Lexical Signal");

       List<BitSymbol>   lBits     = new List<BitSymbol>   ();
       List<GatedSymbol> lBitViews = new List<GatedSymbol> ();
             
       double lAccDuration = 0 ;
       lGatedInput.Symbols.ForEach( s => lAccDuration += s.Duration ) ;
       double lAvgDuration = lAccDuration / (double)lGatedInput.Symbols.Count ;

       double lMaxDuration = 0 ;
       lGatedInput.Symbols.ForEach( s => { if ( s.Duration < 3 * lAvgDuration ) lMaxDuration = Math.Max(s.Duration, lMaxDuration) ; } ) ;

       foreach ( GatedSymbol lGI in lGatedInput.Symbols ) 
       {
         bool lOne = ( lGI.Duration / lMaxDuration ) > mThreshold ;

         GatedSymbol lViewSym = lGI.Clone() as GatedSymbol ;
         lViewSym.Amplitude = lOne ? - lGI.Amplitude * 0.5f : - lGI.Amplitude * 0.2f;
         lBitViews.Add( lViewSym ) ; 

         lBits.Add( new BitSymbol( lBits.Count, lOne, lViewSym )) ;

       }
   
       mResult = new BitsSignal(lBits);

       mResult.Idx  = aStep + 1 ;
       mResult.Name = "DurationBits";

       mViewGS = new GatedLexicalSignal(lBitViews) ;
       mView   = mViewGS.ConvertToWave();
 
       mView.Idx              = aStep + 1 ;
       mView.Name             = "DurationBits";
       mView.RenderFillColor  = Color.Empty ;
       mView.RenderLineColor  = Color.BlueViolet ;
       mView.RenderTopLine    = false ;  
       mView.RenderBottomLine = true ;  

       return mResult ;
     }

     public override void ShowResult ( int aSegmentIdx, int aStep )
     {
       if ( aSegmentIdx == 0 ) 
       {
         Context.Form.AddRenderModule(mView.Name);
         mView.Render();
       }
 
       //Context.Log(aSegmentIdx==0,$"Duration-based Bits View:{lView}");
       Context.Log($"Duration-based Bits:{mResult}");
    }

    double mThreshold ;

    GatedLexicalSignal mViewGS ;
    WaveSignal         mView;

  }

}
