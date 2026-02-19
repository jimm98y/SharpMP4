using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4Common;

namespace SharpISOBMFF
{
/*
aligned(8) class PartitionEntry extends Box('paen') {
	FilePartitionBox	blocks_and_symbols;
	FECReservoirBox	FEC_symbol_locations; //optional
	FileReservoirBox	File_symbol_locations; //optional
}


*/
public partial class PartitionEntry : Box
{
	public const string TYPE = "paen";
	public override string DisplayName { get { return "PartitionEntry"; } }
	public FilePartitionBox BlocksAndSymbols { get { return this.children.OfType<FilePartitionBox>().FirstOrDefault(); } }
	public FECReservoirBox FECSymbolLocations { get { return this.children.OfType<FECReservoirBox>().FirstOrDefault(); } }
	public FileReservoirBox FileSymbolLocations { get { return this.children.OfType<FileReservoirBox>().FirstOrDefault(); } }

	public PartitionEntry(): base(IsoStream.FromFourCC("paen"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		// boxSize += stream.ReadBox(boxSize, readSize, this,  out this.blocks_and_symbols, "blocks_and_symbols"); 
		// if (stream.HasMoreData(boxSize, readSize)) boxSize += stream.ReadBox(boxSize, readSize, this,  out this.FEC_symbol_locations, "FEC_symbol_locations"); //optional
		// if (stream.HasMoreData(boxSize, readSize)) boxSize += stream.ReadBox(boxSize, readSize, this,  out this.File_symbol_locations, "File_symbol_locations"); //optional
		boxSize += stream.ReadBoxArrayTillEnd(boxSize, readSize, this);
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		// boxSize += stream.WriteBox( this.blocks_and_symbols, "blocks_and_symbols"); 
		// boxSize += stream.WriteBox( this.FEC_symbol_locations, "FEC_symbol_locations"); //optional
		// boxSize += stream.WriteBox( this.File_symbol_locations, "File_symbol_locations"); //optional
		boxSize += stream.WriteBoxArrayTillEnd(this);
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		// boxSize += IsoStream.CalculateBoxSize(blocks_and_symbols); // blocks_and_symbols
		// boxSize += IsoStream.CalculateBoxSize(FEC_symbol_locations); // FEC_symbol_locations
		// boxSize += IsoStream.CalculateBoxSize(File_symbol_locations); // File_symbol_locations
		boxSize += IsoStream.CalculateBoxArray(this);
		return boxSize;
	}
}

}
