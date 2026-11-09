namespace ApiContracts;

// Sent from the Blazor app to POST /auth/login
public record LoginRequest(string UserName, string Password);
