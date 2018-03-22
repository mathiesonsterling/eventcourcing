using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CheckingAccountExample.Entities.Events;
using EventCoursing.Entities;
using EventCoursing.Services;
using EventCoursingSimple.Entities;

namespace CheckingAccountExample.Entities
{
    public class CheckingAccount : BaseEntity
    {
        public string AccountHolderName { get; set; }
        public decimal Balance { get; set; }
        public bool Overdrawn { get; set; }

        private const decimal OverdraftPenalty = 25.00m;
        
        public Task CreateAccount(string name, decimal openingBalance)
        {
            var ev = new AccountCreatedEvent
            {
                AccountHolderName = name,
                EmployeeOpening = "Bruce Springsteen",
                OpeningBalance = openingBalance
            };

            return EventPipeline.AddEvent<CheckingAccount>(ev);
        }

        public Task Deposit(decimal amount)
        {
            var ev = new DepositMadeEvent(Id)
            {
                Amount = amount
            };

            return EventPipeline.AddEvent<CheckingAccount>(ev);
        }

        public Task WriteCheck(string recipient, decimal amount, string memo)
        {
            return EventPipeline.AddEvent<CheckingAccount>(
                new CheckCashedEvent(Id)
                {
                    Amount = amount,
                    Recepient = recipient,
                    Date = DateTime.Now,
                    Memo = memo
                }
            );
        }
        protected override IDictionary<string, Func<IEntityEvent<Guid>, Task<EntityEventResult>>> SetupHandlers()
        {
            return new Dictionary<string, Func<IEntityEvent<Guid>, Task<EntityEventResult>>>
            {
                {AccountCreatedEvent.EVENT_NAME, HandleAccountCreated},
                {CheckCashedEvent.EVENT_NAME, HandleCheckCashed},
                {DepositMadeEvent.EVENT_NAME, HandleDepositMade},
                {AccountOverdrawnEvent.EVENT_NAME, HandleAccountOverdrawn}
            };
        }

        protected virtual Task<EntityEventResult> HandleAccountOverdrawn(IEntityEvent<Guid> arg)
        {
            var aoe = (AccountOverdrawnEvent)arg;
            Overdrawn = true;
            Balance -= OverdraftPenalty;

            return Task.FromResult(EntityEventResult.Applied);
        }

        private Task<EntityEventResult> HandleDepositMade(IEntityEvent<Guid> arg)
        {
            var dep = (DepositMadeEvent)arg;
            Balance += dep.Amount;
            
            return Task.FromResult(EntityEventResult.Applied);
        }

        private Task<EntityEventResult> HandleCheckCashed(IEntityEvent<Guid> arg)
        {
            var cce = (CheckCashedEvent)arg;
            Balance -= cce.Amount;

            if (Balance < 0)
            {
                //we overdrafted!  we put out another event to let the system handle this
                var overdraftEvent = new AccountOverdrawnEvent(Id);
                EventPipeline.AddEvent<CheckingAccount>(overdraftEvent);
            }
            
            return Task.FromResult(EntityEventResult.Applied);
        }

        private Task<EntityEventResult> HandleAccountCreated(IEntityEvent<Guid> ev)
        {
            var ac = (AccountCreatedEvent)ev;
            AccountHolderName = ac.AccountHolderName;
            Balance = ac.OpeningBalance;
            Id = ac.EntityId;
            return Task.FromResult(EntityEventResult.Applied);
        }
    }
}