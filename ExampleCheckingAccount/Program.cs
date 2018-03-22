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
            var account = await repo.CreateEntity<CheckingAccount>();

            
            await account.CreateAccount("Pete Seeger", 25.00m);

            Console.WriteLine($"Account now exists for {account.AccountHolderName} with balance of ${account.Balance}");


            await account.Deposit(15.0m);
            Console.WriteLine($"Deposited $15, balance is now {account.Balance}");
            
            await account.WriteCheck("Bruce Springstein", 100.00m, "Billy Joel sucks");
            Console.WriteLine($"Wrote check for $100, balance is now ${account.Balance}");
            //we should be overdrawn now
            Console.WriteLine($"Account is now overdrawn = {account.Overdrawn}!");
            Console.WriteLine($"Account balance with penalty is {account.Balance}");
            
            Console.WriteLine("Let's try that again with a penalty-free account, reprocessing the past with new rules. . . ");

            var newAccount = new CheckingAccountNoOverdraftCost {Id = account.Id};
            await repo.RegisterEntity(newAccount);
            Console.WriteLine($"Account balance with penalty (removed) is {newAccount.Balance} for the account of {newAccount.AccountHolderName}");
            
        }
    }
}