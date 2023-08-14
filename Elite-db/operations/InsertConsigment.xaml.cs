using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace Elite_db.operations;

public partial class InsertConsigment : ContentPage
{
    private MySqlConnection _con;
    public InsertConsigment(MySqlConnection mySqlConnection)
    {
        InitializeComponent();
        _con = mySqlConnection;
        LoadCostumers();
        LoadNrChassis();
    }

    private void LoadNrChassis()
    {
        try {
            _con.Open();
            var selectQuery = "SELECT NrTelaio " +
                              "FROM VERSIONE " +
                              "WHERE VERSIONE.CodOrdine IS NOT NULL";
            var cmd = new MySqlCommand(selectQuery, _con);
            var reader = cmd.ExecuteReader();
            while (reader.Read()) {
                var chassis = reader["NrTelaio"].ToString();
                NrChassisPicker.Items.Add(chassis);
            }
        }
        catch (Exception e) {
            Console.WriteLine("errror while loading chassis nr");
            throw;
        }
        finally {
            _con.Close();
        }
    }

    private void LoadCostumers()
    {
        try {
            _con.Open();
            var selectQuery = "SELECT IDBadge FROM CLIENTE ";
            var cmd = new MySqlCommand(selectQuery, _con);
            var reader = cmd.ExecuteReader();
            while (reader.Read()) {
                var chassis = reader["IDBadge"].ToString();
                CostumerPicker.Items.Add(chassis);
            }
        }
        catch (Exception e) {
            Console.WriteLine("errror while loading costumers id");
            throw;
        }
        finally {
            _con.Close();
        }
    }

    private void AddConsigmentBtnClicked(object sender, EventArgs e)
    {
        var chassisSelected = NrChassisPicker.SelectedItem?.ToString();
        var costumerSelected = CostumerPicker.SelectedItem?.ToString();
        if (!string.IsNullOrEmpty(chassisSelected) && !string.IsNullOrEmpty(costumerSelected))
        {
            try {
                _con.Open();
                var insertQuery =
                    "INSERT INTO CONTO_VENDITA(LivMotore, LivCarrozzeria, LivInterni, NrTelaio, Prezzo, Commissione, IDBadge) " +
                                "VALUES (@engineStatus, @bodyWorkStatus, @insideStatus, @NrChassis, @price, @fee, @idBadge);";
                var cmd = new MySqlCommand(insertQuery, _con);
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@engineStatus", EngineStatusEntry.Text);
                cmd.Parameters.AddWithValue("@bodyWorkStatus", BodyworkStatusEntry.Text);
                cmd.Parameters.AddWithValue("@insideStatus", InsideStatusEntry.Text);
                cmd.Parameters.AddWithValue("@NrChassis", chassisSelected);
                cmd.Parameters.AddWithValue("@price", PriceEntry.Text);
                cmd.Parameters.AddWithValue("@fee", FeeEntry.Text);
                cmd.Parameters.AddWithValue("@idBadge", costumerSelected);
                if (cmd.ExecuteNonQuery() == 1) {
                    UpdateVersion(chassisSelected);
                }
            }
            catch (Exception exception) {
                Console.WriteLine("error while inserting consigment");
                throw;
            }
            finally {
                _con.Close();
            }   
        }
    }

    private void UpdateVersion(string chassisSelected)
    {
        var updateQuery = "UPDATE VERSIONE " +
                          "SET VERSIONE.CodOrdine = null, " +
                          "VERSIONE.Prezzo = (SELECT CONTO_VENDITA.Prezzo " +
                                                      "FROM CONTO_VENDITA " +
                                                      "WHERE CONTO_VENDITA.NrTelaio = VERSIONE.NrTelaio) " +
                          "WHERE VERSIONE.NrTelaio = @NrChassis ";
        var cmd = new MySqlCommand(updateQuery, _con);
        cmd.CommandType = CommandType.Text;
        cmd.Parameters.AddWithValue("@NrChassis", chassisSelected);
        if (cmd.ExecuteNonQuery() == 1) {
            AddBtn.Text = "Consigment added";
            AddBtn.IsEnabled = false;
        }
    }
}