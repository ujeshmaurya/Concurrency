namespace PoliticalPartyWashRoom;

class PoliticalPartyWashRoom {
    /// DS for signifying the toilet stalls being used: Shared resource.
    private static Queue<string> toiletStalls = new Queue<string>();

    /// Lock for accessing the Shared resource.
    private static object _lock = new object();

    /// Entry point of dotnet program: Use 'dotnet run'
    static void Main(string[] args) {
        // Configure the various conditions here:

        // Spawn threads here:
    }    
}
