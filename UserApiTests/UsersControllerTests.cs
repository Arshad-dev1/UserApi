using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UserApi;
using UserApi.Controllers;
using UserApi.DB;
using UserApi.DB.Models;
using UserApi.Models;

namespace UserApiTests
{
    [TestClass]
    public sealed class UsersControllerTests
    {
        private UserDbContext _dbContext = null!;
        private UsersController _controller = null!;

        [TestInitialize]
        public void Setup()
        {
            // Create an in-memory database for testing
            var options = new DbContextOptionsBuilder<UserDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _dbContext = new UserDbContext(options);
            _controller = new UsersController(_dbContext);
        }

        [TestCleanup]
        public void Teardown()
        {
            _dbContext.Dispose();
        }

        #region GetAll Tests

        [TestMethod]
        [Description("Success: GetAll should return empty list when no users exist")]
        public async Task GetAll_NoUsers_ReturnsOkWithEmptyList()
        {
            // Arrange
            var ct = CancellationToken.None;

            // Act
            var result = await _controller.GetAll(ct);

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            var okResult = (OkObjectResult)result;
            Assert.AreEqual(200, okResult.StatusCode);
            var users = (List<UserResponse>)okResult.Value!;
            Assert.AreEqual(0, users.Count);
        }

        [TestMethod]
        [Description("Success: GetAll should return all users ordered by Id descending")]
        public async Task GetAll_WithUsers_ReturnsOkWithUsersList()
        {
            // Arrange
            var user1 = new User { Name = "John Doe", Age = 25, City = "New York", State = "NY", Pincode = "100001" };
            var user2 = new User { Name = "Jane Smith", Age = 30, City = "Los Angeles", State = "CA", Pincode = "900001" };

            _dbContext.Users.Add(user1);
            _dbContext.Users.Add(user2);
            await _dbContext.SaveChangesAsync();

            var ct = CancellationToken.None;

            // Act
            var result = await _controller.GetAll(ct);

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            var okResult = (OkObjectResult)result;
            Assert.AreEqual(200, okResult.StatusCode);
            var users = (List<UserResponse>)okResult.Value!;
            Assert.AreEqual(2, users.Count);
        }

        #endregion

        #region GetById Tests

        [TestMethod]
        [Description("Success: GetById should return a single user when ID exists")]
        public async Task GetById_ValidId_ReturnsOkWithUser()
        {
            // Arrange
            var user = new User { Name = "John Doe", Age = 25, City = "New York", State = "NY", Pincode = "100001" };
            _dbContext.Users.Add(user);
            await _dbContext.SaveChangesAsync();

            var ct = CancellationToken.None;

            // Act
            var result = await _controller.GetById(user.Id, ct);

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            var okResult = (OkObjectResult)result;
            Assert.AreEqual(200, okResult.StatusCode);
            var returnedUser = (UserResponse)okResult.Value!;
            Assert.AreEqual(user.Id, returnedUser.Id);
            Assert.AreEqual("John Doe", returnedUser.Name);
        }

        [TestMethod]
        [Description("Negative: GetById should return 404 when user does not exist")]
        public async Task GetById_NonExistentId_ReturnsNotFound()
        {
            // Arrange
            var ct = CancellationToken.None;

            // Act
            var result = await _controller.GetById(999, ct);

            // Assert
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
            var notFoundResult = (NotFoundResult)result;
            Assert.AreEqual(404, notFoundResult.StatusCode);
        }

        [TestMethod]
        [Description("Negative: GetById should return 400 when ID is zero or negative")]
        public async Task GetById_InvalidId_ReturnsBadRequest()
        {
            // Arrange
            var ct = CancellationToken.None;

            // Act
            var result = await _controller.GetById(0, ct);

            // Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
            var badResult = (BadRequestObjectResult)result;
            Assert.AreEqual(400, badResult.StatusCode);
            Assert.AreEqual("User ID must be greater than 0.", badResult.Value);
        }

        #endregion

        #region Create Tests

        [TestMethod]
        [Description("Success: Create should add a new user and return 201 Created")]
        public async Task Create_ValidRequest_ReturnsCreatedAtAction()
        {
            // Arrange
            var request = new UserRequest { Name = "John Doe", Age = 25, City = "New York", State = "NY", Pincode = "100001" };
            var ct = CancellationToken.None;

            // Act
            var result = await _controller.Create(request, ct);

            // Assert
            Assert.IsInstanceOfType(result, typeof(CreatedAtActionResult));
            var createdResult = (CreatedAtActionResult)result;
            Assert.AreEqual(201, createdResult.StatusCode);
            Assert.AreEqual(nameof(UsersController.GetById), createdResult.ActionName);

            var returnedUser = (UserResponse)createdResult.Value!;
            Assert.AreEqual("John Doe", returnedUser.Name);
            Assert.AreEqual(25, returnedUser.Age);

            // Verify user was actually saved to database
            var dbUser = await _dbContext.Users.FindAsync(returnedUser.Id);
            Assert.IsNotNull(dbUser);
            Assert.AreEqual("John Doe", dbUser.Name);
        }

