using System;
using System.IO;
using System.IO.Ports;
using System.Collections.Generic;

namespace Miniquad_Controller.SerialCommunication
{
    /// <summary>
    /// 表示通信端口每个字节的标准数据位长度。
    /// </summary>
    public enum DataBits
    {
        Five = 5,
        Six = 6,
        Seven = 7,
        Eight = 8
    }


    /// <summary>
    /// 表示一般通信端口的波特率。
    /// </summary>
    public enum BaudRates
    {
        BR110 = 110,
        BR300 = 300,
        BR600 = 600,
        BR1200 = 1200,
        BR2400 = 2400,
        BR4800 = 4800,
        BR9600 = 9600,
        BR14400 = 14400,
        BR19200 = 19200,
        BR38400 = 38400,
        BR56000 = 56000,
        BR57600 = 57600,
        BR115200 = 115200,
        BR128000 = 128000,
        BR256000 = 256000
    }


    /// <summary>
    /// 表示从串口发送的信息的结尾。
    /// </summary>
    public enum MessageEnding
    {
        NoEnding = 0,
        NewLine = 1,
        CarrigeReturn = 2,
        NewLineAndCarrigeReturn = 3
    }


    /// <summary>
    /// 表示串行接口管理器。
    /// </summary>
    static class SerialPortController
    {
        private static string[] _currentSerialPorts = { };
        private static SerialPort _port = new SerialPort();

        private static List<byte> _receivedDataBuffer = new List<byte>(1024 + 128);
        private static int _receivedDataBufferSize = 1024;


        /// <summary>
        /// 获取一个 bool 值，表示串行接口连接是否发生了改变。
        /// </summary>
        public static bool ConnectionChanged
        {
            get
            {
                // Detect the new serial port connections
                string[] ports = { };
                try
                {
                    ports = SerialPort.GetPortNames();
                }
                catch (System.Exception)
                {
                    string[] newPorts = { };
                    _currentSerialPorts = newPorts;
                    return true;
                }

                // Check the number difference of serial ports
                if (_currentSerialPorts.Length != ports.Length)
                {
                    // Serial port connections changed
                    // Reset the current serial ports
                    _currentSerialPorts = ports;

                    // Return true indicating the change
                    return true;
                }

                // Number of serial ports is the same
                // Check the names of the serial ports
                bool hasChanges = false;
                for (int i = 0; i < ports.Length; ++i)
                {
                    if (_currentSerialPorts[i] != ports[i])
                    {
                        // Port name difference detected
                        hasChanges = true;

                        // Reset the current serial ports
                        _currentSerialPorts = ports;
                        break;
                    }
                }

                // Return the detection result
                return hasChanges;
            }
        }

        /// <summary>
        /// 获取一个 string[] 数组，表示当前连接的所有可用串行接口名。（例如：COM1。）
        /// 若当前没有任何可用串行接口，则返回一个空的 string[] 数组。
        /// </summary>
        public static string[] CurrentSerialPorts
        {
            get
            {
                // Detect the latest serial port connection information
                try
                {
                    _currentSerialPorts = SerialPort.GetPortNames();
                }
                catch (System.Exception)
                {
                    string[] newPorts = { };
                    _currentSerialPorts = newPorts;
                }
                
                // Return the detection result
                return _currentSerialPorts;
            }
        }

        /// <summary>
        /// 获取一个 int 值，表示当前连接的所有可用串行接口的数量。
        /// </summary>
        public static int Count
        {
            get
            {
                // Detect the latest serial port connection information
                _currentSerialPorts = SerialPort.GetPortNames();

                // Return the number of current serial ports
                return _currentSerialPorts.Length;
            }
        }


        /// <summary>
        /// 串行端口配置变更事件处理程序。
        /// </summary>
        public delegate void SerialPortConfigurationChangedEventHandler();

        /// <summary>
        /// 串行端口配置变成事件。
        /// </summary>
        public static event SerialPortConfigurationChangedEventHandler SerialPortConfigurationChangedEvent;

        /// <summary>
        /// 触发串行端口配置变成事件。
        /// </summary>
        private static void OnSerialPortConfigurationChangedEvent()
        {
            if (SerialPortConfigurationChangedEvent != null) SerialPortConfigurationChangedEvent();
        }

