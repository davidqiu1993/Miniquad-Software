using System;
using System.Collections.Generic;
using Miniquad_Controller.Math3D;
using Miniquad_Controller.SerialCommunication;
using System.Windows;

namespace Miniquad_Controller.Miniquad
{
    /// <summary>
    /// 表示飞行控制程序的计算模式。
    /// </summary>
    public enum ComputingMode
    {
        /// <summary>
        /// 上位机模式，即飞行控制程序在上位机中运算。
        /// </summary>
        PricipalComputerMode = 0,

        /// <summary>
        /// 下位机模式，即飞行控制程序在下位机中运算。
        /// </summary>
        SlaveComputerMode = 1
    }
    
    /// <summary>
    /// 表示飞行器的动力装置。
    /// </summary>
    public class Propeller
    {
        int _pinNumber;
        int _throttle;

        /// <summary>
        /// 实例化一个默认的飞行器动力装置类型对象。
        /// </summary>
        public Propeller()
        {
            _pinNumber = 0;
            _throttle = 0;
        }

        /// <summary>
        /// 使用装置引脚编号实例化一个新的飞行器动力装置类型对象。
        /// </summary>
        /// <param name="pinNumber">指定的装置引脚编号。</param>
        /// <exception cref="System.ArgumentOutOfRangeException">当引脚编号为负数时抛出此异常。</exception>
        public Propeller(int pinNumber)
        {
            // Check the pin number
            if (pinNumber < 0) throw new ArgumentOutOfRangeException("Miniquad_Controller.Miniquad.Propeller.Propeller(int): The pin number is negative.");

            // Set the parameters
            _pinNumber = pinNumber;
            _throttle = 0;
        }

        /// <summary>
        /// 获取或设置动力装置的引脚编号。
        /// 必须为非负整数。
        /// </summary>
        /// <exception cref="System.ArgumentOutOfRangeException">当引脚编号为负数时抛出此异常。</exception>
        public int PinNumber
        {
            get { return _pinNumber; }
            set
            {
                if (value < 0) throw new ArgumentOutOfRangeException("Miniquad_Controller.Miniquad.Propeller.PinNumber: The pin number is negative.");
                _pinNumber = value;
            }
        }

        /// <summary>
        /// 获取或设置动力装置的油门输出。
        /// 范围为 0 - 255。
        /// </summary>
        /// <exception cref="System.ArgumentOutOfRangeException">当油门输出超出 0 - 255 的范围时抛出此异常。</exception>
        public int Throttle
        {
            get { return _throttle; }
            set
            {
                if (value < 0 || value > 255) throw new ArgumentOutOfRangeException("Miniquad_Controller.Miniquad.Propeller.Throttle: The throttle output is not between 0 and 255.");
                _throttle = value;
            }
        }
    }

    /// <summary>
    /// 表示飞行器状态。
    /// </summary>
    public class MiniquadStatus
    {
        protected Quaternion _quaternion = new Quaternion();
        protected Rotation _rotation= new Rotation();
        protected Acceleration _acceleration= new Acceleration();
        protected Propeller[] _propellers;


        /// <summary>
        /// 实例化一个默认的飞行器状态。
        /// </summary>
        public MiniquadStatus()
        {
            _propellers = new Propeller[4];
            for (int i = 0; i < 4; ++i) _propellers[i] = new Propeller();
        }

        /// <summary>
        /// 使用四个飞行器动力装置的引脚编号实例化一个新的飞行器状态。
        /// </summary>
        /// <param name="propeller1Pin">飞行器动力装置1的引脚编号。</param>
        /// <param name="propeller2Pin">飞行器动力装置2的引脚编号。</param>
        /// <param name="propeller3Pin">飞行器动力装置3的引脚编号。</param>
        /// <param name="propeller4Pin">飞行器动力装置4的引脚编号。</param>
        public MiniquadStatus(int propeller1Pin, int propeller2Pin, int propeller3Pin, int propeller4Pin)
        {
            _propellers = new Propeller[4];
            for (int i = 0; i < 4; ++i) _propellers[i] = new Propeller();
            _propellers[0].PinNumber = propeller1Pin;
            _propellers[1].PinNumber = propeller2Pin;
            _propellers[2].PinNumber = propeller3Pin;
            _propellers[3].PinNumber = propeller4Pin;
        }


        /// <summary>
        /// 获取或设置表示飞行器姿态的四元数。
        /// </summary>
        public Quaternion Quaternion
        {
            get { return _quaternion; }
            set { _quaternion = value; }
        }

