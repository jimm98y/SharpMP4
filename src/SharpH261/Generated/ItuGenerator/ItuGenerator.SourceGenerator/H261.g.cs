using System;
using System.Collections.Generic;
using System.Numerics;
using SharpH26X;

namespace SharpH261
{

    public partial class H261Context : IItuContext
    {

    }

    /*
picture_layer() {
 /* Transcribed from ITU-T Rec. H.261 (03/93), 4.2.1 and Figure 5, which give the picture layer as prose: not the
    Recommendation's text. Each element is named after its abbreviation, the bits of PTYPE after what they are. *//*
 psc u(20) /* 4.2.1.1 *//*
 tr u(5) /* 4.2.1.2 *//*
 split_screen_indicator u(1) /* 4.2.1.3: PTYPE bit 1 *//*
 document_camera_indicator u(1) /* bit 2 *//*
 freeze_picture_release u(1) /* bit 3 *//*
 source_format u(1) /* bit 4: 0 QCIF, 1 CIF *//*
 hi_res u(1) /* bit 5: the still image mode of Annex D, on where 0 *//*
 ptype_spare_bit u(1) /* bit 6 *//*
 pei u(1) /* 4.2.1.4 *//*
 if( pei == 1 ) {
  do {
   pspare u(8) /* 4.2.1.5 *//*
   more_pei u(1)
  } while( more_pei_set )
 }
 picture_data() /* the GOBs (4.2.2), to the next picture start code *//*
}
    */
    public class PictureLayer : IItuSerializable
    {
		private uint psc;
		public uint Psc { get { return psc; } set { psc = value; } }
		private uint tr;
		public uint Tr { get { return tr; } set { tr = value; } }
		private byte split_screen_indicator;
		public byte SplitScreenIndicator { get { return split_screen_indicator; } set { split_screen_indicator = value; } }
		private byte document_camera_indicator;
		public byte DocumentCameraIndicator { get { return document_camera_indicator; } set { document_camera_indicator = value; } }
		private byte freeze_picture_release;
		public byte FreezePictureRelease { get { return freeze_picture_release; } set { freeze_picture_release = value; } }
		private byte source_format;
		public byte SourceFormat { get { return source_format; } set { source_format = value; } }
		private byte hi_res;
		public byte HiRes { get { return hi_res; } set { hi_res = value; } }
		private byte ptype_spare_bit;
		public byte PtypeSpareBit { get { return ptype_spare_bit; } set { ptype_spare_bit = value; } }
		private byte pei;
		public byte Pei { get { return pei; } set { pei = value; } }
		private Dictionary<int, uint> pspare;
		public Dictionary<int, uint> Pspare { get { return pspare ??= new Dictionary<int, uint>(); } set { pspare = value; } }
		private Dictionary<int, byte> more_pei;
		public Dictionary<int, byte> MorePei { get { return more_pei ??= new Dictionary<int, byte>(); } set { more_pei = value; } }
		private PictureData picture_data;
		public PictureData PictureData { get { return picture_data; } set { picture_data = value; } }

         public int HasMoreRbspData { get; set; }
         public int[] ReadNextBits { get; set; }

         public PictureLayer()
         { 

         }