        /// <summary>
        /// 获取或设置通讯端口，包括但不限于所有可用的 COM 端口。
        /// 在配置更改后，数据自动接收将会被中断，须要重新开始。
        /// </summary>
        /// <exception cref="Miniquad_Controller.InvalidSerialPortException">当设置的端口不可用时抛出此异常。</exception>
        /// <exception cref="System.ArgumentNullException">当 PortName 属性已设置为 null 时抛出此异常。</exception>
        /// <exception cref="System.InvalidOperationException">当指定的端口已打开时抛出此异常。</exception>
        public static string PortName
        {
            get
            {
                return _port.PortName;
            }
            set
            {
                // Close the serial port
                if (_port.IsOpen) _port.Close();

                // Refresh the current serial ports
                _currentSerialPorts = SerialPort.GetPortNames();

                // Try to reset the selected port name
                try
                {
                    _port.PortName = value;
                }
                catch (ArgumentNullException eArgumentNull)
                {
                    throw eArgumentNull;
                }
                catch (ArgumentException)
                {
                    throw new InvalidSerialPortException(value, _currentSerialPorts,
                        "Miniquad_Controller.SerialCommunication.SerialPortController.SelectedPortName: The port specified is not within the available serial ports.");
                }
                catch (InvalidOperationException eInvalidOperation)
                {
                    throw eInvalidOperation;
                }

                // Trigger configuration changed event
                OnSerialPortConfigurationChangedEvent();
            }
        }

        /// <summary>
        /// 获取或设置读取操作未完成时发生超时之前的毫秒数。
        /// 默认为 InfiniteTimeout 类型，即不会超时。
        /// 在配置更改后，数据自动接收将会被中断，须要重新开始。
        /// 该操作会触发 SerialPortConfigurationChangedEvent 事件。
        /// </summary>
        /// <exception cref="System.IO.IOException">此端口处于无效状态；或尝试设置基础端口状态失败（例如，从此 SerialPort 对象传递的参数无效）时抛出此异常。</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">读取超时值小于零，且不等于 InfiniteTimeout 时抛出此异常。</exception>
        public static int ReadTimeout
        {
            get
            {
                return _port.ReadTimeout;
            }
            set
            {
                // Close the port
                if (_port.IsOpen) _port.Close();

                try
                {
                    _port.ReadTimeout = value;
                }
                catch (IOException eIO)
                {
                    throw eIO;
                }
                catch (ArgumentOutOfRangeException eArgumentOutOfRange)
                {
                    throw eArgumentOutOfRange;
                }

                // Trigger configuration changed event
                //OnSerialPortConfigurationChangedEvent();
            }
        }

        /// <summary>
        /// 获取或设置写入操作未完成时发生超时之前的毫秒数。
        /// 默认为 InfiniteTimeout 类型，即不会超时。
        /// 在配置更改后，数据自动接收将会被中断，须要重新开始。
        /// 该操作会触发 SerialPortConfigurationChangedEvent 事件。
        /// </summary>
        /// <exception cref="System.IO.IOException">此端口处于无效状态；或尝试设置基础端口状态失败（例如，从此 SerialPort 对象传递的参数无效）时抛出此异常。</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">读取超时值小于零，且不等于 InfiniteTimeout 时抛出此异常。</exception>
        public static int WriteTimeout
        {
            get
            {
                return _port.WriteTimeout;
            }
            set
            {
                // Close the port
                if (_port.IsOpen) _port.Close();

                try
                {
                    _port.WriteTimeout = value;
                }
                catch (IOException eIO)
                {
                    throw eIO;
                }
                catch (ArgumentOutOfRangeException eArgumentOutOfRange)
                {
                    throw eArgumentOutOfRange;
                }

                // Trigger configuration changed event
                //OnSerialPortConfigurationChangedEvent();
            }
        }

        /// <summary>
        /// 获取或设置串行波特率。
        /// 默认值为 9600 比特/每秒 (bps)。
        /// 在配置更改后，数据自动接收将会被中断，须要重新开始。
        /// 该操作会触发 SerialPortConfigurationChangedEvent 事件。
        /// </summary>
        /// <exception cref="System.IO.IOException">此端口处于无效状态；或尝试设置基础端口状态失败（例如，从此 SerialPort 对象传递的参数无效）时抛出此异常。</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">指定的波特率小于或等于零，或者大于设备所允许的最大波特率时抛出此异常。</exception>
        public static int BaudRate
        {
            get
            {
                return _port.BaudRate;
            }
            set
            {
                // Close the port
                if (_port.IsOpen) _port.Close();

                try
                {
                    _port.BaudRate = value;
                }
                catch (IOException eIO)
                {
                    throw eIO;
                }
                catch (ArgumentOutOfRangeException eArgumentOutOfRange)
                {
                    throw eArgumentOutOfRange;
                }

                // Trigger configuration changed event
                OnSerialPortConfigurationChangedEvent();
            }
        }

