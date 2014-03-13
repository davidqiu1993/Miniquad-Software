using System;

namespace Miniquad_Controller
{
    /// <summary>
    /// 表示温度。
    /// </summary>
    class Temperature
    {
        protected double _temperatureInDegreeCelsius;

        /// <summary>
        /// 实例化一个新的温度类型对象。
        /// 初始化温度为 0 摄氏度。
        /// </summary>
        public Temperature()
        {
            _temperatureInDegreeCelsius = 0;
        }

        /// <summary>
        /// 使用初始的摄氏温度实例化一个新的温度类型对象。
        /// </summary>
        /// <param name="temperatureInDegreeCelsius">初始的摄氏温度。</param>
        public Temperature(double temperatureInDegreeCelsius)
        {
            _temperatureInDegreeCelsius = temperatureInDegreeCelsius;
        }

        /// <summary>
        /// 获取或设置摄氏温度。
        /// </summary>
        /// <exception cref="System.ArgumentOutOfRangeException">当温度低于绝对零度时抛出此异常。</exception>
        public double TemperatureInDegreeCelsius
        {
            get { return _temperatureInDegreeCelsius; }
            set
            {
                if (value < -273.15) throw new ArgumentOutOfRangeException("Miniquad_Controller.Temperature.TemperatureInDegreeCelsius: Temperature must not be less than absolute zero.");
                _temperatureInDegreeCelsius = value;
            }
        }

        /// <summary>
        /// 获取或设置华氏温度。
        /// </summary>
        /// <exception cref="System.ArgumentOutOfRangeException">当温度低于绝对零度时抛出此异常。</exception>
        public double TemperatureInDegreeFahrenheit
        {
            get { return 32 + 1.8 * _temperatureInDegreeCelsius; }
            set
            {
                if (value < -459.67) throw new ArgumentOutOfRangeException("Miniquad_Controller.Temperature.TemperatureInDegreeCelsius: Temperature must not be less than absolute zero.");
                _temperatureInDegreeCelsius = (value - 32) / 1.8;
            }
        }

        /// <summary>
        /// 获取或设置热力学温度。
        /// </summary>
        /// <exception cref="System.ArgumentOutOfRangeException">当温度低于绝对零度时抛出此异常。</exception>
        public double TemperatureInKelvin
        {
            get { return _temperatureInDegreeCelsius + 273.15; }
            set
            {
                if (value < 0) throw new ArgumentOutOfRangeException("Miniquad_Controller.Temperature.TemperatureInDegreeCelsius: Temperature must not be less than absolute zero.");
                _temperatureInDegreeCelsius = value - 273.15;
            }
        }
    }
}
