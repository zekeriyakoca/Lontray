using Lontray.Services.Identity.API.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;

namespace Lontray.Services.Identity.API.Data
{
    public class ApplicationDbContextSeeder
    {

        public async Task SeedAsync(ApplicationDbContext context, IWebHostEnvironment env,
            ILogger logger, int? retry = 0)
        {
            try
            {
                if (!context.Users.Any())
                {
                    await context.Users.AddRangeAsync(DefaultUsers);
                    await context.SaveChangesAsync();
                }
                GetPreconfiguredImages(env, logger);

            }
            catch (Exception ex)
            {
                logger.LogError(ex, "EXCEPTION ERROR while migrating {DbContextName}", nameof(ApplicationDbContext));
                throw;
            }
        }

        private List<ApplicationUser> DefaultUsers => new List<ApplicationUser>() {

           new ApplicationUser()
            {
                CardHolderName = "DemoUser",
                CardNumber = "4012888888881881",
                CardType = 1,
                City = "Redmond",
                Country = "U.S.",
                Email = "demouser@microsoft.com",
                Expiration = "12/21",
                Id = Guid.NewGuid().ToString(),
                LastName = "DemoLastName",
                Name = "DemoUser",
                PhoneNumber = "1234567890",
                UserName = "demouser@microsoft.com",
                ZipCode = "98052",
                State = "WA",
                Street = "15703 NE 61st Ct",
                SecurityNumber = "535",
                NormalizedEmail = "DEMOUSER@MICROSOFT.COM",
                NormalizedUserName = "DEMOUSER@MICROSOFT.COM",
                SecurityStamp = Guid.NewGuid().ToString("D"),
                Password = "P@assCode"
            }
        };

        private void GetPreconfiguredImages(IWebHostEnvironment env, ILogger logger)
        {
            try
            {
                var zipFilePath = Path.Combine(env.ContentRootPath, "Static/SeedData", "images.zip");
                if (!File.Exists(zipFilePath))
                {
                    logger.LogError("Zip file '{ZipFileName}' does not exists.", zipFilePath);
                    return;
                }
                var pathToSaveImages = Path.Combine(env.WebRootPath, "images");
                string[] currentImages = Directory.GetFiles(pathToSaveImages).Select(file => Path.GetFileName(file)).ToArray();

                using (ZipArchive zip = ZipFile.Open(zipFilePath, ZipArchiveMode.Read))
                {
                    foreach (var image in zip.Entries)
                    {
                        if (currentImages.Contains(image.Name))
                        {
                            string destinationFilename = Path.Combine(pathToSaveImages, image.Name);
                            image.ExtractToFile(destinationFilename);
                        }
                        else
                        {
                            logger.LogWarning("Skipped file '{FileName}' in zipfile '{ZipFileName}'", image.Name, zipFilePath);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "EXCEPTION ERROR: {Message}", ex.Message);
            }


        }


    }
}
