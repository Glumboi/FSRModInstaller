using System.Collections.Generic;
using System.Security.Principal;
using System.Windows;
using System.Windows.Input;
using FSRModInstaller.FsrFiles;
using Microsoft.WindowsAPICodePack.Dialogs;
using Wpf.Ui.Common;
using Wpf.Ui.Controls;

namespace FSRModInstaller.ViewModels;

internal class MainPageViewModel : ViewModelBase
{
    private List<string> _gameNames = new List<string>();

    public List<string> GameNames
    {
        get => _gameNames;
        set
        {
            if (value != _gameNames)
            {
                SetProperty(ref _gameNames, value);
            }
        }
    }

    private List<string> _gameBins = new List<string>();

    public List<string> GameBins
    {
        get => _gameBins;
        set
        {
            if (value != _gameBins)
            {
                SetProperty(ref _gameBins, value);
            }
        }
    }

    private List<string> _fsrVersions = new List<string>();

    public List<string> FsrVersions
    {
        get => _fsrVersions;
        set
        {
            if (value != _fsrVersions)
            {
                SetProperty(ref _fsrVersions, value);
            }
        }
    }

    private List<string> _fsrVersionsPath = new List<string>();

    public List<string> FsrVersionsPath
    {
        get => _fsrVersionsPath;
        set
        {
            if (value != _fsrVersionsPath)
            {
                SetProperty(ref _fsrVersionsPath, value);
            }
        }
    }

    private string _gamePath = Properties.Settings.Default.LastPath;

    public string GamePath
    {
        get => _gamePath;
        set
        {
            if (value != _gamePath)
            {
                Properties.Settings.Default.LastPath = value;
                SetProperty(ref _gamePath, value);
            }
        }
    }

    private int _selectedConfigIndex = Properties.Settings.Default.LastConfigIndex;

    public int SelectedConfigIndex
    {
        get => _selectedConfigIndex;
        set
        {
            if (value != _selectedConfigIndex)
            {
                Properties.Settings.Default.LastConfigIndex = value;
                SetProperty(ref _selectedConfigIndex, value);
            }
        }
    }

    private int _selectedFsrIndex = Properties.Settings.Default.LastFsrIndex;

    public int SelectedFsrIndex
    {
        get => _selectedFsrIndex;
        set
        {
            if (value != _selectedFsrIndex)
            {
                Properties.Settings.Default.LastFsrIndex = value;
                SetProperty(ref _selectedFsrIndex, value);
            }
        }
    }

    private void GetConfigs()
    {
        foreach (var item in Xml.GameConfigs.GetAllGames("./Xml/GameConfigs.xml"))
        {
            GameNames.Add(item.Name);
            GameBins.Add(item.Bin);
        }
    }

    private void GetFsrVersions()
    {
        FsrFiles.FsrVersions.GetAllFsrVersions(); //Get all versions

        //Assign versions to vm
        FsrVersionsPath = FsrFiles.FsrVersions.FsrVersionsPath;
        FsrVersions = FsrFiles.FsrVersions.FsrVersionsName;
    }

    public ICommand InstallFsrCommand
    {
        get;
        internal set;
    }

    private bool ValidConfig()
    {
        if (string.IsNullOrWhiteSpace(GamePath))
        {
            return false;
        }

        if (SelectedConfigIndex == null)
        {
            return false;
        }

        if (GameNames.Count < 1)
        {
            return false;
        }

        return true;
    }

    private void CreateClickInstallCommand()
    {
        InstallFsrCommand = new RelayCommand(InstallFsr, ValidConfig);
    }

    private void InstallFsr()
    {
        try
        {
            FSRModInstaller.FsrFiles.FsrBinaries.InstallFsr(SelectedFsrIndex, GamePath + "\\" +
                GameBins[SelectedConfigIndex]);

            ShowNotification($"Successfully installed FSR {FsrVersions[SelectedFsrIndex]} " +
                $"to the Game {GameNames[SelectedConfigIndex]}");
        }
        catch (System.Exception)
        {
            ShowNotification("Something went wrong in the Installation Process!\n" +
                "Make sure all configs and FSR Versions are valid!", SymbolRegular.ErrorCircle24);
        }
    }

    public ICommand UninstallFsrCommand
    {
        get;
        internal set;
    }

    private bool IsGamePathFilled()
    {
        return !string.IsNullOrEmpty(GamePath);
    }

    private void CreateUninstallFsrCommand()
    {
        UninstallFsrCommand = new RelayCommand(UninstallFsr, IsGamePathFilled);
    }

    private void UninstallFsr()
    {
        try
        {
            FsrBinaries.UninstallFsr(SelectedFsrIndex,
           GamePath + "\\" + GameBins[SelectedConfigIndex]);

            ShowNotification($"Successfully uninstalled FSR {FsrVersions[SelectedFsrIndex]} " +
                $"from the Game {GameNames[SelectedConfigIndex]}");
        }
        catch (System.Exception)
        {
            ShowNotification("Something went wrong in the uninstallation Process!\n" +
                "Make sure all configs and FSR Versions are valid!", SymbolRegular.ErrorCircle24);
        }
    }

    public ICommand BrowseFolderCommand
    {
        get;
        internal set;
    }

    private bool CanChoose()
    {
        return true;
    }

    private void CreateBrowseFolderCommand()
    {
        BrowseFolderCommand = new RelayCommand(BrowseFolder, CanChoose);
    }

    private void BrowseFolder()
    {
        var dialog = new CommonOpenFileDialog();
        dialog.IsFolderPicker = true;
        CommonFileDialogResult result = dialog.ShowDialog();

        if (result == CommonFileDialogResult.Ok)
        {
            GamePath = dialog.FileName;
        }
    }

    public ICommand ApplyRegistryTweakscommand
    {
        get;
        internal set;
    }

    private bool CanExecuteRegFiles()
    {
        var identity = WindowsIdentity.GetCurrent();
        var principal = new WindowsPrincipal(identity);
        return principal.IsInRole(WindowsBuiltInRole.Administrator);
    }

    private void CreateApplyRegistryTweaksCommand()
    {
        ApplyRegistryTweakscommand = new RelayCommand(ExecuteInstallReg, CanExecuteRegFiles);
    }

    private void ExecuteInstallReg()
    {
        FsrBinaries.ExecuteReg(true);
    }

    public ICommand RemoveRegistryTweakscommand
    {
        get;
        internal set;
    }

    private void CreateRemoveRegistryTweaksCommand()
    {
        RemoveRegistryTweakscommand = new RelayCommand(ExecuteUninstallReg, CanExecuteRegFiles);
    }

    private void ExecuteUninstallReg()
    {
        FsrBinaries.ExecuteReg(false);
    }

    public Snackbar NotificationBar
    {
        get;
        set;
    }

    private void LoadNotificationBar()
    {
        NotificationBar = new Snackbar()
        {
            Icon = SymbolRegular.Info28,
            Timeout = 4000,
            FontWeight = FontWeights.Bold,
            Content = "{This is a Placeholder, replace later}"
        };
    }

    private void ShowNotification(string content, SymbolRegular icon = SymbolRegular.Info28)
    {
        NotificationBar.Content = content;
        NotificationBar.Icon = icon;

        NotificationBar.Show();
    }

    public MainPageViewModel()
    {
        CreateClickInstallCommand();
        CreateUninstallFsrCommand();
        CreateBrowseFolderCommand();
        CreateApplyRegistryTweaksCommand();
        CreateRemoveRegistryTweaksCommand();
        LoadNotificationBar();
        GetConfigs();
        GetFsrVersions();
    }
}