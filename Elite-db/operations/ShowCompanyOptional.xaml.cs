using System.Collections.ObjectModel;
using System.Data;
using MySql.Data.MySqlClient;

namespace Elite_db.operations;

public partial class ShowCompanyOptional
{
    private readonly MySqlConnection _con;
    private string _companyName;
    public ShowCompanyOptional(MySqlConnection mySqlConnection)
    {
        InitializeComponent();
        _con = mySqlConnection;
        
        try {
            _con.Open();
            var selectCompaniesQuery = "SELECT NomeProduttore " +
                                                "FROM PRODUTTORE";
            var cmdCompanies = new MySqlCommand(selectCompaniesQuery, _con);
            var readerCompanies = cmdCompanies.ExecuteReader();
            while (readerCompanies.Read()) {
                DropdownPicker.Items.Add(readerCompanies["NomeProduttore"].ToString());
            }
            readerCompanies.Close();
        }
        catch (Exception e) {
            Console.WriteLine(e);
            throw;
        }
        finally {
            _con.Close();
        }
    }

    private void ShowOpionals()
    {
        try {
            _con.Open();
            
            var selectQuery =
                "SELECT NomeOptional, Prezzo, LivelloQualita " +
                "FROM OPTIONAL_AUTO, PRODUTTORE " +
                "WHERE PRODUTTORE.NomeProduttore = @companyName " +
                "AND OPTIONAL_AUTO.NomeProduttore = PRODUTTORE.NomeProduttore";

            var cmd = new MySqlCommand(selectQuery, _con);
            cmd.CommandType = CommandType.Text;
            cmd.Parameters.AddWithValue("@companyName", _companyName);

            var dataList = new ObservableCollection<RowData>();
            var reader = cmd.ExecuteReader();
            while (reader.Read()) {
                var row = new RowData {
                    OptionalName = reader["NomeOptional"].ToString(),
                    Price = reader["Prezzo"].ToString(),
                    QualityLevel = reader["LivelloQualita"].ToString()
                };
                dataList.Add(row);
            }
            DataListView.ItemsSource = dataList;
            reader.Close();
        }
        catch (Exception e) {
            Console.WriteLine(e);
            throw;
        }
        finally {
            _con.Close();
        }
    }
    
    public class RowData
    {
        public string OptionalName { get; set; }
        public string Price { get; set; }
        public string QualityLevel { get; set; }
    }

    private void DropdownPicker_SelectedIndexChanged(object sender, EventArgs e)
    {
        var selectedItem = DropdownPicker.SelectedItem?.ToString();
        if (!string.IsNullOrEmpty(selectedItem)) {
            _companyName = selectedItem;
            ShowOpionals();
        }
    }
}