﻿namespace GajinoAgencies.Utilities;

public class ForbiddenAccessException : Exception
{
    public ForbiddenAccessException(string message) : base(message)
    {
    }
}