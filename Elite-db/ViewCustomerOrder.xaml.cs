using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace Elite_db;

public partial class ViewCustomerOrder : ContentPage
{
    public ViewCustomerOrder()
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
            
            string insertQuery =
                "SELECT Cod_Ordine, Data, Ora, Email_Aziendale FROM ORDINE " +
                "WHERE ID_Badge = @costumerID AND " +
                "Data BETWEEN @startDate AND @endDate " +
                "ORDER BY Data";
                
             
            var cmd = new MySqlCommand(insertQuery, con);
            cmd.CommandType = CommandType.Text;
            
            cmd.Parameters.AddWithValue("@costumerID", costumerID);
            cmd.Parameters.AddWithValue("@startDate", startDate);
            cmd.Parameters.AddWithValue("@endDate", endDate);

            MySqlDataReader reader = cmd.ExecuteReader();
            
            OrderList.RowDefinitions.Add(new RowDefinition());
            int row = 1;
            while (reader.Read()) {
                OrderList.RowDefinitions.Add(new RowDefinition());
                OrderList.Add(new Label {Text = reader["Cod_Ordine"].ToString()}, 0, row);
                OrderList.Add(new Label {Text = reader["Data"].ToString()}, 1, row);
                OrderList.Add(new Label {Text = reader["Ora"].ToString()}, 2, row);
                OrderList.Add(new Label {Text = reader["Email_Aziendale"].ToString()}, 3, row);
                row++;
            }
            
            reader.Close();
            SearchButton.IsEnabled = false;
            SearchButton.Text = "Click on ->View Costumer order<- to search again!";    
        }
        catch {
            SearchButton.Text = "Error, retry!";
        }
        finally {
            con.Close();
        }
        
    }
}