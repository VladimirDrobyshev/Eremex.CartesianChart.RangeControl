using System;
using System.Collections.ObjectModel;
using Avalonia;
using Avalonia.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using Eremex.AvaloniaUI.Charts;

namespace CartesianRangeControl.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    static double Sin(double argument) => Math.Sin(argument);
    static double Cos(double argument) => Math.Cos(argument);

    const int ItemsCount = 1000;
    const double Step = 0.025;

    [ObservableProperty] ObservableCollection<SeriesViewModel> series = new()
    {
        new SeriesViewModel { Color = Color.FromArgb(255, 189, 20, 54), DataAdapter = new FormulaDataAdapter(0, Step, ItemsCount, Sin)},
        new SeriesViewModel { Color = Color.FromArgb(255, 0, 120, 122), DataAdapter = new FormulaDataAdapter(0, Step, ItemsCount, Cos)},
    };
    [ObservableProperty] double visualMin = 10;
    [ObservableProperty] double visualMax = 15;
    [ObservableProperty] Thickness diagramMargin = new(50, 0, 10, 0);
}
