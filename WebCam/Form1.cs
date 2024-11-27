using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using AForge.Video;
using AForge.Video.DirectShow;


using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace WebCam
{
    public partial class Form1 : Form
    {
        private FilterInfoCollection videoDevices;
        private VideoCaptureDevice videoSource;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // Detect all video devices
            videoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);

            // Populate ComboBox with video device names
            foreach (FilterInfo device in videoDevices)
            {
                comboBox1.Items.Add(device.Name);
            }

            // Select the first device by default
            if (comboBox1.Items.Count > 0)
            {
                comboBox1.SelectedIndex = 0;
            }
            else
            {
                MessageBox.Show("No video devices found.");
            }
        }

        private void StartCamera()
        {
            if (videoSource != null && videoSource.IsRunning)
            {
                videoSource.SignalToStop();
                videoSource.WaitForStop();
            }

            // Start the selected video device
            int selectedIndex = comboBox1.SelectedIndex;
            if (selectedIndex >= 0)
            {
                videoSource = new VideoCaptureDevice(videoDevices[selectedIndex].MonikerString);
                videoSource.NewFrame += VideoSource_NewFrame;
                videoSource.Start();
            }
            else
            {
                MessageBox.Show("Please select a video device.");
            }
        }

        private void StopCamera()
        {
            if (videoSource != null && videoSource.IsRunning)
            {
                videoSource.SignalToStop();
                videoSource.WaitForStop();
                videoSource.NewFrame -= VideoSource_NewFrame;
            }
        }

        private void VideoSource_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            // Display the captured frame in the PictureBox
            Bitmap frame = (Bitmap)eventArgs.Frame.Clone();
            pictureBox1.Image?.Dispose();
            pictureBox1.Image = frame;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            StopCamera();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            StartCamera();
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            StopCamera();
            base.OnFormClosing(e);
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (pictureBox1.Image != null)
            {
                // Open a SaveFileDialog to specify where to save the image
                using (SaveFileDialog saveFileDialog = new SaveFileDialog())
                {
                    saveFileDialog.Filter = "JPEG Image|*.jpg|PNG Image|*.png|Bitmap Image|*.bmp";
                    saveFileDialog.Title = "Save Captured Image";
                    saveFileDialog.FileName = "CapturedImage";

                    if (saveFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        // Save the image in the selected format
                        var format = System.Drawing.Imaging.ImageFormat.Jpeg;

                        if (saveFileDialog.FilterIndex == 2)
                        {
                            format = System.Drawing.Imaging.ImageFormat.Png;
                        }
                        else if (saveFileDialog.FilterIndex == 3)
                        {
                            format = System.Drawing.Imaging.ImageFormat.Bmp;
                        }

                        pictureBox1.Image.Save(saveFileDialog.FileName, format);
                        MessageBox.Show("Image saved successfully!");
                    }
                }
            }
            else
            {
                MessageBox.Show("No image to save!");
            }
        }
    }
}
