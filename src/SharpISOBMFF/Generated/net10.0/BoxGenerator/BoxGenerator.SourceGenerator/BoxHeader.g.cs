using System;
using System.Linq;
using System.Collections.Generic;

namespace SharpISOBMFF
{
/*
aligned(8) class BoxHeader (
		unsigned int(32) boxtype,
		optional unsigned int(8)[16] extended_type) {
	unsigned int(32) size;
	unsigned int(32) type = boxtype;
	if (size==1) {
		unsigned int(64) largesize;
	} else if (size==0) {
		// box extends to end of file
	}
	if (type=='uuid') {
		unsigned int(8)[16] usertype = extended_type;
	}
}
*/
public partial class BoxHeader : IMp4Serializable
{
	public StreamMarker Padding { get; set; }
	protected IMp4Serializable parent = null;
	public IMp4Serializable GetParent() { return parent; }
	public void SetParent(IMp4Serializable parent) { this.parent = parent; }
	public virtual string DisplayName { get { return "BoxHeader"; } }

	protected uint size; 
	public uint Size { get { return this.size; } set { this.size = value; } }

	protected uint type; // = boxtype
	public uint Type { get { return this.type; } set { this.type = value; } }

	protected ulong largesize; 
	public ulong Largesize { get { return this.largesize; } set { this.largesize = value; } }

	protected byte[] usertype; // = extended_type
	public byte[] Usertype { get { return this.usertype; } set { this.usertype = value; } }

	public BoxHeader(uint boxtype = 0, byte[] extended_type = null): base()
	{
		this.type = boxtype;
		this.usertype = extended_type;
	}

	public virtual ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.size, "size"); 
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.type, "type"); 

		if (size==1)
		{
			boxSize += stream.ReadUInt64(boxSize, readSize,  out this.largesize, "largesize"); 
		}

		else if (size==0)
		{
			/*  box extends to end of file */
		}

		if (type==IsoStream.FromFourCC("uuid"))
		{
			boxSize += stream.ReadUInt8Array(boxSize, readSize, 16,  out this.usertype, "usertype"); 
		}
		return boxSize;
	}

	public virtual ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += stream.WriteUInt32( this.size, "size"); 
		boxSize += stream.WriteUInt32( this.type, "type"); 

		if (size==1)
		{
			boxSize += stream.WriteUInt64( this.largesize, "largesize"); 
		}

		else if (size==0)
		{
			/*  box extends to end of file */
		}

		if (type==IsoStream.FromFourCC("uuid"))
		{
			boxSize += stream.WriteUInt8Array(16,  this.usertype, "usertype"); 
		}
		return boxSize;
	}

	public virtual ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += 32; // size
		boxSize += 32; // type

		if (size==1)
		{
			boxSize += 64; // largesize
		}

		else if (size==0)
		{
			/*  box extends to end of file */
		}

		if (type==IsoStream.FromFourCC("uuid"))
		{
			boxSize += 16 * 8; // usertype
		}
		return boxSize;
	}
}

}
