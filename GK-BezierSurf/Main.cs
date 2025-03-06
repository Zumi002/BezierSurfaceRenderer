using GK_BezierSurf.Drawing.Camera;
using GK_BezierSurf.Drawing.Lightning;
using GK_BezierSurf.Drawing.Surface;
using Microsoft.VisualBasic.Devices;
using System.Windows.Forms;
using GK_BezierSurf.Drawing.Samplers;
using GK_BezierSurf.Drawing.Samplers.NormalVectorSampler;
using System.Numerics;

namespace GK_BezierSurf
{
    public partial class Main : Form
    {
        BezierSurface surf;
        OrthographicCamera camera;
        bool rotationChange;
        bool triangleCountChange;
        DateTime lastDraw;
        FileNormalVectorSampler oldSampler;

        Color startSurfColor = Color.Red;
        int animZ = 10;
        int animOption = 0;
        float animSpeed = 2;
        float animTimer = 0;
        float animPower = 5;
        bool spiralDir = false;
        TimeSpan span;
        Vector3 lastAnimDir;

        bool moving = false;
        Point originMousePos;
        float sens = 0.4f;
        int ZRot;
        int XRot;

        public Main()
        {
            surf = new BezierSurface();
            rotationChange = false;
            InitializeComponent();
            camera = new OrthographicCamera(imageBox.Width, imageBox.Height);
            camera.drawTriangle = DrawingTriangleCheck.Checked;
            camera.drawMesh = DrawingMeshCheck.Checked;
            XRotNUP.Controls[0].Visible = false;
            ZRotNUP.Controls[0].Visible = false;
            mNUP.Controls[0].Visible = false;
            ksNUP.Controls[0].Visible = false;
            kdNUP.Controls[0].Visible = false;

            GlobalLambertianLightning.m = mSlid.Value;
            GlobalLambertianLightning.kd = (float)kdSlid.Value / 100;
            GlobalLambertianLightning.ks = (float)ksSlid.Value / 100;
            rotationChange = true;
            triangleCountChange = true;
            lastDraw = DateTime.Now;
            camera.surfColor = new StaticSampler(startSurfColor);
            TextureButton.BackColor = startSurfColor;
            TextureColorRadio.Checked = true;
            NormalMapButton.Image = null;
            camera.normalVectorSampler = new NormalVectorSampler();
            noneTextureRadio.Checked = true;
            ZAnimNUP.Controls[0].Visible = false;
            ZAnimNUP.Value = animZ;
            ZAnimSlid.Value = animZ;
            lastAnimDir = new Vector3(0, 0, animZ);
            GlobalLambertianLightning.ChangeLighningDir(lastAnimDir);
            Application.Idle += MainLoop;
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void imageBox_Click(object sender, EventArgs e)
        {
        }

        private void MainLoop(object sender, EventArgs e)
        {

            MainLoop();

        }
        private void MainLoop()
        {
            if (rotationChange)
            {
                surf.Rotate(XRotSlid.Value, ZRotSlid.Value);
                rotationChange = false;
            }
            if (triangleCountChange)
            {
                surf.Divide(TriSlid.Value);
                triangleCountChange = false;
            }

            DateTime now = DateTime.Now;
            span = now - lastDraw;
            Animate();
            if (surf != null)
                camera.Draw(surf);
            imageBox.Image = camera.getNewFrame();

            int fps = (int)(1 / span.TotalSeconds);
            fpsLabel.Text = $"FPS: {fps}";
            lastDraw = now;
        }

        private void Animate()
        {
            if (animOption == 0)//pause
            {
                GlobalLambertianLightning.ChangeLighningDir(lastAnimDir);
                return;
            }
            animTimer += (float)span.TotalSeconds * animSpeed;
            if (animTimer > 2 * Math.PI * 5)
            {
                animTimer = 0;
            }
            Vector3 newDir = Vector3.UnitZ * animZ;
            if (animOption == 1)
            {
                newDir = new Vector3((float)Math.Cos(animTimer) * animPower, (float)Math.Sin(animTimer) * animPower, animZ);
            }
            else if (animOption == 2)
            {
                float exp = (float)Math.Exp(-0.2 * animTimer);
                newDir = new Vector3(exp * (float)Math.Cos(animTimer) * animPower * animPower, exp * (float)Math.Sin(animTimer) * animPower * animPower, animZ);
            }
            lastAnimDir = newDir;
            GlobalLambertianLightning.ChangeLighningDir(newDir);
        }

        private void XRotSlid_ValueChanged(object sender, EventArgs e)
        {
            XRotNUP.Value = XRotSlid.Value;
            rotationChange = true;
        }

        private void XRotNUP_ValueChanged(object sender, EventArgs e)
        {
            XRotSlid.Value = (int)XRotNUP.Value;
            rotationChange = true;
        }

        private void ZRotNUP_ValueChanged(object sender, EventArgs e)
        {
            ZRotSlid.Value = (int)ZRotNUP.Value;
            rotationChange = true;
        }

        private void ZRotSlid_ValueChanged(object sender, EventArgs e)
        {
            ZRotNUP.Value = ZRotSlid.Value;
            rotationChange = true;
        }

        private void TriNUP_ValueChanged(object sender, EventArgs e)
        {
            TriSlid.Value = (int)TriNUP.Value;
            triangleCountChange = true;
        }

        private void TriSlid_Scroll(object sender, EventArgs e)
        {
            TriNUP.Value = TriSlid.Value;
            triangleCountChange = true;
        }
        protected override void OnMouseWheel(MouseEventArgs e)
        {
            // Do not use MouseEventArgs.X, Y because they are relative!
            Point pt_MouseAbs = MousePosition;
            Control i_Ctrl = imageBox;

            Rectangle r_Ctrl = i_Ctrl.RectangleToScreen(i_Ctrl.ClientRectangle);
            if (!r_Ctrl.Contains(pt_MouseAbs))
            {
                base.OnMouseWheel(e);
                return; // mouse position is outside the picturebox or it's parents
            }
            float change = camera.scale + 0.0005f * e.Delta;
            camera.ChangeScale(change > 0 ? change : 0);
        }

        private void tableLayoutPanel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            camera.drawMesh = DrawingMeshCheck.Checked;
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            camera.drawTriangle = DrawingTriangleCheck.Checked;
        }

