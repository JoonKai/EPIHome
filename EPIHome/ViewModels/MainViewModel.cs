using Caliburn.Micro;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;

namespace EPIHome.ViewModels
{
    public class MainViewModel : Conductor<IScreen>.Collection.OneActive
    {
        private readonly IWindowManager _windowManager;

        public MainViewModel(IWindowManager windowManager)
        {
            _windowManager = windowManager;
        }

        // ✅ 앱 시작 시 자동으로 WaferMap 활성화
        protected override async Task OnInitializeAsync(CancellationToken cancellationToken)
        {
            //await ActivateItemAsync(new EPIWaferMapViewModel(), cancellationToken);
            await ActivateItemAsync(new EPIWaferMap3DViewModel(), cancellationToken);
        }

        public async Task OpenWaferMap()
        {
            await ActivateItemAsync(new EPIWaferMapViewModel());
        }

        public async Task OpenWaferMapWindow()
        {
            await _windowManager.ShowWindowAsync(new EPIWaferMapViewModel());
        }
    }
}
