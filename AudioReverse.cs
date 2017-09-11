using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace AudioReverse
{
    public partial class Form1 : Form
    {
        string filename = "";
        string audiofile
        {
            get
            {
                return filename;
            }
            set
            {
                filename = value;
                textBox1.Text = filename;
            }
        }


        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (audiofile != "")
            {
                byte[] bytes = File.ReadAllBytes(audiofile);
                byte[] audioBytes = new byte[bytes.Length - 44];

                for (int i = 0; i < audioBytes.Length; i++)
                {
                    audioBytes[i] = bytes[i + 44];
                }

                int sampleSize = BitConverter.ToInt16(new byte[] { bytes[34], bytes[35] }, 0);
                int numOfChannels = BitConverter.ToInt16(new byte[] { bytes[22], bytes[23] }, 0);

                if (numOfChannels == 2)
                {

                    byte[] reversedBytes = new byte[audioBytes.Length];

                    for (int i = 0; i < reversedBytes.Length - 3; i++)
                    {
                        reversedBytes[i] = audioBytes[audioBytes.Length - 1 - i - 3];
                        reversedBytes[i + 1] = audioBytes[audioBytes.Length - 1 - i - 2];
                        reversedBytes[i + 2] = audioBytes[audioBytes.Length - 1 - i - 1];
                        reversedBytes[i + 3] = audioBytes[audioBytes.Length - 1 - i];
                    }

                    for (int i = 0; i < reversedBytes.Length; i++)
                    {
                        bytes[i + 44] = reversedBytes[i];
                    }

                    string name = audiofile.Substring(audiofile.LastIndexOf("\\") + 1);
                    name = name.Substring(0, name.IndexOf(".wav"));
                    name += "_reversed.wav";
                    string fullnewpath = audiofile.Substring(0, audiofile.LastIndexOf("\\") + 1) + name;
                    File.WriteAllBytes(fullnewpath, bytes);
                }
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            AllowDrop = true;
            DragEnter += Form1_DragEnter;
            DragDrop += Form1_DragDrop;

        }

        private void Form1_DragDrop(object sender, DragEventArgs e)
        {
            int X = e.X - Location.X;
            int Y = e.Y - Location.Y;

            bool b1 = X > textBox1.Location.X;
            bool b2 = X < textBox1.Location.X + textBox1.Width;
            bool b3 = Y > textBox1.Location.Y - 40;
            bool b4 = Y < textBox1.Location.Y + textBox1.Height + 40;

            if (b1 && b2 && b3 && b4)
            {
                if(e.Data.GetDataPresent(DataFormats.FileDrop))
                {
                    string[] filepaths = (string[])e.Data.GetData(DataFormats.FileDrop);
                    if (File.Exists(filepaths[0]))                    
                        audiofile = filepaths[0];                 
                }
            }
        }

        private void Form1_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Effect == DragDropEffects.None)
                e.Effect = DragDropEffects.Copy;
        }
    }
}