        /// <summary>
        /// 获取关于飞行器姿态的欧拉角。（可通过设置表示飞行器姿态的四元数改变。）
        /// </summary>
        public EulerAngle EulerAngle
        {
            get { return _quaternion.GetEulerAngle(); }
        }

        /// <summary>
        /// 获取飞行器在 X、Y、Z 轴上的重力分量。（可通过设置表示飞行器姿态的四元数改变。）
        /// </summary>
        public Gravity Gravity
        {
            get { return _quaternion.GetGravity(); }
        }

        /// <summary>
        /// 获取飞行器的偏航角、俯仰角和旋转角。（可通过设置表示飞行器姿态的四元数改变。）
        /// </summary>
        public YawPitchRoll YawPitchRoll
        {
            get { return _quaternion.GetYawPitchRoll(); }
        }

        /// <summary>
        /// 获取或设置飞行器关于 X、Y、Z 轴的旋转角速度。
        /// </summary>
        public Rotation Rotation
        {
            get { return _rotation; }
            set { _rotation = value; }
        }

        /// <summary>
        /// 获取或设置飞行器在 X、Y、Z 轴上的线加速度。
        /// </summary>
        public Acceleration Acceleration
        {
            get { return _acceleration; }
            set { _acceleration = value; }
        }


        /// <summary>
        /// 获取或设置飞行器1号动力装置信息。
        /// </summary>
        public Propeller Propeller1
        {
            get { return _propellers[0]; }
            set { _propellers[0] = value; }
        }

        /// <summary>
        /// 获取或设置飞行器2号动力装置信息。
        /// </summary>
        public Propeller Propeller2
        {
            get { return _propellers[1]; }
            set { _propellers[1] = value; }
        }

        /// <summary>
        /// 获取或设置飞行器3号动力装置信息。
        /// </summary>
        public Propeller Propeller3
        {
            get { return _propellers[2]; }
            set { _propellers[2] = value; }
        }

        /// <summary>
        /// 获取或设置飞行器4号动力装置信息。
        /// </summary>
        public Propeller Propeller4
        {
            get { return _propellers[3]; }
            set { _propellers[3] = value; }
        }
    }

    /// <summary>
    /// 表示上位机与飞行器间的通讯机制。
    /// </summary>
    public static class Communication
    {
        private static byte[] _receivedData = new byte[48];
        private static byte[] _sentDataInPrincipalComputerMode = new byte[12];
        private static byte[] _sentDataInSlaveComputerMode = new byte[6];


        /// <summary>
        /// 从缓冲块中接收最新的一次数据，并分析是否存在状态记录。
        /// </summary>
        /// <param name="bufferBytes">缓冲区里面的数据。</param>
        /// <returns>返回一个 bool 值表示该次数据接收是否成功并存在状态记录。</returns>
        public static bool ReceiveFromBuffer(List<byte> bufferBytes)
        {
            // =========================== [ Received Data Structure ] ============================= //
            //                                                                                       //
            //    '$', 0x02, [Quaternion: 4*float (w,x,y,z)], [Rotation: 3*float (x,y,z)],           //
            //                                                                                       //
            //    [Acceleration: 3*float (x,y,z)], [Throttle: 4*int16_t (1,2,3,4)], '\r', '\n'       //
            //                                                                                       //
            // ===================================================================================== //

            // Scan from the back of the buffer
            for (int i = bufferBytes.Count - 1; i >= 51; --i)
            {
                // Find the last format-matched record
                if (bufferBytes[i] == BitConverter.GetBytes('\n')[0] &&
                    bufferBytes[i - 1] == BitConverter.GetBytes('\r')[0] &&
                    bufferBytes[i - 50] == BitConverter.GetBytes(((char)2))[0] &&
                    bufferBytes[i - 51] == BitConverter.GetBytes('$')[0])
                {
                    // Record found
                    // Copy to the received data bytes
                    for (int j = 0; j < 48; ++j)
                    {
                        _receivedData[j] = bufferBytes[i - 49 + j];
                    }

                    // Return success
                    return true;
                }
                //MessageBox.Show(bufferBytes[i-51].ToString() + " " + bufferBytes[i - 50].ToString() + " " + bufferBytes[i - 1].ToString() +" "+ bufferBytes[i].ToString());
            }

            // Failed to find such record
            return false;
        }

