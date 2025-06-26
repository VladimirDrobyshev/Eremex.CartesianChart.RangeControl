using Avalonia.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using Eremex.AvaloniaUI.Charts;

namespace CartesianRangeControl.ViewModels;

public partial class SeriesViewModel : ViewModelBase
{
    [ObservableProperty] Color color;
    [ObservableProperty] ISeriesDataAdapter? dataAdapter;
    [ObservableProperty] double thickness = 2;
}
