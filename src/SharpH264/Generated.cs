using System;
using System.Linq;
using System.Collections.Generic;

namespace SharpH264
{

    public class NalUnit : IItuSerializable
    {
        private bool forbidden_zero_bit;
        public bool ForbiddenZeroBit { get { return forbidden_zero_bit; } set { forbidden_zero_bit = value; } }
        private uint nal_ref_idc;
        public uint NalRefIdc { get { return nal_ref_idc; } set { nal_ref_idc = value; } }
        private byte nal_unit_type;
        public byte NalUnitType { get { return nal_unit_type; } set { nal_unit_type = value; } }
        private bool svc_extension_flag;
        public bool SvcExtensionFlag { get { return svc_extension_flag; } set { svc_extension_flag = value; } }
        private bool avc_3d_extension_flag;
        public bool Avc3dExtensionFlag { get { return avc_3d_extension_flag; } set { avc_3d_extension_flag = value; } }
        private NalUnitHeaderSvcExtension nal_unit_header_svc_extension;
        public NalUnitHeaderSvcExtension NalUnitHeaderSvcExtension { get { return nal_unit_header_svc_extension; } set { nal_unit_header_svc_extension = value; } }
        private NalUnitHeader3davcExtension nal_unit_header_3davc_extension;
        public NalUnitHeader3davcExtension NalUnitHeader3davcExtension { get { return nal_unit_header_3davc_extension; } set { nal_unit_header_3davc_extension = value; } }
        private NalUnitHeaderMvcExtension nal_unit_header_mvc_extension;
        public NalUnitHeaderMvcExtension NalUnitHeaderMvcExtension { get { return nal_unit_header_mvc_extension; } set { nal_unit_header_mvc_extension = value; } }
        private byte[] rbsp_byte;
        public byte[] RbspByte { get { return rbsp_byte; } set { rbsp_byte = value; } }
        private byte emulation_prevention_three_byte;
        public byte EmulationPreventionThreeByte { get { return emulation_prevention_three_byte; } set { emulation_prevention_three_byte = value; } }
        private uint numBytesInNALunit;
        public uint NumBytesInNALunit { get { return numBytesInNALunit; } set { numBytesInNALunit = value; } }

        public NalUnit(uint NumBytesInNALunit)
        {
            this.numBytesInNALunit = NumBytesInNALunit;
        }

        public ulong Read(ItuStream stream)
        {
            ulong size = 0;

            size += stream.ReadFixed(size, 1, out this.forbidden_zero_bit);
            size += stream.ReadUnsignedInt(size, 2, out this.nal_ref_idc);
            size += stream.ReadUnsignedInt(size, 5, out this.nal_unit_type);
            var NumBytesInRBSP = 0;
            var nalUnitHeaderBytes = 1;

            if (nal_unit_type == 14 || nal_unit_type == 20 || nal_unit_type == 21)
            {

                if (nal_unit_type != 21)
                {
                    size += stream.ReadUnsignedInt(size, 1, out this.svc_extension_flag);
                }

                else
                {
                    size += stream.ReadUnsignedInt(size, 1, out this.avc_3d_extension_flag);
                }

                if (svc_extension_flag)
                {
                    size += stream.ReadClass<NalUnitHeaderSvcExtension>(size, out this.nal_unit_header_svc_extension); // specified in Annex G 
                    nalUnitHeaderBytes += 3;
                }

                else if (avc_3d_extension_flag)
                {
                    size += stream.ReadClass<NalUnitHeader3davcExtension>(size, out this.nal_unit_header_3davc_extension); // specified in Annex J 
                    nalUnitHeaderBytes += 2;
                }

                else
                {
                    size += stream.ReadClass<NalUnitHeaderMvcExtension>(size, out this.nal_unit_header_mvc_extension); // specified in Annex H 
                    nalUnitHeaderBytes += 3;
                }
            }

            this.rbsp_byte = new byte[NumBytesInNALunit];
            for (int i = nalUnitHeaderBytes; i < NumBytesInNALunit; i++)
            {

                if (i + 2 < NumBytesInNALunit && stream.NextBits(24) == 0x000003)
                {
                    size += stream.ReadBits(size, 8, out this.rbsp_byte[NumBytesInRBSP++]);
                    size += stream.ReadBits(size, 8, out this.rbsp_byte[NumBytesInRBSP++]);
                    i += 2;
                    size += stream.ReadFixed(size, 8, out this.emulation_prevention_three_byte); // equal to 0x03 
                }

                else
                {
                    size += stream.ReadBits(size, 8, out this.rbsp_byte[NumBytesInRBSP++]);
                }
            }

            return size;
        }

    }

}
