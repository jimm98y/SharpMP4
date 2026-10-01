using System;
using System.Collections.Generic;
using System.Numerics;
using SharpH26X;

namespace SharpH263
{

    public partial class H263Context : IItuContext
    {

    }

    /*
picture_layer() {
 /* Transcribed from ITU-T Rec. H.263 (01/2005), 5.1 and Figures 7 and 8, which give the picture layer as prose: not
    the Recommendation's text. Each element is named after its abbreviation, the bits of PTYPE and PLUSPTYPE after
    what they are. *//*
 psc u(22) /* 5.1.1 *//*
 tr u(8) /* 5.1.2 *//*
 ptype_start_code_emulation_bit u(1) /* 5.1.3: PTYPE bit 1, always 1 *//*
 ptype_h261_distinction_bit u(1) /* bit 2, always 0 *//*
 split_screen_indicator u(1) /* bit 3 *//*
 document_camera_indicator u(1) /* bit 4 *//*
 full_picture_freeze_release u(1) /* bit 5 *//*
 source_format u(3) /* bits 6-8 *//*
 if( source_format != 7 ) {
  picture_coding_type u(1) /* bit 9 *//*
  unrestricted_motion_vector_mode u(1) /* bit 10, Annex D *//*
  syntax_based_arithmetic_coding_mode u(1) /* bit 11, Annex E *//*
  advanced_prediction_mode u(1) /* bit 12, Annex F *//*
  pb_frames_mode u(1) /* bit 13, Annex G *//*
 } else {
  /* PLUSPTYPE, 5.1.4 *//*
  ufep u(3) /* 5.1.4.1 *//*
  if( ufep == 1 ) {
   /* OPPTYPE, 5.1.4.2 *//*
   opptype_source_format u(3) /* bits 1-3 *//*
   custom_pcf u(1) /* bit 4 *//*
   umv_mode u(1) /* bit 5, Annex D *//*
   sac_mode u(1) /* bit 6, Annex E *//*
   ap_mode u(1) /* bit 7, Annex F *//*
   aic_mode u(1) /* bit 8, Annex I *//*
   df_mode u(1) /* bit 9, Annex J *//*
   ss_mode u(1) /* bit 10, Annex K *//*
   rps_mode u(1) /* bit 11, Annex N *//*
   isd_mode u(1) /* bit 12, Annex R *//*
   aiv_mode u(1) /* bit 13, Annex S *//*
   mq_mode u(1) /* bit 14, Annex T *//*
   opptype_start_code_emulation_bit u(1) /* bit 15, always 1 *//*
   opptype_reserved_bits u(3) /* bits 16-18, 0 *//*
  }
  /* MPPTYPE, 5.1.4.3 *//*
  picture_type_code u(3) /* bits 1-3 *//*
  rpr_mode u(1) /* bit 4, Annex P *//*
  rru_mode u(1) /* bit 5, Annex Q *//*
  rounding_type u(1) /* bit 6 *//*
  mpptype_reserved_bits u(2) /* bits 7-8, 0 *//*
  mpptype_start_code_emulation_bit u(1) /* bit 9, always 1 *//*
  cpm u(1) /* 5.1.20: after PLUSPTYPE where it is present (5.1.4.7) *//*
  if( cpm == 1 )
   psbi u(2) /* 5.1.21 *//*
  if( ufep == 1 && opptype_source_format == 6 ) {
   /* CPFMT, 5.1.5 *//*
   pixel_aspect_ratio_code u(4) /* bits 1-4 *//*
   picture_width_indication u(9) /* bits 5-13 *//*
   cpfmt_start_code_emulation_bit u(1) /* bit 14, always 1 *//*
   picture_height_indication u(9) /* bits 15-23 *//*
   if( pixel_aspect_ratio_code == 15 ) {
    /* EPAR, 5.1.6 *//*
    par_width u(8)
    par_height u(8)
   }
  }
  if( ufep == 1 && custom_pcf == 1 ) {
   /* CPCFC, 5.1.7 *//*
   clock_conversion_code u(1)
   clock_divisor u(7)
  }
  if( custom_pcf_in_use )
   etr u(2) /* 5.1.8 *//*
  if( ufep == 1 && umv_mode == 1 ) {
   /* UUI, 5.1.9: "1", or "01" *//*
   uui u(1)
   if( uui == 0 )
    uui_unlimited u(1)
  }
  if( ufep == 1 && ss_mode == 1 ) {
   /* SSS, 5.1.10 *//*
   rectangular_slices u(1)
   arbitrary_slice_ordering u(1)
  }
  if( scalability_in_use ) {
   elnum u(4) /* 5.1.11 *//*
   if( ufep == 1 )
    rlnum u(4) /* 5.1.12 *//*
  }
  if( rps_in_use ) {
   if( ufep == 1 )
    rpsmf u(3) /* 5.1.13 *//*
   trpi u(1) /* 5.1.14 *//*
   if( trpi == 1 )
    trp u(10) /* 5.1.15 *//*
   /* BCI, 5.1.16: "1", a BCM following, or "01" *//*
   bci u(1)
   if( bci == 1 )
    bcm() /* 5.1.17, N.4.2 *//*
   else
    bci_end u(1)
  }
  if( rpr_mode == 1 )
   rprp() /* 5.1.18, P.2 *//*
 }
 pquant u(5) /* 5.1.19 *//*
 if( source_format != 7 ) {
  cpm u(1) /* 5.1.20: after PQUANT where PLUSPTYPE is not present *//*
  if( cpm == 1 )
   psbi u(2)
 }
 if( pb_frame ) {
  trb u(v) /* 5.1.22: 3 bits, or 5 with a custom picture clock frequency *//*
  dbquant u(2) /* 5.1.23 *//*
 }
 pei u(1) /* 5.1.24 *//*
 if( pei == 1 ) {
  do {
   psupp u(8) /* 5.1.25 *//*
   more_pei u(1)
  } while( more_pei_set )
 }
 picture_data() /* the GOBs or slices, ESTUF, EOS and PSTUF to the next picture start code *//*
}
    */
    public class PictureLayer : IItuSerializable
    {
		private uint psc;
		public uint Psc { get { return psc; } set { psc = value; } }
		private uint tr;
		public uint Tr { get { return tr; } set { tr = value; } }
		private byte ptype_start_code_emulation_bit;
		public byte PtypeStartCodeEmulationBit { get { return ptype_start_code_emulation_bit; } set { ptype_start_code_emulation_bit = value; } }
		private byte ptype_h261_distinction_bit;
		public byte PtypeH261DistinctionBit { get { return ptype_h261_distinction_bit; } set { ptype_h261_distinction_bit = value; } }
		private byte split_screen_indicator;
		public byte SplitScreenIndicator { get { return split_screen_indicator; } set { split_screen_indicator = value; } }
		private byte document_camera_indicator;
		public byte DocumentCameraIndicator { get { return document_camera_indicator; } set { document_camera_indicator = value; } }
		private byte full_picture_freeze_release;
		public byte FullPictureFreezeRelease { get { return full_picture_freeze_release; } set { full_picture_freeze_release = value; } }
		private uint source_format;
		public uint SourceFormat { get { return source_format; } set { source_format = value; } }
		private byte picture_coding_type;
		public byte PictureCodingType { get { return picture_coding_type; } set { picture_coding_type = value; } }
		private byte unrestricted_motion_vector_mode;
		public byte UnrestrictedMotionVectorMode { get { return unrestricted_motion_vector_mode; } set { unrestricted_motion_vector_mode = value; } }
		private byte syntax_based_arithmetic_coding_mode;
		public byte SyntaxBasedArithmeticCodingMode { get { return syntax_based_arithmetic_coding_mode; } set { syntax_based_arithmetic_coding_mode = value; } }
		private byte advanced_prediction_mode;
		public byte AdvancedPredictionMode { get { return advanced_prediction_mode; } set { advanced_prediction_mode = value; } }
		private byte pb_frames_mode;
		public byte PbFramesMode { get { return pb_frames_mode; } set { pb_frames_mode = value; } }
		private uint ufep;
		public uint Ufep { get { return ufep; } set { ufep = value; } }
		private uint opptype_source_format;
		public uint OpptypeSourceFormat { get { return opptype_source_format; } set { opptype_source_format = value; } }
		private byte custom_pcf;
		public byte CustomPcf { get { return custom_pcf; } set { custom_pcf = value; } }
		private byte umv_mode;
		public byte UmvMode { get { return umv_mode; } set { umv_mode = value; } }
		private byte sac_mode;
		public byte SacMode { get { return sac_mode; } set { sac_mode = value; } }
		private byte ap_mode;
		public byte ApMode { get { return ap_mode; } set { ap_mode = value; } }
		private byte aic_mode;
		public byte AicMode { get { return aic_mode; } set { aic_mode = value; } }
		private byte df_mode;
		public byte DfMode { get { return df_mode; } set { df_mode = value; } }
		private byte ss_mode;
		public byte SsMode { get { return ss_mode; } set { ss_mode = value; } }
		private byte rps_mode;
		public byte RpsMode { get { return rps_mode; } set { rps_mode = value; } }
		private byte isd_mode;
		public byte IsdMode { get { return isd_mode; } set { isd_mode = value; } }
		private byte aiv_mode;
		public byte AivMode { get { return aiv_mode; } set { aiv_mode = value; } }
		private byte mq_mode;
		public byte MqMode { get { return mq_mode; } set { mq_mode = value; } }
		private byte opptype_start_code_emulation_bit;
		public byte OpptypeStartCodeEmulationBit { get { return opptype_start_code_emulation_bit; } set { opptype_start_code_emulation_bit = value; } }
		private uint opptype_reserved_bits;
		public uint OpptypeReservedBits { get { return opptype_reserved_bits; } set { opptype_reserved_bits = value; } }
		private uint picture_type_code;
		public uint PictureTypeCode { get { return picture_type_code; } set { picture_type_code = value; } }
		private byte rpr_mode;
		public byte RprMode { get { return rpr_mode; } set { rpr_mode = value; } }
		private byte rru_mode;
		public byte RruMode { get { return rru_mode; } set { rru_mode = value; } }
		private byte rounding_type;
		public byte RoundingType { get { return rounding_type; } set { rounding_type = value; } }
		private uint mpptype_reserved_bits;
		public uint MpptypeReservedBits { get { return mpptype_reserved_bits; } set { mpptype_reserved_bits = value; } }
		private byte mpptype_start_code_emulation_bit;
		public byte MpptypeStartCodeEmulationBit { get { return mpptype_start_code_emulation_bit; } set { mpptype_start_code_emulation_bit = value; } }
		private byte cpm;
		public byte Cpm { get { return cpm; } set { cpm = value; } }
		private uint psbi;
		public uint Psbi { get { return psbi; } set { psbi = value; } }
		private uint pixel_aspect_ratio_code;
		public uint PixelAspectRatioCode { get { return pixel_aspect_ratio_code; } set { pixel_aspect_ratio_code = value; } }
		private uint picture_width_indication;
		public uint PictureWidthIndication { get { return picture_width_indication; } set { picture_width_indication = value; } }
		private byte cpfmt_start_code_emulation_bit;
		public byte CpfmtStartCodeEmulationBit { get { return cpfmt_start_code_emulation_bit; } set { cpfmt_start_code_emulation_bit = value; } }
		private uint picture_height_indication;
		public uint PictureHeightIndication { get { return picture_height_indication; } set { picture_height_indication = value; } }
		private uint par_width;
		public uint ParWidth { get { return par_width; } set { par_width = value; } }
		private uint par_height;
		public uint ParHeight { get { return par_height; } set { par_height = value; } }
		private byte clock_conversion_code;
		public byte ClockConversionCode { get { return clock_conversion_code; } set { clock_conversion_code = value; } }
		private uint clock_divisor;
		public uint ClockDivisor { get { return clock_divisor; } set { clock_divisor = value; } }
		private uint etr;
		public uint Etr { get { return etr; } set { etr = value; } }
		private byte uui;
		public byte Uui { get { return uui; } set { uui = value; } }
		private byte uui_unlimited;
		public byte UuiUnlimited { get { return uui_unlimited; } set { uui_unlimited = value; } }
		private byte rectangular_slices;
		public byte RectangularSlices { get { return rectangular_slices; } set { rectangular_slices = value; } }
		private byte arbitrary_slice_ordering;
		public byte ArbitrarySliceOrdering { get { return arbitrary_slice_ordering; } set { arbitrary_slice_ordering = value; } }
		private uint elnum;
		public uint Elnum { get { return elnum; } set { elnum = value; } }
		private uint rlnum;
		public uint Rlnum { get { return rlnum; } set { rlnum = value; } }
		private uint rpsmf;
		public uint Rpsmf { get { return rpsmf; } set { rpsmf = value; } }
		private byte trpi;
		public byte Trpi { get { return trpi; } set { trpi = value; } }
		private uint trp;
		public uint Trp { get { return trp; } set { trp = value; } }
		private byte bci;
		public byte Bci { get { return bci; } set { bci = value; } }
		private Bcm bcm;
		public Bcm Bcm { get { return bcm; } set { bcm = value; } }
		private byte bci_end;
		public byte BciEnd { get { return bci_end; } set { bci_end = value; } }
		private Rprp rprp;
		public Rprp Rprp { get { return rprp; } set { rprp = value; } }
		private uint pquant;
		public uint Pquant { get { return pquant; } set { pquant = value; } }
		private ulong trb;
		public ulong Trb { get { return trb; } set { trb = value; } }
		private uint dbquant;
		public uint Dbquant { get { return dbquant; } set { dbquant = value; } }
		private byte pei;
		public byte Pei { get { return pei; } set { pei = value; } }
		private Dictionary<int, uint> psupp;
		public Dictionary<int, uint> Psupp { get { return psupp ??= new Dictionary<int, uint>(); } set { psupp = value; } }
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
            H263Context ituContext = context as H263Context;
            if (ituContext == null)
                throw new ArgumentException($"Context should be of type H263Context");