        /// <summary>
        /// 获取最近一次接收到的数据中的四元数。
        /// </summary>
        /// <returns>返回一个 Quaternion 表示最近一次接收到的数据中的四元数。</returns>
        public static Quaternion GetQuaternion()
        {
            return new Quaternion(
                (double)BitConverter.ToSingle(_receivedData, 0),
                (double)BitConverter.ToSingle(_receivedData, 4),
                (double)BitConverter.ToSingle(_receivedData, 8),
                (double)BitConverter.ToSingle(_receivedData, 12));
        }

        /// <summary>
        /// 获取最近一次接收到的数据中的旋转速度。
        /// </summary>
        /// <returns>返回一个 Rotation 表示最近一次接收到的数据中的旋转速度。</returns>
        public static Rotation GetRotation()
        {
            return new Rotation(
                (double)BitConverter.ToSingle(_receivedData, 16),
                (double)BitConverter.ToSingle(_receivedData, 20),
                (double)BitConverter.ToSingle(_receivedData, 24));
        }

        /// <summary>
        /// 获取最近一次接收到的数据中的线加速度。
        /// </summary>
        /// <returns>返回一个 Acceleration 表示最近一次接收到的数据中的线加速度。</returns>
        public static Acceleration GetAcceleration()
        {
            return new Acceleration(
                (double)BitConverter.ToSingle(_receivedData, 28),
                (double)BitConverter.ToSingle(_receivedData, 32),
                (double)BitConverter.ToSingle(_receivedData, 36));
        }

        /// <summary>
        /// 获取最近一次接收到的数据中的油门输出。
        /// </summary>
        /// <returns>返回一个 int[] 表示最近一次接收到的数据中的四个动力装置的油门输出。</returns>
        public static int[] GetThrottleOutputs()
        {
            int[] throttleOutputs = new int[4];
            throttleOutputs[0] = (int)BitConverter.ToUInt16(_receivedData, 40);
            throttleOutputs[1] = (int)BitConverter.ToUInt16(_receivedData, 42);
            throttleOutputs[2] = (int)BitConverter.ToUInt16(_receivedData, 44);
            throttleOutputs[3] = (int)BitConverter.ToUInt16(_receivedData, 46);
            return throttleOutputs;
        }


        /// <summary>
        /// 设置四个螺旋桨的油门输出。（上位机模式）
        /// </summary>
        /// <param name="throttle1">第1号螺旋桨的油门输出。</param>
        /// <param name="throttle2">第2号螺旋桨的油门输出。</param>
        /// <param name="throttle3">第3号螺旋桨的油门输出。</param>
        /// <param name="throttle4">第4号螺旋桨的油门输出。</param>
        /// <returns>返回一个 bool 值表示指令是否成功送出。</returns>
        /// <exception cref="System.ArgumentOutOfRangeException">当油门输出不在 0 至 255 范围内时抛出该异常。</exception>
        public static bool SetThrottleOutputs_PC(int throttle1, int throttle2, int throttle3, int throttle4)
        {
            // =================== [ Sent Data Structure ] =================== //
            //                                                                 //
            //    '@', '0x04', [Throttle: 4*uint16_t (1,2,3,4)], '\r', '\n'    //
            //                                                                 //
            // =============================================================== //

            // Check the ranges of the throttles
            if(throttle1<0 || throttle1 >255 ||
                throttle2<0 || throttle2 >255 ||
                throttle3<0 || throttle3 >255 ||
                throttle4 < 0 || throttle4 > 255)
            {
                throw new ArgumentOutOfRangeException("Miniquad_Controller.Miniquad.Communication.SetThrottleOutputs_PC: The value of throttle must be between 0 and 255.");
            }

            // Construct the message head bytes
            _sentDataInPrincipalComputerMode[0] = BitConverter.GetBytes('@')[0];
            _sentDataInPrincipalComputerMode[1] = BitConverter.GetBytes(4)[0];

            // Construct the message content bytes
            BitConverter.GetBytes((UInt16)throttle1).CopyTo(_sentDataInPrincipalComputerMode, 2);
            BitConverter.GetBytes((UInt16)throttle2).CopyTo(_sentDataInPrincipalComputerMode, 4);
            BitConverter.GetBytes((UInt16)throttle3).CopyTo(_sentDataInPrincipalComputerMode, 6);
            BitConverter.GetBytes((UInt16)throttle4).CopyTo(_sentDataInPrincipalComputerMode, 8);

            // Construct the message ending bytes
            _sentDataInPrincipalComputerMode[10] = BitConverter.GetBytes('\r')[0];
            _sentDataInPrincipalComputerMode[11] = BitConverter.GetBytes('\n')[0];

            // Send the message
            try
            {
                SerialPortController.Write(_sentDataInPrincipalComputerMode, 0, 12);
                return true;
            }
            catch (System.Exception)
            {
                return false;
            }
        }
    }

