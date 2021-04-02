using System;
using Catalog.Api.Repository;
using Catalog.Api;
using Moq;
using Xunit;
using Catalog.Api.Models;
using Microsoft.Extensions.Logging;
using Catalog.Api.Controllers;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Catalog.Api.Dtos;
using System.Diagnostics;
using FluentAssertions;
using System.Collections.Generic;
using System.Linq;

namespace Catalog.Tests
{
    public class ItemsControllerTests
    {
        private readonly Mock<IItemsRepository> repositoryStub = new Mock<IItemsRepository>();
        private readonly Mock<ILogger<ItemsController>> loggerStub = new Mock<ILogger<ItemsController>>();

        [Fact]
        public async Task GetItemsAsync_ReturnsItems()
        {
            // Arrange
            var expectedItems = new[] {
                CreateItem(Guid.NewGuid()),
                CreateItem(Guid.NewGuid())
            };

            repositoryStub.Setup(repo => repo.GetItemsAsync())
                .ReturnsAsync(expectedItems);

            var controller = new ItemsController(loggerStub.Object, repositoryStub.Object);

            // Act
            var result = await controller.GetItemsAsync();

            // Assert

            // Normal approach but repetitive. Here I a only cheching the name property.
            var resultArray = result.ToArray();
            Assert.Equal(expectedItems[0].Name, resultArray[0].Name);
            Assert.Equal(expectedItems[1].Name, resultArray[1].Name);

            // Using fluent assertions nuget package to check all properties.
            result.Should().BeEquivalentTo(expectedItems);
        }

        [Fact]
        public async Task GetItemAsync_WhenItemDoesNotExit_ReturnsNotFound()
        {
            // Arrange
            repositoryStub.Setup(repo => repo.GetItemAsync(It.IsAny<Guid>()))
                .ReturnsAsync((Item)null);

            var controller = new ItemsController(loggerStub.Object, repositoryStub.Object);


            // Act
            var result = await controller.GetItemAsync(Guid.NewGuid());

            // Assert

            // Normal appraoch
            Assert.IsType<NotFoundResult>(result.Result);

            // Using fluent assertions nuget package
            result.Result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task GetItemAsync_WhenItemExits_ReturnsExpectedItem()
        {
            // Arrange
            var id = Guid.NewGuid();
            var expectedItem = CreateItem(id);

            repositoryStub.Setup(repo => repo.GetItemAsync(It.IsAny<Guid>()))
                .ReturnsAsync(expectedItem);

            var loggerStub = new Mock<ILogger<ItemsController>>();
            var controller = new ItemsController(loggerStub.Object, repositoryStub.Object);

            // Art
            var result = await controller.GetItemAsync(id);

            // Assert

            // Normal approach but repetitive
            Assert.IsType<ItemDto>(result.Value);
            var dto = (result as ActionResult<ItemDto>).Value;
            Assert.Equal(expectedItem.Id, dto.Id);
            Assert.Equal(expectedItem.Name, dto.Name);

            // Using fluent assertions nuget package
            result.Value.Should().BeEquivalentTo(expectedItem);
        }

        [Fact]
        public async Task CreateItemAsync_ReturnsCreatedAction()
        {
            // Arrange
            CreateItemDto expectedItem = new CreateItemDto
            (
                "Test item",
                "Description for test item",
                new Random().Next(1, 1000)
            );

            // This setup is not necessary for the unit test and may actually be a bad practice since in unit testing,
            // all we are concerned about is input and output, not implementation details.
            repositoryStub.Setup(repo => repo.CreateItemAsync(It.IsAny<Item>()));

            var controller = new ItemsController(loggerStub.Object, repositoryStub.Object);

            // Art
            var result = await controller.CreateItemAsync(expectedItem);

            // Assert
            Assert.IsType<ActionResult<ItemDto>>(result);
            var createdItem = (result.Result as CreatedAtActionResult).Value as ItemDto;
            Assert.Equal(expectedItem.Name, createdItem.Name);
            Assert.Equal(expectedItem.Description, createdItem.Description);
            Assert.Equal(expectedItem.Price, createdItem.Price);

            // using Fluent assertions
            expectedItem.Should().BeEquivalentTo(
                createdItem,
                options => options.ComparingByMembers<ItemDto>()
                    .ExcludingMissingMembers()
            );
            createdItem.Id.Should().NotBeEmpty();
            // The time the item is created should be within the time frame we run and
            // execute this unit test, in this case 1 second.
            createdItem.CreatedDate.Should().BeCloseTo(DateTimeOffset.UtcNow, 1000);
        }

        /// <summary>
        /// I am not mocking the UpdateItemAsync(item) because that is an implementation
        /// detail that is not necessary to perform unit testing
        /// </summary>
        [Fact]
        public async Task UpdateItemAsync_UpdateItem_ReturnsNoContent()
        {
            // Arrange
            var id = Guid.NewGuid();
            var oldItem = CreateItem(id);
            var updatedItem = new UpdateItemDto(
                "Test item",
                "Description for test item",
                new Random().Next(1, 1000)
            );

            repositoryStub.Setup(repo => repo.GetItemAsync(It.IsAny<Guid>()))
                .ReturnsAsync(oldItem);
            
            var controller = new ItemsController(loggerStub.Object, repositoryStub.Object);

            // Art
            var result = await controller.UpdateItemAsync(id, updatedItem);

            // Assert
            Assert.IsType<NoContentResult>(result);
            
            // Using fluent assertions
            result.Should().BeOfType<NoContentResult>();
        }

        /// <summary>
        /// I am not mocking the RemoveItemAsync(id) because that is an implementation
        /// detail that is not necessary to perform unit testing
        /// </summary>
        [Fact]
        public async Task RemoveItemAsync_RemoveItem_ReturnsNoContent()
        {
            // Arrange
            var id = Guid.NewGuid();
            var itemToRemove = CreateItem(id);

            repositoryStub.Setup(repo => repo.GetItemAsync(It.IsAny<Guid>()))
                .ReturnsAsync(itemToRemove);

            var controller = new ItemsController(loggerStub.Object, repositoryStub.Object);

            // Art
            var result = await controller.RemoveItemAsync(id);

            // Assert
            Assert.IsType<NoContentResult>(result);

            // Using fluent assertions
            result.Should().BeOfType<NoContentResult>();
        }

        private static Item CreateItem(Guid id)
        {
            return new Item
            {
                Id = id,
                Name = "Test item",
                Price = new Random().Next(1, 1000),
                CreatedDate = DateTimeOffset.UtcNow
            };
        }
    }
}
