using System;

namespace Miniquad_Controller.Math3D
{
    /// <summary>
    /// 表示向量接口。
    /// </summary>
    /// <typeparam name="TVector">向量类型。</typeparam>
    /// <typeparam name="TMagnitude">向量模的类型。</typeparam>
    public interface IVector<TVector, TMagnitude>
    {
        /// <summary>
        /// 获取该向量的模。
        /// </summary>
        TMagnitude Magnitude { get; }

        /// <summary>
        /// 标准化该向量。
        /// </summary>
        void Normalize();

        /// <summary>
        /// 获取标准化后的该向量。
        /// </summary>
        /// <returns>返回一个与该向量相同类型的对象，表示经过标准化后的该向量。</returns>
        TVector GetNormalized();
    }

    /// <summary>
    /// 表示可经过四元数进行旋转的向量接口。
    /// </summary>
    /// <typeparam name="TVector">向量类型。</typeparam>
    public interface IRotatableVector<TVector>
    {
        /// <summary>
        /// 经过指定的四元数对该向量进行旋转操作。
        /// </summary>
        /// <param name="quaternion">指定的四元数。</param>
        void Rotate(Quaternion quaternion);

        /// <summary>
        /// 获取该向量经过指定四元数进行旋转操作后的新向量。
        /// </summary>
        /// <param name="quaternion">指定的四元数。</param>
        /// <returns>返回一个与该向量相同类型的对象，表示经过旋转操作后的新向量。</returns>
        TVector GetRotated(Quaternion quaternion);
    }

    /// <summary>
    /// 表示四元数。
    /// </summary>
    public class Quaternion : IVector<Quaternion, double>
    {
        protected double _w, _x, _y, _z;


        /// <summary>
        /// 实例化一个默认的四元数类型对象。
        /// </summary>
        public Quaternion()
        {
            _w = 1.0;
            _x = 0.0;
            _y = 0.0;
            _z = 0.0;
        }

        /// <summary>
        /// 使用指定的 w、x、y、z 实例化一个新的四元数类型对象。
        /// </summary>
        /// <param name="w">指定的 w 值。</param>
        /// <param name="x">指定的 x 值。</param>
        /// <param name="y">指定的 y 值。</param>
        /// <param name="z">指定的 z 值。</param>
        public Quaternion(double w, double x, double y, double z)
        {
            _w = w;
            _x = x;
            _y = y;
            _z = z;
        }


        /// <summary>
        /// 获取或设置四元数的 w 值。
        /// </summary>
        public double W
        {
            get { return _w; }
            set { _w = value; }
        }

        /// <summary>
        /// 获取或设置四元数的 x 值。
        /// </summary>
        public double X
        {
            get { return _x; }
            set { _x = value; }
        }

        /// <summary>
        /// 获取或设置四元数的 y 值。
        /// </summary>
        public double Y
        {
            get { return _y; }
            set { _y = value; }
        }

        /// <summary>
        /// 获取或设置四元数的 z 值。
        /// </summary>
        public double Z
        {
            get { return _z; }
            set { _z = value; }
        }


        /// <summary>
        /// 获取该四元数的共轭四元数。
        /// </summary>
        public Quaternion Conjugate
        {
            get { return new Quaternion(_w, -_x, -_y, -_z); }
        }

        /// <summary>
        /// 获取该四元数的模。
        /// </summary>
        public double Magnitude
        {
            get { return Math.Sqrt(_w * _w + _x * _x + _y * _y + _z * _z); }
        }


        /// <summary>
        /// 获取该四元数与另外一个四元数的乘积。
        /// </summary>
        /// <param name="other">另外的一个四元数。</param>
        /// <returns>返回一个 Quaternion 表示该四元数与另外一个四元数的乘积。</returns>
        public Quaternion GetProduct(Quaternion other)
        {
            // Quaternion multiplication is defined by:
            //     (Q1 * Q2).w = (w1w2 - x1x2 - y1y2 - z1z2)
            //     (Q1 * Q2).x = (w1x2 + x1w2 + y1z2 - z1y2)
            //     (Q1 * Q2).y = (w1y2 - x1z2 + y1w2 + z1x2)
            //     (Q1 * Q2).z = (w1z2 + x1y2 - y1x2 + z1w2
            return new Quaternion(
                _w * other._w - _x * other._x - _y * other._y - _z * other._z,
                _w * other._x + _x * other._w + _y * other._z - _z * other._y,
                _w * other._y - _x * other._z + _y * other._w + _z * other._x,
                _w * other._z + _x * other._y - _y * other._x + _z * other._w);
        }