    /// <summary>
    /// 表示静态PID飞行控制算法。
    /// </summary>
    public static class StaticPIDFlyingControllingAlgorithm
    {
        // Propeller and engine characteristic parameters
        private static double _K1 = 0.000000000224;
        private static double _K2 = 0.00000000000132;
        private static int _Kr = 1; // Range: {1, -1} ==> MOTOR1 : Clockwise ->_Kr=1; Counterclockwise ->_Kr=-1
        private static double _Kt = 2925923;
        private static double _Tau = 2;

        // Propeller and engine status variables
        private static double _Throttle1 = 0;
        private static double _Throttle2 = 0;
        private static double _Throttle3 = 0;
        private static double _Throttle4 = 0;

        // Copter characteristic parameters
        private static double _m = 0.0338; // unit: kilogram
        private static double _R = 0.44; // unit: meter
        private static double _Jx = 0.0000158 / 2;
        private static double _Jy = 0.0000158 / 2;
        private static double _Jz = 0.0000158;

        //gravity constant
        private const double _g = 9.7833;

        // Computer status variables
        private static double _Angle_roll = 0, _Angle_roll_last = 0; // !SENSOR, unit: rad
        public static double Angle_roll
        {
            set { _Angle_roll_last = _Angle_roll; _Angle_roll = value / 180 * Math.PI; }
        }

        private static double _Angle_pitch = 0, _Angle_pitch_last = 0; // !SENSOR, unit: rad
        public static double Angle_pitch
        {
            set { _Angle_pitch_last = _Angle_pitch; _Angle_pitch = value / 180 * Math.PI; }
        }

        private static double _Angle_yaw = 0, _Angle_yaw_last = 0; // !SENSOR, unit: rad
        public static double Angle_yaw
        {
            set { _Angle_yaw_last = _Angle_yaw; _Angle_yaw = value / 180 * Math.PI; }
        }

        private static double _Angle_gamma = 0; // !SENSOR, unit: rad
        public static double Angle_gamma
        {
            set { _Angle_gamma = value / 180 * Math.PI; }
        }

        private static double _Accel_z = 0, _Accel_z_sum = 0; // !SENSOR, unit: m/s^2
        public static double Accel_z
        {
            set { _Accel_z = value * _g; _Accel_z_sum += _Accel_z; }
        }

        //Control strength
        private const double _A_x = -0.1, _B_x = -0.25;
        private const double _A_y = -0.1, _B_y = -0.25;
        private const double _A_z = -0.1, _B_z = -0.25;
        private const double _A_a = -0.1, _B_a = -0.25;

        // PID controlling algorithm parameters according to control strength
        private static double _KP_x = -(_A_x - 1) * (_B_x - 1) * _Jx / _T;
        private static double _KD_x = (1 - _A_x * _B_x) / _T / _T;
        private static double _Angle_roll_E = 0;
        private static double _Angle_roll_R = 0;

        private static double _KP_y = -(_A_y - 1) * (_B_y - 1) * _Jy / _T;
        private static double _KD_y = (1 - _A_y * _B_y) / _T / _T;
        private static double _Angle_pitch_E = 0;
        private static double _Angle_pitch_R = 0;

        private static double _KP_z = -(_A_z - 1) * (_B_z - 1) * _Jz / _T;
        private static double _KD_z = (1 - _A_z * _B_z) / _T / _T;
        private static double _Angle_yaw_E = 0;
        private static double _Angle_yaw_R = 0;

        private static double _KP_a = _A_a * _B_a * _m;
        private static double _KI_a = -(_A_a - 1) * (_B_a - 1) * _m;
        private static double _Accel_z_E = 0;
        private static double _Accel_z_R = 0;
        private static double _Velocity_z_E = 0;
        private static double _Velocity_z_R = 0;

        // Hardware ability related parameters
        private static double _T = 0.053;//unit: second

