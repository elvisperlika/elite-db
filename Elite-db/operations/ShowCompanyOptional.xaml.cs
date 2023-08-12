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
            var selectCompaniesQuery = "SELECT Nome_Produttore FROM PRODUTTORE";
            var cmdCompanies = new MySqlCommand(selectCompaniesQuery, _con);
            var readerCompanies = cmdCompanies.ExecuteReader();
            while (readerCompanies.Read()) {
                DropdownPicker.Items.Add(readerCompanies["Nome_Produttore"].ToString());
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

    private void SearchOptionalsClicked(object sender, EventArgs e)
    {
        try {
            _con.Open();
            
            var selectQuery =
                "SELECT Nome_Optional, Prezzo, Livello_Qualita " +
                "FROM OPTIONAL_AUTO, PRODUTTORE " +
                "WHERE PRODUTTORE.Nome_Produttore = @companyName " +
                "AND OPTIONAL_AUTO.P_IVA = PRODUTTORE.P_IVA";

            var cmd = new MySqlCommand(selectQuery, _con);
            cmd.CommandType = CommandType.Text;
            cmd.Parameters.AddWithValue("@companyName", _companyName);

            var dataList = new ObservableCollection<RowData>();
            var reader = cmd.ExecuteReader();
            while (reader.Read()) {
                dataList.Add(new RowData(reader["Nome_Optional"].ToString(),
                    reader["Prezzo"].ToString(), reader["Livello_Qualita"].ToString()));
            }
            DataListView.ItemsSource = dataList;
            reader.Close();
        }
        catch {
            SearchButton.Text = "Error, retry!";
        }
        finally {
            _con.Close();
            SearchButton.IsEnabled = false;
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
            SearchButton.IsEnabled = true;
        }
    }
}