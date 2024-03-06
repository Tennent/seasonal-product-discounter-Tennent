using System.Data;
using CodeCool.SeasonalProductDiscounter.Model.Products;
using CodeCool.SeasonalProductDiscounter.Model.Transactions;
using CodeCool.SeasonalProductDiscounter.Model.Users;
using CodeCool.SeasonalProductDiscounter.Service.Logger;
using CodeCool.SeasonalProductDiscounter.Service.Persistence;
using CodeCool.SeasonalProductDiscounter.Utilities;

namespace CodeCool.SeasonalProductDiscounter.Service.Transactions.Repository;

public class TransactionRepository : SqLiteConnector, ITransactionRepository
{
    private readonly string _tableName;

    public TransactionRepository(string dbFile, ILogger logger) : base(dbFile, logger)
    {
        _tableName = DatabaseManager.TransactionsTableName;
    }

    public bool Add(Transaction transaction)
    {
        var query = @$"INSERT INTO {_tableName} (date, user_id, product_id, price_paid)
                    VALUES ('{transaction.Date}', {transaction.User.Id}, {transaction.Product.Id}, '{transaction.PricePaid}')";

        return ExecuteNonQuery(query);
    }

    public IEnumerable<Transaction> GetAll()
    {
        var query = @$"SELECT * FROM {_tableName}
                    INNER JOIN {DatabaseManager.UsersTableName} on {DatabaseManager.UsersTableName}.Id = {_tableName}.user_id
                    INNER JOIN {DatabaseManager.ProductsTableName} on {DatabaseManager.ProductsTableName}.Id = {_tableName}.product_id";

        try
        {
            using var connection = GetPhysicalDbConnection();
            using var command = GetCommand(query, connection);
            using var reader = command.ExecuteReader();
            Logger.LogInfo($"{GetType().Name} executing query: {query}");


            var dt = new DataTable();

            //This is required otherwise the DataTable tries to force the DB constrains on the result set, which can cause problems in some cases (e.g. UNIQUE)
            using var ds = new DataSet { EnforceConstraints = false };
            ds.Tables.Add(dt);
            dt.Load(reader);
            ds.Tables.Remove(dt);

            var lst = new List<Transaction>();
            foreach (DataRow row in dt.Rows)
            {
                var user = ToUser(row);
                var product = ToProduct(row);
                var transaction = ToTransaction(row, user, product);

                lst.Add(transaction);
            }

            return lst;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    private static User ToUser(DataRow row)
    {
        var id = TypeConverters.ToInt(row["user_id"]);
        var user = new User
        (
            id,
            TypeConverters.ToString(row["user_name"]),
            TypeConverters.ToString(row["password"])
        );

        return user;
    }

    private static Product ToProduct(DataRow row)
    {
        var id = TypeConverters.ToInt(row["product_id"]);
        var product = new Product
        (
            (uint)id,
            TypeConverters.ToString(row["name"]),
            TypeConverters.GetColorEnum(TypeConverters.ToString(row["color"])),
            TypeConverters.GetSeasonEnum(TypeConverters.ToString(row["season"])),
            TypeConverters.ToDouble(row["price"]),
            TypeConverters.ToBool(TypeConverters.ToInt(row["sold"]))
        );

        return product;
    }

    private static Transaction ToTransaction(DataRow row, User user, Product product)
    {
        var id = TypeConverters.ToInt(row["id"]);
        var transaction = new Transaction
        (
            id,
            TypeConverters.ToDateTime(TypeConverters.ToString(row["date"])),
            user,
            product,
            TypeConverters.ToDouble(row["price_paid"])
        );

        return transaction;
    }
}