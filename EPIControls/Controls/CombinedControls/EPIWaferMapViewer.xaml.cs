using System.Collections;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace EPIControls.Controls.CombinedControls
{
    /// <summary>
    /// EPIWaferMapViewer.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class EPIWaferMapViewer : UserControl
    {
        #region 디펜던시 프로퍼티

        public static readonly DependencyProperty MapCellsProperty =
            DependencyProperty.Register(nameof(MapCells), typeof(IEnumerable), typeof(EPIWaferMapViewer),
                new FrameworkPropertyMetadata(null, OnMapCellsChanged));

        public static readonly DependencyProperty WaferSizeProperty =
            DependencyProperty.Register(nameof(WaferSize), typeof(double), typeof(EPIWaferMapViewer),
                new FrameworkPropertyMetadata(100.0, OnWaferSizeChanged));

        public static readonly DependencyProperty EdgeExclusionProperty =
            DependencyProperty.Register(nameof(EdgeExclusion), typeof(double), typeof(EPIWaferMapViewer),
                new FrameworkPropertyMetadata(4.0, OnEdgeExclusionChanged));

        public static readonly DependencyProperty CellWidthProperty =
            DependencyProperty.Register(nameof(CellWidth), typeof(double), typeof(EPIWaferMapViewer),
                new FrameworkPropertyMetadata(2.0, OnCellWidthChanged));

        public static readonly DependencyProperty CellHeightProperty =
            DependencyProperty.Register(nameof(CellHeight), typeof(double), typeof(EPIWaferMapViewer),
                new FrameworkPropertyMetadata(2.0, OnCellHeightChanged));

        public static readonly DependencyProperty WaferFillProperty =
            DependencyProperty.Register(nameof(WaferFill), typeof(Brush), typeof(EPIWaferMapViewer),
                new FrameworkPropertyMetadata(Brushes.Transparent, OnWaferFillChanged));

        public static readonly DependencyProperty WaferStrokeProperty =
            DependencyProperty.Register(nameof(WaferStroke), typeof(Brush), typeof(EPIWaferMapViewer),
                new FrameworkPropertyMetadata(Brushes.DarkGray, OnWaferStrokeChanged));

        public static readonly DependencyProperty RangeStartProperty =
            DependencyProperty.Register(nameof(RangeStart), typeof(double), typeof(EPIWaferMapViewer),
                new FrameworkPropertyMetadata(0.0, OnRangeStartChanged));

        public static readonly DependencyProperty RangeEndProperty =
            DependencyProperty.Register(nameof(RangeEnd), typeof(double), typeof(EPIWaferMapViewer),
                new FrameworkPropertyMetadata(1.0, OnRangeEndChanged));

        public static readonly DependencyProperty UseAutoRangeProperty =
            DependencyProperty.Register(nameof(UseAutoRange), typeof(bool), typeof(EPIWaferMapViewer),
                new FrameworkPropertyMetadata(true, OnUseAutoRangeChanged));

        public static readonly DependencyProperty UseDistributionProperty =
            DependencyProperty.Register(nameof(UseDistribution), typeof(bool), typeof(EPIWaferMapViewer),
                new FrameworkPropertyMetadata(false, OnUseDistributionChanged));

        // ── null 체크가 포함된 콜백 메서드들 ──────────────────────────────────
        private static void OnMapCellsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var ctrl = (EPIWaferMapViewer)d;
            if (ctrl.WaferMap != null)
                ctrl.WaferMap.MapCells = (IEnumerable)e.NewValue;
        }

        private static void OnWaferSizeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var ctrl = (EPIWaferMapViewer)d;
            if (ctrl.WaferMap != null)
                ctrl.WaferMap.WaferSize = (double)e.NewValue;
        }

        private static void OnEdgeExclusionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var ctrl = (EPIWaferMapViewer)d;
            if (ctrl.WaferMap != null)
                ctrl.WaferMap.EdgeExclusion = (double)e.NewValue;
        }

        private static void OnCellWidthChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var ctrl = (EPIWaferMapViewer)d;
            if (ctrl.WaferMap != null)
                ctrl.WaferMap.CellWidth = (double)e.NewValue;
        }

        private static void OnCellHeightChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var ctrl = (EPIWaferMapViewer)d;
            if (ctrl.WaferMap != null)
                ctrl.WaferMap.CellHeight = (double)e.NewValue;
        }

        private static void OnWaferFillChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var ctrl = (EPIWaferMapViewer)d;
            if (ctrl.WaferMap != null)
                ctrl.WaferMap.WaferFill = (Brush)e.NewValue;
        }

        private static void OnWaferStrokeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var ctrl = (EPIWaferMapViewer)d;
            if (ctrl.WaferMap != null)
                ctrl.WaferMap.WaferStroke = (Brush)e.NewValue;
        }

        private static void OnRangeStartChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var ctrl = (EPIWaferMapViewer)d;
            if (ctrl.WaferMapRange != null)
                ctrl.WaferMapRange.RangeStart = (double)e.NewValue;
        }

        private static void OnRangeEndChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var ctrl = (EPIWaferMapViewer)d;
            if (ctrl.WaferMapRange != null)
                ctrl.WaferMapRange.RangeEnd = (double)e.NewValue;
        }

        private static void OnUseAutoRangeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var ctrl = (EPIWaferMapViewer)d;
            if (ctrl.WaferMapRange != null)
                ctrl.WaferMapRange.UseAutoRange = (bool)e.NewValue;
        }

        private static void OnUseDistributionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var ctrl = (EPIWaferMapViewer)d;
            if (ctrl.WaferMapRange != null)
                ctrl.WaferMapRange.UseDistribution = (bool)e.NewValue;
        }

        #endregion

        #region 프로퍼티

        public IEnumerable MapCells
        {
            get => (IEnumerable)GetValue(MapCellsProperty);
            set => SetValue(MapCellsProperty, value);
        }

        public double WaferSize
        {
            get => (double)GetValue(WaferSizeProperty);
            set => SetValue(WaferSizeProperty, value);
        }

        public double EdgeExclusion
        {
            get => (double)GetValue(EdgeExclusionProperty);
            set => SetValue(EdgeExclusionProperty, value);
        }

        public double CellWidth
        {
            get => (double)GetValue(CellWidthProperty);
            set => SetValue(CellWidthProperty, value);
        }

        public double CellHeight
        {
            get => (double)GetValue(CellHeightProperty);
            set => SetValue(CellHeightProperty, value);
        }

        public Brush WaferFill
        {
            get => (Brush)GetValue(WaferFillProperty);
            set => SetValue(WaferFillProperty, value);
        }

        public Brush WaferStroke
        {
            get => (Brush)GetValue(WaferStrokeProperty);
            set => SetValue(WaferStrokeProperty, value);
        }

        public double RangeStart
        {
            get => (double)GetValue(RangeStartProperty);
            set => SetValue(RangeStartProperty, value);
        }

        public double RangeEnd
        {
            get => (double)GetValue(RangeEndProperty);
            set => SetValue(RangeEndProperty, value);
        }

        public bool UseAutoRange
        {
            get => (bool)GetValue(UseAutoRangeProperty);
            set => SetValue(UseAutoRangeProperty, value);
        }

        public bool UseDistribution
        {
            get => (bool)GetValue(UseDistributionProperty);
            set => SetValue(UseDistributionProperty, value);
        }

        #endregion

        /// <summary>
        /// 외부(View 코드비하인드)에서 RangeChanged 구독을 위해 노출
        /// </summary>
        public EPIControls.Controls.BaseUserControls.EPIWaferMapRange RangeControl
            => WaferMapRange;

        public EPIWaferMapViewer()
        {
            InitializeComponent();

            // ✅ Loaded 이후 DP 값을 자식 컨트롤에 동기화
            // InitializeComponent()로 자식이 생성된 뒤 현재 DP 값을 반영
            Loaded += OnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            // InitializeComponent() 전 바인딩으로 설정된 DP 값을
            // 자식 컨트롤이 생성된 지금 다시 밀어넣어줌
            if (WaferMap != null)
            {
                WaferMap.MapCells = MapCells;
                WaferMap.WaferSize = WaferSize;
                WaferMap.EdgeExclusion = EdgeExclusion;
                WaferMap.CellWidth = CellWidth;
                WaferMap.CellHeight = CellHeight;
                WaferMap.WaferFill = WaferFill;
                WaferMap.WaferStroke = WaferStroke;
            }

            if (WaferMapRange != null)
            {
                WaferMapRange.RangeStart = RangeStart;
                WaferMapRange.RangeEnd = RangeEnd;
                WaferMapRange.UseAutoRange = UseAutoRange;
                WaferMapRange.UseDistribution = UseDistribution;

            }
        }
    }
}
