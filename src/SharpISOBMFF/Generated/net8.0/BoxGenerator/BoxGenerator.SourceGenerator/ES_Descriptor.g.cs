using System;
using System.Linq;
using System.Collections.Generic;

namespace SharpISOBMFF
{
/*
class ES_Descriptor extends BaseDescriptor : bit(8) tag=ES_DescrTag {
 bit(16) ES_ID;
 bit(1) streamDependenceFlag;
 bit(1) URL_Flag;
 bit(1) OCRstreamFlag;
 bit(5) streamPriority;
 if (streamDependenceFlag)
 bit(16) dependsOn_ES_ID;
 if (URL_Flag) {
 bit(8) URLlength;
 bit(8) URLstring[URLlength];
 }
 if (OCRstreamFlag)
 bit(16) OCR_ES_Id;
 DecoderConfigDescriptor decConfigDescr;
 SLConfigDescriptor slConfigDescr;
 IPI_DescrPointer ipiPtr[0 .. 1];
 IP_IdentificationDataSet ipIDS[0 .. 255];
 IPMP_DescriptorPointer ipmpDescrPtr[0 .. 255];
 LanguageDescriptor langDescr[0 .. 255];
 QoS_Descriptor qosDescr[0 .. 1];
 RegistrationDescriptor regDescr[0 .. 1];
 ExtensionDescriptor extDescr[0 .. 255];
 }
*/
public partial class ES_Descriptor : BaseDescriptor
{
	public const byte TYPE = DescriptorTags.ES_DescrTag;
	public override string DisplayName { get { return "ES_Descriptor"; } }

	protected ushort ES_ID; 
	public ushort ESID { get { return this.ES_ID; } set { this.ES_ID = value; } }

	protected bool streamDependenceFlag; 
	public bool StreamDependenceFlag { get { return this.streamDependenceFlag; } set { this.streamDependenceFlag = value; } }

	protected bool URL_Flag; 
	public bool URLFlag { get { return this.URL_Flag; } set { this.URL_Flag = value; } }

	protected bool OCRstreamFlag; 
	public bool _OCRstreamFlag { get { return this.OCRstreamFlag; } set { this.OCRstreamFlag = value; } }

	protected byte streamPriority; 
	public byte StreamPriority { get { return this.streamPriority; } set { this.streamPriority = value; } }

	protected ushort dependsOn_ES_ID; 
	public ushort DependsOnESID { get { return this.dependsOn_ES_ID; } set { this.dependsOn_ES_ID = value; } }

	protected byte URLlength; 
	public byte _URLlength { get { return this.URLlength; } set { this.URLlength = value; } }

	protected byte[] URLstring; 
	public byte[] _URLstring { get { return this.URLstring; } set { this.URLstring = value; } }

	protected ushort OCR_ES_Id; 
	public ushort OCRESId { get { return this.OCR_ES_Id; } set { this.OCR_ES_Id = value; } }
	public DecoderConfigDescriptor DecConfigDescr { get { return this.children.OfType<DecoderConfigDescriptor>().FirstOrDefault(); } }
	public SLConfigDescriptor SlConfigDescr { get { return this.children.OfType<SLConfigDescriptor>().FirstOrDefault(); } }
	public IEnumerable<IPI_DescrPointer> IpiPtr { get { return this.children.OfType<IPI_DescrPointer>(); } }
	public IEnumerable<IP_IdentificationDataSet> IpIDS { get { return this.children.OfType<IP_IdentificationDataSet>(); } }
	public IEnumerable<IPMP_DescriptorPointer> IpmpDescrPtr { get { return this.children.OfType<IPMP_DescriptorPointer>(); } }
	public IEnumerable<LanguageDescriptor> LangDescr { get { return this.children.OfType<LanguageDescriptor>(); } }
	public IEnumerable<QoS_Descriptor> QosDescr { get { return this.children.OfType<QoS_Descriptor>(); } }
	public IEnumerable<RegistrationDescriptor> RegDescr { get { return this.children.OfType<RegistrationDescriptor>(); } }
	public IEnumerable<ExtensionDescriptor> ExtDescr { get { return this.children.OfType<ExtensionDescriptor>(); } }

	public ES_Descriptor(): base(DescriptorTags.ES_DescrTag)
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadUInt16(boxSize, readSize,  out this.ES_ID, "ES_ID"); 
		boxSize += stream.ReadBit(boxSize, readSize,  out this.streamDependenceFlag, "streamDependenceFlag"); 
		boxSize += stream.ReadBit(boxSize, readSize,  out this.URL_Flag, "URL_Flag"); 
		boxSize += stream.ReadBit(boxSize, readSize,  out this.OCRstreamFlag, "OCRstreamFlag"); 
		boxSize += stream.ReadBits(boxSize, readSize, 5,  out this.streamPriority, "streamPriority"); 

		if (streamDependenceFlag)
		{
			boxSize += stream.ReadUInt16(boxSize, readSize,  out this.dependsOn_ES_ID, "dependsOn_ES_ID"); 
		}

