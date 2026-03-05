using BethanysPieShop.Controllers;
using BethanysPieShop.Models;
using BethanysPieShop.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace BethanysPieShop.Tests
{
    public class PieControllerTests
    {
        [Fact]
        public void PieController_ShouldHaveParameterlessList()
        {
            // Arrange
            var mockPieRepository = new MockPieRepository();
            var mockCategoryRepository = new MockCategoryRepository();
            
            // Act
            var controller = new PieController(mockPieRepository, mockCategoryRepository);
            
            // Assert
            Assert.NotNull(controller);
        }
        
        [Fact]
        public void PieController_List_ShouldReturnViewResult()
        {
            // Arrange
            var mockPieRepository = new MockPieRepository();
            var mockCategoryRepository = new MockCategoryRepository();
            var controller = new PieController(mockPieRepository, mockCategoryRepository);
            
            // Act
            var result = controller.List();
            
            // Assert
            Assert.IsType<ViewResult>(result);
        }
        
        [Fact]
        public void PieController_List_ShouldReturnViewWithPieListViewModel()
        {
            // Arrange
            var mockPieRepository = new MockPieRepository();
            var mockCategoryRepository = new MockCategoryRepository();
            var controller = new PieController(mockPieRepository, mockCategoryRepository);
            
            // Act
            var result = controller.List();
            var viewResult = result as ViewResult;
            
            // Assert
            Assert.NotNull(viewResult);
            Assert.IsType<PieListViewModel>(viewResult.Model);
        }
        
        [Fact]
        public void PieController_List_ShouldContainAllPies()
        {
            // Arrange
            var mockPieRepository = new MockPieRepository();
            var mockCategoryRepository = new MockCategoryRepository();
            var controller = new PieController(mockPieRepository, mockCategoryRepository);
            
            // Act
            var result = controller.List();
            var viewResult = result as ViewResult;
            var model = viewResult?.Model as PieListViewModel;
            
            // Assert
            Assert.NotNull(model);
            Assert.NotEmpty(model.Pies);
            Assert.Equal("All Pies", model.CurrentCategory);
        }
        
        [Fact]
        public void PieController_Details_ShouldReturnViewResultWithPie()
        {
            // Arrange
            var mockPieRepository = new MockPieRepository();
            var mockCategoryRepository = new MockCategoryRepository();
            var controller = new PieController(mockPieRepository, mockCategoryRepository);
            var pieId = 1;
            
            // Act
            var result = controller.Details(pieId);
            var viewResult = result as ViewResult;
            
            // Assert
            Assert.NotNull(viewResult);
            Assert.IsType<Pie>(viewResult.Model);
            
            var pie = viewResult.Model as Pie;
            Assert.Equal(pieId, pie?.PieId);
        }
        
        [Fact]
        public void PieController_Details_ShouldReturnNotFoundForInvalidId()
        {
            // Arrange
            var mockPieRepository = new MockPieRepository();
            var mockCategoryRepository = new MockCategoryRepository();
            var controller = new PieController(mockPieRepository, mockCategoryRepository);
            var invalidPieId = 999;
            
            // Act
            var result = controller.Details(invalidPieId);
            
            // Assert
            Assert.IsType<NotFoundResult>(result);
        }
    }
}
