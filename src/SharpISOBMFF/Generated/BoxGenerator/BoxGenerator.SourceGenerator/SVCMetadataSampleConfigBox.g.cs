using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
class SVCMetadataSampleConfigBox extends FullBox('svmC')
{
	int i;		// local variable, not a field
	unsigned int(8) sample_statement_type;	/* normally group, or seq *//*
	unsigned int(8) default_statement_type;
	unsigned int(8) default_statement_length;
	unsigned int(8) entry_count;
	for (i=1; i<=entry_count; i++) {
		unsigned int(8) statement_type;	// from the user extension ranges
		string statement_namespace;
	}
}
*/
public partial class SVCMetadataSampleConfigBox : FullBox
{
	public const string TYPE = "svmC";
	public override string DisplayName { get { return "SVCMetadataSampleConfigBox"; } }

	protected byte sample_statement_type; 
	public byte SampleStatementType { get { return this.sample_statement_type; } set { this.sample_statement_type = value; } }

	protected byte default_statement_type; 
	public byte DefaultStatementType { get { return this.default_statement_type; } set { this.default_statement_type = value; } }

	protected byte default_statement_length; 
	public byte DefaultStatementLength { get { return this.default_statement_length; } set { this.default_statement_length = value; } }

	protected byte entry_count; 
	public byte EntryCount { get { return this.entry_count; } set { this.entry_count = value; } }

	protected byte[] statement_type;  //  from the user extension ranges
	public byte[] StatementType { get { return this.statement_type; } set { this.statement_type = value; } }

	protected BinaryUTF8String[] statement_namespace; 
	public BinaryUTF8String[] StatementNamespace { get { return this.statement_namespace; } set { this.statement_namespace = value; } }

	public SVCMetadataSampleConfigBox(): base(IsoStream.FromFourCC("svmC"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		
		boxSize += stream.ReadUInt8(boxSize, readSize,  out this.sample_statement_type, "sample_statement_type"); 
		/*  normally group, or seq  */
		boxSize += stream.ReadUInt8(boxSize, readSize,  out this.default_statement_type, "default_statement_type"); 
		boxSize += stream.ReadUInt8(boxSize, readSize,  out this.default_statement_length, "default_statement_length"); 
		boxSize += stream.ReadUInt8(boxSize, readSize,  out this.entry_count, "entry_count"); 

		this.statement_type = new byte[IsoStream.GetInt(entry_count)];
		this.statement_namespace = new BinaryUTF8String[IsoStream.GetInt(entry_count)];
		for (int i=0; i<entry_count; i++)
		{
			boxSize += stream.ReadUInt8(boxSize, readSize,  out this.statement_type[i], "statement_type"); // from the user extension ranges
			boxSize += stream.ReadStringZeroTerminated(boxSize, readSize,  out this.statement_namespace[i], "statement_namespace"); 
		}
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		
		boxSize += stream.WriteUInt8( this.sample_statement_type, "sample_statement_type"); 
		/*  normally group, or seq  */
		boxSize += stream.WriteUInt8( this.default_statement_type, "default_statement_type"); 
		boxSize += stream.WriteUInt8( this.default_statement_length, "default_statement_length"); 
		boxSize += stream.WriteUInt8( this.entry_count, "entry_count"); 

		for (int i=0; i<entry_count; i++)
		{
			boxSize += stream.WriteUInt8( this.statement_type[i], "statement_type"); // from the user extension ranges
			boxSize += stream.WriteStringZeroTerminated( this.statement_namespace[i], "statement_namespace"); 
		}
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		
		boxSize += 8; // sample_statement_type
		/*  normally group, or seq  */
		boxSize += 8; // default_statement_type
		boxSize += 8; // default_statement_length
		boxSize += 8; // entry_count

		for (int i=0; i<entry_count; i++)
		{
			boxSize += 8; // statement_type
			boxSize += IsoStream.CalculateStringSize(statement_namespace); // statement_namespace
		}
		return boxSize;
	}
}

}