        /// <summary>
        /// 获取或设置每个字节的标准数据位长度。
        /// 默认值为 8。
        /// 在配置更改后，数据自动接收将会被中断，须要重新开始。
        /// 该操作会触发 SerialPortConfigurationChangedEvent 事件。
        /// </summary>
        /// <exception cref="System.IO.IOException">此端口处于无效状态；或尝试设置基础端口状态失败（例如，从此 SerialPort 对象传递的参数无效）时抛出此异常。</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">传递的 DataBits 值不是 DataBits 枚举中的有效值时抛出此异常。</exception>
        public static DataBits DataBits
        {
            get
            {
                return (DataBits)_port.DataBits;
            }
            set
            {
                // Close the port
                if (_port.IsOpen) _port.Close();

                try
                {
                    _port.DataBits = (int)value;
                    // Here, it is impossible to trigger System.ArgumentOutOfRangeException.
                }
                catch (IOException eIO)
                {
                    throw eIO;
                }
                catch (ArgumentOutOfRangeException eArgumentOutOfRange)
                {
                    throw eArgumentOutOfRange;
                }

                // Trigger configuration changed event
                OnSerialPortConfigurationChangedEvent();
            }
        }

        /// <summary>
        /// 获取或设置每个字节的标准停止位数。
        /// 在配置更改后，数据自动接收将会被中断，须要重新开始。
        /// 该操作会触发 SerialPortConfigurationChangedEvent 事件。
        /// </summary>
        /// <exception cref="System.IO.IOException">此端口处于无效状态；或尝试设置基础端口状态失败（例如，从此 SerialPort 对象传递的参数无效）时抛出此异常。</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">StopBits 值为 StopBits.None 时抛出此异常。</exception>
        public static StopBits StopBits
        {
            get
            {
                return _port.StopBits;
            }
            set
            {
                // Close the port
                if (_port.IsOpen) _port.Close();

                try
                {
                    _port.StopBits = value;
                }
                catch (IOException eIO)
                {
                    throw eIO;
                }
                catch (ArgumentOutOfRangeException eArgumentOutOfRange)
                {
                    throw eArgumentOutOfRange;
                }

                // Trigger configuration changed event
                OnSerialPortConfigurationChangedEvent();
            }
        }

        /// <summary>
        /// 获取或设置奇偶校验检查协议。
        /// 在配置更改后，数据自动接收将会被中断，须要重新开始。
        /// 该操作会触发 SerialPortConfigurationChangedEvent 事件。
        /// </summary>
        /// <exception cref="System.IO.IOException">此端口处于无效状态；或尝试设置基础端口状态失败（例如，从此 SerialPort 对象传递的参数无效）时抛出此异常。</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">传递的 Parity 值不是 Parity 枚举中的有效值时抛出此异常。</exception>
        public static Parity Parity
        {
            get
            {
                return _port.Parity;
            }
            set
            {
                // Close the port
                if (_port.IsOpen) _port.Close();

                try
                {
                    _port.Parity = value;
                }
                catch (IOException eIO)
                {
                    throw eIO;
                }
                catch (ArgumentOutOfRangeException eArgumentOutOfRange)
                {
                    throw eArgumentOutOfRange;
                }

                // Trigger configuration changed event
                OnSerialPortConfigurationChangedEvent();
            }
        }

        /// <summary>
        /// 获取一个 bool 值，表示当前端口配置是否有效。
        /// </summary>
        public static bool IsReady
        {
            get
            {
                // Try to open the serial port
                try
                {
                    // Open the serial port
                    if (!_port.IsOpen) _port.Open();

                    // Return the current state
                    return _port.IsOpen;
                }
                catch (System.Exception)
                {
                    // The configuration is not valid
                    return false;
                }
            }
        }


