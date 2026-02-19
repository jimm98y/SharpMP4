using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4Common;

namespace SharpISOBMFF
{
/*
class DSTSpecificConfig(channelConfiguration) {
  uimsbf(1) DSDDST_Coded;
  uimsbf(14) N_Channels;
  uimsbf(1) reserved;
}


*/
public partial class DSTSpecificConfig : IMp4Serializable
{
	public StreamMarker Padding { get; set; }
	protected IMp4Serializable parent = null;
	public IMp4Serializable GetParent() { return parent; }
	public void SetParent(IMp4Serializable parent) { this.parent = parent; }
	public virtual string DisplayName { get { return "DSTSpecificConfig"; } }

	protected bool DSDDST_Coded; 
	public bool DSDDSTCoded { get { return this.DSDDST_Coded; } set { this.DSDDST_Coded = value; } }

	protected ushort N_Channels; 
	public ushort NChannels { get { return this.N_Channels; } set { this.N_Channels = value; } }

	protected bool reserved; 
	public bool Reserved { get { return this.reserved; } set { this.reserved = value; } }

	public DSTSpecificConfig(int channelConfiguration): base()
	{
	}

	public virtual ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += stream.ReadBit(boxSize, readSize,  out this.DSDDST_Coded, "DSDDST_Coded"); 
		boxSize += stream.ReadBits(boxSize, readSize, 14,  out this.N_Channels, "N_Channels"); 
		boxSize += stream.ReadBit(boxSize, readSize,  out this.reserved, "reserved"); 
		return boxSize;
	}

	public virtual ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += stream.WriteBit( this.DSDDST_Coded, "DSDDST_Coded"); 
		boxSize += stream.WriteBits(14,  this.N_Channels, "N_Channels"); 
		boxSize += stream.WriteBit( this.reserved, "reserved"); 
		return boxSize;
	}

	public virtual ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += 1; // DSDDST_Coded
		boxSize += 14; // N_Channels
		boxSize += 1; // reserved
		return boxSize;
	}
}

}
