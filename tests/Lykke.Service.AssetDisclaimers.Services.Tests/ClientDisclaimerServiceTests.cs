using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common.Log;
using Lykke.Common.Log;
using Lykke.Payments.FxPaygate.Client;
using Lykke.Service.AssetDisclaimers.Core.Domain;
using Lykke.Service.AssetDisclaimers.Core.Repositories;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using StackExchange.Redis;

namespace Lykke.Service.AssetDisclaimers.Services.Tests
{
    [TestClass]
    public class ClientDisclaimerServiceTests
    {
        private readonly Mock<IClientDisclaimerRepository> _clientDisclaimerRepositoryMock =
            new Mock<IClientDisclaimerRepository>();
        
        private readonly Mock<ILykkeEntityRepository> _lykkeEntityRepositoryMock =
            new Mock<ILykkeEntityRepository>();
        
        private readonly Mock<IDisclaimerRepository> _disclaimerRepositoryMock =
            new Mock<IDisclaimerRepository>();

        private readonly Mock<ILogFactory> _logMock =
            new Mock<ILogFactory>();

        private readonly Mock<IFxPaygateClient> _fxPaygateClientMock = 
            new Mock<IFxPaygateClient>();
        
        private readonly Mock<IDatabase> _databaseMock = 
            new Mock<IDatabase>();
        
        private RedisService _redisService;
        
        private ClientDisclaimerService _service;
        
        [TestInitialize]
        public void TestInitialized()
        {
            _logMock.Setup(o => o.CreateLog(It.IsAny<object>())).Returns(EmptyLog.Instance);
            _redisService = new RedisService(
                _lykkeEntityRepositoryMock.Object,
                _disclaimerRepositoryMock.Object,
                _clientDisclaimerRepositoryMock.Object,
                _databaseMock.Object
                );
            
            _service = new ClientDisclaimerService(
                _clientDisclaimerRepositoryMock.Object,
                _fxPaygateClientMock.Object,
                _redisService,
                new TimeSpan(0, 1, 0),
                Guid.NewGuid().ToString(),
                _logMock.Object);
        }

        [TestMethod]
        public async Task CheckWithdrawalAsync_Creates_Client_Pending_Disclaimer_With_Actual_StartDate()
        {
            // arrange
            string clientId = "me";
            string lykkeEntityId = "LKE";
            string disclaimerId = null;

            var disclaimers = new List<IDisclaimer>
            {
                new Disclaimer
                {
                    Id = "1",
                    LykkeEntityId = lykkeEntityId,
                    StartDate = DateTime.UtcNow.Date.AddDays(1),
                    Type = DisclaimerType.Withdrawal
                },
                new Disclaimer
                {
                    Id = "2",
                    LykkeEntityId = lykkeEntityId,
                    StartDate = DateTime.UtcNow.Date,
                    Type = DisclaimerType.Withdrawal
                },
                new Disclaimer
                {
                    Id = "3",
                    LykkeEntityId = lykkeEntityId,
                    StartDate = DateTime.UtcNow.Date.AddDays(-1),
                    Type = DisclaimerType.Withdrawal
                }
            };
            
            _lykkeEntityRepositoryMock.Setup(o => o.GetAsync(It.IsAny<string>()))
                .ReturnsAsync(new LykkeEntity {Id = lykkeEntityId});

            _clientDisclaimerRepositoryMock.Setup(o => o.GetAsync(It.IsAny<string>()))
                .ReturnsAsync(new List<IClientDisclaimer>());

            _disclaimerRepositoryMock.Setup(o => o.FindAsync(It.IsAny<string>()))
                .ReturnsAsync((string id) => disclaimers.FirstOrDefault(x => x.Id == id));

            _clientDisclaimerRepositoryMock.Setup(o => o.InsertOrReplaceAsync(It.IsAny<IClientDisclaimer>()))
                .Returns(Task.CompletedTask)
                .Callback((IClientDisclaimer c) => disclaimerId = c.DisclaimerId);
            
            _databaseMock.Setup(o => o.SetMembersAsync(It.IsAny<RedisKey>(), It.IsAny<CommandFlags>())).ReturnsAsync(new RedisValue[]{"1", "2", "3"});
            
            // act
            bool created = await _service.CheckWithdrawalAsync(clientId, lykkeEntityId);
            
            // assert
            Assert.IsTrue(created && disclaimerId == "2");
        }
        
        [TestMethod]
        public async Task CheckWithdrawalAsync_Not_Creates_Client_Pending_Disclaimer_If_It_Approved()
        {
            // arrange
            string clientId = "me";
            string lykkeEntityId = "LKE";
            string disclaimerId = null;

            var disclaimers = new List<IDisclaimer>
            {
                new Disclaimer
                {
                    Id = "1",
                    LykkeEntityId = lykkeEntityId,
                    StartDate = DateTime.UtcNow.Date.AddDays(1),
                    Type = DisclaimerType.Withdrawal
                },
                new Disclaimer
                {
                    Id = "2",
                    LykkeEntityId = lykkeEntityId,
                    StartDate = DateTime.UtcNow.Date,
                    Type = DisclaimerType.Withdrawal
                },
                new Disclaimer
                {
                    Id = "3",
                    LykkeEntityId = lykkeEntityId,
                    StartDate = DateTime.UtcNow.Date.AddDays(-1),
                    Type = DisclaimerType.Withdrawal
                }
            };
            
            _lykkeEntityRepositoryMock.Setup(o => o.GetAsync(It.IsAny<string>()))
                .ReturnsAsync(new LykkeEntity {Id = lykkeEntityId});

            _clientDisclaimerRepositoryMock.Setup(o => o.GetAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(new ClientDisclaimer
                    {
                        ClientId = clientId,
                        DisclaimerId = "2",
                        Approved = true,
                        ApprovedDate = DateTime.UtcNow
                    }
                );

            _disclaimerRepositoryMock.Setup(o => o.FindAsync(It.IsAny<string>()))
                .ReturnsAsync((string id) => disclaimers.FirstOrDefault(x => x.Id == id));

            _clientDisclaimerRepositoryMock.Setup(o => o.InsertOrReplaceAsync(It.IsAny<IClientDisclaimer>()))
                .Returns(Task.CompletedTask)
                .Callback((IClientDisclaimer c) => disclaimerId = c.DisclaimerId);
            
            _databaseMock.Setup(o => o.SetMembersAsync(It.IsAny<RedisKey>(), It.IsAny<CommandFlags>())).ReturnsAsync(new RedisValue[]{"2"});
            
            // act
            bool created = await _service.CheckWithdrawalAsync(clientId, lykkeEntityId);
            
            // assert
            Assert.IsTrue(!created && disclaimerId == null);
        }
    }
}
