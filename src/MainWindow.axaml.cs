using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Interactivity;

namespace HXE;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    private Settings? _settings = null;
    private Positions? _positions = null;

    public Settings Settings
    {
        get
        {
            if (_settings is null)
            {
                QueryExistingWindows();
                if (_settings is null)
                    _settings = new();
            }
            return _settings;
        }
        set => _settings = value;
    }
    public Positions Positions
    {
        get
        {
            if (_positions is null)
            {
                QueryExistingWindows();
                if (_positions is null)
                    _positions = new();
            }
            return _positions;
        }
        set => _positions = value;
    }

    public void OpenSettings(object sender, RoutedEventArgs args)
    {
        Settings.Show(this);
    }

    public void OpenPositions(object sender, RoutedEventArgs args)
    {
        Positions.Show(this);
    }

    private void QueryExistingWindows()
    {
        if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime lifetime)
        {
            foreach (var w in lifetime.Windows)
            {
                if (_settings is null && w is Settings settings)
                    _settings = settings;
                else if (_positions is null && w is Positions positions)
                    _positions = positions;
            }
        }
    }
}
