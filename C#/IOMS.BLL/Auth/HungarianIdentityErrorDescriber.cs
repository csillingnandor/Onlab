using Microsoft.AspNetCore.Identity;

namespace IOMS.BLL.Auth;

// Az Identity hibaüzenetei magyarul (a felhasználókezelés validációs válaszaiba kerülnek)
internal class HungarianIdentityErrorDescriber : IdentityErrorDescriber
{
    public override IdentityError DuplicateUserName(string userName) =>
        new() { Code = nameof(DuplicateUserName), Description = $"Már létezik felhasználó ezzel a névvel: {userName}" };

    public override IdentityError InvalidUserName(string? userName) =>
        new() { Code = nameof(InvalidUserName), Description = "A felhasználónév csak betűket, számjegyeket és . _ - @ jeleket tartalmazhat." };

    public override IdentityError DuplicateRoleName(string role) =>
        new() { Code = nameof(DuplicateRoleName), Description = $"Már létezik szerepkör ezzel a névvel: {role}" };

    public override IdentityError PasswordTooShort(int length) =>
        new() { Code = nameof(PasswordTooShort), Description = $"A jelszónak legalább {length} karakter hosszúnak kell lennie." };

    public override IdentityError PasswordRequiresDigit() =>
        new() { Code = nameof(PasswordRequiresDigit), Description = "A jelszónak tartalmaznia kell számjegyet." };

    public override IdentityError PasswordRequiresLower() =>
        new() { Code = nameof(PasswordRequiresLower), Description = "A jelszónak tartalmaznia kell kisbetűt." };

    public override IdentityError PasswordRequiresUpper() =>
        new() { Code = nameof(PasswordRequiresUpper), Description = "A jelszónak tartalmaznia kell nagybetűt." };

    public override IdentityError PasswordRequiresNonAlphanumeric() =>
        new() { Code = nameof(PasswordRequiresNonAlphanumeric), Description = "A jelszónak tartalmaznia kell speciális karaktert." };

    public override IdentityError PasswordRequiresUniqueChars(int uniqueChars) =>
        new() { Code = nameof(PasswordRequiresUniqueChars), Description = $"A jelszónak legalább {uniqueChars} különböző karaktert kell tartalmaznia." };
}
