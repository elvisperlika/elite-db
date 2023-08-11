using System.Collections.ObjectModel;
using System.Data;
using MySql.Data.MySqlClient;

namespace Elite_db.operations;

public partial class ShowCompanyOptional
{
    private readonly MySqlConnection _con; 
    public ShowCompanyOptional(MySqlConnection mySqlConnection)
    {
        InitializeComponent();
        _con = mySqlConnection;
    }

    private void SearchOptionalsClicked(object sender, EventArgs e)
    {
        var companyName = CompanyNameBox.Text;

        try {
            _con.Open();

            var selectQuery =
                "SELECT Nome_Optional, Prezzo, Livello_Qualita " +
                "FROM OPTIONAL_AUTO, PRODUTTORE " +
                "WHERE PRODUTTORE.Nome_Produttore = @companyName " +
                "AND OPTIONAL_AUTO.P_IVA = PRODUTTORE.P_IVA";

            var cmd = new MySqlCommand(selectQuery, _con);
            cmd.CommandType = CommandType.Text;
            cmd.Parameters.AddWithValue("@companyName", companyName);

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
}