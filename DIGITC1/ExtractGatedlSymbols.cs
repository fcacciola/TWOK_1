﻿using System;
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

    public override ModuleSignature GetSignature() { return new ModuleSignature( GetType().Name, mMinDuration, mMergeGap ); }

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

      mResult = new GatedLexicalSignal(mFinal);

      mResult.Idx  = aStep + 1 ;
      mResult.Name = "GatedSymbols";

      if ( mFinal.Count > 0 ) 
      {
        mView = (mResult as GatedLexicalSignal).ConvertToWave();

        mView.RenderFillColor  = Color.Empty ;
        mView.RenderLineColor  = Color.FromArgb(255, Color.Black) ;
        mView.RenderTopLine    = true ;  
        mView.RenderBottomLine = false ; 
 
      }

      return mResult ;
    }

    public override void ShowResult ( int aSegmentIdx,  int aStep )
    {
      if ( mView != null ) 
      {
        if ( aSegmentIdx == 0 ) 
        {
         Context.Form.AddRenderModule(mResult.Name);
         mView.Render();
        }
      }

      Context.Log(aSegmentIdx==0,$"GatedSymbols:{mResult}");
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

    WaveSignal mView ;
  }
}
