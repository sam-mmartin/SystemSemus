using System.Data.Entity;
using System.Data.Entity.SqlServer;

namespace SistemaSemus.DAL
{
    public class SemusConfiguration : DbConfiguration
    {
        public SemusConfiguration()
        {
            SetExecutionStrategy("System.Data.SqlClient", () => new SqlAzureExecutionStrategy());
        }
    }
}