        /// <summary>
        /// 从端口接收到数据接受请求时的处理程序。
        /// </summary>
        /// <param name="newReceivedBytes">从端口接收到的新数据。</param>
        public delegate void DataReceivedEventHandler(byte[] newReceivedBytes);

        /// <summary>
        /// 端口接收到数据时触发的事件。
        /// </summary>
        public static event DataReceivedEventHandler DataReceivedEvent;

        /// <summary>
        /// 串行端口有数据接收时的处理程序。
        /// </summary>
        /// <param name="sender">触发该事件的串行端口。</param>
        /// <param name="e">事件参数。</param>
        private static void _receiveSerialData(object sender, SerialDataReceivedEventArgs e)
        {
            // Receive the data to the buffer
            int bytesToRead = ((SerialPort)sender).BytesToRead;
            byte[] newReceivedBytes = new byte[bytesToRead];
            ((SerialPort)sender).Read(newReceivedBytes, 0, bytesToRead);
            _receivedDataBuffer.AddRange(newReceivedBytes);

            // Check the buffer size limit
            if (_receivedDataBuffer.Count > _receivedDataBufferSize)
            {
                _receivedDataBuffer.RemoveRange(0, _receivedDataBuffer.Count - _receivedDataBufferSize);
            }

            // Trigger data received event and give the latest received data
            DataReceivedEvent(newReceivedBytes);
        }

        /// <summary>
        /// 在编码的基础上，读取 SerialPort 对象的流和输入缓冲区中所有立即可用的字节。
        /// </summary>
        /// <returns>返回一个 string 表示 SerialPort 对象的流和输入缓冲区的内容。</returns>
        /// <exception cref="System.UnauthorizedAccessException">对端口的访问被拒绝；或当前进程或系统上的另一个进程已经打开了指定的 COM 端口（通过 SerialPort 实例或在非托管代码中）时抛出此异常。</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">此实例的一个或多个属性无效。 例如，Parity、DataBits 或 Handshake 属性不是有效值；BaudRate 小于或等于零；ReadTimeout 或 WriteTimeout 属性小于零且不是 InfiniteTimeout。</exception>
        /// <exception cref="System.ArgumentException">端口名称不是以“COM”开始的；或端口的文件类型不受支持时抛出此异常。</exception>
        /// <exception cref="System.IO.IOException">此端口处于无效状态；或尝试设置基础端口状态失败（例如，从此 SerialPort 对象传递的参数无效）时抛出此异常。</exception>
        /// <exception cref="Miniquad_Controller.HardwareException">当程序正在读取内容时，设备连接丢失，则会抛出此异常。</exception>
        public static string ReadExisting()
        {
            // Open the serial port
            try
            {
                if (!_port.IsOpen) _port.Open();
            }
            catch (UnauthorizedAccessException eUnauthorizedAccess)
            {
                throw eUnauthorizedAccess;
            }
            catch (ArgumentOutOfRangeException eArgumentOutOfRange)
            {
                throw eArgumentOutOfRange;
            }
            catch (ArgumentException eArgument)
            {
                throw eArgument;
            }
            catch (IOException eIO)
            {
                throw eIO;
            }

            string readText;
            try
            {
                // Read all existing text from the serial port
                readText = _port.ReadExisting();

                // Close the serial port
                //_port.Close();
            }
            catch (InvalidOperationException)
            {
                throw new HardwareException("Miniquad_Controller.SerialCommunication.SerialPortController.ReadExisting(): Device pulled out while reading data.");
            }

            // Return the result
            return readText;
        }

