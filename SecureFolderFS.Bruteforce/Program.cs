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

Parallel.ForEach(File.ReadAllLines(passwordsFile), password =>
{
    Console.WriteLine($"Trying password \"{password}\"...");
   
    try
    {
        unlockRoutine.DeriveKeystore(new VaultPassword(password));
        Console.WriteLine($"\nFound password! \"{password}\"");
        Console.ReadKey();
        Environment.Exit(0);
    }
    catch {}
});