using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.IO;
using System.Linq;

namespace CSV
{
    class Program
    {
        static void Main(string[] args)
        {
            //DisplayAll();
            //BestAccelerationInOrigins();
            //OringinStatistics(cars);
            Database.SetInitializer(new DropCreateDatabaseIfModelChanges<CarDb>());
            //InsertToDatabase();
            QueryDatabase();

            using (IRepository<Car> carRepository = new Repository<Car>(new CarDb()))
            {
                //AddCar(carRepository);
                //DumpCars(carRepository);
                //FindCar(carRepository);
            }
        }

        private static void FindCar(IRepository<Car> carRepository)
        {
            var car = carRepository.Get(1);
            Console.WriteLine(car.Model);
        }

        private static void DumpCars(IRepository<Car> carRepository)
        {
            var cars = carRepository.GetAll();
            foreach (var car in cars)
            {
                Console.WriteLine(car.Model);
            }
        }

        private static void AddCar(IRepository<Car> carRepository)
        {
            carRepository.Add(new Car
            {
                Model = "Test",
                MPG = 5.5,
                Cylinders = 4,
                EngineDisp = 100,
                Horsepower = 100,
                Weight = 1000,
                Accelerate = 5,
                Year = 2017,
                Origin = "Polska"
            });
            carRepository.Commit();
        }

        private static void QueryDatabase()
        {
            var db = new CarDb();
            db.Database.Log = Console.WriteLine;

            var methodSyntaxQuery =
                db.Cars.GroupBy(c => c.Origin)
                        .Select(g => new
                        {
                            Name = g.Key,
                            Cars = g.Select(c => new
                            {
                                c.Model,
                                c.Accelerate
                            }).OrderBy(c => c.Accelerate).Take(3)
                        });

            var querySyntaxQuery =
                from car in db.Cars
                group car by car.Origin into origin
                select new
                {
                    Name = origin.Key,
                    Cars = (from car in origin
                            orderby car.Accelerate ascending
                            select new
                            {
                                car.Model,
                                car.Accelerate
                            }).Take(3)
                };

            foreach (var group in querySyntaxQuery)
            {
                Console.WriteLine(group.Name);
                foreach (var car in group.Cars)
                {
                    Console.WriteLine($"\t{car.Model} : {car.Accelerate}");
                }
            }
        }

        private static void InsertToDatabase()
        {
            var cars = ProcessFile("cars.csv");
            var db = new CarDb();

            if (!db.Cars.Any())
            {
                foreach (var car in cars)
                {
                    db.Cars.Add(car);
                }
                db.SaveChanges();
            }
        }

        private static void OringinStatistics()
        {
            var cars = ProcessFile("cars.csv");
            var query =
                cars.GroupBy(c => c.Origin)
                    .Select(g =>
                    {
                        var results = g.Aggregate(new CarStatistics(),
                                            (acc, c) => acc.Accumulate(c),
                                            acc => acc.Compute());
                        return new
                        {
                            Origin = g.Key,
                            Avg = results.Average,
                            Min = results.Min,
                            Max = results.Max
                        };
                    })
                    .OrderByDescending(r => r.Max);

            foreach (var result in query)
            {
                Console.WriteLine($"{result.Origin}");
                Console.WriteLine($"\t Max: {result.Max}");
                Console.WriteLine($"\t Min: {result.Min}");
                Console.WriteLine($"\t Avg: {result.Avg}");
            }
        }

        private static void BestAccelerationInOrigins()
        {
            var cars = ProcessFile("cars.csv");
            var query =
                cars.GroupBy(c => c.Origin)
                    .Select(g => new
                    {
                        Name = g.Key,
                        Cars = g.Select(c => new
                        {
                            c.Model,
                            c.Accelerate
                        }).OrderBy(c => c.Accelerate).Take(3)
                    });

            foreach (var group in query)
            {
                Console.WriteLine(group.Name);
                foreach (var car in group.Cars)
                {
                    Console.WriteLine($"\t{car.Model} : {car.Accelerate}");
                }
            }
        }

        private static void DisplayAll()
        {
            var cars = ProcessFile("cars.csv");

            foreach (var car in cars)
            {
                Console.WriteLine($"{car.Model} : {car.MPG} : {car.Cylinders} : {car.EngineDisp} : {car.Horsepower} : {car.Weight} : {car.Accelerate} : {car.Year} : {car.Origin}");
            }
            Console.WriteLine(cars.Count());
        }

        private static List<Car> ProcessFile(string path)
        {
            var query = File.ReadAllLines(path)
                            .Skip(1)
                            .Where(l => l.Length > 1)
                            .Select(l =>
                            {
                                var columns = l.Split(',');
                                return new Car
                                {
                                    Model = columns[0],
                                    MPG = double.Parse(columns[1], CultureInfo.InvariantCulture),
                                    Cylinders = int.Parse(columns[2]),
                                    EngineDisp = double.Parse(columns[3], CultureInfo.InvariantCulture),
                                    Horsepower = int.Parse(columns[4]),
                                    Weight = int.Parse(columns[5]),
                                    Accelerate = double.Parse(columns[6], CultureInfo.InvariantCulture),
                                    Year = int.Parse(columns[7]),
                                    Origin = columns[8]
                                };
                            });

            return query.ToList();
        }
    }
}