            ulong size = 0;

			int whileIndex = -1;
/*  Transcribed from ITU-T Rec. H.263 (01/2005), 5.1 and Figures 7 and 8, which give the picture layer as prose: not
    the Recommendation's text. Each element is named after its abbreviation, the bits of PTYPE and PLUSPTYPE after
    what they are.  */

			size += stream.ReadUnsignedInt(size, 22, out this.psc, "psc"); 
/*  5.1.1  */

			size += stream.ReadUnsignedInt(size, 8, out this.tr, "tr"); 
/*  5.1.2  */

			size += stream.ReadUnsignedInt(size, 1, out this.ptype_start_code_emulation_bit, "ptype_start_code_emulation_bit"); 
/*  5.1.3: PTYPE bit 1, always 1  */

			size += stream.ReadUnsignedInt(size, 1, out this.ptype_h261_distinction_bit, "ptype_h261_distinction_bit"); 
/*  bit 2, always 0  */

			size += stream.ReadUnsignedInt(size, 1, out this.split_screen_indicator, "split_screen_indicator"); 
/*  bit 3  */

			size += stream.ReadUnsignedInt(size, 1, out this.document_camera_indicator, "document_camera_indicator"); 
/*  bit 4  */

			size += stream.ReadUnsignedInt(size, 1, out this.full_picture_freeze_release, "full_picture_freeze_release"); 
/*  bit 5  */

