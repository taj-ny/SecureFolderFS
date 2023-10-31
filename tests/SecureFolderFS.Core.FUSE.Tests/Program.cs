using SecureFolderFS.Core.Cryptography.Enums;
using SecureFolderFS.Core.Enums;
using SecureFolderFS.Core.FileSystem;
using SecureFolderFS.Core.FileSystem.Enums;
using SecureFolderFS.Core.FUSE.AppModels;
using SecureFolderFS.Core.Models;
using SecureFolderFS.Sdk.AppModels;
using SecureFolderFS.Shared.Utils;
using SecureFolderFS.UI.AppModels;
using SecureFolderFS.UI.ServiceImplementation;
using SecureFolderFS.UI.Storage.NativeStorage;

// Testing FUSE requires external software. See docs/TESTING_FUSE.md for more information.

namespace SecureFolderFS.Core.Fuse.Tests
{
    internal static class Program
    {
        private static readonly string BaseVaultDirectory =
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), nameof(SecureFolderFS), "test_vaults"); 
        
        private static readonly string TestVaultPath = Path.Combine(BaseVaultDirectory, "sffs_test_vault_test");
        private static readonly string ScratchVaultPath = Path.Combine(BaseVaultDirectory, "sffs_test_vault_scratch");

        private static IPassword Password => new VaultPassword(" ");

        public static async Task Main()
        {
            Console.WriteLine("Creating temporary vaults...");
            Cleanup();
            await CreateVaultAsync(TestVaultPath);
            await CreateVaultAsync(ScratchVaultPath);

            Console.WriteLine("Unlocking and mounting vaults...");
            var testFileSystem = await UnlockVaultAsync(TestVaultPath);
            var scratchFileSystem = await UnlockVaultAsync(ScratchVaultPath);

            Console.WriteLine("Vaults have been mounted successfully. You can now run the testing suite.");
            Console.WriteLine("Press any key to lock the vaults.");
            Console.ReadKey();

            Console.WriteLine("Locking vaults...");
            await testFileSystem.CloseAsync(FileSystemCloseMethod.CloseForcefully);
            await scratchFileSystem.CloseAsync(FileSystemCloseMethod.CloseForcefully);

            Console.WriteLine("Cleaning up...");
            Cleanup();
        }

        private static void Cleanup()
        {
            if (Directory.Exists(BaseVaultDirectory))
                Directory.Delete(BaseVaultDirectory, true);
        }

        private static async Task<IVirtualFileSystem> UnlockVaultAsync(string path)
        {
            await using var configStream = File.OpenRead(Path.Combine(path, "sfconfig.cfg"));
            await using var keystoreStream = File.OpenRead(Path.Combine(path, "keystore.cfg"));

            using var routine = VaultHelpers.NewUnlockRoutine();
            await routine.SetVaultStoreAsync(new NativeFolder(path), new NativeStorageService());
            await routine.ReadConfigurationAsync(configStream, StreamSerializer.Instance);
            await routine.ReadKeystoreAsync(keystoreStream, StreamSerializer.Instance);
            routine.DeriveKeystore(Password);

            return await (await routine.PrepareAndUnlockAsync(new()
            {
                AdapterType = FileSystemAdapterType.FuseAdapter,
            })).MountAsync(new FuseMountOptions
            {
                AllowRootUserAccess = true,
                PrintDebugInformation = false
            });
        }

        private static async Task CreateVaultAsync(string path)
        {
            Directory.CreateDirectory(path);
            await using var configStream = File.Open(Path.Combine(path, "sfconfig.cfg"), FileMode.Create);
            await using var keystoreStream = File.Open(Path.Combine(path, "keystore.cfg"), FileMode.Create);

            using var routine = VaultHelpers.NewCreationRoutine();
            await routine.CreateContentFolderAsync(new NativeFolder(path));
            routine.SetVaultPassword(Password);
            await routine.WriteConfigurationAsync(new VaultOptions(ContentCipherScheme.XChaCha20_Poly1305, FileNameCipherScheme.AES_SIV), configStream, StreamSerializer.Instance);
            await routine.WriteKeystoreAsync(keystoreStream, StreamSerializer.Instance);
        }
    }
}