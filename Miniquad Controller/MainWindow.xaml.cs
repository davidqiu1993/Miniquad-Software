using System;
using System.Timers;
using System.Threading;
using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;
using System.IO.Ports;
using MahApps.Metro;
using MahApps.Metro.Controls;
using Miniquad_Controller.SerialCommunication;
using Miniquad_Controller.Miniquad;

namespace Miniquad_Controller
{
    /// <summary>
    /// 表示飞行器控制器状态。
    /// </summary>
    public enum RobotControllerState
    {
        Disconnected = 0,
        Connected = 1,
        OnControl = 2
    }


    /// <summary>
    /// 表示 MainWindow 的交互逻辑。
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        protected MiniquadConfiguration _configPanel;

        protected bool _isAotoControlled = false; // Auto controlled by algorithm

        protected System.Timers.Timer _refreshTimer = new System.Timers.Timer(10.0); // 10.0 ms per refreshment
        protected void _refreshHandler(object sender, ElapsedEventArgs args)
        {
            // Refresh the serial port list
            _refreshSerialPortList();

            // Refresh the serial port connection state
            _refreshSerialPortConnectionState();
        }

        /// <summary>
        /// 刷新可用串行接口的列表。
        /// </summary>
        protected void _refreshSerialPortList()
        {
            // Detect serial port connection change
            if (SerialPortController.ConnectionChanged)
            {
                // Serial port connection changed
                // Get the current list of serial ports
                string[] ports = SerialPortController.CurrentSerialPorts;

                // Update the serial port list
                lstSerialPorts.Dispatcher.Invoke(new Action(() => { lstSerialPorts.Items.Clear(); }));
                foreach (string portName in ports)
                {
                    lstSerialPorts.Dispatcher.Invoke(new Action(() => { lstSerialPorts.Items.Add(portName); }));
                }
            }
        }

        /// <summary>
        /// 刷新串口连接状态。
        /// </summary>
        protected void _refreshSerialPortConnectionState()
        {
            if ((_robotControllerState == RobotControllerState.Disconnected) && SerialPortController.IsReady)
            {
                _robotControllerState = RobotControllerState.Connected;
                shapeConnected.Dispatcher.Invoke(new Action(() => { shapeConnected.Stroke = Brushes.Orange; }));
                shapeConnected.Dispatcher.Invoke(new Action(() => { shapeConnected.Fill = Brushes.Gray; }));
            }
            else if ((_robotControllerState== RobotControllerState.Connected || _robotControllerState== RobotControllerState.OnControl)  && (!SerialPortController.IsReady))
            {
                _robotControllerState = RobotControllerState.Disconnected;
                BrushConverter converter = new BrushConverter();
                shapeConnected.Dispatcher.Invoke(new Action(() => { shapeConnected.Stroke = (Brush)converter.ConvertFromString("#FF808080"); }));
                shapeConnected.Dispatcher.Invoke(new Action(() => { shapeConnected.Fill = null; }));
            }
        }
        protected RobotControllerState _robotControllerState = RobotControllerState.Disconnected;
        
        /// <summary>
        /// 端口数据接收处理程序。
        /// </summary>
        /// <param name="text">从端口接收到的数据。</param>
        protected void _receiveData(byte[] receivedBytes)
        {
            switch (_robotControllerState)
            {
                case RobotControllerState.OnControl:
                    // Refresh the robot status with the received data
                    _receiveRobotStatus();
                    break;
                default:
                    // Display directly on the receipt panel
                    _receiveDataToReceiptPanel(System.Text.Encoding.ASCII.GetString(receivedBytes));
                    break;
            }
        }

        /// <summary>
        /// 将从端口接收到的数据显示在数据接收显示板处。
        /// </summary>
        /// <param name="text">从端口接收到的新数据。</param>
        protected void _receiveDataToReceiptPanel(string text)
        {
            // Check if receipt available
            bool receiptAvailable = false;
            chkbReceiving.Dispatcher.Invoke(new Action(() => { if (chkbReceiving.IsChecked.HasValue && chkbReceiving.IsChecked.Value) receiptAvailable = true; }));

            // Receive the text
            if (receiptAvailable) txtReceipt.Dispatcher.Invoke(new Action(() => { txtReceipt.AppendText(text); }));
        }

