using SecureFolderFS.Core.Routines.UnlockRoutines;
using SecureFolderFS.Sdk.AppModels;
using SecureFolderFS.UI.AppModels;

Console.Write("Vault directory path: ");
var vaultDirectory = Console.ReadLine()!;

Console.Write("Passwords text file path (one password per line): ");
var passwordsFile = Console.ReadLine()!;

var unlockRoutine = new UnlockRoutine();
using var keystoreStream = new FileStream(Path.Combine(vaultDirectory, "keystore.cfg"), FileMode.Open, FileAccess.Read);
await unlockRoutine.ReadKeystoreAsync(keystoreStream, StreamSerializer.Instance);

var cts = new CancellationTokenSource();
var po = new ParallelOptions();
po.CancellationToken = cts.Token;
po.MaxDegreeOfParallelism = Environment.ProcessorCount;
Parallel.ForEach(File.ReadAllLines(passwordsFile), po, password =>
{
    Console.WriteLine($"Trying password \"{password}\"...");
   
    try
    {
        unlockRoutine.DeriveKeystore(new VaultPassword(password));
        Console.WriteLine($"\nFound password! \"{password}\"");
        cts.Cancel();
    }
    catch {}
});

Console.ReadKey();
Environment.Exit(0);