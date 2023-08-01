using System.Data;
using MySql.Data.MySqlClient;

namespace Elite_db;

public partial class MainPage
{
    public MainPage()
    {
        InitializeComponent();
    }

    private async void OnLogInClicked(object sender, EventArgs e)
    {
        var emailTmp = EmailBox.Text;

        MySqlConnection con = new("SERVER=localhost; DATABASE=ElegantMotors; " +
                                  "UID=root; PASSWORD=Elvis101");
        con.Open();
        string query = "SELECT * FROM DIPENDENTE WHERE Email_Aziendale = @email";
        MySqlCommand cmd = new MySqlCommand(query, con);
        cmd.CommandType = CommandType.Text;
        cmd.Parameters.AddWithValue("@email", emailTmp);

        using (MySqlDataAdapter sda = new MySqlDataAdapter(cmd)) {
            using (DataTable dt = new DataTable()) {
                sda.Fill(dt);
                if (dt.Rows.Count > 0) {
                    await Navigation.PushAsync(new OperationsPage());
                }
                else {
                    LogInBtn.Text = "Email not found, retry!";
                }
            }
        }
        con.Close();
    }
}