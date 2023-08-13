using System.Data;
using MySql.Data.MySqlClient;

namespace Elite_db.operations;

public partial class InsertNewCostumer
{
    private readonly MySqlConnection _con;

    public InsertNewCostumer(MySqlConnection mySqlConnection)
    {
        InitializeComponent();
        _con = mySqlConnection;
    }

    private void InsertClicked(object sender, EventArgs e)
    {
        try {
            _con.Open();
            var insertQuery =
                "INSERT INTO CLIENTE(Nome, Cognome, CF, CellularePersonale, MailPersonale, DataScadenza) " +
                "VALUES (@name, @surname, @cf, @cellular, @email, date_add(curdate(), INTERVAL 1 YEAR))";
            var cmd = new MySqlCommand(insertQuery, _con);
            cmd.CommandType = CommandType.Text;
            cmd.Parameters.AddWithValue("@name", Name.Text);
            cmd.Parameters.AddWithValue("@surname", Surname.Text);
            cmd.Parameters.AddWithValue("@cf", CF.Text);
            cmd.Parameters.AddWithValue("@cellular", Cellular.Text);
            cmd.Parameters.AddWithValue("@email", Email.Text);

            if (cmd.ExecuteNonQuery() == 1) {
                InsertBtn.Text = "Done!";
            }
        }
        catch {
            InsertBtn.Text = "Error, retry!";
        }
        finally {
            _con.Close();
        }
        
    }

}