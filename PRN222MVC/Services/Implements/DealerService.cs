using AutoMapper;
using BusinessObject.BusinessObject.DealerModels.Respond;
using Repositories.CustomRepositories.Interfaces;
using Repositories.Model;
using Repositories.WorkSeeds.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Implements
{
    public class DealerService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public DealerService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<GetDealerRespond> GetDealerById(int dealerId)
        {
            var dealerRepo = _unitOfWork.GetCustomRepository<IDealerRepository>();
            var dealer = await dealerRepo.GetDealerByIdAsync(dealerId);
            if (dealer == null)
            {
                throw new Exception($"Dealer with ID {dealerId} not found.");
            }
            var respond = _mapper.Map<GetDealerRespond>(dealer);

            return respond;
        }
    }
}
