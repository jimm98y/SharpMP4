using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
class ld_sbr_header(channelConfiguration)
{
  switch (channelConfiguration) {
    case 1:
    case 2:
      numSbrHeader = 1;
      break;
    case 3:
      numSbrHeader = 2;
      break;
    case 4:
    case 5:
    case 6:
      numSbrHeader = 3;
      break;
    case 7:
      numSbrHeader = 4;
      break;
    default:
      numSbrHeader = 0;
      break;
  }
  for (el = 0; el < numSbrHeader; el++) {
    sbr_header();
  }
}


*/
public partial class ld_sbr_header : IMp4Serializable
{
	public StreamMarker Padding { get; set; }
	protected IMp4Serializable parent = null;
	public IMp4Serializable GetParent() { return parent; }
	public void SetParent(IMp4Serializable parent) { this.parent = parent; }
	public virtual string DisplayName { get { return "ld_sbr_header"; } }

	protected sbr_header[] sbr_header; 
	public sbr_header[] SbrHeader { get { return this.sbr_header; } set { this.sbr_header = value; } }

	protected int channelConfiguration; 
	public int ChannelConfiguration { get { return this.channelConfiguration; } set { this.channelConfiguration = value; } }

	public ld_sbr_header(int channelConfiguration): base()
	{
		this.channelConfiguration = channelConfiguration;
	}

	public virtual ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		int numSbrHeader = 0;


		switch (channelConfiguration)
		{
			case 1:
			case 2:
			numSbrHeader = 1;
			break;

			case 3:
			numSbrHeader = 2;
			break;

			case 4:
			case 5:
			case 6:
			numSbrHeader = 3;
			break;

			case 7:
			numSbrHeader = 4;
			break;

			default:

			numSbrHeader = 0;
			break;

		}

		this.sbr_header = new sbr_header[IsoStream.GetInt( numSbrHeader)];
		for (int el = 0; el < numSbrHeader; el++)
		{
			boxSize += stream.ReadClass(boxSize, readSize, this, () => new sbr_header(),  out this.sbr_header[el], "sbr_header"); 
		}
		return boxSize;
	}

	public virtual ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		int numSbrHeader = 0;


		switch (channelConfiguration)
		{
			case 1:
			case 2:
			numSbrHeader = 1;
			break;

			case 3:
			numSbrHeader = 2;
			break;

			case 4:
			case 5:
			case 6:
			numSbrHeader = 3;
			break;

			case 7:
			numSbrHeader = 4;
			break;

			default:

			numSbrHeader = 0;
			break;

		}

		for (int el = 0; el < numSbrHeader; el++)
		{
			boxSize += stream.WriteClass( this.sbr_header[el], "sbr_header"); 
		}
		return boxSize;
	}

	public virtual ulong CalculateSize()
	{
		ulong boxSize = 0;
		int numSbrHeader = 0;


		switch (channelConfiguration)
		{
			case 1:
			case 2:
			numSbrHeader = 1;
			break;

			case 3:
			numSbrHeader = 2;
			break;

			case 4:
			case 5:
			case 6:
			numSbrHeader = 3;
			break;

			case 7:
			numSbrHeader = 4;
			break;

			default:

			numSbrHeader = 0;
			break;

		}

		for (int el = 0; el < numSbrHeader; el++)
		{
			boxSize += IsoStream.CalculateClassSize(sbr_header); // sbr_header
		}
		return boxSize;
	}
}

}
