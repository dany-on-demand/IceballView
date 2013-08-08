using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace IceballView
{
    public partial class FormRender : Form
    {
        public ARenderer renderer { get; set; }

        public FormRender()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.renderer = new PMFRenderer(
                    Task.Factory.StartNew(() => PMFRenderer.LoadPMF(Properties.Resources.nade)).Result, 64);
            this.renderPictureBox.Image = this.renderer.Img;
        }

        private void Form1_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = DragDropEffects.Copy; // Okay
            else
                e.Effect = DragDropEffects.None;
        }

        private void Form1_DragDrop(object sender, DragEventArgs e)
        {
            var filepaths = ((string[])e.Data.GetData(DataFormats.FileDrop, false)).ToList();
            foreach (string fpath in filepaths)
            {
                FormRender formToWorkWith;
                if (filepaths.IndexOf(fpath) == 0)
                {
                    formToWorkWith = this;
                }
                else
                {
                    formToWorkWith = new FormRender();
                    formToWorkWith.Show();
                }
                var file = File.Open(fpath, FileMode.Open);
                if (PMFRenderer.IsPMFFormat((new BinaryReader(file)).ReadBytes(8)))
                {
                    file.Close();
                    formToWorkWith.renderer = new PMFRenderer(
                        Task.Factory.StartNew(() => PMFRenderer.LoadPMF(File.ReadAllBytes(fpath), Path.GetFileNameWithoutExtension(fpath))).Result,
                        512);
                    formToWorkWith.renderPictureBox.Image = formToWorkWith.renderer.Img;
                    formToWorkWith.Text = Path.GetFileNameWithoutExtension(fpath);
                }
                else if (VXLRenderer.IsVXLFormat((new BinaryReader(file)).ReadBytes(8)))
                {
                    formToWorkWith.renderPictureBox.Image = Properties.Resources.nopreview;
                }
                else
                {
                    formToWorkWith.renderPictureBox.Image = Properties.Resources.nopreview;
                }
                file.Close();
            }
        }
    }
}
