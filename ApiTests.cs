using NUnit.Framework;
using RestSharp;
using Newtonsoft.Json.Linq;
using System.Reflection;

namespace SauceDemoTests;

public class ApiTests
{
    private RestClient _client = null!;

    [SetUp]
    public void SetUp()
    {
        _client = new RestClient("https://jsonplaceholder.typicode.com");
    }

    [TearDown]
    public void TearDown()
    {
        _client.Dispose();
    }

    [Test]
    public async Task GET_Posty_ZwracaListe()
    {
        var request = new RestRequest("/posts", Method.Get);

        var response = await _client.ExecuteAsync(request);

        Assert.That((int)response.StatusCode, Is.EqualTo(200));

        var posts = JArray.Parse(response.Content!);
        Assert.That(posts.Count, Is.GreaterThan(0));
    }

    [Test]
    public async Task GET_KonkretnyPost_ZwracaPoprawneId()
    {
        var request = new RestRequest("/posts/1", Method.Get);

        var response = await _client.ExecuteAsync(request);

        Assert.That((int)response.StatusCode, Is.EqualTo(200));

        var post = JObject.Parse(response.Content!);
        Assert.That(post["id"]!.Value<int>(), Is.EqualTo(1));
        Assert.That(post["title"], Is.Not.Null);
    }

    [Test]
    public async Task POST_NowyPost_Zwraca201()
    {
        var request = new RestRequest("/posts", Method.Post);
        request.AddJsonBody(new
        {
            title = "Test z SauceDemoTests",
            body = "Treść testowego posta",
            userId = 1
        });

        var response = await _client.ExecuteAsync(request);

        Assert.That((int)response.StatusCode, Is.EqualTo(201));

        var post = JObject.Parse(response.Content!);
        Assert.That(post["id"], Is.Not.Null);
        Assert.That(post["title"]!.Value<string>(), Is.EqualTo("Test z SauceDemoTests"));
    }

    [Test]
    public async Task PUT_AktualizacjaPostu_Zwraca200()
    {
        var request = new RestRequest("/posts/1", Method.Put);
        request.AddJsonBody(new
        {
            id = 1,
            title = "Zaktualizowany tytuł",
            body = "Zaktualizowana treść",
            userId = 1
        });

        var response = await _client.ExecuteAsync(request);

        Assert.That((int)response.StatusCode, Is.EqualTo(200));

        var post = JObject.Parse(response.Content!);
        Assert.That(post["title"]!.Value<string>(), Is.EqualTo("Zaktualizowany tytuł"));
    }

    [Test]
    public async Task DELETE_UsuniesciePostu_Zwraca200()
    {
        var request = new RestRequest("/posts/1", Method.Delete);

        var response = await _client.ExecuteAsync(request);

        Assert.That((int)response.StatusCode, Is.EqualTo(200));
    }

    [Test]
    public async Task GET_NieistniejacyPost_Zwraca404()
    {
        var request = new RestRequest("/posts/9999", Method.Get);

        var response = await _client.ExecuteAsync(request);

        Assert.That((int)response.StatusCode, Is.EqualTo(404));
    }

    [Test]
    public async Task GET_User1_ZwracaPoprawneDaneIEmail()
    {
        var request = new RestRequest("/users/1", Method.Get);
        var response = await _client.ExecuteAsync(request);

        Assert.That((int)response.StatusCode, Is.EqualTo(200));
        
        var post = JObject.Parse(response.Content!);
        Assert.That(post["id"]!.Value<int>(), Is.EqualTo(1));
        Assert.That(post["name"], Is.Not.Null);
        var email = post["email"]!.Value<string>();
        Assert.That(email, Does.Contain("@"));
        Assert.That(post["username"], Is.Not.Null);
        
    }
}