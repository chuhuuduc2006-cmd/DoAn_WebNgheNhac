using Microsoft.Data.SqlClient;

namespace WebNgheNhac.Models;

public class DbHelper
{
    private readonly string _conn;
    public DbHelper(IConfiguration config)
    {
        _conn = config.GetConnectionString("DefaultConnection")!;
    }

    public SqlConnection GetConnection() => new SqlConnection(_conn);
}