        /// <summary>
        /// 从输入缓冲区中读入一行字符串。（不包括换行符。）
        /// </summary>
        /// <returns>从输入缓冲区读入的字符串。</returns>
        /// <exception cref="System.UnauthorizedAccessException">对端口的访问被拒绝；或当前进程或系统上的另一个进程已经打开了指定的 COM 端口（通过 SerialPort 实例或在非托管代码中）时抛出此异常。</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">此实例的一个或多个属性无效。 例如，Parity、DataBits 或 Handshake 属性不是有效值；BaudRate 小于或等于零；ReadTimeout 或 WriteTimeout 属性小于零且不是 InfiniteTimeout。</exception>
        /// <exception cref="System.ArgumentException">端口名称不是以“COM”开始的；或端口的文件类型不受支持时抛出此异常。</exception>
        /// <exception cref="System.IO.IOException">此端口处于无效状态；或尝试设置基础端口状态失败（例如，从此 SerialPort 对象传递的参数无效）时抛出此异常。</exception>
        /// <exception cref="System.TimeoutException">当 WriteLine 方法超时未能写入流时抛出此异常。</exception>
        /// <exception cref="Miniquad_Controller.HardwareException">当程序正在读取内容时，设备连接丢失，则会抛出此异常。</exception>
        public static string ReadLine()
        {
            // Open the serial port
            try
            {
                if (!_port.IsOpen) _port.Open();
            }
            catch (UnauthorizedAccessException eUnauthorizedAccess)
            {
                throw eUnauthorizedAccess;
            }
            catch (ArgumentOutOfRangeException eArgumentOutOfRange)
            {
                throw eArgumentOutOfRange;
            }
            catch (ArgumentException eArgument)
            {
                throw eArgument;
            }
            catch (IOException eIO)
            {
                throw eIO;
            }

            string readText;
            try
            {
                // Read a string until the new line from the device
                readText = _port.ReadLine();

                // Close the serial port
                //_port.Close();

                // Return the string
                return readText;
            }
            catch (TimeoutException eTimeout)
            {
                throw eTimeout;
            }
            catch (InvalidOperationException)
            {
                throw new HardwareException("Miniquad_Controller.SerialCommunication.SerialPortController.ReadExisting(): Device pulled out while reading data.");
            }
        }

        /// <summary>
        /// 将指定的字符串及信息结尾写入输出缓冲区中。
        /// </summary>
        /// <param name="text">要写入的指定字符串。</param>
        /// <param name="messageEnding">信息结尾。</param>
        /// <exception cref="System.UnauthorizedAccessException">对端口的访问被拒绝；或当前进程或系统上的另一个进程已经打开了指定的 COM 端口（通过 SerialPort 实例或在非托管代码中）时抛出此异常。</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">此实例的一个或多个属性无效。 例如，Parity、DataBits 或 Handshake 属性不是有效值；BaudRate 小于或等于零；ReadTimeout 或 WriteTimeout 属性小于零且不是 InfiniteTimeout。</exception>
        /// <exception cref="System.ArgumentException">端口名称不是以“COM”开始的；或端口的文件类型不受支持时抛出此异常。</exception>
        /// <exception cref="System.IO.IOException">此端口处于无效状态；或尝试设置基础端口状态失败（例如，从此 SerialPort 对象传递的参数无效）时抛出此异常。</exception>
        /// <exception cref="System.ArgumentNullException">str 参数为 null 时抛出此异常。</exception>
        /// <exception cref="System.TimeoutException">当 WriteLine 方法超时未能写入流时抛出此异常。</exception>
        /// <exception cref="Miniquad_Controller.HardwareException">当程序正在读取内容时，设备连接丢失，则会抛出此异常。</exception>
        public static void Write(string text, MessageEnding messageEnding = MessageEnding.NoEnding)
        {
            // Open the serial port
            try
            {
                if (!_port.IsOpen) _port.Open();
            }
            catch (UnauthorizedAccessException eUnauthorizedAccess)
            {
                throw eUnauthorizedAccess;
            }
            catch (ArgumentOutOfRangeException eArgumentOutOfRange)
            {
                throw eArgumentOutOfRange;
            }
            catch (ArgumentException eArgument)
            {
                throw eArgument;
            }
            catch (IOException eIO)
            {
                throw eIO;
            }

            try
            {
                switch (messageEnding)
                {
                    case MessageEnding.NoEnding:
                        _port.Write(text);
                        break;
                    case MessageEnding.NewLine:
                        _port.Write(text + "\n");
                        break;
                    case MessageEnding.CarrigeReturn:
                        _port.Write(text + "\r");
                        break;
                    case MessageEnding.NewLineAndCarrigeReturn:
                        _port.Write(text + "\r\n");
                        break;
                    default:
                        _port.Write(text);
                        break;
                }
            }
            catch (ArgumentNullException eArgumentNull)
            {
                throw eArgumentNull;
            }
            catch (TimeoutException eTimeout)
            {
                throw eTimeout;
            }
            catch (InvalidOperationException)
            {
                throw new HardwareException("Miniquad_Controller.SerialCommunication.SerialPortController.Write(string, MessageEnding): Device pulled out while reading data.");
            }
        }

