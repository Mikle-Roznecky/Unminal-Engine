public class Crash: Exception {
    public Crash(): base(){}
    public Crash(string message): base(message){}
    public Crash(string message, Exception exception): base(message, exception){}
}