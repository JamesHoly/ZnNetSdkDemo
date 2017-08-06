using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ZN_demo
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        public int listenCallbakc(int lHandle, string pIp, UInt16 wPort, int lCommand, IntPtr pParam, UInt32 dwParamLen, IntPtr dwUserData)
        {
            //MessageBox.Show("listenCallback");
            IntPtr intPrt = new IntPtr();
            ZLNetSDKDemo_CSharp.Zlnetsdk.ZLNET_DEVICEINFO deviceInfo = new ZLNetSDKDemo_CSharp.Zlnetsdk.ZLNET_DEVICEINFO();
            int errCode=-100;
            int n = ZLNetSDKDemo_CSharp.Zlnetsdk.ZLNET_LoginEx("", 0, "admin", "123456", 8, intPrt, ref deviceInfo, ref errCode);
            MessageBox.Show("登录信息：" + n);
            return lHandle;
        }


        /// <summary>
        /// 开启反向注册监听
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            IntPtr intPrt = new IntPtr();
            int n = ZLNetSDKDemo_CSharp.Zlnetsdk.ZLNET_ListenServer("192.168.1.201", 5904, 10000, listenCallbakc, intPrt);
            MessageBox.Show(n.ToString()); //这里始终返回0
        }

        private void button2_Click(object sender, EventArgs e)
        {
            IntPtr user = new IntPtr();
            int n = ZLNetSDKDemo_CSharp.Zlnetsdk.ZLNET_Init(null, user);
            MessageBox.Show(n.ToString());
        }
    }
}
