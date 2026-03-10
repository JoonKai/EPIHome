using EPI.Wafers;
using EPIControls.Controls.BaseUserControls;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace EPIControls.Controls.CombinedUserControls
{
    /// <summary>
    /// EPIWaferMap3DViewer.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class EPIWaferMap3DViewer : UserControl
    {
        #region 디펜던시 프로퍼티

        // MapCells: EPIWaferMapViewer와 동일하게 IEnumerable<WaferCell> 받음
        public static readonly DependencyProperty MapCellsProperty =
            DependencyProperty.Register(nameof(MapCells), typeof(IEnumerable), typeof(EPIWaferMap3DViewer),
                new FrameworkPropertyMetadata(null, OnMapCellsChanged));

        public static readonly DependencyProperty RangeStartProperty =
            DependencyProperty.Register(nameof(RangeStart), typeof(double), typeof(EPIWaferMap3DViewer),
                new FrameworkPropertyMetadata(0.0,
                    FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                    OnRangeStartChanged));

        public static readonly DependencyProperty RangeEndProperty =
            DependencyProperty.Register(nameof(RangeEnd), typeof(double), typeof(EPIWaferMap3DViewer),
                new FrameworkPropertyMetadata(1.0,
                    FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                    OnRangeEndChanged));

        public static readonly DependencyProperty UseAutoRangeProperty =
            DependencyProperty.Register(nameof(UseAutoRange), typeof(bool), typeof(EPIWaferMap3DViewer),
                new FrameworkPropertyMetadata(true,
                    FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                    OnUseAutoRangeChanged));

        public static readonly DependencyProperty UseDistributionProperty =
            DependencyProperty.Register(nameof(UseDistribution), typeof(bool), typeof(EPIWaferMap3DViewer),
                new FrameworkPropertyMetadata(false, OnUseDistributionChanged));

        public static readonly DependencyProperty ColorSetProperty =
            DependencyProperty.Register(nameof(ColorSet), typeof(IList<Color>), typeof(EPIWaferMap3DViewer),
                new FrameworkPropertyMetadata(null,
                    FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                    OnColorSetChanged));

        public static readonly DependencyProperty HeightScaleProperty =
            DependencyProperty.Register(nameof(HeightScale), typeof(double), typeof(EPIWaferMap3DViewer),
                new FrameworkPropertyMetadata(3.0, OnHeightScaleChanged));

        #endregion

        #region DP 콜백

        private static void OnMapCellsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var ctrl = (EPIWaferMap3DViewer)d;
            ctrl.ApplyCellsToMap((IEnumerable)e.NewValue);
        }

        private static void OnRangeStartChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var ctrl = (EPIWaferMap3DViewer)d;
            if (ctrl.WaferMapRange != null)
                ctrl.WaferMapRange.RangeStart = (double)e.NewValue;
            if (ctrl.WaferMap3D != null)
                ctrl.WaferMap3D.RangeStart = (double)e.NewValue;
        }

        private static void OnRangeEndChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var ctrl = (EPIWaferMap3DViewer)d;
            if (ctrl.WaferMapRange != null)
                ctrl.WaferMapRange.RangeEnd = (double)e.NewValue;
            if (ctrl.WaferMap3D != null)
                ctrl.WaferMap3D.RangeEnd = (double)e.NewValue;
        }

        private static void OnUseAutoRangeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var ctrl = (EPIWaferMap3DViewer)d;
            if (ctrl.WaferMapRange != null)
                ctrl.WaferMapRange.UseAutoRange = (bool)e.NewValue;
        }

        private static void OnUseDistributionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var ctrl = (EPIWaferMap3DViewer)d;
            if (ctrl.WaferMapRange != null)
                ctrl.WaferMapRange.UseDistribution = (bool)e.NewValue;
        }

        private static void OnColorSetChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var ctrl = (EPIWaferMap3DViewer)d;
            if (ctrl.WaferMapRange != null)
                ctrl.WaferMapRange.ColorSet = (IList<Color>)e.NewValue;
            if (ctrl.WaferMap3D != null)
                ctrl.WaferMap3D.ColorSet = (IList<Color>)e.NewValue;
        }

        private static void OnHeightScaleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var ctrl = (EPIWaferMap3DViewer)d;
            if (ctrl.WaferMap3D != null)
                ctrl.WaferMap3D.HeightScale = (double)e.NewValue;
        }

        #endregion

        #region 프로퍼티

        public IEnumerable MapCells
        {
            get => (IEnumerable)GetValue(MapCellsProperty);
            set => SetValue(MapCellsProperty, value);
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

        public IList<Color> ColorSet
        {
            get => (IList<Color>)GetValue(ColorSetProperty);
            set => SetValue(ColorSetProperty, value);
        }

        /// <summary>Z축 높이 배율 (기본 3.0)</summary>
        public double HeightScale
        {
            get => (double)GetValue(HeightScaleProperty);
            set => SetValue(HeightScaleProperty, value);
        }

        #endregion

        #region 생성자 / Loaded

        public EPIWaferMap3DViewer()
        {
            InitializeComponent();
            Loaded += OnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            // WaferMapRange 초기값 동기화
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

            // WaferMap3D 초기값 동기화
            if (WaferMap3D != null)
            {
                WaferMap3D.RangeStart = RangeStart;
                WaferMap3D.RangeEnd = RangeEnd;
                WaferMap3D.HeightScale = HeightScale;

                if (ColorSet != null)
                    WaferMap3D.ColorSet = ColorSet;
            }

            // 셀 데이터 전달
            ApplyCellsToMap(MapCells);
        }

        #endregion

        #region 이벤트 핸들러

        /// <summary>
        /// EPIWaferMapRange에서 Range가 바뀌면 → WaferMap3D에도 즉시 반영
        /// EPIWaferMapViewer.OnWaferMapRangeChanged와 동일한 패턴
        /// </summary>
        private void OnWaferMapRangeChanged(object sender, EventArgs e)
        {
            var range = sender as EPIWaferMapRange;
            if (range == null) return;

            // 루프 방지: 값이 다를 때만 SetValue
            if (RangeStart != range.RangeStart)
                SetValue(RangeStartProperty, range.RangeStart);

            if (RangeEnd != range.RangeEnd)
                SetValue(RangeEndProperty, range.RangeEnd);

            if (UseAutoRange != range.UseAutoRange)
                SetValue(UseAutoRangeProperty, range.UseAutoRange);

            if (range.ColorSet != null)
                SetValue(ColorSetProperty, range.ColorSet);

            // 3D 맵에 즉시 적용
            if (WaferMap3D != null)
            {
                WaferMap3D.RangeStart = range.RangeStart;
                WaferMap3D.RangeEnd = range.RangeEnd;
                WaferMap3D.ColorSet = range.ColorSet;
            }
        }

        #endregion

        #region 내부 메서드

        /// <summary>
        /// IEnumerable → IEnumerable&lt;WaferCell&gt; 변환 후 WaferMap3D에 전달
        /// EPIWaferMapViewer는 WaferMap.MapCells(IEnumerable)로 넘기지만
        /// EPIWaferMap3D는 LoadCells(IEnumerable&lt;WaferCell&gt;)를 사용하므로 여기서 변환
        /// </summary>
        private void ApplyCellsToMap(IEnumerable source)
        {
            if (WaferMap3D == null) return;

            if (source == null)
            {
                WaferMap3D.LoadCells(null);
                return;
            }

            var cells = new List<WaferCell>();
            foreach (var item in source)
            {
                if (item is WaferCell cell)
                    cells.Add(cell);
            }

            WaferMap3D.LoadCells(cells);

            // AutoRange: 셀 값의 Min/Max로 Range 자동 설정
            if (UseAutoRange && cells.Count > 0)
            {
                var vals = cells
                    .Where(c => c.Value.HasValue)
                    .Select(c => c.Value.Value)
                    .ToList();

                if (vals.Count > 0)
                {
                    double min = vals.Min();
                    double max = vals.Max();

                    // 루프 방지
                    if (RangeStart != min) SetValue(RangeStartProperty, min);
                    if (RangeEnd != max) SetValue(RangeEndProperty, max);

                    if (WaferMapRange != null)
                    {
                        WaferMapRange.RangeStart = min;
                        WaferMapRange.RangeEnd = max;
                    }

                    WaferMap3D.RangeStart = min;
                    WaferMap3D.RangeEnd = max;
                }
            }

            // Distribution 데이터 전달 (WaferMapRange 히스토그램용)
            UpdateDistribution(cells);
        }

        /// <summary>
        /// ColorMapService를 통해 히스토그램 분포 계산 후 WaferMapRange에 전달
        /// EPIWaferMapViewer와 동일한 역할
        /// </summary>
        private void UpdateDistribution(List<WaferCell> cells)
        {
            if (WaferMapRange == null || !UseDistribution) return;

            var vals = cells
                .Where(c => c.Value.HasValue)
                .Select(c => c.Value.Value)
                .ToList();

            if (vals.Count == 0) return;

            var colorMap = new ColorMapService();
            var dist = colorMap.BuildDistribution(vals, WaferMapRange.ColorBarWidth,
                                                  RangeStart, RangeEnd);
            WaferMapRange.Distributions = dist;
        }

        #endregion
    }
}