        /// <summary>
        /// 接收并显示飞行器的当前状态。
        /// </summary>
        protected void _receiveRobotStatus()
        {
            // Get the latest Miniquad status
            if (Miniquad.Miniquad.RefreshStatus(SerialPortController.ReceivedDataBuffer))
            {
                // Get the status
                MiniquadStatus status = Miniquad.Miniquad.Status;

                // Get the throttle outputs (PC mode)
                int[] throttles = Miniquad.Miniquad.GetAdjustedThrottleOutputs_PC();

                // Check the existence of the status
                if (status != null)
                {
                    // Clear the buffer
                    SerialPortController.ClearReceivedDataBuffer();

                    // Quaternion
                    lblQuaternion.Dispatcher.Invoke(new Action(() =>
                    {
                        lblQuaternion.Content =
                            status.Quaternion.W.ToString("0.00") + ", " +
                            status.Quaternion.X.ToString("0.00") + ", " +
                            status.Quaternion.Y.ToString("0.00") + ", " +
                            status.Quaternion.Z.ToString("0.00");
                    }));

                    // Euler Angle
                    lblEulerAngle.Dispatcher.Invoke(new Action(() =>
                    {
                        lblEulerAngle.Content =
                            status.EulerAngle.Psi.ToString("0.00") + ", " +
                            status.EulerAngle.Theta.ToString("0.00") + ", " +
                            status.EulerAngle.Phi.ToString("0.00");
                    }));

                    // Rotation
                    lblRotation.Dispatcher.Invoke(new Action(() =>
                    {
                        lblRotation.Content =
                            status.Rotation.X.ToString("0.00") + ", " +
                            status.Rotation.Y.ToString("0.00") + ", " +
                            status.Rotation.Z.ToString("0.00");
                    }));

                    // Acceleration
                    lblAcceleration.Dispatcher.Invoke(new Action(() =>
                    {
                        lblAcceleration.Content =
                            status.Acceleration.X.ToString("0.00") + ", " +
                            status.Acceleration.Y.ToString("0.00") + ", " +
                            status.Acceleration.Z.ToString("0.00");
                    }));

                    // Gravity
                    lblGravity.Dispatcher.Invoke(new Action(() =>
                    {
                        lblGravity.Content =
                            status.Gravity.X.ToString("0.00") + ", " +
                            status.Gravity.Y.ToString("0.00") + ", " +
                            status.Gravity.Z.ToString("0.00");
                    }));

                    // Yaw Pitch Roll
                    lblYawPitchRoll.Dispatcher.Invoke(new Action(() =>
                    {
                        lblYawPitchRoll.Content =
                            status.YawPitchRoll.Yaw.ToString("0.00") + ", " +
                            status.YawPitchRoll.Pitch.ToString("0.00") + ", " +
                            status.YawPitchRoll.Roll.ToString("0.00");
                    }));

                    // Temperature
                    //lblTemperature.Content = "null";

                    // Throttles
                    lblThrottles.Dispatcher.Invoke(new Action(() =>
                    {
                        lblThrottles.Content =
                            status.Propeller1.Throttle.ToString() + ", " +
                            status.Propeller2.Throttle.ToString() + ", " +
                            status.Propeller3.Throttle.ToString() + ", " +
                            status.Propeller4.Throttle.ToString();

                        //lblThrottles.Content =
                        //    throttles[0].ToString() + ", " +
                        //    throttles[1].ToString() + ", " +
                        //    throttles[2].ToString() + ", " +
                        //    throttles[3].ToString();
                    }));
                    
                    // Auto controlled by algorithm
                    if (_isAotoControlled) Miniquad.Miniquad.SetThrottleOutputs_PC(throttles[0], throttles[1], throttles[2], throttles[3]);
                }
            }   
        }

        /// <summary>
        /// 串行端口控制器配置变更事件处理器。
        /// </summary>
        /// <param name="sender">触发该事件的对象。</param>
        /// <param name="e">事件参数。</param>
        protected void _ctrlConfigChangedHandler()
        {
            // Check if the serial port is ready
            if (SerialPortController.IsReady)
            {
                // Serial port ready
                // Start receiving data
                SerialPortController.StartReceivingData();
            }
            else
            {
                // Serial port not ready
                // Stop receiving data
                SerialPortController.StopReceivingData();
            }
        }

