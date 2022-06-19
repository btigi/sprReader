using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace sprReader
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void btnOpen_Click(object sender, EventArgs e)
        {
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                var converter = new sprConverter();
                try
                {
                    converter.Palette = Image.FromFile(tbPalette.Text) as Bitmap;
                    converter.Open(openFileDialog.FileName);
                    converter.Parse();
                }
                catch (Exception ex)
                {
                    rtOutput.Text += ex.Message;
                    System.Diagnostics.Trace.Write(ex.ToString());
                }
                finally
                {
                    converter.Close();
                }
            }
        }

        private void btnBulk_Click(object sender, EventArgs e)
        {
            var converter = new sprConverter();
            var txt = new StringBuilder();
            try
            {
                converter.Palette = Image.FromFile(tbPalette.Text) as Bitmap;

                var sprFiles = new List<string>(Directory.EnumerateFiles(tbFolder.Text, "*.spr", SearchOption.AllDirectories));
                progressBar.Value = 0;
                progressBar.Maximum = sprFiles.Count;
                foreach (var sprFile in sprFiles)
                {
                    try
                    {
                        txt.AppendLine(sprFile);
                        converter.Open(sprFile);
                        converter.Parse();
                        converter.Close();
                    }
                    catch (Exception ex)
                    {
                        txt.AppendLine($"   {ex.Message}");
                    }
                    finally
                    {
                        converter?.Close();
                        progressBar.PerformStep();
                        txt.AppendLine($"   Complete");
                    }
                }
            }
            finally
            {
                rtOutput.Text = txt.ToString();
            }
        }
    }
}