        /// <summary>
        /// 标准化该四元数。
        /// </summary>
        public void Normalize()
        {
            // Get the magnitude of this quaternion
            double magnitude = this.Magnitude;

            // Normalize this quaternion
            _w /= magnitude;
            _x /= magnitude;
            _y /= magnitude;
            _z /= magnitude;
        }

        /// <summary>
        /// 获取一个经过标准化的当前的四元数。
        /// </summary>
        /// <returns>返回一个 Quaternion 表示经过标准化的当前的四元数。</returns>
        public Quaternion GetNormalized()
        {
            Quaternion q = new Quaternion(_w, _x, _y, _z);
            q.Normalize();
            return q;
        }


        /// <summary>
        /// 获得由该四元数转化所得的欧拉角。
        /// </summary>
        /// <returns>返回一个 EulerAngle 表示转化所得的欧拉角。</returns>
        public EulerAngle GetEulerAngle()
        {
            EulerAngle eulerAngle = new EulerAngle();

            // Psi
            eulerAngle.Psi = (180.0 / Math.PI) * Math.Atan2(2 * _x * _y - 2 * _z * _z, 2 * _w * _w + 2 * _x * _x - 1);

            // Theta
            eulerAngle.Theta = (180.0 / Math.PI) * (-Math.Asin(2 * _x * _z + 2 * _w * _y));

            // Phi
            eulerAngle.Phi = (180.0 / Math.PI) * Math.Atan2(2 * _y * _z - 2 * _w * _x, 2 * _w * _w + 2 * _z * _z - 1);

            // Return the result
            return eulerAngle;
        }

        /// <summary>
        /// 获取由该四元数转化所得的重力分量。
        /// </summary>
        /// <returns>返回一个 Gravity 表示转化所得的重力分量。</returns>
        public Gravity GetGravity()
        {
            Gravity gravity = new Gravity();

            // Gravity on X-axis
            gravity.X = 2 * (_x * _z - _w * _y);

            // Gravity on Y-axis
            gravity.Y = 2 * (_w * _x + _y * _z);

            // Gravity on Z-axis
            gravity.Z = _w * _w - _x * _x - _y * _y + _z * _z;

            // Return the result
            return gravity;
        }

        /// <summary>
        /// 获取由该四元数转化所得的偏航角、俯仰角、旋转角的角度。
        /// </summary>
        /// <returns>返回一个 YawPitchRoll 表示转化所得的偏航角、俯仰角、旋转角的角度。</returns>
        public YawPitchRoll GetYawPitchRoll()
        {
            YawPitchRoll ypr = new YawPitchRoll();
            Gravity gravity = this.GetGravity();

            // Yaw
            ypr.Yaw = (180.0 / Math.PI) * Math.Atan2(2 * _x * _y - 2 * _w * _z, 2 * _w * _w + 2 * _x * _x - 1);
            
            // Pitch
            ypr.Pitch = (180.0 / Math.PI) * Math.Atan(gravity.X / Math.Sqrt(gravity.Y * gravity.Y + gravity.Z * gravity.Z));
            
            // Roll
            ypr.Roll = (180.0 / Math.PI) * Math.Atan(gravity.Y / Math.Sqrt(gravity.X * gravity.X + gravity.Z * gravity.Z));

            // Return the result
            return ypr;
        }
    }

    /// <summary>
    /// 表示一个三维数组的抽象类
    /// </summary>
    /// <typeparam name="T">数组的数据类型。</typeparam>
    public abstract class ThreeDimensionalArray<T>
    {
        protected T[] _array = new T[3];

        /// <summary>
        /// 实例化该三维数组。（只能通过子对象实例化。）
        /// </summary>
        protected ThreeDimensionalArray()
        {
            ;
        }

