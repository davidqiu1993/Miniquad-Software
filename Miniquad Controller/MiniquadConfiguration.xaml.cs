using System.Windows;
using System.Windows.Controls;
using MahApps.Metro;
using MahApps.Metro.Controls;
using Miniquad_Controller.Miniquad;

namespace Miniquad_Controller
{
    /// <summary>
    /// 表示飞行器配置面板。
    /// </summary>
    public partial class MiniquadConfiguration : MetroWindow
    {
        private void _ResetWarning()
        {
            // Reset algorithm parameter input fields
            _ResetWarnedTextBox(txtPropellerDirection);
            _ResetWarnedTextBox(txtParamKP_x);
            _ResetWarnedTextBox(txtParamKP_y);
            _ResetWarnedTextBox(txtParamKP_z);
            _ResetWarnedTextBox(txtParamKP_a);
            _ResetWarnedTextBox(txtParamKD_x);
            _ResetWarnedTextBox(txtParamKD_y);
            _ResetWarnedTextBox(txtParamKD_z);
            _ResetWarnedTextBox(txtParamKI_a);
        }
        private void _ResetWarnedTextBox(TextBox target)
        {
            target.Background = System.Windows.Media.Brushes.White;
            target.Foreground = System.Windows.Media.Brushes.Black;
        }
        private void _WarnTextBoxInputInvalid(TextBox target)
        {
            target.Background = System.Windows.Media.Brushes.Red;
            target.Foreground = System.Windows.Media.Brushes.White;
        }

        private void _InitAlgorithmParameters()
        {
            txtPropellerDirection.Text = IncrementPIDFlyingControllingAlgorithm.Param_PropellerDirection.ToString();
            txtParamKP_x.Text = ((decimal)IncrementPIDFlyingControllingAlgorithm.Param_KP_x).ToString();
            txtParamKP_y.Text = ((decimal)IncrementPIDFlyingControllingAlgorithm.Param_KP_y).ToString();
            txtParamKP_z.Text = ((decimal)IncrementPIDFlyingControllingAlgorithm.Param_KP_z).ToString();
            txtParamKP_a.Text = ((decimal)IncrementPIDFlyingControllingAlgorithm.Param_KP_a).ToString();
            txtParamKD_x.Text = ((decimal)IncrementPIDFlyingControllingAlgorithm.Param_KD_x).ToString();
            txtParamKD_y.Text = ((decimal)IncrementPIDFlyingControllingAlgorithm.Param_KD_y).ToString();
            txtParamKD_z.Text = ((decimal)IncrementPIDFlyingControllingAlgorithm.Param_KD_z).ToString();
            txtParamKI_a.Text = ((decimal)IncrementPIDFlyingControllingAlgorithm.Param_KI_a).ToString();
        }

        // Initialize the controls
        public MiniquadConfiguration()
        {
            InitializeComponent();

            // Init: Algorithm parameters
            _InitAlgorithmParameters();
        }

        // Keep the instance of this window
        private void windowMiniquadConfiguration_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            this.Visibility = Visibility.Hidden;
        }


        // Algorithm: propeller direction parameter
        private void txtPropellerDirection_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            // Trigger apply available
            btnAlgorithmParamsApply.IsEnabled = true;
        }

        // Algorithm: KP_x
        private void txtParamKP_x_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            // Trigger apply available
            btnAlgorithmParamsApply.IsEnabled = true;
        }

        // Algorithm: KP_y
        private void txtParamKP_y_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            // Trigger apply available
            btnAlgorithmParamsApply.IsEnabled = true;
        }

        // Algorithm: KP_z
        private void txtParamKP_z_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            // Trigger apply available
            btnAlgorithmParamsApply.IsEnabled = true;
        }

        // Algorithm: KP_a
        private void txtParamKP_a_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            // Trigger apply available
            btnAlgorithmParamsApply.IsEnabled = true;
        }

        // Algorithm: KD_x
        private void txtParamKD_x_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            // Trigger apply available
            btnAlgorithmParamsApply.IsEnabled = true;
        }

        // Algorithm: KD_y
        private void txtParamKD_y_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            // Trigger apply available
            btnAlgorithmParamsApply.IsEnabled = true;
        }

        // Algorithm: KD_z
        private void txtParamKD_z_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            // Trigger apply available
            btnAlgorithmParamsApply.IsEnabled = true;
        }

        // Algorithm: KI_a
        private void txtParamKI_a_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            // Trigger apply available
            btnAlgorithmParamsApply.IsEnabled = true;
        }

        // Algorithm: Apply
        private void btnAlgorithmParamsApply_Click(object sender, RoutedEventArgs e)
        {
            // Trigger apply unavailable
            btnAlgorithmParamsApply.IsEnabled = false;

            // Reset warning
            _ResetWarning();

            // Transformation containers
            int propellerDirection;
            double kp_x, kp_y, kp_z, kp_a, kd_x, kd_y, kd_z, ki_a;

            // Check inputs
            bool passCheck = true;
            if (!(int.TryParse(txtPropellerDirection.Text, out propellerDirection) && (propellerDirection == -1 || propellerDirection == 1))) { _WarnTextBoxInputInvalid(txtPropellerDirection); passCheck = false; }
            if (!(double.TryParse(txtParamKP_x.Text, out kp_x) && kp_x >= 0)) { _WarnTextBoxInputInvalid(txtParamKP_x); passCheck = false; }
            if (!(double.TryParse(txtParamKP_y.Text, out kp_y) && kp_y >= 0)) { _WarnTextBoxInputInvalid(txtParamKP_y); passCheck = false; }
            if (!(double.TryParse(txtParamKP_z.Text, out kp_z) && kp_z >= 0)) { _WarnTextBoxInputInvalid(txtParamKP_z); passCheck = false; }
            if (!(double.TryParse(txtParamKP_a.Text, out kp_a) && kp_a >= 0)) { _WarnTextBoxInputInvalid(txtParamKP_a); passCheck = false; }
            if (!(double.TryParse(txtParamKD_x.Text, out kd_x) && kd_x >= 0)) { _WarnTextBoxInputInvalid(txtParamKD_x); passCheck = false; }
            if (!(double.TryParse(txtParamKD_y.Text, out kd_y) && kd_y >= 0)) { _WarnTextBoxInputInvalid(txtParamKD_y); passCheck = false; }
            if (!(double.TryParse(txtParamKD_z.Text, out kd_z) && kd_z >= 0)) { _WarnTextBoxInputInvalid(txtParamKD_z); passCheck = false; }
            if (!(double.TryParse(txtParamKI_a.Text, out ki_a) && ki_a >= 0)) { _WarnTextBoxInputInvalid(txtParamKI_a); passCheck = false; }
            if (passCheck == false) return;

            // Apply parameters
            IncrementPIDFlyingControllingAlgorithm.Param_PropellerDirection = propellerDirection;
            IncrementPIDFlyingControllingAlgorithm.Param_KP_x = kp_x;
            IncrementPIDFlyingControllingAlgorithm.Param_KP_y = kp_y;
            IncrementPIDFlyingControllingAlgorithm.Param_KP_z = kp_z;
            IncrementPIDFlyingControllingAlgorithm.Param_KP_a = kp_a;
            IncrementPIDFlyingControllingAlgorithm.Param_KD_x = kd_x;
            IncrementPIDFlyingControllingAlgorithm.Param_KD_y = kd_y;
            IncrementPIDFlyingControllingAlgorithm.Param_KD_z = kd_z;
            IncrementPIDFlyingControllingAlgorithm.Param_KI_a = ki_a;
        }
    }
}
