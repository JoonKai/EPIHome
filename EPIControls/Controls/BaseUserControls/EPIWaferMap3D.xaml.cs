using EPI.Wafers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace EPIControls.Controls.BaseUserControls
{
    /// <summary>
    /// EPIWaferMap3d.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class EPIWaferMap3D : UserControl
    {
        // ════════════════════════════════════════════════════════════════════
        // DependencyProperties  (EPIWaferMapRange 와 동일한 이름으로 바인딩)
        // ════════════════════════════════════════════════════════════════════

        public static readonly DependencyProperty RangeStartProperty =
            DependencyProperty.Register(nameof(RangeStart), typeof(double), typeof(EPIWaferMap3D),
                new FrameworkPropertyMetadata(0.0, OnRangeOrColorChanged));

        public static readonly DependencyProperty RangeEndProperty =
            DependencyProperty.Register(nameof(RangeEnd), typeof(double), typeof(EPIWaferMap3D),
                new FrameworkPropertyMetadata(1.0, OnRangeOrColorChanged));

        public static readonly DependencyProperty ColorSetProperty =
            DependencyProperty.Register(nameof(ColorSet), typeof(IList<Color>), typeof(EPIWaferMap3D),
                new FrameworkPropertyMetadata(null, OnRangeOrColorChanged));

        public static readonly DependencyProperty HeightScaleProperty =
            DependencyProperty.Register(nameof(HeightScale), typeof(double), typeof(EPIWaferMap3D),
                new FrameworkPropertyMetadata(3.0, OnRangeOrColorChanged));

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
        public IList<Color> ColorSet
        {
            get => (IList<Color>)GetValue(ColorSetProperty);
            set => SetValue(ColorSetProperty, value);
        }
        public double HeightScale
        {
            get => (double)GetValue(HeightScaleProperty);
            set => SetValue(HeightScaleProperty, value);
        }

        // ════════════════════════════════════════════════════════════════════
        // 멤버
        // ════════════════════════════════════════════════════════════════════

        private List<WaferCell> _cells = new List<WaferCell>();

        // 카메라 상태
        private PerspectiveCamera _camera;
        private double _theta = 45.0;
        private double _phi = 40.0;
        private double _zoom = 3.5;
        private Point3D _lookAt = new Point3D(0, 0, 1.2);

        // 마우스 상태
        private bool _rotating = false;
        private bool _panning = false;
        private Point _lastMouse;

        // ════════════════════════════════════════════════════════════════════
        // 생성자
        // ════════════════════════════════════════════════════════════════════

        public EPIWaferMap3D()
        {
            InitializeComponent();
            Loaded += OnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            SetupCamera();
            Rebuild();
        }

        // ════════════════════════════════════════════════════════════════════
        // Public API
        // ════════════════════════════════════════════════════════════════════

        /// <summary>
        /// WaferCell 목록을 받아 3D 맵을 그린다.
        /// EPIWaferMap.MapCells 와 동일한 컬렉션을 그대로 전달하면 된다.
        /// </summary>
        public void LoadCells(IEnumerable<WaferCell> cells)
        {
            _cells = cells?.ToList() ?? new List<WaferCell>();
            Rebuild();
        }

        /// <summary>카메라를 기본 위치로 리셋 (더블클릭과 동일)</summary>
        public void ResetCamera()
        {
            _theta = 45.0;
            _phi = 40.0;
            _zoom = 3.5;
            _lookAt = new Point3D(0, 0, 1.2);
            UpdateCamera();
        }

        /// <summary>Top-down 뷰 (2D 맵과 동일한 시점)</summary>
        public void SetTopView()
        {
            _theta = 0.0;
            _phi = 89.0;
            _zoom = 3.5;
            _lookAt = new Point3D(0, 0, 1.5);
            UpdateCamera();
        }

        // ════════════════════════════════════════════════════════════════════
        // DependencyProperty 콜백
        // ════════════════════════════════════════════════════════════════════

        private static void OnRangeOrColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as EPIWaferMap3D)?.Rebuild();
        }

        // ════════════════════════════════════════════════════════════════════
        // 3D 씬 빌드
        // ════════════════════════════════════════════════════════════════════

        private void Rebuild()
        {
            // PART_ 요소가 아직 생성되지 않은 경우 (InitializeComponent 전) 방어
            if (PART_SceneVisual == null) return;

            if (_cells == null || _cells.Count == 0)
            {
                PART_SceneVisual.Content = null;
                if (PART_StatsText != null)
                    PART_StatsText.Text = "셀 데이터 없음";
                return;
            }

            var validCells = _cells.Where(c => c.Value.HasValue).ToList();
            if (validCells.Count == 0)
            {
                if (PART_StatsText != null)
                    PART_StatsText.Text = "유효한 값이 없습니다.";
                return;
            }

            double rangeStart = RangeStart;
            double rangeEnd = RangeEnd;
            double span = rangeEnd - rangeStart;
            if (span < 1e-9) span = 1.0;

            double heightScale = HeightScale;

            // Row/Col 범위 파악
            int minRow = validCells.Min(c => c.Row);
            int maxRow = validCells.Max(c => c.Row);
            int minCol = validCells.Min(c => c.Col);
            int maxCol = validCells.Max(c => c.Col);
            int rowRange = Math.Max(1, maxRow - minRow);
            int colRange = Math.Max(1, maxCol - minCol);

            // 씬 구성
            var scene = new Model3DGroup();

            // 조명
            scene.Children.Add(new AmbientLight(Color.FromRgb(85, 85, 95)));
            scene.Children.Add(new DirectionalLight(Colors.White,
                new Vector3D(-0.6, -0.8, -1.0)));
            scene.Children.Add(new DirectionalLight(
                Color.FromRgb(100, 110, 200),
                new Vector3D(0.7, 0.5, -0.4)));

            // 바닥 그리드
            BuildFloorGrid(scene);

            // 각 WaferCell → 직육면체 기둥
            double cellW = 2.0 / colRange * 0.88;
            double cellH = 2.0 / rowRange * 0.88;

            foreach (var cell in validCells)
            {
                double val = cell.Value.Value;

                // Row → Y축, Col → X축 (-1 ~ +1 정규화)
                double cx = ((double)(cell.Col - minCol) / colRange) * 2.0 - 1.0;
                double cy = ((double)(cell.Row - minRow) / rowRange) * 2.0 - 1.0;

                // Z 높이
                double t = Math.Max(0.0, Math.Min(1.0, (val - rangeStart) / span));
                double zTop = Math.Max(0.001, t * heightScale);

                // 색상: ViewModel에서 ColorSets로 이미 계산된 WaferCell.Brush 그대로 사용
                // ColorSet 팔레트 변경 시 ViewModel이 Brush를 갱신 → 여기서 자동 반영
                Color color = (cell.Brush is SolidColorBrush scb)
                    ? scb.Color
                    : Colors.Gray;

                AddCellBox(scene, cx, cy, 0.0, zTop, cellW * 0.5, cellH * 0.5, color);
            }

            // 웨이퍼 외곽 원
            AddWaferOutline(scene);

            PART_SceneVisual.Content = scene;

            // 통계 업데이트
            UpdateStats(validCells);
        }

        // ── 셀 기둥 (직육면체) ───────────────────────────────────────────────

        private void AddCellBox(Model3DGroup scene,
            double cx, double cy, double zBot, double zTop,
            double hw, double hh, Color color)
        {
            // 8 꼭짓점
            var p = new[]
            {
                new Point3D(cx - hw, cy - hh, zBot), // 0
                new Point3D(cx + hw, cy - hh, zBot), // 1
                new Point3D(cx + hw, cy + hh, zBot), // 2
                new Point3D(cx - hw, cy + hh, zBot), // 3
                new Point3D(cx - hw, cy - hh, zTop), // 4
                new Point3D(cx + hw, cy - hh, zTop), // 5
                new Point3D(cx + hw, cy + hh, zTop), // 6
                new Point3D(cx - hw, cy + hh, zTop), // 7
            };

            Color sideColor = Darken(color, 0.72);

            // 상단면 (원색)
            var topMesh = BuildFaceMesh(p, 4, 5, 6, 7);
            // 측면 (어두운 색)
            var sideMesh = new MeshGeometry3D();
            foreach (var pt in p) sideMesh.Positions.Add(pt);
            AddQuad(sideMesh, 0, 1, 5, 4);
            AddQuad(sideMesh, 3, 7, 6, 2);
            AddQuad(sideMesh, 0, 4, 7, 3);
            AddQuad(sideMesh, 1, 2, 6, 5);

            var topMat = new DiffuseMaterial(new SolidColorBrush(color));
            var sideMat = new DiffuseMaterial(new SolidColorBrush(sideColor));

            scene.Children.Add(new GeometryModel3D { Geometry = topMesh, Material = topMat, BackMaterial = topMat });
            scene.Children.Add(new GeometryModel3D { Geometry = sideMesh, Material = sideMat, BackMaterial = sideMat });
        }

        private static MeshGeometry3D BuildFaceMesh(Point3D[] p, int a, int b, int c, int d)
        {
            var mesh = new MeshGeometry3D();
            foreach (var pt in p) mesh.Positions.Add(pt);
            AddQuad(mesh, a, b, c, d);
            return mesh;
        }

        private static void AddQuad(MeshGeometry3D mesh, int a, int b, int c, int d)
        {
            mesh.TriangleIndices.Add(a); mesh.TriangleIndices.Add(b); mesh.TriangleIndices.Add(c);
            mesh.TriangleIndices.Add(a); mesh.TriangleIndices.Add(c); mesh.TriangleIndices.Add(d);
        }

        // ── 웨이퍼 외곽 원 ──────────────────────────────────────────────────

        private static void AddWaferOutline(Model3DGroup scene)
        {
            const int segs = 72;
            const double r = 1.03;
            const double w = 0.015;
            var mesh = new MeshGeometry3D();

            for (int i = 0; i < segs; i++)
            {
                double a0 = 2 * Math.PI * i / segs;
                double a1 = 2 * Math.PI * (i + 1) / segs;

                int b = mesh.Positions.Count;
                mesh.Positions.Add(new Point3D(Math.Cos(a0) * (r - w), Math.Sin(a0) * (r - w), 0));
                mesh.Positions.Add(new Point3D(Math.Cos(a0) * r, Math.Sin(a0) * r, 0));
                mesh.Positions.Add(new Point3D(Math.Cos(a1) * (r - w), Math.Sin(a1) * (r - w), 0));
                mesh.Positions.Add(new Point3D(Math.Cos(a1) * r, Math.Sin(a1) * r, 0));

                mesh.TriangleIndices.Add(b); mesh.TriangleIndices.Add(b + 2); mesh.TriangleIndices.Add(b + 1);
                mesh.TriangleIndices.Add(b + 1); mesh.TriangleIndices.Add(b + 2); mesh.TriangleIndices.Add(b + 3);
            }

            var mat = new DiffuseMaterial(new SolidColorBrush(Color.FromRgb(220, 160, 60)));
            scene.Children.Add(new GeometryModel3D { Geometry = mesh, Material = mat, BackMaterial = mat });
        }

        // ── 바닥 그리드 ─────────────────────────────────────────────────────

        private static void BuildFloorGrid(Model3DGroup scene)
        {
            const int lines = 8;
            const double size = 1.1;
            const double z = -0.02;
            const double lw = 0.004;
            var mesh = new MeshGeometry3D();

            for (int i = 0; i <= lines; i++)
            {
                double pos = -size + i * (size * 2.0 / lines);
                AddFloorLine(mesh, -size, pos, size, pos, z, lw);
                AddFloorLine(mesh, pos, -size, pos, size, z, lw);
            }

            var mat = new DiffuseMaterial(new SolidColorBrush(Color.FromArgb(70, 80, 100, 150)));
            scene.Children.Add(new GeometryModel3D { Geometry = mesh, Material = mat, BackMaterial = mat });
        }

        private static void AddFloorLine(MeshGeometry3D mesh,
            double x0, double y0, double x1, double y1, double z, double w)
        {
            double dx = x1 - x0, dy = y1 - y0;
            double len = Math.Sqrt(dx * dx + dy * dy);
            if (len < 1e-9) return;
            double nx = -dy / len * w * 0.5;
            double ny = dx / len * w * 0.5;

            int b = mesh.Positions.Count;
            mesh.Positions.Add(new Point3D(x0 + nx, y0 + ny, z));
            mesh.Positions.Add(new Point3D(x0 - nx, y0 - ny, z));
            mesh.Positions.Add(new Point3D(x1 + nx, y1 + ny, z));
            mesh.Positions.Add(new Point3D(x1 - nx, y1 - ny, z));
            AddQuad(mesh, b, b + 1, b + 3, b + 2);
        }

        // ════════════════════════════════════════════════════════════════════
        // 카메라
        // ════════════════════════════════════════════════════════════════════

        private void SetupCamera()
        {
            _camera = new PerspectiveCamera
            {
                FieldOfView = 45,
                NearPlaneDistance = 0.01,
                FarPlaneDistance = 100
            };
            PART_Viewport.Camera = _camera;
            UpdateCamera();
        }

        private void UpdateCamera()
        {
            if (_camera == null) return;

            double tr = _theta * Math.PI / 180.0;
            double pr = _phi * Math.PI / 180.0;

            double x = _zoom * Math.Cos(pr) * Math.Sin(tr);
            double y = _zoom * Math.Cos(pr) * Math.Cos(tr);
            double z = _zoom * Math.Sin(pr);

            _camera.Position = new Point3D(_lookAt.X + x, _lookAt.Y + y, _lookAt.Z + z);
            _camera.LookDirection = new Vector3D(
                _lookAt.X - _camera.Position.X,
                _lookAt.Y - _camera.Position.Y,
                _lookAt.Z - _camera.Position.Z);
            _camera.UpDirection = new Vector3D(0, 0, 1);
        }

        // ════════════════════════════════════════════════════════════════════
        // 마우스 이벤트 (XAML에서 연결)
        // ════════════════════════════════════════════════════════════════════

        private void Viewport_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2) { ResetCamera(); return; }
            _rotating = true;
            _lastMouse = e.GetPosition(PART_MouseOverlay);
            PART_MouseOverlay.CaptureMouse();
        }

        private void Viewport_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            _rotating = false;
            PART_MouseOverlay.ReleaseMouseCapture();
        }

        private void Viewport_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            _panning = true;
            _lastMouse = e.GetPosition(PART_MouseOverlay);
            PART_MouseOverlay.CaptureMouse();
        }

        private void Viewport_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            _panning = false;
            PART_MouseOverlay.ReleaseMouseCapture();
        }

        private void Viewport_MouseMove(object sender, MouseEventArgs e)
        {
            if (!_rotating && !_panning) return;

            Point cur = e.GetPosition(PART_MouseOverlay);
            double dx = cur.X - _lastMouse.X;
            double dy = cur.Y - _lastMouse.Y;
            _lastMouse = cur;

            if (_rotating)
            {
                _theta -= dx * 0.45;
                _phi = Math.Max(5, Math.Min(88, _phi + dy * 0.45));
            }
            else // panning
            {
                double speed = _zoom * 0.002;
                _lookAt = new Point3D(
                    _lookAt.X - dx * speed,
                    _lookAt.Y + dy * speed,
                    _lookAt.Z);
            }
            UpdateCamera();
        }

        private void Viewport_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            _zoom *= e.Delta > 0 ? 0.88 : 1.12;
            _zoom = Math.Max(1.0, Math.Min(15.0, _zoom));
            UpdateCamera();
        }

        // ════════════════════════════════════════════════════════════════════
        // 통계
        // ════════════════════════════════════════════════════════════════════

        private void UpdateStats(List<WaferCell> cells)
        {
            if (PART_StatsText == null || cells.Count == 0) return;

            var vals = cells.Where(c => c.Value.HasValue).Select(c => c.Value.Value).ToList();
            if (vals.Count == 0) return;

            double avg = vals.Average();
            double min = vals.Min();
            double max = vals.Max();
            double sd = Math.Sqrt(vals.Average(v => (v - avg) * (v - avg)));
            double uni = avg > 0 ? (max - min) / avg * 100.0 : 0;

            PART_StatsText.Text =
                $"Cells: {vals.Count}  │  " +
                $"Mean: {avg:F3} nm  │  " +
                $"Min: {min:F3} nm  │  " +
                $"Max: {max:F3} nm  │  " +
                $"Range: {(max - min):F3} nm  │  " +
                $"σ: {sd:F3} nm  │  " +
                $"Uniformity: {uni:F2} %";
        }

        // ════════════════════════════════════════════════════════════════════
        // 유틸
        // ════════════════════════════════════════════════════════════════════

        private static Color Darken(Color c, double factor)
            => Color.FromRgb(
                (byte)(c.R * factor),
                (byte)(c.G * factor),
                (byte)(c.B * factor));
    }
}
