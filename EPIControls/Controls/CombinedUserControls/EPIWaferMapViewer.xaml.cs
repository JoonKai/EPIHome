using EPIControls.Controls.BaseUserControls;
using System.Collections;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace EPIControls.Controls.CombinedUserControls
{
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

        // ✅ TwoWay 기본값으로 등록 — 바인딩 시 Mode=TwoWay 생략 가능
        public static readonly DependencyProperty RangeStartProperty =
            DependencyProperty.Register(nameof(RangeStart), typeof(double), typeof(EPIWaferMapViewer),
                new FrameworkPropertyMetadata(0.0,
                    FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                    OnRangeStartChanged));

        public static readonly DependencyProperty RangeEndProperty =
            DependencyProperty.Register(nameof(RangeEnd), typeof(double), typeof(EPIWaferMapViewer),
                new FrameworkPropertyMetadata(1.0,
                    FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                    OnRangeEndChanged));

        public static readonly DependencyProperty UseAutoRangeProperty =
            DependencyProperty.Register(nameof(UseAutoRange), typeof(bool), typeof(EPIWaferMapViewer),
                new FrameworkPropertyMetadata(true,
                    FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                    OnUseAutoRangeChanged));

        public static readonly DependencyProperty UseDistributionProperty =
            DependencyProperty.Register(nameof(UseDistribution), typeof(bool), typeof(EPIWaferMapViewer),
                new FrameworkPropertyMetadata(false, OnUseDistributionChanged));

        // ✅ ColorSet DP 추가 — TwoWay로 ViewModel과 팔레트 동기화
        public static readonly DependencyProperty ColorSetProperty =
            DependencyProperty.Register(nameof(ColorSet), typeof(IList<Color>), typeof(EPIWaferMapViewer),
                new FrameworkPropertyMetadata(null,
                    FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                    OnColorSetChanged));

        #endregion

        #region DP 콜백

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

        // ✅ ViewModel → WaferMapRange 팔레트 전달
        private static void OnColorSetChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var ctrl = (EPIWaferMapViewer)d;
            if (ctrl.WaferMapRange != null)
                ctrl.WaferMapRange.ColorSet = (IList<Color>)e.NewValue;
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

        // ✅ 추가
        public IList<Color> ColorSet
        {
            get => (IList<Color>)GetValue(ColorSetProperty);
            set => SetValue(ColorSetProperty, value);
        }

        #endregion

        public EPIWaferMapViewer()
        {
            InitializeComponent();
            Loaded += OnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            if (WaferMap != null)
            {
                WaferMap.MapCells = MapCells;
                WaferMap.WaferSize = WaferSize;
                WaferMap.EdgeExclusion = EdgeExclusion;
                WaferMap.CellWidth = CellWidth;
                WaferMap.CellHeight = CellHeight;
                WaferMap.WaferFill = WaferFill;
                WaferMap.WaferStroke = WaferStroke;

                // ✅ 호버 이벤트 연결
                WaferMap.CellHovered += OnCellHovered;
            }

            if (WaferMapRange != null)
            {
                WaferMapRange.RangeStart = RangeStart;
                WaferMapRange.RangeEnd = RangeEnd;
                WaferMapRange.UseAutoRange = UseAutoRange;
                WaferMapRange.UseDistribution = UseDistribution;

                if (ColorSet != null)
                    WaferMapRange.ColorSet = ColorSet;

                WaferMapRange.RangeChanged += OnWaferMapRangeChanged;
            }
        }

        // ✅ 호버 셀 정보를 HoverInfoText에 표시
        private void OnCellHovered(object sender, WaferCellHoveredEventArgs e)
        {
            if (HoverInfoText == null) return;
            if (e.Cell == null) { HoverInfoText.Text = "—"; return; }

            var c = e.Cell;
            var sb = new System.Text.StringBuilder();
            sb.Append($"Row: {e.Row}  Col: {e.Col}");
            sb.Append($"  |  PW: {c.Value:F4}");
            sb.Append($"  DW: {c.DominantWavelength:F4}");
            sb.Append($"  FWHM: {c.FWHM:F4}");
            sb.Append($"  PI: {c.PeakIntensity:F4}");
            sb.Append($"  II: {c.IntegratedIntensity:F4}");

            if (c.Reflection != 0)
                sb.Append($"  Refl: {c.Reflection:F4}");
            if (c.Thickness != 0)
                sb.Append($"  Thick: {c.Thickness:F4}");
            if (c.Transmittance != 0)
                sb.Append($"  Trans: {c.Transmittance:F4}");
            if (c.PhotoDetect != 0)
                sb.Append($"  PD: {c.PhotoDetect:F4}");
            if (c.ZPos != 0)
                sb.Append($"  Z: {c.ZPos:F4}");
            if (c.BlueReflection != 0)
                sb.Append($"  BlueRefl: {c.BlueReflection:F4}");

            HoverInfoText.Text = sb.ToString();
        }

        // ✅ RangeChanged: sender(EPIWaferMapRange)에서 직접 값 읽기
        //    EventArgs.Empty이므로 args에서 읽을 수 없음
        private void OnWaferMapRangeChanged(object sender, System.EventArgs e)
        {
            var range = sender as EPIControls.Controls.BaseUserControls.EPIWaferMapRange;
            if (range == null) return;

            // 루프 방지: 값이 다를 때만 SetValue
            if (RangeStart != range.RangeStart)
                SetValue(RangeStartProperty, range.RangeStart);

            if (RangeEnd != range.RangeEnd)
                SetValue(RangeEndProperty, range.RangeEnd);

            if (UseAutoRange != range.UseAutoRange)
                SetValue(UseAutoRangeProperty, range.UseAutoRange);

            // ✅ ColorSet: 팔레트 교체 시 ViewModel로 역전달
            if (range.ColorSet != null)
                SetValue(ColorSetProperty, range.ColorSet);
        }
    }
}