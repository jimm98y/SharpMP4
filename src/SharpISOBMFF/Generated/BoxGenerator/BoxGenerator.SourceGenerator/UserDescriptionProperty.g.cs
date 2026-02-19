using System;
using System.Linq;
using System.Collections.Generic;
using SharpMP4Common;

namespace SharpISOBMFF
{
/*
aligned(8) class UserDescriptionProperty
extends ItemFullProperty('udes', version = 0, flags = 0){
	utf8string lang;
	utf8string name;
	utf8string description;
	utf8string tags;
}

*/
public partial class UserDescriptionProperty : ItemFullProperty
{
	public const string TYPE = "udes";
	public override string DisplayName { get { return "UserDescriptionProperty"; } }

	protected BinaryUTF8String lang; 
	public BinaryUTF8String Lang { get { return this.lang; } set { this.lang = value; } }

	protected BinaryUTF8String name; 
	public BinaryUTF8String Name { get { return this.name; } set { this.name = value; } }

	protected BinaryUTF8String description; 
	public BinaryUTF8String Description { get { return this.description; } set { this.description = value; } }

	protected BinaryUTF8String tags; 
	public BinaryUTF8String Tags { get { return this.tags; } set { this.tags = value; } }

	public UserDescriptionProperty(): base(IsoStream.FromFourCC("udes"), 0, 0)
	{
	}

	public override ulong Read(IsoStream stream, ulong readSize)
	{
		ulong boxSize = 0;
		boxSize += base.Read(stream, readSize);
		boxSize += stream.ReadStringZeroTerminated(boxSize, readSize,  out this.lang, "lang"); 
		boxSize += stream.ReadStringZeroTerminated(boxSize, readSize,  out this.name, "name"); 
		boxSize += stream.ReadStringZeroTerminated(boxSize, readSize,  out this.description, "description"); 
		boxSize += stream.ReadStringZeroTerminated(boxSize, readSize,  out this.tags, "tags"); 
		return boxSize;
	}

	public override ulong Write(IsoStream stream)
	{
		ulong boxSize = 0;
		boxSize += base.Write(stream);
		boxSize += stream.WriteStringZeroTerminated( this.lang, "lang"); 
		boxSize += stream.WriteStringZeroTerminated( this.name, "name"); 
		boxSize += stream.WriteStringZeroTerminated( this.description, "description"); 
		boxSize += stream.WriteStringZeroTerminated( this.tags, "tags"); 
		return boxSize;
	}

	public override ulong CalculateSize()
	{
		ulong boxSize = 0;
		boxSize += base.CalculateSize();
		boxSize += IsoStream.CalculateStringSize(lang); // lang
		boxSize += IsoStream.CalculateStringSize(name); // name
		boxSize += IsoStream.CalculateStringSize(description); // description
		boxSize += IsoStream.CalculateStringSize(tags); // tags
		return boxSize;
	}
}

}
