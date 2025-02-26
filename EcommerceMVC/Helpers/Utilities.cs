using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace EcommerceMVC.Helpers
{
  public class Utilities
  {
    public static bool IsValidEmail(string email)
    {
      if (string.IsNullOrWhiteSpace(email))
        return false;

      try
      {
        // Normalize the domain
        email = Regex.Replace(email, @"(@)(.+)$", DomainMapper,
                              RegexOptions.None, TimeSpan.FromMilliseconds(200));

        // Examines the domain part of the email and normalizes it.
        string DomainMapper(Match match)
        {
          // Use IdnMapping class to convert Unicode domain names.
          var idn = new IdnMapping();

          // Pull out and process domain name (throws ArgumentException on invalid)
          string domainName = idn.GetAscii(match.Groups[2].Value);

          return match.Groups[1].Value + domainName;
        }
      }
      catch 
      {
        return false;
      }

      try
      {
        return Regex.IsMatch(email,
            @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
            RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250));
      }
      catch (RegexMatchTimeoutException)
      {
        return false;
      }
    }
    public static string UploadHinh(IFormFile Hinh, string folder)
    {
        try
        {
            var fullPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Hinh", folder, Hinh.FileName);
            using (var myfile = new FileStream(fullPath, FileMode.CreateNew))
            {
                Hinh.CopyTo(myfile);
            }
            return Hinh.FileName;
        }
        catch (Exception ex)
        {
            return string.Empty;
        }
    }
  }
}