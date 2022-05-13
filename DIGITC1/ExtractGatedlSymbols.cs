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

  public class ExtractGatedlSymbols : AudioInputModule
  {
    public ExtractGatedlSymbols( float aMinDuration, float aMergeGap ) : base() { mMinDuration = aMinDuration ; mMergeGap = aMergeGap ; }

     protected override Signal ProcessAudioSignal ( int aSegmentIdx,  int aStep, WaveSignal aInput )
    {
      mInput      = aInput ; 
      mAllSymbols = new List<GatedSymbol>();

      mCurrCount = 0; 
      mPos       = 0 ;
      foreach( float lV in aInput.Samples)
      {
        if( mCurrCount == 0 || mCurrLevel != lV ) 
        {
          AddSymbol();

          mCurrLevel = lV;
          mCurrCount = 1 ;
        }
        else
        { 
          mCurrCount ++ ;
        }

        mPos ++ ;
      }

      AddSymbol();

      MergeSymbols();

      RemoveShortSymbols();

      GatedLexicalSignal rSignal = new GatedLexicalSignal(mFinal);
      //GatedLexicalSignal rSignal = new GatedLexicalSignal(mAllSymbols);

      rSignal.Idx  = aStep + 1 ;
      rSignal.Name = "GatedSymbols";

      WaveSignal lView = rSignal.ConvertToWave();

      lView.RenderFillColor  = Color.Empty ;
      lView.RenderLineColor  = Color.FromArgb(255, Color.Black) ;
      lView.RenderTopLine    = true ;  
      lView.RenderBottomLine = false ; 

      if ( aSegmentIdx == 0 ) 
      {
       Context.Form.AddRenderModule(rSignal.Name);
       lView.Render();
      }

      Context.Log(aSegmentIdx==0,$"GatedSymbols View:{lView}");
      Context.Log(aSegmentIdx==0,$"GatedSymbols:{rSignal}");

      return rSignal ;
    }

    void AddSymbol()
    {
      if ( mCurrCount > 0 )
        mAllSymbols.Add( new GatedSymbol(mAllSymbols.Count, mCurrLevel, mInput.SamplingRate, mPos - mCurrCount, mCurrCount ) );
    }

    void MergeSymbols()
    {
      mMerged = new List<GatedSymbol>();

      List<int> lSeparatorGapPositions = new List<int>();

      for( int i = 0; i < mAllSymbols.Count; i++ )  
      {
        GatedSymbol lSymbol = mAllSymbols[i];

        if ( lSymbol.IsGap && lSymbol.Duration >= mMergeGap )
          lSeparatorGapPositions.Add( i );
      }

      int lMergeStart = 0 ;
      foreach( int j in lSeparatorGapPositions ) 
      {
        MergeSymbols(lMergeStart,j);
        lMergeStart = j + 1 ;
      }
    }


    void MergeSymbols(int aBegin, int aEnd )
    {
      if ( aBegin < aEnd )
      { 
        int lTrimmedBegin = aBegin ;
        int lTrimmedEnd   = aEnd   ;

        while ( lTrimmedBegin < aEnd && mAllSymbols[lTrimmedBegin].IsGap )
          lTrimmedBegin++;

        while ( lTrimmedEnd > aBegin && mAllSymbols[lTrimmedEnd-1].IsGap )
          lTrimmedEnd--;

        int lMergedLen = 0 ;

        for ( int i = lTrimmedBegin ; i < lTrimmedEnd ; i++ ) 
         lMergedLen += mAllSymbols[i].Length ; 

         mMerged.Add( new GatedSymbol(mMerged.Count, mAllSymbols[lTrimmedBegin].Amplitude, mInput.SamplingRate, mAllSymbols[lTrimmedBegin].Pos, lMergedLen));
      }
    }

    void RemoveShortSymbols()
    {
      mFinal = new List<GatedSymbol> ();
      mMerged.ForEach( s => { if ( s.Duration >= mMinDuration ) mFinal.Add(s); } );
    }

    double            mMinDuration ;
    double            mMergeGap ;
    WaveSignal        mInput ;
                                                                                                                                                                               
    int               mPos ;
    float             mCurrLevel ;
    int               mCurrCount ;
    List<GatedSymbol> mAllSymbols ;
    List<GatedSymbol> mMerged ;
    List<GatedSymbol> mFinal ;
  }
}
