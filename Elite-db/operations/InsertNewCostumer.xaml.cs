using System.Data;
using MySql.Data.MySqlClient;

namespace Elite_db;

public partial class InsertNewCostumer : ContentPage
{
    private string _name;
    private string _surname;
    private string _cf;
    private string _cellular;
    private string _email;
    public InsertNewCostumer()
    {
        InitializeComponent();
    }

    private void InsertClicked(object sender, EventArgs e)
    {
        _name = Name.Text;
        _surname = Surname.Text;
        _cf = CF.Text;
        _cellular = Cellular.Text;
        _email = Email.Text;

        ConfirmBtn.IsEnabled = true;
        
        Namelabel.Text = _name;
        Surnamelabel.Text = _surname;
        Cflabel.Text = _cf;
        Cellularelabel.Text = _cellular;
        EmailLabel.Text = _email;
    }

    private void ConfirmClicked(object sender, EventArgs e)
    {
        MySqlConnection con = new("SERVER=localhost; DATABASE=ElegantMotors; " +
                                  "UID=root; PASSWORD=Elvis101");
        try {
            con.Open();
            string insertQuery =
                "INSERT INTO CLIENTE(Data_Scadenza, Nome, Cognome, CF, Cellulare_Personale, Mail_Personale) " +
                "VALUES (date_add(curdate(), INTERVAL 1 YEAR), @name, @surname, @cf, @cellular, @email)";
            MySqlCommand cmd = new MySqlCommand(insertQuery, con);
            cmd.CommandType = CommandType.Text;
            cmd.Parameters.AddWithValue("@name", _name);
            cmd.Parameters.AddWithValue("@surname", _surname);
            cmd.Parameters.AddWithValue("@cf", _cf);
            cmd.Parameters.AddWithValue("@cellular", _cellular);
            cmd.Parameters.AddWithValue("@email", _email);

            if (cmd.ExecuteNonQuery() == 1) {
                ConfirmBtn.Text = "Customer inserted!";
                ConfirmBtn.IsEnabled = false;
            }
        }
        catch {
            ConfirmBtn.Text = "Error, retry!";
        }
        finally {
            con.Close();   
        }
    }
}