using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Agenda.Application.DTOs.Requests;
using Agenda.Application.DTOs.Responses;
using Agenda.Application.Services;
using Agenda.Domain.Entities;
using Agenda.Domain.Repositories;
using AutoMapper;
using FluentAssertions;
using Moq;
using Xunit;

namespace Agenda.Tests
{
    public class ContactServiceTests
    {
        private readonly Mock<IContactRepository> _repoMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly ContactService _service;

        public ContactServiceTests()
        {
            _repoMock = new Mock<IContactRepository>(MockBehavior.Strict);
            _mapperMock = new Mock<IMapper>(MockBehavior.Strict);
            _service = new ContactService(_repoMock.Object, _mapperMock.Object);
        }

        [Fact]
        public async Task CreateAsync_WhenEmailDoesNotExist_CreatesAndReturnsResponse()
        {
            // Arrange
            var request = new CreateContactRequest { Email = "test@email.com" };

            var mappedContact = new Contact
            {
                Email = request.Email
                // outros campos que o mapper normalmente preencheria
            };

            Contact? capturedToSave = null;

            var savedContact = new Contact
            {
                Id = Guid.NewGuid(),
                Email = request.Email,
                CreatedAt = DateTime.UtcNow
            };

            var response = new ContactResponse { Id = savedContact.Id };

            _repoMock.Setup(r => r.ExistsByEmailAsync(request.Email)).ReturnsAsync(false);
            _mapperMock.Setup(m => m.Map<Contact>(request)).Returns(mappedContact);

            _repoMock
                .Setup(r => r.AddAsync(It.IsAny<Contact>()))
                .Callback<Contact>(c => capturedToSave = c)
                .ReturnsAsync(savedContact);

            _mapperMock.Setup(m => m.Map<ContactResponse>(savedContact)).Returns(response);

            // Act
            var result = await _service.CreateAsync(request);

            // Assert
            result.Should().BeEquivalentTo(response);

            capturedToSave.Should().NotBeNull();
            capturedToSave!.Id.Should().NotBe(Guid.Empty);
            capturedToSave.CreatedAt.Should().NotBe(default);
            capturedToSave.Email.Should().Be(request.Email);

            _repoMock.Verify(r => r.ExistsByEmailAsync(request.Email), Times.Once);
            _repoMock.Verify(r => r.AddAsync(It.IsAny<Contact>()), Times.Once);
            _mapperMock.Verify(m => m.Map<Contact>(request), Times.Once);
            _mapperMock.Verify(m => m.Map<ContactResponse>(savedContact), Times.Once);
        }

        [Fact]
        public async Task CreateAsync_WhenEmailExists_ThrowsConflictException_AndDoesNotAdd()
        {
            // Arrange
            var request = new CreateContactRequest { Email = "existing@email.com" };

            _repoMock.Setup(r => r.ExistsByEmailAsync(request.Email)).ReturnsAsync(true);

            // Act
            Func<Task> act = () => _service.CreateAsync(request);

            // Assert
            await act.Should().ThrowAsync<ContactService.ConflictException>()
                .WithMessage("Email já existe.");

            _repoMock.Verify(r => r.ExistsByEmailAsync(request.Email), Times.Once);
            _repoMock.Verify(r => r.AddAsync(It.IsAny<Contact>()), Times.Never);
            _mapperMock.Verify(m => m.Map<Contact>(It.IsAny<CreateContactRequest>()), Times.Never);
        }

