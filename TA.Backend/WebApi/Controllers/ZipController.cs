using Microsoft.AspNetCore.Mvc;
using System.Collections.Immutable;
using System.IO.Compression;
using System.Text.Json;

namespace WebApi.Controllers
{
    public class ZipController : BaseController
    {
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Filter.Validate]
        public IActionResult Get()
        {
            string zip_folder_path = Path.Combine(Directory.GetCurrentDirectory(), "Upload\\Zips");
            if (!Directory.Exists(zip_folder_path))
            {
                Directory.CreateDirectory(zip_folder_path);
            }
            string[] zip_paths = Directory.GetFiles(zip_folder_path);
            string[] zips = new string[zip_paths.Length];
            for (int i = 0; i < zips.Length; i++)
            {
                zips[i] = Path.GetFileName(zip_paths[i]);
            }
            return Ok(JsonSerializer.Serialize(new { zips }));
        }

        private class Folder
        {
            public readonly string Name;
            public LinkedList<string> Files { get; set; }

            public Folder(string name)
            {
                this.Name = name;
                Files = new LinkedList<string>();
            }
        }

        private class Zip
        {
            public readonly string Name;
            public LinkedList<Folder> Folders { get; set; }
            public LinkedList<string> Files { get; set; }

            public Zip(string name) 
            {
                this.Name = name;
                Folders = new LinkedList<Folder>();
                Files = new LinkedList<string>();
            }
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Filter.Validate]
        public IActionResult Validate(IFormFile file)
        {
            using Stream stream = file.OpenReadStream();
            using ZipArchive archive = new(stream);

            Zip zip = new(file.FileName.Split('.')[0]);

            LinkedList<string> errors = new LinkedList<string>(); 

            const string string_dlls = "dlls";
            const string string_images = "images";
            const string string_languages = "languages";

            string[] img_allowed = new string[2]
            {
                    "jpg",
                    "png"
            };

            string[] languages_file_extension_allowed = new string[1]
            {
                "xml"
            };

            Folder dlls_folder = new(string_dlls);
            Folder images_folder = new(string_images);
            Folder languages_folder = new(string_languages);

            if (archive.Entries.Count == 0)
                errors.AddLast("Zip is empty");

            for (int i = 0; i < archive.Entries.Count; i++)
            {
                string full_name = archive.Entries[i].FullName;
                string[] separated = full_name.Split('/');
                if (separated.Length > 2)
                    errors.AddLast("Nesting level more than 2 not allowed");

                string[] file_name_splitted = separated[1].Split('.');
                string file_name = file_name_splitted[0];
                string extension = file_name_splitted[^1];

                if (separated[1] != "")
                {
                    switch (separated[0])
                    {
                        case string_dlls:
                            dlls_folder.Files.AddLast(file_name + "." + extension);
                            break;
                        case string_images:
                            if (!img_allowed.Contains(extension))
                                errors.AddLast("File " + separated[1] +" extension not allowed");
                            images_folder.Files.AddLast(file_name + "." + extension);
                            break;
                        case string_languages:
                            string[] splitted = file_name.Split("_");
                            if (!languages_file_extension_allowed.Contains(extension))
                                errors.AddLast("File "+ separated[1] + " extension not allowed");
                            else if (splitted[0] != zip.Name || splitted[1].Length != 2)
                                errors.AddLast("File " + file_name + " naming convension not allowed");
                            languages_folder.Files.AddLast(file_name + "." + extension);
                            break;
                        default:
                            errors.AddLast("Folder " + separated[0] +" naming convension not allowed");
                            break;
                    }
                }
            }

            if (!dlls_folder.Files.Contains(zip.Name + ".dll"))
                errors.AddLast("File " + zip.Name + ".dll not found");
            if (images_folder.Files.Count <= 0)
                errors.AddLast("No images found");

            if (errors.Count > 0)
                return BadRequest(JsonSerializer.Serialize(new { dlls = dlls_folder, images = images_folder, languages = languages_folder, errors }));

            return Ok(JsonSerializer.Serialize(new { dlls = dlls_folder, images = images_folder, languages = languages_folder }));
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Filter.Validate]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            string[] zip_name_splitted = file.FileName.Split('.');
            string extension = "." + zip_name_splitted[^1];
            string zip_name = DateTime.Now.Ticks.ToString() + extension;
            string currect_directory = Directory.GetCurrentDirectory();
            string zip_path = Path.Combine(currect_directory, "Upload\\Zips");

            if (!Directory.Exists(zip_path))
            {
                Directory.CreateDirectory(zip_path);
            }

            IActionResult validate_result = Validate(file);

            if (((Microsoft.AspNetCore.Mvc.ObjectResult)validate_result).StatusCode == 200)
            {
                string exact_path = Path.Combine(zip_path, zip_name);
                using var stream = new FileStream(exact_path, FileMode.Create, FileAccess.ReadWrite);
                await file.CopyToAsync(stream);
            }
            else
            {
                return validate_result;
            }

            return Ok(zip_name);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Filter.Validate]
        public async Task<IActionResult> Delete(string zip_name)
        {
            string file_path = Path.Combine(Directory.GetCurrentDirectory(), "Upload\\Zips", zip_name);
            if (!System.IO.File.Exists(file_path))
                return BadRequest(JsonSerializer.Serialize(new { errors = "File does not exists with the given name" }));
            await Task.Factory.StartNew(() =>
            {
                System.IO.File.Delete(file_path);
            });
            return Ok();
        }
    }
}