			size += stream.ReadUnsignedInt(size, 3, out this.source_format, "source_format"); 
			ituContext.OnSourceFormat(source_format);
/*  bits 6-8  */


			if ( source_format != 7 )
			{
				size += stream.ReadUnsignedInt(size, 1, out this.picture_coding_type, "picture_coding_type"); 
				ituContext.OnPictureCodingType(picture_coding_type);
/*  bit 9  */

				size += stream.ReadUnsignedInt(size, 1, out this.unrestricted_motion_vector_mode, "unrestricted_motion_vector_mode"); 
/*  bit 10, Annex D  */

				size += stream.ReadUnsignedInt(size, 1, out this.syntax_based_arithmetic_coding_mode, "syntax_based_arithmetic_coding_mode"); 
/*  bit 11, Annex E  */

				size += stream.ReadUnsignedInt(size, 1, out this.advanced_prediction_mode, "advanced_prediction_mode"); 
/*  bit 12, Annex F  */

				size += stream.ReadUnsignedInt(size, 1, out this.pb_frames_mode, "pb_frames_mode"); 
/*  bit 13, Annex G  */

			}
			else 
			{
/*  PLUSPTYPE, 5.1.4  */

				size += stream.ReadUnsignedInt(size, 3, out this.ufep, "ufep"); 
/*  5.1.4.1  */


				if ( ufep == 1 )
				{
/*  OPPTYPE, 5.1.4.2  */

					size += stream.ReadUnsignedInt(size, 3, out this.opptype_source_format, "opptype_source_format"); 
/*  bits 1-3  */

					size += stream.ReadUnsignedInt(size, 1, out this.custom_pcf, "custom_pcf"); 
/*  bit 4  */

					size += stream.ReadUnsignedInt(size, 1, out this.umv_mode, "umv_mode"); 
/*  bit 5, Annex D  */

					size += stream.ReadUnsignedInt(size, 1, out this.sac_mode, "sac_mode"); 
/*  bit 6, Annex E  */

					size += stream.ReadUnsignedInt(size, 1, out this.ap_mode, "ap_mode"); 
/*  bit 7, Annex F  */

					size += stream.ReadUnsignedInt(size, 1, out this.aic_mode, "aic_mode"); 
/*  bit 8, Annex I  */

					size += stream.ReadUnsignedInt(size, 1, out this.df_mode, "df_mode"); 
/*  bit 9, Annex J  */

					size += stream.ReadUnsignedInt(size, 1, out this.ss_mode, "ss_mode"); 
/*  bit 10, Annex K  */

					size += stream.ReadUnsignedInt(size, 1, out this.rps_mode, "rps_mode"); 
/*  bit 11, Annex N  */

					size += stream.ReadUnsignedInt(size, 1, out this.isd_mode, "isd_mode"); 
/*  bit 12, Annex R  */

					size += stream.ReadUnsignedInt(size, 1, out this.aiv_mode, "aiv_mode"); 
/*  bit 13, Annex S  */

					size += stream.ReadUnsignedInt(size, 1, out this.mq_mode, "mq_mode"); 
/*  bit 14, Annex T  */

					size += stream.ReadUnsignedInt(size, 1, out this.opptype_start_code_emulation_bit, "opptype_start_code_emulation_bit"); 
/*  bit 15, always 1  */

					size += stream.ReadUnsignedInt(size, 3, out this.opptype_reserved_bits, "opptype_reserved_bits"); 
					ituContext.OnOpptype(this);
/*  bits 16-18, 0  */

				}
/*  MPPTYPE, 5.1.4.3  */

				size += stream.ReadUnsignedInt(size, 3, out this.picture_type_code, "picture_type_code"); 
				ituContext.OnPictureTypeCode(picture_type_code);
/*  bits 1-3  */

				size += stream.ReadUnsignedInt(size, 1, out this.rpr_mode, "rpr_mode"); 
/*  bit 4, Annex P  */

				size += stream.ReadUnsignedInt(size, 1, out this.rru_mode, "rru_mode"); 
/*  bit 5, Annex Q  */

				size += stream.ReadUnsignedInt(size, 1, out this.rounding_type, "rounding_type"); 
/*  bit 6  */

				size += stream.ReadUnsignedInt(size, 2, out this.mpptype_reserved_bits, "mpptype_reserved_bits"); 
/*  bits 7-8, 0  */

				size += stream.ReadUnsignedInt(size, 1, out this.mpptype_start_code_emulation_bit, "mpptype_start_code_emulation_bit"); 
/*  bit 9, always 1  */

				size += stream.ReadUnsignedInt(size, 1, out this.cpm, "cpm"); 
/*  5.1.20: after PLUSPTYPE where it is present (5.1.4.7)  */


				if ( cpm == 1 )
				{
					size += stream.ReadUnsignedInt(size, 2, out this.psbi, "psbi"); 
				}
/*  5.1.21  */


				if ( ufep == 1 && opptype_source_format == 6 )
				{
/*  CPFMT, 5.1.5  */

					size += stream.ReadUnsignedInt(size, 4, out this.pixel_aspect_ratio_code, "pixel_aspect_ratio_code"); 
/*  bits 1-4  */

					size += stream.ReadUnsignedInt(size, 9, out this.picture_width_indication, "picture_width_indication"); 
/*  bits 5-13  */

					size += stream.ReadUnsignedInt(size, 1, out this.cpfmt_start_code_emulation_bit, "cpfmt_start_code_emulation_bit"); 
/*  bit 14, always 1  */

					size += stream.ReadUnsignedInt(size, 9, out this.picture_height_indication, "picture_height_indication"); 
					ituContext.OnCustomPictureFormat(picture_width_indication, picture_height_indication);
/*  bits 15-23  */


					if ( pixel_aspect_ratio_code == 15 )
					{
/*  EPAR, 5.1.6  */

						size += stream.ReadUnsignedInt(size, 8, out this.par_width, "par_width"); 
						size += stream.ReadUnsignedInt(size, 8, out this.par_height, "par_height"); 
					}
				}

				if ( ufep == 1 && custom_pcf == 1 )
				{
/*  CPCFC, 5.1.7  */

					size += stream.ReadUnsignedInt(size, 1, out this.clock_conversion_code, "clock_conversion_code"); 
					size += stream.ReadUnsignedInt(size, 7, out this.clock_divisor, "clock_divisor"); 
				}

				if ( (ituContext.CustomPcfInUse ? 1 : 0) != 0 )
				{
					size += stream.ReadUnsignedInt(size, 2, out this.etr, "etr"); 
				}
/*  5.1.8  */


				if ( ufep == 1 && umv_mode == 1 )
				{
/*  UUI, 5.1.9: "1", or "01"  */

					size += stream.ReadUnsignedInt(size, 1, out this.uui, "uui"); 

					if ( uui == 0 )
					{
						size += stream.ReadUnsignedInt(size, 1, out this.uui_unlimited, "uui_unlimited"); 
					}
				}

				if ( ufep == 1 && ss_mode == 1 )
				{
/*  SSS, 5.1.10  */

					size += stream.ReadUnsignedInt(size, 1, out this.rectangular_slices, "rectangular_slices"); 
					size += stream.ReadUnsignedInt(size, 1, out this.arbitrary_slice_ordering, "arbitrary_slice_ordering"); 
				}

				if ( (ituContext.ScalabilityInUse ? 1 : 0) != 0 )
				{
					size += stream.ReadUnsignedInt(size, 4, out this.elnum, "elnum"); 
/*  5.1.11  */


					if ( ufep == 1 )
					{
						size += stream.ReadUnsignedInt(size, 4, out this.rlnum, "rlnum"); 
					}
/*  5.1.12  */

				}

				if ( (ituContext.RpsInUse ? 1 : 0) != 0 )
				{

					if ( ufep == 1 )
					{
						size += stream.ReadUnsignedInt(size, 3, out this.rpsmf, "rpsmf"); 
					}
/*  5.1.13  */

					size += stream.ReadUnsignedInt(size, 1, out this.trpi, "trpi"); 
/*  5.1.14  */


					if ( trpi == 1 )
					{
						size += stream.ReadUnsignedInt(size, 10, out this.trp, "trp"); 
					}
/*  5.1.15  */

/*  BCI, 5.1.16: "1", a BCM following, or "01"  */

					size += stream.ReadUnsignedInt(size, 1, out this.bci, "bci"); 

					if ( bci == 1 )
					{
						this.bcm =  new Bcm() ;
						size +=  stream.ReadClass<Bcm>(size, context, this.bcm, "bcm"); // 5.1.17, N.4.2 
					}
					else 
					{
						size += stream.ReadUnsignedInt(size, 1, out this.bci_end, "bci_end"); 
					}
				}

				if ( rpr_mode == 1 )
				{
					this.rprp =  new Rprp() ;
					size +=  stream.ReadClass<Rprp>(size, context, this.rprp, "rprp"); // 5.1.18, P.2 
				}
			}
			size += stream.ReadUnsignedInt(size, 5, out this.pquant, "pquant"); 
/*  5.1.19  */


