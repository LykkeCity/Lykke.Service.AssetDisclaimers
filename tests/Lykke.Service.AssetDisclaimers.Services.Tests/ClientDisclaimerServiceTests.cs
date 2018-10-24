using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Common.Log;
using Lykke.Payments.FxPaygate.Client;
using Lykke.Service.AssetDisclaimers.Core.Domain;
using Lykke.Service.AssetDisclaimers.Core.Repositories;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

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

        private readonly Mock<ILog> _logMock =
            new Mock<ILog>();

        private readonly Mock<IFxPaygateClient> _fxPaygateClientMock = 
            new Mock<IFxPaygateClient>();
        
        private ClientDisclaimerService _service;
        
        [TestInitialize]
        public void TestInitialized()
        {
            _service = new ClientDisclaimerService(
                _clientDisclaimerRepositoryMock.Object,
                _lykkeEntityRepositoryMock.Object,
                _disclaimerRepositoryMock.Object,
                _fxPaygateClientMock.Object,
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
                    StartDate = DateTime.UtcNow.Date.AddDays(1),
                    Type = DisclaimerType.Withdrawal
                },
                new Disclaimer
                {
                    Id = "2",
                    StartDate = DateTime.UtcNow.Date,
                    Type = DisclaimerType.Withdrawal
                },
                new Disclaimer
                {
                    Id = "3",
                    StartDate = DateTime.UtcNow.Date.AddDays(-1),
                    Type = DisclaimerType.Withdrawal
                }
            };
            
            _lykkeEntityRepositoryMock.Setup(o => o.GetAsync(It.IsAny<string>()))
                .ReturnsAsync(new LykkeEntity {Id = lykkeEntityId});

            _clientDisclaimerRepositoryMock.Setup(o => o.GetAsync(It.IsAny<string>()))
                .ReturnsAsync(new List<IClientDisclaimer>());

            _disclaimerRepositoryMock.Setup(o => o.GetAsync(It.IsAny<string>()))
                .ReturnsAsync(disclaimers);

            _clientDisclaimerRepositoryMock.Setup(o => o.InsertOrReplaceAsync(It.IsAny<IClientDisclaimer>()))
                .Returns(Task.CompletedTask)
                .Callback((IClientDisclaimer c) => disclaimerId = c.DisclaimerId);
            
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
                    StartDate = DateTime.UtcNow.Date.AddDays(1),
                    Type = DisclaimerType.Withdrawal
                },
                new Disclaimer
                {
                    Id = "2",
                    StartDate = DateTime.UtcNow.Date,
                    Type = DisclaimerType.Withdrawal
                },
                new Disclaimer
                {
                    Id = "3",
                    StartDate = DateTime.UtcNow.Date.AddDays(-1),
                    Type = DisclaimerType.Withdrawal
                }
            };
            
            _lykkeEntityRepositoryMock.Setup(o => o.GetAsync(It.IsAny<string>()))
                .ReturnsAsync(new LykkeEntity {Id = lykkeEntityId});

            _clientDisclaimerRepositoryMock.Setup(o => o.GetAsync(It.IsAny<string>()))
                .ReturnsAsync(new List<IClientDisclaimer>
                {
                    new ClientDisclaimer
                    {
                        ClientId = clientId,
                        DisclaimerId = "2",
                        Approved = true,
                        ApprovedDate = DateTime.UtcNow
                    }
                });

            _disclaimerRepositoryMock.Setup(o => o.GetAsync(It.IsAny<string>()))
                .ReturnsAsync(disclaimers);

            _clientDisclaimerRepositoryMock.Setup(o => o.InsertOrReplaceAsync(It.IsAny<IClientDisclaimer>()))
                .Returns(Task.CompletedTask)
                .Callback((IClientDisclaimer c) => disclaimerId = c.DisclaimerId);
            
            // act
            bool created = await _service.CheckWithdrawalAsync(clientId, lykkeEntityId);
            
            // assert
            Assert.IsTrue(!created && disclaimerId == null);
        }
    }
}
