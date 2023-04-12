using System;
using FSRModInstaller.ViewModels;
using Wpf.Ui.Controls;

namespace FSRModInstaller.Pages;
/// <summary>
/// Interaction logic for MainPage.xaml
/// </summary>
public partial class MainPage : UiPage
{
    private readonly MainPageViewModel viewModel;

    public MainPage()
    {
        InitializeComponent();
    }
}