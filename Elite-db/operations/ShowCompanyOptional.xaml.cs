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
                dataList.Add(new RowData(reader["NomeOptional"].ToString(),
                    reader["Prezzo"].ToString(), reader["LivelloQualita"].ToString()));
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
        public string OptionalName { get; }
        public string Price { get; }
        public string QualityLevel { get; }

        public RowData(string optionalName, string price, string qualityLevel)
        {
            OptionalName = optionalName;
            Price = price;
            QualityLevel = qualityLevel;
        }
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