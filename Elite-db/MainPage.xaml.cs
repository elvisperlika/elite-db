using System.Data;
using MySql.Data.MySqlClient;

namespace Elite_db;

public partial class MainPage
{
    public MySqlConnection Con { get; }
    public string Email { get; set; }

    public MainPage()
    {
        InitializeComponent();
        Con = new("SERVER=localhost; DATABASE=ElegantMotors; " +
                  "UID=root; PASSWORD=Elvis101");
    }

    private async void OnLogInClicked(object sender, EventArgs e)
    {
        var emailTmp = EmailBox.Text;
        
        Con.Open();
        string query = "SELECT * FROM DIPENDENTE WHERE Email_Aziendale = @email";
        MySqlCommand cmd = new MySqlCommand(query, Con);
        cmd.CommandType = CommandType.Text;
        cmd.Parameters.AddWithValue("@email", emailTmp);

        using (MySqlDataAdapter sda = new MySqlDataAdapter(cmd)) {
            using (DataTable dt = new DataTable()) {
                sda.Fill(dt);
                if (dt.Rows.Count > 0) {
                    Email = emailTmp;
                    await Navigation.PushAsync(new OperationsPage(this));
                }
                else {
                    LogInBtn.Text = "Email not found, retry!";
                }
            }
        }
        Con.Close();
    }
}