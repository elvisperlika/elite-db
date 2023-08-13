using System.Data;
using MySql.Data.MySqlClient;

namespace Elite_db;

public partial class MainPage
{
    public MySqlConnection Con { get; }
    public string UserMail { get; private set; }

    public MainPage()
    {
        InitializeComponent();
        Con = new MySqlConnection("SERVER=localhost; DATABASE=Elite; " +
                                  "UID=root; PASSWORD=Elvis101");
    }

    private async void OnLogInClicked(object sender, EventArgs e)
    {
        Con.Open();
        string query = "SELECT * " +
                       "FROM DIPENDENTE " +
                       "WHERE EmailAziendale = @email";
        MySqlCommand cmd = new MySqlCommand(query, Con);
        cmd.CommandType = CommandType.Text;
        cmd.Parameters.AddWithValue("@email", EmailBox.Text);

        using (MySqlDataAdapter sda = new MySqlDataAdapter(cmd)) {
            using (DataTable dt = new DataTable()) {
                sda.Fill(dt);
                if (dt.Rows.Count > 0) {
                    UserMail = EmailBox.Text;
                    await Navigation.PushAsync(new OperationsPage(this));
                }
                else {
                    LogInBtn.Text = "Email not found, retry!";
                }
            }
        }
        await Con.CloseAsync();
    }
}