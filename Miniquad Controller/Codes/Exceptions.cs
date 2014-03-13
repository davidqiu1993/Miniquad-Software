using System;

namespace Miniquad_Controller
{
    /// <summary>
    /// 使用了不可用的串行端口时引发的异常。
    /// </summary>
    class InvalidSerialPortException : Exception
    {
        protected string _invalidSerialPort = null;
        protected string[] _currentAvailableSerialPorts = null;


        /// <summary>
        /// 获取或设置被错误使用的不可用的串行端口。
        /// </summary>
        public string InvalidSerialPort
        {
            get { return _invalidSerialPort; }
            set { _invalidSerialPort = value; }
        }

        /// <summary>
        /// 获取或设置当前可用的所有串口。
        /// </summary>
        public string[] CurrentAvailableSerialPorts
        {
            get { return _currentAvailableSerialPorts; }
            set { _currentAvailableSerialPorts = value; }
        }
        

        /// <summary>
        /// 实例化一个新的 InvalidSerialPortException 类的对象。
        /// </summary>
        public InvalidSerialPortException()
        {
            ;
        }

        /// <summary>
        /// 使用引发此异常的端口名和当前可用的端口数组实例化一个新的 InvalidSerialPortException 类的对象。
        /// </summary>
        /// <param name="invalidSerialPort">引发此异常的被错误使用的不可用端口的端口名。</param>
        /// <param name="currentAvailableSerialPorts">当前可以使用的所有端口的端口名数组。</param>
        public InvalidSerialPortException(string invalidSerialPort, string[] currentAvailableSerialPorts)
        {
            _invalidSerialPort = InvalidSerialPort;
            _currentAvailableSerialPorts = currentAvailableSerialPorts;
        }
        
        /// <summary>
        /// 使用引发此异常的端口名、当前可用的端口数组和指定的错误反馈信息实例化一个新的 InvalidSerialPortException 类的对象。
        /// </summary>
        /// <param name="invalidSerialPort">引发此异常的被错误使用的不可用端口的端口名。</param>
        /// <param name="currentAvailableSerialPorts">当前可以使用的所有端口的端口名数组。</param>
        /// <param name="message">指定的错误反馈信息。</param>
        public InvalidSerialPortException(string invalidSerialPort, string[] currentAvailableSerialPorts, string message)
            : base(message)
        {
            _invalidSerialPort = InvalidSerialPort;
            _currentAvailableSerialPorts = currentAvailableSerialPorts;
        }
    }

    /// <summary>
    /// 硬件发生错误时引发的异常。
    /// </summary>
    class HardwareException : Exception
    {
        /// <summary>
        /// 实例化一个新的 HardwareException 类的对象。
        /// </summary>
        public HardwareException()
        {
            ;
        }

        /// <summary>
        /// 使用指定的错误反馈信息实例化一个新的 HardwareException 类的对象。
        /// </summary>
        /// <param name="message">指定的错误反馈信息。</param>
        public HardwareException(string message)
            : base(message)
        {
            ;
        }
    }
}