        [TestMethod]
        [Description("Negative: Create should return 400 when request is null")]
        public async Task Create_NullRequest_ReturnsBadRequest()
        {
            // Arrange
            UserRequest? request = null;
            var ct = CancellationToken.None;

            // Act
            var result = await _controller.Create(request!, ct);

            // Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
            var badResult = (BadRequestObjectResult)result;
            Assert.AreEqual(400, badResult.StatusCode);
        }

        [TestMethod]
        [Description("Negative: Create should return 400 when name is too short (validation)")]
        public async Task Create_NameTooShort_ReturnsBadRequest()
        {
            // Arrange
            var request = new UserRequest { Name = "J", Age = 25, City = "New York", State = "NY", Pincode = "100001" };
            var ct = CancellationToken.None;

            // Act - need to manually validate since Swagger/MVC does it automatically
            var validator = new System.ComponentModel.DataAnnotations.ValidationContext(request);
            var validationErrors = new List<System.ComponentModel.DataAnnotations.ValidationResult>();
            bool isValid = System.ComponentModel.DataAnnotations.Validator.TryValidateObject(request, validator, validationErrors, true);

            // Assert
            Assert.IsFalse(isValid);
            Assert.IsTrue(validationErrors.Any(v => v.MemberNames.Contains(nameof(UserRequest.Name))));
        }

        #endregion

        #region Update Tests

        [TestMethod]
        [Description("Success: Update should modify an existing user and return 200 OK")]
        public async Task Update_ValidIdAndRequest_ReturnsOkWithUpdatedUser()
        {
            // Arrange
            var user = new User { Name = "John Doe", Age = 25, City = "New York", State = "NY", Pincode = "100001" };
            _dbContext.Users.Add(user);
            await _dbContext.SaveChangesAsync();

            var request = new UserRequest { Name = "Jane Doe", Age = 30, City = "Los Angeles", State = "CA", Pincode = "900001" };
            var ct = CancellationToken.None;

            // Act
            var result = await _controller.Update(user.Id, request, ct);

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            var okResult = (OkObjectResult)result;
            Assert.AreEqual(200, okResult.StatusCode);

            var updatedUser = (UserResponse)okResult.Value!;
            Assert.AreEqual("Jane Doe", updatedUser.Name);
            Assert.AreEqual(30, updatedUser.Age);
        }

        [TestMethod]
        [Description("Negative: Update should return 404 when user does not exist")]
        public async Task Update_NonExistentId_ReturnsNotFound()
        {
            // Arrange
            var request = new UserRequest { Name = "Jane Doe", Age = 30, City = "Los Angeles", State = "CA", Pincode = "900001" };
            var ct = CancellationToken.None;

            // Act
            var result = await _controller.Update(999, request, ct);

            // Assert
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
            var notFoundResult = (NotFoundResult)result;
            Assert.AreEqual(404, notFoundResult.StatusCode);
        }

        [TestMethod]
        [Description("Negative: Update should return 400 when ID is zero or negative")]
        public async Task Update_InvalidId_ReturnsBadRequest()
        {
            // Arrange
            var request = new UserRequest { Name = "Jane Doe", Age = 30, City = "Los Angeles", State = "CA", Pincode = "900001" };
            var ct = CancellationToken.None;

            // Act
            var result = await _controller.Update(0, request, ct);

            // Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
            var badResult = (BadRequestObjectResult)result;
            Assert.AreEqual(400, badResult.StatusCode);
            Assert.AreEqual("User ID must be greater than 0.", badResult.Value);
        }

        #endregion

        #region Delete Tests

        [TestMethod]
        [Description("Success: Delete should remove a user and return 204 NoContent")]
        public async Task Delete_ValidId_ReturnsNoContent()
        {
            // Arrange
            var user = new User { Name = "John Doe", Age = 25, City = "New York", State = "NY", Pincode = "100001" };
            _dbContext.Users.Add(user);
            await _dbContext.SaveChangesAsync();

            var ct = CancellationToken.None;

            // Act
            var result = await _controller.Delete(user.Id, ct);

            // Assert
            Assert.IsInstanceOfType(result, typeof(NoContentResult));
            var noContentResult = (NoContentResult)result;
            Assert.AreEqual(204, noContentResult.StatusCode);

            // Verify user was actually deleted from database
            var deletedUser = await _dbContext.Users.FindAsync(user.Id);
            Assert.IsNull(deletedUser);
        }

        [TestMethod]
        [Description("Negative: Delete should return 404 when user does not exist")]
        public async Task Delete_NonExistentId_ReturnsNotFound()
        {
            // Arrange
            var ct = CancellationToken.None;

            // Act
            var result = await _controller.Delete(999, ct);

            // Assert
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
            var notFoundResult = (NotFoundResult)result;
            Assert.AreEqual(404, notFoundResult.StatusCode);
        }

        [TestMethod]
        [Description("Negative: Delete should return 400 when ID is zero or negative")]
        public async Task Delete_InvalidId_ReturnsBadRequest()
        {
            // Arrange
            var ct = CancellationToken.None;

            // Act
            var result = await _controller.Delete(-1, ct);

            // Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
            var badResult = (BadRequestObjectResult)result;
            Assert.AreEqual(400, badResult.StatusCode);
            Assert.AreEqual("User ID must be greater than 0.", badResult.Value);
        }

        #endregion
    }
}