        /// <summary>
        /// 使用指定的数组内容实例化该三维数组。（只能通过子对象实例化。）
        /// </summary>
        /// <param name="x">指定的第一纬参数。</param>
        /// <param name="y">指定的第二纬参数。</param>
        /// <param name="z">指定的第三纬参数。</param>
        protected ThreeDimensionalArray(T x, T y, T z)
        {
            _array[0] = x;
            _array[1] = y;
            _array[2] = z;
        }

        /// <summary>
        /// 获取对应的数组。
        /// </summary>
        public T[] Array
        {
            get { return _array; }
            set { _array = value; }
        }
    }

    /// <summary>
    /// 表示欧拉角。
    /// </summary>
    public class EulerAngle : ThreeDimensionalArray<double>
    {
        /// <summary>
        /// 实例化一个默认的欧拉角类型对象。
        /// </summary>
        public EulerAngle()
            : base(0, 0, 0)
        {
            ;
        }

        /// <summary>
        /// 使用指定的旋进角、章动角和自转角实例化一个新的欧拉角类型对象。
        /// </summary>
        /// <param name="psi">指定的旋进角。以度（°）为单位。</param>
        /// <param name="theta">指定的章动角。以度（°）为单位。</param>
        /// <param name="phi">指定的自转角。以度（°）为单位。</param>
        /// <exception cref="System.ArgumentOutOfRangeException">当设置的角度超出指定范围时抛出此异常。</exception>
        public EulerAngle(double psi, double theta, double phi)
        {
            // Check the Euler angle
            if (psi < -180 || psi > 180) throw new ArgumentOutOfRangeException("Miniquad_Controller.Math3D.EulerAngle.EulerAngle(double, double, double): The value of Psi is out of the range between -180 degrees to 180 degrees.");
            if (theta < -180 || theta > 180) throw new ArgumentOutOfRangeException("Miniquad_Controller.Math3D.EulerAngle.EulerAngle(double, double, double): The value of Theta is out of the range between -180 degrees to 180 degrees.");
            if (phi < -180 || phi > 180) throw new ArgumentOutOfRangeException("Miniquad_Controller.Math3D.EulerAngle.EulerAngle(double, double, double): The value of Phi is out of the range between -180 degrees to 180 degrees.");

            // Set the Euler angle
            _array[0] = psi;
            _array[1] = theta;
            _array[2] = phi;
        }

        
        /// <summary>
        /// 表示欧拉角中的旋进角（进动角）ψ。以度（°）为单位。
        /// 范围为 -180 度至 180 度。
        /// </summary>
        /// <exception cref="System.ArgumentOutOfRangeException">当设置的角度超出 -180 度至 180 度的范围时抛出此异常。</exception>
        public double Psi
        {
            get { return _array[0]; }
            set
            {
                if (value < -180 || value > 180) throw new ArgumentOutOfRangeException("Miniquad_Controller.Math3D.EulerAngle.Psi: The value of Psi is out of the range between -180 degrees to 180 degrees.");
                _array[0] = value;
            }
        }

        /// <summary>
        /// 表示欧拉角中的章动角θ。以度（°）为单位。
        /// 范围为 -180 度至 180 度。
        /// </summary>
        /// <exception cref="System.ArgumentOutOfRangeException">当设置的角度超出 -180 度至 180 度的范围时抛出此异常。</exception>
        public double Theta
        {
            get { return _array[1]; }
            set
            {
                if (value < -180 || value > 180) throw new ArgumentOutOfRangeException("Miniquad_Controller.Math3D.EulerAngle.Theta: The value of Theta is out of the range between -180 degrees to 180 degrees.");
                _array[1] = value;
            }
        }

        /// <summary>
        /// 表示欧拉角中的自转角φ。以度（°）为单位。
        /// 范围为 -180 度至 180 度。
        /// </summary>
        /// <exception cref="System.ArgumentOutOfRangeException">当设置的角度超出 -180 度至 180 度的范围时抛出此异常。</exception>
        public double Phi
        {
            get { return _array[2]; }
            set
            {
                if (value < -180 || value > 180) throw new ArgumentOutOfRangeException("Miniquad_Controller.Math3D.EulerAngle.Phi: The value of Phi is out of the range between -180 degrees to 180 degrees.");
                _array[2] = value;
            }
        }
    }

