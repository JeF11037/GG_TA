using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApi.Controllers;

namespace Api.Tests
{
    public class ZipControllerTests
    {
        [Fact]
        public void ValidateCorrectZip()
        {
            // Arrange

            var controller = new ZipController();

            string currect_directory = Directory.GetCurrentDirectory();
            string file_path = Path.Combine(currect_directory, "..\\..\\..\\Common\\CatGame.zip");

            string file_name = Path.GetFileName(file_path);
            string name = file_name.Split('.')[0];

            using var stream = File.OpenRead(file_path);
            var file = new FormFile(stream, 0, stream.Length, name, file_name)
            {
                Headers = new HeaderDictionary(),
                ContentType = "application/zip"
            };

            // Act

            var action_result = controller.Validate(file);

            // Assert

            Assert.IsType<OkObjectResult>(action_result);
        }

        [Fact]
        public void ValidateEmptyZip()
        {
            // Arrange

            var controller = new ZipController();

            string currect_directory = Directory.GetCurrentDirectory();
            string file_path = Path.Combine(currect_directory, "..\\..\\..\\Common\\Empty.zip");

            string file_name = Path.GetFileName(file_path);
            string name = file_name.Split('.')[0];

            using var stream = File.OpenRead(file_path);
            var file = new FormFile(stream, 0, stream.Length, name, file_name)
            {
                Headers = new HeaderDictionary(),
                ContentType = "application/zip"
            };

            // Act

            var action_result = controller.Validate(file);

            // Assert

            Assert.IsType<BadRequestObjectResult>(action_result);
        }

        [Fact]
        public void ValidateMoreThanOneFileInDlls()
        {
            // Arrange

            var controller = new ZipController();

            string currect_directory = Directory.GetCurrentDirectory();
            string file_path = Path.Combine(currect_directory, "..\\..\\..\\Common\\MoreFilesInDlls.zip");

            string file_name = Path.GetFileName(file_path);
            string name = file_name.Split('.')[0];

            using var stream = File.OpenRead(file_path);
            var file = new FormFile(stream, 0, stream.Length, name, file_name)
            {
                Headers = new HeaderDictionary(),
                ContentType = "application/zip"
            };

            // Act

            var action_result = controller.Validate(file);

            // Assert

            Assert.IsType<OkObjectResult>(action_result);
        }

        [Fact]
        public void ValidateNoImages()
        {
            // Arrange

            var controller = new ZipController();

            string currect_directory = Directory.GetCurrentDirectory();
            string file_path = Path.Combine(currect_directory, "..\\..\\..\\Common\\NoImages.zip");

            string file_name = Path.GetFileName(file_path);
            string name = file_name.Split('.')[0];

            using var stream = File.OpenRead(file_path);
            var file = new FormFile(stream, 0, stream.Length, name, file_name)
            {
                Headers = new HeaderDictionary(),
                ContentType = "application/zip"
            };

            // Act

            var action_result = controller.Validate(file);

            // Assert

            Assert.IsType<BadRequestObjectResult>(action_result);
        }

        [Fact]
        public void ValidateNoRootDllFile()
        {
            // Arrange

            var controller = new ZipController();

            string currect_directory = Directory.GetCurrentDirectory();
            string file_path = Path.Combine(currect_directory, "..\\..\\..\\Common\\NoRootDllFile.zip");

            string file_name = Path.GetFileName(file_path);
            string name = file_name.Split('.')[0];

            using var stream = File.OpenRead(file_path);
            var file = new FormFile(stream, 0, stream.Length, name, file_name)
            {
                Headers = new HeaderDictionary(),
                ContentType = "application/zip"
            };

            // Act

            var action_result = controller.Validate(file);

            // Assert

            Assert.IsType<BadRequestObjectResult>(action_result);
        }

        [Fact]
        public void ValidateWrongFormatImage()
        {
            // Arrange

            var controller = new ZipController();

            string currect_directory = Directory.GetCurrentDirectory();
            string file_path = Path.Combine(currect_directory, "..\\..\\..\\Common\\WrongFormatImage.zip");

            string file_name = Path.GetFileName(file_path);
            string name = file_name.Split('.')[0];

            using var stream = File.OpenRead(file_path);
            var file = new FormFile(stream, 0, stream.Length, name, file_name)
            {
                Headers = new HeaderDictionary(),
                ContentType = "application/zip"
            };

            // Act

            var action_result = controller.Validate(file);

            // Assert

            Assert.IsType<BadRequestObjectResult>(action_result);
        }

        [Fact]
        public void ValidateWrongFormatLanguage()
        {
            // Arrange

            var controller = new ZipController();

            string currect_directory = Directory.GetCurrentDirectory();
            string file_path = Path.Combine(currect_directory, "..\\..\\..\\Common\\WrongFormatLanguage.zip");

            string file_name = Path.GetFileName(file_path);
            string name = file_name.Split('.')[0];

            using var stream = File.OpenRead(file_path);
            var file = new FormFile(stream, 0, stream.Length, name, file_name)
            {
                Headers = new HeaderDictionary(),
                ContentType = "application/zip"
            };

            // Act

            var action_result = controller.Validate(file);

            // Assert

            Assert.IsType<BadRequestObjectResult>(action_result);
        }

        [Fact]
        public void ValidateWrongFormatLanguageCode()
        {
            // Arrange

            var controller = new ZipController();

            string currect_directory = Directory.GetCurrentDirectory();
            string file_path = Path.Combine(currect_directory, "..\\..\\..\\Common\\WrongFormatLanguage2.zip");

            string file_name = Path.GetFileName(file_path);
            string name = file_name.Split('.')[0];

            using var stream = File.OpenRead(file_path);
            var file = new FormFile(stream, 0, stream.Length, name, file_name)
            {
                Headers = new HeaderDictionary(),
                ContentType = "application/zip"
            };

            // Act

            var action_result = controller.Validate(file);

            // Assert

            Assert.IsType<BadRequestObjectResult>(action_result);
        }
    }
}
