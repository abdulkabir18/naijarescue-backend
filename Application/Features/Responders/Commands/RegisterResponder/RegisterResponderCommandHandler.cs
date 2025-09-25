using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Application.Common.Dtos;

namespace Application.Features.Responders.Commands.RegisterResponder
{
    public class RegisterResponderCommandHandler : IRequestHandler<RegisterResponderCommand, Result<Guid>>
    {
        public RegisterResponderCommandHandler()
        {

        }

        public async Task<Result<Guid>> Handle(RegisterResponderCommand request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}