using System.Data.Entity;

namespace CSV
{
    public class CarDb : DbContext
    {
        public CarDb() : base("CarsDB")
        {        
        }
        public DbSet<Car> Cars { get; set; }
    }
}
