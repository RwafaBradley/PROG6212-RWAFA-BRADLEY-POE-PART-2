using System;
using Xunit;
using Moq;
using CMCS.Controllers;
using CMCS.Services;
using CMCS.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.IO;

namespace CMCSUnitTests.Tests;

public class ClaimControllerTests
{
    [Fact]
    public void MyClaims_Returns_View_With_Model()
    {
        // arrange
        var mockRepo = new Mock<IClaimRepository>();
        mockRepo.Setup(r => r.GetByLecturer("lect"))
                .Returns(new List<Claim> { new Claim { Lecturer = "lect" } });

        var mockEnv = new Mock<IWebHostEnvironment>();
        var controller = new ClaimController(mockRepo.Object, mockEnv.Object);

       
        var claims = new[] { new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Name, "lect") };
        var identity = new System.Security.Claims.ClaimsIdentity(claims, "TestAuth");
        var principal = new System.Security.Claims.ClaimsPrincipal(identity);

        controller.ControllerContext = new Microsoft.AspNetCore.Mvc.ControllerContext
        {
            HttpContext = new Microsoft.AspNetCore.Http.DefaultHttpContext
            {
                User = principal
            }
        };

        
        var result = controller.MyClaims();

       
        Assert.IsType<ViewResult>(result);
    }

    [Fact]
    public void Download_Returns_NotFound_When_NoClaimOrFile()
    {
        var mockRepo = new Mock<IClaimRepository>();
        mockRepo.Setup(r => r.Get(It.IsAny<Guid>())).Returns((Claim)null);
        var mockEnv = new Mock<IWebHostEnvironment>();
        var controller = new ClaimController(mockRepo.Object, mockEnv.Object);

        var res = controller.Download(Guid.NewGuid());
        Assert.IsType<NotFoundResult>(res);
    }

    [Fact]
    public void Download_Returns_PhysicalFile_When_FileExists()
    {
        var tempRoot = Path.Combine(Path.GetTempPath(), "cmcs_test_wwwroot");
        Directory.CreateDirectory(Path.Combine(tempRoot, "uploads"));

        var filename = "testfile.txt";
        var filePath = Path.Combine(tempRoot, "uploads", filename);
        File.WriteAllText(filePath, "dummy");

        var mockRepo = new Mock<IClaimRepository>();
        var claim = new Claim { UploadedFileName = filename };
        mockRepo.Setup(r => r.Get(It.IsAny<Guid>())).Returns(claim);

        var mockEnv = new Mock<IWebHostEnvironment>();
        mockEnv.Setup(e => e.WebRootPath).Returns(tempRoot);

        var controller = new ClaimController(mockRepo.Object, mockEnv.Object);

        var res = controller.Download(Guid.NewGuid());
        Assert.IsType<PhysicalFileResult>(res);

      
        File.Delete(filePath);
        Directory.Delete(Path.Combine(tempRoot, "uploads"));
        Directory.Delete(tempRoot);
    }
}