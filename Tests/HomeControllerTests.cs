using BethanysPieShop.Controllers;
using BethanysPieShop.Models;
using BethanysPieShop.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace BethanysPieShop.Tests
{
    public class HomeControllerTests
    {
        [Fact]
        public void HomeController_Index_ShouldReturnViewResult()
        {
            // Arrange
            var mockPieRepository = new MockPieRepository();
            var controller = new HomeController(mockPieRepository);

            // Act
            var result = controller.Index();

            // Assert
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public void HomeController_Index_ShouldReturnViewWithHomeViewModel()
        {
            // Arrange
            var mockPieRepository = new MockPieRepository();
            var controller = new HomeController(mockPieRepository);

            // Act
            var result = controller.Index();
            var viewResult = result as ViewResult;

            // Assert
            Assert.NotNull(viewResult);
            Assert.IsType<HomeViewModel>(viewResult.Model);
        }

        [Fact]
        public void HomeController_Index_ShouldOnlyContainPiesOfTheWeek()
        {
            // Arrange
            var mockPieRepository = new MockPieRepository();
            var controller = new HomeController(mockPieRepository);

            // Act
            var result = controller.Index();
            var viewResult = result as ViewResult;
            var model = viewResult?.Model as HomeViewModel;

            // Assert
            Assert.NotNull(model);
            Assert.NotEmpty(model.PiesOfTheWeek);
            Assert.All(model.PiesOfTheWeek, pie => Assert.True(pie.IsPieOfTheWeek));
        }

        [Fact]
        public void HomeController_Index_PiesOfTheWeekShouldMatchRepository()
        {
            // Arrange
            var mockPieRepository = new MockPieRepository();
            var controller = new HomeController(mockPieRepository);
            var expectedCount = mockPieRepository.PiesOfTheWeek.Count();

            // Act
            var result = controller.Index();
            var viewResult = result as ViewResult;
            var model = viewResult?.Model as HomeViewModel;

            // Assert
            Assert.NotNull(model);
            Assert.Equal(expectedCount, model.PiesOfTheWeek.Count());
        }
    }
}
