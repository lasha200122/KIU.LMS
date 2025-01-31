namespace KIU.LMS.Domain.Entities.SQL;

public class User : Aggregate
{
    public string FirstName { get; private set; } = null!;
    public string LastName { get; private set; } = null!;
    public string Email { get; private set; } = null!;
    public UserRole Role { get; private set; }
    public string PasswordHash { get; private set; } = null!;
    public string PasswordSalt { get; private set; } = null!;
    public bool EmailVerified { get; private set; }

    private List<UserCourse> _userCourses = new();
    public IReadOnlyCollection<UserCourse> UserCourses => _userCourses;

    private List<LoginAttempt> _loginAttempts = new();
    public IReadOnlyCollection<LoginAttempt> LoginAttempts => _loginAttempts;

    private List<UserDevice> _devices = new();
    public IReadOnlyCollection<UserDevice> Devices => _devices;
    public  List<Solution> Solutions { get; private set; } = null!;

    public User() { }

    public User(
        Guid id,
        string firstName,
        string lastName,
        string email,
        UserRole role,
        string passwordHash,
        string passwordSalt,
        Guid createUserId) : base(id, DateTimeOffset.Now, createUserId)
    {
        FirstName = firstName;
        LastName = lastName;
        Email = email;
        Role = role;
        PasswordHash = passwordHash;
        PasswordSalt = passwordSalt;
        EmailVerified = false;
        Validate(this);
    }

    public void ChangePassword(string passwordHash, byte[] passwordSalt, Guid updateUserId) 
    {
        PasswordHash = passwordHash;
        PasswordSalt = Convert.ToHexString(passwordSalt);

        Update(updateUserId, DateTimeOffset.UtcNow);
        Validate(this);
    }

    public void VerifyEmail() 
    {
        EmailVerified = true;
    }

    private void Validate(User user)
    {
        if (string.IsNullOrEmpty(user.FirstName))
            throw new Exception("სახელი სავალდებულოა");
        if (string.IsNullOrEmpty(user.LastName))
            throw new Exception("გვარი სავალდებულოა");
        if (string.IsNullOrEmpty(user.Email))
            throw new Exception("ელ-ფოსტა სავალდებულოა");
        if (string.IsNullOrEmpty(user.PasswordHash))
            throw new Exception("პაროლის ჰეში სავალდებულოა");
    }
}