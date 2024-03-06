﻿using CodeCool.SeasonalProductDiscounter.Model.Offers;
using CodeCool.SeasonalProductDiscounter.Model.Products;
using CodeCool.SeasonalProductDiscounter.Model.Transactions;
using CodeCool.SeasonalProductDiscounter.Model.Users;
using CodeCool.SeasonalProductDiscounter.Service.Discounts;
using CodeCool.SeasonalProductDiscounter.Service.Logger;
using CodeCool.SeasonalProductDiscounter.Service.Products.Repository;
using CodeCool.SeasonalProductDiscounter.Service.Transactions.Repository;
using CodeCool.SeasonalProductDiscounter.Service.Users;

namespace CodeCool.SeasonalProductDiscounter.Service.Transactions.Simulator;

public class TransactionsSimulator
{
    private static readonly Random Random = new();

    private readonly ILogger _logger;
    private readonly IUserRepository _userRepository;
    private readonly IProductRepository _productRepository;
    private readonly IAuthenticationService _authenticationService;
    private readonly IDiscounterService _discounterService;
    private readonly ITransactionRepository _transactionRepository;

    public TransactionsSimulator(
        ILogger logger,
        IUserRepository userRepository,
        IProductRepository productRepository,
        IAuthenticationService authenticationService,
        IDiscounterService discounterService,
        ITransactionRepository transactionRepository)
    {
        _logger = logger;
        _userRepository = userRepository;
        _productRepository = productRepository;
        _authenticationService = authenticationService;
        _discounterService = discounterService;
        _transactionRepository = transactionRepository;
    }

    public void Run(TransactionsSimulatorSettings settings)
    {
        int successfulTransactions = 0;
        int rounds = 0;

        _logger.LogInfo("Starting simulation");
        while (successfulTransactions <= settings.TransactionsCount)
        {
            _logger.LogInfo(
                $"Simulation round #{rounds++}, successful transactions: {successfulTransactions}/{settings.TransactionsCount}");

            //Get a random user
            var user = GetRandomUser(settings.UsersCount);
            _logger.LogInfo($"User [{user.UserName}] looking to buy a product");

            //Auth user
            if (!AuthUser(user))
            {
                //If auth is not successful, register the user
                RegisterUser(user);
            }

            //Get user from the repo to have an ID (ID is auto-generated by the database)
            user = GetUserFromRepo(user.UserName);

            //User selects product
            var product = SelectProduct(user);

            //Out of products to sell - terminate cycle
            if (product == null)
            {
                break;
            }

            //Get offer
            var offer = GetOffer(product, settings.Date);

            //Create transaction
            var transaction = CreateTransaction(settings.Date, user, product, offer.Price);

            //Save transaction & set product_sold to TRUE
            if (SaveTransaction(transaction))
            {
                SetProductAsSold(product);
                successfulTransactions++;
            }
        }
    }


    private static User GetRandomUser(int usersCount)
    {
        return new User(0, $"user{Random.Next(0, usersCount)}", "pw");
    }

    private User GetUserFromRepo(string username)
    {
        _logger.LogInfo($"Retrieving user {username} from DB.");
        return _userRepository.Get(username);
    }

    private bool AuthUser(User user)
    {
        _logger.LogInfo($"Authenticating user: {user.UserName}");
        return _authenticationService.Authenticate(user);
    }

    private bool RegisterUser(User user)
    {
        _logger.LogInfo($"Registering user: {user.UserName}");
        return _userRepository.Add(user);
    }

    private Product? SelectProduct(User user)
    {
        _logger.LogInfo($"Selecting random product.");
        var product = GetRandomProduct();
        return product ?? null;
    }

    private Product? GetRandomProduct()
    {
        var allProducts = _productRepository.AvailableProducts.ToList();

        if (!allProducts.Any())
        {
            return null;
        }

        return allProducts[Random.Next(0, allProducts.Count)];
    }

    private Offer GetOffer(Product product, DateTime date)
    {
        _logger.LogInfo($"Retrieving offers.");
        return _discounterService.GetOffer(product, date);
    }

    private static Transaction CreateTransaction(DateTime date, User user, Product product, double price)
    {
        return new Transaction(0, date, user, product, price);
    }

    private bool SaveTransaction(Transaction transaction)
    {
        _logger.LogInfo($"Saving transaction to DB.");
        return _transactionRepository.Add(transaction);
    }

    private bool SetProductAsSold(Product product)
    {
        _logger.LogInfo($"Setting {product.Name} with ID {product.Id} to sold.");
        return _productRepository.SetProductAsSold(product);
    }
}