        // Temperate calculation variables
        private static double _X, _Y, _Z, _A;
        private static void _calculateTemprateVariables()
        {
            _X = (_KP_x * (_Angle_roll - _Angle_roll_E - _Angle_roll_R) + _KD_x / _T * (_Angle_roll - _Angle_roll_last)) / (0.70710678 * _R * _K1);
            _Y = (_KP_y * (_Angle_pitch - _Angle_pitch_E - _Angle_pitch_R) + _KD_y / _T * (_Angle_pitch - _Angle_pitch_last)) / (0.70710678 * _R * _K1);
            _Z = (_KP_z * (_Angle_yaw - _Angle_yaw_E - _Angle_yaw_R) + _KD_z / _T * (_Angle_yaw - _Angle_yaw_last)) / (_K2 * _Kr);
            _A = (_KP_a * (_Accel_z - _Accel_z_E - _Accel_z_R) + _KI_a * (_Accel_z_sum - _Velocity_z_E / _T - _Velocity_z_R / _T) + _m * _g / Math.Cos(_Angle_gamma)) / (_K1);
        }

        // Throttle outputs
        public static int[] GetThrottleOutputs()
        {
            int[] throttles = new int[4];

            throttles[0] = (int)(Math.Pow(+_A - _X + _Y + _Z, _Tau / 2) / 4 / _Kt);
            throttles[1] = (int)(Math.Pow(+_A - _X - _Y - _Z, _Tau / 2) / 4 / _Kt);
            throttles[2] = (int)(Math.Pow(+_A + _X - _Y + _Z, _Tau / 2) / 4 / _Kt);
            throttles[3] = (int)(Math.Pow(+_A + _X + _Y - _Z, _Tau / 2) / 4 / _Kt);

            for (int i = 0; i < 4; ++i)
            {
                if (throttles[i] < 0) throttles[i] = 0;
                if (throttles[i] > 255) throttles[i] = 255;
            }

            return throttles;
        }
    }

    /// <summary>
    /// 表示增量PID飞行控制算法。
    /// </summary>
    public static class IncrementPIDFlyingControllingAlgorithm
    {
        // Propeller and engine characteristic parameters
        private static int _Kr = 1; // Range: {1, -1}

        // Propeller and engine status variables
        private static double _Throttle1 = 0;
        private static double _Throttle2 = 0;
        private static double _Throttle3 = 0;
        private static double _Throttle4 = 0;
        private static double _MinThrottle = 60;
        private static double _MaxThrottle = 160;

        // Copter status variables
        private static double _Angle_roll = 0; // !SENSOR, unit: degree
        public static double Angle_roll
        {
            set {_Angle_roll = value; }
        }
        private static double _Rotation_xpi; // !SENSOR, unit: degree/s
        public static double Rotation_xpi
        {
            set { _Rotation_xpi = value; }
        }

        private static double _Angle_pitch = 0; // !SENSOR, unit: degree
        public static double Angle_pitch
        {
            set {_Angle_pitch = value; }
        }
        private static double _Rotation_ypi; // !SENSOR, unit: degree/s
        public static double Rotation_ypi
        {
            set { _Rotation_ypi = value; }
        }

        private static double _Angle_yaw = 0; // !SENSOR, unit: degree
        public static double Angle_yaw
        {
            set { _Angle_yaw = value; }
        }
        private static double _Rotation_zpi; // !SENSOR, unit: degree/s
        public static double Rotation_zpi
        {
            set { _Rotation_zpi = value; }
        }

        private static double _Accel_z = 0, _Accel_z_sum = 0; // !SENSOR, unit: g
        public static double Accel_z
        {
            set { _Accel_z = value; _Accel_z_sum += _Accel_z; }
        }

        // PID controlling algorithm parameters
        private static double _KP_x = 0.00005, _KD_x = 0.0025, _Angle_roll_E = 0, _Angle_roll_R = 0, _Rotation_xpi_E = 0, _Rotation_xpi_R = 0;
        private static double _KP_y = 0.00005, _KD_y = 0.0025, _Angle_pitch_E = 0, _Angle_pitch_R = 0, _Rotation_ypi_E = 0, _Rotation_ypi_R = 0;
        private static double _KP_z = 0.0005, _KD_z = 0.005, _Angle_yaw_E = 0, _Angle_yaw_R = 0, _Rotation_zpi_E = 0, _Rotation_zpi_R = 0;
        private static double _KP_a = 0.01, _KI_a = 2.0, _Accel_z_E = 0, _Accel_z_R = 0, _Velocity_z_E = 0, _Velocity_z_R = 0;

        // Device ability related parameters
        private static double _T = 0.053;

