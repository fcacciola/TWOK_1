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
  using BitsSignal  = GenericLexicalSignal<BitSymbol>;
  using BytesSignal = GenericLexicalSignal<ByteSymbol>;

  public class BinaryToBytes : LexicalModule
  {
    public BinaryToBytes( int aBitsPerByte, bool aLittleEndian = true ) : base() { mLittleEndian = aLittleEndian ; mBitsPerByte = aBitsPerByte ; }

     public override ModuleSignature GetSignature() { return new ModuleSignature( GetType().Name, mLittleEndian, mBitsPerByte); }

     protected override Signal ProcessLexicalSignal ( int aSegmentIdx, int aStep, LexicalSignal aInput )
     {
       BitsSignal lBinaryInput = aInput as BitsSignal ;
       if ( lBinaryInput == null )
         throw new ArgumentException("Input Signal must be a Binary Signal");

      mBitValues  = new List<bool>(); 

      int lLen = lBinaryInput.Symbols.Count ;
      int lByteCount = 0; 
      int i = 0;


      do
      { 
        mDEBUG_ByteString = "" ;

        int lRem = lLen - i ;

        if ( mLittleEndian )
          AddPadding();

        for ( int j = 0 ; i < lLen && j < mBitsPerByte ; j++, i ++ )
        {
          mDEBUG_ByteString += $"[{(lBinaryInput.Symbols[i].One ? "1" : "0")}]";
          mBitValues.Add( lBinaryInput.Symbols[i].One ) ;  
        }
        
        AddRemainder(lRem);

        if ( !mLittleEndian )
          AddPadding();

        Context.Log(aSegmentIdx==0,mDEBUG_ByteString+Environment.NewLine);

        lByteCount ++ ;
      }
      while ( i < lLen ) ;

      BitArray lBits = new BitArray(mBitValues.ToArray());
        
      byte[] lBytes = new byte[lByteCount]; 
      lBits.CopyTo( lBytes, 0 ) ;

      List<ByteSymbol> lByteSymbols = new List<ByteSymbol>();

      foreach( byte lByte in lBytes )
        lByteSymbols.Add( new ByteSymbol(lByteSymbols.Count, lByte ) ) ;

      mResult = new BytesSignal(lByteSymbols);

      mResult.Idx  = aStep + 1 ;
      mResult.Name = "Bytes";

      return mResult ;
    }

    public override void ShowResult ( int aSegmentIdx, int aStep )
    {
      Context.Log(aSegmentIdx==0, $"Bytes:{mResult}");
    }

    void AddPadding()
    {
      if (  mBitsPerByte < 8 )
      {
        for( int k = mBitsPerByte ; k < 8 ; k++ ) 
        {
          mDEBUG_ByteString += $"[P:0]";
          mBitValues.Add( false ) ;
        }

      }
    }

    void AddRemainder( int aRem )
    {
      if ( aRem < mBitsPerByte )
      {
        for( int k = aRem ; k < mBitsPerByte ; k++ ) 
        {
          mDEBUG_ByteString += $"[R:0]";
          mBitValues.Add( false ) ;
        }
      }
    }

    bool mLittleEndian ;
    int  mBitsPerByte ;

    List<bool> mBitValues ;

    string mDEBUG_ByteString ;

  }
}
