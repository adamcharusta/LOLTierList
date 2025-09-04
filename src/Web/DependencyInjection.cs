namespace LOLTierList.Web;

public static class DependencyInjection
{
    public static IServiceCollection AddWeb(this IServiceCollection s)
    {
        s.AddControllersWithViews();
        return s;
    }

    public static WebApplication UseWeb(this WebApplication a)
    {
        if (!a.Environment.IsDevelopment())
        {
            a.UseExceptionHandler("/Home/Error");
            a.UseHsts();
        }

        a.MapHealthChecks("/health");
        a.UseHttpsRedirection();
        a.UseStaticFiles();

        a.UseRouting();

        a.UseAuthorization();

        a.MapControllerRoute(
            "default",
            "{controller=Home}/{action=Index}/{id?}");

        return a;
    }
}
