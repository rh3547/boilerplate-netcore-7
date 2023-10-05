using Nukleus.Application.Common.Services;
using Nukleus.Application.AccountModule;
using MapsterMapper;
using Nukleus.Infrastructure.Common.Persistence;
using Nukleus.Domain.Entities;
using Nukleus.Application.Common.Validation;
using Nukleus.Application.UserModule;

namespace Nukleus.Infrastructure.AccountModule
{
    public class AccountService : EFService<Account>, IAccountService
    {
        private readonly INukleusLogger _logger;
        private readonly IAccountRepository _accountsRepository;
        private readonly IUserRepository _usersRepository;
        private readonly IMapper _mapper;
        private readonly IHashingService _hashingService;
        private readonly ISession _session;

        public AccountService(INukleusLogger logger, IAccountRepository accountsRepository, IUserRepository usersRepository, IMapper mapper, IHashingService hashingService, ISession session) : base(logger, accountsRepository, mapper)
        {
            _logger = logger;
            _accountsRepository = accountsRepository;
            _usersRepository = usersRepository;
            _mapper = mapper;
            _hashingService = hashingService;
            _session = session;

            User? SessionUser = _session.GetUser();

            if (SessionUser != null)
            {
                SetReadConstraints(x => x.Id == SessionUser.AccountId);
                SetWriteConstraints(x => x.Id == SessionUser.AccountId && x.OwnerUserId == SessionUser.Id);
            }
        }

        public async Task<Result<NewAccountResponseDTO>> NewAccount(NewAccountDTO dto)
        {
            // Do the mappings
            Account accountToAdd = new Account() { Id = Guid.NewGuid(), Name = dto.AccountName };
            User userToAdd = _mapper.Map<User>(dto);

            await _accountsRepository.BeginTransactionAsync();

            // Hash the provided password
            userToAdd.Password = _hashingService.Hash(userToAdd.Password);

            // Check if an account with the provided account name exists. If so, abort.
            IQueryable<Account> accountExistsQuery = _accountsRepository.Query().Where(account => (account.Name == accountToAdd.Name));
            Result<Account?> accountExistsOperation = await _accountsRepository.GetFirstOrDefaultAsync(accountExistsQuery);
            if (accountExistsOperation.IsFaulted)
            {
                return accountExistsOperation.Error;
            }

            // If not null, then an account already exists. Abort.
            if (accountExistsOperation.Value is not null)
            {
                return Error.ValidationError("The provided username, email, or account already exists.");
            }

            // Check if a user with that info already exists. If so, abort.
            IQueryable<User> userExistsQuery = _usersRepository.Query().Where(user => (user.Username == userToAdd.Username) || (user.Email == userToAdd.Email));
            Result<User?> userExistsOperation = await _usersRepository.GetFirstOrDefaultAsync(userExistsQuery);
            if (userExistsOperation.IsFaulted)
            {
                return userExistsOperation.Error;
            }

            // If not null, then a user already exists. Abort.
            if(userExistsOperation.Value is not null)
            {
                return Error.ValidationError("The provided username, email, or account already exists.");
            }
            
            // Since we passed checks for if the account/user exists, Add the User.
            Result<User> userAddResult = await _usersRepository.AddAsync(userToAdd);
            if (userAddResult.IsFaulted)
            {
                await _accountsRepository.RollbackTransactionAsync();
                return userAddResult.Error;
            }
            await _accountsRepository.SaveChangesAsync();

            // Assign the Owner of the Account to the User we just created.
            accountToAdd.OwnerUser = userAddResult.Value;
            Result<Account> accountAddResult = await _accountsRepository.AddAsync(accountToAdd);
            if (accountAddResult.IsFaulted)
            {
                await _accountsRepository.RollbackTransactionAsync();
                return accountAddResult.Error;
            }
            await _accountsRepository.SaveChangesAsync();

            // Update the user with the newly created account FK
            userAddResult.Value.Account = accountAddResult.Value;
            Result<User> userUpdateResult = _usersRepository.Update(userAddResult.Value);
            if (userUpdateResult.IsFaulted)
            {
                await _accountsRepository.RollbackTransactionAsync();
                return userAddResult.Error;
            }
            await _accountsRepository.SaveChangesAsync();

            // Commit the Transaction
            Result<bool> transactionResult = await _accountsRepository.CommitTransactionAsync();
            if (transactionResult.IsFaulted)
            {
                await _accountsRepository.RollbackTransactionAsync();
                return transactionResult.Error;
            }
            
            AccountSingleDTO accountResponse = _mapper.Map<AccountSingleDTO>(accountAddResult.Value);
            UserSingleDTO userResponse = _mapper.Map<UserSingleDTO>(userUpdateResult.Value);

            
            return new NewAccountResponseDTO(){User = userResponse, Account = accountResponse};
        }
    }
}