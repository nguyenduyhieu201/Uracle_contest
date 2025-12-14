using SharedKernel.Domains;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Uracle.Application.Abstractions.Interfaces;
using Uracle.Application.DTOs;
using Uracle.Domain.Models;
using Uracle.Infrastructure.Data;

namespace Uracle.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _context;
        public UserRepository(ApplicationDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task AddUserAsync(User user, CancellationToken cancellationToken)
        {
            await _context.Users.AddAsync(user, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task<User?> FindByIdAsync(string Id, CancellationToken cancellationToken)
        {
            return await _context.Users.FirstOrDefaultAsync(user => user.Id == Id);
        }

        public async Task<User> GetByEmailAsync(string email, CancellationToken cancellationToken)
        {
            var user = await _context.Users
                            .AsNoTracking()
                            .FirstOrDefaultAsync(u => u.Email == email, cancellationToken);
            return user;
        }

        public async Task<User> GetByUserNameAsync(string username, CancellationToken cancellationToken)
        {
            var user = await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Username == username, cancellationToken);
            return user;
        }
        
        public async Task SetRefreshTokenAsync(string userId, string refreshToken, CancellationToken cancellationToken)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);
            if (user != null)
            { 
                user.JwtRefreshToken = refreshToken;
                await _context.SaveChangesAsync(cancellationToken);
            }
        }

        public async Task<Result<User?>> UpdateResetTokenAsync(string userId, string resetToken, DateTime expiresAt, CancellationToken cancellationToken)
        {
            var user = await _context.Users
                        .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);

            if (user == null)
            {
                return Result<User?>.Fail("User not found");
            }

            user.SetResetToken(resetToken, expiresAt);
            await _context.SaveChangesAsync(cancellationToken);
            return Result<User?>.Success(user);
        }

        public async Task UpdateUserTokensAsync(string userId, StravaTokenResponse stravaResponse, CancellationToken cancellationToken)
        {
            var user = await _context.Users
                            .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);
            if (user != null)
            {
                user.AccessToken = stravaResponse.AccessToken;
                user.RefreshToken = stravaResponse.RefreshToken;
                user.ExpiresAt = stravaResponse.ExpiresAt;
                await _context.SaveChangesAsync();
            }
        }
    }
}
