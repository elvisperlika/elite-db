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
            string selectQuery = "SELECT PRODUTTORE.Nome_Produttore, VERSIONE.Nome_Modello, VERSIONE.Colore, " +
                                 "SUPERCAR.Cavalli_Potenza, SUPERCAR.Alimentazione, VERSIONE.Prezzo  " +
                                 "FROM VERSIONE, SUPERCAR, PRODUTTORE " +
                                 "WHERE VERSIONE.Nome_Modello = SUPERCAR.Nome_Modello " +
                                 "AND SUPERCAR.P_IVA = PRODUTTORE.P_IVA " +
                                 "AND VERSIONE.Cod_Ordine IS NULL";
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
        var costumerBadge = EntryCostumerBadge.Text;
        
        try {
            _con.Open();
            
            string insertQuery = "INSERT INTO ORDINE(Data_Ordine, ORA, EMAIL_AZIENDALE, ID_BADGE) " +
                                 "VALUES (curdate(), current_time(), @userMail, @costumerBadge)";
            MySqlCommand cmd = new MySqlCommand(insertQuery, _con);
            cmd.CommandType = CommandType.Text;
            cmd.Parameters.AddWithValue("@costumerBadge", costumerBadge);
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
}