using System.Data;
using MySql.Data.MySqlClient;

namespace Elite_db.operations;

public partial class InsertNewSuperCarVersion : ContentPage
{
    private readonly MySqlConnection _con;
    public InsertNewSuperCarVersion(MySqlConnection mySqlConnection)
    {
        InitializeComponent();
        _con = mySqlConnection;
        FillBrandPicker();
    }
    
    private void FillBrandPicker()
    {
        try {
            _con.Open();
            var selectQuery = "SELECT NomeProduttore " +
                              "FROM PRODUTTORE";
            var cmd = new MySqlCommand(selectQuery, _con);
            var reader = cmd.ExecuteReader();
            while (reader.Read()) {
                var brand = reader["NomeProduttore"].ToString();
                BrandPicker.Items.Add(brand);
            }
            reader.Close();
        }
        catch (Exception ex) {
            Console.WriteLine("Error while loading brands");
            throw;
        }
        finally {
            _con.Close();
        }
    }

    private void BrandPicker_SelectedIndexChanged(object sender, EventArgs e)
    {
        var selectedItem = BrandPicker.SelectedItem?.ToString();
        if (!string.IsNullOrEmpty(selectedItem)) {
            try {
                _con.Open();
                string selectQuery = "SELECT NomeModello " +
                                     "FROM SUPERCAR " +
                                     "WHERE SUPERCAR.NomeProduttore = @companyName";
                
                MySqlCommand cmd = new MySqlCommand(selectQuery, _con);
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@companyName", selectedItem);
                
                var reader = cmd.ExecuteReader();
                while (reader.Read()) {
                    var brand = reader["NomeModello"].ToString();
                    ModelPicker.Items.Add(brand);
                }
                reader.Close();
            }
            catch (Exception ex) {
                Console.WriteLine("Error while loading models");
                throw;
            }
            finally {
                _con.Close();
            }   
        }
    }


    private void AddNewVersion(object sender, EventArgs e)
    {
        try {
            _con.Open();
            var brand = BrandPicker.SelectedItem?.ToString();
            var model = ModelPicker.SelectedItem?.ToString();
            
            var insertQuery =
                "INSERT INTO VERSIONE(NomeVersione, NrTelaio, Colore, Prezzo, CodOrdine, NomeProduttore, NomeModello)" +
                "VALUES(@versionName, @chassisNr, @color, @price, null, @brand, @model);";
            var cmd = new MySqlCommand(insertQuery, _con);
            cmd.CommandType = CommandType.Text;
            cmd.Parameters.AddWithValue("@versionName", VersionNameEntry.Text);
            cmd.Parameters.AddWithValue("@chassisNr", ChassisEntry.Text);
            cmd.Parameters.AddWithValue("@color", ColorEntry.Text);
            cmd.Parameters.AddWithValue("@price", PriceEntry.Text);
            cmd.Parameters.AddWithValue("@brand", brand);
            cmd.Parameters.AddWithValue("@model", model);
            if (cmd.ExecuteNonQuery() == 1) {
                AddVersionBtn.Text = "Version added";
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