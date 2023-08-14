using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace Elite_db.operations;

public partial class ShowHumanResourcesCost : ContentPage
{
    private readonly MySqlConnection _con;
    public ShowHumanResourcesCost(MySqlConnection mySqlConnection)
    {
        InitializeComponent();
        _con = mySqlConnection;
    }

    private void CalculateCost(object sender, EventArgs e)
    {
        try {
            _con.Open();
            var selectQuery = "SELECT SUM(STIPENDIO.Importo) + SUM(STIPENDIO.Bonus) AS 'Spesa Annuale' " +
                              "FROM STIPENDIO " +
                              "WHERE STIPENDIO.Anno = @year;";
            var cmd = new MySqlCommand(selectQuery, _con);
            cmd.Parameters.AddWithValue("@year", YearEntry.Text);
            var reader = cmd.ExecuteReader();
            while (reader.Read()) {
                var cost = reader["Spesa Annuale"].ToString();
                CostLabel.Text = cost;
            }
            
        }
        catch (Exception exception) {
            Console.WriteLine(exception);
            throw;
        }
        finally {
            _con.Close();
        }
    }
}