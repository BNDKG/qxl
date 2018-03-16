using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using System.Drawing.Imaging;

namespace qxlgai
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        public bool runflag = false;
        private void button1_Click(object sender, EventArgs e)
        {

            Thread t2 = new Thread(new ThreadStart(Printmain));
            if (runflag == false)
            {
                runflag = true;
                t2.IsBackground = true;
                t2.Priority = ThreadPriority.Highest;
                t2.Start();
            }
            else
            {
                runflag = false;
            }

        }
        private void Printmain()
        {
            PaintEventArgs pe = new PaintEventArgs(this.CreateGraphics(), this.ClientRectangle);
            Graphics g = pe.Graphics; //创建画板,这里的画板是由Form提供的.
            g.Clear(Color.Transparent);
            while (runflag)
            {

                
                Z_Paint(g);

                //System.Threading.Thread.Sleep(100);

            }
        }

        double angle = 0;

        public Color[] linergb = new Color[400];
        int CoreX = 225;
        int CoreY = 225;

        private void Z_Paint(Graphics g)
        {
            int speed = 64;

            double angle2 = angle - Math.PI / speed * 16;
            double angle3 = angle - Math.PI / 2;
            double angle4 = angle - Math.PI / 2  - Math.PI / speed * 16;


            Pen p2 = new Pen(Color.Black, 4);

            Pen p4 = new Pen(Color.Black, 4);


            Z_Line(angle, g);
            Z_ClearLine(angle2, p2, g);
            Z_Line(angle3, g);
            Z_ClearLine(angle4, p4, g);

            angle += Math.PI / speed;

        }
        private void Z_Line(double angle, Graphics g)
        {

            int Long = 200;

            int pixel = 20;

            for(int i=0; i < (2*pixel); i++)
            {
                double sx = CoreX - (Long - (Long / pixel) * i) * Math.Sin(angle);
                double sy = CoreY - (Long - (Long / pixel) * i) * Math.Cos(angle);
                double ex = CoreX - (Long - (Long / pixel) * (i+1)) * Math.Sin(angle);
                double ey = CoreY - (Long - (Long / pixel) * (i+1)) * Math.Cos(angle);

                linergb[i]=getpixel((int)sx, (int)sy);

                Color cc = Color.FromArgb(linergb[i].R, linergb[i].G, linergb[i].B);

                Pen p = new Pen(cc, 2);
                g.DrawLine(p, (int)sx, (int)sy, (int)ex, (int)ey);

            }

        }
        private Color getpixel(int x,int y)
        {

            int tempr = cuted_data[x, y].r;
            int tempg = cuted_data[x, y].g;
            int tempb = cuted_data[x, y].b;

            Color tempcolor = Color.FromArgb(tempr, tempg, tempb);
            //Color tempcolor = Color.FromArgb(0, 255, 0);

            return tempcolor;
        }
        private void Z_ClearLine(double angle, Pen p, Graphics g)
        {

            int Long = 220;

                double sx = CoreX - (Long) * Math.Sin(angle);
                double sy = CoreY - (Long) * Math.Cos(angle);
                double ex = CoreX + (Long) * Math.Sin(angle);
                double ey = CoreY + (Long) * Math.Cos(angle);

                g.DrawLine(p, (int)sx, (int)sy, (int)ex, (int)ey);


        }
        public struct color_z
        {
            public byte a;
            public byte r;
            public byte g;
            public byte b;
        }
        public struct cut_position
        {
            public int x;
            public int y;
            public int width;
            public int height;
        }

        color_z[,] cuted_data;
        private void button2_Click(object sender, EventArgs e)
        {
            Image cur_screen_image = Image.FromFile(".\\test4.jpg");
            Bitmap b = new Bitmap(cur_screen_image);
            cuted_data = new color_z[b.Width, b.Height];
            cut_position V_cut_position= new cut_position { x = 135, y = 0, width = (450), height = (b.Height) };

            BitmapData bmData = b.LockBits(new Rectangle(0, 0, b.Width, b.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            int stride = bmData.Stride;   // 扫描的宽度 
            int height_whole = bmData.Height;
            unsafe
            {
                byte* p = (byte*)bmData.Scan0.ToPointer(); // 获取图像首地址 
                byte* p3 = p;//防止溢出
                int nOffset = stride - b.Width * 3;  // 实际宽度与系统宽度的距离 
                //byte red, green, blue;
                p += (stride * V_cut_position.y + V_cut_position.x * 3);//跳到开始点

                for (int y = 0; y < V_cut_position.height; ++y)
                {
                    for (int x = 0; x < V_cut_position.width; ++x)
                    {
                        cuted_data[x, y].b = p[0];
                        cuted_data[x, y].g = p[1];
                        cuted_data[x, y].r = p[2];

                        //bmpout[y, x] = (byte)(255 - (.299 * red + .587 * green + .114 * blue));
                        p += 3;  // 跳过3个字节处理下个像素点 

                    }
                    p += (stride - V_cut_position.width * 3); // 加上间隔 
                    if ((p - p3) > (stride * height_whole))
                    {
                        break;
                    }
                }
            }
            b.UnlockBits(bmData); // 解锁 



        }
    }
}
