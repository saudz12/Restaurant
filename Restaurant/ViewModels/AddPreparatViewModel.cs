using Database.Enums;
using Database.Services.Dtos;
using Database.Services.Interfaces;
using Database;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Restaurant.ViewModels;

public class AddPreparatViewModel : INotifyPropertyChanged
{
    private readonly IPreparatService _preparatService;
    private readonly IAlergenService _alergenService;
    private readonly IDataRefreshService _dataRefreshService;

    #region Properties

    private string _nume;
    public string Nume
    {
        get => _nume;
        set { _nume = value; OnPropertyChanged(); }
    }

    private double _pret;
    public double Pret
    {
        get => _pret;
        set { _pret = value; OnPropertyChanged(); }
    }

    private int _cantitatePortie;
    public int CantitatePortie
    {
        get => _cantitatePortie;
        set { _cantitatePortie = value; OnPropertyChanged(); }
    }

    private int _cantitateTotala;
    public int CantitateTotala
    {
        get => _cantitateTotala;
        set { _cantitateTotala = value; OnPropertyChanged(); }
    }

    private CategoriiPreparate _categorie;
    public CategoriiPreparate Categorie
    {
        get => _categorie;
        set { _categorie = value; OnPropertyChanged(); }
    }

    private string _pozaUrl;
    public string PozaUrl
    {
        get => _pozaUrl;
        set { _pozaUrl = value; OnPropertyChanged(); }
    }

    public ObservableCollection<CategoriiPreparate> AvailableCategories { get; } = new ObservableCollection<CategoriiPreparate>(
        Enum.GetValues(typeof(CategoriiPreparate)).Cast<CategoriiPreparate>()
    );

    public ObservableCollection<AllergenSelection> AvailableAllergens { get; } = new ObservableCollection<AllergenSelection>();

    #endregion

    #region Commands

    public ICommand SaveCommand { get; }
    public ICommand CancelCommand { get; }
    public ICommand BrowseImageCommand { get; }

    #endregion

    public event EventHandler<bool> CloseRequested;

    public AddPreparatViewModel(
        IPreparatService preparatService,
        IAlergenService alergenService,
        IDataRefreshService dataRefreshService)
    {
        _preparatService = preparatService;
        _alergenService = alergenService;
        _dataRefreshService = dataRefreshService;

        SaveCommand = new RelayCommand(async _ => await SavePreparatAsync(), _ => CanSave());
        CancelCommand = new RelayCommand(_ => CloseRequested?.Invoke(this, false));
        BrowseImageCommand = new RelayCommand(_ => BrowseForImage());

        Categorie = CategoriiPreparate.Aperitiv;

        LoadAllergensAsync().ConfigureAwait(false);
    }

    private async Task LoadAllergensAsync()
    {
        try
        {
            var allergens = await _alergenService.GetAllAlergeniAsync();

            foreach (var allergen in allergens)
            {
                AvailableAllergens.Add(new AllergenSelection
                {
                    Id = allergen.Id,
                    Nume = allergen.Nume,
                    TipAlergen = allergen.TipAlergen,
                    IsSelected = false
                });
            }
        }
        catch (Exception ex)
        {
            // Handle error (log or show message)
            System.Diagnostics.Debug.WriteLine($"Error loading allergens: {ex.Message}");
        }
    }

    private async Task SavePreparatAsync()
    {
        try
        {
            // Create DTO
            var preparatDto = new PreparatCreateDto
            {
                Nume = Nume,
                Pret = Pret,
                CantitatePortie = CantitatePortie,
                CantitateTotala = CantitateTotala,
                Categorie = Categorie,
                PozaUrl = PozaUrl,
                AlergenIds = AvailableAllergens
                    .Where(a => a.IsSelected)
                    .Select(a => a.Id)
                    .ToList()
            };

            await _preparatService.CreatePreparatAsync(preparatDto);

            _dataRefreshService.NotifyDataChanged();

            CloseRequested?.Invoke(this, true);
        }
        catch (Exception ex)
        {
            System.Windows.MessageBox.Show(
                $"Error saving preparat: {ex.Message}",
                "Save Error",
                System.Windows.MessageBoxButton.OK,
                System.Windows.MessageBoxImage.Error);
        }
    }

    private bool CanSave()
    {
        // Validate required fields
        return !string.IsNullOrWhiteSpace(Nume)
            && Pret > 0
            && CantitatePortie > 0
            && CantitateTotala >= CantitatePortie;
    }

    private void BrowseForImage()
    {
        var dialog = new OpenFileDialog
        {
            Filter = "Image files (*.png;*.jpeg;*.jpg;*.gif)|*.png;*.jpeg;*.jpg;*.gif|All files (*.*)|*.*",
            Title = "Select an image"
        };

        if (dialog.ShowDialog() == true)
        {
            PozaUrl = dialog.FileName;
        }
    }

    #region INotifyPropertyChanged

    public event PropertyChangedEventHandler PropertyChanged;

    protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    #endregion
}

public class AllergenSelection : INotifyPropertyChanged
{
    public int Id { get; set; }
    public string Nume { get; set; }
    public Alergeni TipAlergen { get; set; }

    private bool _isSelected;
    public bool IsSelected
    {
        get => _isSelected;
        set
        {
            _isSelected = value;
            OnPropertyChanged();
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}