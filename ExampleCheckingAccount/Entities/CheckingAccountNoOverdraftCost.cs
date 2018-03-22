using System;
using System.Threading.Tasks;
using EventCoursing.Entities;
using ExampleCheckingAccount.Entities.Events;

namespace ExampleCheckingAccount.Entities
{
    public class CheckingAccountNoOverdraftCost : CheckingAccount
    {
        
        [EntityEventHandler(typeof(AccountOverdrawnEvent))]
        protected Task<EntityEventResult> HandleAnAccountOverdrawn(IEntityEvent<Guid> arg)
        {
            //we ignore this, in reality we'd move money from savings, etc
            return Task.FromResult(EntityEventResult.Applied);
        }
    }
}