using System.Windows;
using System.Windows.Controls;

namespace EPIControls.Controls.BaseUserControls
{
    /// <summary>
    /// EPIRndRobot.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class EPIRndRobot : UserControl
    {
        public static readonly DependencyProperty Axis1AngleProperty =
        DependencyProperty.Register(nameof(Axis1Angle), typeof(double), typeof(EPIRndRobot),
            new PropertyMetadata(0.0, OnAxisAngleChanged));

        public static readonly DependencyProperty Axis2AngleProperty =
            DependencyProperty.Register(nameof(Axis2Angle), typeof(double), typeof(EPIRndRobot),
                new PropertyMetadata(0.0, OnAxisAngleChanged));

        public static readonly DependencyProperty Axis3AngleProperty =
            DependencyProperty.Register(nameof(Axis3Angle), typeof(double), typeof(EPIRndRobot),
                new PropertyMetadata(0.0, OnAxisAngleChanged));

        public double Axis1Angle
        {
            get => (double)GetValue(Axis1AngleProperty);
            set => SetValue(Axis1AngleProperty, value);
        }

        public double Axis2Angle
        {
            get => (double)GetValue(Axis2AngleProperty);
            set => SetValue(Axis2AngleProperty, value);
        }

        public double Axis3Angle
        {
            get => (double)GetValue(Axis3AngleProperty);
            set => SetValue(Axis3AngleProperty, value);
        }

        public EPIRndRobot()
        {
            InitializeComponent();
        }

        private static void OnAxisAngleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var ctrl = (EPIRndRobot)d;
            ctrl.ApplyAngles();
        }

        private void ApplyAngles()
        {
            Arm1Rotate.Angle = Axis1Angle;
            Arm2Rotate.Angle = Axis2Angle;
            Arm3Rotate.Angle = Axis3Angle;
        }
    }
}
