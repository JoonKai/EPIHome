using System.Windows;
using System.Windows.Controls;

namespace EPIControls.Controls.CombinedUserControls
{
    /// <summary>
    /// EPIRndRobotViewer.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class EPIRndRobotViewer : UserControl
    {
        public EPIRndRobotViewer()
        {
            InitializeComponent();
        }
        private void OnAxis1Changed(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (RobotControl != null) RobotControl.Axis1Angle = e.NewValue;
            if (Axis1Label != null) Axis1Label.Text = $"{e.NewValue:F1}°";
        }

        private void OnAxis2Changed(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (RobotControl != null) RobotControl.Axis2Angle = e.NewValue;
            if (Axis2Label != null) Axis2Label.Text = $"{e.NewValue:F1}°";
        }

        private void OnAxis3Changed(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (RobotControl != null) RobotControl.Axis3Angle = e.NewValue;
            if (Axis3Label != null) Axis3Label.Text = $"{e.NewValue:F1}°";
        }
    }
}
