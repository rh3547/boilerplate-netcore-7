using Nukleus.Application.Common.Services;
using Nukleus.Application.Common.Validation;
using Nukleus.Application.UserModule;
using Nukleus.Domain.Entities;
using MapsterMapper;
using Nukleus.Infrastructure.Common.Persistence;

namespace Nukleus.Infrastructure.UserModule
{
    public class UserService : EFService<User>, IUserService
    {
        private readonly INukleusLogger _logger;
        private readonly IUserRepository _repository;
        private readonly IMapper _mapper;
        private readonly IHashingService _hashingService;
        private readonly ISession _session;

        public UserService(INukleusLogger logger, IUserRepository repository, IMapper mapper, IHashingService hashingService, ISession session) : base(logger, repository, mapper)
        {
            _logger = logger;
            _repository = repository;
            _mapper = mapper;
            _hashingService = hashingService;
            _session = session;

            User? SessionUser = _session.GetUser();
            Account? SessionAccount = _session.GetAccount();

            if (SessionUser != null && SessionAccount != null)
            {
                SetConstraints(x => x.Id == SessionUser.Id || (SessionAccount.OwnerUserId == SessionUser.Id && x.AccountId == SessionAccount.Id));
            }
            else if (SessionUser != null)
            {
                SetConstraints(x => x.Id == SessionUser.Id);
            }
        }

        public override async Task<Result<UserSingleDTO>> AddAsync<UserSingleDTO, UserAddDTO>(UserAddDTO addDTO)
        {
            User entityToAdd = _mapper.Map<User>(addDTO);

            // Hash the provided password
            entityToAdd.Password = _hashingService.Hash(entityToAdd.Password);

            if (AddConstraint != null && !AddConstraint.Compile().Invoke(entityToAdd))
            {
                return Error.Unauthorized($"You do not have access to add that resource.");
            }

            Result<User> operation = await _repository.AddAsync(entityToAdd);

            if (operation.IsFaulted)
            {
                return operation.Error;
            }

            Result<int> saveOperation = await _repository.SaveChangesAsync();

            if (saveOperation.IsFaulted)
            {
                return saveOperation.Error;
            }

            return _mapper.Map<UserSingleDTO>(operation.Value);
        }

        public async Task<Result<bool>> DoesEmailExist(string email)
        {
            // Don't know if it succeeded or not. But since we don't need to do anything else at this layer
            // for instance, short-circuit in the event we needed to reach out to multiple repositories,
            // we can let higher layers handle it and let it 'bubble-up'
            return await _repository.ExistsAsync(u => u.Email == email);
        }

        public async Task<Result<bool>> DoesUsernameExist(string username)
        {
            return await _repository.ExistsAsync(u => u.Username == username);
        }
    }
}