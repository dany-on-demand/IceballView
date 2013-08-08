using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace IceballView
{
    #region structs
    public struct VXLVoxelData
    {
        public int x; public int y; public int z;
        public int r; public int g; public int b;
    }

    public struct VXLData
    {
        public PMFVoxelData[] data;
        public string map_name;
        public int version;
    }
    #endregion

    public class VXLRenderer : ARenderer
    {
        #region props
        private VXLData _map;

        public VXLData Map
        {
            get { return _map; }
            set { _map = value; }
        }
        #endregion

        public VXLRenderer(VXLData mapToRender, int width)
        {
            this.Map = mapToRender;
            this.Width = width;
            this.Img = this.RenderVXL(this.Map.data);
        }

        #region functions
        public Image RenderVXL(PMFVoxelData[] data)
        {
            try
            {
                Image img = new Bitmap(this.Width, this.Width);
                List<PMFVoxelData> lst = data.ToList();
                int center = this.Width / 2;
                //lst.Sort((vd1, vd2) => vd1.z.CompareTo(vd2.z));
                using (Graphics g = Graphics.FromImage(img))
                {
                    foreach (PMFVoxelData vd in lst)
                    {
                        g.FillRectangle(new SolidBrush(Color.FromArgb(vd.r, vd.g, vd.b)), vd.x + center - vd.rad / 2, vd.y + center - vd.rad / 2, vd.rad, vd.rad);
                    }
                }

                return img;
            }
            catch
            {
                return Properties.Resources.nopreview;
            }
        }

        public static PMFData LoadVXL(byte[] file)
        {
            try
            {
                PMFData result = new PMFData();
                using (BinaryReader reader = new BinaryReader(new MemoryStream(file)))
                {
                    if (!IsVXLFormat(reader.ReadBytes(8)))
                    {
                        MessageBox.Show("File is not PMF1 format or is damaged");
                        throw new IOException();
                    }
                    result.version = 1;

                    UInt32 number_of_bones = reader.ReadUInt32();

                    if (number_of_bones > 256)
                    {
                        MessageBox.Show("Model has too many bones, conversion failed");
                        throw new IOException();
                    }
                    result.bones = new PMFBone[number_of_bones];

                    for (int i = 0; i < number_of_bones; i++)
                    {
                        var bone_name = reader.ReadChars(16);

                        if (bone_name[15] != '\0')
                        {
                            MessageBox.Show("Model bone name is not null-terminated!");
                            throw new IOException();
                        }

                        UInt32 point_count = reader.ReadUInt32();

                        PMFVoxelData[] vxl_data = new PMFVoxelData[point_count];

                        for (int j = 0; j < point_count; j++)
                        {
                            vxl_data[j].rad = Convert.ToInt32(BitConverter.ToUInt16(reader.ReadBytes(2), 0));
                            vxl_data[j].x = Convert.ToInt32(BitConverter.ToInt16(reader.ReadBytes(2), 0));
                            vxl_data[j].y = Convert.ToInt32(BitConverter.ToInt16(reader.ReadBytes(2), 0));
                            vxl_data[j].z = Convert.ToInt32(BitConverter.ToInt16(reader.ReadBytes(2), 0));
                            vxl_data[j].b = Convert.ToInt32(reader.ReadByte()); //no sanity check there
                            vxl_data[j].g = Convert.ToInt32(reader.ReadByte()); //as anything out of range will
                            vxl_data[j].r = Convert.ToInt32(reader.ReadByte()); //fail on render
                            reader.Read();
                        }

                        //finally fill in the info
                        result.bones[i].name = new string(bone_name);
                        result.bones[i].number_of_points = point_count;
                        result.bones[i].data = vxl_data;
                    }

                    //MessageBox.Show("Done!");
                }
                return result;
            }
            catch { return new PMFData(); }
        }

        public static bool IsVXLFormat(byte[] header)
        {
            if (!header.SequenceEqual( new byte[] { 0x50, 0x4D, 0x46, 0x1A, 0x01, 0x00, 0x00, 0x00 } ))
            {
                return false;
            }
            return true;
        }
        #endregion
    }
}
