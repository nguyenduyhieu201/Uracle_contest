using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Uracle.Application.DTOs.ContestDto;

namespace Uracle.Application.Commands.ContestsCommand
{
    public record ContestCreateCommand (ContestCreateRequestDto contestCreateDto) : ICommand<Result<ContestCreateResponseDto>>;

    public class ContestCreateCommandHandler : ICommandHandler<ContestCreateCommand, Result<ContestCreateResponseDto>>
    {
        public IJWTService _jWTService;
        public ContestCreateCommandHandler(IJWTService jWTService)
        {
            _jWTService = jWTService;
        }
        public async Task<Result<ContestCreateResponseDto>> Handle(ContestCreateCommand request, CancellationToken cancellationToken)
        {
            var userId = await _jWTService.ValidateUserAsync(request.contestCreateDto.token);
            if (userId.IsFail) 
            {
                return Result<ContestCreateResponseDto>.Fail("Invalid token");
            }
            
            throw new NotImplementedException();
        }
    }
}
