using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace Elite_db.operations;

public partial class InsertNewOrder : ContentPage
{
    private readonly MySqlConnection _con;
    private readonly string _userMail;
    public InsertNewOrder(string mainPageEmail)
    {
        InitializeComponent();
        _con = new("SERVER=localhost; DATABASE=ElegantMotors; " +
                                  "UID=root; PASSWORD=Elvis101");
        _userMail = mainPageEmail;
    }

    private void AddPickerClicked(object sender, EventArgs e)
    {
        Picker myPicker = new Picker
        {
            Title = "Click to Chose", // Titolo visualizzato quando il Picker è chiuso
            VerticalOptions = LayoutOptions.CenterAndExpand // Opzioni di layout
        };

        try {
            _con.Open();
            string insertQuery = "SELECT Nome_Modello FROM VERSIONE";
            MySqlCommand cmd = new MySqlCommand(insertQuery, _con);
            
            var reader = cmd.ExecuteReader();
            while (reader.Read()) {
                myPicker.Items.Add(reader["Nome_Modello"].ToString());
            }
            reader.Close();
            
        }
        catch (Exception exception) {
            Console.WriteLine(exception);
        }
        finally {
            _con.Close();
        }

        StackLayout.Children.Add(myPicker);
    }

    private void ConfirmBtnClicked(object sender, EventArgs e)
    {
        var costumerBadge = EntryCostumerBadge.Text;
        try {
            _con.Open();
            
            string insertQuery = "INSERT INTO ORDINE(DATA, ORA, EMAIL_AZIENDALE, ID_BADGE) " +
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