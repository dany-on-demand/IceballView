using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace IceballView
{
    #region structs
    public struct PMFRenderData
    {
        public Rectangle[] rct;
        public Brush color; //making a cache/palette of colours turned out to be slower
    }

    public struct PMFVoxelData
    {
        public int x; public int y; public int z;
        public int r; public int g; public int b;
        public int rad;
    }

    public struct PMFBone
    {
        public PMFVoxelData[] data;
        public int width;
        public int height;
        public string name; //public string char[15] name
        public UInt32 number_of_points; //I don't use this?
    }

    public struct PMFData
    {
        public PMFBone[] bones;
        public string model_name; //hmm?
        public int version; //render PMF version on the bottom of the thumbnail?
    }
    #endregion

    public class PMFRenderer : ARenderer
    {
        #region props
        private PMFData _model;

        public PMFData Model
        {
            get { return _model; }
            set { _model = value; }
        }
        #endregion

        public PMFRenderer(PMFData modelToRender, int width)
        {
            this.Model = modelToRender;
            this.Width = width;
            this.Img = new Bitmap(width, width);
            using (Graphics g = Graphics.FromImage(this.Img))
            {
                int bone_count = this.Model.bones.Length;
                if (bone_count == 1)
                {
                    this.Img = Task.Factory.StartNew(() => this.RenderPMFBone(this.Model.bones[0])).Result;
                }
                else if (bone_count == 2)
                {
                    Image bone1 = Task.Factory.StartNew(() => this.RenderPMFBone(this.Model.bones[0], 2)).Result;
                    Image bone2 = Task.Factory.StartNew(() => this.RenderPMFBone(this.Model.bones[1], 2)).Result;
                    g.DrawImage(bone1, Point.Empty);
                    g.DrawImage(bone2, new Point(this.Width / 2, 0));
                }
                else if (bone_count == 3)
                {
                    Image bone1 = Task.Factory.StartNew(() => this.RenderPMFBone(this.Model.bones[0], 2, 2)).Result;
                    Image bone2 = Task.Factory.StartNew(() => this.RenderPMFBone(this.Model.bones[1], 2, 2)).Result;
                    Image bone3 = Task.Factory.StartNew(() => this.RenderPMFBone(this.Model.bones[2], 1, 2)).Result;
                    g.DrawImage(bone1, Point.Empty);
                    g.DrawImage(bone2, new Point(this.Width / 2, 0));
                    g.DrawImage(bone3, new Point(0, this.Width / 2));
                }
                else
                {
                    Image bone1 = Task.Factory.StartNew(() => this.RenderPMFBone(this.Model.bones[0], 2, 2)).Result;
                    Image bone2 = Task.Factory.StartNew(() => this.RenderPMFBone(this.Model.bones[1], 2, 2)).Result;
                    Image bone3 = Task.Factory.StartNew(() => this.RenderPMFBone(this.Model.bones[2], 2, 2)).Result;
                    Image bone4 = Task.Factory.StartNew(() => this.RenderPMFBone(this.Model.bones[2], 2, 2)).Result;
                    g.DrawImage(bone1, Point.Empty);
                    g.DrawImage(bone2, new Point(this.Width / 2, 0));
                    g.DrawImage(bone3, new Point(0, this.Width / 2));
                    g.DrawImage(bone3, new Point(this.Width / 2, this.Width / 2));
                }
            }
        }

        #region functions
        public Image RenderPMFBone(PMFBone bone, int widthFactor = 1, int heightFactor = 1)
        {
            try
            {
                Bitmap img = new Bitmap(bone.width, bone.height);
                List<PMFVoxelData> lst = bone.data.ToList();
                lst = lst.AsParallel().OrderByDescending(vxl => vxl.z).ThenByDescending(vxl => vxl.y).ThenBy(vxl => vxl.x).ToList();
                using (Graphics g = Graphics.FromImage(img))
                {
                    int ptsize = 0;
                    int cubeScaleFactor = 8;
                    Point[] voxelTop = new Point[] {
                                new Point(0, 1  * cubeScaleFactor),
                                new Point(1 * cubeScaleFactor, 0),
                                new Point(4 * cubeScaleFactor, 0),
                                new Point(3 * cubeScaleFactor, 1 * cubeScaleFactor)
                                };
                    //Point[] voxelFront = new Point[] //it's a rectangle now
                    //            {
                    //            new Point(0, 1 * fac),
                    //            new Point(3 * fac, 1 * fac),
                    //            new Point(3 * fac, 4 * fac),
                    //            new Point(0, 4 * fac)
                    //            };
                    Point[] voxelSide = new Point[]
                                {
                                new Point(4 * cubeScaleFactor, 0),
                                new Point(4 * cubeScaleFactor, 3 * cubeScaleFactor),
                                new Point(3 * cubeScaleFactor, 4 * cubeScaleFactor),
                                new Point(3 * cubeScaleFactor, 1 * cubeScaleFactor)
                                };
                    Bitmap imgCube;
                    foreach (PMFVoxelData vd in lst)
                    {
                        Brush vdcolbrush = new SolidBrush(Color.FromArgb(vd.r, vd.g, vd.b));
                        ptsize = (vd.rad * 2);
                        //fake front rendering
                        //g.FillRectangle(new SolidBrush(Color.FromArgb(vd.r, vd.g, vd.b)), vd.x + centerOffset - vd.rad, vd.y + centerOffset - vd.rad, vd.rad * 2, vd.rad * 2);
                        if (ptsize <= 6)
                        {
                            //fake iso rendering
                            g.FillRectangle(vdcolbrush,
                                ((int)(vd.x * 0.75)) + bone.width / 2 - vd.rad / 2 + (int)(vd.z * 0.25), ((int)(vd.y * 0.75)) + bone.height / 2 - vd.rad / 2 - (int)(vd.z * 0.25), ptsize, ptsize);
                        }
                        else
                        {
                            imgCube = new Bitmap(4 * cubeScaleFactor, 4 * cubeScaleFactor);
                            using (Graphics cubeGfx = Graphics.FromImage(imgCube))
                            {
                                cubeGfx.FillPolygon(new SolidBrush(
                                    Color.FromArgb((int)Math.Min((vd.r + 2) * 1.5, 255), (int)Math.Min((vd.g + 2) * 1.5, 255), (int)Math.Min((vd.b + 2) * 1.5, 255))),
                                    voxelTop
                                );

                                //gfx.FillPolygon(new SolidBrush(Color.FromArgb(vd.r, vd.g, vd.b)), voxelFront);
                                cubeGfx.FillRectangle(vdcolbrush, 0, 1 * cubeScaleFactor, 3 * cubeScaleFactor, 4 * cubeScaleFactor);

                                cubeGfx.FillPolygon(new SolidBrush(Color.FromArgb(vd.r / 2, vd.g / 2, vd.b / 2)), voxelSide);
                            }
                            //fake voxel iso rendering
                            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
                            g.DrawImage(imgCube, new Rectangle(
                                new Point(((int)(vd.x * 0.75)) + bone.width / 2 - vd.rad + (int)(vd.z * 0.25),
                                    ((int)(vd.y * 0.75)) + bone.height / 2 - vd.rad - (int)(vd.z * 0.25)),
                                    new Size(ptsize, ptsize)),
                                new Rectangle(Point.Empty, new Size(4 * cubeScaleFactor, 4 * cubeScaleFactor)),
                                GraphicsUnit.Pixel);
                        }
                    }
                }
                GC.Collect();
                return BitmapProcessing.ResizeImage(BitmapProcessing.CropBitmap(img), this.Width / widthFactor, this.Width / heightFactor);
            }
            catch { return Properties.Resources.nopreview; }
        }

        public static PMFData LoadPMF(byte[] file, string name = "")
        {
            try
            {
                PMFData result = new PMFData();
                result.model_name = name;
                using (BinaryReader reader = new BinaryReader(new MemoryStream(file)))
                {
                    if (!IsPMFFormat(reader.ReadBytes(8)))
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

                        UInt32 point_count = BitConverter.ToUInt32(reader.ReadBytes(4), 0);

                        PMFVoxelData[] vxl_data = new PMFVoxelData[point_count];

                        int min_x = int.MaxValue;
                        int min_y = int.MaxValue;
                        int max_x = 0;
                        int max_y = 0;
                        int max_rad = 0;
                        //List<Color> palette = new List<Color>();

                        //string a = string.Empty;
                        for (int j = 0; j < point_count; j++)
                        {
                            vxl_data[j].rad = Convert.ToInt32(BitConverter.ToUInt16(reader.ReadBytes(2), 0));
                            vxl_data[j].x = Convert.ToInt32(BitConverter.ToInt16(reader.ReadBytes(2), 0));
                            vxl_data[j].y = Convert.ToInt32(BitConverter.ToInt16(reader.ReadBytes(2), 0));
                            vxl_data[j].z = Convert.ToInt32(BitConverter.ToInt16(reader.ReadBytes(2), 0));

                            if (vxl_data[j].rad > max_rad) { max_rad = vxl_data[j].rad; }
                            if (vxl_data[j].x < min_x) { min_x = vxl_data[j].x; }
                            if (vxl_data[j].y < min_y) { min_y = vxl_data[j].y; }
                            if (vxl_data[j].x > max_x) { max_x = vxl_data[j].x; }
                            if (vxl_data[j].y > max_y) { max_y = vxl_data[j].y; }

                            //a += String.Format("voxel No. {0} x: {1} y: {2} z: {3} rad: {4}", j + 1, vxl_data[j].x, vxl_data[j].y, vxl_data[j].z, vxl_data[j].rad) + Environment.NewLine;
                            vxl_data[j].b = Convert.ToInt32(reader.ReadByte()); //no sanity check there
                            vxl_data[j].g = Convert.ToInt32(reader.ReadByte()); //as anything out of range will
                            vxl_data[j].r = Convert.ToInt32(reader.ReadByte()); //fail on render
                            reader.ReadByte(); //skip over the "reserved" thing
                        }
                        //Debug.Print(a);

                        //finally fill in the info
                        result.bones[i].data = vxl_data;
                        result.bones[i].width = 2 * (Math.Abs(max_x - min_x) + max_rad * 2);
                        result.bones[i].height = 2 * (Math.Abs(max_y - min_y) + max_rad * 2);
                        result.bones[i].name = new string(bone_name);
                        result.bones[i].number_of_points = point_count;
                    }

                    //MessageBox.Show("Done!");
                }
                return result;
            }
            catch (Exception e) { MessageBox.Show(e.Message); return new PMFData(); }
        }

        public static bool IsPMFFormat(byte[] header)
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