        /// <summary>
        /// 初始化窗体控件。
        /// </summary>
        protected void _initializeControls()
        {
            // Init: cbDataBits
            cbDataBits.Items.Clear();
            cbDataBits.Items.Add((int)DataBits.Eight);
            cbDataBits.Items.Add((int)DataBits.Seven);
            cbDataBits.Items.Add((int)DataBits.Six);
            cbDataBits.Items.Add((int)DataBits.Five);
            cbDataBits.SelectedIndex = 0;

            // Init: cbStopBits
            cbStopBits.Items.Clear();
            cbStopBits.Items.Add(StopBits.One);
            cbStopBits.Items.Add(StopBits.OnePointFive);
            cbStopBits.Items.Add(StopBits.Two);
            cbStopBits.SelectedIndex = 0;

            // Init: cbParity
            cbParity.Items.Clear();
            cbParity.Items.Add(Parity.None);
            cbParity.Items.Add(Parity.Odd);
            cbParity.Items.Add(Parity.Even);
            cbParity.Items.Add(Parity.Space);
            cbParity.Items.Add(Parity.Mark);
            cbParity.SelectedIndex = 0;

            // Init: cbBaudRate
            cbBaudRate.Items.Clear();
            //cbBaudRate.Items.Add((int)BaudRates.BR110);
            //cbBaudRate.Items.Add((int)BaudRates.BR300);
            //cbBaudRate.Items.Add((int)BaudRates.BR600);
            //cbBaudRate.Items.Add((int)BaudRates.BR1200);
            //cbBaudRate.Items.Add((int)BaudRates.BR2400);
            cbBaudRate.Items.Add((int)BaudRates.BR4800);
            cbBaudRate.Items.Add((int)BaudRates.BR9600);
            cbBaudRate.Items.Add((int)BaudRates.BR14400);
            cbBaudRate.Items.Add((int)BaudRates.BR19200);
            cbBaudRate.Items.Add((int)BaudRates.BR38400);
            cbBaudRate.Items.Add((int)BaudRates.BR56000);
            cbBaudRate.Items.Add((int)BaudRates.BR57600);
            cbBaudRate.Items.Add((int)BaudRates.BR115200);
            //cbBaudRate.Items.Add((int)BaudRates.BR128000);
            //cbBaudRate.Items.Add((int)BaudRates.BR256000);
            cbBaudRate.SelectedIndex = 7;

            // Init: cbMessageEnding
            cbMessageEnding.Items.Clear();
            cbMessageEnding.Items.Add(MessageEnding.NoEnding);
            cbMessageEnding.Items.Add(MessageEnding.NewLine);
            cbMessageEnding.Items.Add(MessageEnding.CarrigeReturn);
            cbMessageEnding.Items.Add(MessageEnding.NewLineAndCarrigeReturn);
            cbMessageEnding.SelectedIndex = 0;
        }


        /// <summary>
        /// 实例化一个 MainWindow 对象。
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            
            // Load configuration panel
            _configPanel = new MiniquadConfiguration();
            _configPanel.Visibility = Visibility.Hidden;
            
            // Initialize the window controls
            _initializeControls();

            // Bind the refresh handler to the refresh timer and run the timer
            _refreshTimer.Elapsed += new ElapsedEventHandler(_refreshHandler);
            _refreshTimer.Enabled = true;

            // Bind the data received event handler
            SerialPortController.DataReceivedEvent += _receiveData;

            // Bind the serial port controller configuration changed event handler
            SerialPortController.SerialPortConfigurationChangedEvent += _ctrlConfigChangedHandler;

