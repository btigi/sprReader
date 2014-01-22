using System;
using System.Drawing;
using System.Windows.Forms;

namespace sprReader
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var converter = new sprConverter();
            try
            {
                converter.Palette = Image.FromFile(@"D:\tmp\Dark Reign\def_pal.bmp") as Bitmap;
                converter.Open(@"D:\tmp\Dark Reign\ecsmosm1.spr");
                //converter.Open(@"D:\tmp\Dark Reign\uowtrst0.spr");
                //converter.Open(@"D:\tmp\Dark Reign\aoctr007.spr");
                //converter.Open(@"D:\tmp\Dark Reign\aoclf000.spr");
                converter.Parse();

                //var sprFiles = new List<string>(Directory.EnumerateFiles(@"D:\tmp\Dark Reign\", "*.spr", SearchOption.AllDirectories));
                //foreach (var sprFile in sprFiles)
                //{
                //    converter.Open(sprFile);
                //    converter.Parse();
                //}
            }
            catch (Exception ex)
            {
                rtOutput.Text += System.Environment.NewLine + converter.filename;
                System.Diagnostics.Trace.Write(ex.ToString());
            }
            finally
            {
                converter.Close();
            }
        }
    }
}