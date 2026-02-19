using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4Common;

namespace SharpISOBMFF
{
/*
class DefaultHevcExtractorConstructorBox extends FullBox('dhec'){
	unsigned int(32) num_entries;
	for (i=1; i<= num_entries; i++) { 
		unsigned int(8) constructor_type;
		unsigned int(8) constructor_flags; 
		if( constructor_type == 0 ) 
			SampleConstructor();
		else if( constructor_type == 2 ) 
			InlineConstructor();
		else if( constructor_type == 3 ) 
			SampleConstructorFromTrackGroup();
		else if( constructor_type == 6 ) 
			NALUStartInlineConstructor ();
	}
}
*/
public partial class DefaultHevcExtractorConstructorBox : FullBox
{
	public const string TYPE = "dhec";
	public override string DisplayName { get { return "DefaultHevcExtractorConstructorBox"; } }

	protected uint num_entries; 
	public uint NumEntries { get { return this.num_entries; } set { this.num_entries = value; } }

	protected byte[] constructor_type; 
	public byte[] ConstructorType { get { return this.constructor_type; } set { this.constructor_type = value; } }

	protected byte[] constructor_flags; 
	public byte[] ConstructorFlags { get { return this.constructor_flags; } set { this.constructor_flags = value; } }
	public IEnumerable<SampleConstructor> _SampleConstructor { get { return this.children.OfType<SampleConstructor>(); } }
	public IEnumerable<InlineConstructor> _InlineConstructor { get { return this.children.OfType<InlineConstructor>(); } }
	public IEnumerable<SampleConstructorFromTrackGroup> _SampleConstructorFromTrackGroup { get { return this.children.OfType<SampleConstructorFromTrackGroup>(); } }
	public IEnumerable<NALUStartInlineConstructor> _NALUStartInlineConstructor { get { return this.children.OfType<NALUStartInlineConstructor>(); } }

	public DefaultHevcExtractorConstructorBox(): base(IsoStream.FromFourCC("dhec"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.num_entries, "num_entries"); 

		this.constructor_type = new byte[IsoStream.GetInt( num_entries)];
		this.constructor_flags = new byte[IsoStream.GetInt( num_entries)];
		for (int i=0; i< num_entries; i++)
		{
			boxSize += stream.ReadUInt8(boxSize, readSize,  out this.constructor_type[i], "constructor_type"); 
			boxSize += stream.ReadUInt8(boxSize, readSize,  out this.constructor_flags[i], "constructor_flags"); 

			if ( constructor_type[i] == 0 )
			{
				// boxSize += stream.ReadBox(boxSize, readSize, this,  out this.SampleConstructor[i], "SampleConstructor"); 
			}

			else if ( constructor_type[i] == 2 )
			{
				// boxSize += stream.ReadBox(boxSize, readSize, this,  out this.InlineConstructor[i], "InlineConstructor"); 
			}

			else if ( constructor_type[i] == 3 )
			{
				// boxSize += stream.ReadBox(boxSize, readSize, this,  out this.SampleConstructorFromTrackGroup[i], "SampleConstructorFromTrackGroup"); 
			}

			else if ( constructor_type[i] == 6 )
			{
				// boxSize += stream.ReadBox(boxSize, readSize, this,  out this.NALUStartInlineConstructor[i], "NALUStartInlineConstructor"); 
			}
		}
		boxSize += stream.ReadBoxArrayTillEnd(boxSize, readSize, this);
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteUInt32( this.num_entries, "num_entries"); 

		for (int i=0; i< num_entries; i++)
		{
			boxSize += stream.WriteUInt8( this.constructor_type[i], "constructor_type"); 
			boxSize += stream.WriteUInt8( this.constructor_flags[i], "constructor_flags"); 

			if ( constructor_type[i] == 0 )
			{
				// boxSize += stream.WriteBox( this.SampleConstructor[i], "SampleConstructor"); 
			}

			else if ( constructor_type[i] == 2 )
			{
				// boxSize += stream.WriteBox( this.InlineConstructor[i], "InlineConstructor"); 
			}

			else if ( constructor_type[i] == 3 )
			{
				// boxSize += stream.WriteBox( this.SampleConstructorFromTrackGroup[i], "SampleConstructorFromTrackGroup"); 
			}

			else if ( constructor_type[i] == 6 )
			{
				// boxSize += stream.WriteBox( this.NALUStartInlineConstructor[i], "NALUStartInlineConstructor"); 
			}
		}
		boxSize += stream.WriteBoxArrayTillEnd(this);
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 32; // num_entries

		for (int i=0; i< num_entries; i++)
		{
			boxSize += 8; // constructor_type
			boxSize += 8; // constructor_flags

			if ( constructor_type[i] == 0 )
			{
				// boxSize += IsoStream.CalculateBoxSize(SampleConstructor); // SampleConstructor
			}

			else if ( constructor_type[i] == 2 )
			{
				// boxSize += IsoStream.CalculateBoxSize(InlineConstructor); // InlineConstructor
			}

			else if ( constructor_type[i] == 3 )
			{
				// boxSize += IsoStream.CalculateBoxSize(SampleConstructorFromTrackGroup); // SampleConstructorFromTrackGroup
			}

			else if ( constructor_type[i] == 6 )
			{
				// boxSize += IsoStream.CalculateBoxSize(NALUStartInlineConstructor); // NALUStartInlineConstructor
			}
		}
		boxSize += IsoStream.CalculateBoxArray(this);
		return boxSize;
	}
}

}
