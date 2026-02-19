using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4Common;

namespace SharpISOBMFF
{
/*
aligned(8) class EventMessageBox() extends Box('emsg') {
 string schemeIdUri;
 string value;
 unsigned int(32) timescale;
 unsigned int(32) presentationTimeDelta;
 unsigned int(32) eventDuration;
 unsigned int(32) id;
 unsigned int(8) messageData[]; } 
*/
public partial class EventMessageBox : Box
{
	public const string TYPE = "emsg";
	public override string DisplayName { get { return "EventMessageBox"; } }

	protected BinaryUTF8String schemeIdUri; 
	public BinaryUTF8String SchemeIdUri { get { return this.schemeIdUri; } set { this.schemeIdUri = value; } }

	protected BinaryUTF8String value; 
	public BinaryUTF8String Value { get { return this.value; } set { this.value = value; } }

	protected uint timescale; 
	public uint Timescale { get { return this.timescale; } set { this.timescale = value; } }

	protected uint presentationTimeDelta; 
	public uint PresentationTimeDelta { get { return this.presentationTimeDelta; } set { this.presentationTimeDelta = value; } }

	protected uint eventDuration; 
	public uint EventDuration { get { return this.eventDuration; } set { this.eventDuration = value; } }

	protected uint id; 
	public uint Id { get { return this.id; } set { this.id = value; } }

	protected byte[] messageData; 
	public byte[] MessageData { get { return this.messageData; } set { this.messageData = value; } }

	public EventMessageBox(): base(IsoStream.FromFourCC("emsg"))
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadStringZeroTerminated(boxSize, readSize,  out this.schemeIdUri, "schemeIdUri"); 
		boxSize += stream.ReadStringZeroTerminated(boxSize, readSize,  out this.value, "value"); 
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.timescale, "timescale"); 
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.presentationTimeDelta, "presentationTimeDelta"); 
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.eventDuration, "eventDuration"); 
		boxSize += stream.ReadUInt32(boxSize, readSize,  out this.id, "id"); 
		boxSize += stream.ReadUInt8ArrayTillEnd(boxSize, readSize,  out this.messageData, "messageData"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteStringZeroTerminated( this.schemeIdUri, "schemeIdUri"); 
		boxSize += stream.WriteStringZeroTerminated( this.value, "value"); 
		boxSize += stream.WriteUInt32( this.timescale, "timescale"); 
		boxSize += stream.WriteUInt32( this.presentationTimeDelta, "presentationTimeDelta"); 
		boxSize += stream.WriteUInt32( this.eventDuration, "eventDuration"); 
		boxSize += stream.WriteUInt32( this.id, "id"); 
		boxSize += stream.WriteUInt8ArrayTillEnd( this.messageData, "messageData"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += IsoStream.CalculateStringSize(schemeIdUri); // schemeIdUri
		boxSize += IsoStream.CalculateStringSize(value); // value
		boxSize += 32; // timescale
		boxSize += 32; // presentationTimeDelta
		boxSize += 32; // eventDuration
		boxSize += 32; // id
		boxSize += ((ulong)messageData.Length * 8); // messageData
		return boxSize;
	}
}

}
