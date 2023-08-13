using System.Collections.ObjectModel;
using System.Data;
using MySql.Data.MySqlClient;

namespace Elite_db.operations;

public partial class ShowEmployeesWithBonus
{
    private readonly MySqlConnection _con;
    public ShowEmployeesWithBonus(MySqlConnection mySqlConnection)
    {
        InitializeComponent();
        _con = mySqlConnection;
    }

    private void ShowEmplyeesClicked(object sender, EventArgs e)
    {
        int year = int.Parse(EntryYear.Text);
        int month = int.Parse(EntryMonth.Text);

        try {
            _con.Open();

            var selectQuery = "SELECT DIPENDENTE.Nome, DIPENDENTE.Cognome, DIPENDENTE.EmailAziendale " +
                                      "FROM DIPENDENTE, STIPENDIO " +
                                      "WHERE STIPENDIO.EmailAziendale = DIPENDENTE.EmailAziendale " +
                                      "AND STIPENDIO.Mese = @month " +
                                      "AND STIPENDIO.Anno = @year " +
                                      "AND STIPENDIO.Bonus is not null";

            var cmd = new MySqlCommand(selectQuery, _con);
            cmd.CommandType = CommandType.Text;
            cmd.Parameters.AddWithValue("@year", year);
            cmd.Parameters.AddWithValue("@month", month);

            var dataList = new ObservableCollection<RowData>();

            var reader = cmd.ExecuteReader();
            while (reader.Read()) {
                dataList.Add(new RowData(reader["Nome"].ToString(),
                    reader["Cognome"].ToString(), reader["Email_Aziendale"].ToString()));
            }

            DataListView.ItemsSource = dataList;
            reader.Close();
        }
        catch {
            ShowBtn.Text = "Error, retry!";
        }
        finally {
            _con.Close();
        }
    }
    
    public class RowData
    {
        public string Name { get; }
        public string Surname { get; }
        public string Email { get; }

        public RowData(string name, string surname, string email)
        {
            Name = name;
            Surname = surname;
            Email = email;
        }
    }
    
}