         public ulong Read(IItuContext context, ItuStream stream)
         {
            H261Context ituContext = context as H261Context;
            if (ituContext == null)
                throw new ArgumentException($"Context should be of type H261Context");

            ulong size = 0;

			int whileIndex = -1;
/*  Transcribed from ITU-T Rec. H.261 (03/93), 4.2.1 and Figure 5, which give the picture layer as prose: not the
    Recommendation's text. Each element is named after its abbreviation, the bits of PTYPE after what they are.  */

			size += stream.ReadUnsignedInt(size, 20, out this.psc, "psc"); 
/*  4.2.1.1  */

			size += stream.ReadUnsignedInt(size, 5, out this.tr, "tr"); 
/*  4.2.1.2  */

			size += stream.ReadUnsignedInt(size, 1, out this.split_screen_indicator, "split_screen_indicator"); 
/*  4.2.1.3: PTYPE bit 1  */

			size += stream.ReadUnsignedInt(size, 1, out this.document_camera_indicator, "document_camera_indicator"); 
/*  bit 2  */

			size += stream.ReadUnsignedInt(size, 1, out this.freeze_picture_release, "freeze_picture_release"); 
/*  bit 3  */

			size += stream.ReadUnsignedInt(size, 1, out this.source_format, "source_format"); 
			ituContext.OnSourceFormat(source_format);
/*  bit 4: 0 QCIF, 1 CIF  */

			size += stream.ReadUnsignedInt(size, 1, out this.hi_res, "hi_res"); 
/*  bit 5: the still image mode of Annex D, on where 0  */

			size += stream.ReadUnsignedInt(size, 1, out this.ptype_spare_bit, "ptype_spare_bit"); 
/*  bit 6  */

			size += stream.ReadUnsignedInt(size, 1, out this.pei, "pei"); 
/*  4.2.1.4  */


			if ( pei == 1 )
			{

				do
				{
					whileIndex++;

					size += stream.ReadUnsignedInt(size, 8, whileIndex, (this.pspare ??= new()), "pspare"); 
/*  4.2.1.5  */

					size += stream.ReadUnsignedInt(size, 1, whileIndex, (this.more_pei ??= new()), "more_pei"); 
				} while ( (this.more_pei[whileIndex] == 1 ? 1 : 0) != 0 );
			}
			this.picture_data =  new PictureData() ;
			size +=  stream.ReadClass<PictureData>(size, context, this.picture_data, "picture_data"); // the GOBs (4.2.2), to the next picture start code 

            return size;
         }

         public ulong Write(IItuContext context, ItuStream stream)
         {
            H261Context ituContext = context as H261Context;
            if (ituContext == null)
                throw new ArgumentException($"Context should be of type H261Context");
            ulong size = 0;

			int whileIndex = -1;
/*  Transcribed from ITU-T Rec. H.261 (03/93), 4.2.1 and Figure 5, which give the picture layer as prose: not the
    Recommendation's text. Each element is named after its abbreviation, the bits of PTYPE after what they are.  */

			size += stream.WriteUnsignedInt(20, this.psc, "psc"); 
/*  4.2.1.1  */

			size += stream.WriteUnsignedInt(5, this.tr, "tr"); 
/*  4.2.1.2  */

			size += stream.WriteUnsignedInt(1, this.split_screen_indicator, "split_screen_indicator"); 
/*  4.2.1.3: PTYPE bit 1  */

			size += stream.WriteUnsignedInt(1, this.document_camera_indicator, "document_camera_indicator"); 
/*  bit 2  */

			size += stream.WriteUnsignedInt(1, this.freeze_picture_release, "freeze_picture_release"); 
/*  bit 3  */

			size += stream.WriteUnsignedInt(1, this.source_format, "source_format"); 
			ituContext.OnSourceFormat(source_format);
/*  bit 4: 0 QCIF, 1 CIF  */

			size += stream.WriteUnsignedInt(1, this.hi_res, "hi_res"); 
/*  bit 5: the still image mode of Annex D, on where 0  */

			size += stream.WriteUnsignedInt(1, this.ptype_spare_bit, "ptype_spare_bit"); 
/*  bit 6  */

			size += stream.WriteUnsignedInt(1, this.pei, "pei"); 
/*  4.2.1.4  */


			if ( pei == 1 )
			{

				do
				{
					whileIndex++;

					size += stream.WriteUnsignedInt(8, whileIndex, (this.pspare ??= new()), "pspare"); 
/*  4.2.1.5  */

					size += stream.WriteUnsignedInt(1, whileIndex, (this.more_pei ??= new()), "more_pei"); 
				} while ( (whileIndex + 1 < this.MorePei.Count ? 1 : 0) != 0 );
			}
			size += stream.WriteClass<PictureData>(context, this.picture_data, "picture_data"); // the GOBs (4.2.2), to the next picture start code 

            return size;
         }

    }

}