    /// <summary>
    /// 表示在 X、Y、Z 轴上的重力分量。
    /// </summary>
    public class Gravity : ThreeDimensionalArray<double>
    {
        /// <summary>
        /// 实例化一个默认的重力。
        /// </summary>
        public Gravity()
            : base(0, 0, 1)
        {
            ;
        }

        /// <summary>
        /// 使用指定的重力分量实例化一个新的重力。
        /// </summary>
        /// <param name="x">在 X 轴上的重力分量。以倍重力加速度（g）为单位。</param>
        /// <param name="y">在 Y 轴上的重力分量。以倍重力加速度（g）为单位。</param>
        /// <param name="z">在 Z 轴上的重力分量。以倍重力加速度（g）为单位。</param>
        public Gravity(double x, double y, double z)
            : base(x, y, z)
        {
            ;
        }


        /// <summary>
        /// 获取或设置在 X 轴上的重力分量。以倍重力加速度（g）为单位。
        /// </summary>
        public double X
        {
            get { return _array[0]; }
            set { _array[0] = value; }
        }

        /// <summary>
        /// 获取或设置在 Y 轴上的重力分量。以倍重力加速度（g）为单位。
        /// </summary>
        public double Y
        {
            get { return _array[1]; }
            set { _array[1] = value; }
        }

        /// <summary>
        /// 获取或设置在 Z 轴上的重力分量。以倍重力加速度（g）为单位。
        /// </summary>
        public double Z
        {
            get { return _array[2]; }
            set { _array[2] = value; }
        }
    }

    /// <summary>
    /// 表示偏航角、俯仰角、旋转角的角度。
    /// </summary>
    public class YawPitchRoll : ThreeDimensionalArray<double>
    {
        /// <summary>
        /// 实例化一个默认的 YPR 类型对象。
        /// </summary>
        public YawPitchRoll()
            : base(0, 0, 0)
        {
            ;
        }

        /// <summary>
        /// 使用指定的偏航角、俯仰角、旋转角角度实例化一个新的 YPR 类型对象。
        /// </summary>
        /// <param name="yaw">指定的偏航角角度。以度（°）为单位。</param>
        /// <param name="pitch">指定的俯仰角角度。以度（°）为单位。</param>
        /// <param name="roll">指定的旋转角角度。以度（°）为单位。</param>
        /// <exception cref="System.ArgumentOutOfRangeException">当设置的角度指定范围时抛出此异常。</exception>
        public YawPitchRoll(double yaw, double pitch, double roll)
        {
            // Check the range of the ypr angles
            if (yaw < -180 || yaw > 180) throw new ArgumentOutOfRangeException("Miniquad_Controller.Math3D.YawPitchRoll.YawPitchRoll(double, double, double): The value of Yaw is out of the range between -180 degrees to 180 degrees.");
            if (pitch < -90 || pitch > 90) throw new ArgumentOutOfRangeException("Miniquad_Controller.Math3D.YawPitchRoll.YawPitchRoll(double, double, double): The value of Pitch is out of the range between -90 degrees to 90 degrees.");
            if (roll < -90 || roll > 90) throw new ArgumentOutOfRangeException("Miniquad_Controller.Math3D.YawPitchRoll.YawPitchRoll(double, double, double): The value of Roll is out of the range between -90 degrees to 90 degrees.");

            // Set the ypr angles
            _array[0] = yaw;
            _array[1] = pitch;
            _array[2] = roll;
        }

        /// <summary>
        /// 获取或设置偏航角角度。以度（°）为单位。
        /// 范围为 -180 度至 180 度。
        /// </summary>
        /// <exception cref="System.ArgumentOutOfRangeException">当设置的角度超出 -180 度至 180 度的范围时抛出此异常。</exception>
        public double Yaw
        {
            get { return _array[0]; }
            set
            {
                if (value < -180 || value > 180) throw new ArgumentOutOfRangeException("Miniquad_Controller.Math3D.YawPitchRoll.Yaw: The value of Yaw is out of the range between -180 degrees to 180 degrees.");
                _array[0] = value;
            }
        }

