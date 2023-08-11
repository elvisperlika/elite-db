using System.Collections.ObjectModel;
using System.Data;
using MySql.Data.MySqlClient;

namespace Elite_db.operations;

public partial class ViewCustomerOrder
{
    public ViewCustomerOrder(MySqlConnection mySqlConnection)
    {
        InitializeComponent();
    }

    private void SearchOrdersClicked(object sender, EventArgs e)
    {
        var costumerID = CustomerBadgeBox.Text;
        var startDate = StartDatePicker.Date;
        var endDate = EndDatePicker.Date;

        MySqlConnection con = new("SERVER=localhost; DATABASE=ElegantMotors; " +
                                  "UID=root; PASSWORD=Elvis101");
        try {
            con.Open();

            var insertQuery =
                "SELECT Cod_Ordine, Data_Ordine, Ora, Email_Aziendale " +
                "FROM ORDINE " +
                "WHERE ID_Badge = @costumerID " +
                "AND Data_Ordine BETWEEN @startDate AND @endDate " +
                "ORDER BY Data_Ordine DESC";
            
            var cmd = new MySqlCommand(insertQuery, con);
            cmd.CommandType = CommandType.Text;
            cmd.Parameters.AddWithValue("@costumerID", costumerID);
            cmd.Parameters.AddWithValue("@startDate", startDate);
            cmd.Parameters.AddWithValue("@endDate", endDate);
            
            var dataList = new ObservableCollection<RowData>();

            var reader = cmd.ExecuteReader();
            while (reader.Read())
                dataList.Add(new RowData(reader["Cod_Ordine"].ToString(), reader["Data"].ToString(),
                    reader["Ora"].ToString(), reader["Email_Aziendale"].ToString()));

            dataListView.ItemsSource = dataList;
            reader.Close();
        }
        catch {
            SearchButton.Text = "Error, retry!";
        }
        finally {
            con.Close();
        }
    }

    public class RowData
    {
        public RowData(string codOrdine, string data, string ora, string emailAziendale)
        {
            Cod_Ordine = codOrdine;
            Data = data;
            Ora = ora;
            Email_Aziendale = emailAziendale;
        }

        public string Cod_Ordine { get; }
        public string Data { get; }
        public string Ora { get; }
        public string Email_Aziendale { get; }
    }
}