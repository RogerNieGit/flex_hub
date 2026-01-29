using System;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using MessageBox = System.Windows.MessageBox;

namespace ModernDesktopApp;

/// <summary>
/// Interaction logic for DatabaseWindow.xaml
/// </summary>
public partial class DatabaseWindow : Window
{
    private readonly DatabaseHelper _dbHelper;
    private const string DB_PATH = @"C:\Users\T917991\AppData\Roaming\com.flexdesk.app\flex_desk_db";

    public DatabaseWindow()
    {
        InitializeComponent();
        _dbHelper = new DatabaseHelper(DB_PATH);
        LoadDatabaseInfo();
    }

    private void LoadDatabaseInfo()
    {
        try
        {
            var dbInfo = _dbHelper.GetDatabaseInfo();
            
            DatabasePath.Text = $"Database: {DB_PATH}";
            
            if (dbInfo.Exists)
            {
                DatabaseStatus.Text = "Connected";
                DatabaseStatus.Foreground = System.Windows.Media.Brushes.Green;
                TableCount.Text = dbInfo.TableCount.ToString();
                DatabaseSize.Text = FormatFileSize(dbInfo.FileSize);
                
                // Load tables list
                TablesList.ItemsSource = dbInfo.Tables;
                
                if (dbInfo.Tables.Count > 0)
                {
                    StatusMessage.Text = $"Database loaded successfully. {dbInfo.TableCount} table(s) found.";
                }
                else
                {
                    StatusMessage.Text = "Database is empty - no tables found.";
                }
            }
            else
            {
                DatabaseStatus.Text = "Not Found";
                DatabaseStatus.Foreground = System.Windows.Media.Brushes.Red;
                TableCount.Text = "0";
                DatabaseSize.Text = "0 bytes";
                StatusMessage.Text = "Database file not found!";
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error loading database: {ex.Message}", "Error", 
                MessageBoxButton.OK, MessageBoxImage.Error);
            StatusMessage.Text = $"Error: {ex.Message}";
        }
    }

    private void RefreshButton_Click(object sender, RoutedEventArgs e)
    {
        LoadDatabaseInfo();
        ResultsDataGrid.ItemsSource = null;
        SelectedTableName.Text = string.Empty;
        SelectedTableInfo.Text = string.Empty;
        QueryTextBox.Text = string.Empty;
        ResultsInfo.Text = string.Empty;
    }

    private void TablesList_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (TablesList.SelectedItem is string tableName)
        {
            try
            {
                SelectedTableName.Text = tableName;
                
                var schema = _dbHelper.GetTableSchema(tableName);
                var rowCount = _dbHelper.GetRowCount(tableName);
                
                SelectedTableInfo.Text = $"{schema.Count} column(s), {rowCount} row(s)";
                
                // Set default query
                QueryTextBox.Text = $"SELECT * FROM {tableName} LIMIT 100";
                
                StatusMessage.Text = $"Selected table: {tableName}";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading table info: {ex.Message}", "Error", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }

    private void ExecuteQueryButton_Click(object sender, RoutedEventArgs e)
    {
        string query = QueryTextBox.Text.Trim();
        
        if (string.IsNullOrEmpty(query))
        {
            MessageBox.Show("Please enter a SQL query.", "Warning", 
                MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        try
        {
            var startTime = DateTime.Now;
            var results = _dbHelper.ExecuteQuery(query);
            var elapsed = (DateTime.Now - startTime).TotalMilliseconds;
            
            ResultsDataGrid.ItemsSource = results.DefaultView;
            ResultsInfo.Text = $"{results.Rows.Count} row(s) returned in {elapsed:F2}ms";
            StatusMessage.Text = "Query executed successfully.";
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Query error: {ex.Message}", "Error", 
                MessageBoxButton.OK, MessageBoxImage.Error);
            StatusMessage.Text = $"Error: {ex.Message}";
            ResultsInfo.Text = "Query failed.";
        }
    }

    private void ViewAllButton_Click(object sender, RoutedEventArgs e)
    {
        if (TablesList.SelectedItem is string tableName)
        {
            QueryTextBox.Text = $"SELECT * FROM {tableName} LIMIT 1000";
            ExecuteQueryButton_Click(sender, e);
        }
        else
        {
            MessageBox.Show("Please select a table first.", "Warning", 
                MessageBoxButton.OK, MessageBoxImage.Warning);
        }
    }

    private static string FormatFileSize(long bytes)
    {
        string[] sizes = { "bytes", "KB", "MB", "GB", "TB" };
        double len = bytes;
        int order = 0;
        
        while (len >= 1024 && order < sizes.Length - 1)
        {
            order++;
            len = len / 1024;
        }
        
        return $"{len:0.##} {sizes[order]}";
    }
}