        /// <summary>
        /// 获取或设置俯仰角角度。以度（°）为单位。
        /// 范围为 -90 度至 90 度。
        /// </summary>
        /// <exception cref="System.ArgumentOutOfRangeException">当设置的角度超出 -90 度至 90 度的范围时抛出此异常。</exception>
        public double Pitch
        {
            get { return _array[1]; }
            set
            {
                if (value < -90 || value > 90) throw new ArgumentOutOfRangeException("Miniquad_Controller.Math3D.YawPitchRoll.Pitch: The value of Pitch is out of the range between -90 degrees to 90 degrees.");
                _array[1] = value;
            }
        }

        /// <summary>
        /// 获取或设置旋转角角度。以度（°）为单位。
        /// 范围为 -90 度至 90 度。
        /// </summary>
        /// <exception cref="System.ArgumentOutOfRangeException">当设置的角度超出 -90 度至 90 度的范围时抛出此异常。</exception>
        public double Roll
        {
            get { return _array[2]; }
            set
            {
                if (value < -90 || value > 90) throw new ArgumentOutOfRangeException("Miniquad_Controller.Math3D.YawPitchRoll.Roll: The value of Roll is out of the range between -90 degrees to 90 degrees.");
                _array[2] = value;
            }
        }
    }

    /// <summary>
    /// 表示关于 X、Y、Z 轴的旋转角速度。
    /// </summary>
    public class Rotation : ThreeDimensionalArray<double>
    {
        /// <summary>
        /// 实例化一个默认的旋转角速度。
        /// </summary>
        public Rotation()
            : base(0, 0, 0)
        {
            ;
        }

        /// <summary>
        /// 使用指定的三维角速度实例化一个新的旋转角速度。
        /// </summary>
        /// <param name="x">关于 X 轴旋转的角速度。以度每秒（°/s）为单位。</param>
        /// <param name="y">关于 Y 轴旋转的角速度。以度每秒（°/s）为单位。</param>
        /// <param name="z">关于 Z 轴旋转的角速度。以度每秒（°/s）为单位。</param>
        public Rotation(double x, double y, double z)
            : base(x, y, z)
        {
            ;
        }

        /// <summary>
        /// 获取或设置关于 X 轴旋转的角速度。以度每秒（°/s）为单位。
        /// </summary>
        public double X
        {
            get { return _array[0]; }
            set { _array[0] = value; }
        }

        /// <summary>
        /// 获取或设置关于 Y 轴旋转的角速度。以度每秒（°/s）为单位。
        /// </summary>
        public double Y
        {
            get { return _array[1]; }
            set { _array[1] = value; }
        }

        /// <summary>
        /// 获取或设置关于 Z 轴旋转的角速度。以度每秒（°/s）为单位。
        /// </summary>
        public double Z
        {
            get { return _array[2]; }
            set { _array[2] = value; }
        }
    }

    /// <summary>
    /// 表示在 X、Y、Z 轴上的线加速度。
    /// </summary>
    public class Acceleration : ThreeDimensionalArray<double>
    {
        /// <summary>
        /// 实例化一个默认的线加速度。
        /// </summary>
        public Acceleration()
            : base(0, 0, 0)
        {
            ;
        }

        /// <summary>
        /// 使用指定的三维线加速度实例化一个新的线加速度。
        /// </summary>
        /// <param name="x">在 X 轴上的线加速度。以倍重力加速度（g）为单位。</param>
        /// <param name="y">在 Y 轴上的线加速度。以倍重力加速度（g）为单位。</param>
        /// <param name="z">在 Z 轴上的线加速度。以倍重力加速度（g）为单位。</param>
        public Acceleration(double x, double y, double z)
            : base(x, y, z)
        {
            ;
        }

        /// <summary>
        /// 获取或设置在 X 轴上的线加速度。以倍重力加速度（g）为单位。
        /// </summary>
        public double X
        {
            get { return _array[0]; }
            set { _array[0] = value; }
        }

        /// <summary>
        /// 获取或设置在 Y 轴上的线加速度。以倍重力加速度（g）为单位。
        /// </summary>
        public double Y
        {
            get { return _array[1]; }
            set { _array[1] = value; }
        }

        /// <summary>
        /// 获取或设置在 Z 轴上的线加速度。以倍重力加速度（g）为单位。
        /// </summary>
        public double Z
        {
            get { return _array[2]; }
            set { _array[2] = value; }
        }
    }
}
