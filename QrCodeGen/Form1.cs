using System;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Zen.Barcode;
using System.Threading;
using MetroFramework.Forms;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;


//TODO : Save it to differnt sizes
namespace QrCodeGen
{
    public partial class Form1 : MetroForm
    {
        Bitmap bmp;
        Color foreGround;
        Color backGround;
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text != string.Empty)
            {
                currentBGcol = Color.White;
                currentFOcol = Color.Black;
                CodeQrBarcodeDraw qrCode = BarcodeDrawFactory.CodeQr;
                switch(comboBox1.SelectedIndex)
                {
                    case 1 : pictureBox1.Image = qrCode.Draw(textBox1.Text, 50, 6);
                        break;
                    case 0 : pictureBox1.Image = qrCode.Draw("https://play.google.com/store/apps/details?id=" + textBox1.Text, 50, 6);
                        break;
                    case 2 : pictureBox1.Image = qrCode.Draw("http://" + textBox1.Text, 50, 6);
                        break;
                }
            }        
        }

        public static Color currentBGcol = Color.White;
       
        private void button2_Click(object sender, EventArgs e)
        {
            ColorDialog cd = new ColorDialog();
            if (cd.ShowDialog() == DialogResult.OK)
            {
                backGround = cd.Color;
                Bitmap bmp = new Bitmap(pictureBox1.Image);
                Color colourToCheck = Color.White;                
                colourToCheck = backGround;                
                for (int i = 0; i < bmp.Width; i++)
                {
                    for (int j = 0; j < bmp.Height; j++)
                    {
                        if (isColourSame(bmp.GetPixel(i, j),currentBGcol))
                        {
                            bmp.SetPixel(i, j, cd.Color);
                            pictureBox1.Image = bmp;
                        }
                    }
                }
                currentBGcol = colourToCheck;
                if (colourSimilarity(currentBGcol, currentFOcol) >= 80f)                    
                    MetroFramework.MetroMessageBox.Show(this, "Colours Are Too Similar to be distinguished", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error,140);
            }
            
        }

        private Bitmap imageOuterEdge(Bitmap img,int size)
        {
            Bitmap bmp = new Bitmap(img.Width, img.Height, PixelFormat.Format32bppArgb);
            //bmp.MakeTransparent();
            Color transparent = Color.FromArgb(0, 0, 0, 0);
            for(int i=0;i<img.Width;i++)
            {
                for (int j = 0; j < img.Height; j++)
                {
                    if (!isColourSame(img.GetPixel(i, j), transparent))
                    {

                        //for (int k = 0; k < 40; k++)
                        //{
                        //    bmp.SetPixel(i + k, j + k, Color.White);
                        //}

                        //for (int k = 0; k < 40; k++)
                        //{
                        //    if(i-k >= 0 && j-k >= 0)
                        //        bmp.SetPixel(i - k, j - k, Color.White);
                        //}
                        bmp.SetPixel(i, j, Color.White);
                    }                          
                                   
                    }
            }
            return bmp;
        }

        private bool isColourSame(Color a , Color b)
        {
            if (a.R == b.R && a.G == b.G && a.B == b.B && a.A == b.A)
                return true;
            return false;
        }

        public static Color currentFOcol = Color.Black;

        private void button3_Click(object sender, EventArgs e)
        {
            ColorDialog cd = new ColorDialog();
            if (cd.ShowDialog() == DialogResult.OK)
            {
                foreGround = cd.Color;
                Bitmap bmp = new Bitmap(pictureBox1.Image);
                Color colourToCheck = Color.Black;
                colourToCheck = foreGround;
                for (int i = 0; i < bmp.Width; i++)
                {
                    for (int j = 0; j < bmp.Height; j++)
                    {
                        if (isColourSame(bmp.GetPixel(i, j), currentFOcol))
                        {
                            bmp.SetPixel(i, j, cd.Color);
                            pictureBox1.Image = bmp;
                        }
                    }
                }
                currentFOcol = colourToCheck;
                if (colourSimilarity(currentBGcol, currentFOcol) >= 80f)
                    MetroFramework.MetroMessageBox.Show(this, "Colours Are Too Similar to be distinguished", "Error", MessageBoxButtons.OK, 140);
            }
        }
        
        private void pictureBox1_MouseClick(object sender, MouseEventArgs e)
        {
            Color a = Color.FromArgb(255, 255, 255);
            Color b = Color.FromArgb(0,0,0);
            
            
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            int textLength = textBox1.Text.Length;
            if (textLength <= 120)
            {                
                label1.Text = "" + textLength + "/120";                
            }
            else
            {
                textBox1.Text = textBox1.Text.Substring(0, 119);
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Bitmap tempImg;
            SaveFileDialog sfd = new SaveFileDialog();
            Rectangle rect = new Rectangle(0, 0, 270, 270);
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                tempImg = new Bitmap(pictureBox1.Image);
                Bitmap mp = tempImg.Clone(rect, tempImg.PixelFormat);
                mp.Save(sfd.FileName,System.Drawing.Imaging.ImageFormat.Jpeg);
                tempImg.Dispose();
                mp.Dispose();
            }
            
           
        }

        private void button5_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            Image img = new Bitmap(pictureBox1.Image.Width, pictureBox1.Image.Height);

            if (ofd.ShowDialog() == DialogResult.OK)
            {               
                Bitmap baseImage;
                Bitmap overlayImage;
                baseImage = (Bitmap)pictureBox1.Image;
                overlayImage = (Bitmap)Image.FromFile(ofd.FileName);
                var finalImage = new Bitmap(baseImage.Width, baseImage.Height, PixelFormat.Format32bppArgb);
                var graphics = Graphics.FromImage(finalImage);
                graphics.CompositingMode = CompositingMode.SourceOver;
                float overlayPosX = baseImage.Width / 2 - 40f;
                float overlayPosY = baseImage.Height / 2 - 40f;
                graphics.DrawImage(baseImage, 0, 0);
                Brush b;
                var br = new SolidBrush(backGround);
                //graphics.FillRectangle(br, overlayPosX, overlayPosY, 60f, 60f);     
                graphics.DrawImage(imageOuterEdge(overlayImage, 2), overlayPosX, overlayPosY,80f,80f);         
                graphics.DrawImage(overlayImage, baseImage.Width/2-30f, baseImage.Height/2-30f, 60f, 60f);                
                pictureBox1.Image = finalImage;
            }
            
        }
        //TODO : Fix this similarity check 
        private float colourSimilarity(Color a,Color b)
        {
            return ((1f - (Math.Abs((float)(a.R + a.G + a.B) - (float)(b.R + b.G + b.B)) / (6f * 255f)))*100f);
        }
    }
}
