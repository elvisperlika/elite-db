using System.Data;
using MySql.Data.MySqlClient;

namespace Elite_db.operations;

public partial class InsertNewOrder
{
    private readonly MySqlConnection _con;
    private readonly string _userMail;
    private string NrTelaio { get; set; }

    public InsertNewOrder(string mainPageEmail, MySqlConnection mySqlConnection)
    {
        InitializeComponent();
        _con = mySqlConnection;
        _userMail = mainPageEmail;

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
    
    private void ModelPicker_SelectedIndexChanged(object sender, EventArgs e)
    {
        var selectedItem = ModelPicker.SelectedItem?.ToString();
        if (!string.IsNullOrEmpty(selectedItem)) {
            try {
                _con.Open();
                string selectQuery = "SELECT NomeVersione " +
                                     "FROM VERSIONE " +
                                     "WHERE VERSIONE.NomeModello = @modelName " +
                                     "AND VERSIONE.CodOrdine IS NULL";
                
                MySqlCommand cmd = new MySqlCommand(selectQuery, _con);
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@modelName", selectedItem);
                
                var reader = cmd.ExecuteReader();
                while (reader.Read()) {
                    var brand = reader["NomeVersione"].ToString();
                    VersionPicker.Items.Add(brand);
                }
                reader.Close();
            }
            catch (Exception ex) {
                Console.WriteLine("Error while loading versions");
                throw;
            }
            finally {
                _con.Close();
            }   
        }
    }

    private void VersionPicker_SelectedIndexChanged(object sender, EventArgs e)
    {
        var versionSelected = VersionPicker.SelectedItem?.ToString();
        if (!string.IsNullOrEmpty(versionSelected)) {
            try {
                _con.Open();
                var selectNrtelaio = "SELECT NrTelaio " +
                                     "FROM VERSIONE " +
                                     "WHERE VERSIONE.NomeVersione = @versionName";
                MySqlCommand cmd = new MySqlCommand(selectNrtelaio, _con);
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@versionName", versionSelected);
                var reader = cmd.ExecuteReader();
                if (reader.Read()) {
                    var version = reader["NrTelaio"].ToString();
                    NrTelaio = version;
                }
                reader.Close();

            }
            catch (Exception exception) {
                Console.WriteLine(exception);
                throw;
            }
            finally {
                ConfirmOrderBtn.IsEnabled = true;
                _con.Close();
            }
        }
    }

    private void ConfirmBtnClicked(object sender, EventArgs e)
    {
        /*
         * Check if all the pickers have a value.
         */
        var brand = BrandPicker.SelectedItem?.ToString();
        if (!string.IsNullOrEmpty(brand)) {
            var model = ModelPicker.SelectedItem?.ToString();
            if (!string.IsNullOrEmpty(model)) {
                var version = VersionPicker.SelectedItem?.ToString();
                if (!string.IsNullOrEmpty(version)) {
                    if (CheckCostumerBadge()) {
                        CreateOrder();   
                    }
                }
            }
        }
        ConfirmOrderBtn.IsEnabled = false;
    }

    private bool CheckCostumerBadge()
    {
        try {
            _con.Open();
            Console.WriteLine(EntryCostumerBadge.Text);
            
            var selectCostumers = "SELECT * " +
                                  "FROM CLIENTE " +
                                  "WHERE CLIENTE.IDBadge = @idBadge " +
                                  "AND (curdate() - CLIENTE.DataScadenza) < 365";
            var cmd = new MySqlCommand(selectCostumers, _con);
            cmd.CommandType = CommandType.Text;
            cmd.Parameters.AddWithValue("@idBadge", EntryCostumerBadge.Text);
            // check if the costumer exists
            using (MySqlDataAdapter sda = new MySqlDataAdapter(cmd)) {
                using (DataTable dt = new DataTable()) {
                    sda.Fill(dt);
                    if (dt.Rows.Count == 0) {
                        ConfirmOrderBtn.Text = "Costumer not found, retry!";
                        return false;
                    }
                    else {
                        _con.Close();
                        return true;
                    }
                }
            }

        }
        catch (Exception e) {
            Console.WriteLine("error while checking costumer badge");
        }
        finally {
            _con.Close();
        }

        return false;
    }

    private void CreateOrder()
    {
        try {
            _con.Open();

            var insertOrderQuery = "INSERT INTO ORDINE(Importo, DataOrdine, EmailAziendale, IDBadge) " +
                                   "VALUES (0 , current_date(), @userMail, @idBadge)";
            var cmd = new MySqlCommand(insertOrderQuery, _con);
            cmd.CommandType = CommandType.Text;
            cmd.Parameters.AddWithValue("@userMail", _userMail);
            cmd.Parameters.AddWithValue("@idBadge", EntryCostumerBadge.Text);
            /*
             * Run the query.
             */
            if (cmd.ExecuteNonQuery() == 1) {
                var updateVersionQuery = "UPDATE VERSIONE " +
                                         "SET VERSIONE.CodOrdine = (SELECT ORDINE.CodOrdine " +
                                                                     "FROM ORDINE " +
                                                                     "ORDER BY ORDINE.CodOrdine DESC LIMIT 1)" +
                                         "WHERE VERSIONE.NrTelaio = @nrTelaio";
                var cmdUpdate = new MySqlCommand(updateVersionQuery, _con);
                cmdUpdate.CommandType = CommandType.Text;
                cmdUpdate.Parameters.AddWithValue("@nrTelaio", NrTelaio);

                var updateAmount = "UPDATE ORDINE " +
                                      "SET ORDINE.Importo = (SELECT SUM(VERSIONE.Prezzo) " +
                                      "FROM VERSIONE " +
                                      "WHERE VERSIONE.CodOrdine = ORDINE.CodOrdine)" +
                                      "ORDER BY ORDINE.CodOrdine DESC " +
                                      "LIMIT 1;";
                var cmdUpdateAmount = new MySqlCommand(updateAmount, _con);
                cmdUpdateAmount.CommandType = CommandType.Text;
                if (cmdUpdate.ExecuteNonQuery() == 1 && cmdUpdateAmount.ExecuteNonQuery() == 1) {
                    ConfirmOrderBtn.Text = "Order created successfully, click -Insert New Order- to create another one";
                }
            }
            
        }
        catch (Exception e) {
            Console.WriteLine("error while creating order");
            throw;
        }
        finally {
            _con.Close();
        }
    }
}