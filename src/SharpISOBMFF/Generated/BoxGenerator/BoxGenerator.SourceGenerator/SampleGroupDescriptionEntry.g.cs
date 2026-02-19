using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4Common;

namespace SharpISOBMFF
{
/*
// Sequence Entry  
abstract class SampleGroupDescriptionEntry (unsigned int(32) grouping_type) 
{ 
} 


*/
public abstract partial class SampleGroupDescriptionEntry : IHasBoxChildren
{
	public StreamMarker Padding { get; set; }
	protected IMp4Serializable parent = null;
	public IMp4Serializable GetParent() { return parent; }
	public void SetParent(IMp4Serializable parent) { this.parent = parent; }
	public virtual string DisplayName { get { return "SampleGroupDescriptionEntry"; } }

	protected List<Box> children= new List<Box>(); 
	public List<Box> Children { get { return this.children; } set { this.children = value; } }

	public SampleGroupDescriptionEntry(uint grouping_type): base()
	{
	}

	public virtual ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		return boxSize;
	}

	public virtual ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		return boxSize;
	}

	public virtual ulong CalculateSize()
	{
		ulong boxSize = 0;
		return boxSize;
	}
}

}
