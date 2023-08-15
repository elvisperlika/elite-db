using Elite_db.operations;

namespace Elite_db;

public partial class OperationsPage
{
    private MainPage MainPage { get; }
    public OperationsPage(MainPage mainPage)
    {
        MainPage = mainPage;
        InitializeComponent();
    }

    private void InsertNewCustomerClicked(object sender, EventArgs e)
    {
        var insertNewCustomerPage = new InsertNewCostumer(MainPage.Con);
        ContentPresenter.Content = insertNewCustomerPage.Content;
    }

    private void ViewCustomerOrderClicked(object sender, EventArgs e)
    {
        var viewCustomersOrderPage = new ViewCustomerOrder(MainPage.Con);
        ContentPresenter.Content = viewCustomersOrderPage.Content;
    }

    private void ShowOptionalsCompanyClicked(object sender, EventArgs e)
    {
        var viewCustomersOrderPage = new ShowCompanyOptional(MainPage.Con);
        ContentPresenter.Content = viewCustomersOrderPage.Content;
    }

    private void InsertNewOrderClicked(object sender, EventArgs e)
    {
        //var viewCustomersOrderPage = new InsertNewOrder(MainPage.UserMail, MainPage.Con);
        //ContentPresenter.Content = viewCustomersOrderPage.Content;
        var viewCustomersOrderPage = new CreateNewOrder(MainPage.UserMail, MainPage.Con);
        ContentPresenter.Content = viewCustomersOrderPage.Content;
    }

    private void InsertConsignmentClicked(object sender, EventArgs e)
    {
        var viewCustomersOrderPage = new InsertConsigment(MainPage.Con);
        ContentPresenter.Content = viewCustomersOrderPage.Content;
    }

    private void ShowEmployeesWithBonusClicked(object sender, EventArgs e)
    {
        var viewCustomersOrderPage = new ShowEmployeesWithBonus(MainPage.Con);
        ContentPresenter.Content = viewCustomersOrderPage.Content;
    }

    private void ShowTop10SupercarClicked(object sender, EventArgs e)
    {
        var viewCustomersOrderPage = new ShowTopSupercar(MainPage.Con);
        ContentPresenter.Content = viewCustomersOrderPage.Content;
    }

    private void InsertNewSupercarVersionClicked(object sender, EventArgs e)
    {
        var viewCustomersOrderPage = new InsertNewSuperCarVersion(MainPage.Con);
        ContentPresenter.Content = viewCustomersOrderPage.Content;
    }

    private void ShowAmountHumanResourcesCost(object sender, EventArgs e)
    {
        var viewCustomersOrderPage = new ShowHumanResourcesCost(MainPage.Con);
        ContentPresenter.Content = viewCustomersOrderPage.Content;
    }
}