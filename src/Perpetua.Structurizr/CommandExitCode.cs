namespace Perpetua.Structurizr;

/// <summary>The exit code the command returns to its caller. A failure carries the same name as the result that caused it.</summary>
internal enum CommandExitCode
{
    Success = 0,
    InvalidArguments = 1,
    OnlyOneContainerAllowed = 2,
    InvalidContainerName = 3,
    InvalidContextName = 4
}
