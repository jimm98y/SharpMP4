using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4.Common;

namespace SharpISOBMFF
{
/*
aligned(8) class AdobeChapterRecord() {
 unsigned int(64) timestamp;
 unsigned int(8) count; char title[count];
 }
*/
public partial class AdobeChapterRecord : IMp4Serializable
{
	public StreamMarker Padding { get; set; }
	protected IMp4Serializable parent = null;
	public IMp4Serializable GetParent() { return parent; }
	public void SetParent(IMp4Serializable parent) { this.parent = parent; }
	public override string ToString() { return BoxStringBuilder.Format(this); }
	public virtual string DisplayName { get { return "AdobeChapterRecord"; } }

	protected ulong timestamp; 
	public ulong Timestamp { get { return this.timestamp; } set { this.timestamp = value; } }

	protected byte count; 
	public byte Count { get { return this.count; } set { this.count = value; } }

	protected byte[] title; 
	public byte[] Title { get { return this.title; } set { this.title = value; } }

	public AdobeChapterRecord(): base()
	{
	}

	public virtual ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += stream.ReadUInt64(boxSize, readSize,  out this.timestamp, "timestamp"); 
		boxSize += stream.ReadUInt8(boxSize, readSize,  out this.count, "count"); 
		boxSize += stream.ReadUInt8Array(boxSize, readSize, (uint)(count),  out this.title, "title"); 
		return boxSize;
	}

	public virtual ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += stream.WriteUInt64( this.timestamp, "timestamp"); 
		boxSize += stream.WriteUInt8( this.count, "count"); 
		boxSize += stream.WriteUInt8Array((uint)(count),  this.title, "title"); 
		return boxSize;
	}

	public virtual ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += 64; // timestamp
		boxSize += 8; // count
		boxSize += ((ulong)(count) * 8); // title
		return boxSize;
	}

	public virtual ulong ToString(BoxStringBuilder builder)
	{
		ulong boxSize = 0;
		boxSize += builder.Field("timestamp", 64, this.timestamp); 
		boxSize += builder.Field("count", 8, this.count); 
		boxSize += builder.Field("title", ((ulong)(count) * 8), this.title); 
		return boxSize;
	}
}

}