            // Initialize the Miniquad
            Miniquad.Miniquad.Initialize(3, 5, 6, 9, ComputingMode.PricipalComputerMode); // Original Sensor Direction
            //Miniquad.Miniquad.Initialize(6, 5, 3, 9, ComputingMode.PricipalComputerMode); // 45-degree-Shifted Sensor Direction
        }


        // Shutdown the application
        private void windowMain_Closed(object sender, EventArgs e)
        {
            Application.Current.Shutdown();
        }

        // Clear the receipt panel
        private void btnClearReceipt_Click(object sender, RoutedEventArgs e)
        {
            txtReceipt.Text = "";
        }

        // Change the serial port name
        private void lstSerialPorts_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                 SerialPortController.PortName = (string)lstSerialPorts.SelectedItem;
            }
            catch (ArgumentNullException)
            {
                SerialPortController.StopReceivingData();
            }
        }

        // Change the receipt available state
        private void chkbReceiving_Checked(object sender, RoutedEventArgs e)
        {
            _ctrlConfigChangedHandler();
        }

        // Change the receipt available state
        private void chkbReceiving_Unchecked(object sender, RoutedEventArgs e)
        {
            _ctrlConfigChangedHandler();
        }

        // Change the data bits
        private void cbDataBits_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SerialPortController.DataBits = (DataBits)cbDataBits.SelectedItem;
        }

        // Change the stop bits
        private void cbStopBits_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SerialPortController.StopBits = (StopBits)cbStopBits.SelectedItem;
        }

        // Change the parity
        private void cbParity_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SerialPortController.Parity = (Parity)cbParity.SelectedItem;
        }

        // Change the baud rate
        private void cbBaudRate_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SerialPortController.BaudRate = (int)cbBaudRate.SelectedItem;
        }

        // Send a string with new line to the serial port device
        private void btnSendLine_Click(object sender, RoutedEventArgs e)
        {
            // Set the timeout
            SerialPortController.WriteTimeout = 1000;

            // Send the string
            try
            {
                SerialPortController.Write(txtSendLine.Text, (MessageEnding)cbMessageEnding.SelectedItem);
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("Failed in sending string:\n" + ex.Message);
            }
        }

        // Key down inside the send line text box (mainly detect enter key)
        private void txtSendLine_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
            {
                btnSendLine_Click(sender, new RoutedEventArgs());
            }
        }

        // Text of the receipt panel changed (scroll to the bottom of the panel)
        private void txtReceipt_TextChanged(object sender, TextChangedEventArgs e)
        {
            txtReceipt.ScrollToEnd();

            // Check length limit
            if (txtReceipt.Text.Length > 102400)
            {
                txtReceipt.Text.Remove(0, txtReceipt.Text.Length - 102400);
            }
        }

        // Change control state of the robot
        private void shapeConnected_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            // Check if state ready
            if ((_robotControllerState == RobotControllerState.Connected) && SerialPortController.IsReady)
            {
                // Change the appearance and state
                _robotControllerState = RobotControllerState.OnControl;
                shapeConnected.Dispatcher.Invoke(new Action(() => { shapeConnected.Stroke = Brushes.Orange; }));
                shapeConnected.Dispatcher.Invoke(new Action(() => { shapeConnected.Fill = Brushes.Orange; }));

                // Initialize the Miniquad
                Miniquad.Miniquad.Initialize();

                // Start receiving data
                _ctrlConfigChangedHandler();
            }
            else if (_robotControllerState == RobotControllerState.OnControl)
            {
                // Change the appearance and state
                _robotControllerState = RobotControllerState.Disconnected;
                BrushConverter converter = new BrushConverter();
                shapeConnected.Dispatcher.Invoke(new Action(() => { shapeConnected.Stroke = (Brush)converter.ConvertFromString("#FF808080"); }));
                shapeConnected.Dispatcher.Invoke(new Action(() => { shapeConnected.Fill = null; }));

                // Start receiving data
                _ctrlConfigChangedHandler();

                // Stop the Miniquad
                Miniquad.Miniquad.SetThrottleOutputs_PC(0, 0, 0, 0);
            }
        }

        // Miniquad goes up
        private void btnMQGoUp_Click(object sender, RoutedEventArgs e)
        {
            if (_robotControllerState == RobotControllerState.OnControl)
            {
                
            }
            Miniquad.Miniquad.SetThrottleOutputs_PC(30, 30, 30, 30);
        }

        // Miniquad goes down
        private void btnMQGoDown_Click(object sender, RoutedEventArgs e)
        {
            if (_robotControllerState == RobotControllerState.OnControl)
            {
                
            }
            Miniquad.Miniquad.SetThrottleOutputs_PC(0, 0, 0, 0);
        }

        // Miniquad goes front
        private void btnMQGoFront_Click(object sender, RoutedEventArgs e)
        {
            if (_robotControllerState == RobotControllerState.OnControl)
            {
                ;
            }
            Miniquad.Miniquad.SetThrottleOutputs_PC(0, 30, 0, 0);
        }

        // Miniquad goes back
        private void btnMQGoBack_Click(object sender, RoutedEventArgs e)
        {
            if (_robotControllerState == RobotControllerState.OnControl)
            {
                ;
            }
            Miniquad.Miniquad.SetThrottleOutputs_PC(0, 0, 0, 30);
        }

        // Miniquad goes left
        private void btnMQGoLeft_Click(object sender, RoutedEventArgs e)
        {
            if (_robotControllerState == RobotControllerState.OnControl)
            {
                ;
            }
            Miniquad.Miniquad.SetThrottleOutputs_PC(0, 0, 30, 0);
        }

        // Miniquad goes right
        private void btnMQGoRight_Click(object sender, RoutedEventArgs e)
        {
            if (_robotControllerState == RobotControllerState.OnControl)
            {
                ;
            }
            Miniquad.Miniquad.SetThrottleOutputs_PC(30, 0, 0, 0);
        }

        // Miniquad auto control
        private void btnMQAutoControl_Click(object sender, RoutedEventArgs e)
        {
            if (_isAotoControlled == true)
            {
                _isAotoControlled = false;
                Miniquad.Miniquad.SetThrottleOutputs_PC(30, 30, 30, 30);
                btnMQAutoControl.Content = "○";
            }
            else
            {
                _isAotoControlled = true;
                btnMQAutoControl.Content = "∞";
            }
        }

        // Show configuration panel 
        private void btnMiniquadConfiguration_Click(object sender, RoutedEventArgs e)
        {
            _configPanel.Owner = this;
            _configPanel.Visibility = Visibility.Visible;
        }
    }
}