        /// <summary>
        /// 将指定的字节及信息结尾写入输出缓冲区中。
        /// </summary>
        /// <param name="buffer">要写入的指定字节。</param>
        /// <param name="offset">buffer 从零开始的字节偏移，将从该偏移开始将字节复制到端口。</param>
        /// <param name="count">要写入的字节数。</param>
        /// <exception cref="System.UnauthorizedAccessException">对端口的访问被拒绝；或当前进程或系统上的另一个进程已经打开了指定的 COM 端口（通过 SerialPort 实例或在非托管代码中）时抛出此异常。</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">此实例的一个或多个属性无效。 例如，Parity、DataBits 或 Handshake 属性不是有效值；BaudRate 小于或等于零；ReadTimeout 或 WriteTimeout 属性小于零且不是 InfiniteTimeout。</exception>
        /// <exception cref="System.ArgumentException">端口名称不是以“COM”开始的；或端口的文件类型不受支持时抛出此异常。</exception>
        /// <exception cref="System.IO.IOException">此端口处于无效状态；或尝试设置基础端口状态失败（例如，从此 SerialPort 对象传递的参数无效）时抛出此异常。</exception>
        /// <exception cref="System.ArgumentNullException">传递的 buffer 为 null。</exception>
        /// <exception cref="Miniquad_Controller.HardwareException">当程序正在读取内容时，设备连接丢失，则会抛出此异常。</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">offset 或 count 参数超出了所传递的 buffer 的有效区域。 offset 或 count 小于零。</exception>
        /// <exception cref="System.ArgumentException">offset 加上 count 大于 buffer 的长度。</exception>
        /// <exception cref="System.TimeoutException">该操作未在超时时间到期之前完成。</exception>
        public static void Write(byte[] buffer, int offset, int count)
        {
            // Open the serial port
            try
            {
                if (!_port.IsOpen) _port.Open();
            }
            catch (UnauthorizedAccessException eUnauthorizedAccess)
            {
                throw eUnauthorizedAccess;
            }
            catch (ArgumentOutOfRangeException eArgumentOutOfRange)
            {
                throw eArgumentOutOfRange;
            }
            catch (ArgumentException eArgument)
            {
                throw eArgument;
            }
            catch (IOException eIO)
            {
                throw eIO;
            }

            try
            {
                _port.Write(buffer, offset, count);
            }
            catch (ArgumentNullException eArgumentNull)
            {
                throw eArgumentNull;
            }
            catch (InvalidOperationException)
            {
                throw new HardwareException("Miniquad_Controller.SerialCommunication.SerialPortController.Write(string, MessageEnding): Device pulled out while reading data.");
            }
            catch (ArgumentOutOfRangeException eArgumentOutOfRange)
            {
                throw eArgumentOutOfRange;
            }
            catch (ArgumentException eArgument)
            {
                throw eArgument;
            }
            catch (TimeoutException eTimeout)
            {
                throw eTimeout;
            }
        }

        /// <summary>
        /// 将指定的字符串和 NewLine 值写入输出缓冲区。（只有“NL”无“CR”）
        /// </summary>
        /// <exception cref="System.UnauthorizedAccessException">对端口的访问被拒绝；或当前进程或系统上的另一个进程已经打开了指定的 COM 端口（通过 SerialPort 实例或在非托管代码中）时抛出此异常。</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">此实例的一个或多个属性无效。 例如，Parity、DataBits 或 Handshake 属性不是有效值；BaudRate 小于或等于零；ReadTimeout 或 WriteTimeout 属性小于零且不是 InfiniteTimeout。</exception>
        /// <exception cref="System.ArgumentException">端口名称不是以“COM”开始的；或端口的文件类型不受支持时抛出此异常。</exception>
        /// <exception cref="System.IO.IOException">此端口处于无效状态；或尝试设置基础端口状态失败（例如，从此 SerialPort 对象传递的参数无效）时抛出此异常。</exception>
        /// <exception cref="System.ArgumentNullException">str 参数为 null 时抛出此异常。</exception>
        /// <exception cref="System.TimeoutException">当 WriteLine 方法超时未能写入流时抛出此异常。</exception>
        /// <exception cref="Miniquad_Controller.HardwareException">当程序正在读取内容时，设备连接丢失，则会抛出此异常。</exception>
        public static void WriteLine(string text)
        {
            // Open the serial port
            try
            {
                if (!_port.IsOpen) _port.Open();
            }
            catch (UnauthorizedAccessException eUnauthorizedAccess)
            {
                throw eUnauthorizedAccess;
            }
            catch (ArgumentOutOfRangeException eArgumentOutOfRange)
            {
                throw eArgumentOutOfRange;
            }
            catch (ArgumentException eArgument)
            {
                throw eArgument;
            }
            catch (IOException eIO)
            {
                throw eIO;
            }

            try
            {
                // Write the text and a new line to the device
                _port.WriteLine(text);

                // Close the serial port
                //_port.Close();
            }
            catch (ArgumentNullException eArgumentNull)
            {
                throw eArgumentNull;
            }
            catch (TimeoutException eTimeout)
            {
                throw eTimeout;
            }
            catch (InvalidOperationException)
            {
                throw new HardwareException("Miniquad_Controller.SerialCommunication.SerialPortController.WriteLine(string): Device pulled out while reading data.");
            }
        }

