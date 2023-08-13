using System.Data;
using MySql.Data.MySqlClient;

namespace Elite_db.operations;

public partial class InsertNewOrder
{
    private readonly MySqlConnection _con;
    private readonly string _userMail;
    public InsertNewOrder(string mainPageEmail, MySqlConnection mySqlConnection)
    {
        InitializeComponent();
        _con = mySqlConnection;
        _userMail = mainPageEmail;

        fillListView();
    }

    private void fillListView()
    {
        
        try {
            _con.Open();
            /*
             * Estrae dal DB solo i veicoli che non sono presenti in un ordine.
             */
            string selectQuery = "SELECT PRODUTTORE.NomeProduttore, VERSIONE.NomeModello, VERSIONE.Colore, " +
                                 "SUPERCAR.CavalliPotenza, SUPERCAR.Alimentazione, VERSIONE.Prezzo  " +
                                 "FROM VERSIONE, SUPERCAR, PRODUTTORE " +
                                 "WHERE VERSIONE.NomeModello = SUPERCAR.NomeModello " +
                                 "AND SUPERCAR.NomeProduttore = PRODUTTORE.NomeProduttore " +
                                 "AND VERSIONE.CodOrdine IS NULL";
            MySqlCommand cmd = new MySqlCommand(selectQuery, _con);
            
            var reader = cmd.ExecuteReader();
            while (reader.Read()) {
                var models = reader["Nome"] + " " + reader["Nome_Modello"] + " - " +
                             reader["Colore"] + " - " + reader["Cavalli_Potenza"] + "HP - " +
                             reader["Alimentazione"] + " - " + reader["Prezzo"] + "$";
            }
            reader.Close();
            // Models.ItemsSource = models;
        }
        catch (Exception exception) {
            Console.WriteLine(exception);
        }
        finally {
            _con.Close();
        }
    }


    private void MyPicker_SelectedIndexChanged(object sender, EventArgs e)
    {
        var picker = (Picker) sender;
        // estrai il valore selezionato

    }

    private void ConfirmBtnClicked(object sender, EventArgs e)
    {
        try {
            _con.Open();
            
            string insertQuery = "INSERT INTO ORDINE(Importo, DataOrdine, EmailAziendale, IDBadge) " +
                                 "VALUES (0, curdate(), @userMail, @costumerBadge)";
            MySqlCommand cmd = new MySqlCommand(insertQuery, _con);
            cmd.CommandType = CommandType.Text;
            cmd.Parameters.AddWithValue("@costumerBadge", EntryCostumerBadge.Text);
            cmd.Parameters.AddWithValue("@userMail", _userMail);

            if (cmd.ExecuteNonQuery() == 1) {
                ConfirmOrderBtn.Text = "Order inserted!";
                ConfirmOrderBtn.IsEnabled = false;
            }
        }
        catch {
            ConfirmOrderBtn.Text = "Error, retry!";
        }
        finally {
            _con.Close();   
        }
    }

    private void ModelPicker_SelectedIndexChanged(object sender, EventArgs e)
    {
        throw new NotImplementedException();
    }

    private void OnAddItemClicked(object sender, EventArgs e)
    {
        throw new NotImplementedException();
    }
}