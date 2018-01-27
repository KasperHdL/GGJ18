public class GameEventArgs{
}

public class ExampleArgs : GameEventArgs{
    public int someInt;
}

public class RumbleArgs : GameEventArgs{
    public InputValues vibrationType;
    public int teamID;
}

public class PatternArgs : GameEventArgs{
    public int teamID;
}