using System.Collections;
using System.Data;
using JetBrains.Annotations;
using MySql.Data.MySqlClient;

namespace Elite_db.operations;

public partial class NewOrder : ContentPage
{
    private readonly MySqlConnection _con;
    private VerticalStackLayout _verticalStackLayout;
    private Button _addOptionalBtn = new();
    private Button _confirmOrderBtn = new();
    private Picker _brandPicker = new();
    private Picker _modelPicker = new();
    private Picker _versionPicker = new();
    private string _Mail;
    private string _segment;
    private List<Tuple<string, string, string, List<int>>> _supercarList = new();
    private int _cartIndex = -1;
    private Picker _optionalPicker = new();

    public NewOrder(string mainPageUserMail, MySqlConnection mySqlConnection)
    {
        InitializeComponent();
        _con = mySqlConnection;
        _Mail = mainPageUserMail;
    }

    private void AddSupercarBtn_Clicked(object sender, EventArgs e)
    {
        _cartIndex++;
        // disable the previous button that add optional
        _addOptionalBtn.IsEnabled = false;
        // disable the previous button that confirm the order
        _confirmOrderBtn.IsEnabled = false;
        // create the elements
        _brandPicker = new Picker {
            AutomationId = "BrandPicker",
            Title = "Select the brand",
        };
        
        _modelPicker = new Picker {
            AutomationId = "ModelPicker",
            Title = "Select the model",
        };
        
        _versionPicker = new Picker {
            AutomationId = "VersionPicker",
            Title = "Select the version"
        };
        
        _addOptionalBtn = new Button {
            IsEnabled = true,
            AutomationId = "AddOptionalBtn",
            Text = "Add optional",
        };
        
        _confirmOrderBtn = new Button {
            IsEnabled = true,
            AutomationId = "ConfirmOrderBtn",
            Text = "Confirm order",
        };
            
        // add the elements to the horizontal stack layout
         HorizontalStackLayout horizontalStackLayout = new HorizontalStackLayout {
            AutomationId = "SupercarHorizontalStackLayout",
            HorizontalOptions = LayoutOptions.Center,
            Children = {
                _brandPicker, _modelPicker, _versionPicker, _addOptionalBtn
            }
        };
         
         _verticalStackLayout = new VerticalStackLayout {
             AutomationId = "SupercarVerticalStackLayout",
             HorizontalOptions = LayoutOptions.Center,
             Children = {
                 horizontalStackLayout, _confirmOrderBtn
             }
         };
        
        _addOptionalBtn.Clicked += OnAddOptionalBtnClicked;
        
        _confirmOrderBtn.Clicked += OnConfirmOrderBtnClicked;
        
        // add the horizontal stack layout to the main stack layout
        MainStackLayout.Children.Add(_verticalStackLayout);
        
        FillBrandPicker();
        _brandPicker.SelectedIndexChanged += BrandPicker_SelectedIndexChanged;
        _modelPicker.SelectedIndexChanged += ModelPicker_SelectedIndexChanged;
        _versionPicker.SelectedIndexChanged += VersionPicker_SelectedIndexChanged;
    }

    private void VersionPicker_SelectedIndexChanged(object sender, EventArgs e)
    {
        var selectedItem = _versionPicker.SelectedItem?.ToString();
        if (!string.IsNullOrEmpty(selectedItem)) {
            try {
                _con.Open();
                var selectQuery = "SELECT SUPERCAR.NomeSegemento " +
                                     "FROM SUPERCAR, VERSIONE " +
                                     "WHERE VERSIONE.NomeModello = SUPERCAR.NomeModello " +
                                     "AND VERSIONE.NomeVersione = @versionName";
                MySqlCommand cmd = new MySqlCommand(selectQuery, _con);
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@versionName", selectedItem);
                var reader = cmd.ExecuteReader();
                if (reader.Read()) {
                    var segment = reader["NomeSegemento"].ToString();
                    _segment = segment;
                }
                reader.Close();
                Console.WriteLine(_cartIndex);
                List<int> optionalList = new();
                
                _supercarList.Add(new Tuple<string, string, string, List<int>>(_brandPicker.SelectedItem?.ToString(),
                    _modelPicker.SelectedItem?.ToString(), _versionPicker.SelectedItem?.ToString(), optionalList));
            }
            catch (Exception exception) {
                Console.WriteLine("error while loading segment");
                throw;
            }
            finally {
                _con.Close();
            }
        }
    }