        // Temperate calculation variables
        private static double _W = 0, _X = 0, _Y = 0, _Z = 0;
        private static void _calculateTemperateVarables()
        {
            _W = _KP_x * (_Angle_roll - _Angle_roll_E - _Angle_roll_R) - _KD_x * (_Rotation_xpi - _Rotation_xpi_E - _Rotation_xpi_R);
            _X = _KP_y * (_Angle_pitch - _Angle_pitch_E - _Angle_pitch_R) - _KD_y * (_Rotation_ypi - _Rotation_ypi_E - _Rotation_ypi_R);
            _Y = _KP_z * (_Angle_yaw - _Angle_yaw_E - _Angle_yaw_R) - _KD_z * (_Rotation_zpi - _Rotation_zpi_E - _Rotation_zpi_R);
            _Z = _KP_a * (_Accel_z - _Accel_z_E - _Accel_z_R);// +_KI_a * (_Accel_z_sum - ((_Velocity_z_E + _Velocity_z_R) / 9.8) / _T);
        }

        // Initialization
        public static void Initialize()
        {
            // Reset the throttle outputs
            _Throttle1 = 0;
            _Throttle2 = 0;
            _Throttle3 = 0;
            _Throttle4 = 0;

            // Reset the acceleration accumulation
            _Accel_z_sum = 0;
        }

        // Throttle outputs
        public static int[] GetThrottleOutputs()
        {
            // Calculate the next throttle outputs
            _calculateTemperateVarables();
            _Throttle1 += -_W + _X - _Y + _Z;
            _Throttle2 += -_W - _X + _Y + _Z;
            _Throttle3 += +_W - _X - _Y + _Z;
            _Throttle4 += +_W + _X + _Y + _Z;

            // Boundary check
            if (_Throttle1 < _MinThrottle) _Throttle1 = _MinThrottle; if (_Throttle1 > _MaxThrottle) _Throttle1 = _MaxThrottle;
            if (_Throttle2 < _MinThrottle) _Throttle2 = _MinThrottle; if (_Throttle2 > _MaxThrottle) _Throttle2 = _MaxThrottle;
            if (_Throttle3 < _MinThrottle) _Throttle3 = _MinThrottle; if (_Throttle3 > _MaxThrottle) _Throttle3 = _MaxThrottle;
            if (_Throttle4 < _MinThrottle) _Throttle4 = _MinThrottle; if (_Throttle4 > _MaxThrottle) _Throttle4 = _MaxThrottle;

            // Construct the returned information
            int[] throttles = new int[4];
            throttles[0] = (int)(_Throttle1);
            throttles[1] = (int)(_Throttle2);
            throttles[2] = (int)(_Throttle3);
            throttles[3] = (int)(_Throttle4);

            // Return the result
            
            return throttles;
        }


        // Algorithm parameters as properties
        public static int Param_PropellerDirection
        {
            get { return _Kr; }
            set
            {
                if (value == -1 || value == 1) _Kr = value;
                else throw new ArgumentOutOfRangeException("Propeller direction parameter must be either -1 or 1.");
            }
        }

        public static double Param_KP_x
        {
            get { return _KP_x; }
            set
            {
                if (value < 0) throw new ArgumentOutOfRangeException("KP_x cannot be negative.");
                _KP_x = value;
            }
        }
        public static double Param_KD_x
        {
            get { return _KD_x; }
            set
            {
                if (value < 0) throw new ArgumentOutOfRangeException("KD_x cannot be negative.");
                _KD_x = value;
            }
        }
        public static double Param_Angle_roll_E
        {
            get { return _Angle_roll_E; }
            set
            {
                _Angle_roll_E = value;
            }
        }
        public static double Param_Angle_roll_R
        {
            get { return _Angle_roll_R; }
            set
            {
                _Angle_roll_R = value;
            }
        }
        public static double Param_Rotation_xpi_E
        {
            get { return _Rotation_xpi_E; }
            set
            {
                _Rotation_xpi_E = value;
            }
        }
        public static double Param_Rotation_xpi_R
        {
            get { return _Rotation_xpi_R; }
            set
            {
                _Rotation_xpi_R = value;
            }
        }

