using System.Text;
using CodeCool.SeasonalProductDiscounter.Model.Products;
using CodeCool.SeasonalProductDiscounter.Service.Logger;
using CodeCool.SeasonalProductDiscounter.Service.Persistence;
using CodeCool.SeasonalProductDiscounter.Utilities;

namespace CodeCool.SeasonalProductDiscounter.Service.Products.Repository;

public class ProductRepository : SqLiteConnector, IProductRepository
{
    private readonly string _tableName;

    public IEnumerable<Product> AvailableProducts => GetAvailableProducts();

    public ProductRepository(string dbFile, ILogger logger) : base(dbFile, logger)
    {
        _tableName = DatabaseManager.ProductsTableName;
    }

    private IEnumerable<Product> GetAvailableProducts()
    {
        var query = 
            @"SELECT * FROM products
               WHERE sold = 0";
        
        var ret = new List<Product>();

        try
        {
            using var connection = GetPhysicalDbConnection();
            using var command = GetCommand(query, connection);
            using var reader = command.ExecuteReader();
            Logger.LogInfo($"{GetType().Name} executing query: {query}");

            while (reader.Read())
            {
                var product = new Product
                (
                    (uint)reader.GetInt32(reader.GetOrdinal("id")),
                    reader.GetString(reader.GetOrdinal("name")),
                    TypeConverters.GetColorEnum(reader.GetString(reader.GetOrdinal("color"))),
                    TypeConverters.GetSeasonEnum(reader.GetString(reader.GetOrdinal("season"))),
                    reader.GetDouble(reader.GetOrdinal("price")),
                    reader.GetBoolean(reader.GetOrdinal("sold"))
                );
                
                ret.Add(product);
            }
        }
        catch (Exception e)
        {
            Logger.LogError("Error retrieving available products!\n" + e.Message);
            throw;
        }

        return ret;
    }

    public bool Add(IEnumerable<Product> products)
    {
        //
        return false;
    }

    public bool SetProductAsSold(Product product)
    {
        //Set the sold field in the database
        var query = "";
        return ExecuteNonQuery(query);
    }
}
