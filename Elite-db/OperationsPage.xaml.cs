using Elite_db.operations;
using MySql.Data.MySqlClient;

namespace Elite_db;

public partial class OperationsPage : ContentPage
{
    public MainPage MainPage { get; set; }
    public OperationsPage(MainPage mainPage)
    {
        MainPage = mainPage;
        InitializeComponent();
    }

    private void InsertNewCustomerClicked(object sender, EventArgs e)
    {
        var insertNewCustomerPage = new InsertNewCostumer();
        ContentPresenter.Content = insertNewCustomerPage.Content;
    }

    private void ViewCustomerOrderClicked(object sender, EventArgs e)
    {
        var viewCustomersOrderPage = new ViewCustomerOrder();
        ContentPresenter.Content = viewCustomersOrderPage.Content;
    }

    private void ShowOptionalsCompanyClicked(object sender, EventArgs e)
    {
        var viewCustomersOrderPage = new ShowCompanyOptional();
        ContentPresenter.Content = viewCustomersOrderPage.Content;
    }

    private void InsertNewOrderClicked(object sender, EventArgs e)
    {
        var viewCustomersOrderPage = new InsertNewOrder(MainPage.Email);
        ContentPresenter.Content = viewCustomersOrderPage.Content;
    }

    private void InsertConsignmentClicked(object sender, EventArgs e)
    {
        throw new NotImplementedException();
    }

    private void ShowEmployeesWithBonusClicked(object sender, EventArgs e)
    {
        throw new NotImplementedException();
    }

    private void ShowTop10SupercarClicked(object sender, EventArgs e)
    {
        throw new NotImplementedException();
    }

    private void InsertNewSupercarVersionClicked(object sender, EventArgs e)
    {
        throw new NotImplementedException();
    }

    private void InsertMaintananceClicked(object sender, EventArgs e)
    {
        throw new NotImplementedException();
    }

    private void ShowOldestCostumersClicked(object sender, EventArgs e)
    {
        throw new NotImplementedException();
    }
}