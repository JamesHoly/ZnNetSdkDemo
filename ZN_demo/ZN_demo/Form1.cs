using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace ZN_demo
{
    public partial class Form1 : Form
    {
        private int loginId;
        int lTransComChannel;
        private int listenId;
        ZLNetSDKDemo_CSharp.Zlnetsdk.ZLNET_SENSOR_DEVICE listDevice;
        public Form1()
        {
            InitializeComponent();
        }
        /// <summary>
        /// 
        /// 监听的回调
        /// </summary>
        /// <param name="lHandle"></param>
        /// <param name="pIp"></param>
        /// <param name="wPort"></param>
        /// <param name="lCommand"></param>
        /// <param name="pParam"></param>
        /// <param name="dwParamLen"></param>
        /// <param name="dwUserData"></param>
        /// <returns></returns>
        public int listenCallbakc(int lHandle, string pIp, UInt16 wPort, int lCommand, IntPtr pParam, UInt32 dwParamLen, IntPtr dwUserData)
        {
            if (lCommand == 1)
            {
                ZLNetSDKDemo_CSharp.Zlnetsdk.ZLNET_DEVICEINFO deviceInfo = new ZLNetSDKDemo_CSharp.Zlnetsdk.ZLNET_DEVICEINFO();
                int errCode = -100;
                //TCP（一般选择此项）     一开始写8：设备主动注册，反向链接，被坑死了，始终返回errCode=3。
                int nSpecCap = 0;
                ZLNetSDKDemo_CSharp.Zlnetsdk.ZLNET_EXTERN_INFO devInfo = (ZLNetSDKDemo_CSharp.Zlnetsdk.ZLNET_EXTERN_INFO)Marshal.PtrToStructure(pParam, typeof(ZLNetSDKDemo_CSharp.Zlnetsdk.ZLNET_EXTERN_INFO));
                loginId = ZLNetSDKDemo_CSharp.Zlnetsdk.ZLNET_LoginEx(pIp, wPort, "admin", "123456", nSpecCap, devInfo.szSerial, ref deviceInfo, ref errCode);
                MessageBox.Show("登录id：" + loginId);
            }
            else if (lCommand == -1)
            {
                MessageBox.Show("设备断线" + pIp + ":" + wPort);
            }
            else {
                MessageBox.Show("未知的lcommand:"+lCommand);
            }
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
            listenId = ZLNetSDKDemo_CSharp.Zlnetsdk.ZLNET_ListenServer("192.168.1.201", 5904, 10000, listenCallbakc, intPrt);
            MessageBox.Show("开启反向注册监听："+listenId.ToString()); //1表示成功   0失败
        }

        private void button2_Click(object sender, EventArgs e)
        {
            IntPtr user = new IntPtr();
            int n = ZLNetSDKDemo_CSharp.Zlnetsdk.ZLNET_Init(null, user);
            MessageBox.Show(n.ToString());
        }

        private void button3_Click(object sender, EventArgs e)
        {
            ZLNetSDKDemo_CSharp.Zlnetsdk.ZLNET_MEASURING_VALUE value=new ZLNetSDKDemo_CSharp.Zlnetsdk.ZLNET_MEASURING_VALUE();
            
            ZLNetSDKDemo_CSharp.Zlnetsdk.ZLNET_SetMeasuringValue(loginId,listDevice.nSensorID,ref value,3000);
          //  int n = ZLNetSDKDemo_CSharp.Zlnetsdk.ZLNET_IOControl(loginId, ZLNetSDKDemo_CSharp.Zlnetsdk.ZLNET_IOTYPE.ZLNET_ALARMOUTPUT, new IntPtr(2), 10);
          //  MessageBox.Show("控制Do的返回结果：" + n);
        }
        /// <summary>
        /// 获取传感器列表
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button4_Click(object sender, EventArgs e)
        {
           // ZLNetSDKDemo_CSharp.Zlnetsdk.ZLNET_SENSOR_DEVICE  listDevice= new ZLNetSDKDemo_CSharp.Zlnetsdk.ZLNET_SENSOR_DEVICE();
         //   int size = Marshal.SizeOf(listDevice);
               int size = Marshal.SizeOf(new  ZLNetSDKDemo_CSharp.Zlnetsdk.ZLNET_SENSOR_DEVICE());
            IntPtr buffer = Marshal.AllocHGlobal(size);
            Int32 rNum=-1;
            ZLNetSDKDemo_CSharp.Zlnetsdk.ZLNET_QuerySensorDevice(loginId, buffer, 2, ref rNum, 3000);

            listDevice= (ZLNetSDKDemo_CSharp.Zlnetsdk.ZLNET_SENSOR_DEVICE)Marshal.PtrToStructure(buffer, typeof(ZLNetSDKDemo_CSharp.Zlnetsdk.ZLNET_SENSOR_DEVICE));
            MessageBox.Show("mSensorId="+listDevice.nSensorID);
            MessageBox.Show(rNum + "");
        }

        /// <summary>
        /// 创建透明通道
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button5_Click(object sender, EventArgs e)
        {
            int RS485 = 1;
            uint baudrate9600 = 4;
            uint databits = 8;
            uint stopbits = 1;
            uint parity_none = 3;
            lTransComChannel = ZLNetSDKDemo_CSharp.Zlnetsdk.ZLNET_CreateTransComChannel(loginId, RS485, baudrate9600, databits, stopbits, parity_none, fZLTransComCallBack, new IntPtr());
            MessageBox.Show("创建透明通道返回值：" + lTransComChannel);
        }

        private void fZLTransComCallBack(int lLoginID, int lTransComChannel, IntPtr pBuffer, UInt32 dwBufSize, IntPtr dwUser)
        {
            MessageBox.Show("");
        }

        /// <summary>
        /// 发送透传数据 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button6_Click(object sender, EventArgs e)
        {
            int n = ZLNetSDKDemo_CSharp.Zlnetsdk.ZLNET_SendTransComData(lTransComChannel, new IntPtr(), 8);
            MessageBox.Show("发送透传数据返回值" + n);
        }

        /// <summary>
        /// 退出设备登录 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button7_Click(object sender, EventArgs e)
        {
            int n = ZLNetSDKDemo_CSharp.Zlnetsdk.ZLNET_Logout(loginId);
            MessageBox.Show("退出登录返回结果:" + n);
        }

        private void button8_Click(object sender, EventArgs e)
        {
            ZLNetSDKDemo_CSharp.Zlnetsdk.ZLNET_StopListenServer(listenId);
        }
    }
}
