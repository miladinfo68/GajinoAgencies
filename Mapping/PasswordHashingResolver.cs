using AutoMapper;
using GajinoAgencies.Dtos;
using GajinoAgencies.Models;
using GajinoAgencies.Services;

namespace GajinoAgencies.Mapping;

public class PasswordHashingResolver : IValueResolver<AddAgentRequestDto, Agency, string>
{
    private readonly IPasswordManagerService _passwordManager;

    public PasswordHashingResolver(IPasswordManagerService passwordManager)
    {
        _passwordManager = passwordManager;
    }

    public string Resolve(AddAgentRequestDto source, Agency destination, string destMember, ResolutionContext context)
    {
        var (hashedPassword, salt) = _passwordManager.HashPassword(source.Password);
        
        destination.Salt = salt;

        return hashedPassword; 
    }
}