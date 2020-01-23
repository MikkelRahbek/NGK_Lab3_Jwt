using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NGK_Lab3_V2.Controllers;
using NGK_Lab3_V2.DbContext;
using NGK_Lab3_V2.DTO_s;
using NGK_Lab3_V2.Models;
using NSubstitute;
using NUnit.Framework;

namespace NGK_Lab3_V2.Test.Unit
{
    public class AccountControllerUnitTest
    {
        private AccountController _uut;
        private UserManager<User> _userManager;
        private SignInManager<User> _signInManager;


        private UserDbContext _context;
        private ServiceCollection services;
        private ServiceProvider provider;


        [SetUp]
        public async Task Setup()
        {
            services = new ServiceCollection();

            services
                .AddSingleton<IConfiguration>(new ConfigurationBuilder().Build())
                .AddDbContext<UserDbContext>(o => o.UseSqlite("DataSource=:memory:", x => { }))
                .AddIdentity<User, IdentityRole>()
                .AddEntityFrameworkStores<UserDbContext>()
                .AddDefaultTokenProviders();
            ;

            services.AddLogging();

            // Has to be configured as in Startup
            services.Configure<IdentityOptions>(config =>
            {
                config.Password.RequiredLength = 4;
                config.Password.RequireDigit = false;
                config.Password.RequireLowercase = false;
                config.Password.RequireNonAlphanumeric = false;
                config.Password.RequireUppercase = false;
            });

            provider = services.BuildServiceProvider();

            _context = provider.GetRequiredService<UserDbContext>();
            _userManager = provider.GetRequiredService<UserManager<User>>();
            _signInManager = provider.GetRequiredService<SignInManager<User>>();

            _context.Database.OpenConnection();
            _context.Database.EnsureCreated();

            _uut = new AccountController(_userManager, _signInManager);
            //_services = await ExampleServiceCollectionFactory.CreateAsync();
        }

        [TearDown]
        public void TearDown()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }


        [TestCase("1234")]
        [TestCase("abcd")]
        [TestCase("1234abcd")]
        public void Register_ValidPassword_ReturnsStatusCode200(string password)
        {
            // Arrange
            DtoUser testUser = new DtoUser
            {
                Email = "email@email.com",
                FirstName = "FirstName",
                LastName = "LastName",
                Password = password
            };

            // Act
            var result = _uut.Register(testUser);
            Thread.Sleep(1000);

            // Assert
            Assert.That(result.Result.GetType(), Is.EqualTo(typeof(OkObjectResult)));
        }
        
        [TestCase("")]
        [TestCase("123")]
        public void Register_PasswordTooShort_ReturnsStatusCode400(string password)
        {
            // Arrange
            DtoUser testUser = new DtoUser
            {
                Email = "email@email.com",
                FirstName = "FirstName",
                LastName = "LastName",
                Password = password
            };

            // Act
            var result = _uut.Register(testUser);
            Thread.Sleep(1000);

            // Assert
            Assert.That(result.Result.GetType(), Is.EqualTo(typeof(BadRequestObjectResult)));
        }


        [Test]
        public void Login_ValidEmailAndPassword_ReturnsStatusCode200()
        {
            // Arrange
            DtoUser testUser = new DtoUser
            {
                Email = "email@email.com",
                FirstName = "FirstName",
                LastName = "LastName",
                Password = "1234"
            };
             var temp = _uut.Register(testUser);
             Thread.Sleep(1000);

            // Act
            var result =  _uut.Login(testUser);
            Thread.Sleep(1000);

            // Assert
            Assert.That(result.Result.GetType(), Is.EqualTo(typeof(OkObjectResult)));
        }

        [Test]
        public void Login_InvalidEmailAndValidPassword_ReturnsStatusCode400()
        {
            // Arrange
            DtoUser trueTestUser = new DtoUser
            {
                Email = "true@email.com",
                FirstName = "FirstName",
                LastName = "LastName",
                Password = "1234"
            };
            DtoUser testUser = new DtoUser
            {
                Email = "fake@email.com",
                FirstName = "FirstName",
                LastName = "LastName",
                Password = "1234"
            };
            var temp = _uut.Register(trueTestUser);
            Thread.Sleep(1000);

            // Act
            var result = _uut.Login(testUser);
            Thread.Sleep(1000);

            // Assert
            Assert.That(result.Result.GetType(), Is.EqualTo(typeof(BadRequestObjectResult)));
        }

        [Test]
        public void Login_ValidEmailAndInvalidPassword_ReturnsStatusCode400()
        {
            // Arrange
            DtoUser trueTestUser = new DtoUser
            {
                Email = "true@email.com",
                FirstName = "FirstName",
                LastName = "LastName",
                Password = "true"
            };
            DtoUser testUser = new DtoUser
            {
                Email = "fake@email.com",
                FirstName = "FirstName",
                LastName = "LastName",
                Password = "wrong"
            };
            var temp = _uut.Register(trueTestUser);
            Thread.Sleep(1000);

            // Act
            var result = _uut.Login(testUser);
            Thread.Sleep(1000);

            // Assert
            Assert.That(result.Result.GetType(), Is.EqualTo(typeof(BadRequestObjectResult)));
        }
    }
}