			if ( source_format != 7 )
			{
				size += stream.ReadUnsignedInt(size, 1, out this.cpm, "cpm"); 
/*  5.1.20: after PQUANT where PLUSPTYPE is not present  */


				if ( cpm == 1 )
				{
					size += stream.ReadUnsignedInt(size, 2, out this.psbi, "psbi"); 
				}
			}

			if ( ((source_format != 7 && pb_frames_mode == 1) || (source_format == 7 && picture_type_code == 2) ? 1 : 0) != 0 )
			{
				size += stream.ReadUnsignedIntVariable(size, (ituContext.CustomPcfInUse ? 5u : 3u), out this.trb, "trb"); 
/*  5.1.22: 3 bits, or 5 with a custom picture clock frequency  */

				size += stream.ReadUnsignedInt(size, 2, out this.dbquant, "dbquant"); 
/*  5.1.23  */

			}
			size += stream.ReadUnsignedInt(size, 1, out this.pei, "pei"); 
/*  5.1.24  */


			if ( pei == 1 )
			{

				do
				{
					whileIndex++;

					size += stream.ReadUnsignedInt(size, 8, whileIndex, (this.psupp ??= new()), "psupp"); 
/*  5.1.25  */

					size += stream.ReadUnsignedInt(size, 1, whileIndex, (this.more_pei ??= new()), "more_pei"); 
				} while ( (this.more_pei[whileIndex] == 1 ? 1 : 0) != 0 );
			}
			this.picture_data =  new PictureData() ;
			size +=  stream.ReadClass<PictureData>(size, context, this.picture_data, "picture_data"); // the GOBs or slices, ESTUF, EOS and PSTUF to the next picture start code 