        public static double Param_KP_y
        {
            get { return _KP_y; }
            set
            {
                if (value < 0) throw new ArgumentOutOfRangeException("KP_y cannot be negative.");
                _KP_y = value;
            }
        }
        public static double Param_KD_y
        {
            get { return _KD_y; }
            set
            {
                if (value < 0) throw new ArgumentOutOfRangeException("KD_y cannot be negative.");
                _KD_y = value;
            }
        }
        public static double Param_Angle_pitch_E
        {
            get { return _Angle_pitch_E; }
            set
            {
                _Angle_pitch_E = value;
            }
        }
        public static double Param_Angle_pitch_R
        {
            get { return _Angle_pitch_R; }
            set
            {
                _Angle_pitch_R = value;
            }
        }
        public static double Param_Rotation_ypi_E
        {
            get { return _Rotation_ypi_E; }
            set
            {
                _Rotation_ypi_E = value;
            }
        }
        public static double Param_Rotation_ypi_R
        {
            get { return _Rotation_ypi_R; }
            set
            {
                _Rotation_ypi_R = value;
            }
        }

        public static double Param_KP_z
        {
            get { return _KP_z; }
            set
            {
                if (value < 0) throw new ArgumentOutOfRangeException("KP_z cannot be negative.");
                _KP_z = value;
            }
        }
        public static double Param_KD_z
        {
            get { return _KD_z; }
            set
            {
                if (value < 0) throw new ArgumentOutOfRangeException("KD_z cannot be negative.");
                _KD_z = value;
            }
        }
        public static double Param_Angle_yaw_E
        {
            get { return _Angle_yaw_E; }
            set
            {
                _Angle_yaw_E = value;
            }
        }
        public static double Param_Angle_yaw_R
        {
            get { return _Angle_yaw_R; }
            set
            {
                _Angle_yaw_R = value;
            }
        }
        public static double Param_Rotation_zpi_E
        {
            get { return _Rotation_zpi_E; }
            set
            {
                _Rotation_zpi_E = value;
            }
        }
        public static double Param_Rotation_zpi_R
        {
            get { return _Rotation_zpi_R; }
            set
            {
                _Rotation_zpi_R = value;
            }
        }

        public static double Param_KP_a
        {
            get { return _KP_a; }
            set
            {
                if (value < 0) throw new ArgumentOutOfRangeException("KP_a cannot be negative.");
                _KP_a = value;
            }
        }
        public static double Param_KI_a
        {
            get { return _KI_a; }
            set
            {
                if (value < 0) throw new ArgumentOutOfRangeException("KI_a cannot be negative.");
                _KI_a = value;
            }
        }
        public static double Param_Accel_z_E
        {
            get { return _Accel_z_E; }
            set
            {
                _Accel_z_E = value;
            }
        }
        public static double Param_Accel_z_R
        {
            get { return _Accel_z_R; }
            set
            {
                _Accel_z_R = value;
            }
        }
        public static double Param_Velocity_z_E
        {
            get { return _Velocity_z_E; }
            set
            {
                _Velocity_z_E = value;
            }
        }
        public static double Param_Velocity_z_R
        {
            get { return _Velocity_z_R; }
            set
            {
                _Velocity_z_R = value;
            }
        }
    }

    /// <summary>
    /// 表示飞行器实体。
    /// </summary>
    public static class Miniquad
    {
        private static ComputingMode _computingMode = ComputingMode.PricipalComputerMode;
        private static MiniquadStatus _status = null;


        /// <summary>
        /// 获取或设置飞行控制程序的计算模式。
        /// 默认为 ComputingMode.PricipalComputerMode 即上位机模式。
        /// </summary>
        /// <exception cref="System.ArgumentNullException">当控制模式为空时抛出该异常。</exception>
        public static ComputingMode ComputingMode
        {
            get { return _computingMode; }
            set
            {
                if (value == null) throw new ArgumentNullException("Miniquad_Controller.Miniquad.Miniquad.ComputingMode: Computing mode cannot be null.");
                _computingMode = value;
            }
        }
        
        /// <summary>
        /// 获取最近一次成功刷新的飞行器状态，若不存在上一次接收记录，则返回 null。
        /// 若要获取新的飞行器状态，须要调用 RefreshStatus 方法进行刷新。
        /// </summary>
        public static MiniquadStatus Status
        {
            get
            {
                return _status;
            }
        }


