using System;
using System.Windows.Forms;
using Composition.Classes;
using System.Threading;
using System.IO;

namespace Composition
{
    public partial class MainForm : Form
    {
        //сцена
        Scene scene;

        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            DirectLight.Checked = false;
        }

        private void OpenMenuItem_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                scene = new Scene(pictureBox1.Width, pictureBox1.Height, openFileDialog1.FileName);
                scene.Tik += Scene_Tik;
                this.Text = openFileDialog1.SafeFileName;
                scene.SetDirectLight(DirectLight.Checked);
                scene.CountLight = (int)numericUpDown1.Value;
                scene.Power = (float)numericUpDown2.Value;
                Camera buf = scene.GetCamera();
                new Thread(
                    delegate() { 
                        pictureBox1.Image = scene.Render();
                    }
                    ).Start(); 
                pictureBox1.Invalidate();
            }
        }

        private void Scene_Tik(object sender, EventArgs e)
        {
            label7.Invoke(new Action ( () => label7.Text = "Выполнено на " + ((float)sender).ToString("0.0") + "%"));
        }


        private void RefreshMenuItem_Click(object sender, EventArgs e)
        {
            if (scene != null)
            {
                try
                {
                    float x = Convert.ToSingle(textBox2.Text);
                    float y = Convert.ToSingle(textBox3.Text);
                    float z = Convert.ToSingle(textBox4.Text);
                    float x0 = Convert.ToSingle(textBox6.Text);
                    float y0 = Convert.ToSingle(textBox5.Text);
                    float z0 = Convert.ToSingle(textBox1.Text);
                    scene.SetCamera(x, y, z, x0, y0, z0, pictureBox1.Width, pictureBox1.Height, 5);
                    scene.SetDirectLight(DirectLight.Checked);
                    scene.CountLight = (int)numericUpDown1.Value;
                    scene.Power = (float)numericUpDown2.Value;
                    new Thread(
                    delegate () {
                        pictureBox1.Image = scene.Render();
                    }
                    ).Start();
                    pictureBox1.Invalidate();
                }
                catch
                {
                    MessageBox.Show("Неверный формат");
                }
            }
        }

        private void RandomRayMenuItem_Click(object sender, EventArgs e)
        {
            if (scene != null)
            {
                scene.SetDirectLight(DirectLight.Checked);
                scene.CountLight = (int)numericUpDown1.Value;
                scene.Power = (float)numericUpDown2.Value;

                new Thread(
                    delegate () {
                        pictureBox1.Image = scene.Render();
                    }
                    ).Start();
                pictureBox1.Invalidate();
            }
        }

        private void SaveImageMenuItem_Click(object sender, EventArgs e)
        {
            string directory = Directory.GetCurrentDirectory() + "/image/";
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
                
            }
            int i = 1;
            string fileImageName = "";
            string fileTestName = scene.GetFileName();
            string format = "png";
            
            while (true)
            {
                fileImageName = directory + fileTestName + "_" + i.ToString() + "." + format;
                if (!File.Exists(fileImageName)) break;
                i++;
            }
            
            pictureBox1.Image.Save(fileImageName, System.Drawing.Imaging.ImageFormat.Png);
        }

    }
}
