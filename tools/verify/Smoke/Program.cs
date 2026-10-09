// Smoke test for tools/verify/verify.sh: inflates every page/popup (and its item templates)
// with sample data on net10.0, failing on XAML load exceptions and missing resources.
using System.Reflection;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;
using Tsundoku;
using Tsundoku.Models;
using Tsundoku.Repository;
using Tsundoku.ViewModels;
using Tsundoku.Views;

var failures = new List<string>();

Microsoft.Maui.Controls.Internals.Registrar.RegisterAll(Array.Empty<Type>());
DependencyService.Register<Microsoft.Maui.Controls.Internals.IFontNamedSizeService, FakeNamedSizes>();
var app = new App(null!, null!, null!);
Application.Current = app;
Console.WriteLine($"App resources: {app.Resources.MergedDictionaries.Sum(d => d.Count)} entries");

var repo = new FakeRepo();
var books = (await repo.GetAllBooksAsync()).ToList();

void Check(string name, Func<Element> make)
{
    try
    {
        var el = make();
        int templates = 0;
        foreach (var cv in Descendants(el).OfType<ItemsView>())
        {
            if (cv.ItemTemplate is DataTemplate t)
                foreach (var b in books)
                {
                    var content = (BindableObject)t.CreateContent();
                    content.BindingContext = b;
                    if (content is Element ce) ce.Parent = cv;
                    templates++;
                }
            if (cv.EmptyView is BindableObject ev) ev.BindingContext = cv.BindingContext;
        }
        Console.WriteLine($"OK   {name} (templates realised: {templates})");
    }
    catch (Exception ex) { failures.Add($"{name}: {ex.GetType().Name}: {ex.Message}"); Console.WriteLine($"FAIL {name}: {ex}"); }
}

static IEnumerable<Element> Descendants(Element e)
{
    yield return e;
    foreach (var c in ((IElementController)e).LogicalChildren)
        foreach (var d in Descendants(c)) yield return d;
    if (e is ContentPage cp && cp.Content is Element content && !((IElementController)e).LogicalChildren.Contains(content))
        foreach (var d in Descendants(content)) yield return d;
}

Check("AppShell", () => new AppShell());
var mainVm = new MainPageViewModel(repo);
await Task.Delay(100);
Check("MainPage", () => new MainPage(mainVm));
var certVm = new CertificatePageViewModel(repo);
await Task.Delay(100);
Check("CertificatePageView", () => new CertificatePageView(certVm));
Check("RegisterPageView", () => new RegisterPageView(new RegisterPageViewModel(repo, null!, null!)));
Check("ConfirmBookView", () => new ConfirmBookView(new ConfirmBookViewModel("4101010013", repo, null!, null!)));
Check("PayWallView", () => new PayWallView(new PayWallViewModel(null!, null!)));
Check("MainPage (empty)", () => new MainPage(new MainPageViewModel(new FakeRepo(empty: true))));

foreach (var p in typeof(MainPageViewModel).GetProperties(BindingFlags.Public | BindingFlags.Instance).Where(p => p.Name.Contains("String")))
    Console.WriteLine($"  {p.Name} = {p.GetValue(mainVm)}");

if (failures.Count > 0)
{
    Console.WriteLine("FAILURES:");
    foreach (var f in failures.Distinct()) Console.WriteLine("  " + f);
    return 1;
}
Console.WriteLine("SMOKE OK");
return 0;

class FakeRepo(bool empty = false) : IBookInfoRepository
{
    readonly List<Book> _books = empty ? [] : Enumerable.Range(1, 6).Select(i => new Book
    {
        Id = i,
        RegistrationDate = DateTime.Today.AddDays(-40 * i),
        ReadDate = DateTime.Today.AddDays(-10 * i),
        Isbn10 = "410101001" + (i % 10),
        Read = i % 2 == 0,
    }).ToList();
    public Task<IEnumerable<Book>> GetAllBooksAsync() => Task.FromResult<IEnumerable<Book>>(_books);
    public Task<int> GetAllBooksCountAsync() => Task.FromResult(_books.Count);
    public Task<int> AddBookInfoAsync(string isbn10, bool isRead) => Task.FromResult(1);
    public Task<int> DeleteBookInfoAsync(int id) => Task.FromResult(1);
    public Task<int> UpdateReadStatusAsync(int Id, DateTime readDateTime, bool read) => Task.FromResult(1);
}

class FakeNamedSizes : Microsoft.Maui.Controls.Internals.IFontNamedSizeService
{
    public double GetNamedSize(NamedSize size, Type targetElementType, bool useOldSizes) => 14;
}
