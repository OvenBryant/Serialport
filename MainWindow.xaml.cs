using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using System.Windows.Threading;

using System.IO.Ports;

namespace _01.串口助手
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        SerialPort sp;
        public MainWindow()
        {
            InitializeComponent();
            cbxPort.ItemsSource = SerialPort.GetPortNames();
            cbxBTL.ItemsSource = new int[] { 9600, 38400, 57600, 115200 };
            sp = new SerialPort();
        }

        private void Sp_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            int len = sp.BytesToRead;
            byte[] buffer = new byte[len];
            sp.Read(buffer, 0, len);
            string strData = "";
            if (十六进制标志位 == 1)
            {
                strData = BitConverter.ToString(buffer, 0, len); // 十六进制
            }
            else if (普通文本 == 1)
            {
                strData = Encoding.UTF8.GetString(buffer);  // 字符串
            }
            Dispatcher.Invoke(() =>
            {
                LB.Items.Add(strData);
            });

            //string strData = BitConverter.ToString(buffer, 0, len);  // 十六进制
            //string strData = Encoding.UTF8.GetString(buffer, 0, len);  // 字符串
            //Dispatcher.Invoke(() =>
            //{
            //    LB.Items.Add(buffer);
            //});
            //Task.Run(() => { lb.Items.Add(strData); }); 
        }

        private void btnOnOrOff_Click(object sender, RoutedEventArgs e)
        {
            if (cbxPort.Text == "")
            {
                MessageBox.Show("请选择一个串口");
                return;
            }

            if (cbxBTL.Text == "")
            {
                MessageBox.Show("请选择一个波特率");
                return;
            }

            if (btnOnOrOff.Tag.ToString() == "1")
            {

                btnOnOrOff.Tag = 0;
                btnOnOrOff.Content = "关闭串口";
                sp.PortName = cbxPort.Text;     //  获取选中的串口(连接串口)
                sp.BaudRate = Convert.ToInt32(cbxBTL.Text);   // 获取选中的波特率(连接波特率)
                sp.Open();    // 打开串口
                btnRun.IsEnabled = true;
            }
            else
            {
                btnOnOrOff.Tag = 1;
                btnOnOrOff.Content = "打开串口";
                sp.Close();
                btnRun.IsEnabled = false;
            }
        }
        int 十六进制标志位 = 0;
        int 普通文本 = 0;

        private void btnRun_Click(object sender, RoutedEventArgs e)
        {
            LB.Items.Add(tbxScanf.Text);
            if (cbxHex.IsChecked == true)
            {
                string[] data = tbxScanf.Text.Split(' ');   // FF 01 0A
                byte[] cmd = new byte[data.Length];
                for (int i = 0; i < data.Length; i++)
                {
                    cmd[i] = Convert.ToByte(data[i], 16);
                }
                sp.Write(cmd, 0, cmd.Length);

                十六进制标志位 = 1;
            }
            else
            {
                byte[] bytes = Encoding.UTF8.GetBytes(tbxScanf.Text);
                sp.Write(bytes, 0, bytes.Length);
                普通文本 = 1;
            }
            tbxScanf.Clear();
        }

        private void cbx_Click(object sender, RoutedEventArgs e)
        {
            if (cbx.IsChecked == true)
            {
                sp.DataReceived += Sp_DataReceived; // 添加数据接收事件 
            }
            else
            {
                sp.DataReceived -= Sp_DataReceived; // 添加数据接收事件 
            }
        }

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            LB.Items.Clear();
        }
    }
}