		if (URL_Flag)
		{
			boxSize += stream.ReadUInt8(boxSize, readSize,  out this.URLlength, "URLlength"); 
			boxSize += stream.ReadUInt8Array(boxSize, readSize, (uint)(URLlength),  out this.URLstring, "URLstring"); 
		}

		if (OCRstreamFlag)
		{
			boxSize += stream.ReadUInt16(boxSize, readSize,  out this.OCR_ES_Id, "OCR_ES_Id"); 
		}
		// boxSize += stream.ReadDescriptor(boxSize, readSize, this,  out this.decConfigDescr, "decConfigDescr"); 
		// boxSize += stream.ReadDescriptor(boxSize, readSize, this,  out this.slConfigDescr, "slConfigDescr"); 
		// boxSize += stream.ReadDescriptor(boxSize, readSize, this,  out this.ipiPtr, "ipiPtr"); 
		// boxSize += stream.ReadDescriptor(boxSize, readSize, this,  out this.ipIDS, "ipIDS"); 
		// boxSize += stream.ReadDescriptor(boxSize, readSize, this,  out this.ipmpDescrPtr, "ipmpDescrPtr"); 
		// boxSize += stream.ReadDescriptor(boxSize, readSize, this,  out this.langDescr, "langDescr"); 
		// boxSize += stream.ReadDescriptor(boxSize, readSize, this,  out this.qosDescr, "qosDescr"); 
		// boxSize += stream.ReadDescriptor(boxSize, readSize, this,  out this.regDescr, "regDescr"); 
		// boxSize += stream.ReadDescriptor(boxSize, readSize, this,  out this.extDescr, "extDescr"); 
		boxSize += stream.ReadDescriptorsTillEnd(boxSize, readSize, this);
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteUInt16( this.ES_ID, "ES_ID"); 
		boxSize += stream.WriteBit( this.streamDependenceFlag, "streamDependenceFlag"); 
		boxSize += stream.WriteBit( this.URL_Flag, "URL_Flag"); 
		boxSize += stream.WriteBit( this.OCRstreamFlag, "OCRstreamFlag"); 
		boxSize += stream.WriteBits(5,  this.streamPriority, "streamPriority"); 

		if (streamDependenceFlag)
		{
			boxSize += stream.WriteUInt16( this.dependsOn_ES_ID, "dependsOn_ES_ID"); 
		}

		if (URL_Flag)
		{
			boxSize += stream.WriteUInt8( this.URLlength, "URLlength"); 
			boxSize += stream.WriteUInt8Array((uint)(URLlength),  this.URLstring, "URLstring"); 
		}

		if (OCRstreamFlag)
		{
			boxSize += stream.WriteUInt16( this.OCR_ES_Id, "OCR_ES_Id"); 
		}
		// boxSize += stream.WriteDescriptor( this.decConfigDescr, "decConfigDescr"); 
		// boxSize += stream.WriteDescriptor( this.slConfigDescr, "slConfigDescr"); 
		// boxSize += stream.WriteDescriptor( this.ipiPtr, "ipiPtr"); 
		// boxSize += stream.WriteDescriptor( this.ipIDS, "ipIDS"); 
		// boxSize += stream.WriteDescriptor( this.ipmpDescrPtr, "ipmpDescrPtr"); 
		// boxSize += stream.WriteDescriptor( this.langDescr, "langDescr"); 
		// boxSize += stream.WriteDescriptor( this.qosDescr, "qosDescr"); 
		// boxSize += stream.WriteDescriptor( this.regDescr, "regDescr"); 
		// boxSize += stream.WriteDescriptor( this.extDescr, "extDescr"); 
		boxSize += stream.WriteDescriptorsTillEnd(this);
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += 16; // ES_ID
		boxSize += 1; // streamDependenceFlag
		boxSize += 1; // URL_Flag
		boxSize += 1; // OCRstreamFlag
		boxSize += 5; // streamPriority

		if (streamDependenceFlag)
		{
			boxSize += 16; // dependsOn_ES_ID
		}

		if (URL_Flag)
		{
			boxSize += 8; // URLlength
			boxSize += ((ulong)(URLlength) * 8); // URLstring
		}

		if (OCRstreamFlag)
		{
			boxSize += 16; // OCR_ES_Id
		}
		// boxSize += IsoStream.CalculateDescriptorSize(decConfigDescr); // decConfigDescr
		// boxSize += IsoStream.CalculateDescriptorSize(slConfigDescr); // slConfigDescr
		// boxSize += IsoStream.CalculateDescriptorSize(ipiPtr); // ipiPtr
		// boxSize += IsoStream.CalculateDescriptorSize(ipIDS); // ipIDS
		// boxSize += IsoStream.CalculateDescriptorSize(ipmpDescrPtr); // ipmpDescrPtr
		// boxSize += IsoStream.CalculateDescriptorSize(langDescr); // langDescr
		// boxSize += IsoStream.CalculateDescriptorSize(qosDescr); // qosDescr
		// boxSize += IsoStream.CalculateDescriptorSize(regDescr); // regDescr
		// boxSize += IsoStream.CalculateDescriptorSize(extDescr); // extDescr
		boxSize += IsoStream.CalculateDescriptors(this);
		return boxSize;
	}
}

}
