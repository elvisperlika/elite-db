using System.Collections.ObjectModel;
using System.Data;
using MySql.Data.MySqlClient;

namespace Elite_db.operations;

public partial class ViewCustomerOrder
{
    private readonly MySqlConnection _con;

    public ViewCustomerOrder(MySqlConnection mySqlConnection)
    {
        InitializeComponent();
        _con = mySqlConnection;
    }

    private void SearchOrdersClicked(object sender, EventArgs e)
    {
        try {
            _con.Open();

            var selectQeury =
                "SELECT ORDINE.CodOrdine, PRODUTTORE.NomeProduttore, SUPERCAR.NomeModello, VERSIONE.NomeVersione " +
                "FROM ORDINE, VERSIONE, SUPERCAR, PRODUTTORE " +
                "where ORDINE.IDBadge = @costumerID " +
                "and ORDINE.CodOrdine = VERSIONE.CodOrdine " +
                "and VERSIONE.NomeModello = SUPERCAR.NomeModello " +
                "and SUPERCAR.NomeProduttore = PRODUTTORE.NomeProduttore " +
                "and ORDINE.DataOrdine BETWEEN @startDate AND @endDate " +
                "order by ORDINE.DataOrdine DESC";

            var cmd = new MySqlCommand(selectQeury, _con);
            cmd.CommandType = CommandType.Text;
            cmd.Parameters.AddWithValue("@costumerID", CustomerBadgeBox.Text);
            cmd.Parameters.AddWithValue("@startDate", StartDatePicker.Date);
            cmd.Parameters.AddWithValue("@endDate", EndDatePicker.Date);

            var dataList = new ObservableCollection<RowData>();
            var reader = cmd.ExecuteReader();
            while (reader.Read()) {
                var row = new RowData {
                    CodOrder = reader["CodOrdine"].ToString(),
                    Company = reader["NomeProduttore"].ToString(),
                    Model = reader["NomeModello"].ToString(),
                    Version = reader["NomeVersione"].ToString()
                };
                dataList.Add(row);   
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
        public string CodOrder { get; set; }
        public string Company { get; set; }
        public string Model { get; set; }
        public string Version { get; set; }
        
    }
}