            return size;
         }

         public ulong Write(IItuContext context, ItuStream stream)
         {
            H263Context ituContext = context as H263Context;
            if (ituContext == null)
                throw new ArgumentException($"Context should be of type H263Context");
            ulong size = 0;

			int whileIndex = -1;
/*  Transcribed from ITU-T Rec. H.263 (01/2005), 5.1 and Figures 7 and 8, which give the picture layer as prose: not
    the Recommendation's text. Each element is named after its abbreviation, the bits of PTYPE and PLUSPTYPE after
    what they are.  */

			size += stream.WriteUnsignedInt(22, this.psc, "psc"); 
/*  5.1.1  */

			size += stream.WriteUnsignedInt(8, this.tr, "tr"); 
/*  5.1.2  */

			size += stream.WriteUnsignedInt(1, this.ptype_start_code_emulation_bit, "ptype_start_code_emulation_bit"); 
/*  5.1.3: PTYPE bit 1, always 1  */

			size += stream.WriteUnsignedInt(1, this.ptype_h261_distinction_bit, "ptype_h261_distinction_bit"); 
/*  bit 2, always 0  */

			size += stream.WriteUnsignedInt(1, this.split_screen_indicator, "split_screen_indicator"); 
/*  bit 3  */

			size += stream.WriteUnsignedInt(1, this.document_camera_indicator, "document_camera_indicator"); 
/*  bit 4  */

			size += stream.WriteUnsignedInt(1, this.full_picture_freeze_release, "full_picture_freeze_release"); 
/*  bit 5  */

			size += stream.WriteUnsignedInt(3, this.source_format, "source_format"); 
			ituContext.OnSourceFormat(source_format);
/*  bits 6-8  */


			if ( source_format != 7 )
			{
				size += stream.WriteUnsignedInt(1, this.picture_coding_type, "picture_coding_type"); 
				ituContext.OnPictureCodingType(picture_coding_type);
/*  bit 9  */

				size += stream.WriteUnsignedInt(1, this.unrestricted_motion_vector_mode, "unrestricted_motion_vector_mode"); 
/*  bit 10, Annex D  */

				size += stream.WriteUnsignedInt(1, this.syntax_based_arithmetic_coding_mode, "syntax_based_arithmetic_coding_mode"); 
/*  bit 11, Annex E  */

				size += stream.WriteUnsignedInt(1, this.advanced_prediction_mode, "advanced_prediction_mode"); 
/*  bit 12, Annex F  */

				size += stream.WriteUnsignedInt(1, this.pb_frames_mode, "pb_frames_mode"); 
/*  bit 13, Annex G  */

			}
			else 
			{
/*  PLUSPTYPE, 5.1.4  */

				size += stream.WriteUnsignedInt(3, this.ufep, "ufep"); 
/*  5.1.4.1  */


				if ( ufep == 1 )
				{
/*  OPPTYPE, 5.1.4.2  */

					size += stream.WriteUnsignedInt(3, this.opptype_source_format, "opptype_source_format"); 
/*  bits 1-3  */

					size += stream.WriteUnsignedInt(1, this.custom_pcf, "custom_pcf"); 
/*  bit 4  */

					size += stream.WriteUnsignedInt(1, this.umv_mode, "umv_mode"); 
/*  bit 5, Annex D  */

					size += stream.WriteUnsignedInt(1, this.sac_mode, "sac_mode"); 
/*  bit 6, Annex E  */

					size += stream.WriteUnsignedInt(1, this.ap_mode, "ap_mode"); 
/*  bit 7, Annex F  */

					size += stream.WriteUnsignedInt(1, this.aic_mode, "aic_mode"); 
/*  bit 8, Annex I  */

					size += stream.WriteUnsignedInt(1, this.df_mode, "df_mode"); 
/*  bit 9, Annex J  */

					size += stream.WriteUnsignedInt(1, this.ss_mode, "ss_mode"); 
/*  bit 10, Annex K  */

					size += stream.WriteUnsignedInt(1, this.rps_mode, "rps_mode"); 
/*  bit 11, Annex N  */

					size += stream.WriteUnsignedInt(1, this.isd_mode, "isd_mode"); 
/*  bit 12, Annex R  */

					size += stream.WriteUnsignedInt(1, this.aiv_mode, "aiv_mode"); 
/*  bit 13, Annex S  */

					size += stream.WriteUnsignedInt(1, this.mq_mode, "mq_mode"); 
/*  bit 14, Annex T  */

					size += stream.WriteUnsignedInt(1, this.opptype_start_code_emulation_bit, "opptype_start_code_emulation_bit"); 
/*  bit 15, always 1  */

					size += stream.WriteUnsignedInt(3, this.opptype_reserved_bits, "opptype_reserved_bits"); 
					ituContext.OnOpptype(this);
/*  bits 16-18, 0  */

				}
/*  MPPTYPE, 5.1.4.3  */

				size += stream.WriteUnsignedInt(3, this.picture_type_code, "picture_type_code"); 
				ituContext.OnPictureTypeCode(picture_type_code);
/*  bits 1-3  */

				size += stream.WriteUnsignedInt(1, this.rpr_mode, "rpr_mode"); 
/*  bit 4, Annex P  */

				size += stream.WriteUnsignedInt(1, this.rru_mode, "rru_mode"); 
/*  bit 5, Annex Q  */

				size += stream.WriteUnsignedInt(1, this.rounding_type, "rounding_type"); 
/*  bit 6  */

				size += stream.WriteUnsignedInt(2, this.mpptype_reserved_bits, "mpptype_reserved_bits"); 
/*  bits 7-8, 0  */

				size += stream.WriteUnsignedInt(1, this.mpptype_start_code_emulation_bit, "mpptype_start_code_emulation_bit"); 
/*  bit 9, always 1  */

				size += stream.WriteUnsignedInt(1, this.cpm, "cpm"); 
/*  5.1.20: after PLUSPTYPE where it is present (5.1.4.7)  */


				if ( cpm == 1 )
				{
					size += stream.WriteUnsignedInt(2, this.psbi, "psbi"); 
				}
/*  5.1.21  */


				if ( ufep == 1 && opptype_source_format == 6 )
				{
/*  CPFMT, 5.1.5  */

					size += stream.WriteUnsignedInt(4, this.pixel_aspect_ratio_code, "pixel_aspect_ratio_code"); 
/*  bits 1-4  */

					size += stream.WriteUnsignedInt(9, this.picture_width_indication, "picture_width_indication"); 
/*  bits 5-13  */

					size += stream.WriteUnsignedInt(1, this.cpfmt_start_code_emulation_bit, "cpfmt_start_code_emulation_bit"); 
/*  bit 14, always 1  */

					size += stream.WriteUnsignedInt(9, this.picture_height_indication, "picture_height_indication"); 
					ituContext.OnCustomPictureFormat(picture_width_indication, picture_height_indication);
/*  bits 15-23  */


					if ( pixel_aspect_ratio_code == 15 )
					{
/*  EPAR, 5.1.6  */

						size += stream.WriteUnsignedInt(8, this.par_width, "par_width"); 
						size += stream.WriteUnsignedInt(8, this.par_height, "par_height"); 
					}
				}

				if ( ufep == 1 && custom_pcf == 1 )
				{
/*  CPCFC, 5.1.7  */

					size += stream.WriteUnsignedInt(1, this.clock_conversion_code, "clock_conversion_code"); 
					size += stream.WriteUnsignedInt(7, this.clock_divisor, "clock_divisor"); 
				}

				if ( (ituContext.CustomPcfInUse ? 1 : 0) != 0 )
				{
					size += stream.WriteUnsignedInt(2, this.etr, "etr"); 
				}
/*  5.1.8  */


				if ( ufep == 1 && umv_mode == 1 )
				{
/*  UUI, 5.1.9: "1", or "01"  */

					size += stream.WriteUnsignedInt(1, this.uui, "uui"); 

					if ( uui == 0 )
					{
						size += stream.WriteUnsignedInt(1, this.uui_unlimited, "uui_unlimited"); 
					}
				}

				if ( ufep == 1 && ss_mode == 1 )
				{
/*  SSS, 5.1.10  */

					size += stream.WriteUnsignedInt(1, this.rectangular_slices, "rectangular_slices"); 
					size += stream.WriteUnsignedInt(1, this.arbitrary_slice_ordering, "arbitrary_slice_ordering"); 
				}

				if ( (ituContext.ScalabilityInUse ? 1 : 0) != 0 )
				{
					size += stream.WriteUnsignedInt(4, this.elnum, "elnum"); 
/*  5.1.11  */


					if ( ufep == 1 )
					{
						size += stream.WriteUnsignedInt(4, this.rlnum, "rlnum"); 
					}
/*  5.1.12  */

				}

				if ( (ituContext.RpsInUse ? 1 : 0) != 0 )
				{

					if ( ufep == 1 )
					{
						size += stream.WriteUnsignedInt(3, this.rpsmf, "rpsmf"); 
					}
/*  5.1.13  */

					size += stream.WriteUnsignedInt(1, this.trpi, "trpi"); 
/*  5.1.14  */


					if ( trpi == 1 )
					{
						size += stream.WriteUnsignedInt(10, this.trp, "trp"); 
					}
/*  5.1.15  */

/*  BCI, 5.1.16: "1", a BCM following, or "01"  */

					size += stream.WriteUnsignedInt(1, this.bci, "bci"); 

					if ( bci == 1 )
					{
						size += stream.WriteClass<Bcm>(context, this.bcm, "bcm"); // 5.1.17, N.4.2 
					}
					else 
					{
						size += stream.WriteUnsignedInt(1, this.bci_end, "bci_end"); 
					}
				}

				if ( rpr_mode == 1 )
				{
					size += stream.WriteClass<Rprp>(context, this.rprp, "rprp"); // 5.1.18, P.2 
				}
			}
			size += stream.WriteUnsignedInt(5, this.pquant, "pquant"); 
/*  5.1.19  */


			if ( source_format != 7 )
			{
				size += stream.WriteUnsignedInt(1, this.cpm, "cpm"); 
/*  5.1.20: after PQUANT where PLUSPTYPE is not present  */


				if ( cpm == 1 )
				{
					size += stream.WriteUnsignedInt(2, this.psbi, "psbi"); 
				}
			}

			if ( ((source_format != 7 && pb_frames_mode == 1) || (source_format == 7 && picture_type_code == 2) ? 1 : 0) != 0 )
			{
				size += stream.WriteUnsignedIntVariable((ituContext.CustomPcfInUse ? 5u : 3u), this.trb, "trb"); 
/*  5.1.22: 3 bits, or 5 with a custom picture clock frequency  */

				size += stream.WriteUnsignedInt(2, this.dbquant, "dbquant"); 
/*  5.1.23  */

			}
			size += stream.WriteUnsignedInt(1, this.pei, "pei"); 
/*  5.1.24  */


			if ( pei == 1 )
			{

				do
				{
					whileIndex++;

					size += stream.WriteUnsignedInt(8, whileIndex, (this.psupp ??= new()), "psupp"); 
/*  5.1.25  */

					size += stream.WriteUnsignedInt(1, whileIndex, (this.more_pei ??= new()), "more_pei"); 
				} while ( (whileIndex + 1 < this.MorePei.Count ? 1 : 0) != 0 );
			}
			size += stream.WriteClass<PictureData>(context, this.picture_data, "picture_data"); // the GOBs or slices, ESTUF, EOS and PSTUF to the next picture start code 

            return size;
         }

    }

}
