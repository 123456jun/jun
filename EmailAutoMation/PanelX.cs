using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
namespace mkRobotClient
{
    /// <summary>
    /// 一个透明的panel
    /// </summary>
    public class PanelX : System.Windows.Forms.Panel
    {
        private Color _borderColor;
        private int _borderWidth = 1;
        private DashStyle _borderStyle = DashStyle.Solid;
        private int _opacity = 125;
        public PanelX()
        {



        }
        #region Property
        [Category("Custom"), Description("Border Color")]
        public Color BorderColor
        {
            set { _borderColor = value; }
            get { return _borderColor; }
        }
        [Category("Custom"), Description("Border Width"), DefaultValue(1)]
        public int BorderWidth
        {
            set
            {
                if (value < 0) value = 0;

                _borderWidth = value;
            }
            get { return _borderWidth; }
        }
        [Category("Custom"), Description("Border Style"), DefaultValue(DashStyle.Solid)]
        public DashStyle BorderStyle
        {
            set { this._borderStyle = value; this.Invalidate(); }
            get { return this._borderStyle; }
        }
        [Bindable(true), Category("Custom"), DefaultValue(125), Description("背景的透明度. 有效值0-255")]
        public int Opacity
        {
            get { return _opacity; }
            set
            {
                if (value > 255) value = 255;
                else if (value < 0) value = 0;
                _opacity = value;
                this.Invalidate();
            }
        }
        #endregion


        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x00000020; //WS_EX_TRANSPARENT
                return cp;
            }
        }
        protected override void OnPaint(System.Windows.Forms.PaintEventArgs e)
        {
            if (this._opacity > 0)
            {
                e.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(this._opacity, this.BackColor)), this.ClientRectangle);
            }
            if (this._borderWidth > 0)
            {
                Pen pen = new Pen(this._borderColor, this._borderWidth);
                pen.DashStyle = this._borderStyle;
                e.Graphics.DrawRectangle(pen, e.ClipRectangle.Left, e.ClipRectangle.Top, this.Width - 1, this.Height - 1);
                pen.Dispose();
            }
        }
        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (e.Location.X < this.Parent.Width - 100 && e.Location.Y < 50)
                if (e.Button == MouseButtons.Left)
                {
                    ReleaseCapture();
                    SendMessage(this.Parent.Handle, 0x112, 0xf012, 0);
                }
        }
        [DllImport("user32.dll")]
        public static extern bool ReleaseCapture();
        [DllImport("user32.dll")]
        public static extern bool SendMessage(IntPtr hwnd, int wMsg, int wParam, int lParam);
    }
}