        private void btTrackBar1_Scroll(object sender, EventArgs e)
        {
            kdNUP.Value = (decimal)kdSlid.Value / 100;
            GlobalLambertianLightning.kd = (float)kdSlid.Value / 100;
        }

        private void mNUP_ValueChanged(object sender, EventArgs e)
        {
            mSlid.Value = (int)mNUP.Value;
            GlobalLambertianLightning.m = mSlid.Value;
        }

        private void kdNUP_ValueChanged(object sender, EventArgs e)
        {
            kdSlid.Value = (int)(kdNUP.Value * 100);
            GlobalLambertianLightning.kd = (float)kdSlid.Value / 100;
        }

        private void ksNUP_ValueChanged(object sender, EventArgs e)
        {
            ksSlid.Value = (int)(ksNUP.Value * 100);
            GlobalLambertianLightning.ks = (float)ksSlid.Value / 100;
        }

        private void ksSlid_Scroll(object sender, EventArgs e)
        {
            ksNUP.Value = (decimal)ksSlid.Value / 100;
            GlobalLambertianLightning.ks = (float)ksSlid.Value / 100;
        }

        private void mSlid_Scroll(object sender, EventArgs e)
        {
            mNUP.Value = mSlid.Value;
            GlobalLambertianLightning.m = mSlid.Value;
        }

        private void loadSurfaceFromToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void txtToolStripMenuItem_Click(object sender, EventArgs e)
        {

            if (loadfromtxtDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    surf.LoadFromTxtFile(loadfromtxtDialog.FileName);
                }
                catch
                {
                    surf.Restore();
                }

            }

        }

