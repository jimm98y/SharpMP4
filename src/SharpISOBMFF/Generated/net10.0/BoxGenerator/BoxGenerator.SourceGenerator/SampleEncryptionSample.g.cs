using System;
using System.Linq;
using System.Collections.Generic;

namespace SharpISOBMFF
{
/*
aligned(8) class SampleEncryptionSample(version, flags, Per_Sample_IV_Size) {
      if (version == 0) {
         unsigned int(Per_Sample_IV_Size*8) InitializationVector;
         if (flags & 0x000002) {            unsigned int(16) subsample_count;
            SampleEncryptionSubsample(version) subsamples[subsample_count];
         }
      } else if (version==1) {
         unsigned int(16) multi_IV_count;
         for (i=1; i <= multi_IV_count; i++) {
            unsigned int(8) multi_subindex_IV;
            unsigned int(Per_Sample_IV_Size*8) IV;
         }
          unsigned int(32) subsample_count;
          SampleEncryptionSubsample(version) subsamples[subsample_count];
      }
}


*/
public partial class SampleEncryptionSample : IMp4Serializable
{
	public StreamMarker Padding { get; set; }
	protected IMp4Serializable parent = null;
	public IMp4Serializable GetParent() { return parent; }
	public void SetParent(IMp4Serializable parent) { this.parent = parent; }
	public virtual string DisplayName { get { return "SampleEncryptionSample"; } }

	protected byte[] InitializationVector; 
	public byte[] _InitializationVector { get { return this.InitializationVector; } set { this.InitializationVector = value; } }

	protected uint subsample_count; 
	public uint SubsampleCount { get { return this.subsample_count; } set { this.subsample_count = value; } }

	protected SampleEncryptionSubsample[] subsamples; 
	public SampleEncryptionSubsample[] Subsamples { get { return this.subsamples; } set { this.subsamples = value; } }

	protected ushort multi_IV_count; 
	public ushort MultiIVCount { get { return this.multi_IV_count; } set { this.multi_IV_count = value; } }

	protected byte[] multi_subindex_IV; 
	public byte[] MultiSubindexIV { get { return this.multi_subindex_IV; } set { this.multi_subindex_IV = value; } }

	protected byte[][] IV; 
	public byte[][] _IV { get { return this.IV; } set { this.IV = value; } }

	protected byte version; 
	public byte Version { get { return this.version; } set { this.version = value; } }

	protected uint flags; 
	public uint Flags { get { return this.flags; } set { this.flags = value; } }

	protected byte Per_Sample_IV_Size; 
	public byte PerSampleIVSize { get { return this.Per_Sample_IV_Size; } set { this.Per_Sample_IV_Size = value; } }

	public SampleEncryptionSample(byte version, uint flags, byte Per_Sample_IV_Size): base()
	{
		this.version = version;
		this.flags = flags;
		this.Per_Sample_IV_Size = Per_Sample_IV_Size;
	}

	public virtual ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;

		if (version == 0)
		{
			boxSize += stream.ReadBits(boxSize, readSize, (uint)(Per_Sample_IV_Size*8 ),  out this.InitializationVector, "InitializationVector"); 

			if ((flags  &  0x000002) ==  0x000002)
			{
				boxSize += stream.ReadUInt16(boxSize, readSize,  out this.subsample_count, "subsample_count"); 
				boxSize += stream.ReadClass(boxSize, readSize, this, (uint)(subsample_count), () => new SampleEncryptionSubsample(version),  out this.subsamples, "subsamples"); 
			}
		}

		else if (version==1)
		{
			boxSize += stream.ReadUInt16(boxSize, readSize,  out this.multi_IV_count, "multi_IV_count"); 

			this.multi_subindex_IV = new byte[IsoStream.GetInt( multi_IV_count)];
			this.IV = new byte[IsoStream.GetInt( multi_IV_count)][];
			for (int i=0; i < multi_IV_count; i++)
			{
				boxSize += stream.ReadUInt8(boxSize, readSize,  out this.multi_subindex_IV[i], "multi_subindex_IV"); 
				boxSize += stream.ReadBits(boxSize, readSize, (uint)(Per_Sample_IV_Size*8 ),  out this.IV[i], "IV"); 
			}
			boxSize += stream.ReadUInt32(boxSize, readSize,  out this.subsample_count, "subsample_count"); 
			boxSize += stream.ReadClass(boxSize, readSize, this, (uint)(subsample_count), () => new SampleEncryptionSubsample(version),  out this.subsamples, "subsamples"); 
		}
		return boxSize;
	}

	public virtual ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;

		if (version == 0)
		{
			boxSize += stream.WriteBits((uint)(Per_Sample_IV_Size*8 ),  this.InitializationVector, "InitializationVector"); 

			if ((flags  &  0x000002) ==  0x000002)
			{
				boxSize += stream.WriteUInt16( this.subsample_count, "subsample_count"); 
				boxSize += stream.WriteClass( this.subsamples, "subsamples"); 
			}
		}

		else if (version==1)
		{
			boxSize += stream.WriteUInt16( this.multi_IV_count, "multi_IV_count"); 

			for (int i=0; i < multi_IV_count; i++)
			{
				boxSize += stream.WriteUInt8( this.multi_subindex_IV[i], "multi_subindex_IV"); 
				boxSize += stream.WriteBits((uint)(Per_Sample_IV_Size*8 ),  this.IV[i], "IV"); 
			}
			boxSize += stream.WriteUInt32( this.subsample_count, "subsample_count"); 
			boxSize += stream.WriteClass( this.subsamples, "subsamples"); 
		}
		return boxSize;
	}

	public virtual ulong CalculateSize()
	{
		ulong boxSize = 0;

		if (version == 0)
		{
			boxSize += (ulong)(Per_Sample_IV_Size*8 ); // InitializationVector

			if ((flags  &  0x000002) ==  0x000002)
			{
				boxSize += 16; // subsample_count
				boxSize += IsoStream.CalculateClassSize(subsamples); // subsamples
			}
		}

		else if (version==1)
		{
			boxSize += 16; // multi_IV_count

			for (int i=0; i < multi_IV_count; i++)
			{
				boxSize += 8; // multi_subindex_IV
				boxSize += (ulong)(Per_Sample_IV_Size*8 ); // IV
			}
			boxSize += 32; // subsample_count
			boxSize += IsoStream.CalculateClassSize(subsamples); // subsamples
		}
		return boxSize;
	}
}

}
