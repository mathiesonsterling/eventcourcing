using System;
using System.Threading.Tasks;
using CheckingAccountExample.Entities;
using EventCoursing.Repositories;
using EventCoursing.Services;
using EventCoursingSimple.Repositories;
using EventCoursingSimple.Services;

namespace CheckingAccountExample
{
    class Program
    {
        static void Main(string[] args)
        {
            var backing = new MemoryBasedEventStore();
            var repo = new BasicEntityRepository(backing, backing);
            CreateAndUseAnAccount(repo, repo).Wait();
        }

        private static async Task CreateAndUseAnAccount(IEventReceiver<Guid> eventPipeline,
            IEntityRepository<Guid> entities)
        {
            //make an account
            var account = new CheckingAccount();

            await account.CreateAccount("Billy Joel", 25.00m);

            Console.WriteLine($"Account now exists for {account.AccountHolderName}");


            await account.Deposit(15.0m);

            await account.WriteCheck("Bruce Springstein", 100.00m, "Billy Joel sucks");

            //we should be overdrawn now
            Console.WriteLine($"Account is now overdrawn = {account.Overdrawn}!");
            Console.WriteLine($"Account balance with penalty is {account.Balance}");
        }
    }
}