global using Microsoft.EntityFrameworkCore;
global using Microsoft.EntityFrameworkCore.Diagnostics;
global using Microsoft.Extensions.Configuration;
global using Microsoft.Extensions.DependencyInjection;
global using Uracle.API.Options;
global using Uracle.Application.Abstractions.Data;
global using Uracle.Application.Abstractions.Interfaces;
global using Uracle.Application.Abstractions.Security;
global using Uracle.Domain.Models;
global using Uracle.Infrastructure.Data;
global using Uracle.Infrastructure.Repositories;
global using Uracle.Infrastructure.Security;
global using Uracle.Domain.Abstractions;
global using Uracle.Domain.Models.GroupMembers;
global using Microsoft.EntityFrameworkCore.ChangeTracking;
global using Microsoft.AspNetCore.Builder;
global using Uracle.Application.DTOs;
global using SharedKernel.Domains;
global using Microsoft.Extensions.Caching.Distributed;
global using Microsoft.Extensions.Options;
global using Microsoft.IdentityModel.Tokens;

global using System.IdentityModel.Tokens.Jwt;
global using System.Security.Claims;
global using System.Security.Cryptography;
global using System.Text;
global using Uracle.Infrastructure.Services;
global using Uracle.Application.DTOs.IndividualContestActivityDto;
global using Uracle.Domain.Models.Contests;
global using Uracle.Domain.Models.TeamMember;
global using Uracle.Application.DTOs.StravasDto;
global using Uracle.Application.Abstractions.Services;
global using Microsoft.AspNetCore.WebUtilities;

global using System.Text.Json;

global using Uracle.Infrastructure.Options;
global using static System.Net.WebRequestMethods;