    private void ModelPicker_SelectedIndexChanged(object sender, EventArgs e)
    {
        var selectedItem = _modelPicker.SelectedItem?.ToString();
        _versionPicker.Items.Clear();
        if (!string.IsNullOrEmpty(selectedItem)) {
            try {
                _con.Open();
                var selectQuery = "SELECT NomeVersione " +
                                  "FROM VERSIONE " +
                                  "WHERE VERSIONE.NomeModello = @modelName " +
                                  "AND VERSIONE.CodOrdine IS NULL";
                
                MySqlCommand cmd = new MySqlCommand(selectQuery, _con);
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@modelName", selectedItem);
                
                var reader = cmd.ExecuteReader();
                while (reader.Read()) {
                    var version = reader["NomeVersione"].ToString();
                    _versionPicker.Items.Add(version);
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

    private void BrandPicker_SelectedIndexChanged(object sender, EventArgs e)
    {
        var selectedItem = _brandPicker.SelectedItem?.ToString();
        _modelPicker.Items.Clear();
        if (!string.IsNullOrEmpty(selectedItem)) {
            try {
                _con.Open();
                var selectQuery = "SELECT NomeModello " +
                                  "FROM SUPERCAR " +
                                  "WHERE SUPERCAR.NomeProduttore = @companyName";
                
                MySqlCommand cmd = new MySqlCommand(selectQuery, _con);
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@companyName", selectedItem);
                
                var reader = cmd.ExecuteReader();
                while (reader.Read()) {
                    var model = reader["NomeModello"].ToString();
                    _modelPicker.Items.Add(model);
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
                _brandPicker.Items.Add(brand);
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

    private void OnConfirmOrderBtnClicked(object sender, EventArgs e)
    {
        if (EntryCostumerBadge != null) {
            try {
                _con.Open();
                var insertQuery = "INSERT INTO ORDINE(Importo, DataOrdine, EmailAziendale, IDBadge) " +
                                  "VALUES (0, curdate(), @mail, @badgeId)";
                MySqlCommand cmd = new MySqlCommand(insertQuery, _con);
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@mail", _Mail);
                cmd.Parameters.AddWithValue("@badgeId", EntryCostumerBadge.Text);
                if (cmd.ExecuteNonQuery() == 1) {
                    // create the order
                    foreach (var tuple in _supercarList) {
                        Console.WriteLine(tuple.Item1 + " " + tuple.Item2 + " " + tuple.Item3);
                        var updateQuery = "UPDATE VERSIONE " +
                                          "SET CodOrdine = (SELECT CodOrdine " +
                                          "                 FROM ORDINE " +
                                          "                 ORDER BY CodOrdine DESC ) " +
                                          "WHERE VERSIONE.NomeVersione = @versionName " +
                                          "AND VERSIONE.NomeModello = @modelName " +
                                          "AND VERSIONE.NomeProduttore = @brandName";
                        MySqlCommand updateCmd = new MySqlCommand(updateQuery, _con);
                        updateCmd.CommandType = CommandType.Text;
                        updateCmd.Parameters.AddWithValue("@brandName", tuple.Item1);
                        updateCmd.Parameters.AddWithValue("@modelName", tuple.Item2);
                        updateCmd.Parameters.AddWithValue("@versionName", tuple.Item3);
                        if (updateCmd.ExecuteNonQuery() == 1) {
                            Console.WriteLine("Order created");
                        }
                    }
                }
            }
            catch (Exception exception) {
                Console.WriteLine("error while creating the order");
                throw;
            }
            finally {
                _con.Close();
                AddSupercarBtn.IsEnabled = false;
                _confirmOrderBtn.Text = "Order confirmed";
                _addOptionalBtn.IsEnabled = false;
                _confirmOrderBtn.IsEnabled = false;
            }
        }
    }

    private void OnAddOptionalBtnClicked(object sender, EventArgs e)
    {
        
        _optionalPicker = new Picker {
            AutomationId = "OptionalPicker",
            Title = "Select the optional",
        };
        _verticalStackLayout.Children.Add(_optionalPicker);
        FillOptionalPicker(_optionalPicker);
        _optionalPicker.SelectedIndexChanged += OptionalPicker_SelectedIndexChanged;
    }

    private void OptionalPicker_SelectedIndexChanged(object sender, EventArgs e)
    {
        var selectedItem = _optionalPicker.SelectedItem?.ToString();
        if (!string.IsNullOrEmpty(selectedItem)) {
            try {
                _con.Open();
                var selectQuery = "SELECT CodOptional " +
                                  "FROM OPTIONAL_AUTO " +
                                  "WHERE OPTIONAL_AUTO.NomeOptional = @optionalName";
                MySqlCommand cmd = new MySqlCommand(selectQuery, _con);
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@optionalName", selectedItem);
                var reader = cmd.ExecuteReader();
                if (reader.Read()) {
                    var optionalId = reader["CodOptional"].ToString();
                    _supercarList[_cartIndex].Item4.Add(int.Parse(optionalId));
                }
                reader.Close();
            }
            catch (Exception exception) {
                Console.WriteLine(exception);
                throw;
            }
            finally {
                _con.Close();
            }
        }
    }

    private void FillOptionalPicker(Picker optionalPicker)
    {
        try {
            _con.Open();
            var selctQuery = "SELECT OPTIONAL_AUTO.NomeOptional " +
                             "FROM OPTIONAL_AUTO, supporto " +
                             "WHERE OPTIONAL_AUTO.CodOptional = supporto.CodOptional " +
                             "AND supporto.NomeSegemento = @segmentName";
            MySqlCommand cmd = new MySqlCommand(selctQuery, _con);
            cmd.CommandType = CommandType.Text;
            cmd.Parameters.AddWithValue("@segmentName", _segment);
            var reader = cmd.ExecuteReader();
            while (reader.Read()) {
                var optional = reader["NomeOptional"].ToString();
                optionalPicker.Items.Add(optional);
            }
            reader.Close();
              
        }
        catch (Exception exception) {
            Console.WriteLine(exception);
            throw;
        }
        finally {
            _con.Close();
        }
    }
}