        /// <summary>
        /// 初始化飞行器。
        /// </summary>
        /// <param name="propeller1Pin">飞行器1号动力装置的引脚编号。</param>
        /// <param name="propeller2Pin">飞行器2号动力装置的引脚编号。</param>
        /// <param name="propeller3Pin">飞行器3号动力装置的引脚编号。</param>
        /// <param name="propeller4Pin">飞行器4号动力装置的引脚编号。</param>
        /// <param name="computingMode">飞行器控制程序的计算模式。</param>
        public static void Initialize(int propeller1Pin, int propeller2Pin, int propeller3Pin, int propeller4Pin, ComputingMode computingMode)
        {
            _status = new MiniquadStatus(propeller1Pin, propeller2Pin, propeller3Pin, propeller4Pin);
            _computingMode = computingMode;

            // Initialize the algorithm
            IncrementPIDFlyingControllingAlgorithm.Initialize();
        }

        /// <summary>
        /// 初始化飞行器。
        /// </summary>
        public static void Initialize()
        {
            // Initialize the algorithm
            IncrementPIDFlyingControllingAlgorithm.Initialize();
        }


        /// <summary>
        /// 使用接收到的数据缓冲块刷新飞行器的状态。
        /// </summary>
        /// <param name="receivedDataBuffer">接收到的数据缓冲块。</param>
        /// <returns>返回一个 bool 值表示刷新是否成功。</returns>
        public static bool RefreshStatus(List<byte> receivedDataBuffer)
        {
            if (Communication.ReceiveFromBuffer(receivedDataBuffer))
            {
                // Quaternion
                _status.Quaternion = Communication.GetQuaternion();

                // Rotation
                _status.Rotation = Communication.GetRotation();

                // Acceleration
                _status.Acceleration = Communication.GetAcceleration();

                // Throttle outputs
                int[] throttleOutputs = Communication.GetThrottleOutputs();
                _status.Propeller1.Throttle = throttleOutputs[0];
                _status.Propeller2.Throttle = throttleOutputs[1];
                _status.Propeller3.Throttle = throttleOutputs[2];
                _status.Propeller4.Throttle = throttleOutputs[3];

                // Return success
                return true;
            }
            else
            {
                return false;
            }
        }


        /// <summary>
        /// 获取调整后的飞行器油门输出。（根据当前的飞行器状态。）
        /// </summary>
        /// <returns>返回一个 int[] 数组表示四个油门的输出。</returns>
        public static int[] GetAdjustedThrottleOutputs_PC()
        {
            if(_status == null) return null;

            //// Static PID
            //StaticPIDFlyingControllingAlgorithm.Angle_pitch = _status.YawPitchRoll.Pitch;
            //StaticPIDFlyingControllingAlgorithm.Angle_roll = _status.YawPitchRoll.Roll;
            //StaticPIDFlyingControllingAlgorithm.Angle_yaw = _status.YawPitchRoll.Yaw;
            //StaticPIDFlyingControllingAlgorithm.Accel_z = _status.Acceleration.Z;
            //return StaticPIDFlyingControllingAlgorithm.GetThrottleOutputs();

            // Increment PID
            IncrementPIDFlyingControllingAlgorithm.Angle_pitch = _status.YawPitchRoll.Pitch;
            IncrementPIDFlyingControllingAlgorithm.Angle_roll = _status.YawPitchRoll.Roll;
            IncrementPIDFlyingControllingAlgorithm.Angle_yaw = _status.YawPitchRoll.Yaw;
            IncrementPIDFlyingControllingAlgorithm.Accel_z = _status.Acceleration.Z;
            IncrementPIDFlyingControllingAlgorithm.Rotation_xpi = _status.Rotation.X;
            IncrementPIDFlyingControllingAlgorithm.Rotation_ypi = _status.Rotation.Y;
            IncrementPIDFlyingControllingAlgorithm.Rotation_zpi = _status.Rotation.Z;
            return IncrementPIDFlyingControllingAlgorithm.GetThrottleOutputs();
        }

        /// <summary>
        /// 设置四个螺旋桨的油门输出。（上位机模式）
        /// </summary>
        /// <param name="throttle1">第1号螺旋桨的油门输出。</param>
        /// <param name="throttle2">第2号螺旋桨的油门输出。</param>
        /// <param name="throttle3">第3号螺旋桨的油门输出。</param>
        /// <param name="throttle4">第4号螺旋桨的油门输出。</param>
        /// <returns>返回一个 bool 值表示指令是否成功送出。</returns>
        /// <exception cref="System.ArgumentOutOfRangeException">当油门输出不在 0 至 255 范围内时抛出该异常。</exception>
        public static bool SetThrottleOutputs_PC(int throttle1, int throttle2, int throttle3, int throttle4)
        {
            return Communication.SetThrottleOutputs_PC(throttle1, throttle2, throttle3, throttle4);
        }
    }
}
