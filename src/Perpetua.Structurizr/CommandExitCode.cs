namespace Perpetua.Structurizr;

/// <summary>
/// The exit code the command returns to its caller. A failure covers one or several problems;
/// the messages on the error output say which.
/// </summary>
internal enum CommandExitCode
{
    Success = 0,
    Failure = 1
}