        /// <summary>
        /// 获取从串口接收到的数据缓冲块的数据。
        /// </summary>
        public static List<byte> ReceivedDataBuffer
        {
            get { return _receivedDataBuffer; }
        }

        /// <summary>
        /// 获取或设置从串口接收到的数据缓冲块的大小。
        /// 最小为 32 字节。默认为 1024 字节。
        /// </summary>
        /// <exception cref="System.ArgumentOutOfRangeException">当缓冲区大小小于 32 字节时抛出此异常。</exception>
        public static int ReceivedDataBufferSize
        {
            get { return _receivedDataBufferSize; }
            set
            {
                if (value < 32) throw new ArgumentOutOfRangeException("Miniquad_Controller.SerialCommunication.SerialPortController.ReceivedDataBufferSize: The size of received data buffer cannot be less than 32 bytes.");

                // Reset the capacity of the buffer
                if (value + 128 >= _receivedDataBuffer.Count) _receivedDataBuffer.Capacity = value + 128;

                // Change the size limit
                _receivedDataBufferSize = value;
            }
        }

        /// <summary>
        /// 开始自动接收端口数据。
        /// 所有接收到的数据都会被存储于接收数据缓冲块里面，当达到缓冲块大小限制时，会自动删除超出长度的历史数据。
        /// 数据在自动接收时会触发 DataReceivedEvent 事件，新接收到的数据会通过该事件的处理程序交由用户处理。
        /// </summary>
        /// <exception cref="System.UnauthorizedAccessException">对端口的访问被拒绝；或当前进程或系统上的另一个进程已经打开了指定的 COM 端口（通过 SerialPort 实例或在非托管代码中）时抛出此异常。</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">此实例的一个或多个属性无效。 例如，Parity、DataBits 或 Handshake 属性不是有效值；BaudRate 小于或等于零；ReadTimeout 或 WriteTimeout 属性小于零且不是 InfiniteTimeout。</exception>
        /// <exception cref="System.ArgumentException">端口名称不是以“COM”开始的；或端口的文件类型不受支持时抛出此异常。</exception>
        /// <exception cref="System.IO.IOException">此端口处于无效状态；或尝试设置基础端口状态失败（例如，从此 SerialPort 对象传递的参数无效）时抛出此异常。</exception>
        public static void StartReceivingData()
        {
            // Bind handler
            _port.DataReceived -= _receiveSerialData;
            _port.DataReceived += _receiveSerialData;

            // Open the serial port
            try
            {
                if (!_port.IsOpen) _port.Open();
            }
            catch (UnauthorizedAccessException eUnauthorizedAccess)
            {
                throw eUnauthorizedAccess;
            }
            catch (ArgumentOutOfRangeException eArgumentOutOfRange)
            {
                throw eArgumentOutOfRange;
            }
            catch (ArgumentException eArgument)
            {
                throw eArgument;
            }
            catch (IOException eIO)
            {
                throw eIO;
            }
        }

        /// <summary>
        /// 停止指定处理程序对自动接收到的端口数据。
        /// </summary>
        public static void StopReceivingData()
        {
            _port.DataReceived -= _receiveSerialData;
        }

        /// <summary>
        /// 清空接收数据缓冲块。
        /// </summary>
        public static void ClearReceivedDataBuffer()
        {
            _receivedDataBuffer.Clear();
        }
    }
}