        private void TextureButton_Click(object sender, EventArgs e)
        {
            if (TextureColorRadio.Checked)
            {
                if (colorDialog1.ShowDialog() == DialogResult.OK)
                {
                    Color chosenColor = colorDialog1.Color;
                    camera.surfColor = new StaticSampler(chosenColor);
                    TextureButton.BackColor = chosenColor;
                    TextureButton.BackgroundImage = null;
                }
            }
            else if (TextureTextureRadio.Checked)
            {
                if (loadImageDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        FileSampler sampler = new FileSampler(loadImageDialog.FileName);
                        camera.surfColor = sampler;
                        TextureButton.BackgroundImage = sampler.img.Bitmap;
                    }
                    catch { }
                }
            }
        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void lightColorButton_Click(object sender, EventArgs e)
        {
            if (colorDialog1.ShowDialog() == DialogResult.OK)
            {
                Color chosenColor = colorDialog1.Color;
                GlobalLambertianLightning.lightningColor = chosenColor;
                lightColorButton.BackColor = chosenColor;
            }
        }


        private void button3_Click(object sender, EventArgs e)
        {

            if (loadImageDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    FileNormalVectorSampler sampler = new FileNormalVectorSampler(loadImageDialog.FileName);
                    if (normalMapTextureRadio.Checked)
                        camera.normalVectorSampler = sampler;
                    NormalMapButton.BackgroundImage = sampler.sampler.img.Bitmap;
                    oldSampler = sampler;
                }
                catch { }
            }
            loadImageDialog.Dispose();
        }

        private void noneTextureRadio_CheckedChanged(object sender, EventArgs e)
        {
            camera.normalVectorSampler = new NormalVectorSampler();
        }

        private void normalMapTextureRadio_CheckedChanged(object sender, EventArgs e)
        {
            if (oldSampler != null)
                camera.normalVectorSampler = oldSampler;
        }


        private void ZAnimNUP_ValueChanged(object sender, EventArgs e)
        {
            ZAnimSlid.Value = (int)ZAnimNUP.Value;
            animZ = ZAnimSlid.Value;
            lastAnimDir.Z = animZ;
        }

        private void ZAnimSlid_Scroll(object sender, EventArgs e)
        {
            ZAnimNUP.Value = ZAnimSlid.Value;
            animZ = ZAnimSlid.Value;
            lastAnimDir.Z = animZ;

        }



        private void animCircleButton_Click(object sender, EventArgs e)
        {
            animOption = 1;
        }

        private void animPauseButton_Click(object sender, EventArgs e)
        {
            animOption = 0;
        }

        private void animSpiralButton_Click(object sender, EventArgs e)
        {
            animOption = 2;
        }

        private void imageBox_MouseDown(object sender, MouseEventArgs e)
        {
            if (!moving)
            {
                moving = true;
                originMousePos = e.Location;
                ZRot = ZRotSlid.Value;
                XRot = XRotSlid.Value;
            }
            
        }

        private void imageBox_MouseUp(object sender, MouseEventArgs e)
        {
            moving = false;
        }

        private void imageBox_MouseMove(object sender, MouseEventArgs e)
        {
            if(moving&&Control.MouseButtons!=MouseButtons.Left)
            {
                moving = false;
            }
            else if (moving)
            {
                Point p = e.Location;
                int dx = originMousePos.X - p.X;
                int dy = originMousePos.Y - p.Y;

                float xchange = ((float)dx * sens/2f);
                float ychange = ((float)dy * sens);

                int XRotTMP = (int)Math.Clamp(XRot+(int)ychange, XRotNUP.Minimum, XRotNUP.Maximum);
                int ZRotTMP = ZRot + (originMousePos.Y > imageBox.Height / 2 ? 1 : -1)*(XRot>90?1:-1) * (int)xchange;
                int k = ZRotTMP % 360;
                if (k < 0)
                    k += 360;
                ZRotTMP = k > 180 ? k - 360 : k;

                ZRotNUP.Value = ZRotTMP;
                ZRotSlid.Value = ZRotTMP;
                XRotNUP.Value = XRotTMP;
                XRotSlid.Value = XRotTMP;
                rotationChange = true;
            }
        }

        private void imageBox_Click_1(object sender, MouseEventArgs e)
        {
           
        }
    }
}
