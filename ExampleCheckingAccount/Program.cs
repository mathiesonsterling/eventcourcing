using System;
using System.Threading.Tasks;
using EventCoursing.Repositories;
using EventCoursing.Services;
using EventCoursingSimple.Repositories;
using EventCoursingSimple.Services;
using ExampleCheckingAccount.Entities;

namespace ExampleCheckingAccount
{
    class Program
    {
        static void Main(string[] args)
        {
            var backing = new MemoryBasedEventStore();
            var repo = new BasicEntityRepository(backing, backing);
            CreateAndUseAnAccount(repo).Wait();
        }

        private static async Task CreateAndUseAnAccount(IEntityRepository<Guid> repo)
        {
            //make an account
            var account = new CheckingAccount();

            await repo.RegisterEntity(account);
            
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