        [Fact]
        public async Task DeleteAsync_WhenContactExists_DeletesAndReturnsTrue()
        {
            // Arrange
            var id = Guid.NewGuid();
            _repoMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(new Contact { Id = id });
            _repoMock.Setup(r => r.DeleteAsync(id)).Returns(Task.CompletedTask);

            // Act
            var result = await _service.DeleteAsync(id);

            // Assert
            result.Should().BeTrue();
            _repoMock.Verify(r => r.GetByIdAsync(id), Times.Once);
            _repoMock.Verify(r => r.DeleteAsync(id), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_WhenContactDoesNotExist_ReturnsFalse_AndDoesNotDelete()
        {
            // Arrange
            var id = Guid.NewGuid();
            _repoMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync((Contact?)null);

            // Act
            var result = await _service.DeleteAsync(id);

            // Assert
            result.Should().BeFalse();
            _repoMock.Verify(r => r.GetByIdAsync(id), Times.Once);
            _repoMock.Verify(r => r.DeleteAsync(It.IsAny<Guid>()), Times.Never);
        }

        [Theory]
        [InlineData(null, 1, 10, 1, 10)]
        [InlineData("luan", 2, 20, 2, 20)]
        [InlineData("x", 0, 0, 1, 10)]
        [InlineData("x", -5, -1, 1, 10)]
        [InlineData("x", 1, 500, 1, 100)]
        public async Task GetAllAsync_ClampsPageAndPageSize_AndReturnsPagedResult(
            string? search,
            int page,
            int pageSize,
            int expectedPage,
            int expectedPageSize)
        {
            // Arrange
            var items = new List<Contact>
            {
                new Contact { Id = Guid.NewGuid() },
                new Contact { Id = Guid.NewGuid() }
            };

            var totalItems = 21; // for totalPages check
            _repoMock.Setup(r => r.GetPagedAsync(search, expectedPage, expectedPageSize))
                .ReturnsAsync((items, totalItems));

            // mapper mapping each Contact -> ContactResponse
            _mapperMock.Setup(m => m.Map<ContactResponse>(It.IsAny<Contact>()))
                .Returns<Contact>(c => new ContactResponse { Id = c.Id });

            // Act
            var result = await _service.GetAllAsync(search, page, pageSize);

            // Assert
            result.Page.Should().Be(expectedPage);
            result.PageSize.Should().Be(expectedPageSize);
            result.TotalItems.Should().Be(totalItems);

            var expectedTotalPages = (int)Math.Ceiling(totalItems / (double)expectedPageSize);
            result.Totalpages.Should().Be(expectedTotalPages);

            result.Items.Should().HaveCount(items.Count);
            result.Items.Select(x => x.Id).Should().BeEquivalentTo(items.Select(x => x.Id));

            _repoMock.Verify(r => r.GetPagedAsync(search, expectedPage, expectedPageSize), Times.Once);
            _mapperMock.Verify(m => m.Map<ContactResponse>(It.IsAny<Contact>()), Times.Exactly(items.Count));
        }

        [Fact]
        public async Task GetByIdAsync_WhenExists_ReturnsMappedResponse()
        {
            // Arrange
            var id = Guid.NewGuid();
            var contact = new Contact { Id = id };
            var response = new ContactResponse { Id = id };

            _repoMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(contact);
            _mapperMock.Setup(m => m.Map<ContactResponse>(contact)).Returns(response);

            // Act
            var result = await _service.GetByIdAsync(id);

            // Assert
            result.Should().BeEquivalentTo(response);
            _repoMock.Verify(r => r.GetByIdAsync(id), Times.Once);
            _mapperMock.Verify(m => m.Map<ContactResponse>(contact), Times.Once);
        }

        [Fact]
        public async Task GetByIdAsync_WhenNotExists_ReturnsNull()
        {
            // Arrange
            var id = Guid.NewGuid();
            _repoMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync((Contact?)null);

            // Act
            var result = await _service.GetByIdAsync(id);

            // Assert
            result.Should().BeNull();
            _repoMock.Verify(r => r.GetByIdAsync(id), Times.Once);
            _mapperMock.Verify(m => m.Map<ContactResponse>(It.IsAny<Contact>()), Times.Never);
        }

        [Fact]
        public async Task UpdateAsync_WhenContactNotFound_ReturnsNull()
        {
            // Arrange
            var id = Guid.NewGuid();
            var request = new UpdateContactRequest { Email = "new@email.com" };

            _repoMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync((Contact?)null);

            // Act
            var result = await _service.UpdateAsync(id, request);

            // Assert
            result.Should().BeNull();
            _repoMock.Verify(r => r.GetByIdAsync(id), Times.Once);
            _repoMock.Verify(r => r.UpdateAsync(It.IsAny<Contact>()), Times.Never);
        }

        [Fact]
        public async Task UpdateAsync_WhenEmailUnchanged_IgnoresExistsCheck_AndUpdates()
        {
            // Arrange
            var id = Guid.NewGuid();
            var request = new UpdateContactRequest { Email = "TEST@EMAIL.COM" };
            var contact = new Contact { Id = id, Email = "test@email.com" };

            _repoMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(contact);

            _mapperMock
                .Setup(m => m.Map(request, contact))
                .Returns(contact);

            _repoMock.Setup(r => r.UpdateAsync(It.Is<Contact>(c => c.UpdatedAt != default)))
                .ReturnsAsync(contact);

            _mapperMock.Setup(m => m.Map<ContactResponse>(contact))
                .Returns(new ContactResponse { Id = id });

            // Act
            var result = await _service.UpdateAsync(id, request);

            // Assert
            result.Should().NotBeNull();
            _repoMock.Verify(r => r.ExistsByEmailAsync(It.IsAny<string>()), Times.Never);
            _repoMock.Verify(r => r.UpdateAsync(It.IsAny<Contact>()), Times.Once);
            _mapperMock.Verify(m => m.Map(request, contact), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_WhenEmailChanged_AndNewEmailExists_ThrowsConflict()
        {
            // Arrange
            var id = Guid.NewGuid();
            var request = new UpdateContactRequest { Email = "new@email.com" };
            var contact = new Contact { Id = id, Email = "old@email.com" };

            _repoMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(contact);
            _repoMock.Setup(r => r.ExistsByEmailAsync(request.Email)).ReturnsAsync(true);

            // Act
            Func<Task> act = () => _service.UpdateAsync(id, request);

            // Assert
            await act.Should().ThrowAsync<ContactService.ConflictException>()
                .WithMessage("Email já existe.");

            _repoMock.Verify(r => r.UpdateAsync(It.IsAny<Contact>()), Times.Never);
            _mapperMock.Verify(m => m.Map(It.IsAny<UpdateContactRequest>(), It.IsAny<Contact>()), Times.Never);
        }

        [Fact]
        public async Task UpdateAsync_WhenEmailChanged_AndNewEmailDoesNotExist_Updates()
        {
            // Arrange
            var id = Guid.NewGuid();
            var request = new UpdateContactRequest { Email = "new@email.com" };
            var contact = new Contact { Id = id, Email = "old@email.com" };

            _repoMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(contact);
            _repoMock.Setup(r => r.ExistsByEmailAsync(request.Email)).ReturnsAsync(false);

            _mapperMock
                .Setup(m => m.Map(request, contact))
                .Returns(contact);

            _repoMock.Setup(r => r.UpdateAsync(It.Is<Contact>(c => c.UpdatedAt != default)))
                .ReturnsAsync(contact);

            _mapperMock.Setup(m => m.Map<ContactResponse>(contact))
                .Returns(new ContactResponse { Id = id });

            // Act
            var result = await _service.UpdateAsync(id, request);

            // Assert
            result.Should().NotBeNull();
            _repoMock.Verify(r => r.ExistsByEmailAsync(request.Email), Times.Once);
            _repoMock.Verify(r => r.UpdateAsync(It.IsAny<Contact>()), Times.Once);
            _mapperMock.Verify(m => m.Map(request, contact), Times.Once);
        }
    }
}
