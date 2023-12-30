using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Net.Http.Headers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews()
    .AddJsonOptions(configure => 
        configure.JsonSerializerOptions.PropertyNamingPolicy = null);

// create an HttpClient used for accessing the API
builder.Services.AddHttpClient("APIClient", client =>
{
    client.BaseAddress = new Uri(builder.Configuration["ImageGalleryAPIRoot"]);
    client.DefaultRequestHeaders.Clear();
    client.DefaultRequestHeaders.Add(HeaderNames.Accept, "application/json");
});

// Configure Authentication Middleware
// This will ensure that when a part of our application requires authentication, OpenID Connect will be triggered as default, as we set the DefaultChallengeScheme to OpenIDConnect as well.
builder.Services.AddAuthentication(options =>
{
    // You can choose this value, but it should correspond to the logical name for a particular authentication scheme. In our case, we're quite okay with value "Cookies"
    // Moreover, if you're hosting different applications on the same server, you'll want to ensure that these have a different scheme name in order to avoid them interfering with each other.
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    // This gives it OpenIDConnect as value, and this will have to match the scheme we use to configure the "OpenIDConnect", as we'll see soon.
    options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
})
    // This configures the cookie handler, and it enables our application to use cookie‑based authentication or our default scheme.
    // What this means is that once an identity token is validated and transformed into a claims identity, it will be stored in an encrypted cookie,
    // and that cookie is then used on subsequent requests to the web app to check whether or not we are making an authenticated request.
    // In other words, it's the cookie that is checked by our web app because we configured it like this.
    .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme)
    // This enables our application to support the OpenID Connect Authentication workflow. In our case, that will be the code flow.
    // In other words, it is this handler that will handle creating the authorization requests, token requests, and other requests, and it will ensure
    // that identity token validation happens.
    .AddOpenIdConnect(OpenIdConnectDefaults.AuthenticationScheme, options =>
    {
        // SignInScheme is set to Cookies, and that matches the default scheme name for authentication
        // This ensures that the successful result of authentication will be stored in the cookie matching this scheme.
        options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        // Hostname of the IDP
        options.Authority = "https://localhost:5001/";
        options.ClientId = "imagegalleryclient";
        options.ClientSecret = "secret";
        options.ResponseType = "code";
        // By default middleware consider this scopes
        //options.Scope.Add("openid");
        //options.Scope.Add("profile");

        // But, signin‑oidc is the default value used by this middleware, so, we don't have to set this manually either
        //options.CallbackPath = new PathString("signin-oidc");

        // This allows the middleware to save the tokens it receives from the identity provider, so, we can use them afterwards.
        options.SaveTokens = true;


    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler();
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// It's important that we add this to the request pipeline, as we want the request to be blocked for unauthenticated users.
// A great place to put this is between UseRouting, so, the middleware potentially has access to the route data, and MapControllerRoute,
// so, the middleware can effectively block access to the endpoint.
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Gallery}/{action=Index}/{id?}");

app.Run();
